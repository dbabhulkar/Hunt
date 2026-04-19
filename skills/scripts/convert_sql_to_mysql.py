"""
Convert SQL Server (T-SQL) Hunt.sql dump to MySQL-compatible Hunt_mysql.sql.

Scope: handles the constructs used in this project's Hunt.sql SSMS-generated
dump. Not a general-purpose T-SQL → MySQL translator.
"""
from __future__ import annotations

import re
import sys
from pathlib import Path

MYSQL_RESERVED = {
    "from", "to", "status", "key", "order", "group", "read",
    "write", "interval", "range", "rank", "database", "schema", "condition",
    "action", "option", "force", "usage", "current_date", "current_time",
    "value", "values", "by", "for", "where", "select", "update",
    "delete", "insert", "into", "set", "call", "return", "case", "when",
    "then", "else", "end", "if", "declare", "and", "or", "not",
    "in", "like", "is", "between", "exists", "limit",
}

TYPE_MAP = [
    (r"\bnvarchar\s*\(\s*max\s*\)", "LONGTEXT"),
    (r"\bvarchar\s*\(\s*max\s*\)", "LONGTEXT"),
    (r"\bntext\b", "LONGTEXT"),
    (r"\btext\b", "LONGTEXT"),
    (r"\bnvarchar\b", "VARCHAR"),
    (r"\bnchar\b", "CHAR"),
    (r"\bbit\b", "TINYINT(1)"),
    (r"\bmoney\b", "DECIMAL(19,4)"),
    (r"\bsmallmoney\b", "DECIMAL(10,4)"),
    (r"\buniqueidentifier\b", "CHAR(36)"),
    (r"\bdatetime2\s*\(\s*\d+\s*\)", "DATETIME"),
    (r"\bdatetime2\b", "DATETIME"),
    (r"\bsmalldatetime\b", "DATETIME"),
    (r"\bimage\b", "LONGBLOB"),
    (r"\bvarbinary\s*\(\s*max\s*\)", "LONGBLOB"),
    (r"\bnumeric\b", "DECIMAL"),
    (r"\bxml\b", "LONGTEXT"),
]

STMT_STARTERS_RE = re.compile(
    r"^\s*(SELECT|INSERT|UPDATE|DELETE|TRUNCATE|SET|DECLARE|IF|WHILE|DROP|CREATE|CALL|RETURN|LEAVE|COMMIT|ROLLBACK|PREPARE|EXECUTE|DEALLOCATE)\b",
    re.IGNORECASE,
)

NO_SEMICOLON_END_RE = re.compile(
    r"(?:;|,|\(|BEGIN|THEN|ELSE|DO|END\s*IF|UNION\s+ALL|UNION)\s*$",
    re.IGNORECASE,
)


# ---------- helpers ----------

def strip_square_brackets(s: str) -> str:
    """Replace [Name With Spaces] with backticked form or bare name."""
    def repl(m: re.Match) -> str:
        inner = m.group(1)
        if re.match(r"^[A-Za-z_][A-Za-z0-9_]*$", inner) and inner.lower() not in MYSQL_RESERVED:
            return inner
        return f"`{inner}`"
    return re.sub(r"\[([^\[\]]+)\]", repl, s)


def backtick_reserved(name: str) -> str:
    if " " in name or name.lower() in MYSQL_RESERVED:
        return f"`{name}`"
    return name


def convert_type(s: str) -> str:
    for pat, repl in TYPE_MAP:
        s = re.sub(pat, repl, s, flags=re.IGNORECASE)
    return s


def split_batches(sql: str) -> list[str]:
    lines = sql.splitlines()
    batches: list[list[str]] = [[]]
    for line in lines:
        if re.match(r"^\s*GO\s*(--.*)?$", line, re.IGNORECASE):
            batches.append([])
        else:
            batches[-1].append(line)
    return ["\n".join(b).strip() for b in batches if b]


SKIP_PREFIXES = (
    "USE ", "CREATE DATABASE", "ALTER DATABASE", "SET ANSI_", "SET QUOTED_IDENTIFIER",
    "SET ARITHABORT", "SET CONCAT_NULL_YIELDS", "SET NUMERIC_ROUNDABORT",
    "IF (1 = FULLTEXTSERVICEPROPERTY", "EXEC ", "/****** ",
)


def should_skip_batch(batch: str) -> bool:
    stripped = batch.lstrip()
    stripped = re.sub(r"^/\*.*?\*/\s*", "", stripped, flags=re.DOTALL)
    stripped = re.sub(r"^--[^\n]*\n", "", stripped)
    stripped = stripped.lstrip()
    if not stripped:
        return True
    upper = stripped.upper()
    for p in SKIP_PREFIXES:
        if upper.startswith(p.upper()):
            return True
    return False


# ---------- TABLE conversion ----------

def convert_table(batch: str) -> str:
    batch = re.sub(r"/\*+.*?\*+/", "", batch, flags=re.DOTALL)
    batch = batch.replace("[dbo].", "")
    m = re.match(r"\s*CREATE TABLE\s+\[?([A-Za-z0-9_]+)\]?\s*\(", batch, re.IGNORECASE)
    if not m:
        return batch
    tbl_name = m.group(1)

    start = batch.index("(", m.end() - 1)
    depth = 0
    end = -1
    for i in range(start, len(batch)):
        c = batch[i]
        if c == "(":
            depth += 1
        elif c == ")":
            depth -= 1
            if depth == 0:
                end = i
                break
    body = batch[start + 1 : end]

    # Split body into logical column/constraint lines — cannot simply splitlines
    # because column definitions may span one line each. But we also need to
    # keep parenthesized default/check clauses together.
    lines = body.splitlines()
    out_lines: list[str] = []
    pk_cols: list[str] = []
    has_ai = False
    ai_col: str | None = None

    i = 0
    while i < len(lines):
        ln = lines[i]
        stripped = ln.strip().rstrip(",")
        if not stripped:
            i += 1
            continue
        upper = stripped.upper()

        # PK constraint block
        if upper.startswith("CONSTRAINT ") and "PRIMARY KEY" in upper:
            combined = stripped
            while "(" not in combined and i + 1 < len(lines):
                i += 1
                combined += " " + lines[i].strip()
            while combined.count("(") > combined.count(")") and i + 1 < len(lines):
                i += 1
                combined += " " + lines[i].strip()
            pk_match = re.search(r"PRIMARY KEY\s+(?:CLUSTERED|NONCLUSTERED)?\s*\(([^)]*)\)", combined, re.IGNORECASE)
            if pk_match:
                raw_cols = pk_match.group(1)
                cols = [c.strip() for c in raw_cols.split(",")]
                cols = [re.sub(r"\s+(ASC|DESC)\b", "", c, flags=re.IGNORECASE).strip() for c in cols]
                cols = [strip_square_brackets(c) for c in cols]
                cols = [backtick_reserved(c) if not c.startswith("`") else c for c in cols]
                pk_cols = cols
            # skip WITH / ON trailing lines
            while i + 1 < len(lines):
                nxt = lines[i + 1].strip()
                if nxt.upper().startswith(("WITH ", "ON ", ")WITH", "ON [")):
                    i += 1
                    continue
                break
            i += 1
            continue

        if upper.startswith("UNIQUE ") or upper.startswith("CONSTRAINT "):
            while i + 1 < len(lines) and not re.match(r"^\s*(\[|CONSTRAINT|\))", lines[i + 1], re.IGNORECASE):
                i += 1
            i += 1
            continue

        if upper.startswith(("WITH ", "ON [PRIMARY", "TEXTIMAGE_ON")):
            i += 1
            continue

        # Column line: [Col Name] [type](...) ...
        col_match = re.match(r"^\s*\[([^\]]+)\]\s+(.*?)(,?)\s*$", ln)
        if not col_match:
            col_match = re.match(r"^\s*([A-Za-z_][A-Za-z0-9_]*)\s+(.*?)(,?)\s*$", ln)
        if col_match:
            col_name = col_match.group(1)
            rest = col_match.group(2)
            rest = strip_square_brackets(rest)
            rest = convert_type(rest)
            if re.search(r"IDENTITY\s*\(", rest, re.IGNORECASE):
                has_ai = True
                ai_col = col_name
                # MySQL AUTO_INCREMENT only on integer types — force BIGINT
                rest = re.sub(r"\bDECIMAL\s*\([^)]*\)", "BIGINT", rest, flags=re.IGNORECASE)
                rest = re.sub(r"\bDECIMAL\b", "BIGINT", rest, flags=re.IGNORECASE)
            rest = re.sub(r"IDENTITY\s*\(\s*\d+\s*,\s*\d+\s*\)", "AUTO_INCREMENT", rest, flags=re.IGNORECASE)
            rest = re.sub(r"\bNOT FOR REPLICATION\b", "", rest, flags=re.IGNORECASE)
            rest = re.sub(r"\bROWGUIDCOL\b", "", rest, flags=re.IGNORECASE)
            rest = re.sub(r"DEFAULT\s*\(\s*getdate\s*\(\s*\)\s*\)", "DEFAULT CURRENT_TIMESTAMP", rest, flags=re.IGNORECASE)
            rest = re.sub(r"DEFAULT\s*\(\s*NULL\s*\)", "DEFAULT NULL", rest, flags=re.IGNORECASE)
            rest = re.sub(r"DEFAULT\s*\(\s*\(\s*(-?\d+)\s*\)\s*\)", r"DEFAULT \1", rest)
            rest = re.sub(r"DEFAULT\s*\(\s*(-?\d+)\s*\)", r"DEFAULT \1", rest)
            rest = re.sub(r"DEFAULT\s*\(\s*N?'([^']*)'\s*\)", r"DEFAULT '\1'", rest)
            # Too-large VARCHAR → TEXT
            def varchar_cap(m: re.Match) -> str:
                n = int(m.group(1))
                if n > 3000:
                    return "LONGTEXT"
                return m.group(0)
            rest = re.sub(r"\bvarchar\s*\(\s*(\d+)\s*\)", varchar_cap, rest, flags=re.IGNORECASE)
            rest = re.sub(r"\s+", " ", rest).strip().rstrip(",")
            quoted_name = backtick_reserved(col_name) if " " not in col_name else f"`{col_name}`"
            out_lines.append(f"  {quoted_name} {rest},")
            i += 1
            continue

        i += 1

    if out_lines:
        out_lines[-1] = out_lines[-1].rstrip(",")

    # If we have an AUTO_INCREMENT column but no PK from constraint, make it the PK
    if has_ai and not pk_cols and ai_col:
        pk_cols = [backtick_reserved(ai_col) if " " not in ai_col else f"`{ai_col}`"]

    if pk_cols:
        out_lines[-1] = out_lines[-1] + ","
        out_lines.append(f"  PRIMARY KEY ({', '.join(pk_cols)})")

    result = f"CREATE TABLE IF NOT EXISTS {tbl_name} (\n"
    result += "\n".join(out_lines)
    result += "\n) ENGINE=InnoDB ROW_FORMAT=DYNAMIC DEFAULT CHARSET=utf8mb4;"
    return result


# ---------- PROCEDURE conversion ----------

def find_procedure_as(batch: str, name_end: int) -> int | None:
    """Find position of the procedure's AS keyword. AS must be at a statement
    boundary (start of line or preceded by newline/whitespace and followed by
    newline/whitespace), AFTER any parenthesized param block."""
    # If there's a `(` immediately (modulo whitespace) after the name, skip
    # the balanced param block first.
    i = name_end
    while i < len(batch) and batch[i].isspace():
        i += 1
    if i < len(batch) and batch[i] == "(":
        depth = 1
        i += 1
        while i < len(batch) and depth > 0:
            if batch[i] == "(":
                depth += 1
            elif batch[i] == ")":
                depth -= 1
            i += 1

    # Now look for AS at statement boundary starting from i
    m = re.search(r"(?:^|\n)\s*AS\b", batch[i:], re.IGNORECASE)
    if m:
        return i + m.end()
    return None


def parse_params(params_src: str) -> list[tuple[str, str]]:
    params_src = params_src.strip()
    if params_src.startswith("("):
        depth = 0
        end = 0
        for i, c in enumerate(params_src):
            if c == "(":
                depth += 1
            elif c == ")":
                depth -= 1
                if depth == 0:
                    end = i
                    break
        params_src = params_src[1:end]

    # DO NOT strip brackets globally here (type names may have them)
    parts: list[str] = []
    depth = 0
    cur = ""
    for c in params_src:
        if c == "(":
            depth += 1
        elif c == ")":
            depth -= 1
        if c == "," and depth == 0:
            parts.append(cur)
            cur = ""
        else:
            cur += c
    if cur.strip():
        parts.append(cur)

    out: list[tuple[str, str]] = []
    for p in parts:
        p = p.strip().rstrip(",")
        if not p or not p.startswith("@"):
            continue
        # Remove optional leading 'as' between name and type: "@x as varchar(10)"
        m = re.match(
            r"@([A-Za-z0-9_]+)\s+(?:AS\s+)?(.+?)(?:\s*=\s*.+?)?(?:\s+OUTPUT)?\s*$",
            p,
            re.IGNORECASE,
        )
        if not m:
            continue
        name = m.group(1)
        typ = m.group(2).strip()
        typ = strip_square_brackets(typ)
        typ = convert_type(typ)
        if typ.lower() == "varchar":
            typ = "varchar(255)"
        out.append((name, typ))
    return out


def extract_declares(body: str) -> tuple[list[tuple[str, str]], str]:
    decls: list[tuple[str, str]] = []

    def handle(raw: str) -> None:
        parts: list[str] = []
        depth = 0
        cur = ""
        for c in raw:
            if c == "(":
                depth += 1
            elif c == ")":
                depth -= 1
            if c == "," and depth == 0:
                parts.append(cur)
                cur = ""
            else:
                cur += c
        if cur.strip():
            parts.append(cur)
        for p in parts:
            p = p.strip()
            dm = re.match(r"@([A-Za-z0-9_]+)\s+([^=]+?)(?:\s*=\s*(.+))?$", p)
            if dm:
                name = dm.group(1)
                typ = convert_type(dm.group(2).strip())
                default = dm.group(3)
                if default is not None:
                    default = default.strip()
                    if default.upper() == "NULL":
                        decls.append((name, f"{typ} DEFAULT NULL"))
                    else:
                        decls.append((name, f"{typ} DEFAULT {default}"))
                else:
                    decls.append((name, typ))

    # Match DECLARE @x type[, @y type ...]  stopping at newline or ;
    pattern = re.compile(r"\bDECLARE\s+(@[^\n;]+)", re.IGNORECASE)
    def repl(m: re.Match) -> str:
        handle(m.group(1))
        return ""
    new_body = pattern.sub(repl, body)
    return decls, new_body


def protect_strings(s: str) -> tuple[str, list[str]]:
    literals: list[str] = []

    def repl(m: re.Match) -> str:
        literals.append(m.group(0))
        return f"\x00STR{len(literals) - 1}\x00"

    out = re.sub(r"N?'(?:[^']|'')*'", repl, s)
    return out, literals


def restore_strings(s: str, literals: list[str]) -> str:
    def repl(m: re.Match) -> str:
        idx = int(m.group(1))
        lit = literals[idx]
        if lit.startswith("N'"):
            lit = lit[1:]
        return lit
    return re.sub(r"\x00STR(\d+)\x00", repl, s)


def protect_comments(s: str) -> tuple[str, list[str]]:
    comments: list[str] = []
    def block(m: re.Match) -> str:
        comments.append(m.group(0))
        return f"\x00CMT{len(comments) - 1}\x00"
    s = re.sub(r"/\*.*?\*/", block, s, flags=re.DOTALL)
    s = re.sub(r"--[^\n]*", block, s)
    return s, comments


def restore_comments(s: str, comments: list[str]) -> str:
    def repl(m: re.Match) -> str:
        return comments[int(m.group(1))]
    return re.sub(r"\x00CMT(\d+)\x00", repl, s)


def convert_concat_plus(s: str) -> str:
    """Convert `+` string concat → CONCAT() on protected string. Runs on text
    where strings have been replaced by \x00STR#\x00 placeholders."""
    out: list[str] = []
    i = 0
    n = len(s)

    def read_operand(idx: int) -> tuple[str, int] | None:
        while idx < n and s[idx] in " \t":
            idx += 1
        if idx >= n:
            return None
        start = idx
        if s[idx] == "\x00":
            m = re.match(r"\x00(?:STR|CMT)\d+\x00", s[idx:])
            if not m:
                return None
            idx += m.end()
            return s[start:idx], idx
        if s[idx] == "(":
            depth = 1
            idx += 1
            while idx < n and depth > 0:
                if s[idx] == "(":
                    depth += 1
                elif s[idx] == ")":
                    depth -= 1
                idx += 1
            return s[start:idx], idx
        if s[idx] == "@" or s[idx].isalpha() or s[idx] == "_":
            m = re.match(r"@?[A-Za-z_][A-Za-z0-9_]*(?:\.[A-Za-z_][A-Za-z0-9_]*)*", s[idx:])
            if not m:
                return None
            idx += m.end()
            if idx < n and s[idx] == "(":
                depth = 1
                idx += 1
                while idx < n and depth > 0:
                    if s[idx] == "(":
                        depth += 1
                    elif s[idx] == ")":
                        depth -= 1
                    idx += 1
            return s[start:idx], idx
        return None

    while i < n:
        ch = s[i]
        if ch == "\x00" or ch == "@" or ch.isalpha() or ch == "_":
            first = read_operand(i)
            if first is None:
                out.append(ch)
                i += 1
                continue
            op1, idx1 = first
            operands = [op1]
            j = idx1
            did = False
            while True:
                k = j
                while k < n and s[k] in " \t":
                    k += 1
                if k >= n or s[k] != "+":
                    break
                nxt = read_operand(k + 1)
                if nxt is None:
                    break
                any_str = any("\x00STR" in o for o in operands) or "\x00STR" in nxt[0]
                if not any_str:
                    break
                operands.append(nxt[0])
                j = nxt[1]
                did = True
            if did:
                out.append("CONCAT(" + ", ".join(o.strip() for o in operands) + ")")
                i = j
                continue
            else:
                out.append(s[i:idx1])
                i = idx1
                continue
        else:
            out.append(ch)
            i += 1
    return "".join(out)


def convert_if_else_blocks(body: str) -> str:
    """Convert T-SQL IF/ELSE IF/ELSE with BEGIN..END blocks to MySQL IF..THEN..
    ELSEIF..ELSE..END IF."""

    n = len(body)

    def match_kw(s: str, idx: int, kw: str) -> bool:
        if idx + len(kw) > len(s):
            return False
        if s[idx : idx + len(kw)].lower() != kw.lower():
            return False
        if idx > 0 and (s[idx - 1].isalnum() or s[idx - 1] == "_"):
            return False
        after = idx + len(kw)
        if after < len(s) and (s[after].isalnum() or s[after] == "_"):
            return False
        return True

    def find_matching_end(s: str, idx: int) -> int:
        """Find the matching END for a BEGIN, tracking nested BEGIN/CASE.
        Both BEGIN and CASE are closed by END in T-SQL."""
        stack = ["begin"]
        j = idx
        N = len(s)
        while j < N:
            if s[j] == "'":
                j += 1
                while j < N and s[j] != "'":
                    if s[j] == "\\":
                        j += 1
                    j += 1
                j += 1
                continue
            if s[j] == "\x00":
                # skip placeholder
                m = re.match(r"\x00(?:STR|CMT)\d+\x00", s[j:])
                if m:
                    j += m.end()
                    continue
            if match_kw(s, j, "begin"):
                stack.append("begin")
                j += 5
                continue
            if match_kw(s, j, "case"):
                stack.append("case")
                j += 4
                continue
            if match_kw(s, j, "end"):
                if stack:
                    stack.pop()
                    if not stack:
                        return j
                j += 3
                continue
            j += 1
        return -1

    def read_paren_or_plain_cond(s: str, idx: int) -> tuple[str, int] | None:
        """Read condition after IF/ELSEIF. Stops when it finds BEGIN at depth 0.
        If a statement-starter keyword appears before BEGIN, bails out — that's
        a single-statement IF we don't handle."""
        N = len(s)
        while idx < N and s[idx].isspace():
            idx += 1
        if idx >= N:
            return None
        start = idx
        depth = 0
        while idx < N:
            c = s[idx]
            if c == "'":
                idx += 1
                while idx < N and s[idx] != "'":
                    idx += 1
                idx += 1
                continue
            if c == "(":
                depth += 1
                idx += 1
                continue
            if c == ")":
                depth -= 1
                idx += 1
                continue
            if depth == 0 and match_kw(s, idx, "begin"):
                return s[start:idx].strip(), idx
            # Single-statement IF — we don't handle; bail.
            if depth == 0 and (
                match_kw(s, idx, "select") or match_kw(s, idx, "update")
                or match_kw(s, idx, "insert") or match_kw(s, idx, "delete")
                or match_kw(s, idx, "set") or match_kw(s, idx, "exec")
                or match_kw(s, idx, "return") or match_kw(s, idx, "raiserror")
            ):
                return None
            # Another IF/ELSE means end of this condition
            if depth == 0 and (match_kw(s, idx, "if") or match_kw(s, idx, "else")):
                return None
            idx += 1
        return None

    result = []
    i = 0
    while i < n:
        if match_kw(body, i, "if"):
            # Make sure `if` isn't glued to `(` as in `IF(...)` — T-SQL
            # control-flow IF allows that too, but so does IIF (already
            # renamed earlier? No — we run this BEFORE IIF→IF rename).
            j = i + 2
            cond_res = read_paren_or_plain_cond(body, j)
            if cond_res is None:
                out_char = body[i]
                result.append(out_char)
                i += 1
                continue
            cond, after_cond = cond_res
            begin_idx = after_cond
            after_begin = begin_idx + 5
            end_idx = find_matching_end(body, after_begin)
            if end_idx < 0:
                result.append(body[i])
                i += 1
                continue
            inner = body[after_begin:end_idx]
            parts: list[tuple[str, str | None, str]] = [("if", cond, inner)]
            k = end_idx + 3
            while True:
                kk = k
                while kk < n and body[kk].isspace():
                    kk += 1
                if not match_kw(body, kk, "else"):
                    break
                after_else = kk + 4
                while after_else < n and body[after_else].isspace():
                    after_else += 1
                if match_kw(body, after_else, "if"):
                    jj = after_else + 2
                    cr = read_paren_or_plain_cond(body, jj)
                    if cr is None:
                        break
                    cond2, after_cond2 = cr
                    bgn = after_cond2 + 5
                    eidx = find_matching_end(body, bgn)
                    if eidx < 0:
                        break
                    parts.append(("elseif", cond2, body[bgn:eidx]))
                    k = eidx + 3
                    continue
                if match_kw(body, after_else, "begin"):
                    bgn = after_else + 5
                    eidx = find_matching_end(body, bgn)
                    if eidx < 0:
                        break
                    parts.append(("else", None, body[bgn:eidx]))
                    k = eidx + 3
                    break
                break

            pieces = []
            for idx_p, (kind, cond_p, inner_p) in enumerate(parts):
                inner_p_converted = convert_if_else_blocks(inner_p)
                if kind == "if":
                    pieces.append(f"IF {cond_p} THEN\n{inner_p_converted}")
                elif kind == "elseif":
                    pieces.append(f"ELSEIF {cond_p} THEN\n{inner_p_converted}")
                else:
                    pieces.append(f"ELSE\n{inner_p_converted}")
            pieces.append("END IF;")
            result.append("\n".join(pieces))
            i = k
            continue
        result.append(body[i])
        i += 1

    return "".join(result)


def handle_exec_dynamic_sql(body: str) -> str:
    """Convert `EXEC(@var)` or `EXEC (v_var)` to PREPARE/EXECUTE block."""
    def repl(m: re.Match) -> str:
        var = m.group(1)
        return (f"SET @_dyn_sql = {var};\n"
                f"PREPARE _dyn_stmt FROM @_dyn_sql;\n"
                f"EXECUTE _dyn_stmt;\n"
                f"DEALLOCATE PREPARE _dyn_stmt;")
    # EXEC(v_x) or EXEC (v_x) or EXEC(@x)
    body = re.sub(r"\bEXEC\s*\(\s*(v_[A-Za-z0-9_]+|@[A-Za-z0-9_]+)\s*\)\s*;?",
                  repl, body, flags=re.IGNORECASE)
    return body


def insert_statement_semicolons(body: str) -> str:
    """Insert `;` between statements inside a procedure body."""
    lines = body.splitlines()
    out: list[str] = []

    def prev_nonempty(idx: int) -> int:
        for j in range(idx, -1, -1):
            s = out[j].strip()
            if s and not s.startswith("--") and not s.startswith("/*"):
                return j
        return -1

    for line in lines:
        stripped = line.strip()
        if not stripped:
            out.append(line)
            continue
        m = STMT_STARTERS_RE.match(line)
        if m:
            kw = m.group(1).upper()
            j = prev_nonempty(len(out) - 1)
            if j >= 0:
                prev = out[j].rstrip()
                prev_upper = prev.upper()
                # Skip if prev already ends with a no-semicolon marker
                if not NO_SEMICOLON_END_RE.search(prev):
                    # Special-case SET after UPDATE/INSERT context: SET is part
                    # of the UPDATE statement, not a new statement.
                    skip = False
                    if kw == "SET":
                        # Look back through recent lines to find the most
                        # recent statement-starter. If it's UPDATE/INSERT and
                        # we haven't closed it with ;, don't split.
                        for jj in range(j, -1, -1):
                            ps = out[jj].strip()
                            if not ps or ps.startswith("--") or ps.startswith("/*"):
                                continue
                            up = ps.upper()
                            if up.startswith("UPDATE") or up.startswith("INSERT"):
                                skip = True
                                break
                            if up.endswith(";"):
                                break
                            if STMT_STARTERS_RE.match(ps) and not up.startswith("SET"):
                                break
                    if not skip:
                        out[j] = prev + ";"
        out.append(line)
    j = prev_nonempty(len(out) - 1)
    if j >= 0:
        prev = out[j].rstrip()
        if not NO_SEMICOLON_END_RE.search(prev):
            out[j] = prev + ";"
    return "\n".join(out)


def convert_body(body: str, param_names: set[str]) -> str:
    # Protect strings FIRST, then comments. This order prevents a `--` inside a
    # string literal from being misread as a comment start.
    body, literals = protect_strings(body)
    body, comments = protect_comments(body)

    # Strip SET NOCOUNT ON / IDENTITY_INSERT
    body = re.sub(r"\bSET\s+NOCOUNT\s+(ON|OFF)\s*;?", "", body, flags=re.IGNORECASE)
    body = re.sub(r"\bSET\s+IDENTITY_INSERT\s+\S+\s+(ON|OFF)\s*;?", "", body, flags=re.IGNORECASE)
    body = re.sub(r"\bSET\s+(ANSI_NULLS|QUOTED_IDENTIFIER)\s+(ON|OFF)\s*;?", "", body, flags=re.IGNORECASE)

    # Remove WITH(NOLOCK) hints
    body = re.sub(r"\s*WITH\s*\(\s*NOLOCK\s*\)", "", body, flags=re.IGNORECASE)
    # Drop dbo. prefix
    body = body.replace("[dbo].", "").replace("dbo.", "")
    # Strip square brackets around identifiers
    body = strip_square_brackets(body)

    # Temp table handling
    body = re.sub(r"\bCREATE\s+TABLE\s+#([A-Za-z_][A-Za-z0-9_]*)",
                  lambda m: f"CREATE TEMPORARY TABLE tmp_{m.group(1)}",
                  body, flags=re.IGNORECASE)
    body = re.sub(r"\bDROP\s+TABLE\s+#([A-Za-z_][A-Za-z0-9_]*)",
                  lambda m: f"DROP TEMPORARY TABLE IF EXISTS tmp_{m.group(1)}",
                  body, flags=re.IGNORECASE)
    def select_into_temp(m: re.Match) -> str:
        cols = m.group(1).strip()
        tmp = m.group(2)
        return f"CREATE TEMPORARY TABLE tmp_{tmp} AS SELECT {cols} FROM "
    body = re.sub(
        r"\bSELECT\s+(.*?)\s+INTO\s+#([A-Za-z_][A-Za-z0-9_]*)\s+FROM\s+",
        select_into_temp,
        body,
        flags=re.IGNORECASE | re.DOTALL,
    )
    body = re.sub(r"#([A-Za-z_][A-Za-z0-9_]*)", r"tmp_\1", body)

    # IF..BEGIN..END conversion BEFORE renaming IIF → IF
    body = convert_if_else_blocks(body)

    # Now rename IIF → IF (function)
    body = re.sub(r"\bIIF\s*\(", "IF(", body, flags=re.IGNORECASE)

    # Function mappings
    body = re.sub(r"\bGETDATE\s*\(\s*\)", "NOW()", body, flags=re.IGNORECASE)
    body = re.sub(r"\bISNULL\s*\(", "IFNULL(", body, flags=re.IGNORECASE)
    body = re.sub(r"\bLEN\s*\(", "CHAR_LENGTH(", body, flags=re.IGNORECASE)
    body = re.sub(r"\bSCOPE_IDENTITY\s*\(\s*\)", "LAST_INSERT_ID()", body, flags=re.IGNORECASE)
    body = re.sub(r"@@IDENTITY", "LAST_INSERT_ID()", body, flags=re.IGNORECASE)
    body = re.sub(r"@@ROWCOUNT", "ROW_COUNT()", body, flags=re.IGNORECASE)

    # CONVERT(type, expr[, style]) → CAST / DATE_FORMAT
    def convert_convert(m: re.Match) -> str:
        args = m.group(1)
        parts: list[str] = []
        depth = 0
        cur = ""
        for c in args:
            if c == "(":
                depth += 1
            elif c == ")":
                depth -= 1
            if c == "," and depth == 0:
                parts.append(cur.strip())
                cur = ""
            else:
                cur += c
        if cur.strip():
            parts.append(cur.strip())
        fmt_map = {
            "105": "%d-%m-%Y", "103": "%d/%m/%Y", "101": "%m/%d/%Y",
            "3": "%d/%m/%y", "23": "%Y-%m-%d",
            "120": "%Y-%m-%d %H:%i:%s", "112": "%Y%m%d",
        }
        if len(parts) == 2:
            typ, expr = parts
            typ_u = typ.upper()
            if typ_u.startswith("VARCHAR") or typ_u.startswith("NVARCHAR"):
                return f"CAST({expr} AS CHAR)"
            if typ_u.startswith("INT"):
                return f"CAST({expr} AS SIGNED)"
            return f"CAST({expr} AS {typ})"
        if len(parts) == 3:
            typ, expr, style = parts
            style = style.strip()
            if style in fmt_map:
                return f"DATE_FORMAT({expr}, \x01FMT{style}\x01)"
            return f"CAST({expr} AS CHAR)"
        return m.group(0)

    body = re.sub(r"\bCONVERT\s*\(([^()]*(?:\([^()]*\)[^()]*)*)\)", convert_convert, body, flags=re.IGNORECASE)
    fmt_map = {"105": "%d-%m-%Y", "103": "%d/%m/%Y", "101": "%m/%d/%Y",
               "3": "%d/%m/%y", "23": "%Y-%m-%d",
               "120": "%Y-%m-%d %H:%i:%s", "112": "%Y%m%d"}
    body = re.sub(r"\x01FMT(\d+)\x01", lambda m: f"'{fmt_map[m.group(1)]}'", body)

    # CAST(x AS VARCHAR) / CAST(x AS VARCHAR(n))
    body = re.sub(r"\bCAST\s*\(\s*(.*?)\s+AS\s+N?VARCHAR\s*\(\s*MAX\s*\)\s*\)",
                  r"CAST(\1 AS CHAR)", body, flags=re.IGNORECASE)
    body = re.sub(r"\bCAST\s*\(\s*(.*?)\s+AS\s+N?VARCHAR\s*\(\s*(\d+)\s*\)\s*\)",
                  r"CAST(\1 AS CHAR(\2))", body, flags=re.IGNORECASE)
    body = re.sub(r"\bCAST\s*\(\s*(.*?)\s+AS\s+N?VARCHAR\s*\)",
                  r"CAST(\1 AS CHAR)", body, flags=re.IGNORECASE)

    # SELECT TOP N → SELECT ... LIMIT N. We convert to a note: strip TOP and
    # defer LIMIT placement is hard without SQL parse. Simplest: replace
    # `SELECT TOP N` with `SELECT` and append `LIMIT N` at the next semicolon
    # found at depth 0. That's still complex. For the trivial case, drop TOP.
    body = re.sub(r"\bSELECT\s+TOP\s*\(?\s*(\d+)\s*\)?\s+", r"SELECT ", body, flags=re.IGNORECASE)

    # SELECT @var = expr FROM → SELECT expr INTO v_var FROM
    body = re.sub(
        r"\bSELECT\s+@([A-Za-z0-9_]+)\s*=\s*([^\n,]+?)\s+FROM\s",
        lambda m: f"SELECT {m.group(2)} INTO v_{m.group(1)} FROM ",
        body,
        flags=re.IGNORECASE,
    )

    # SET @x = (SELECT ...)  → SELECT (...) INTO v_x
    def set_from_subquery(m: re.Match) -> str:
        var = m.group(1)
        inner = m.group(2)
        return f"SELECT ({inner}) INTO v_{var}"
    body = re.sub(
        r"\bSET\s+@([A-Za-z0-9_]+)\s*=\s*\(\s*(SELECT[^;]*?)\)(?=\s*(?:;|\n|$))",
        set_from_subquery,
        body,
        flags=re.IGNORECASE | re.DOTALL,
    )

    # SET @x = expr → SET v_x = expr
    body = re.sub(r"\bSET\s+@([A-Za-z0-9_]+)\s*\+=",
                  lambda m: f"SET v_{m.group(1)} = v_{m.group(1)} +",
                  body, flags=re.IGNORECASE)
    body = re.sub(r"\bSET\s+@([A-Za-z0-9_]+)\s*=",
                  lambda m: f"SET v_{m.group(1)} =",
                  body, flags=re.IGNORECASE)

    # String `+` concat → CONCAT()
    body = convert_concat_plus(body)

    # @var → p_var or v_var
    def var_repl(m: re.Match) -> str:
        name = m.group(1)
        if name in param_names:
            return f"p_{name}"
        return f"v_{name}"

    body = re.sub(r"@([A-Za-z_][A-Za-z0-9_]*)", var_repl, body)

    # Backtick `From`, `To` as column names in INSERT/SELECT lists (common in email proc)
    body = re.sub(r"(?<=[(,\s])(From|To)(?=[),\s])",
                  lambda m: f"`{m.group(1)}`", body)

    # EXEC dynamic SQL
    body = handle_exec_dynamic_sql(body)

    # Strip `print <expr>` statements
    body = re.sub(r"^\s*print\s+[^\n;]+;?", "", body, flags=re.IGNORECASE | re.MULTILINE)

    # Restore comments and strings (comments first since they were protected second)
    body = restore_comments(body, comments)
    body = restore_strings(body, literals)

    # Insert statement semicolons
    body = insert_statement_semicolons(body)

    return body


def convert_procedure(batch: str) -> str | None:
    batch = re.sub(r"^\s*/\*+.*?\*+/\s*", "", batch, flags=re.DOTALL)
    m = re.search(
        r"CREATE\s+(?:PROC(?:EDURE)?|FUNCTION)\s+\[?dbo\]?\.\[?([A-Za-z0-9_]+)\]?",
        batch,
        re.IGNORECASE,
    )
    if not m:
        return None
    name = m.group(1)
    name_end = m.end()

    as_end = find_procedure_as(batch, name_end)
    if as_end is None:
        return None

    # Params are between name_end and (as_end - len("AS"))
    # Find the exact position of the AS keyword we matched
    as_keyword_m = re.search(r"\bAS\b", batch[as_end - 2 : as_end], re.IGNORECASE)
    # Simpler: AS keyword is at as_end - 2 .. as_end
    params_src = batch[name_end:as_end - 2]
    body = batch[as_end:]

    params = parse_params(params_src)
    param_names = {p[0] for p in params}

    # Strip the outer BEGIN / END wrapping FIRST, before any body transforms.
    body = body.strip()
    if body.upper().startswith("BEGIN"):
        body = body[5:]
    # Strip trailing END (case-insensitive), possibly followed by a stray ;
    body_r = body.rstrip().rstrip(";").rstrip()
    if body_r.upper().endswith("END"):
        body = body_r[:-3]
    body = body.strip()

    decls, body = extract_declares(body)

    body = convert_body(body, param_names)

    # Auto-declare any v_* variable referenced in body that wasn't declared.
    declared_names = {d[0] for d in decls}
    used_locals = set(re.findall(r"\bv_([A-Za-z_][A-Za-z0-9_]*)", body))
    for uname in sorted(used_locals):
        if uname not in declared_names:
            decls.append((uname, "TEXT"))
            declared_names.add(uname)

    param_parts = [f"IN p_{n} {t}" for n, t in params]
    decl_lines = "\n".join(f"  DECLARE v_{n} {t};" for n, t in decls)

    parts = []
    parts.append(f"DROP PROCEDURE IF EXISTS {name};")
    parts.append("DELIMITER //")
    parts.append(f"CREATE PROCEDURE {name}({', '.join(param_parts)})")
    parts.append("BEGIN")
    if decl_lines:
        parts.append(decl_lines)
    parts.append(body)
    parts.append("END //")
    parts.append("DELIMITER ;")
    return "\n".join(parts)


# ---------- Top-level driver ----------

def convert(sql: str) -> str:
    batches = split_batches(sql)
    out_parts: list[str] = []
    out_parts.append("-- Generated by convert_sql_to_mysql.py")
    out_parts.append("-- Source: Hunt.sql (SQL Server dump)")
    out_parts.append("SET FOREIGN_KEY_CHECKS = 0;")
    out_parts.append("SET SQL_MODE = 'NO_ENGINE_SUBSTITUTION';")
    out_parts.append("")
    out_parts.append("""-- SplitString helper (populates temporary table tmp_split_result)
DROP PROCEDURE IF EXISTS SplitString;
DELIMITER //
CREATE PROCEDURE SplitString(IN p_string TEXT, IN p_delimiter VARCHAR(10))
BEGIN
    DECLARE v_idx INT DEFAULT 1;
    DECLARE v_count INT;
    DROP TEMPORARY TABLE IF EXISTS tmp_split_result;
    CREATE TEMPORARY TABLE tmp_split_result (Item VARCHAR(1000));
    SET v_count = (CHAR_LENGTH(p_string) - CHAR_LENGTH(REPLACE(p_string, p_delimiter, ''))) / CHAR_LENGTH(p_delimiter) + 1;
    WHILE v_idx <= v_count DO
        INSERT INTO tmp_split_result (Item)
        VALUES (SUBSTRING_INDEX(SUBSTRING_INDEX(p_string, p_delimiter, v_idx), p_delimiter, -1));
        SET v_idx = v_idx + 1;
    END WHILE;
END //
DELIMITER ;
""")

    failed: list[str] = []
    for batch in batches:
        if should_skip_batch(batch):
            continue
        upper = batch.lstrip().upper()
        if upper.startswith("CREATE TABLE"):
            try:
                out_parts.append(convert_table(batch))
                out_parts.append("")
            except Exception as e:
                failed.append(f"TABLE: {e}")
                out_parts.append(f"-- TABLE CONVERT FAILED: {e}")
        elif re.match(r"^CREATE\s+(PROC(EDURE)?|FUNCTION)", upper):
            try:
                converted = convert_procedure(batch)
                if converted:
                    out_parts.append(converted)
                    out_parts.append("")
                else:
                    failed.append("PROC: could not parse header")
            except Exception as e:
                failed.append(f"PROC: {e}")
                out_parts.append(f"-- PROC CONVERT FAILED: {e}")
        elif upper.startswith("INSERT"):
            b = batch.replace("[dbo].", "")
            b = strip_square_brackets(b)
            b = re.sub(r"\bN'", "'", b)
            b = re.sub(r"CAST\(([^)]+)\s+AS\s+DateTime\)", r"\1", b, flags=re.IGNORECASE)
            if not b.rstrip().endswith(";"):
                b = b.rstrip() + ";"
            out_parts.append(b)
        elif upper.startswith("ALTER TABLE"):
            continue
        else:
            continue

    if failed:
        out_parts.append("\n-- Conversion warnings:")
        for f in failed:
            out_parts.append(f"-- {f}")

    out_parts.append("SET FOREIGN_KEY_CHECKS = 1;")
    return "\n".join(out_parts)


if __name__ == "__main__":
    src = Path(sys.argv[1] if len(sys.argv) > 1 else "Hunt.sql")
    dst = Path(sys.argv[2] if len(sys.argv) > 2 else "Hunt_mysql.sql")
    sql = src.read_text(encoding="utf-8", errors="replace")
    out = convert(sql)
    dst.write_text(out, encoding="utf-8")
    print(f"Wrote {dst} ({len(out)} bytes)")
