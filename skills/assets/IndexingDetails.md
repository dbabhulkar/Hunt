Indexing summary
#	Table	Likely-intended PK (NOT NULL col)	Explicit Index / PK	Auto-Inc
1	TBL_OVI_RM_Hierarchy_Mapping	EmpCode, EmpName, EmpRole, SupervisorCode, SupervisorName, SupervisorRole, Business	—	—
2	LoginAttempts	Id	—	—
3	ProfileMaster	ProfileId	—	—
4	tbl_API_hunt_Activity_Log_Tracker	(none)	—	—
5	tbl_API_hunt_Feedback	Feedback_Id	—	—
6	tbl_API_hunt_Misccd	(none)	—	—
7	tbl_API_hunt_USER	Id	—	—
8	tbl_API_ApplicationsSPOC	Id	—	—
9	TBL_API_EXTERNALSERVICES	EXTERNALSERVICE_ID	—	—
10	Tbl_API_FilePath	Tbl_API_FilePath_Id	—	—
11	TBL_API_Main	TBL_API_Main_ID	—	—
12	Tbl_API_Main_Schedule_ActivityLog	Id	—	—
13	TBL_API_Main_Scheduler_Data	(none — no NOT NULL)	—	—
14	TBL_API_Main_Temp	TBL_API_Main_ID	—	—
15	TBL_API_Master	API_Master_ID	—	—
16	TBL_API_Master_Values	Id	—	—
17	tbl_API_MstApplications	Id	—	—
18	TBL_API_statusMaster	statusCode	—	—
19	TBL_API_URLMaster	ID	—	—
20	TBL_API_HUNT_APIDetails	API_ID	—	—
21	tbl_API_HUNT_Approval_trace_trial	Id	—	—
22	tbl_API_HUNT_Audit_log	id	—	—
23	tbl_API_HUNT_ExceptionLevel	apiid	—	—
24	tbl_API_HUNT_ExceptionManagement	ID	—	—
25	TBL_API_HUNT_Integration	IntegrationId	—	—
26	tbl_API_HUNT_MstExceptionApprovalMetrix	(none)	—	—
27	tbl_API_HUNT_MstExepMetrix	(none)	—	—
28	tbl_API_HUNT_MstPartnerCaseApprovalMetrix	(none)	—	—
29	tbl_API_HUNT_MSTPartnerType	(none)	—	—
30	TBL_API_HUNT_NewExceptionManagement	OriginalOnboardingGASID	—	—
31	tbl_API_HUNT_Partner_Onboarding	PartnetOnboading_ID	—	—
32	tbl_API_HUNT_PartnerCaseServiceList	CaseID	—	—
33	tbl_API_HUNT_PartnerOffboardning_getdetails	ID	—	—
34	tbl_API_HUNT_PartnerOnboarding	PartnetOnboadingID	—	—
35	tbl_API_HUNT_PO_ApiDeatil	Id	—	—
36	tbl_API_HUNT_PO_ApprovalTrailTable	Id	—	—
37	tbl_API_HUNT_PO_ApprovalTrailTable_New	Id	—	—
38	tbl_API_HUNT_POCaseApproverMetrix	ID	—	—
39	tbl_API_HUNT_POFeedbackReply_history	Id	—	—
40	TBL_API_HUNT_Producer	ProducerId	—	—
41	TBL_API_HUNT_QuestionData	ID	—	—
42	tbl_API_HUNT_QusServiceDetails	QusSerID	—	—
43	TBL_API_HUNT_ServiceDetails	ServiceID	—	—
44	TBL_API_HUNT_ServiceQuestion	QID	—	—
45	UserMaster	Id	PRIMARY KEY (Id) (L852)	AUTO_INCREMENT


Observations
Only 1 real index in the entire schema: PRIMARY KEY (Id) on UserMaster.
44 tables have no PRIMARY KEY, no secondary indexes, no unique constraints, and no foreign keys — InnoDB will fall back to internal row IDs.
7 tables don't even have a NOT NULL column to hint at a PK: tbl_API_hunt_Activity_Log_Tracker, tbl_API_hunt_Misccd, TBL_API_Main_Scheduler_Data, tbl_API_HUNT_MstExceptionApprovalMetrix, tbl_API_HUNT_MstExepMetrix, tbl_API_HUNT_MstPartnerCaseApprovalMetrix, tbl_API_HUNT_MSTPartnerType.
Want me to draft an ALTER TABLE … ADD PRIMARY KEY / ADD INDEX patch based on the likely PKs and the FK-style columns (CaseId, IntegrationId, APPID, etc.) actually joined in the stored procs?