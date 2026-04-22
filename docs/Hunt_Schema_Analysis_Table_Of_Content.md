# Hunt Database – Schema Analysis Report

**Source file:** `Hunt_mysql_tables.sql` (converted from SQL Server dump via `convert_sql_to_mysql.py`)
**Engine:** InnoDB · **Charset:** utf8mb4
**Totals:** 46 tables · 18 declared FKs · 1 UNIQUE key (outside PKs) · 2 AUTO_INCREMENT columns

---

## 📑 Table of Contents

| # | Section | Description |
|---|---|---|
| 1 | [Executive Summary](#1-executive-summary) | High-level overview of the schema and modernization priorities |
| 2 | [Table Indexing Details](#2-table-indexing-details) | Primary keys, AUTO_INCREMENT, unique constraints, secondary indexes |
| &nbsp;&nbsp;2.1 | [Primary keys](#21-primary-keys) | Tables with and without declared PKs |
| &nbsp;&nbsp;2.2 | [AUTO_INCREMENT](#22-auto_increment) | Auto-increment usage across tables |
| &nbsp;&nbsp;2.3 | [Unique constraints](#23-unique-constraints) | Existing and missing unique keys |
| &nbsp;&nbsp;2.4 | [Secondary indexes](#24-secondary-indexes) | Recommended indexes for migration |
| 3 | [Foreign Key Mismatches & Missing FKs](#3-foreign-key-mismatches--missing-fks) | FK inventory, data-type mismatches, missing FKs |
| &nbsp;&nbsp;3.1 | [Declared FKs (inventory)](#31-declared-fks-inventory) | All 18 currently declared FKs |
| &nbsp;&nbsp;3.2 | [FK data-type mismatches](#32-fk-data-type-mismatches-fk-cannot-be-declared-as-is) | Intended relationships that MySQL rejects |
| &nbsp;&nbsp;3.3 | [Missing FKs](#33-missing-fks-types-match--just-not-declared) | Types match — simply not declared |
| &nbsp;&nbsp;3.4 | [ON DELETE / ON UPDATE behavior](#34-on-delete--on-update-behavior) | Referential action gaps |
| 4 | [Data-Type & Structural Inconsistencies](#4-data-type--structural-inconsistencies) | Type mismatches, legacy artifacts, clear bugs |
| &nbsp;&nbsp;4.1 | [DECIMAL(18,0) used where integer belongs](#41-decimal180-used-where-integer-belongs) | SQL Server legacy types |
| &nbsp;&nbsp;4.2 | [EmpCode length varies](#42-empcode-length-varies-by-table) | Varchar width inconsistency |
| &nbsp;&nbsp;4.3 | [CaseId / caseID / CaseID — three types](#43-caseid--caseid--caseid--three-different-types) | Case/type chaos across partner-onboarding |
| &nbsp;&nbsp;4.4 | [Status / statusCode / Flag — type chaos](#44-status--status--statuscode--flag--type-chaos) | Seven modeling styles for one concept |
| &nbsp;&nbsp;4.5 | [IsActive inconsistencies](#45-isactive-inconsistencies) | Five variants of an active flag |
| &nbsp;&nbsp;4.6 | [Date stored as varchar](#46-date-stored-as-varchar) | Dates in string columns |
| &nbsp;&nbsp;4.7 | [Clear bugs](#47-clear-bugs) | Obvious schema defects |
| &nbsp;&nbsp;4.8 | [Inconsistent audit-column naming](#48-inconsistent-audit-column-naming) | Six audit-field conventions in one DB |
| 5 | [Normalization Analysis](#5-normalization-analysis) | 1NF, 2NF, 3NF violations |
| &nbsp;&nbsp;5.1 | [1NF violations — repeating groups](#51-1nf-violations--repeating-groups) | QValue1..5, FILLER_01..10 anti-patterns |
| &nbsp;&nbsp;5.2 | [2NF / 3NF violations](#52-2nf--3nf-violations--transitive-and-partial-dependencies) | Transitive and partial dependencies |
| &nbsp;&nbsp;5.3 | [Candidate master tables](#53-candidate-master-tables-that-arent-masters) | Free-text columns that should be FKs |
| 6 | [Duplicate / Redundant Tables](#6-duplicate--redundant-tables) | Table-level duplication and orphans |
| 7 | [Naming Convention Inconsistencies](#7-naming-convention-inconsistencies) | Prefix, casing, reserved words, typos |
| &nbsp;&nbsp;7.1 | [Prefix / casing](#71-prefix--casing) | Mixed-case prefix usage |
| &nbsp;&nbsp;7.2 | [Reserved/quoted identifiers](#72-reservedquoted-identifiers) | Status, Value as reserved words |
| &nbsp;&nbsp;7.3 | [Typos embedded in schema](#73-typos-embedded-in-schema) | Spelling errors baked into column/table names |
| 8 | [Recommendations for Migration / Refactoring](#8-recommendations-for-migration--refactoring) | Phased modernization plan + Claude AI workflow |

---

## 1. Executive Summary

The Hunt database is a legacy API-governance / partner-onboarding system ported from SQL Server to MySQL. The structure works but carries clear signs of long-term organic growth: three copies of the API Main table, two Partner Onboarding tables with overlapping purpose, inconsistent naming/casing across objects, widespread use of `DECIMAL(18,0)` where `INT`/`BIGINT` belongs, and many CaseID-style links that cannot be enforced as foreign keys because the source and target columns disagree on data type.

Only 18 foreign keys are declared out of roughly **30+ genuine relationships** inferable from column names. No secondary indexes are defined anywhere except one `UNIQUE KEY`; InnoDB's implicit FK-index creation is the only thing saving the schema from full-table-scan joins.

For a modernization / refactoring project, this schema would benefit most from (a) deduplicating the three `TBL_API_Main*` clones and the two Partner Onboarding tables, (b) aligning `CaseId` data types across the partner-onboarding flow so FKs can be declared, (c) normalizing the repeating `QValue1..5 / QWeightage1..5` groups in `ServiceDetails`, and (d) standardizing audit columns (`CreatedBy` / `CreatedAt` / `UpdatedBy` / `UpdatedAt`) across every table.

<sub>[⬆ Back to Table of Contents](#-table-of-contents)</sub>

---

## 2. Table Indexing Details

### 2.1 Primary keys

| Status | Count | Tables |
|---|---|---|
| Has declared PK | **36** | Most transactional and master tables |
| **No PK declared** | **10** | `TBL_API_HUNT_Hierarchy_Mapping`, `tbl_API_hunt_Activity_Log_Tracker`, `tbl_API_hunt_Misccd`, `TBL_API_Main_Scheduler_Data`, `tbl_API_HUNT_ExceptionLevel`, `tbl_API_HUNT_MstExceptionApprovalMetrix`, `tbl_API_HUNT_MstExepMetrix`, `tbl_API_HUNT_MstPartnerCaseApprovalMetrix`, `tbl_API_HUNT_MSTPartnerType`, `tbl_API_HUNT_PartnerCaseServiceList` |

Tables without PKs generate an internal 6-byte `GEN_CLUST_INDEX` in InnoDB — rows become non-uniquely addressable, replication (binlog row format) gets slower, and ORM tooling (EF Core, Hibernate, SQLAlchemy) will refuse to map them without a manual workaround.

Several tables **have a natural identifier column but simply forgot to declare it PK** — e.g. `tbl_API_hunt_Misccd.MisccdId`, `tbl_API_HUNT_ExceptionLevel.apiid`, `tbl_API_HUNT_PartnerCaseServiceList.CaseID int NOT NULL`. These are easy wins.

### 2.2 AUTO_INCREMENT

Only **two** tables use `AUTO_INCREMENT`: `tbl_Mofee_Url.ID` and `UserMaster.Id`. Every other PK must be assigned from application code — a common source of race conditions, especially on tables like `TBL_API_HUNT_Integration` and `tbl_API_HUNT_Partner_Onboarding` where concurrent inserts are likely.

### 2.3 Unique constraints

Only one non-PK unique constraint exists in the whole schema:

```sql
UNIQUE KEY uk_UserMaster_EmpCode (EmpCode)   -- UserMaster
```

**Missing uniqueness** that should almost certainly exist:
- `ProfileMaster.ProfileShortCode`
- `tbl_API_MstApplications.APPShortName` / `ITGRCCode`
- `TBL_API_statusMaster.statusDescription`
- `tbl_API_hunt_USER.EmpCode`
- `tbl_API_HUNT_Producer.ApplicationName`
- `TBL_API_URLMaster.URLName`
- `TBL_API_HUNT_ServiceQuestion.Question`

### 2.4 Secondary indexes

**Zero user-defined secondary indexes exist in the entire script.** InnoDB auto-creates an index behind each declared FK, which covers 18 columns. Every other query predicate — e.g. `UserMaster.Department`, `Integration.ProjectId`, `LoginAttempts.LoginTime`, `Partner_Onboarding.Partner_Name`, `APIDetails.APIName` — is unindexed.

#### Recommended indexes (migration candidates)

| Table | Suggested index | Rationale |
|---|---|---|
| `LoginAttempts` | `(Empcode, LoginTime)` | Per-user login history lookup |
| `LoginAttempts` | `(ProfileId)` | Role-based reporting |
| `UserMaster` | `(Department, Active)` | Org reports |
| `UserMaster` | `(SuperVisorCode)` | Hierarchy traversal |
| `tbl_API_hunt_Activity_Log_Tracker` | `(Emp_Code, Activity_Date)` | Activity dashboards |
| `TBL_API_HUNT_Integration` | `(ProjectId)`, `(ProjectName)` | Search |
| `tbl_API_hunt_Feedback` | `(Integration_Id, Created_Date)` | Feedback timeline |
| `TBL_API_HUNT_ServiceDetails` | `(IntegrationId)` | Already auto via FK — confirm |
| `tbl_API_HUNT_Partner_Onboarding` | `(Partner_Name)`, `(statusCode)` | Search & status boards |
| `tbl_API_HUNT_PO_ApprovalTrailTable` | `(CaseId)` | Approval trail queries |
| `Tbl_API_FilePath` | `(TBL_API_Main_ID)` | Already auto via FK |
| `tbl_API_HUNT_Audit_log` | `(CaseID, createdDate)` | Audit queries |
| `TBL_API_HUNT_APIDetails` | `(APIName)`, `(ProjectId)` | API catalogue search |

<sub>[⬆ Back to Table of Contents](#-table-of-contents)</sub>

---

## 3. Foreign Key Mismatches & Missing FKs

### 3.1 Declared FKs (inventory)

| # | Child table | Child column | Parent table | Parent column |
|---|---|---|---|---|
| 1 | `TBL_API_HUNT_Integration` | `Status` | `TBL_API_statusMaster` | `statusCode` |
| 2 | `TBL_API_HUNT_Integration` | `Parent_IntegrationId` | `TBL_API_HUNT_Integration` | `IntegrationId` (self) |
| 3 | `TBL_API_HUNT_Integration` | `ConsumerApplicationId` | `tbl_API_MstApplications` | `Id` |
| 4 | `tbl_API_hunt_Feedback` | `Integration_Id` | `TBL_API_HUNT_Integration` | `IntegrationId` |
| 5 | `tbl_API_ApplicationsSPOC` | `APPID` | `tbl_API_MstApplications` | `Id` |
| 6 | `Tbl_API_FilePath` | `TBL_API_Main_ID` | `TBL_API_Main` | `TBL_API_Main_ID` |
| 7 | `TBL_API_Master_Values` | `API_Master_ID` | `TBL_API_Master` | `API_Master_ID` |
| 8 | `tbl_API_HUNT_Partner_Onboarding` | `statusCode` | `TBL_API_statusMaster` | `statusCode` |
| 9 | `tbl_API_HUNT_POCaseApproverMetrix` | `caseID` | `tbl_API_HUNT_Partner_Onboarding` | `PartnetOnboading_ID` |
| 10 | `tbl_API_HUNT_POFeedbackReply_history` | `CaseID` | `tbl_API_HUNT_Partner_Onboarding` | `PartnetOnboading_ID` |
| 11 | `tbl_API_HUNT_POFeedbackReply_history` | `ApprovalId` | `tbl_API_HUNT_PO_ApprovalTrailTable` | `Id` |
| 12 | `tbl_API_HUNT_Audit_log` | `CaseID` | `tbl_API_HUNT_Partner_Onboarding` | `PartnetOnboading_ID` |
| 13 | `tbl_API_HUNT_Audit_log` | `ApprovalID` | `tbl_API_HUNT_PO_ApprovalTrailTable` | `Id` |
| 14 | `TBL_API_HUNT_ServiceDetails` | `IntegrationId` | `TBL_API_HUNT_Integration` | `IntegrationId` |
| 15 | `tbl_API_HUNT_QusServiceDetails` | `ServiceID` | `TBL_API_HUNT_ServiceDetails` | `ServiceID` |
| 16 | `tbl_API_HUNT_QusServiceDetails` | `QID` | `TBL_API_HUNT_ServiceQuestion` | `QID` |
| 17 | `tbl_API_HUNT_QusServiceDetails` | `OptionsID` | `TBL_API_HUNT_QuestionData` | `ID` |
| 18 | `UserMaster` | `ProfileId` | `ProfileMaster` | `ProfileId` |

### 3.2 FK data-type mismatches (FK cannot be declared as-is)

These relationships are clearly intended by the column names but the data types don't match, so MySQL will reject the FK. This is the single largest integrity risk in the schema:

| Child column (type) | Intended parent (type) | Issue |
|---|---|---|
| `tbl_API_HUNT_PO_ApprovalTrailTable.CaseId varchar(50)` | `tbl_API_HUNT_Partner_Onboarding.PartnetOnboading_ID int` | varchar ↔ int |
| `tbl_API_HUNT_PO_ApprovalTrailTable_New.CaseId varchar(50)` | `... PartnetOnboading_ID int` | varchar ↔ int |
| `tbl_API_HUNT_PO_ApiDeatil.CaseId varchar(50)` | `... PartnetOnboading_ID int` | varchar ↔ int |
| `tbl_API_HUNT_ExceptionManagement.CaseId varchar(100)` | `... PartnetOnboading_ID int` | varchar ↔ int and length differs |
| `tbl_API_HUNT_Approval_trace_trial.caseID varchar(50)` | `... PartnetOnboading_ID int` | varchar ↔ int |
| `tbl_API_HUNT_PartnerCaseServiceList.CaseID int` | `... PartnetOnboading_ID int` | Types match; FK simply not declared |
| `tbl_API_HUNT_POFeedbackReply_history.FeedbackID varchar(20)` | `tbl_API_hunt_Feedback.Feedback_Id int` | varchar ↔ int |
| `tbl_API_HUNT_APIDetails.ProjectId int` | `TBL_API_HUNT_Integration.ProjectId varchar(100)` | int ↔ varchar |
| `TBL_API_HUNT_ServiceDetails.Existing_New_Id / Rest_SOAP_Id / ServiceType_Id / APICategory_Id / APIRiskScore_Id / PartnerRiskScore_Id / DomainName_Id / ConsumerDC_Id / ProducerDC_Id (int)` | `TBL_API_Master_Values.Id int` | Types match; 9 FKs simply not declared |

### 3.3 Missing FKs (types match — just not declared)

| Child | Parent | Comment |
|---|---|---|
| `LoginAttempts.ProfileId` | `ProfileMaster.ProfileId` | Data type differs (`int` vs `DECIMAL(18,0)`) — would need casting |
| `LoginAttempts.Empcode` | `UserMaster.EmpCode` | varchar/varchar, lengths differ (50 vs 50) — declarable |
| `tbl_API_hunt_USER.EmpCode` | `UserMaster.EmpCode` | varchar(25) vs varchar(50) — length mismatch |
| `tbl_API_HUNT_Hierarchy_Mapping.EmpCode / SupervisorCode` | `UserMaster.EmpCode` | varchar(100) vs varchar(50) — length mismatch |
| `TBL_API_HUNT_Producer.ApplicationName` | `tbl_API_MstApplications.APPShortName` | Should be an ID FK, not a name match |
| `tbl_API_ApplicationsSPOC.SpocEMPCode` | `UserMaster.EmpCode` | varchar(20) vs varchar(50) |
| `TBL_API_HUNT_Integration.ConsumerApplication` (text) | `tbl_API_MstApplications.APPShortName` | Redundant with `ConsumerApplicationId` |
| `tbl_API_HUNT_ExceptionLevel.apiid` | `TBL_API_HUNT_APIDetails.API_ID` | Types match — declarable |
| `TBL_API_HUNT_QuestionData.QID` | `TBL_API_HUNT_ServiceQuestion.QID` | Strong candidate — missing |

### 3.4 `ON DELETE` / `ON UPDATE` behavior

**None of the 18 declared FKs specify `ON DELETE` or `ON UPDATE` actions.** MySQL defaults to `RESTRICT`, which is usually fine, but reference-heavy tables like `TBL_API_HUNT_Integration` (self-referential) need explicit `ON DELETE SET NULL` or `ON DELETE CASCADE` for the parent-child link to behave correctly when an integration is deleted.

<sub>[⬆ Back to Table of Contents](#-table-of-contents)</sub>

---

## 4. Data-Type & Structural Inconsistencies

### 4.1 `DECIMAL(18,0)` used where integer belongs

Legacy from the SQL Server source (`numeric(18,0)` is SQL Server's default for `SCOPE_IDENTITY()`).

| Table | Column | Current | Should be |
|---|---|---|---|
| `LoginAttempts` | `Id` | `DECIMAL(18,0)` | `BIGINT` |
| `ProfileMaster` | `ProfileId` | `DECIMAL(18,0)` | `INT` or `BIGINT` |
| `UserMaster` | `BranchCode` | `DECIMAL(18,0)` | `INT` |
| `UserMaster` | `ProfileId` | `DECIMAL(18,0)` | Match `ProfileMaster.ProfileId` |

This mismatch (`DECIMAL(18,0)` in `ProfileMaster.ProfileId` vs `int` in `LoginAttempts.ProfileId`) is why an FK can't be declared between them.

### 4.2 `EmpCode` length varies by table

| Table | Column | Length |
|---|---|---|
| `UserMaster` | `EmpCode` | varchar(50) |
| `tbl_API_hunt_USER` | `EmpCode` | varchar(25) |
| `tbl_API_ApplicationsSPOC` | `SpocEMPCode` | varchar(20) |
| `TBL_API_HUNT_Hierarchy_Mapping` | `EmpCode` | varchar(100) |
| `LoginAttempts` | `Empcode` | varchar(50) |
| `tbl_API_hunt_Activity_Log_Tracker` | `Emp_Code` | varchar(50) |

Standardize on **one** width (recommended `VARCHAR(50)`) everywhere.

### 4.3 `CaseId` / `caseID` / `CaseID` — three different types

| Table | Column | Type |
|---|---|---|
| `tbl_API_HUNT_Partner_Onboarding` | `PartnetOnboading_ID` | `int` (the canonical PK) |
| `tbl_API_HUNT_PO_ApprovalTrailTable` | `CaseId` | `varchar(50)` |
| `tbl_API_HUNT_POCaseApproverMetrix` | `caseID` | `int` |
| `tbl_API_HUNT_POFeedbackReply_history` | `CaseID` | `int` |
| `tbl_API_HUNT_Audit_log` | `CaseID` | `int` |
| `tbl_API_HUNT_ExceptionManagement` | `CaseId` | `varchar(100)` |
| `tbl_API_HUNT_Approval_trace_trial` | `caseID` | `varchar(50)` |

### 4.4 Status / status / statusCode / Flag — type chaos

| Pattern | Tables using it |
|---|---|
| `Status int` + FK to `statusMaster` | `Integration`, `Feedback`, `Misccd`, `ServiceQuestion`, `QuestionData` |
| `status int` (no FK) | `MstApplications`, `ApplicationsSPOC`, `URLMaster` |
| `statusCode int` + FK | `Partner_Onboarding` |
| `Status varchar(50)` | `Approval_trace_trial`, `PO_ApprovalTrailTable`, `POFeedbackReply_history`, `Main_Schedule_ActivityLog`, `PO_ApprovalTrailTable_New` |
| `status varchar(20)` | `POCaseApproverMetrix`, `Audit_log` |
| `Flag varchar(50)` | `LoginAttempts`, `ProfileMaster`, `UserMaster` |

Three different ways to model the same concept in one database.

### 4.5 `IsActive` inconsistencies

| Column | Type | Table |
|---|---|---|
| `Is_active` | `TINYINT(1)` | `tbl_Mofee_Url` |
| `IsActive` | `TINYINT(1)` | `tbl_API_hunt_Feedback`, `QusServiceDetails` |
| `IsActive` | **`int`** (!) | `tbl_API_hunt_USER` |
| `Active` | `TINYINT(1)` | `Tbl_API_FilePath`, `UserMaster` |
| `Authorised` | `TINYINT(1)` | `ProfileMaster` |

### 4.6 Date stored as `varchar`

| Table | Column | Type | Should be |
|---|---|---|---|
| `tbl_API_hunt_Misccd` | `LUDT` | `varchar(20)` | `DATE` |
| `tbl_API_hunt_Misccd` | `LUTM` | `varchar(20)` | `TIME` |
| `TBL_API_EXTERNALSERVICES` | `DAT_GO_LIVE` | `LONGTEXT` | `DATETIME` |
| `TBL_API_EXTERNALSERVICES` | `DAT_LAST_MNT` | `LONGTEXT` | `DATETIME` |

### 4.7 Clear bugs

| Field | Declaration | Problem |
|---|---|---|
| `TBL_API_HUNT_NewExceptionManagement.ToDate` | `varchar(1)` | Can only hold a single character — almost certainly a typo, should be `DATE` |
| `TBL_API_HUNT_Integration.ContainerName` | `CHAR(10)` | Fixed-width name pads with spaces, causing subtle string-comparison bugs |
| `TBL_API_Main.CTR_UPDAT_SRLNO` | `TINYINT(1)` | Same column is `LONGTEXT` in `TBL_API_EXTERNALSERVICES` — the counter intent is clearly numeric |
| `tbl_API_HUNT_PartnerOnboarding` uses backticked column names with spaces: `` `Tentative Go Live Date` ``, `` `Partner Type` `` | Forces every query to use backticks; safer to rename to snake_case |

### 4.8 Inconsistent audit-column naming

One database ships **six** different conventions for "who changed this, and when":

| Convention | Example tables |
|---|---|
| `CreatedBy` / `CreatedAt` / `UpdatedBy` / `UpdatedAt` | `TBL_API_HUNT_Integration`, `ServiceDetails`, `APIDetails`, `Producer` |
| `CreatedBy` / `CreatedDate` / `UpdateBy` / `UpdatedDate` (note the typo "UpdateBy") | `tbl_API_MstApplications`, `tbl_API_hunt_USER`, `ApplicationsSPOC` |
| `Created_By` / `Created_Date` / `Updated_By` / `Updated_Date` | `tbl_API_hunt_Feedback` |
| `created_By` / `created_date` / `Updated_By` / `Updated_date` | `tbl_API_HUNT_Partner_Onboarding` |
| `createdBy` / `createdDate` / `UpdatedBy` / `updatedDate` | `tbl_API_HUNT_Audit_log`, `Approval_trace_trial` |
| `Maker` / `MakerDate` / `Checker` / `CheckerDate` | `UserMaster`, `ProfileMaster` |
| `LUSR` / `LUDT` / `LUTM` (legacy CODI-style) | `tbl_API_hunt_Misccd` |

<sub>[⬆ Back to Table of Contents](#-table-of-contents)</sub>

---

## 5. Normalization Analysis

### 5.1 1NF violations — repeating groups

**`TBL_API_HUNT_ServiceDetails`** stores five questionnaire answers as columns instead of rows:

```sql
QValue1 int, QWeightage1 real,
QValue2 int, QWeightage2 real,
QValue3 int, QWeightage3 real,
QValue4 int, QWeightage4 real,
QValue5 int, QWeightage5 real
```

Ironically, `tbl_API_HUNT_QusServiceDetails` already models the same answers correctly (one row per service+question+option). The `QValue*` columns are redundant and should be removed.

**`TBL_API_EXTERNALSERVICES`**, **`TBL_API_Main`**, **`TBL_API_Main_Temp`**, **`TBL_API_Main_Scheduler_Data`** all contain `FILLER_01` through `FILLER_10` (ten unnamed spare columns). This is an anti-pattern inherited from older systems; in MySQL it's better to drop them and use `JSON` for truly variable payloads.

### 5.2 2NF / 3NF violations — transitive and partial dependencies

| Table | Redundant columns | Correct source |
|---|---|---|
| `LoginAttempts` | `Empname`, `ProfileName`, `Brname` | Derivable from `Empcode` → `UserMaster`, `ProfileId` → `ProfileMaster`, `Brcode` → branch master |
| `UserMaster` | `ProfileDescription` | Derivable from `ProfileId` → `ProfileMaster.ProfileDescription` |
| `TBL_API_HUNT_Hierarchy_Mapping` | `EmpName`, `EmpRole`, `SupervisorName`, `SupervisorRole` | All derivable from `EmpCode` and `SupervisorCode` pointing to `UserMaster` |
| `tbl_API_ApplicationsSPOC` | `APPShortName` | Derivable from `APPID` → `tbl_API_MstApplications` |
| `TBL_API_HUNT_Integration` | `ConsumerApplication` (text) | Redundant with `ConsumerApplicationId` (FK) |
| `TBL_API_HUNT_ServiceDetails` | `ConsumerApplication`, `ProducerApplication`, `ConsumerDC`, `ProducerDC` (all text) | Duplicated by parallel `*_Id` columns meant to FK into master tables |
| `TBL_API_HUNT_ServiceQuestion` | `APICategory` (text) | Redundant with `APICategoryId` (int) |
| `tbl_API_HUNT_Partner_Onboarding` | `Partner_Name`, `Partnerrisk`, `APIrisk_score` (all free-text) | Should reference partner and risk-level master tables |

### 5.3 Candidate master tables that aren't masters

Several columns hold values from an obviously-finite domain but are stored as plain `varchar`, meaning typos proliferate and joins can't validate:

- `PartnerType`, `PartnerEntityType`, `TPRMAapplicable` — already have `tbl_API_HUNT_MSTPartnerType`, but it has no PK and no FK from the transactional tables back to it.
- `ExceptionLevel` / `ExceptionImpact` — have `tbl_API_HUNT_MstExepMetrix`, same problem.
- `API_Type`, `ServiceInterfaceType`, `ServiceCategory`, `HostingDC`, `DomainName` — each appears as free-text in several tables; they should live in `TBL_API_Master_Values` under a category ID.

<sub>[⬆ Back to Table of Contents](#-table-of-contents)</sub>

---

## 6. Duplicate / Redundant Tables

| # | Tables | Observation |
|---|---|---|
| 1 | `TBL_API_Main` · `TBL_API_Main_Temp` · `TBL_API_Main_Scheduler_Data` | Three variants of the same 55-column schema. `_Temp` and `_Scheduler_Data` look like ETL staging areas; they should either be clearly documented as staging or consolidated with a `source_system` column |
| 2 | `TBL_API_EXTERNALSERVICES` vs `TBL_API_Main` | ~50 overlapping columns with slight type differences (`varchar` vs `LONGTEXT`). Likely two different ingestion paths feeding the same concept |
| 3 | `tbl_API_HUNT_PartnerOnboarding` vs `tbl_API_HUNT_Partner_Onboarding` | Near-identical intent, different PK names (`PartnetOnboadingID` vs `PartnetOnboading_ID`), different column casing. Only the second one is referenced by FKs — the first looks orphaned |
| 4 | `tbl_API_HUNT_PO_ApprovalTrailTable` vs `tbl_API_HUNT_PO_ApprovalTrailTable_New` | Byte-for-byte identical schema. Classic "we'll migrate later" situation |
| 5 | `tbl_API_HUNT_ExceptionManagement` vs `TBL_API_HUNT_NewExceptionManagement` | Very similar, with `New...` adding `DurationofApproval` and `HowExceptiontobeImplemented` (note the typo) |
| 6 | `tbl_API_HUNT_MstExceptionApprovalMetrix` vs `tbl_API_HUNT_MstPartnerCaseApprovalMetrix` | Both approval-matrix masters, could unify with a `matrix_type` column |

### "PartnetOnboading" — a propagating typo

The misspelling of **Partner Onboarding** as **Partnet Onboading** appears in the PK name (`PartnetOnboading_ID`), the table name (`tbl_API_HUNT_PartnerOnboarding` has `PartnetOnboadingID`), and in every FK constraint referencing it. Any rename needs to be coordinated across 5+ tables and every ORM/DAO layer.

<sub>[⬆ Back to Table of Contents](#-table-of-contents)</sub>

---

## 7. Naming Convention Inconsistencies

### 7.1 Prefix / casing

- `tbl_` (lowercase) · `TBL_` (uppercase) · no prefix (`UserMaster`, `ProfileMaster`, `LoginAttempts`)
- `tbl_API_hunt_XXX` · `tbl_API_HUNT_XXX` · `TBL_API_HUNT_XXX` — all refer to the same "API HUNT" module
- Same column appears as `CaseId`, `CaseID`, `caseID` in sibling tables

### 7.2 Reserved/quoted identifiers

`Status` and `Value` are MySQL reserved words used unquoted in some places and with backticks in others — this is a portability risk when migrating to a different SQL dialect.

### 7.3 Typos embedded in schema

| Typo | Correct |
|---|---|
| `PartnetOnboading_ID` | `PartnerOnboarding_ID` |
| `UpdateBy` | `UpdatedBy` |
| `PO_ApiDeatil` | `PO_ApiDetail` |
| `DEPRICATED_API` | `DEPRECATED_API` |
| `IsDepricated` | `IsDeprecated` |
| `Authorised` / `AuthoriseDate` vs `Authoriser` | inconsistent British/American spellings |
| `Offboardning` (in `tbl_API_HUNT_PartnerOffboardning_getdetails`) | `Offboarding` |
| `HowExceptiontobeImplemented` | `HowExceptionToBeImplemented` |
| `PartenerstobeIntegrated` | `PartnersToBeIntegrated` |
| `partner_checkilist` | `partner_checklist` |
| `TPRMAapplicable` | `TPRMApplicable` |

<sub>[⬆ Back to Table of Contents](#-table-of-contents)</sub>

---

## 8. Recommendations for Migration / Refactoring

### Phase 1 — Low-risk, high-value cleanup
1. Add `PRIMARY KEY` to the 10 tables missing one.
2. Convert all `DECIMAL(18,0)` id columns to `BIGINT` (or `INT`).
3. Add the obvious missing FKs where types already match (see §3.3).
4. Add `AUTO_INCREMENT` to surrogate PKs where application currently generates IDs.
5. Create the secondary indexes recommended in §2.4.

### Phase 2 — Breaking changes (coordinate with application team)
6. Unify `EmpCode` length to `VARCHAR(50)` everywhere, then declare the missing FKs to `UserMaster.EmpCode`.
7. Unify `CaseId` type across partner-onboarding tables (pick `INT`) and declare the 5 missing FKs.
8. Migrate `Status` columns: one canonical `INT` + FK to `TBL_API_statusMaster` everywhere.
9. Deduplicate the three `TBL_API_Main*` tables and the two Partner Onboarding tables.
10. Remove redundant text columns in favor of FKs (`ProfileDescription`, `APIShortName`, `ConsumerApplication`, `APICategory`, etc.).
11. Move `QValue1..5 / QWeightage1..5` out of `ServiceDetails` — the proper table (`QusServiceDetails`) already exists.
12. Drop `FILLER_01..10` or replace with a single `extra JSON` column.

### Phase 3 — Cosmetic/schema-hygiene
13. Standardize audit columns to `CreatedBy` / `CreatedAt` / `UpdatedBy` / `UpdatedAt` across every table.
14. Rename typos (`PartnetOnboading`, `Depricated`, `Offboardning`, …) — coordinate with application DTOs.
15. Replace backticked-with-spaces column names with snake_case.
16. Add `ON DELETE` / `ON UPDATE` actions to every FK explicitly.

### Using Claude AI in this workflow
For a legacy-modernization project, Claude can automate several of the above steps: generate `ALTER TABLE` scripts for the type-widening in Phase 1, produce data-migration SQL for the Partner Onboarding consolidation (item 9), generate DTO / repository diffs for the typo renames (item 14), and write validation queries that confirm no orphaned rows exist before turning each new FK on. Feed Claude the DDL plus the table-by-table recommendations in this document and it can produce an executable migration script set with forward + rollback SQL per change.

<sub>[⬆ Back to Table of Contents](#-table-of-contents)</sub>

---

*Generated from a static analysis of the DDL. Row-level validation (orphan detection, duplicate-row counts, actual column cardinality) will need queries executed against a live copy of the database.*
