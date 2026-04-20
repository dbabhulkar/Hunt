-- ============================================================
-- Setup script for Hunt application - User creation & prerequisites
-- Run against MySQL database: hunt (localhost:3306)
-- ============================================================


INSERT INTO UserMaster (EmpCode, EmpName, BranchCode, BranchName, ProfileDescription, ProfileId, Department, Active, LoggedIn, Locked, Dormant, Enabled, CreationDate, Password)
SELECT '1004858', 'Dinesh Babhulkar', 1, 'HQ', 'Admin', 1, 'IT', 1, 0, 0, 0, 1, NOW(), 'hunt_api'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM UserMaster WHERE EmpCode = '1004858');

INSERT INTO tbl_API_HUNT_USER (Id, EmpCode, Role, EmailId, IsActive, CreatedBy, CreatedDate)
SELECT COALESCE((SELECT MAX(Id) FROM tbl_API_HUNT_USER), 0) + 1,
       '1004858', 'USER', 'dinesh.babhulkar@3i-infotech.com', 1, 'SYSTEM', NOW()
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM tbl_API_HUNT_USER WHERE EmpCode = '1004858');


SELECT 'UserMaster' AS TableName, EmpCode, EmpName, Active FROM UserMaster WHERE EmpCode = '1004858';

-- Should return 1 row with Role='USER', IsActive=1
SELECT 'tbl_API_HUNT_USER' AS TableName, EmpCode, Role, IsActive FROM tbl_API_HUNT_USER WHERE EmpCode = '1004858';

-- Should return 1 row
-- SELECT 'Hierarchy' AS TableName, EmpCode, EmpRole FROM TBL_OVI_RM_Hierarchy_Mapping WHERE EmpCode = '1004858';
