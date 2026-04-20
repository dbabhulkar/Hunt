-- ============================================================
-- Seed data for Service Details dropdowns
-- All values go into tbl_API_Hunt_Misccd
-- Table: MisccdId (PK), CDTP (category), CDValDesc (display value), Seq, Status (1=active)
-- ============================================================

-- Existing / New
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (1, 'Existing_New', 'Existing', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (2, 'Existing_New', 'New', 2, 1);

-- Rest / SOAP
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (3, 'Rest_Soap', 'REST', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (4, 'Rest_Soap', 'SOAP', 2, 1);

-- Service Type
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (5, 'Service Type', 'Synchronous', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (6, 'Service Type', 'Asynchronous', 2, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (7, 'Service Type', 'Batch', 3, 1);

-- API Type
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (8, 'API Type', 'Internal', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (9, 'API Type', 'External', 2, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (10, 'API Type', 'Partner', 3, 1);

-- API Category
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (11, 'API Category', 'Internal', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (12, 'API Category', 'External', 2, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (13, 'API Category', 'Partner', 3, 1);

-- Domain Name
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (14, 'Domain Name', 'Payments', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (15, 'Domain Name', 'Lending', 2, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (16, 'Domain Name', 'Insurance', 3, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (17, 'Domain Name', 'Investment', 4, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (18, 'Domain Name', 'Core Banking', 5, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (19, 'Domain Name', 'Digital', 6, 1);

-- Middleware Name
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (20, 'Middleware Name', 'API Gateway', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (21, 'Middleware Name', 'ESB', 2, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (22, 'Middleware Name', 'MQ', 3, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (23, 'Middleware Name', 'Kafka', 4, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (24, 'Middleware Name', 'Direct Integration', 5, 1);

-- API Risk Score (also used in the SP)
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (25, 'API Risk Score', 'Low', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (26, 'API Risk Score', 'Medium', 2, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (27, 'API Risk Score', 'High', 3, 1);

-- Partner Risk Score
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (28, 'Partner Risk Score', 'Low', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (29, 'Partner Risk Score', 'Medium', 2, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (30, 'Partner Risk Score', 'High', 3, 1);

-- Consumer DC
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (31, 'Consumer DC', 'DC1', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (32, 'Consumer DC', 'DC2', 2, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (33, 'Consumer DC', 'Cloud', 3, 1);

-- Producer DC
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (34, 'Producer DC', 'DC1', 1, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (35, 'Producer DC', 'DC2', 2, 1);
INSERT INTO tbl_API_Hunt_Misccd (MisccdId, CDTP, CDValDesc, Seq, Status) VALUES (36, 'Producer DC', 'Cloud', 3, 1);

-- ============================================================
-- VERIFICATION
-- ============================================================
SELECT CDTP, COUNT(*) AS cnt FROM tbl_API_Hunt_Misccd WHERE Status = 1 GROUP BY CDTP ORDER BY CDTP;

