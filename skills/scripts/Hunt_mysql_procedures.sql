-- SplitString helper (populates temporary table tmp_split_result)
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

DROP PROCEDURE IF EXISTS api_hunt_consumerapplication_name;
DELIMITER //
CREATE PROCEDURE api_hunt_consumerapplication_name()
BEGIN
select fullname from tbl_API_MstApplications;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS api_hunt_SearchUser;
DELIMITER //
CREATE PROCEDURE api_hunt_SearchUser(IN p_role VARCHAR(30))
BEGIN
SELECT A.EmpCode,B.EmpName,IF(role =p_role ,'','disbled') as disbledCSS       
FROM tbl_API_hunt_USER A inner join UserMaster B on A.EmpCode=B.EmpCode        
WHERE Role =p_role;
SELECT A.EmpCode,B.EmpName  ,IF(role !=p_role  ,'disbled','') as disbledCSS     
FROM tbl_API_hunt_USER A inner join UserMaster B on A.EmpCode=B.EmpCode        
WHERE Role != p_role and role not in ('ADMIN','ITARCHITECH');
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS api_hunt_user;
DELIMITER //
CREATE PROCEDURE api_hunt_user(IN p_role VARCHAR(30))
BEGIN
SELECT * 
FROM tbl_API_hunt_USER 
WHERE Role = p_role;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_API_Search;
DELIMITER //
CREATE PROCEDURE SP_API_Search(IN p_searchText varchar(1000), IN p_action varchar(50), IN p_id int)
BEGIN
IF (p_action ='getAll') THEN

	
	select A.TBL_API_Main_ID,A.PRODUCT_PROCESSOR_URL_UAT  as ServiceURL,A.SERVICE_DESC,SERVICE_MIDDLEWARE.Value as SERVICE_TYPE 
	,A.COD_SERVICE_ID,A.SERVICE_SIGNATURE,
	SERVICE_PROVIDER.Value as SERVICE_PROVIDER
	,A.PRODUCT_PROCESSOR_WSDL , DOMAIN_NAME.Value as DOMAIN_NAME
	from TBL_API_Main A
	left join TBL_API_Master_Values SERVICE_MIDDLEWARE
	on A.NAM_SERVICE_MIDDLEWARE = SERVICE_MIDDLEWARE.Id And SERVICE_MIDDLEWARE.API_Master_ID=1
	left join TBL_API_Master_Values SERVICE_INTERFACE_TYPE 
	on a.SERVICE_INTERFACE_TYPE = SERVICE_INTERFACE_TYPE.Id and SERVICE_INTERFACE_TYPE.API_Master_ID=2
	left join TBL_API_Master_Values SERVICE_CATEGORY 
	on a.SERVICE_CATEGORY = SERVICE_CATEGORY.Id and SERVICE_CATEGORY.API_Master_ID=3
	left join TBL_API_Master_Values SERVICE_PROVIDER 
	on a.SERVICE_PROVIDER = SERVICE_PROVIDER.Id and SERVICE_PROVIDER.API_Master_ID=4

	left join TBL_API_Master_Values SERVICE_TYPE 
	on a.SERVICE_TYPE = SERVICE_TYPE.Id and SERVICE_TYPE.API_Master_ID=5

	left join TBL_API_Master_Values NAM_CONTAINER 
	on a.NAM_CONTAINER = NAM_CONTAINER.Id and NAM_CONTAINER.API_Master_ID=6

	left join TBL_API_Master_Values NAM_DOMAIN 
	on a.NAM_DOMAIN = NAM_DOMAIN.Id and NAM_DOMAIN.API_Master_ID=7

	left join TBL_API_Master_Values DOMAIN_NAME 
	on a.DOMAIN_NAME = DOMAIN_NAME.Id and DOMAIN_NAME.API_Master_ID=8

	Where (A.COD_SERVICE_ID like CONCAT('%', p_searchText, '%') OR  A.SERVICE_SIGNATURE like CONCAT('%', p_searchText, '%')
		OR  SERVICE_MIDDLEWARE.Value like CONCAT('%', p_searchText, '%')  OR  SERVICE_PROVIDER.Value like CONCAT('%', p_searchText, '%')
		OR  A.PRODUCT_PROCESSOR_WSDL like CONCAT('%', p_searchText, '%') OR  DOMAIN_NAME.Value like CONCAT('%', p_searchText, '%'));
	
ELSEIF (p_action='getOne') THEN

		select A.TBL_API_Main_ID,A.PRODUCT_PROCESSOR_URL_UAT  as ServiceURL,A.SERVICE_DESC,SERVICE_MIDDLEWARE.Value as SERVICE_TYPE 
	,A.COD_SERVICE_ID,A.SERVICE_SIGNATURE,
	SERVICE_PROVIDER.Value as SERVICE_PROVIDER
	,A.PRODUCT_PROCESSOR_WSDL , DOMAIN_NAME.Value as DOMAIN_NAME
	from TBL_API_Main A
	left join TBL_API_Master_Values SERVICE_MIDDLEWARE
	on A.NAM_SERVICE_MIDDLEWARE = SERVICE_MIDDLEWARE.Id And SERVICE_MIDDLEWARE.API_Master_ID=1
	left join TBL_API_Master_Values SERVICE_INTERFACE_TYPE 
	on a.SERVICE_INTERFACE_TYPE = SERVICE_INTERFACE_TYPE.Id and SERVICE_INTERFACE_TYPE.API_Master_ID=2
	left join TBL_API_Master_Values SERVICE_CATEGORY 
	on a.SERVICE_CATEGORY = SERVICE_CATEGORY.Id and SERVICE_CATEGORY.API_Master_ID=3
	left join TBL_API_Master_Values SERVICE_PROVIDER 
	on a.SERVICE_PROVIDER = SERVICE_PROVIDER.Id and SERVICE_PROVIDER.API_Master_ID=4

	left join TBL_API_Master_Values SERVICE_TYPE 
	on a.SERVICE_TYPE = SERVICE_TYPE.Id and SERVICE_TYPE.API_Master_ID=5

	left join TBL_API_Master_Values NAM_CONTAINER 
	on a.NAM_CONTAINER = NAM_CONTAINER.Id and NAM_CONTAINER.API_Master_ID=6

	left join TBL_API_Master_Values NAM_DOMAIN 
	on a.NAM_DOMAIN = NAM_DOMAIN.Id and NAM_DOMAIN.API_Master_ID=7

	left join TBL_API_Master_Values DOMAIN_NAME 
	on a.DOMAIN_NAME = DOMAIN_NAME.Id and DOMAIN_NAME.API_Master_ID=8
	where A.TBL_API_Main_ID =p_id;
	
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_API_Search_Temp;
DELIMITER //
CREATE PROCEDURE SP_API_Search_Temp(IN p_searchText varchar(1000), IN p_action varchar(50), IN p_id int, IN p_whereClause LONGTEXT)
BEGIN
  DECLARE v_query LONGTEXT DEFAULT '';
IF (p_action ='getAll') THEN

	select A.TBL_API_Main_ID,A.PRODUCT_PROCESSOR_URL_UAT  as ServiceURL,A.SERVICE_DESC,SERVICE_MIDDLEWARE.Value as SERVICE_TYPE
	,A.COD_SERVICE_ID,A.SERVICE_SIGNATURE,
	SERVICE_PROVIDER.Value as SERVICE_PROVIDER
	,A.PRODUCT_PROCESSOR_WSDL , DOMAIN_NAME.Value as DOMAIN_NAME,A.fileName
	from TBL_API_Main A
	left join TBL_API_Master_Values SERVICE_MIDDLEWARE
	on A.NAM_SERVICE_MIDDLEWARE = SERVICE_MIDDLEWARE.Id And SERVICE_MIDDLEWARE.API_Master_ID=1
	left join TBL_API_Master_Values SERVICE_INTERFACE_TYPE 
	on a.SERVICE_INTERFACE_TYPE = SERVICE_INTERFACE_TYPE.Id and SERVICE_INTERFACE_TYPE.API_Master_ID=2
	left join TBL_API_Master_Values SERVICE_CATEGORY 
	on a.SERVICE_CATEGORY = SERVICE_CATEGORY.Id and SERVICE_CATEGORY.API_Master_ID=3
	left join TBL_API_Master_Values SERVICE_PROVIDER 
	on a.SERVICE_PROVIDER = SERVICE_PROVIDER.Id and SERVICE_PROVIDER.API_Master_ID=4

	left join TBL_API_Master_Values SERVICE_TYPE 
	on a.SERVICE_TYPE = SERVICE_TYPE.Id and SERVICE_TYPE.API_Master_ID=5

	left join TBL_API_Master_Values NAM_CONTAINER 
	on a.NAM_CONTAINER = NAM_CONTAINER.Id and NAM_CONTAINER.API_Master_ID=6

	left join TBL_API_Master_Values NAM_DOMAIN 
	on a.NAM_DOMAIN = NAM_DOMAIN.Id and NAM_DOMAIN.API_Master_ID=7

	left join TBL_API_Master_Values DOMAIN_NAME 
	on a.DOMAIN_NAME = DOMAIN_NAME.Id and DOMAIN_NAME.API_Master_ID=8

	Where (A.COD_SERVICE_ID like CONCAT('%', p_searchText, '%') OR  A.SERVICE_SIGNATURE like CONCAT('%', p_searchText, '%')
		OR  SERVICE_MIDDLEWARE.Value like CONCAT('%', p_searchText, '%')  OR  SERVICE_PROVIDER.Value like CONCAT('%', p_searchText, '%')
		OR  A.PRODUCT_PROCESSOR_WSDL like CONCAT('%', p_searchText, '%') OR  DOMAIN_NAME.Value like CONCAT('%', p_searchText, '%')OR  A.fileName like CONCAT('%', p_searchText, '%'));
		
	
ELSEIF (p_action='Filter') THEN

	  SET v_query =' select A.TBL_API_Main_ID,A.PRODUCT_PROCESSOR_URL_UAT  as ServiceURL,A.SERVICE_DESC,SERVICE_MIDDLEWARE.Value as SERVICE_TYPE,A.SERVICE_INTERFACE_TYPE,A.SERVICE_CATEGORY
	,A.COD_SERVICE_ID,A.SERVICE_SIGNATURE,
	SERVICE_PROVIDER.Value as SERVICE_PROVIDER
	,A.PRODUCT_PROCESSOR_WSDL , DOMAIN_NAME.Value as DOMAIN_NAME,A.fileName
	   INTO #tmp_DynamicTable  from TBL_API_Main A
	left join TBL_API_Master_Values SERVICE_MIDDLEWARE
	on A.NAM_SERVICE_MIDDLEWARE = SERVICE_MIDDLEWARE.Id And SERVICE_MIDDLEWARE.API_Master_ID=1
	left join TBL_API_Master_Values SERVICE_INTERFACE_TYPE 
	on a.SERVICE_INTERFACE_TYPE = SERVICE_INTERFACE_TYPE.Id and SERVICE_INTERFACE_TYPE.API_Master_ID=2
	left join TBL_API_Master_Values SERVICE_CATEGORY 
	on a.SERVICE_CATEGORY = SERVICE_CATEGORY.Id and SERVICE_CATEGORY.API_Master_ID=3
	left join TBL_API_Master_Values SERVICE_PROVIDER 
	on a.SERVICE_PROVIDER = SERVICE_PROVIDER.Id and SERVICE_PROVIDER.API_Master_ID=4

	left join TBL_API_Master_Values SERVICE_TYPE 
	on a.SERVICE_TYPE = SERVICE_TYPE.Id and SERVICE_TYPE.API_Master_ID=5

	left join TBL_API_Master_Values NAM_CONTAINER 
	on a.NAM_CONTAINER = NAM_CONTAINER.Id and NAM_CONTAINER.API_Master_ID=6

	left join TBL_API_Master_Values NAM_DOMAIN 
	on a.NAM_DOMAIN = NAM_DOMAIN.Id and NAM_DOMAIN.API_Master_ID=7

	left join TBL_API_Master_Values DOMAIN_NAME 
	on a.DOMAIN_NAME = DOMAIN_NAME.Id and DOMAIN_NAME.API_Master_ID=8;
	     
	 select * into #tmp_finalTable from  #tmp_DynamicTable ';
	  SET v_query = CONCAT(v_query, ' Select * From #tmp_finalTable where  ', p_whereClause, ';
	     Drop table #tmp_DynamicTable ; Drop table #tmp_finalTable ;');

	SET @_dyn_sql = v_query;
PREPARE _dyn_stmt FROM @_dyn_sql;
EXECUTE _dyn_stmt;
DEALLOCATE PREPARE _dyn_stmt; 
	
ELSEIF (p_action='getOne') THEN

		select A.TBL_API_Main_ID,A.PRODUCT_PROCESSOR_URL_UAT  as ServiceURL,A.SERVICE_DESC,SERVICE_MIDDLEWARE.Value as SERVICE_TYPE 
	,A.COD_SERVICE_ID,A.SERVICE_SIGNATURE,
	SERVICE_PROVIDER.Value as SERVICE_PROVIDER
	,A.PRODUCT_PROCESSOR_WSDL , DOMAIN_NAME.Value as DOMAIN_NAME,A.fileName
	from TBL_API_Main A
	left join TBL_API_Master_Values SERVICE_MIDDLEWARE
	on A.NAM_SERVICE_MIDDLEWARE = SERVICE_MIDDLEWARE.Id And SERVICE_MIDDLEWARE.API_Master_ID=1
	left join TBL_API_Master_Values SERVICE_INTERFACE_TYPE 
	on a.SERVICE_INTERFACE_TYPE = SERVICE_INTERFACE_TYPE.Id and SERVICE_INTERFACE_TYPE.API_Master_ID=2
	left join TBL_API_Master_Values SERVICE_CATEGORY 
	on a.SERVICE_CATEGORY = SERVICE_CATEGORY.Id and SERVICE_CATEGORY.API_Master_ID=3
	left join TBL_API_Master_Values SERVICE_PROVIDER 
	on a.SERVICE_PROVIDER = SERVICE_PROVIDER.Id and SERVICE_PROVIDER.API_Master_ID=4

	left join TBL_API_Master_Values SERVICE_TYPE 
	on a.SERVICE_TYPE = SERVICE_TYPE.Id and SERVICE_TYPE.API_Master_ID=5

	left join TBL_API_Master_Values NAM_CONTAINER 
	on a.NAM_CONTAINER = NAM_CONTAINER.Id and NAM_CONTAINER.API_Master_ID=6

	left join TBL_API_Master_Values NAM_DOMAIN 
	on a.NAM_DOMAIN = NAM_DOMAIN.Id and NAM_DOMAIN.API_Master_ID=7

	left join TBL_API_Master_Values DOMAIN_NAME 
	on a.DOMAIN_NAME = DOMAIN_NAME.Id and DOMAIN_NAME.API_Master_ID=8
	where A.TBL_API_Main_ID =p_id;
	
	
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_API_HUNT_AdminMaster;
DELIMITER //
CREATE PROCEDURE SP_API_HUNT_AdminMaster(IN p_IdentFlag varchar(50), IN p_Id int, IN p_AppName varchar(50), IN p_EmpCode varchar(25), IN p_UserRole varchar(50), IN p_EmailId varchar(70), IN p_AppShortName varchar(100), IN p_FullName varchar(150), IN p_Purpose varchar(500), IN p_ITGRCCode varchar(100), IN p_ITGRCName varchar(250), IN p_HostingDC varchar(250), IN p_CreatedBy varchar(25), IN p_SPOCDepartment varchar(100), IN p_SpocEMPCode varchar(25), IN p_SpocName varchar(150), IN p_SPOCLevel varchar(50), IN p_Status int)
BEGIN
  DECLARE v_AppName TEXT;
  DECLARE v_id TEXT;
IF (p_IdentFlag='SelectAll') THEN

			SELECT A.EmpCode UserID,B.EmpName UserName,Case when A.IsActive=1 then 'A'
			when A.IsActive=0 then 'I' end Status,DATE_FORMAT(A.CreatedDate, '%d-%m-%Y') DateCreated,A.Role UserRole
			from tbl_API_hunt_USER A inner join UserMaster B on A.EmpCode=b.EmpCode;

			SELECT Id,APPShortName,FullName,Purpose,CAST(ITGRCCode AS CHAR) ITGRCCode,ITGRCName,HostingDC,status 
			from tbl_API_MstApplications;

			SELECT Id,APPID,APPShortName,SPOCDepartment,SpocEMPCode,SPOCName,EmailAddress,SPOCLevel,status from 
			tbl_API_ApplicationsSPOC;
			
			SELECT '-- Select --' as ID, '-- Select --' as val
			Union
			SELECT CDValDesc AS ID, CDValDesc AS VAL FROM tbl_API_hunt_Misccd WHERE CDTP = 'Hosting DC fields';

			-- select '-- Select --' as ID, '-- Select --' as val
			-- Union
			-- select 'Bank DC' as ID, 'Bank DC' as val
			-- Union
			-- select 'Bank Landing Zone' as ID, 'Bank Landing Zone' as val
			-- Union
			-- select 'External' as ID, 'External' as val

			SELECT '-- Select --' as ID, '-- Select --' as val
			Union
			SELECT 'USER' as ID, 'USER' as val
			Union
			SELECT Distinct Role as ID,Role as  val from tbl_API_hunt_USER;
			
			-- SELECT DISTINCT SUBSTRING(Role,1,Len(Role)-4) AS ID,  SUBSTRING(Role,1,Len(Role)-4) AS Role from tbl_API_hunt_USER WHERE Role IN ('BTGUSER', 'ITUSER')

			SELECT '-- Select --' as ID, '-- Select --' as val
			Union
			-- select 'ADMIN' as ID, 'ADMIN' as val
			-- Union
			SELECT 'IT' as ID, 'IT' as val
			Union
			SELECT 'BTG' as ID, 'BTG' as val order by ID;
			-- Union
			-- select 'ITARCHITECH' as ID, 'ITARCHITECH'
			-- Union
			-- select 'USER' as ID, 'USER' as val order by ID
			
			SELECT '-- Select --' as ID, '-- Select --' as val
			Union
			SELECT CDValDesc AS ID, CDValDesc AS VAL FROM tbl_API_hunt_Misccd WHERE CDTP = 'Status';

			-- select '-- Select --' as ID, '-- Select --' as val
			-- Union
			-- select 'Active' as ID, 'Active' as val
			-- Union
			-- select 'Inactive' as ID, 'Inactive' as val order by ID
			SELECT '-- Select --' as ID, '-- Select --' as val
			Union
			SELECT CDValDesc AS ID, CDValDesc AS VAL FROM tbl_API_hunt_Misccd WHERE CDTP = 'Spoc Level';

			-- select '-- Select --' as ID, '-- Select --' as val
			-- Union
			-- select 'Unit Head' as ID, 'Unit Head' as val
			-- Union
			-- select 'Function Head' as ID, 'Function Head' as val 
			-- Union
			-- select 'Vertical Head' as ID, 'Vertical Head' as val 
			-- Union
			-- select 'Manager' as ID, 'Manager' as val order by ID
	
END IF;
	IF (p_IdentFlag='AddUsers') THEN

			IF NOT EXISTS(select EmpCode from tbl_API_hunt_USER where EmpCode=p_EmpCode) THEN

				insert into tbl_API_hunt_USER(EmpCode,Role,EmailId,CreatedBy,CreatedDate,IsActive)
									values(p_EmpCode,p_UserRole,p_EmailId,p_CreatedBy,NOW(),p_Status);
					select CONCAT('User Id ', p_EmpCode, ' has been added successfully.') as Msg;
			
ELSE

					select CONCAT('User Id ', p_EmpCode, ' already exist.') as Msg;
			
END IF;
	
END IF;
	IF (p_IdentFlag='AddApp') THEN

				IF EXISTS(select APPShortName from tbl_API_MstApplications where APPShortName=p_AppShortName) THEN
			
					select CONCAT('Application ', p_AppShortName, ' already exists.') as Msg,v_id as Id;
				
ELSEIF EXISTS(select FullName from tbl_API_MstApplications where FullName=p_FullName) THEN
			
					select CONCAT('Application fullname ', p_FullName, ' already exists.') as Msg,v_id as Id;
				
ELSEIF EXISTS(select ITGRCName from tbl_API_MstApplications where ITGRCName=p_ITGRCName) THEN
			
					select CONCAT('ITGRC name ', p_ITGRCName, ' already exists.') as Msg,v_id as Id;
				
ELSEIF EXISTS(select ITGRCCode from tbl_API_MstApplications where ITGRCCode=p_ITGRCCode) THEN
			
					select CONCAT('ITGRC number ', p_ITGRCCode, ' already exists.') as Msg,v_id as Id;
				
ELSE

					insert into tbl_API_MstApplications(APPShortName,FullName,Purpose,ITGRCName,ITGRCCode,HostingDC,
					status,CreatedBy,CreatedDate) values(p_AppShortName,p_FullName,p_Purpose,p_ITGRCName,p_ITGRCCode,p_HostingDC,
					p_Status,p_CreatedBy,NOW());
				    SET v_id =  LAST_INSERT_ID();
					select CONCAT('Application ', p_AppShortName, ' has been added successfully.') as Msg,v_id as Id;
				
END IF;

			-- IF NOT EXISTS(select APPShortName from tbl_API_MstApplications where APPShortName=@AppShortName)
			-- begin
			-- 		insert into tbl_API_MstApplications(APPShortName,FullName,Purpose,ITGRCName,ITGRCCode,HostingDC,
			-- 		status,CreatedBy,CreatedDate)
			-- 								values(@AppShortName,@FullName,@Purpose,@ITGRCName,@ITGRCCode,@HostingDC,
			-- 		@Status,@CreatedBy,getdate())
			-- 	    set @id =  SCOPE_IDENTITY()
			-- 				select 'Application ' + @AppShortName + ' has been added successfully.' as Msg,@id as Id
			-- end
			-- else
			-- begin			
			-- 		select 'Application ' + @AppShortName + ' already exists.' as Msg,@id as Id
			-- end
	
END IF;
	IF (p_IdentFlag='UpdateApps') THEN

			update tbl_API_MstApplications set  APPShortName=p_AppShortName,FullName=p_FullName,Purpose=p_Purpose,ITGRCCode=p_ITGRCCode,
			ITGRCName=p_ITGRCName,HostingDC=p_HostingDC,status=p_Status,UpdateBy=p_CreatedBy,UpdatedDate=NOW()
			where Id=p_Id;
					select CONCAT('Application ', p_AppShortName, ' has been updated successfully.') as Msg;
	
END IF;
	IF (p_IdentFlag='UpdateUsers') THEN

			update tbl_API_hunt_USER set Role=p_UserRole,EmailId=p_EmailId,UpdateBy=p_CreatedBy,
			UpdatedDate=NOW(),IsActive=p_Status where EmpCode=p_EmpCode;
			select CONCAT('User Id ', p_EmpCode, ' has been updated successfully.') as Msg;
	
	
END IF;
	IF (p_IdentFlag='EditUser') THEN

			select A.EmpCode,B.EmpName,Role,EmailId,isActive Status, DATE_FORMAT(CreatedDate, '%d-%m-%Y')CreatedDate,
			 DATE_FORMAT(UpdatedDate, '%d-%m-%Y')UpdatedDate,DATE_FORMAT(LastSuccessLoginDate, '%d-%m-%Y')LastLoginDate
			 from tbl_API_hunt_USER A inner join UserMaster B on A.EmpCode=B.EmpCode where A.EmpCode=p_EmpCode;
	
END IF;
	IF (p_IdentFlag='EditApp' ) THEN

	       SELECT (select case when p_AppName Like '%#%' then SUBSTRING(p_AppName, 1,LOCATE('#', p_AppName)-1) else p_AppName end) INTO v_AppName;
		   IF (p_AppName !='' AND p_Id is  null) THEN

		    select Id,APPShortName,FullName,Purpose,ITGRCCode,ITGRCName,HostingDC,status from tbl_API_MstApplications where (FullName= p_AppName OR APPShortName=p_AppName);
			select APPID,APPShortName,SPOCDepartment,SpocEMPCode,SPOCName,EmailAddress,SPOCLevel,status from tbl_API_ApplicationsSPOC where APPID=( select Id from tbl_API_MstApplications where( FullName= p_AppName OR APPShortName=p_AppName));
		   
ELSE

			select Id,APPShortName,FullName,Purpose,ITGRCCode,ITGRCName,HostingDC,status from tbl_API_MstApplications where Id= p_Id;
			select APPID,APPShortName,SPOCDepartment,SpocEMPCode,SPOCName,EmailAddress,SPOCLevel,status from tbl_API_ApplicationsSPOC where APPID=p_Id;
			
END IF;
	
END IF;
	IF (p_IdentFlag='GetName') THEN

			-- select EmpName UserName, 'Emailid' as Email from UserMaster where EmpCode=@EmpCode
			-- SELECT DISTINCT C.EmpName AS UserName, B.EmailId as Email from tbl_API_ApplicationsSPOC A
			-- INNER JOIN tbl_API_hunt_USER B ON A.SpocEMPCode = B.EmpCode 
			-- INNER JOIN UserMaster C ON C.EmpCode = A.SpocEMPCode where C.EmpCode=@EmpCode
		SELECT DISTINCT A.EmpName AS UserName, B.EmailId as Email 
			FROM UserMaster A 
			Left JOIN tbl_API_hunt_USER B ON A.EmpCode = B.EmpCode  where A.EmpCode=p_EmpCode;
	
END IF;
	IF (p_IdentFlag='AddSpocs') THEN

			insert into tbl_API_ApplicationsSPOC (APPID,APPShortName,SPOCDepartment,SpocEMPCode,SPOCName,EmailAddress,
			SPOCLevel,status,CreatedBy,CreatedDate) values(p_Id,p_AppShortName,p_SPOCDepartment,p_SpocEMPCode,p_SpocName,
			p_EmailId,p_SPOCLevel,1,p_CreatedBy,NOW());
	
	
END IF;
	IF (p_IdentFlag='DeleteSpoc') THEN

			delete from tbl_API_ApplicationsSPOC where APPID=p_Id;
	
	
END IF;

	IF (p_IdentFlag='SearchApplicatonList') THEN

	
	select APPShortName,FullName from tbl_API_MstApplications where FullName like CONCAT('%', p_FullName, '%') and status=1;
	
	
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_API_HUNT_APIFilterSearch;
DELIMITER //
CREATE PROCEDURE sp_API_HUNT_APIFilterSearch(IN p_whereClause LONGTEXT)
BEGIN
  DECLARE v_query LONGTEXT DEFAULT '';
  DECLARE v_query1 LONGTEXT DEFAULT '';

    
  
   SET v_query =' select A.TBL_API_Main_ID,A.PRODUCT_PROCESSOR_URL_UAT  as ServiceURL,A.SERVICE_DESC,SERVICE_MIDDLEWARE.Value as SERVICE_TYPE,A.SERVICE_INTERFACE_TYPE,A.SERVICE_CATEGORY  
 ,A.COD_SERVICE_ID,A.SERVICE_SIGNATURE,  
 SERVICE_PROVIDER.Value as SERVICE_PROVIDER  
 ,A.PRODUCT_PROCESSOR_WSDL , DOMAIN_NAME.Value as DOMAIN_NAME,A.fileName,A.NAM_SERVICE_MIDDLEWARE  
    INTO #tmp_DynamicTable  from TBL_API_Main A  
 left join TBL_API_Master_Values SERVICE_MIDDLEWARE  
 on A.NAM_SERVICE_MIDDLEWARE = SERVICE_MIDDLEWARE.Id And SERVICE_MIDDLEWARE.API_Master_ID=1  
 left join TBL_API_Master_Values SERVICE_INTERFACE_TYPE   
 on a.SERVICE_INTERFACE_TYPE = SERVICE_INTERFACE_TYPE.Id and SERVICE_INTERFACE_TYPE.API_Master_ID=2  
 left join TBL_API_Master_Values SERVICE_CATEGORY   
 on a.SERVICE_CATEGORY = SERVICE_CATEGORY.Id and SERVICE_CATEGORY.API_Master_ID=3  
 left join TBL_API_Master_Values SERVICE_PROVIDER   
 on a.SERVICE_PROVIDER = SERVICE_PROVIDER.Id and SERVICE_PROVIDER.API_Master_ID=4  
  
 left join TBL_API_Master_Values SERVICE_TYPE   
 on a.SERVICE_TYPE = SERVICE_TYPE.Id and SERVICE_TYPE.API_Master_ID=5  
  
 left join TBL_API_Master_Values NAM_CONTAINER   
 on a.NAM_CONTAINER = NAM_CONTAINER.Id and NAM_CONTAINER.API_Master_ID=6  
  
 left join TBL_API_Master_Values NAM_DOMAIN   
 on a.NAM_DOMAIN = NAM_DOMAIN.Id and NAM_DOMAIN.API_Master_ID=7  
  
 left join TBL_API_Master_Values DOMAIN_NAME   
 on a.DOMAIN_NAME = DOMAIN_NAME.Id and DOMAIN_NAME.API_Master_ID=8;
      
  select * into #tmp_finalTable from  #tmp_DynamicTable ';
   SET v_query = CONCAT(v_query, ' Select * From #tmp_finalTable where NAM_SERVICE_MIDDLEWARE in (''', p_whereClause, ''');
      Drop table #tmp_DynamicTable ; Drop table #tmp_finalTable ;');

 SET @_dyn_sql = v_query;
PREPARE _dyn_stmt FROM @_dyn_sql;
EXECUTE _dyn_stmt;
DEALLOCATE PREPARE _dyn_stmt;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_API_HUNT_APIFilterSearchF;
DELIMITER //
CREATE PROCEDURE sp_API_HUNT_APIFilterSearchF(IN p_whereClause LONGTEXT)
BEGIN
  DECLARE v_query LONGTEXT DEFAULT '';
  DECLARE v_query1 LONGTEXT DEFAULT '';
CREATE TEMPORARY TABLE tmp_PAR
(id varchar(10));
-- INSERT INTO tmp_PAR SELECT Item FROM  SplitString(p_whereClause, ','); -- VALUES (@whereClause)

INSERT INTO tmp_PAR
WITH RECURSIVE SplitString AS (
    SELECT 
        TRIM(SUBSTRING_INDEX(p_whereClause, ',', 1)) AS Item,
        IF(LOCATE(',', p_whereClause) > 0, 
           SUBSTRING(p_whereClause, LOCATE(',', p_whereClause) + 1), 
           NULL) AS Remaining
    UNION ALL
    SELECT 
        TRIM(SUBSTRING_INDEX(Remaining, ',', 1)),
        IF(LOCATE(',', Remaining) > 0, 
           SUBSTRING(Remaining, LOCATE(',', Remaining) + 1), 
           NULL)
    FROM SplitString
    WHERE Remaining IS NOT NULL
)
SELECT Item FROM SplitString;
 
  SET v_query ='select 
 distinct
 A.TBL_API_Main_ID,
 A.PRODUCT_PROCESSOR_URL_UAT  as ServiceURL,
 A.SERVICE_DESC,
 SERVICE_MIDDLEWARE.Value as SERVICE_TYPE,
 A.SERVICE_INTERFACE_TYPE,
 A.SERVICE_CATEGORY  
,A.COD_SERVICE_ID,
 A.SERVICE_SIGNATURE,  
 A.SERVICE_PROVIDER as SERVICE_PROVIDER,  
 A.PRODUCT_PROCESSOR_WSDL , 
 A.DOMAIN_NAME as DOMAIN_NAME,
 A.fileName,
 A.NAM_SERVICE_MIDDLEWARE  
 from TBL_API_Main A Left Join  TBL_API_Master_Values SERVICE_MIDDLEWARE   on A.NAM_SERVICE_MIDDLEWARE = SERVICE_MIDDLEWARE.Value -- And SERVICE_MIDDLEWARE.API_Master_ID=1

 where SERVICE_MIDDLEWARE.Value in ( select id from #PAR  ) ';

 SET @_dyn_sql = v_query;
PREPARE _dyn_stmt FROM @_dyn_sql;
EXECUTE _dyn_stmt;
DEALLOCATE PREPARE _dyn_stmt;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_API_HUNT_ApplicationMaster;
DELIMITER //
CREATE PROCEDURE SP_API_HUNT_ApplicationMaster(IN p_IdentFlag varchar(50), IN p_EmpCode varchar(25), IN p_UserRole varchar(50), IN p_EmailId varchar(70), IN p_Status TINYINT(1))
BEGIN
IF (p_IdentFlag='SelectAll') THEN

			select A.EmpCode UserID,B.EmpName UserName,Case when A.IsActive=1 then 'A'
			when A.IsActive=0 then 'I' end Status,DATE_FORMAT(A.CreatedDate, '%d-%m-%Y') DateCreated,A.Role UserRole
			from tbl_API_hunt_USER A inner join UserMaster B on A.EmpCode=b.EmpCode;
	
END IF;
	IF (p_IdentFlag='AddUsers') THEN

			insert into tbl_API_hunt_USER(EmpCode,Role,EmailId,IsActive)values(p_EmpCode,p_UserRole,p_EmailId,p_Status);
	
END IF;
	IF (p_IdentFlag='UpdateUsers') THEN

			update tbl_API_hunt_USER set Role=p_UserRole,EmailId=p_EmailId,IsActive=p_Status where EmpCode=p_EmpCode;
	
END IF;
	IF (p_IdentFlag='DeleteUsers') THEN

			update tbl_API_hunt_USER set  Isactive=p_Status where EmpCode=p_EmpCode;
	
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_API_HUNT_BulkInsert_Scheduler_Data;
DELIMITER //
CREATE PROCEDURE SP_API_HUNT_BulkInsert_Scheduler_Data()
BEGIN
truncate table TBL_API_Main_Bak;    
-- select * into TBL_API_Main_Bak from TBL_API_Main;    
INSERT INTO  TBL_API_Main_Bak  select * from  TBL_API_Main;

truncate table TBL_API_Main;    
 Insert Into TBL_API_Main (    
 COD_SERVICE_ID    
,SERVICE_SIGNATURE    
,NAM_SERVICE_MIDDLEWARE    
,SERVICE_DESC    
,SERVICE_PROVIDER    
,SERVICE_INTERFACE_TYPE    
,SERVICE_CATEGORY    
,SERVICE_TYPE    
,ORCHESTRATED_SERVICE_DTLS    
,OBP_SERVICE_URL_UAT    
,OBP_SERVICE_URL_PSUPP    
,OBP_SERVICE_URL_PROD    
,OBP_WSDL_URL    
,PRODUCT_PROCESSOR_WSDL    
,SERVICE_VERSION    
,NAM_CONTAINER    
,NAM_DOMAIN    
,OBP_REQUEST    
,OBP_RESPONSE    
,PRODUCT_PROCESSOR_URL_UAT    
,PRODUCT_PROCESSOR_URL_PSUPP    
,PRODUCT_PROCESSOR_URL_PROD    
,TXT_ERROR_DESC    
,CONNECT_TIMEOUT_VALUE    
,SOCKET_TIMEOUT_VALUE    
,NAM_WORKMANAGER    
,WORKMANAGER_MAX_THREADS    
,WORKMANAGER_CAPACITY    
,TXT_SECURITY_FEATURE_DESC    
,SERVICE_DOC_PATH    
,NAM_INITIATOR    
,DAT_GO_LIVE    
,JIRA_ID    
,TXT_REMARKS    
,FILLER_01    
,FILLER_02    
,FILLER_03    
,FILLER_04    
,FILLER_05    
,FILLER_06    
,FILLER_07    
,FILLER_08    
,FILLER_09    
,FILLER_10    
,FLG_MNT_STATUS    
,COD_MNT_ACTION    
,COD_LAST_MNT_MAKERID    
,COD_LAST_MNT_CHKRID    
,DAT_LAST_MNT    
,CTR_UPDAT_SRLNO    
,API_CAT    
,VIRTUALIZED    
,AUTOMATED    
,DEPRICATED_API    
,REQUEST_SAMPLE    
,RESPONSE_SAMPLE    
,DOC_TYPE    
,DOMAIN_NAME    
,API_TYPE) select COD_SERVICE_ID    
,SERVICE_SIGNATURE    
,NAM_SERVICE_MIDDLEWARE    
,SERVICE_DESC    
,SERVICE_PROVIDER    
,SERVICE_INTERFACE_TYPE    
,SERVICE_CATEGORY    
,SERVICE_TYPE    
,ORCHESTRATED_SERVICE_DTLS    
,OBP_SERVICE_URL_UAT    
,OBP_SERVICE_URL_PSUPP    
,OBP_SERVICE_URL_PROD    
,OBP_WSDL_URL    
,PRODUCT_PROCESSOR_WSDL    
,SERVICE_VERSION    
,NAM_CONTAINER    
,NAM_DOMAIN    
,OBP_REQUEST    
,OBP_RESPONSE    
,PRODUCT_PROCESSOR_URL_UAT    
,PRODUCT_PROCESSOR_URL_PSUPP    
,PRODUCT_PROCESSOR_URL_PROD    
,TXT_ERROR_DESC    
,CONNECT_TIMEOUT_VALUE    
,SOCKET_TIMEOUT_VALUE    
,NAM_WORKMANAGER    
,WORKMANAGER_MAX_THREADS    
,WORKMANAGER_CAPACITY    
,TXT_SECURITY_FEATURE_DESC    
,SERVICE_DOC_PATH    
,NAM_INITIATOR    
,DAT_GO_LIVE    
,JIRA_ID    
,TXT_REMARKS    
,FILLER_01    
,FILLER_02    
,FILLER_03    
,FILLER_04    
,FILLER_05    
,FILLER_06    
,FILLER_07    
,FILLER_08    
,FILLER_09    
,FILLER_10    
,FLG_MNT_STATUS    
,COD_MNT_ACTION    
,COD_LAST_MNT_MAKERID    
,COD_LAST_MNT_CHKRID    
,DAT_LAST_MNT    
,CTR_UPDAT_SRLNO    
,API_CAT    
,VIRTUALIZED    
,AUTOMATED    
,DEPRICATED_API    
,REQUEST_SAMPLE    
,RESPONSE_SAMPLE    
,DOC_TYPE    
,DOMAIN_NAME    
,API_TYPE from TBL_API_Main_Scheduler_Data;    
truncate table TBL_API_Main_Scheduler_Data;    
select 1 as Msg;
END //
DELIMITER ;

-- ------------------------------------------------------------
-- ------------------------------------------------------------

DROP PROCEDURE IF EXISTS SP_API_HUNT_GetUserRole;
DELIMITER //
CREATE PROCEDURE SP_API_HUNT_GetUserRole(IN p_UserId varchar(50))
BEGIN
select Role from tbl_API_hunt_USER where EmpCode=p_UserId and IsActive=1;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_API_HUNT_NewAPIIntegration;  -- Failed
DELIMITER //
CREATE PROCEDURE sp_API_HUNT_NewAPIIntegration(IN p_IntegrationId int, IN p_ProjectManagerBTG varchar(100), IN p_ProjectManagerIT varchar(100), IN p_ProjectName varchar(100), IN p_ProjectId varchar(100), IN p_PlannedGoLiveDate datetime, IN p_BusinessJustification LONGTEXT, IN p_BusinessSponsor varchar(100), IN p_ExecutiveSponsor varchar(100), IN p_CostCenterCode varchar(100), IN p_UserJourneyDocumentFileName varchar(100), IN p_Feedback varchar(500), IN p_Status int, IN p_CreatedBy varchar(100), IN p_UpdatedBy varchar(100), IN p_IdentFlag varchar(50), IN p_ServiceName varchar(100), IN p_Purpose varchar(100), IN p_Existing_New varchar(100), IN p_ConsumerApplication varchar(100), IN p_ProducerApplication varchar(100), IN p_Is_APIGW_Request TINYINT(1), IN p_Rest_Soap varchar(100), IN p_Transformation varchar(100), IN p_Volume varchar(100), IN p_UpdateFlag varchar(100), IN p_ServiceID varchar(100), IN p_Assign Varchar(50), IN p_AssignFrom Varchar(50), IN p_Existing_New_Id int, IN p_Rest_SOAP_Id int, IN p_ServiceType_Id int, IN p_APIType_Id int, IN p_APICategory_Id int, IN p_APIRiskScore_Id int, IN p_PartnerRiskScore_Id int, IN p_DomainName_Id int, IN p_IsActive TINYINT(1), IN p_FeedbackId int, IN p_servicenameId int, IN p_ConsumerDC_Id int, IN p_ProducerDC_Id int, IN p_Platform varchar(200), IN p_QValue1 int, IN p_QValue2 int, IN p_QValue3 int, IN p_QValue4 int, IN p_QValue5 int, IN p_RiskScore FLOAT, IN p_Classification varchar(10), IN p_BTGUSER VARCHAR(50), IN p_ITUSER VARCHAR(50), IN p_ITARCHITECTURE VARCHAR(50), IN p_ChannelID VARCHAR(50), IN p_ContainerName VARCHAR(50), IN p_InternalServiceName VARCHAR(50), IN p_ExternalServiceName VARCHAR(50), IN p_ExternalServiceText LONGTEXT, IN p_HostAppID int, IN p_ConsumerDC VARCHAR(50), IN p_ProducerDC VARCHAR(50), IN p_HostApptext VARCHAR(50), IN p_QID int, IN p_OptionsID int, IN p_RDConceptNoteFileName varchar(100), IN p_ExpectedAPISpecificationFileName varchar(100), IN p_SequenceDiagramFileName varchar(100), IN p_ConsumerId INT, IN p_flagPl varchar(10), IN p_ConsumerApplicationId INT)
BEGIN
  DECLARE v_NewID INT DEFAULT 0;
  DECLARE v_InternalCountS int;
  DECLARE v_ExternalCountS int;
  DECLARE v_InternalExternalCountS int;
  DECLARE v_IntegrationIdIE int;
  DECLARE v_StatusNew VARCHAR(100) DEFAULT NULL;
  DECLARE v_APICategory_IdOld VARCHAR(100) DEFAULT NULL;
  DECLARE v_InternalServiceId LONGTEXT;
  DECLARE v_ExternalServiceId LONGTEXT;
  DECLARE v_ExternalCodServiceId LONGTEXT;
  DECLARE v_CheckExternalService int;
  DECLARE v_QueServiceID int;
  DECLARE v_IntegrationId VARCHAR(100) DEFAULT '1485';
  DECLARE v_SVCId VARCHAR(100) DEFAULT '1615';
  DECLARE v_InternalCount int;
  DECLARE v_ExternalCount int;
  DECLARE v_InternalExternalCount int;
  DECLARE v_IntegrationIdIED int;
  DECLARE v_InternalCountID int;
  DECLARE v_ExternalCountID int;
  DECLARE v_InternalExternalCountID int;
  DECLARE v_IntegrationIdIEDE int;
  DECLARE v_CreatedBy TEXT;
  DECLARE v_Feedback TEXT;
  DECLARE v_HostApptext TEXT;
  DECLARE v_flagPl TEXT;
IF (p_IdentFlag='AddNewIntegration') THEN
    
  SELECT (SELECT MAX(IntegrationId)+1 FROM TBL_API_HUNT_Integration) INTO v_NewID;

  INSERT INTO TBL_API_HUNT_Integration(ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,            
  BusinessJustification,BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,CreatedAt,CreatedBy,Assign,AssignFrom, ConsumerApplication,RDConceptNote,ConsumerApplicationId,IntegrationId)            
  VALUES(p_ProjectManagerBTG,p_ProjectManagerIT,p_ProjectName,p_ProjectId,p_PlannedGoLiveDate,p_BusinessJustification,            
  p_BusinessSponsor,p_ExecutiveSponsor,p_CostCenterCode,p_UserJourneyDocumentFileName,p_Status,NOW(),p_CreatedBy,p_Assign,p_AssignFrom, p_ConsumerApplication,p_RDConceptNoteFileName,p_ConsumerApplicationId,v_NewID);
SELECT (SELECT LAST_INSERT_ID() AS IntegrationId) INTO v_IntegrationId;
  SET v_Feedback = 'New Integration  Created  By User';
  IF (p_IntegrationId <> 0  or p_IntegrationId <> null) THEN

        INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom)     
      VALUES(p_IntegrationId,p_Feedback,p_Status,p_CreatedBy,NOW(),p_IsActive,p_Assign,p_AssignFrom) ;
  
END IF;
 
   
      
SELECT p_IntegrationId AS IntegrationId;    
    
 -- SELECT SCOPE_IDENTITY() AS IntegrationId;    
 -- SELECT MAX(IntegrationId) AS IntegrationId FROM TBL_API_HUNT_Integration          
          
  
ELSEIF p_IdentFlag='BTGProjectMgr' THEN

		SELECT IFNULL(EmpName,'') AS ProjectManagerBTG,IFNULL(EmpCode,'') AS IntegrationId  FROM UserMaster;
	
ELSEIF (p_IdentFlag='GetNewIntegrationDetails') THEN
          
        
  IF (p_AssignFrom='USER') THEN
         
  Select   
  IntegrationId,    
  CONCAT('APIHUNT', (CAST(CreatedAt AS CHAR) ,' ',''), Cast(IntegrationId AS CHAR))  as 'APIHUNTID',    
  ProjectName,    
  ProjectId,    
  ProjectManagerBTG,    
  ProjectManagerIT,     
  statusDescription as Status,    
  Assign,    
  AssignFrom,    
  Status as workflowstatus ,    
  IFNULL(In_Platform,'') as 'Platform'

 -- (    
 --  case      
 --       when Platform ='External,Internal' Then  'External & Internal'    
 --       when Platform like'%Internal &amp; External%' Then  'External & Internal'    
 -- else Platform end    
    
 --  ) as 'Platform'    
  -- CASE         
  -- WHEN Status=0 THEN 'Created'        
  -- WHEN Status=1 THEN 'Feedback'        
  -- WHEN Status=2 THEN 'Updated By User'        
  -- WHEN Status=3 THEN 'Review To ITUSER'        
  -- WHEN Status=4 THEN 'Review To ITARCHITECH'        
  -- WHEN Status=5 THEN 'Rejected'        
  -- WHEN Status=6 THEN 'Approved'        
  -- END AS Status          
  from TBL_API_HUNT_Integration    
   Inner Join TBL_API_statusMaster As SM on SM.statusCode=Status    
   -- Inner Join (    
   --          select cu.IntegrationId as InId,    
   --        stuff( (select ',' + co.Platform    
   --            from (select DISTINCT  IntegrationId,Platform from   TBL_API_HUNT_ServiceDetails) co    
   --            where cu.IntegrationId = co.IntegrationId    
   --            order by co.IntegrationId    
   --            for xml path ('')    
   --           ), 1, 1, ''    
   --         ) as Platform    
   --     from (select DISTINCT  IntegrationId,Platform from   TBL_API_HUNT_ServiceDetails)As cu group by cu.IntegrationId    
   -- ) As SdPlatfrom on SdPlatfrom.InId=IntegrationId    
    
  where CreatedBy=p_CreatedBy          
  ORDER BY IntegrationId DESC;      
  
ELSEIF (p_AssignFrom<>'USER') THEN
         
  Select     
  IntegrationId,    
  CONCAT('API', (CAST(CreatedAt AS CHAR) ,' ',''), Cast(IntegrationId AS CHAR) ) as 'APIHUNTID',    
  ProjectName,    
  ProjectId,    
  ProjectManagerBTG,    
  ProjectManagerIT,      
  statusDescription as Status,    
  Assign,    
  AssignFrom, 
  Status as workflowstatus,    
   IFNULL(In_Platform,'') as 'Platform'
 -- (    
 --  case      
 --       when Platform ='External,Internal' Then  'External & Internal'    
 --       when Platform like'%Internal &amp; External%' Then  'External & Internal'    
 -- else Platform end    
    
 --  ) as 'Platform'    
-- CASE         
  -- WHEN Status=0 THEN 'Created'        
  -- WHEN Status=1 THEN 'Feedback'        
  -- WHEN Status=2 THEN 'Updated By User'        
  -- WHEN Status=3 THEN 'Review To ITUSER'        
  -- WHEN Status=4 THEN 'Review To ITARCHITECH'        
  -- WHEN Status=5 THEN 'Rejected'        
  -- WHEN Status=6 THEN 'Approved'        
  -- END AS Status          
  from TBL_API_HUNT_Integration     
   Inner Join TBL_API_statusMaster As SM on SM.statusCode=Status    
   -- Inner Join (    
   --           select cu.IntegrationId as InId,    
   --         stuff( (select ',' + co.Platform    
   --            from (select DISTINCT  IntegrationId,Platform from   TBL_API_HUNT_ServiceDetails) co    
   --            where cu.IntegrationId = co.IntegrationId    
   --            order by co.IntegrationId    
   --            for xml path ('')    
   --           ), 1, 1, ''    
   --         ) as Platform    
   --     from (select DISTINCT  IntegrationId,Platform from   TBL_API_HUNT_ServiceDetails)As cu group by cu.IntegrationId    
   -- ) As SdPlatfrom on SdPlatfrom.InId=IntegrationId    
 -- where Assign=@AssignFrom     
  order by IntegrationId desc;  
    
    
  
END IF;        
        
 
ELSEIF (p_IdentFlag='AddServiceDetails') THEN
  
  


	  -- If( @Platform  !='External')
	  -- BEGIN
   
		
IF (p_flagPl='All') THEN

              UPDATE TBL_API_HUNT_Integration set In_Platform= 'Internal' WHERE IntegrationId=p_IntegrationId;
			
ELSEIF (p_flagPl='single' and p_Platform='Internal & External') THEN
 
      UPDATE TBL_API_HUNT_Integration set In_Platform= 'Internal' WHERE IntegrationId=p_IntegrationId;
	  SET v_flagPl ='All';
  
ELSE

	UPDATE TBL_API_HUNT_Integration set In_Platform= p_Platform WHERE IntegrationId=p_IntegrationId;

END IF;
      -- IF(@Purpose!=null and @Purpose!='')
		-- Begin
			INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
			 ,Existing_New_Id      
			,Rest_SOAP_Id      
			,ServiceType_Id      
			,APIType_Id      
			,APICategory_Id      
			,APIRiskScore_Id      
			,PartnerRiskScore_Id      
			,DomainName_Id,     
			ConsumerDC_Id,    
			ProducerDC_Id,    
			Platform,    
			QValue1,    
			QValue2,    
			QValue3,    
			QValue4,    
			QValue5,    
			RiskScore,
			InternalServiceName,
			ExternalServiceName ,
			ConsumerDC,
			ProducerDC,
			ExpectedServiceSpecificationDocument,
			Classification
			  )          
      
      
			values(      
			p_IntegrationId,p_ServiceName,p_Purpose,p_Existing_New,p_ConsumerApplication,p_ProducerApplication,p_Is_APIGW_Request,p_Rest_Soap,p_Transformation,      
			p_Volume,      
			p_CreatedBy      
			,NOW()      
			,p_Existing_New_Id      
			,p_Rest_SOAP_Id      
			,p_ServiceType_Id      
			,p_APIType_Id      
			,p_APICategory_Id      
			,p_APIRiskScore_Id      
			,p_PartnerRiskScore_Id     
			,p_DomainName_Id,    
			p_ConsumerDC_Id,    
			p_ProducerDC_Id,    
			p_Platform,    
			p_QValue1,    
			p_QValue2,    
			p_QValue3,    
			p_QValue4,    
			p_QValue5,    
			p_RiskScore,
			p_InternalServiceName,
			p_ExternalServiceName ,
			p_ConsumerDC,
			p_ProducerDC,
			p_ExpectedAPISpecificationFileName,
			p_Classification
			);
		-- End
	--  END

 --
 --
 --

 -- SET @InternalCountS = (select count(Platform) from TBL_API_HUNT_ServiceDetails where Platform='Internal' and IntegrationId=@IntegrationId)
 -- SET @ExternalCountS = (select count(Platform) from TBL_API_HUNT_ServiceDetails where Platform='External' and IntegrationId=@IntegrationId)
 -- SET @InternalExternalCountS = (select count(Platform) from TBL_API_HUNT_ServiceDetails where Platform='Internal & External' and IntegrationId=@IntegrationId)


 IF (p_flagPl='All')--((@InternalCountS>0 AND @ExternalCountS>0) OR (@InternalCountS>0 AND @InternalExternalCountS>0) OR (@ExternalCountS>0 AND @InternalExternalCountS>0) ) THEN

  IF (p_Platform ='Internal & External' Or p_Platform ='External' Or  p_Platform ='Internal') THEN

	  
 
	  -- ----------------------------------------------------------------- Add New Integration----Based On Internal & External--------------------------------------------------------------------------
 
	 IF ((select Count(Parent_IntegrationId) from TBL_API_HUNT_Integration where Parent_IntegrationId=p_IntegrationId)=0) THEN

		 INSERT INTO TBL_API_HUNT_Integration(ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,            
		 BusinessJustification,BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,CreatedAt,CreatedBy,Assign,AssignFrom, ConsumerApplication,Parent_IntegrationId,In_Platform,RDConceptNote)
		  select ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,BusinessJustification,            
		  BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,NOW(),CreatedBy,Assign,AssignFrom, ConsumerApplication,p_IntegrationId,'External',RDConceptNote  From TBL_API_HUNT_Integration  where   IntegrationId=p_IntegrationId;

	SELECT (SELECT LAST_INSERT_ID() AS IntegrationId) INTO v_IntegrationIdIE;


	  IF (v_IntegrationIdIE <> 0  or v_IntegrationIdIE <> null) THEN

	    SET v_Feedback = 'New Integration  Created  By User';

        INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom)     
      VALUES(v_IntegrationIdIE,p_Feedback,1,p_CreatedBy,NOW(),p_IsActive,'BTGUSER','USER') ;
    
END IF;

	  
ELSE

    SELECT (SELECT IntegrationId from TBL_API_HUNT_Integration where Parent_IntegrationId=p_IntegrationId and Parent_IntegrationId is not null) INTO v_IntegrationIdIE;
  
END IF; 

  -- -------------------------------------------Add Service-------Based On Internal & External---------------------------------------------------
-- IF(@Purpose!=null and @Purpose!='')
--	Begin
		 INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
		,Existing_New_Id      
		,Rest_SOAP_Id      
		,ServiceType_Id      
		,APIType_Id      
		,APICategory_Id      
		,APIRiskScore_Id      
		,PartnerRiskScore_Id      
		,DomainName_Id,     
		ConsumerDC_Id,    
		ProducerDC_Id,    
		Platform,    
		QValue1,    
		QValue2,    
		QValue3,    
		QValue4,    
		QValue5,    
		RiskScore,
		InternalServiceName,
		ExternalServiceName ,
		ConsumerDC,
		ProducerDC,
		ExpectedServiceSpecificationDocument,
		Classification
		  )          
      
		values(      
		v_IntegrationIdIE,p_ServiceName,p_Purpose,p_Existing_New,p_ConsumerApplication,p_ProducerApplication,p_Is_APIGW_Request,p_Rest_Soap,p_Transformation,      
		p_Volume,      
		p_CreatedBy
		,NOW()    
		,p_Existing_New_Id      
		,p_Rest_SOAP_Id      
		,p_ServiceType_Id      
		,p_APIType_Id      
		,p_APICategory_Id      
		,p_APIRiskScore_Id      
		,p_PartnerRiskScore_Id     
		,p_DomainName_Id,    
		p_ConsumerDC_Id,    
		p_ProducerDC_Id,    
		p_Platform,    
		p_QValue1,    
		p_QValue2,    
		p_QValue3,    
		p_QValue4,    
		p_QValue5,    
		p_RiskScore,
		p_InternalServiceName,
		p_ExternalServiceName ,
		p_ConsumerDC,
		p_ProducerDC,
		p_ExpectedAPISpecificationFileName,
		p_Classification
		);
	-- End

  
END IF;

END IF;
  

ELSEIF (p_IdentFlag='GetNewIntegrationDetailsById') THEN
          
    Select IntegrationId,ProjectName,ProjectId,ProjectManagerBTG,ProjectManagerIT,Status as workflowstatus, statusDescription as Status,Assign, AssignFrom,ConsumerApplication,BTG_USER,IT_USER,IT_ARCHITECTURE,ChannelID,ContainerName        
    ,In_Platform AS IN_Platform,ConsumerApplicationId,IFNULL(Parent_IntegrationId,0) as ParentIntegrationId,CreatedAt
           
  -- CASE         
  -- WHEN Status=0 THEN 'Created'        
  -- WHEN Status=1 THEN 'Feedback'        
  -- WHEN Status=2 THEN 'Updated By User'        
  -- WHEN Status=3 THEN 'Review To ITUSER'        
  -- WHEN Status=4 THEN 'Review To ITARCHITECH'        
  -- WHEN Status=5 THEN 'Rejected'        
  -- WHEN Status=6 THEN 'Approved'        
  -- END AS Status          
  from TBL_API_HUNT_Integration     
    Inner Join TBL_API_statusMaster As SM on SM.statusCode=Status    
  where IntegrationId=p_IntegrationId          
  order by IntegrationId desc;
        
        
        
SELECT 
    IntegrationId,
    ProjectManagerBTG,
    ProjectManagerIT,
    ProjectName,
    ProjectId,
    DATE_FORMAT(PlannedGoLiveDate, '%d-%m-%Y') AS PlannedGoLiveDate,
    BusinessJustification,
    BusinessSponsor,
    ExecutiveSponsor,
    CostCenterCode,
    UserJourneyDocumentPath,
    RDConceptNote,
    SequenceDiagram,
    Status,
    Assign,
    AssignFrom,
    ConsumerApplication,
    BTG_USER,
    IT_USER,
    IT_ARCHITECTURE,
    ChannelID,
    ContainerName,
    CreatedAt,
    Parent_IntegrationId,
    In_Platform AS IN_Platform,
    IFNULL(Parent_IntegrationId, 0) AS ParentIntegrationId
FROM
    TBL_API_HUNT_Integration
WHERE
    IntegrationId = p_IntegrationId;
          
SELECT 
    ServiceID,
    IntegrationId,
    ServiceName,
    Purpose,
    Existing_New,
    ConsumerApplication,
    ProducerApplication,
    Is_APIGW_Request,
    Rest_Soap,
    Transformation,
    Volume,
    Existing_New_Id,
    Rest_SOAP_Id,
    ServiceType_Id,
    APIType_Id,
    APICategory_Id,
    APIRiskScore_Id,
    PartnerRiskScore_Id,
    DomainName_Id,
    ConsumerDC_Id,
    ProducerDC_Id,
    Platform,
    QValue1,
    QValue2,
    QValue3,
    QValue4,
    QValue5,
    RiskScore,
    Classification,
    InternalServiceName,
    ExternalServiceName,
    ConsumerDC,
    ProducerDC,
    ExpectedServiceSpecificationDocument
FROM
    TBL_API_HUNT_ServiceDetails
WHERE
    IntegrationId = p_IntegrationId;      
          
  
ELSEIF (p_IdentFlag='UpdateIntegrationDetails') THEN
   
-- SET @Assign=(select Top 1 AssignFrom from tbl_API_hunt_Feedback where Integration_Id=@IntegrationId  order by Feedback_Id desc)        
    
  UPDATE TBL_API_HUNT_Integration SET ProjectManagerBTG=p_ProjectManagerBTG,          
  ProjectManagerIT=p_ProjectManagerIT,          
  ProjectName=p_ProjectName,          
  ProjectId=p_ProjectId,          
  PlannedGoLiveDate=p_PlannedGoLiveDate,          
  BusinessJustification=p_BusinessJustification,          
  BusinessSponsor = p_BusinessSponsor,          
  ExecutiveSponsor=p_ExecutiveSponsor,          
  CostCenterCode=p_CostCenterCode,          
  UserJourneyDocumentPath=p_UserJourneyDocumentFileName, 
  RDConceptNote=p_RDConceptNoteFileName,
  Status=p_Status,          
  UpdatedBy=p_UpdatedBy,          
  UpdatedAt=NOW(),        
  Assign=p_Assign,    
  AssignFrom=p_AssignFrom ,  
  ConsumerApplication =p_ConsumerApplication,
  BTG_USER = p_BTGUSER,
  IT_USER = p_ITUSER,
  IT_ARCHITECTURE = p_ITARCHITECTURE,  
  ChannelID =p_ChannelID ,
  ContainerName=p_ContainerName,
  SequenceDiagram = p_SequenceDiagramFileName
  where IntegrationId=p_IntegrationId;
   -- ---------Feedback table------------------------------------------------------------------------       
  IF(p_Feedback Is not null And p_IntegrationId<>0 ) THEN
     INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom)         
     VALUES(p_IntegrationId,p_Feedback,p_Status,p_UpdatedBy,NOW(),1,p_Assign,p_AssignFrom);
        
  -- Set @FeedbackId =(SELECT SCOPE_IDENTITY() AS FeedbackId)    
    
  
ELSEIF (p_IdentFlag='UpdateServiceDetails') THEN
       
	  
	  SELECT (SELECT Status FROM TBL_API_HUNT_Integration WHERE IntegrationId=p_IntegrationId AND Status IN ('9','10','11')) INTO v_StatusNew;

	  /* IF user change API category we need new questions and delete the Old One */
      IF v_StatusNew IS NOT NULL THEN

			SELECT (SELECT APICategory_Id FROM TBL_API_HUNT_ServiceDetails WHERE IntegrationId=p_IntegrationId AND ServiceID=p_ServiceID) INTO v_APICategory_IdOld;
			IF p_APICategory_Id<>v_APICategory_IdOld THEN

				DELETE FROM tbl_API_HUNT_QusServiceDetails WHERE ServiceID=p_ServiceID;
			
END IF;
	  
END IF;
          
                  
  IF (p_UpdateFlag='Update') THEN
          
  UPDATE TBL_API_HUNT_ServiceDetails SET IntegrationId=p_IntegrationId,          
  ServiceName=p_ServiceName, 
  ExpectedServiceSpecificationDocument = p_ExpectedAPISpecificationFileName,
  Purpose=p_Purpose,          
  Existing_New=p_Existing_New,          
  ConsumerApplication=p_ConsumerApplication,          
  ProducerApplication=p_ProducerApplication,          
  Is_APIGW_Request=p_Is_APIGW_Request,          
  Rest_Soap=p_Rest_Soap,          
  Transformation=p_Transformation,          
  Volume=p_Volume,          
  UpdatedBy=p_UpdatedBy,          
  UpdatedAt=NOW()  ,        
  Existing_New_Id = p_Existing_New_Id,      
  Rest_SOAP_Id = p_Rest_SOAP_Id,      
  ServiceType_Id = p_ServiceType_Id,      
  APIType_Id = p_APIType_Id,      
  APICategory_Id = p_APICategory_Id,      
  APIRiskScore_Id = p_APIRiskScore_Id,        PartnerRiskScore_Id = p_PartnerRiskScore_Id,      
  DomainName_Id = p_DomainName_Id ,    
  ConsumerDC_Id= p_ConsumerDC_Id,    
  ProducerDC_Id=p_ProducerDC_Id,    
  Platform=p_Platform,    
  QValue1=p_QValue1,    
  QWeightage1=case when p_QValue1=1 then 10     
                   when p_QValue1=2 then 10    
       when p_QValue1=3 then 10    
       when p_QValue1=4 then 2 end,    
  QValue2=p_QValue2,    
  QWeightage2=case when p_QValue2=1 then 12.5     
                   when p_QValue2=2 then 5    
       when p_QValue2=3 then 0 end,    
  QValue3=p_QValue3,    
  QWeightage3=case when p_QValue3=1 then 12.5     
                   when p_QValue3=2 then 5    
       when p_QValue3=3 then 0 end,    
  QValue4=p_QValue4,    
  QWeightage4=case when p_QValue4=1 then 5     
      when p_QValue4=2 then 2 end,    
  QValue5=p_QValue5,    
  QWeightage5=case when p_QValue5=1 then 10     
                   when p_QValue5=2 then 2    
       when p_QValue5=3 then 0 end,    
  RiskScore=p_RiskScore,    
  Classification=p_Classification,
  InternalServiceName = p_InternalServiceName,
  ExternalServiceName = p_ExternalServiceName,
  ConsumerDC=p_ConsumerDC,
  ProducerDC=p_ProducerDC
  where ServiceID=p_ServiceID;
      
  -- select  @FeedbackId as FeedbackId    
SELECT 
    MAX(Feedback_Id) AS FeedbackId
FROM
    tbl_API_hunt_Feedback;
    
  
ELSEIF (p_UpdateFlag='Insert') THEN
   
  
  -- IF(@Purpose!=null and @Purpose!='')
		-- Begin
		  INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
		 ,Existing_New_Id      
		,Rest_SOAP_Id      
		,ServiceType_Id      
		,APIType_Id      
		,APICategory_Id      
		,APIRiskScore_Id      
		,PartnerRiskScore_Id      
		,DomainName_Id    
		,ConsumerDC_Id    
		,ProducerDC_Id    
		,Platform    
		 ,QValue1    
		 ,QValue2    
		 ,QValue3    
		 ,QValue4    
		 ,QValue5    
		 ,RiskScore    
		 ,Classification 
		 ,InternalServiceName 
		 ,ExternalServiceName
		  ,ConsumerDC
		  ,ProducerDC
		  ,ExpectedServiceSpecificationDocument
		  )          
		  values(p_IntegrationId,p_ServiceName,p_Purpose,p_Existing_New,p_ConsumerApplication,p_ProducerApplication,p_Is_APIGW_Request,p_Rest_Soap,p_Transformation,p_Volume,p_CreatedBy,NOW()      
		,p_Existing_New_Id      
		,p_Rest_SOAP_Id      
		,p_ServiceType_Id      
		,p_APIType_Id      
		,p_APICategory_Id      
		,p_APIRiskScore_Id      
		,p_PartnerRiskScore_Id      
		,p_DomainName_Id    
		,p_ConsumerDC_Id    
		,p_ProducerDC_Id    
		,p_Platform    
		,p_QValue1    
		,p_QValue2    
		,p_QValue3    
		,p_QValue4    
		,p_QValue5    
		,p_RiskScore    
		,p_Classification 
		,p_InternalServiceName
		,p_ExternalServiceName
		,p_ConsumerDC
		,p_ProducerDC,
		p_ExpectedAPISpecificationFileName
		  );
	-- End
            
SELECT 
    ServiceID,
    IntegrationId,
    ServiceName,
    Purpose,
    Existing_New,
    ConsumerApplication,
    ProducerApplication,
    Is_APIGW_Request,
    Rest_Soap,
    Transformation,
    Volume,
    Existing_New_Id,
    Rest_SOAP_Id,
    ServiceType_Id,
    APIType_Id,
    APICategory_Id,
    APIRiskScore_Id,
    PartnerRiskScore_Id,
    DomainName_Id,
    ConsumerDC_Id,
    ProducerDC_Id,
    Platform,
    QValue1,
    QValue2,
    QValue3,
    QValue4,
    QValue5,
    RiskScore,
    Classification,
    p_FeedbackId AS FeedbackId,
    InternalServiceName,
    ExternalServiceName,
    ConsumerDC,
    ProducerDC,
    ExpectedServiceSpecificationDocument
FROM
    TBL_API_HUNT_ServiceDetails
WHERE
    IntegrationId = p_IntegrationId;
    
 -- set @FeedbackId as FeedbackId    
            
  
END IF;          
        
        
  
ELSEIF (p_IdentFlag='CheckProjectExist') THEN
        
  select Count(*) count1 from TBL_API_HUNT_Integration where ProjectId=p_ProjectId;
  
ELSEIF (p_IdentFlag='FillExectingServiceName') THEN
        
 -- select TBL_API_Main_ID as 'ExServiceId' ,COD_SERVICE_ID as 'ExServiceName'  from TBL_API_Main    
    select TBL_API_Main_ID as 'ExServiceId' ,OBP_SERVICE_URL_UAT as 'ExServiceName', COD_SERVICE_ID as 'ExCodServiceId'  from TBL_API_Main;
  
ELSEIF (p_IdentFlag='FillExectingSerNameOnId') THEN
  


  
   SELECT (SELECT RIGHT(OBP_SERVICE_URL_UAT, LOCATE('/',REVERSE(OBP_SERVICE_URL_UAT))-1) from TBL_API_Main where   TBL_API_Main_ID=p_servicenameId ) INTO v_InternalServiceId;

  
SELECT 
    (SELECT 
            COD_SERVICE_ID
        FROM
            TBL_API_EXTERNALSERVICES
        WHERE
            OBP_SERVICE_URL_UAT LIKE CONCAT('%', v_InternalServiceId, '%'))
INTO v_ExternalServiceId;

SELECT 
    SERVICE_PROVIDER,
    IFNULL(APIType.MisccdId, 0) AS APITypeId,
    IFNULL(APICate.MisccdId, 0) AS APICatId,
    IFNULL(DOMAINNAME.MisccdId, 0) AS DomainId,
    IFNULL(RestSoap.MisccdId, 0) AS RestSoapId,
    -- ISNULL(ProducerDC.MisccdId, 0) AS ProducerDC,
    IFNULL(SERVICE_PROVIDER, 0) AS ProducerDC,
    IFNULL(ServiceType.MisccdId, 0) AS ServiceType,
    IFNULL(APIRiskClassify.MisccdId, 0) AS APIRiskClassify,
    IFNULL(v_ExternalServiceId, '') AS ExternalCoDServiceID,
    IFNULL(FILLER_09, 0) AS APIRiskSocre,
    IFNULL(FILLER_10, '') AS APIRiskClassification
FROM
    TBL_API_Main
        LEFT JOIN
    tbl_API_hunt_Misccd AS APIType ON APIType.CDValDesc = TBL_API_Main.API_TYPE
        LEFT JOIN
    tbl_API_hunt_Misccd AS APICate ON APICate.CDValDesc = TBL_API_Main.API_CAT
        LEFT JOIN
    tbl_API_hunt_Misccd AS DOMAINNAME ON DOMAINNAME.CDValDesc = TBL_API_Main.DOMAIN_NAME
        LEFT JOIN
    tbl_API_hunt_Misccd AS RestSoap ON RestSoap.CDValDesc = TBL_API_Main.SERVICE_INTERFACE_TYPE
        LEFT JOIN
    tbl_API_hunt_Misccd AS ServiceType ON ServiceType.CDValDesc = TBL_API_Main.Service_Type
        LEFT JOIN
    tbl_API_hunt_Misccd AS APIRiskClassify ON APIRiskClassify.CDValDesc = TBL_API_Main.filler_02
WHERE
    TBL_API_Main_ID = p_servicenameId;
  
ELSEIF (p_IdentFlag='FillExternalServiceName') THEN
        
    
	 -- select EXTERNALSERVICE_ID as 'ExServiceId' ,OBP_SERVICE_URL_UAT as 'ExServiceName'  from [TBL_API_EXTERNALSERVICES] 
   select COD_SERVICE_ID as 'ExServiceId' ,OBP_SERVICE_URL_UAT as 'ExServiceName', COD_SERVICE_ID as 'ExCodServiceId'  from TBL_API_EXTERNALSERVICES;
  
ELSEIF (p_IdentFlag='FillExternalService ') THEN
 
  
  
  SELECT (SELECT RIGHT(OBP_SERVICE_URL_UAT, LOCATE('/',REVERSE(OBP_SERVICE_URL_UAT))-1) from TBL_API_EXTERNALSERVICES where (OBP_SERVICE_URL_UAT Like CONCAT('%', p_ExternalServiceText, '%') OR COD_SERVICE_ID Like CONCAT('%', p_ExternalServiceText, '%'))) INTO v_ExternalCodServiceId;

  

SELECT 
    (SELECT 
            COUNT(COD_SERVICE_ID)
        FROM
            TBL_API_Main
        WHERE
            OBP_SERVICE_URL_UAT LIKE CONCAT('%', v_ExternalCodServiceId, '%'))
INTO v_CheckExternalService;
  
  IF (v_CheckExternalService>0) THEN

  SELECT OBP_SERVICE_URL_UAT as 'OBP_SERVICE_URL_UAT',
  SERVICE_PROVIDER,    
  IFNULL(APIType. MisccdId,0) as APITypeId,    
  IFNULL(APICate. MisccdId,0) as APICatId,    
  IFNULL(DOMAINNAME. MisccdId,0) as DomainId,    
  IFNULL(RestSoap. MisccdId,0) as RestSoapId,    
  -- Isnull(ProducerDC. MisccdId,0) as ProducerDC,  
  IFNULL(SERVICE_PROVIDER,0) as ProducerDC, 
  IFNULL(ServiceType. MisccdId,0) as ServiceType,   
  IFNULL(APIRiskClassify. MisccdId,0) as APIRiskClassify,
  IFNULL( FILLER_09,'0') As APIRiskSocre,
  IFNULL( FILLER_10,'') As APIRiskClassification
    
  -- from TBL_API_EXTERNALSERVICES Main 
  from TBL_API_Main  Main 
  Left join tbl_API_hunt_Misccd As APIType on APIType.CDValDesc=Main.API_TYPE    
  Left join tbl_API_hunt_Misccd As APICate on APICate.CDValDesc=Main.API_CAT    
  Left join tbl_API_hunt_Misccd As DOMAINNAME on DOMAINNAME.CDValDesc=Main.DOMAIN_NAME    
  Left join tbl_API_hunt_Misccd As RestSoap on RestSoap.CDValDesc=Main.SERVICE_INTERFACE_TYPE    
  -- Left join [dbo].[tbl_API_hunt_Misccd] As ProducerDC on ProducerDC.CDValDesc=Main.FILLER_10     
  -- and ProducerDC.CDTP='Producer DC'    
  Left join tbl_API_hunt_Misccd As ServiceType on ServiceType.CDValDesc=Main.Service_Type   
  Left join tbl_API_hunt_Misccd As APIRiskClassify on APIRiskClassify.CDValDesc=Main.filler_02    
  where OBP_SERVICE_URL_UAT like CONCAT('%', v_ExternalCodServiceId, '%');
  
ELSE

  SELECT -- COD_SERVICE_ID as 'OBP_SERVICE_URL_UAT',
 '' as 'OBP_SERVICE_URL_UAT',
  SERVICE_PROVIDER,    
  IFNULL(APIType. MisccdId,0) as APITypeId,    
  IFNULL(APICate. MisccdId,0) as APICatId,    
  IFNULL(DOMAINNAME. MisccdId,0) as DomainId,    
  IFNULL(RestSoap. MisccdId,0) as RestSoapId,    
 -- Isnull(ProducerDC. MisccdId,0) as ProducerDC,    
  IFNULL(SERVICE_PROVIDER,0) as ProducerDC,  
  IFNULL(ServiceType. MisccdId,0) as ServiceType,   
  IFNULL(APIRiskClassify. MisccdId,0) as APIRiskClassify,
  IFNULL( FILLER_03,'0') As APIRiskSocre,
  IFNULL( FILLER_02,'') As APIRiskClassification
    
  from  TBL_API_EXTERNALSERVICES   TBL_API_Main   
  Left join tbl_API_hunt_Misccd As APIType on APIType.CDValDesc=TBL_API_Main.API_TYPE_    
  Left join tbl_API_hunt_Misccd As APICate on APICate.CDValDesc=TBL_API_Main.API_CAT    
  Left join tbl_API_hunt_Misccd As DOMAINNAME on DOMAINNAME.CDValDesc=TBL_API_Main.DOMAIN_NAME    
  Left join tbl_API_hunt_Misccd As RestSoap on RestSoap.CDValDesc=TBL_API_Main.SERVICE_INTERFACE_TYPE    
  -- Left join [dbo].[tbl_API_hunt_Misccd] As ProducerDC on ProducerDC.CDValDesc=TBL_API_Main.FILLER_10     
  -- and ProducerDC.CDTP='Producer DC'    
  Left join tbl_API_hunt_Misccd As ServiceType on ServiceType.CDValDesc=TBL_API_Main.Service_Type    
  Left join tbl_API_hunt_Misccd As APIRiskClassify on APIRiskClassify.CDValDesc=TBL_API_Main.filler_02    
  where OBP_SERVICE_URL_UAT like CONCAT('%', v_ExternalCodServiceId, '%');
  
END IF;
  



  
ELSEIF (p_IdentFlag='FillTestApiAuto') THEN
        
  -- select TBL_API_Main_ID as Id,OBP_SERVICE_URL_UAT as ServiceURL,FileName as 'path' from TBL_API_Main where  (PRODUCT_PROCESSOR_URL_UAT is not null and PRODUCT_PROCESSOR_URL_UAT<>'NA')    
    select distinct tblapimain.TBL_API_Main_ID as Id,tblapimain.OBP_SERVICE_URL_UAT as ServiceURL,tblapifilepath.FileName as 'path',SERVICE_INTERFACE_TYPE as apiCategory from TBL_API_Main tblapimain    
  Left Join Tbl_API_FilePath tblapifilepath on tblapimain.OBP_SERVICE_URL_UAT=tblapifilepath.OBP_SERVICE_URL_UAT     
  where (tblapimain.OBP_SERVICE_URL_UAT is not null and tblapimain.OBP_SERVICE_URL_UAT<>'NA');
  
ELSEIF (p_IdentFlag='FillHostApplication') THEN
        
   
   select Id As 'HostId',(CONCAT(IFNULL(APPShortName,''), '-', IFNULL(FullName,'')) ) as 'HostAppName', ITGRCCode,ITGRCName from tbl_API_MstApplications;
  
ELSEIF (p_IdentFlag='FillHostingDC') THEN
        
   
   select Id As 'HostId',FullName  as 'HostAppName', HostingDC from tbl_API_MstApplications where Id=p_HostAppID;
  
ELSEIF (p_IdentFlag='FillHostingDCtext') THEN
        
   
    
	SELECT (select case when p_HostApptext Like '%#%' then SUBSTRING(p_HostApptext, 1,LOCATE('#', p_HostApptext)-1) else p_HostApptext end) INTO v_HostApptext;


SELECT 
    Id AS 'HostId', FullName AS 'HostAppName', HostingDC
FROM
    tbl_API_MstApplications
WHERE
    (FullName = p_HostApptext
        OR APPShortName = p_HostApptext);

SELECT 
    Id AS 'HostId',
    FullName AS 'HostAppName',
    ITGRCCode,
    ITGRCName
FROM
    tbl_API_MstApplications
WHERE
    (FullName = p_HostApptext
        OR APPShortName = p_HostApptext);

  
ELSEIF (p_IdentFlag='FillConsumerDCNamebyText') THEN
   
          select  Id As 'HostId',FullName  as 'HostAppName', HostingDC from tbl_API_MstApplications where FullName=SUBSTRING(p_ConsumerApplication, LOCATE('-', p_ConsumerApplication) + 1, CHAR_LENGTH(p_ConsumerApplication));
  
ELSEIF (p_IdentFlag='CheckConsumerAppName') THEN
   
  -- select Count(*) count1 from tbl_API_MstApplications where FullName=@ConsumerApplication       
  select Count(*) count1 from tbl_API_MstApplications where FullName=SUBSTRING(p_ConsumerApplication, LOCATE('-', p_ConsumerApplication) + 1, CHAR_LENGTH(p_ConsumerApplication));
  
ELSEIF (p_IdentFlag='CheckProducerAppName') THEN
        
  -- select Count(*) count1 from tbl_API_MstApplications where FullName=@ProducerApplication  
  select Count(*) count1 from tbl_API_MstApplications where FullName=SUBSTRING(p_ProducerApplication, LOCATE('-', p_ProducerApplication) + 1, CHAR_LENGTH(p_ProducerApplication));
  
ELSEIF (p_IdentFlag='GetId') THEN
        
  SELECT IntegrationId FROM TBL_API_HUNT_Integration ORDER BY IntegrationId DESC;
  
ELSEIF (p_IdentFlag='GetDate') THEN
        
  SELECT CreatedAt FROM TBL_API_HUNT_Integration WHERE IntegrationId = p_IntegrationId;
  
ELSEIF (p_IdentFlag='GetSpocEmailIDs') THEN
        
  SELECT EmailAddress from tbl_API_ApplicationsSPOC where APPID =  p_ConsumerId;
  
ELSEIF (p_IdentFlag='QuestionInsert') THEN
        
 

  SELECT (select ServiceID From tbl_API_HUNT_QusServiceDetails where ServiceID=p_ServiceID AND QID=p_QID) INTO v_QueServiceID;

 IF IFNULL(v_QueServiceID,'')<>'' THEN

	-- DELETE  from tbl_API_HUNT_QusServiceDetails where ServiceID=@QueServiceID

	UPDATE tbl_API_HUNT_QusServiceDetails
	SET 
	OptionsID=p_OptionsID,
	Updatedby= p_CreatedBy,
	UpdatedAt= NOW()
	WHERE ServiceID=p_ServiceID AND QID=p_QID;
 
ELSE

  INSERT INTO tbl_API_HUNT_QusServiceDetails
	(
	       ServiceID
           ,QID
           ,OptionsID
           ,IsActive
           ,CreatedBy
           ,CreatedAt
           )
     VALUES
           (
		   p_ServiceID,
           p_QID,
           p_OptionsID,
           p_IsActive,
           p_CreatedBy,
           NOW()
		   );
   
END IF;     
  
ELSEIF (p_IdentFlag='GetQes') THEN

     select ServiceID,APQ.QID,OptionsID,Weightage,options as val
	 from tbl_API_HUNT_QusServiceDetails APQ
	 Inner Join  TBL_API_HUNT_QuestionData as AQ on AQ.ID=OptionsID
	 WHERE ServiceID=p_ServiceID;
	 
  
END IF;
-- -------------------------------------------------------------------Darfting Integration---start-----------------------------------------------------------------------------------------------
    ELSEIF p_IdentFlag='DeleteDraft' THEN


		DELETE FROM TBL_API_HUNT_ServiceDetails
WHERE IntegrationId = p_IntegrationId
  AND IFNULL(p_ServiceID, '') <> ''
  AND IFNULL(p_ServiceID, ',') <> ','
  AND ServiceID NOT IN (
      WITH RECURSIVE SplitString AS (
          SELECT 
              TRIM(SUBSTRING_INDEX(p_ServiceID, ',', 1)) AS Item,
              IF(LOCATE(',', p_ServiceID) > 0,
                 SUBSTRING(p_ServiceID, LOCATE(',', p_ServiceID) + 1),
                 NULL) AS Remaining
          UNION ALL
          SELECT 
              TRIM(SUBSTRING_INDEX(Remaining, ',', 1)),
              IF(LOCATE(',', Remaining) > 0,
                 SUBSTRING(Remaining, LOCATE(',', Remaining) + 1),
                 NULL)
          FROM SplitString
          WHERE Remaining IS NOT NULL
      )
      SELECT CAST(Item AS UNSIGNED) FROM SplitString
  );
      
	
ELSEIF (p_IdentFlag='DraftAddNewIntegration') THEN
            
  INSERT INTO TBL_API_HUNT_Integration(ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,            
  BusinessJustification,BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,CreatedAt,CreatedBy,Assign,AssignFrom, ConsumerApplication,RDConceptNote,In_Platform)            
  VALUES(p_ProjectManagerBTG,p_ProjectManagerIT,p_ProjectName,p_ProjectId,p_PlannedGoLiveDate,p_BusinessJustification,            
  p_BusinessSponsor,p_ExecutiveSponsor,p_CostCenterCode,p_UserJourneyDocumentFileName,p_Status,NOW(),p_CreatedBy,p_Assign,p_AssignFrom, p_ConsumerApplication,p_RDConceptNoteFileName, 'NoPlatfrom');
    
SELECT (SELECT LAST_INSERT_ID() AS IntegrationId) INTO v_IntegrationId;
  SET v_Feedback = 'New Integration  Draft  By User';
  IF(p_IntegrationId <> 0  or p_IntegrationId <> null) THEN
        INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom)     
      VALUES(p_IntegrationId,p_Feedback,p_Status,p_CreatedBy,NOW(),p_IsActive,p_Assign,p_AssignFrom);
      
SELECT p_IntegrationId AS IntegrationId;    
           
          
  
ELSEIF (p_IdentFlag='DraftAddServiceDetails') THEN
  

		-- IF(@Purpose!=null and @Purpose!='')
		-- Begin
			INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
			,Existing_New_Id      
			,Rest_SOAP_Id      
			,ServiceType_Id      
			,APIType_Id      
			,APICategory_Id      
			,APIRiskScore_Id      
			,PartnerRiskScore_Id      
			,DomainName_Id,     
			ConsumerDC_Id,    
			ProducerDC_Id,    
			Platform,    
			QValue1,    
			QValue2,    
			QValue3,    
			QValue4,    
			QValue5,    
			RiskScore,
			InternalServiceName,
			ExternalServiceName ,
			ConsumerDC,
			ProducerDC,
			ExpectedServiceSpecificationDocument,
			Classification
			)          
			values(      
			p_IntegrationId,p_ServiceName,p_Purpose,p_Existing_New,p_ConsumerApplication,p_ProducerApplication,p_Is_APIGW_Request,p_Rest_Soap,p_Transformation,      
			p_Volume,      
			p_CreatedBy      
			,NOW()      
			,p_Existing_New_Id      
			,p_Rest_SOAP_Id      
			,p_ServiceType_Id      
			,p_APIType_Id      
			,p_APICategory_Id      
			,p_APIRiskScore_Id      
			,p_PartnerRiskScore_Id     
			,p_DomainName_Id,    
			p_ConsumerDC_Id,    
			p_ProducerDC_Id,    
			p_Platform,    
			p_QValue1,    
			p_QValue2,    
			p_QValue3,    
			p_QValue4,    
			p_QValue5,    
			p_RiskScore,
			p_InternalServiceName,
			p_ExternalServiceName ,
			p_ConsumerDC,
			p_ProducerDC,
			p_ExpectedAPISpecificationFileName,
			p_Classification
			);
		-- End
  
ELSEIF (p_IdentFlag='DarftUpdateServiceDetails') THEN
          
 IF (p_UpdateFlag='Update') THEN
 

 -- IF(@Platform ='Internal & External' Or @Platform ='External')
 -- BEGIN
  
 
  -- -----------------------------------------------------------------Add New Integration----Based On Internal & External--------------------------------------------------------------------------
 --
 --
 --

 -- SET @InternalCount= (select count(Platform) from TBL_API_HUNT_ServiceDetails where Platform='Internal' and IntegrationId=@IntegrationId)
 -- SET @ExternalCount= (select count(Platform) from TBL_API_HUNT_ServiceDetails where Platform='External' and IntegrationId=@IntegrationId)
 -- SET @InternalExternalCount= (select count(Platform) from TBL_API_HUNT_ServiceDetails where Platform='Internal & External' and IntegrationId=@IntegrationId)

 -- IF (@ExternalCount>0 AND @ExternalCount=0 AND @InternalExternalCount=0)
 -- BEGIN
        IF (p_flagPl='All') THEN

              UPDATE TBL_API_HUNT_Integration set In_Platform= 'Internal' WHERE IntegrationId=p_IntegrationId;
			
ELSEIF (p_flagPl='single' and p_Platform='Internal & External' ) THEN
 
               UPDATE TBL_API_HUNT_Integration set In_Platform= 'Internal' WHERE IntegrationId=p_IntegrationId;
	           SET v_flagPl ='All';
             
ELSE

			     UPDATE TBL_API_HUNT_Integration set In_Platform= p_Platform WHERE IntegrationId=p_IntegrationId;
			
END IF;
		UPDATE TBL_API_HUNT_ServiceDetails 
SET 
    IntegrationId = p_IntegrationId,
    ServiceName = p_ServiceName,
    Purpose = p_Purpose,
    Existing_New = p_Existing_New,
    ConsumerApplication = p_ConsumerApplication,
    ProducerApplication = p_ProducerApplication,
    Is_APIGW_Request = p_Is_APIGW_Request,
    Rest_Soap = p_Rest_Soap,
    Transformation = p_Transformation,
    Volume = p_Volume,
    UpdatedBy = p_UpdatedBy,
    UpdatedAt = NOW(),
    Existing_New_Id = p_Existing_New_Id,
    Rest_SOAP_Id = p_Rest_SOAP_Id,
    ServiceType_Id = p_ServiceType_Id,
    APIType_Id = p_APIType_Id,
    APICategory_Id = p_APICategory_Id,
    APIRiskScore_Id = p_APIRiskScore_Id,
    PartnerRiskScore_Id = p_PartnerRiskScore_Id,
    DomainName_Id = p_DomainName_Id,
    ConsumerDC_Id = p_ConsumerDC_Id,
    ProducerDC_Id = p_ProducerDC_Id,
    Platform = p_Platform,
    QValue1 = p_QValue1,
    QWeightage1 = CASE
        WHEN p_QValue1 = 1 THEN 10
        WHEN p_QValue1 = 2 THEN 10
        WHEN p_QValue1 = 3 THEN 10
        WHEN p_QValue1 = 4 THEN 2
    END,
    QValue2 = p_QValue2,
    QWeightage2 = CASE
        WHEN p_QValue2 = 1 THEN 12.5
        WHEN p_QValue2 = 2 THEN 5
        WHEN p_QValue2 = 3 THEN 0
    END,
    QValue3 = p_QValue3,
    QWeightage3 = CASE
        WHEN p_QValue3 = 1 THEN 12.5
        WHEN p_QValue3 = 2 THEN 5
        WHEN p_QValue3 = 3 THEN 0
    END,
    QValue4 = p_QValue4,
    QWeightage4 = CASE
        WHEN p_QValue4 = 1 THEN 5
        WHEN p_QValue4 = 2 THEN 2
    END,
    QValue5 = p_QValue5,
    QWeightage5 = CASE
        WHEN p_QValue5 = 1 THEN 10
        WHEN p_QValue5 = 2 THEN 2
        WHEN p_QValue5 = 3 THEN 0
    END,
    RiskScore = p_RiskScore,
    Classification = p_Classification,
    InternalServiceName = p_InternalServiceName,
    ExternalServiceName = p_ExternalServiceName,
    ConsumerDC = p_ConsumerDC,
    ProducerDC = p_ProducerDC
WHERE
    ServiceID = p_ServiceID;

-- END
 IF (p_flagPl='All') THEN -- ((@InternalCount>0 AND @ExternalCount>0) OR (@InternalCount>0 AND @InternalExternalCount>0) OR (@ExternalCount>0 AND @InternalExternalCount>0) )

 
 
 IF ((select Count(Parent_IntegrationId) from TBL_API_HUNT_Integration where Parent_IntegrationId=p_IntegrationId)=0) THEN

     INSERT INTO TBL_API_HUNT_Integration(ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,            
     BusinessJustification,BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,CreatedAt,CreatedBy,Assign,AssignFrom, ConsumerApplication,Parent_IntegrationId,In_Platform,RDConceptNote)
      select ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,BusinessJustification,            
      BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,NOW(),CreatedBy,Assign,AssignFrom, ConsumerApplication,p_IntegrationId,'External',RDConceptNote  From TBL_API_HUNT_Integration  where   IntegrationId=p_IntegrationId;

SELECT (SELECT LAST_INSERT_ID() AS IntegrationId) INTO v_IntegrationIdIED;

  SET v_Feedback = 'New Integration  Draft  By User';
SELECT 
    (SELECT 
            CreatedBy
        FROM
            TBL_API_HUNT_Integration
        WHERE
            IntegrationId = p_IntegrationId)
INTO v_CreatedBy;
  IF (v_IntegrationIdIED <> 0  or v_IntegrationIdIED <> null) THEN

        INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom)     
        VALUES(v_IntegrationIdIED,p_Feedback,12,p_CreatedBy,NOW(),p_IsActive,'BTGUSER','USER');
		
		 SET v_Feedback = 'New Integration  Created  By User';
		INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom)     
        VALUES(v_IntegrationIdIED,p_Feedback,1,p_CreatedBy,NOW(),p_IsActive,'BTGUSER','USER');

		INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom)     
        VALUES(p_IntegrationId,p_Feedback,1,p_CreatedBy,NOW(),p_IsActive,'BTGUSER','USER');
	   
    
END IF; 
  
ELSE

    SELECT (SELECT IntegrationId from TBL_API_HUNT_Integration where Parent_IntegrationId=p_IntegrationId and Parent_IntegrationId is not null) INTO v_IntegrationIdIED;
  
END IF;

UPDATE TBL_API_HUNT_Integration 
SET 
    In_Platform = 'External'
WHERE
    Parent_IntegrationId = v_IntegrationIdIED;
  -- -------------------------------------------Add Service-------Based On Internal & External---------------------------------------------------

  INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
  ,Existing_New_Id      
,Rest_SOAP_Id   
,ServiceType_Id      
,APIType_Id      
,APICategory_Id      
,APIRiskScore_Id      
,PartnerRiskScore_Id      
,DomainName_Id,     
ConsumerDC_Id,    
ProducerDC_Id,    
Platform,    
QValue1,    
QValue2,    
QValue3,    
QValue4,    
QValue5,    
RiskScore,
InternalServiceName,
ExternalServiceName ,
ConsumerDC,
ProducerDC,
ExpectedServiceSpecificationDocument,
Classification
)
  Select  
  v_IntegrationIdIED,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
  ,Existing_New_Id      
,Rest_SOAP_Id      
,ServiceType_Id      
,APIType_Id      
,APICategory_Id      
,APIRiskScore_Id      
,PartnerRiskScore_Id      
,DomainName_Id,     
ConsumerDC_Id,    
ProducerDC_Id,    
Platform,    
QValue1,    
QValue2,    
QValue3,    
QValue4,    
QValue5,    
RiskScore,
InternalServiceName,
ExternalServiceName ,
ConsumerDC,
ProducerDC,
ExpectedServiceSpecificationDocument,
Classification
From TBL_API_HUNT_ServiceDetails where ServiceID=p_ServiceID;

     -- DELETE FROM TBL_API_HUNT_ServiceDetails where ServiceID=@ServiceID   

END IF;


END IF;
 
SELECT 
    MAX(Feedback_Id) AS FeedbackId
FROM
    tbl_API_hunt_Feedback;
 IF (p_UpdateFlag='Insert') THEN
      
 
    IF (p_flagPl='All') THEN

              UPDATE TBL_API_HUNT_Integration set In_Platform= 'Internal' WHERE IntegrationId=p_IntegrationId;
			
ELSEIF (p_flagPl='single' and p_Platform='Internal & External' ) THEN
 
               UPDATE TBL_API_HUNT_Integration set In_Platform= 'Internal' WHERE IntegrationId=p_IntegrationId;
	           SET v_flagPl ='All';
             
ELSE

			     UPDATE TBL_API_HUNT_Integration set In_Platform= p_Platform WHERE IntegrationId=p_IntegrationId;
			
END IF;
	-- IF(@Purpose!=null and @Purpose!='')
	-- Begin
		INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
		 ,Existing_New_Id      
		,Rest_SOAP_Id      
		,ServiceType_Id      
		,APIType_Id      
		,APICategory_Id      
		,APIRiskScore_Id      
		,PartnerRiskScore_Id      
		,DomainName_Id    
		,ConsumerDC_Id    
		,ProducerDC_Id    
		,Platform    
		 ,QValue1    
		 ,QValue2    
		 ,QValue3    
		 ,QValue4    
		 ,QValue5    
		 ,RiskScore    
		 ,Classification 
		 ,InternalServiceName 
		 ,ExternalServiceName
		  ,ConsumerDC
		  ,ProducerDC
		  ,ExpectedServiceSpecificationDocument
  
		  )          
		  values(p_IntegrationId,p_ServiceName,p_Purpose,p_Existing_New,p_ConsumerApplication,p_ProducerApplication,p_Is_APIGW_Request,p_Rest_Soap,p_Transformation,p_Volume,p_CreatedBy,NOW()      
		,p_Existing_New_Id      
		,p_Rest_SOAP_Id      
		,p_ServiceType_Id      
		,p_APIType_Id      
		,p_APICategory_Id      
		,p_APIRiskScore_Id      
		,p_PartnerRiskScore_Id      
		,p_DomainName_Id    
		,p_ConsumerDC_Id    
		,p_ProducerDC_Id    
		,p_Platform    
		,p_QValue1    
		,p_QValue2    
		,p_QValue3    
		,p_QValue4    
		,p_QValue5    
		,p_RiskScore    
		,p_Classification 
		,p_InternalServiceName
		,p_ExternalServiceName
		,p_ConsumerDC
		,p_ProducerDC
		,p_ExpectedAPISpecificationFileName

		  );
		  -- End
   
 --
 --
 --

 -- SET @InternalCountID= (select count(Platform) from TBL_API_HUNT_ServiceDetails where Platform='Internal' and IntegrationId=@IntegrationId)
 -- SET @ExternalCountID= (select count(Platform) from TBL_API_HUNT_ServiceDetails where Platform='External' and IntegrationId=@IntegrationId)
 -- SET @InternalExternalCountID= (select count(Platform) from TBL_API_HUNT_ServiceDetails where Platform='Internal & External' and IntegrationId=@IntegrationId)


IF (p_flagPl='All') THEN   -- ((@InternalCountID>0 AND @ExternalCountID>0) OR (@InternalCountID>0 AND @InternalExternalCountID>0) OR (@ExternalCountID>0 AND @InternalExternalCountID>0) )

  
  -- -----------------------------------------------------------------Add New Integration----Based On Internal & External--------------------------------------------------------------------------
 IF ((select Count(Parent_IntegrationId) from TBL_API_HUNT_Integration where Parent_IntegrationId=p_IntegrationId)=0) THEN

     INSERT INTO TBL_API_HUNT_Integration(ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,            
     BusinessJustification,BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,CreatedAt,CreatedBy,Assign,AssignFrom, ConsumerApplication,Parent_IntegrationId,In_Platform,RDConceptNote)
      select ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,BusinessJustification,            
      BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,NOW(),CreatedBy,Assign,AssignFrom, ConsumerApplication,p_IntegrationId,'External',RDConceptNote  From TBL_API_HUNT_Integration  where   IntegrationId=p_IntegrationId;

  SELECT (SELECT LAST_INSERT_ID() AS IntegrationId) INTO v_IntegrationIdIEDE;

  
ELSE

    SELECT (SELECT IntegrationId from TBL_API_HUNT_Integration where Parent_IntegrationId=p_IntegrationId and Parent_IntegrationId is not null) INTO v_IntegrationIdIEDE;
  
END IF; 
  -- -------------------------------------------Add Service-------Based On Internal & External---------------------------------------------------
   UPDATE TBL_API_HUNT_Integration set In_Platform= 'External'WHERE IntegrationId=p_IntegrationId;
  -- 	IF(@Purpose!=null and @Purpose!='')
		-- Begin
		   INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
		 ,Existing_New_Id      
		,Rest_SOAP_Id      
		,ServiceType_Id      
		,APIType_Id      
		,APICategory_Id      
		,APIRiskScore_Id      
		,PartnerRiskScore_Id      
		,DomainName_Id    
		,ConsumerDC_Id    
		,ProducerDC_Id    
		,Platform    
		 ,QValue1    
		 ,QValue2    
		 ,QValue3    
		 ,QValue4    
		 ,QValue5    
		 ,RiskScore   
		 ,Classification 
		 ,InternalServiceName 
		 ,ExternalServiceName
		  ,ConsumerDC
		  ,ProducerDC
		  ,ExpectedServiceSpecificationDocument
		  )          
		  values(v_IntegrationIdIEDE,p_ServiceName,p_Purpose,p_Existing_New,p_ConsumerApplication,p_ProducerApplication,p_Is_APIGW_Request,p_Rest_Soap,p_Transformation,p_Volume,p_CreatedBy,NOW()      
		,p_Existing_New_Id      
		,p_Rest_SOAP_Id      
		,p_ServiceType_Id      
		,p_APIType_Id      
		,p_APICategory_Id      
		,p_APIRiskScore_Id      
		,p_PartnerRiskScore_Id      
		,p_DomainName_Id    
		,p_ConsumerDC_Id    
		,p_ProducerDC_Id    
		,p_Platform    
		,p_QValue1    
		,p_QValue2    
		,p_QValue3    
		,p_QValue4    
		,p_QValue5    
		,p_RiskScore    
		,p_Classification 
		,p_InternalServiceName
		,p_ExternalServiceName
		,p_ConsumerDC
		,p_ProducerDC
		,p_ExpectedAPISpecificationFileName
		  );
		-- End

-- DELETE FROM TBL_API_HUNT_ServiceDetails where ServiceID=@ServiceID   


END IF;
 select ServiceID,IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume ,Existing_New_Id      
,Rest_SOAP_Id      
,ServiceType_Id      
,APIType_Id      
,APICategory_Id      
,APIRiskScore_Id      
,PartnerRiskScore_Id      
,DomainName_Id     
,ConsumerDC_Id 
 ,ProducerDC_Id    
 ,Platform    
 ,QValue1    
 ,QValue2    
 ,QValue3    
 ,QValue4    
 ,QValue5    
 ,RiskScore    
,Classification    
,p_FeedbackId as FeedbackId  
,InternalServiceName
,ExternalServiceName
,ConsumerDC
,ProducerDC
,ExpectedServiceSpecificationDocument
from TBL_API_HUNT_ServiceDetails          
where IntegrationId =p_IntegrationId;
    
 -- set @FeedbackId as FeedbackId    
            
  
END IF;          
          
        
  
END IF;  


END IF;
      
	



-- select * from TBL_API_HUNT_ServiceDetails where IntegrationId='20'        
        
--  select ServiceID,IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume          
--  from TBL_API_HUNT_ServiceDetails          
--  where IntegrationId =20
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_API_HUNT_NewAPIIntegration_Test;
DELIMITER //
CREATE PROCEDURE sp_API_HUNT_NewAPIIntegration_Test(IN p_IntegrationId int, IN p_ProjectManagerBTG varchar(100), IN p_ProjectManagerIT varchar(100), IN p_ProjectName varchar(100), IN p_ProjectId varchar(100), IN p_PlannedGoLiveDate datetime, IN p_BusinessJustification LONGTEXT, IN p_BusinessSponsor varchar(100), IN p_ExecutiveSponsor varchar(100), IN p_CostCenterCode varchar(100), IN p_UserJourneyDocumentFileName varchar(100), IN p_Feedback varchar(500), IN p_Status int, IN p_CreatedBy varchar(100), IN p_UpdatedBy varchar(100), IN p_IdentFlag varchar(50), IN p_ServiceName varchar(100), IN p_Purpose varchar(100), IN p_Existing_New varchar(100), IN p_ConsumerApplication varchar(100), IN p_ProducerApplication varchar(100), IN p_Is_APIGW_Request TINYINT(1), IN p_Rest_Soap varchar(100), IN p_Transformation varchar(100), IN p_Volume varchar(100), IN p_UpdateFlag varchar(100), IN p_ServiceID varchar(100), IN p_Assign Varchar(50), IN p_AssignFrom Varchar(50), IN p_Existing_New_Id int, IN p_Rest_SOAP_Id int, IN p_ServiceType_Id int, IN p_APIType_Id int, IN p_APICategory_Id int, IN p_APIRiskScore_Id int, IN p_PartnerRiskScore_Id int, IN p_DomainName_Id int, IN p_IsActive TINYINT(1), IN p_FeedbackId int, IN p_servicenameId int, IN p_ConsumerDC_Id int, IN p_ProducerDC_Id int, IN p_Platform varchar(200), IN p_QValue1 int, IN p_QValue2 int, IN p_QValue3 int, IN p_QValue4 int, IN p_QValue5 int, IN p_RiskScore FLOAT, IN p_Classification varchar(10), IN p_BTGUSER VARCHAR(50), IN p_ITUSER VARCHAR(50), IN p_ITARCHITECTURE VARCHAR(50), IN p_ChannelID VARCHAR(50), IN p_ContainerName VARCHAR(50), IN p_InternalServiceName VARCHAR(50), IN p_ExternalServiceName VARCHAR(50), IN p_ExternalServiceText LONGTEXT, IN p_HostAppID int, IN p_ConsumerDC VARCHAR(50), IN p_ProducerDC VARCHAR(50), IN p_HostApptext VARCHAR(50), IN p_RDConceptNoteFileName varchar(100), IN p_SequenceDiagramFileName varchar(100), IN p_ExpectedServiceSpecificationDocumentFileName varchar(100))
BEGIN
  DECLARE v_IntegrationIdIE int;
  DECLARE v_InternalServiceId LONGTEXT;
  DECLARE v_ExternalServiceId LONGTEXT;
  DECLARE v_ExternalCodServiceId LONGTEXT;
  DECLARE v_Feedback TEXT;
  DECLARE v_IntegrationId TEXT;
IF (p_IdentFlag='AddNewIntegration') THEN
            
  INSERT INTO TBL_API_HUNT_Integration(ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,            
  BusinessJustification,BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,CreatedAt,CreatedBy,Assign,AssignFrom, ConsumerApplication,RDConceptNote)            
  VALUES(p_ProjectManagerBTG,p_ProjectManagerIT,p_ProjectName,p_ProjectId,p_PlannedGoLiveDate,p_BusinessJustification,            
  p_BusinessSponsor,p_ExecutiveSponsor,p_CostCenterCode,p_UserJourneyDocumentFileName,p_Status,NOW(),p_CreatedBy,p_Assign,p_AssignFrom, p_ConsumerApplication,p_RDConceptNoteFileName);
    
  SELECT (SELECT LAST_INSERT_ID() AS IntegrationId) INTO v_IntegrationId;
  SET v_Feedback = 'New Integration  Created  By User';
  IF (p_IntegrationId <> 0  or p_IntegrationId <> null) Then
        INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom)     
      VALUES(p_IntegrationId,p_Feedback,p_Status,p_CreatedBy,NOW(),p_IsActive,p_Assign,p_AssignFrom);
      
    SELECT p_IntegrationId AS IntegrationId;    
    
 -- SELECT SCOPE_IDENTITY() AS IntegrationId;    
 -- SELECT MAX(IntegrationId) AS IntegrationId FROM TBL_API_HUNT_Integration          
          
  
ELSEIF (p_IdentFlag='GetNewIntegrationDetails') THEN
          
        
  IF (p_AssignFrom='USER') THEN
         
  Select     
  I.IntegrationId,    
  CONCAT('API', (CAST(I.CreatedAt AS CHAR) ,' ',''), Cast(I.IntegrationId AS CHAR))  as 'APIHUNTID',    
  ProjectName,    
  ProjectId,    
  ProjectManagerBTG,    
  ProjectManagerIT,     
  statusDescription as Status,    
  Assign,    
  AssignFrom,    
  Status as workflowstatus ,  Platform       
  from TBL_API_HUNT_Integration I   
   Inner Join TBL_API_statusMaster As SM on SM.statusCode=Status    
   Inner Join TBL_API_HUNT_ServiceDetails  SD on I.IntegrationId = SD.IntegrationId
     where I.CreatedBy=p_CreatedBy      
  ORDER BY IntegrationId DESC;
  
ELSEIF (p_AssignFrom<>'USER') THEN
         
  Select     
  I.IntegrationId,    
  CONCAT('API', (CAST(I.CreatedAt AS CHAR) ,' ',''), Cast(I.IntegrationId AS CHAR))  as 'APIHUNTID',    
  ProjectName,    
  ProjectId,    
  ProjectManagerBTG,    
  ProjectManagerIT,     
  statusDescription as Status,    
  Assign,    
  AssignFrom,    
  Status as workflowstatus ,  Platform       
  from TBL_API_HUNT_Integration I   
   Inner Join TBL_API_statusMaster As SM on SM.statusCode=Status    
   Inner Join TBL_API_HUNT_ServiceDetails  SD on I.IntegrationId = SD.IntegrationId
     where I.CreatedBy=p_CreatedBy      
  ORDER BY IntegrationId DESC;
    
    
  
END IF;        
        
 
ELSEIF (p_IdentFlag='AddServiceDetails') THEN
          
  INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
  ,Existing_New_Id      
,Rest_SOAP_Id      
,ServiceType_Id      
,APIType_Id      
,APICategory_Id      
,APIRiskScore_Id      
,PartnerRiskScore_Id      
,DomainName_Id,     
ConsumerDC_Id,    
ProducerDC_Id,    
Platform,    
QValue1,    
QValue2,    
QValue3,    
QValue4,    
QValue5,    
RiskScore,
InternalServiceName,
ExternalServiceName ,
ConsumerDC,
ProducerDC,
ExpectedServiceSpecificationDocument
  )          
      
      
  values(      
  p_IntegrationId,p_ServiceName,p_Purpose,p_Existing_New,p_ConsumerApplication,p_ProducerApplication,p_Is_APIGW_Request,p_Rest_Soap,p_Transformation,      
  p_Volume,      
  p_CreatedBy      
 ,NOW()      
,p_Existing_New_Id      
,p_Rest_SOAP_Id      
,p_ServiceType_Id      
,p_APIType_Id      
,p_APICategory_Id      
,p_APIRiskScore_Id      
,p_PartnerRiskScore_Id     
,p_DomainName_Id,    
p_ConsumerDC_Id,    
p_ProducerDC_Id,    
p_Platform,    
p_QValue1,    
p_QValue2,    
p_QValue3,    
p_QValue4,    
p_QValue5,    
p_RiskScore,
p_InternalServiceName,
p_ExternalServiceName ,
p_ConsumerDC,
p_ProducerDC,p_ExpectedServiceSpecificationDocumentFileName
  );
  
  IF (p_Platform ='Internal & External') THEN

  
  -- -----------------------------------------------------------------Add New Integration----Based On Internal & External--------------------------------------------------------------------------
   INSERT INTO TBL_API_HUNT_Integration(ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,            
  BusinessJustification,BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,CreatedAt,CreatedBy,Assign,AssignFrom, ConsumerApplication)
  select ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,PlannedGoLiveDate,BusinessJustification,            
  BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,Status,NOW(),CreatedBy,Assign,AssignFrom, ConsumerApplication From TBL_API_HUNT_Integration  where   IntegrationId=p_IntegrationId;
    
  SELECT (SELECT LAST_INSERT_ID() AS IntegrationId) INTO v_IntegrationIdIE;



  -- -------------------------------------------Add Service-------Based On Internal & External---------------------------------------------------

  INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
  ,Existing_New_Id      
,Rest_SOAP_Id      
,ServiceType_Id      
,APIType_Id      
,APICategory_Id      
,APIRiskScore_Id      
,PartnerRiskScore_Id      
,DomainName_Id,     
ConsumerDC_Id,    
ProducerDC_Id,    
Platform,    
QValue1,    
QValue2,    
QValue3,    
QValue4,    
QValue5,    
RiskScore,
InternalServiceName,
ExternalServiceName ,
ConsumerDC,
ProducerDC
  )          
      
  values(      
  v_IntegrationIdIE,p_ServiceName,p_Purpose,p_Existing_New,p_ConsumerApplication,p_ProducerApplication,p_Is_APIGW_Request,p_Rest_Soap,p_Transformation,      
  p_Volume,      
  p_CreatedBy      
 ,NOW()      
,p_Existing_New_Id      
,p_Rest_SOAP_Id      
,p_ServiceType_Id      
,p_APIType_Id      
,p_APICategory_Id      
,p_APIRiskScore_Id      
,p_PartnerRiskScore_Id     
,p_DomainName_Id,    
p_ConsumerDC_Id,    
p_ProducerDC_Id,    
p_Platform,    
p_QValue1,    
p_QValue2,    
p_QValue3,    
p_QValue4,    
p_QValue5,    
p_RiskScore,
p_InternalServiceName,
p_ExternalServiceName ,
p_ConsumerDC,
p_ProducerDC
  );

  
END IF;

  

ELSEIF (p_IdentFlag='GetNewIntegrationDetailsById') THEN
          
    Select IntegrationId,ProjectName,ProjectId,ProjectManagerBTG,ProjectManagerIT,Status as workflowstatus, statusDescription as Status,Assign, AssignFrom,ConsumerApplication,BTG_USER,IT_USER,IT_ARCHITECTURE,ChannelID,ContainerName        
  
  -- CASE         
  -- WHEN Status=0 THEN 'Created'        
  -- WHEN Status=1 THEN 'Feedback'        
  -- WHEN Status=2 THEN 'Updated By User'        
  -- WHEN Status=3 THEN 'Review To ITUSER'        
  -- WHEN Status=4 THEN 'Review To ITARCHITECH'        
  -- WHEN Status=5 THEN 'Rejected'        
  -- WHEN Status=6 THEN 'Approved'        
  -- END AS Status          
  from TBL_API_HUNT_Integration     
    Inner Join TBL_API_statusMaster As SM on SM.statusCode=Status    
  where IntegrationId=p_IntegrationId          
  order by IntegrationId desc;
        
        
        
  select IntegrationId,ProjectManagerBTG,ProjectManagerIT,ProjectName,ProjectId,DATE_FORMAT(PlannedGoLiveDate, '%d-%m-%Y') as PlannedGoLiveDate ,BusinessJustification,BusinessSponsor,ExecutiveSponsor,CostCenterCode,UserJourneyDocumentPath,RDConceptNote,SequenceDiagram,ExpectedServiceSpecificationDocument,Status,Assign,AssignFrom,
  ConsumerApplication,BTG_USER,IT_USER,IT_ARCHITECTURE,ChannelID,ContainerName,CreatedAt  
  From TBL_API_HUNT_Integration           
  where IntegrationId =p_IntegrationId;
          
 select ServiceID,IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume      
 ,Existing_New_Id,Rest_SOAP_Id,ServiceType_Id,APIType_Id,APICategory_Id,APIRiskScore_Id,PartnerRiskScore_Id,DomainName_Id,ConsumerDC_Id                     
 ,ProducerDC_Id,Platform,QValue1,QValue2,QValue3,QValue4,QValue5 ,RiskScore,Classification,InternalServiceName ,ExternalServiceName,ConsumerDC,
ProducerDC                    
  from TBL_API_HUNT_ServiceDetails          
  where IntegrationId =p_IntegrationId;
          
  
ELSEIF (p_IdentFlag='UpdateIntegrationDetails') THEN
          
        
          
        
-- SET @Assign=(select Top 1 AssignFrom from tbl_API_hunt_Feedback where Integration_Id=@IntegrationId  order by Feedback_Id desc)        
        
  UPDATE TBL_API_HUNT_Integration SET ProjectManagerBTG=p_ProjectManagerBTG,          
  ProjectManagerIT=p_ProjectManagerIT,          
  ProjectName=p_ProjectName,          
  ProjectId=p_ProjectId,          
  PlannedGoLiveDate=p_PlannedGoLiveDate,          
  BusinessJustification=p_BusinessJustification,          
  BusinessSponsor = p_BusinessSponsor,          
  ExecutiveSponsor=p_ExecutiveSponsor,          
  CostCenterCode=p_CostCenterCode,          
  UserJourneyDocumentPath=p_UserJourneyDocumentFileName,          
  Status=p_Status,          
  UpdatedBy=p_UpdatedBy,          
  UpdatedAt=NOW(),        
  Assign=p_Assign,    
  AssignFrom=p_AssignFrom ,  
  ConsumerApplication =p_ConsumerApplication,
  BTG_USER = p_BTGUSER,
  IT_USER = p_ITUSER,
  IT_ARCHITECTURE = p_ITARCHITECTURE,  
  ChannelID =p_ChannelID ,
  ContainerName=p_ContainerName
  where IntegrationId=p_IntegrationId;
   -- ---------Feedback table------------------------------------------------------------------------       
  IF (p_Feedback Is not null And p_IntegrationId<>0) THEN
     INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom)         
     VALUES(p_IntegrationId,p_Feedback,p_Status,p_UpdatedBy,NOW(),1,p_Assign,p_AssignFrom);
        
  -- Set @FeedbackId =(SELECT SCOPE_IDENTITY() AS FeedbackId)    
    
  
ELSEIF (p_IdentFlag='UpdateServiceDetails') THEN
          
  IF (p_UpdateFlag='Update') THEN
          
  UPDATE TBL_API_HUNT_ServiceDetails SET IntegrationId=p_IntegrationId,          
  ServiceName=p_ServiceName,          
  Purpose=p_Purpose,          
  Existing_New=p_Existing_New,          
  ConsumerApplication=p_ConsumerApplication,          
  ProducerApplication=p_ProducerApplication,          
  Is_APIGW_Request=p_Is_APIGW_Request,          
  Rest_Soap=p_Rest_Soap,          
  Transformation=p_Transformation,          
  Volume=p_Volume,          
  UpdatedBy=p_UpdatedBy,          
  UpdatedAt=NOW()  ,        
  Existing_New_Id = p_Existing_New_Id,      
  Rest_SOAP_Id = p_Rest_SOAP_Id,      
  ServiceType_Id = p_ServiceType_Id,      
  APIType_Id = p_APIType_Id,      
  APICategory_Id = p_APICategory_Id,      
  APIRiskScore_Id = p_APIRiskScore_Id,        PartnerRiskScore_Id = p_PartnerRiskScore_Id,      
  DomainName_Id = p_DomainName_Id ,    
  ConsumerDC_Id= p_ConsumerDC_Id,    
  ProducerDC_Id=p_ProducerDC_Id,    
  Platform=p_Platform,    
  QValue1=p_QValue1,    
  QWeightage1=case when p_QValue1=1 then 10     
                   when p_QValue1=2 then 10    
       when p_QValue1=3 then 10    
       when p_QValue1=4 then 2 end,    
  QValue2=p_QValue2,    
  QWeightage2=case when p_QValue2=1 then 12.5     
                   when p_QValue2=2 then 5    
       when p_QValue2=3 then 0 end,    
  QValue3=p_QValue3,    
  QWeightage3=case when p_QValue3=1 then 12.5     
                   when p_QValue3=2 then 5    
       when p_QValue3=3 then 0 end,    
  QValue4=p_QValue4,    
  QWeightage4=case when p_QValue4=1 then 5     
      when p_QValue4=2 then 2 end,    
  QValue5=p_QValue5,    
  QWeightage5=case when p_QValue5=1 then 10     
                   when p_QValue5=2 then 2    
       when p_QValue5=3 then 0 end,    
  RiskScore=p_RiskScore,    
  Classification=p_Classification,
  InternalServiceName = p_InternalServiceName,
  ExternalServiceName = p_ExternalServiceName,
  ConsumerDC=p_ConsumerDC,
  ProducerDC=p_ProducerDC
  where ServiceID=p_ServiceID;
      
  -- select  @FeedbackId as FeedbackId    
  SELECT MAX(Feedback_Id) AS FeedbackId FROM tbl_API_hunt_Feedback;
    
  
ELSEIF (p_UpdateFlag='Insert') THEN
          
  INSERT INTO TBL_API_HUNT_ServiceDetails(IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume,CreatedBy,CreatedAt      
 ,Existing_New_Id      
,Rest_SOAP_Id      
,ServiceType_Id      
,APIType_Id      
,APICategory_Id      
,APIRiskScore_Id      
,PartnerRiskScore_Id      
,DomainName_Id    
,ConsumerDC_Id    
,ProducerDC_Id    
,Platform    
 ,QValue1    
 ,QValue2    
 ,QValue3    
 ,QValue4    
 ,QValue5    
 ,RiskScore    
 ,Classification 
 ,InternalServiceName 
 ,ExternalServiceName
  ,ConsumerDC
  ,ProducerDC
  )          
  values(p_IntegrationId,p_ServiceName,p_Purpose,p_Existing_New,p_ConsumerApplication,p_ProducerApplication,p_Is_APIGW_Request,p_Rest_Soap,p_Transformation,p_Volume,p_CreatedBy,NOW()      
,p_Existing_New_Id      
,p_Rest_SOAP_Id      
,p_ServiceType_Id      
,p_APIType_Id      
,p_APICategory_Id      
,p_APIRiskScore_Id      
,p_PartnerRiskScore_Id      
,p_DomainName_Id    
,p_ConsumerDC_Id    
,p_ProducerDC_Id    
,p_Platform    
,p_QValue1    
,p_QValue2    
,p_QValue3    
,p_QValue4    
,p_QValue5    
,p_RiskScore    
,p_Classification 
,p_InternalServiceName
,p_ExternalServiceName
,p_ConsumerDC
,p_ProducerDC
  );
            
  select ServiceID,IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume ,Existing_New_Id      
,Rest_SOAP_Id      
,ServiceType_Id      
,APIType_Id      
,APICategory_Id      
,APIRiskScore_Id      
,PartnerRiskScore_Id      
,DomainName_Id     
,ConsumerDC_Id    
 ,ProducerDC_Id    
 ,Platform    
 ,QValue1    
 ,QValue2    
 ,QValue3    
 ,QValue4    
 ,QValue5    
 ,RiskScore    
 ,Classification    
,p_FeedbackId as FeedbackId  
,InternalServiceName
,ExternalServiceName
,ConsumerDC
,ProducerDC
from TBL_API_HUNT_ServiceDetails          
  where IntegrationId =p_IntegrationId;
    
 -- set @FeedbackId as FeedbackId    
            
  
END IF;          
          
        
  
ELSEIF (p_IdentFlag='CheckProjectExist') THEN
        
  select Count(*) count1 from TBL_API_HUNT_Integration where ProjectId=p_ProjectId;
  
ELSEIF (p_IdentFlag='FillExectingServiceName') THEN
        
 -- select TBL_API_Main_ID as 'ExServiceId' ,COD_SERVICE_ID as 'ExServiceName'  from TBL_API_Main    
    select TBL_API_Main_ID as 'ExServiceId' ,OBP_SERVICE_URL_UAT as 'ExServiceName', COD_SERVICE_ID as 'ExCodServiceId'  from TBL_API_Main;
  
ELSEIF (p_IdentFlag='FillExectingSerNameOnId') THEN
  


  
   SELECT (Select RIGHT(OBP_SERVICE_URL_UAT, LOCATE('/',REVERSE(OBP_SERVICE_URL_UAT))-1) from TBL_API_Main where   TBL_API_Main_ID=p_servicenameId ) INTO v_InternalServiceId;

  
  SELECT (Select COD_SERVICE_ID from TBL_API_EXTERNALSERVICES where OBP_SERVICE_URL_UAT Like CONCAT('%', v_InternalServiceId, '%')) INTO v_ExternalServiceId;

  select     
     
  SERVICE_PROVIDER,    
  IFNULL(APIType. MisccdId,0) as APITypeId,    
  IFNULL(APICate. MisccdId,0) as APICatId,    
  IFNULL(DOMAINNAME. MisccdId,0) as DomainId,    
  IFNULL(RestSoap. MisccdId,0) as RestSoapId,    
  IFNULL(ProducerDC. MisccdId,0) as ProducerDC,    
  IFNULL(ServiceType. MisccdId,0) as ServiceType,    
  IFNULL(APIRiskClassify. MisccdId,0) as APIRiskClassify ,
  IFNULL( v_ExternalServiceId,'') As ExternalCoDServiceID
    
  from TBL_API_Main     
  Left join tbl_API_hunt_Misccd As APIType on APIType.CDValDesc=TBL_API_Main.API_TYPE    
  Left join tbl_API_hunt_Misccd As APICate on APICate.CDValDesc=TBL_API_Main.API_CAT    
  Left join tbl_API_hunt_Misccd As DOMAINNAME on DOMAINNAME.CDValDesc=TBL_API_Main.DOMAIN_NAME    
  Left join tbl_API_hunt_Misccd As RestSoap on RestSoap.CDValDesc=TBL_API_Main.SERVICE_INTERFACE_TYPE    
  Left join tbl_API_hunt_Misccd As ProducerDC on ProducerDC.CDValDesc=TBL_API_Main.FILLER_10     
  and ProducerDC.CDTP='Producer DC'    
  Left join tbl_API_hunt_Misccd As ServiceType on ServiceType.CDValDesc=TBL_API_Main.Service_Type    
  Left join tbl_API_hunt_Misccd As APIRiskClassify on APIRiskClassify.CDValDesc=TBL_API_Main.filler_02    
  where TBL_API_Main_ID=p_servicenameId;
  
ELSEIF (p_IdentFlag='FillExternalServiceName') THEN
        
    
	 -- select EXTERNALSERVICE_ID as 'ExServiceId' ,OBP_SERVICE_URL_UAT as 'ExServiceName'  from [TBL_API_EXTERNALSERVICES] 
   select COD_SERVICE_ID as 'ExServiceId' ,OBP_SERVICE_URL_UAT as 'ExServiceName', COD_SERVICE_ID as 'ExCodServiceId'  from TBL_API_EXTERNALSERVICES;
  
ELSEIF (p_IdentFlag='FillExternalService ') THEN
 
  
  
  SELECT (Select  RIGHT(OBP_SERVICE_URL_UAT, LOCATE('/',REVERSE(OBP_SERVICE_URL_UAT))-1) from TBL_API_EXTERNALSERVICES where (OBP_SERVICE_URL_UAT Like CONCAT('%', p_ExternalServiceText, '%') OR COD_SERVICE_ID Like CONCAT('%', p_ExternalServiceText, '%'))) INTO v_ExternalCodServiceId;


  SELECT COD_SERVICE_ID as 'OBP_SERVICE_URL_UAT',
  SERVICE_PROVIDER,    
  IFNULL(APIType. MisccdId,0) as APITypeId,    
  IFNULL(APICate. MisccdId,0) as APICatId,    
  IFNULL(DOMAINNAME. MisccdId,0) as DomainId,    
  IFNULL(RestSoap. MisccdId,0) as RestSoapId,    
  IFNULL(ProducerDC. MisccdId,0) as ProducerDC,    
  IFNULL(ServiceType. MisccdId,0) as ServiceType,    
  IFNULL(APIRiskClassify. MisccdId,0) as APIRiskClassify    
    
  from TBL_API_Main     
  Left join tbl_API_hunt_Misccd As APIType on APIType.CDValDesc=TBL_API_Main.API_TYPE    
  Left join tbl_API_hunt_Misccd As APICate on APICate.CDValDesc=TBL_API_Main.API_CAT    
  Left join tbl_API_hunt_Misccd As DOMAINNAME on DOMAINNAME.CDValDesc=TBL_API_Main.DOMAIN_NAME    
  Left join tbl_API_hunt_Misccd As RestSoap on RestSoap.CDValDesc=TBL_API_Main.SERVICE_INTERFACE_TYPE    
  Left join tbl_API_hunt_Misccd As ProducerDC on ProducerDC.CDValDesc=TBL_API_Main.FILLER_10     
  and ProducerDC.CDTP='Producer DC'    
  Left join tbl_API_hunt_Misccd As ServiceType on ServiceType.CDValDesc=TBL_API_Main.Service_Type    
  Left join tbl_API_hunt_Misccd As APIRiskClassify on APIRiskClassify.CDValDesc=TBL_API_Main.filler_02    
  where OBP_SERVICE_URL_UAT like CONCAT('%', v_ExternalCodServiceId, '%');
  
ELSEIF (p_IdentFlag='FillTestApiAuto') THEN
        
  -- select TBL_API_Main_ID as Id,OBP_SERVICE_URL_UAT as ServiceURL,FileName as 'path' from TBL_API_Main where  (PRODUCT_PROCESSOR_URL_UAT is not null and PRODUCT_PROCESSOR_URL_UAT<>'NA')    
    select distinct tblapimain.TBL_API_Main_ID as Id,tblapimain.OBP_SERVICE_URL_UAT as ServiceURL,tblapifilepath.FileName as 'path',SERVICE_INTERFACE_TYPE as apiCategory from TBL_API_Main tblapimain    
  Left Join Tbl_API_FilePath tblapifilepath on tblapimain.OBP_SERVICE_URL_UAT=tblapifilepath.OBP_SERVICE_URL_UAT     
  where(tblapimain.OBP_SERVICE_URL_UAT is not null and tblapimain.OBP_SERVICE_URL_UAT<>'NA');
  
ELSEIF (p_IdentFlag='FillHostApplication') THEN
        
   
   select Id As 'HostId',FullName  as 'HostAppName', ITGRCCode,ITGRCName from tbl_API_MstApplications;
  
ELSEIF (p_IdentFlag='FillHostingDC') THEN
        
   
   select Id As 'HostId',FullName  as 'HostAppName', HostingDC from tbl_API_MstApplications where Id=p_HostAppID;
  
ELSEIF (p_IdentFlag='FillHostingDCtext') THEN
        
   
   select Id As 'HostId',FullName  as 'HostAppName', HostingDC from tbl_API_MstApplications where FullName=p_HostApptext;
  
ELSEIF (p_IdentFlag='CheckConsumerAppName') THEN
        
  select Count(*) count1 from tbl_API_MstApplications where FullName=p_ConsumerApplication;
  
ELSEIF (p_IdentFlag='CheckProducerAppName') THEN
        
  select Count(*) count1 from tbl_API_MstApplications where FullName=p_ProducerApplication;
  
ELSEIF (p_IdentFlag='GetId') THEN
        
  SELECT IntegrationId FROM TBL_API_HUNT_Integration ORDER BY IntegrationId DESC;
  
ELSEIF (p_IdentFlag='GetDate') THEN
        
  SELECT CreatedAt FROM TBL_API_HUNT_Integration WHERE IntegrationId = p_IntegrationId;
  
END IF;
END IF;
 
-- select * from TBL_API_HUNT_ServiceDetails where IntegrationId='20'        
        
--  select ServiceID,IntegrationId,ServiceName,Purpose,Existing_New,ConsumerApplication,ProducerApplication,Is_APIGW_Request,Rest_Soap,Transformation,Volume          
--  from TBL_API_HUNT_ServiceDetails          
--  where IntegrationId =20
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_API_HUNT_NewExceptionManagement;
DELIMITER //
CREATE PROCEDURE sp_API_HUNT_NewExceptionManagement(IN p_CaseId int, IN p_OriginalOnboardingGASID VARCHAR(100), IN p_ExceptionRequestor VARCHAR(100), IN p_APIProjectName VARCHAR(200), IN p_APIProjectDescription VARCHAR(500), IN p_PartnersToBeIntegrated VARCHAR(500), IN p_ProductAPIToBeConsumed VARCHAR(500), IN p_RequestedException VARCHAR(500), IN p_ReasonForException VARCHAR(500), IN p_StartDate VARCHAR(100), IN p_EndDate VARCHAR(100), IN p_HowExceptionToBeImplemented VARCHAR(500), IN p_IdentFlag VARCHAR(100), IN p_ImpactOnBank VARCHAR(100), IN p_ExceptionLevel VARCHAR(100), IN p_currentUser varchar(100), IN p_ExceptionCaseID VARCHAR(100), IN p_ApproverUserID VARCHAR(100), IN p_ApproverType VARCHAR(100), IN p_ApproverLevel VARCHAR(100), IN p_BusinessVerticalHead varchar(100), IN p_BusinessGroupHead varchar(100), IN p_CIOGroup VARCHAR(100), IN p_ITDRMVerticalHead VARCHAR(100), IN p_ITDRMGroupHead VARCHAR(100), IN p_CISOGroup VARCHAR(100), IN p_ITVerticalHead VARCHAR(100), IN p_BSGVerticalHead VARCHAR(100), IN p_ComplianceVerticalHead VARCHAR(100), IN p_ISGVerticalHead VARCHAR(100), IN p_APEXSteeringCommittee VARCHAR(100))
BEGIN
  DECLARE v_CaseUniqueno int;
  DECLARE v_UniqueString varchar(100);
IF (p_IdentFlag = 'ExceptionManagement') THEN
         
		 
		 
                 
				INSERT INTO tbl_API_HUNT_ExceptionManagement      
				(      
		   OriginalOnboardingGASID,      
		   ExceptionRequestor,      
		   APIProjectName,      
		   APIProjectDescription,    
		   PartnersToBeIntegrated,      
		   ProductAPIToBeConsumed,      
		   RequestedException,      
		   ReasonForException,      
		   StartDate,      
		   EndDate,      
		   HowExceptionToBeImplemented,      
		   ImpactOnBank,      
		   ExceptionLevel,
		   createdBy,
		   createdDate      
				)      
				VALUES      
				(      
		   p_OriginalOnboardingGASID,      
		   p_ExceptionRequestor,      
		   p_APIProjectName,      
		   p_APIProjectDescription,      
		   p_PartnersToBeIntegrated,      
		   p_ProductAPIToBeConsumed,      
		   p_RequestedException,      
		   p_ReasonForException,      
		   p_StartDate,      
		   p_EndDate,      
		   p_HowExceptionToBeImplemented,      
		   p_ImpactOnBank,      
		   p_ExceptionLevel,
		   p_currentUser,
		   NOW()      
				);
		   
  
		 -- set @CaseUniqueno=scope_identity()   
		 SELECT (SELECT MAX(ID) FROM tbl_API_HUNT_ExceptionManagement) INTO v_CaseUniqueno;
    
		 SET v_UniqueString =CONCAT('APIGW', CAST(v_CaseUniqueno AS CHAR));

		 INSERT INTO tbl_API_HUNT_Exception_Audit_Log 
			 (
				CaseID ,
				ApprovalID ,
				`Status` ,
				createdBy ,
				createdDate 
			 )	

			 VALUES
			 (
				v_CaseUniqueno,
				 NULL ,
				'created',
				p_currentUser,
				NOW()      
			  );
    
		 update tbl_API_HUNT_ExceptionManagement    
		 SET OriginalOnboardingGASID=CONCAT('APIGW', REPLACE(DATE_FORMAT(IFNULL(createdDate,NOW()),'%d/%m/%y'),'/',''), (CASE WHEN CHAR_LENGTH(ID)=1 THEN '0000'
									WHEN CHAR_LENGTH(ID)=2 THEN '000' WHEN CHAR_LENGTH(ID)=3 THEN '00' WHEN CHAR_LENGTH(ID)=4 THEN '0' END),
									CAST(ID AS CHAR)) -- @UniqueString
		 where ID= v_CaseUniqueno;
    
		 select v_UniqueString  AS OriginalOnboardingGASID  
			
ELSEIF p_IdentFlag = 'GetExceptionLevel' THEN
      
		SELECT       
			IFNULL(apiid, '') AS APIId,      
			IFNULL(ImpactOnbank, '') AS ImpactOnBank,      
			IFNULL(Level, '') AS Level      
		FROM       
			tbl_API_HUNT_ExceptionLevel      
		WHERE       
			ImpactOnBank LIKE CONCAT('%', p_ImpactOnBank, '%')      
    
ELSEIF (p_IdentFlag = 'LevelNumber1') THEN
  
 
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=1 and ApproverType='Business' and ApproverLevel='VH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=1 and ApproverType='Business' and ApproverLevel='GH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=1 and ApproverType='CIO' and ApproverLevel='GH' 
	   
ELSEIF (p_IdentFlag = 'LevelNumber2') THEN
  
 
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=2 and ApproverType='Business' and ApproverLevel='VH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=2 and ApproverType='ITDRM' and ApproverLevel='VH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=2 and ApproverType='Business' and ApproverLevel='GH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=2 and ApproverType='CIO' and ApproverLevel='GH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=2 and ApproverType='ITDRM' and ApproverLevel='GH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=2 and ApproverType='CISO' and ApproverLevel='GH' 
	   
ELSEIF (p_IdentFlag = 'LevelNumber3') THEN
  
 
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=3 and ApproverType='APEX IT Steering Committee' -- and ApproverLevel='VH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=3 and ApproverType='APEX IT Steering Committee' and ApproverLevel='VH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=3 and ApproverType='APEX IT Steering Committee' and ApproverLevel='VH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=3 and ApproverType='APEX IT Steering Committee' and ApproverLevel='VH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=3 and ApproverType='APEX IT Steering Committee' and ApproverLevel='VH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=3 and ApproverType='APEX IT Steering Committee' and ApproverLevel='VH';
		select * from tbl_API_HUNT_MstExceptionApprovalMetrix where ExceptionLevel=3 and ApproverType='APEX IT Steering Committee' and ApproverLevel='VH'
  	   
ELSEIF p_IdentFlag='GetNewCaseId' THEN

		SELECT OriginalOnboardingGASID AS CaseID FROM tbl_API_HUNT_ExceptionManagement ORDER BY createdDate desc 
	
ELSEIF p_IdentFlag='ExceptionList' THEN

		SELECT /*'APIGW'+REPLACE(CONVERT(VARCHAR,ISNULL(PO.createdDate,GETDATE()),3),'/','')+(CASE WHEN LEN(PO.ID)=1 THEN '0000' 
		WHEN LEN(PO.ID)=2 THEN '000' WHEN LEN(PO.ID)=3 THEN '00' WHEN LEN(PO.ID)=4 THEN '0' END)+
		CAST(PO.ID AS VARCHAR)*/PO.OriginalOnboardingGASID AS CASEID,APIProjectName AS PartnerName,IFNULL(AU.Status,'') AS Status,
		IFNULL(DATE_FORMAT(PO.createdDate, '%d-%m-%Y'),'') AS DateCreated,IFNULL(DATE_FORMAT(PO.updatedDate, '%d-%m-%Y'),'') AS DateUpdated
		FROM tbl_API_HUNT_ExceptionManagement PO
		LEFT JOIN tbl_API_HUNT_Exception_Audit_Log AU ON AU.CASEID=AU.ID 
		ORDER BY PO.createdDate desc
	
END IF; 
	-- ELSE IF @IdentFlag='GetExceptionCaseApprovalMetrix'
	-- BEGIN
	-- 	SELECT ApproverUserID,ApproverName,ApproverType,ApproverLevel
	-- 	FROM tbl_API_HUNT_MstExceptionApprovalMetrix
	-- 	-- WHERE PartnerRiskClassification=@PartnerRiskClassification AND APIRiskClassification=@APIRiskClassification AND ApproverType=@ApproverType AND ApproverLevel=@ApproverLevel;
	-- END
	ELSEIF p_IdentFlag='ExceptionLevels' THEN

			INSERT INTO tbl_API_HUNT_PartnerExceptionApprovalList      
				(      
	       ExceptionCaseID,      
		   ApproverUserID,    
		   `Status`,    
		   ApproverType,      
		   ApproverLevel,      
		   CreatedBy,
		    DateCreated
	            );
			SELECT p_ExceptionCaseID,p_BusinessVerticalHead,'Created','Business','VH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_BusinessGroupHead,'Created','Business','GH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_CIOGroup,'Created','Business','FH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_ITDRMVerticalHead,'Created','ITDRM','VH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_ITDRMGroupHead,'Created','ITDRM','GH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_CISOGroup,'Created','CISO','FH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_ITVerticalHead,'Created','IT','VH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_BSGVerticalHead,'Created','BSG','VH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_ComplianceVerticalHead,'Created','Compliance','VH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_ISGVerticalHead,'Created','ISG','VH',p_currentUser,NOW() UNION
			SELECT p_ExceptionCaseID,p_APEXSteeringCommittee,'Created','APEX','VH',p_currentUser,NOW() 

				-- VALUES      
				-- (  
		  -- @ExceptionCaseID,    
		  -- @ApproverUserID,
		  --  'Created',      
		  -- @ApproverType,      
		  -- @ApproverLevel,
		  -- @currentUser,
    --       GETDATE()      
				-- )  
	
END IF;
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_API_HUNT_URLMaster;
DELIMITER //
CREATE PROCEDURE SP_API_HUNT_URLMaster(IN p_ID int, IN p_URLName varchar(100))
BEGIN
select * from TBL_API_URLMaster where (ID=p_ID or URLName=p_URLName) and Status=1;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_API_HUNT_WorkFlowApprovalProcess;
DELIMITER //
CREATE PROCEDURE SP_API_HUNT_WorkFlowApprovalProcess(IN p_IdentFlag varchar(50), IN p_IntegrationId int, IN p_AssignTo varchar(20), IN p_AssignFrom varchar(20), IN p_Feedback LONGTEXT, IN p_WorkflowStatus int, IN p_UserId varchar(50), IN p_IsActive TINYINT(1))
BEGIN
-- IF(@IdentFlag='AddFeedback')    
 -- BEGIN 
   
 --   INSERT INTO tbl_API_hunt_Feedback (Integration_Id,Feedback_Details,Status,Created_By,Created_Date,IsActive,AssignTo,AssignFrom) 
	-- VALUES(@IntegrationId,@Feedback,@WorkflowStatus,@UserId,GETDATE(),@IsActive,@AssignTo,@AssignFrom)


	-- UPDATE TBL_API_HUNT_Integration SET Assign= @AssignTo, Status = @WorkflowStatus,UpdatedBy=@UserId,UpdatedAt=GETDATE() WHERE IntegrationId=@IntegrationId

	-- SELECT max(Feedback_Id) AS FeedbackId from tbl_API_hunt_Feedback
		
 -- END
IF (p_IdentFlag='FeedbackCount') THEN

	 Select  Count(Integration_Id)As FeedbackCount,
	 AssignFrom From tbl_API_hunt_Feedback 
	 Where (Integration_Id=p_IntegrationId and Created_By=p_UserId AND Status=p_WorkflowStatus) 
	 group by AssignFrom;

    
ELSEIF (p_IdentFlag='FeedbackHistory') THEN

	 SELECT EmpName,
	 Feedback_Details ,
	 statusDescription as Status,
	 AssignFrom As Role,
	 DATE_FORMAT(Created_Date, '%d/%m/%y') AS Created_Date
	 -- CASE 
  --    WHEN Status=0 THEN 'Created'
  --    WHEN Status=1 THEN 'Feedback'
  --    WHEN Status=2 THEN 'Updated By User'
  --    WHEN Status=3 THEN 'Review To ITUSER'
  --    WHEN Status=4 THEN 'Review To ITARCHITECH'
  --    WHEN Status=5 THEN 'Rejected'
  --    WHEN Status=6 THEN 'Approved'
  --  END AS Status  ,
	
	
	 FROM tbl_API_hunt_Feedback   FB
	 LEFT JOIN UserMaster AS UM On UM.EmpCode=FB.Created_By
	 Inner Join TBL_API_statusMaster As SM on SM.statusCode=Status
	 where Integration_Id=p_IntegrationId

	 order by Feedback_Id Asc;

    
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_Insert_Data_API_Main_Schedule_ActivityLog;
DELIMITER //
CREATE PROCEDURE SP_Insert_Data_API_Main_Schedule_ActivityLog(IN p_Errormsg LONGTEXT, IN p_Status Varchar(100), IN p_ModuleName varchar(50))
BEGIN
INSERT INTO Tbl_API_Main_Schedule_ActivityLog    
           (Errormsg    
           ,`Status`  
     ,ModuleName  
           ,InsertedOn    
           )    
     VALUES    
        (p_Errormsg    
     ,p_Status  
  ,p_ModuleName  
     ,NOW());
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_Login_New;
DELIMITER //
CREATE PROCEDURE sp_Login_New(IN p_Type int, IN p_Empcode varchar(50), IN p_UnqId DECIMAL(18,0), IN p_AssetCode varchar(100), IN p_IpAddress varchar(100))
BEGIN
  DECLARE v_Brcode int;
  DECLARE v_Brname varchar(100);
  DECLARE v_ProfileId int;
  DECLARE v_ProfileName varchar(150);
  DECLARE v_Empname varchar(100);
  DECLARE v_Id DECIMAL(18,0);





                    
                    
 IF (p_Type = 1) THEN
                    
   -- Unsuccessful Login                    
   Update UserMaster                    
   set UnsuccessfulAttempts =(select IFNULL(UnsuccessfulAttempts,0) + 1 from UserMaster where EmpCode=p_Empcode)                    
   where EmpCode = p_Empcode;
                       
   SELECT UnsuccessfulAttempts from UserMaster where EmpCode = p_Empcode;
                       
                       
   insert into LoginAttempts                    
    (Empcode,Empname,LoginTime,Attempts,ProfileId,ProfileName,Brcode,Brname,Flag,AssetCode,IpAddress)
    select p_Empcode,Empname,NOW(),1,ProfileId,ProfileDescription,BranchCode,BranchName,'Unsuccessful',p_AssetCode,p_IpAddress                   
    from UserMaster where EmpCode = p_Empcode         and IFNULL(Flag,'') <> 'Resigned';
                 
                       
  
END IF;                    
                      
  -- Successful Login                    
  IF (p_Type = 2) THEN
                    
   Update UserMaster                    
   set UnsuccessfulAttempts=0,LastLoginDate=NOW(),LoggedIn='True',Locked='False',LockedDate=null,AssetCode=p_AssetCode,IpAddress=p_IpAddress                    
   where EmpCode = p_Empcode;
                       
   insert into LoginAttempts                    
    (Empcode,Empname,LoginTime,Attempts,ProfileId,ProfileName,Brcode,Brname,Flag,AssetCode,IpAddress)
   select p_Empcode, Empname,NOW(),1,ProfileId,ProfileDescription,BranchCode,BranchName,'Successful',p_AssetCode,p_IpAddress                   
   from UserMaster where EmpCode = p_Empcode      and IFNULL(Flag,'') <> 'Resigned';
                       
   
   SELECT (SELECT Id from LoginAttempts where Empcode = p_Empcode and LoginTime <> '' order by LoginTime desc) INTO v_Id;
   select v_Id;
                       
  
END IF;                    
                    
  -- Lock User                    
 IF (p_Type = 3) THEN
                    
   Update UserMaster                    
   set Locked='True',Active='False',LockedDate = NOW() -- , [Enabled]='False'                  
   where EmpCode = p_Empcode    and IFNULL(Flag,'') <> 'Resigned';
                       
                       
                       
  
END IF;                    
  -- Successful Logout                    
 IF (p_Type = 4) THEN
                    
   Update UserMaster                    
   set LastLogoutDate = NOW(),LoggedIn='False'                    
   where EmpCode = p_Empcode   and IFNULL(Flag,'') <> 'Resigned';
                       
   Update LoginAttempts                    
   set LogoutTime = NOW() where Id = p_UnqId and Empcode = p_Empcode    and IFNULL(Flag,'') <> 'Resigned';
                       
  
END IF;                    
                      
                     
  IF (p_Type = 5) THEN
                    
   -- select a.EmpCode,a.ProfileDescription,a.ProfileId,a.BranchCode,a.Id,a.Active             
   -- ,case when a.Locked='True' then 'Locked'            
   -- when a.Dormant='True' then 'Dormant'            
   -- when a.[Enabled]='False' then 'Disabled'            
   -- when a.Active='True' then 'Active'            
   -- end as 'Status',a.LastLoginDate,            
   -- a.LastLogoutDate             --from UserMaster a where LTRIM(RTRIM(a.Empcode)) = @Empcode             
   -- and ISNULL(flag,'') <> 'Resigned'           
             
      select a.EmpName, a.EmpCode,a.ProfileDescription,a.ProfileId,a.BranchCode,a.Id,a.Active             
   ,case when a.Locked='True' then 'Locked'            
   when a.Dormant='True' then 'Dormant'            
   when a.Enabled='False' then 'Disabled'            
   when a.Active='True' then 'Active'            
    end as 'Status',a.LastLoginDate,            
    a.LastLogoutDate            
 ,b.EmpRole as User_Role          
   from UserMaster a      
   Left join TBL_OVI_RM_Hierarchy_Mapping b       
   on a.EmpCode = b.EmpCode          
   where LTRIM(RTRIM(a.Empcode)) = p_Empcode        
   and IFNULL(flag,'') <> 'Resigned';



END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_Select_Mofee_Url;
DELIMITER //
CREATE PROCEDURE SP_Select_Mofee_Url()
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	

    -- Insert statements for procedure here
	select Id,Url from tbl_Mofee_Url;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS SP_Select_NewIntegrationdpdata;
DELIMITER //
CREATE PROCEDURE SP_Select_NewIntegrationdpdata()
BEGIN


    SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'Existing_New' AND `Status` = 1;  -- 0


   SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'Rest_Soap' AND `Status` = 1;  -- 1


	SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'Service Type' AND `Status` = 1;  -- 2


	SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'API Risk Score' AND `Status` = 1;  -- 3


	SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'Partner Risk Score' AND `Status` = 1;  -- 4



   SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'API Category' AND `Status` = 1;  -- 5
  
  
   SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'Domain Name' AND `Status` = 1;  -- 6
  
  
    SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'API Type' AND `Status` = 1;  -- 7


	SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'Consumer DC' AND `Status` = 1;  -- 8


	SELECT 0 As Id, '-- Select --' As Val   
    UNION ALL   
    select MisccdId As ID ,CDValDesc AS 'Val' from tbl_API_hunt_Misccd   WHERE CDTP = 'Producer DC' AND `Status` = 1;  -- 9

 -- SELECT MisccdId As [Id], CDValDesc As [Val] FROM EMA_Misccd WHERE CDTP = 'IsActive' AND [Status] = 1  

    select QID As ID ,Question, QuestionType QType,IFNULL(APICategory,'Internal') as APICategory ,IFNULL(APICategoryId,0)As APICategoryId from TBL_API_HUNT_ServiceQuestion where Status=1 order by ID asc;

    select   0 As Id, '-- Select --' As Val ,  0 as 'Weightage',0 as 'QID'
    UNION ALL
	select ID ,Options AS 'Val',Weightage,QID from TBL_API_HUNT_QuestionData where Status=1 order by ID asc;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS Spx_EmailAPI;
DELIMITER //
CREATE PROCEDURE Spx_EmailAPI(IN p_From varchar(50), IN p_To varchar(50), IN p_Subject varchar(50), IN p_Body varchar(50))
BEGIN
INSERT INTO TBL_Email_Extender
		(`From`, `To`, Subject, Body,AddedOn)
		VALUES
		(p_From, p_To, p_Subject,p_Body,NOW());
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS Spx_EmailAPI_Details;
DELIMITER //
CREATE PROCEDURE Spx_EmailAPI_Details()
BEGIN
select * from TBL_Email_Extender;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS Usp_api_hunt_FillUser;
DELIMITER //
CREATE PROCEDURE Usp_api_hunt_FillUser(IN p_role VARCHAR(30))
BEGIN
SELECT '0' As EmpCode,
'-- Select --' As EmpName   
UNION ALL
SELECT 
AAU.EmpCode,
EmpName
-- AAU.Role
FROM tbl_API_hunt_USER AAU
INNER JOIN UserMaster as UM On UM.EmpCode=AAU.EmpCode
 -- order by EmpCode Asc
WHERE Role = p_role order by EmpCode Asc;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS Usp_API_HUNT_ExceptionManagement;
DELIMITER //
CREATE PROCEDURE Usp_API_HUNT_ExceptionManagement(IN p_IdentFlag varchar(50), IN p_CaseID int, IN p_OriginalOnboardingGASID VARCHAR(50), IN p_ExceptionRequestor VARCHAR(100), IN p_APIProjectName varchar(255), IN p_APIProjectDescription VARCHAR(100), IN p_PartnersToBeIntegrated VARCHAR(100), IN p_ProductAPIToBeConsumed VARCHAR(100), IN p_RequestedException VARCHAR(100), IN p_ReasonForException VARCHAR(100), IN p_StartDate date, IN p_EndDate date, IN p_HowExceptionToBeImplemented VARCHAR(100), IN p_ImpactOnBank varchar(50), IN p_ExceptionLevel VARCHAR(100))
BEGIN
IF p_IdentFlag='ExceptionManagement' THEN

		INSERT INTO tbl_API_HUNT_ExceptionManagement(CaseID,OriginalOnboardingGASID,ExceptionRequestor,APIProjectName,APIProjectDescription,PartnersToBeIntegrated,ProductAPIToBeConsumed,RequestedException
		,ReasonForException,StartDate,EndDate,HowExceptionToBeImplemented,ImpactOnBank,ExceptionLevel)
		values(
		p_CaseID,p_OriginalOnboardingGASID,p_ExceptionRequestor,p_APIProjectName,p_APIProjectDescription,p_PartnersToBeIntegrated,p_ProductAPIToBeConsumed,p_RequestedException
		,p_ReasonForException,p_StartDate,p_EndDate,p_HowExceptionToBeImplemented,p_ImpactOnBank,p_ExceptionLevel);
		
	
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS Usp_API_HUNT_ExceptionManagementDetails;
DELIMITER //
CREATE PROCEDURE Usp_API_HUNT_ExceptionManagementDetails(IN p_IdentFlag varchar(50), IN p_CaseID int, IN p_OriginalOnboardingGASID VARCHAR(50), IN p_ExceptionRequestor VARCHAR(100), IN p_APIProjectName varchar(255), IN p_APIProjectDescription VARCHAR(100), IN p_PartnersToBeIntegrated VARCHAR(100), IN p_ProductAPIToBeConsumed VARCHAR(100), IN p_RequestedException VARCHAR(100), IN p_ReasonForException VARCHAR(100), IN p_StartDate date, IN p_EndDate date, IN p_HowExceptionToBeImplemented VARCHAR(100), IN p_ImpactOnBank varchar(50), IN p_ExceptionLevel VARCHAR(100))
BEGIN
IF p_IdentFlag='ExceptionManagement' THEN


		INSERT INTO tbl_API_HUNT_ExceptionManagement(OriginalOnboardingGASID,ExceptionRequestor,APIProjectName,APIProjectDescription,PartnersToBeIntegrated,ProductAPIToBeConsumed,RequestedException

		,ReasonForException,StartDate,EndDate,HowExceptionToBeImplemented,ImpactOnBank,ExceptionLevel)

		values(

		p_OriginalOnboardingGASID,p_ExceptionRequestor,p_APIProjectName,p_APIProjectDescription,p_PartnersToBeIntegrated,p_ProductAPIToBeConsumed,p_RequestedException

		,p_ReasonForException,p_StartDate,p_EndDate,p_HowExceptionToBeImplemented,p_ImpactOnBank,p_ExceptionLevel);


	
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS Usp_API_HUNT_PartnerOffboarding;
DELIMITER //
CREATE PROCEDURE Usp_API_HUNT_PartnerOffboarding(IN p_Exit_scenario VARCHAR(50), IN p_IdentFlag VARCHAR(50), IN p_Partner_Name VARCHAR(100), IN p_API_Name VARCHAR(100), IN p_Remark VARCHAR(100), IN p_partner_checkilist VARCHAR(100), IN p_fileUpload VARCHAR(100))
BEGIN
IF p_IdentFlag = 'SavePartnerOffboardingDetails' THEN

        INSERT INTO tbl_API_HUNT_PartnerOffboardning_getdetails 
            (Exit_scenario, Partner_Name, API_Name, Remark, partner_checkilist, fileUpload) 
        VALUES 
            (p_Exit_scenario, p_Partner_Name, p_API_Name, p_Remark, p_partner_checkilist, p_fileUpload);
    
END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS Usp_API_HUNT_PartnerOnBoarding;
DELIMITER //
CREATE PROCEDURE Usp_API_HUNT_PartnerOnBoarding(IN p_PartnetOnboading_ID VARCHAR(50), IN p_Partner_Name VARCHAR(50), IN p_Project_Description VARCHAR(100), IN p_TentativeGoLive_Date DATE, IN p_PartnerType VARCHAR(100), IN p_PartnerEntityType VARCHAR(100), IN p_PartnerTPRM_Application VARCHAR(100), IN p_Partnerrisk_score VARCHAR(100), IN p_Partnerrisk VARCHAR(100), IN p_API_name VARCHAR(100), IN p_API_risk VARCHAR(100), IN p_APIrisk_score VARCHAR(100), IN p_IdentFlag varchar(50), IN p_PartnerRiskClassification VARCHAR(100), IN p_APIRiskClassification VARCHAR(100), IN p_ApproverType VARCHAR(100), IN p_ApproverLevel VARCHAR(100), IN p_APIName VARCHAR(100), IN p_HOPP_FH varchar(100), IN p_HOPP_VH varchar(100), IN p_HOPP_GH varchar(100), IN p_HOB_FH varchar(100), IN p_HOB_VH varchar(100), IN p_HOB_GH varchar(100), IN p_HODB_FH varchar(100), IN p_HODB_VH varchar(100), IN p_HODB_GH varchar(100), IN p_HOISG_FH varchar(100), IN p_HOISG_VH varchar(100), IN p_HOISG_GH varchar(100), IN p_HOITDRM_FH varchar(100), IN p_HOITDRM_VH varchar(100), IN p_HOITDRM_GH varchar(100), IN p_PartnerXMl LONGTEXT, IN p_ApproverUserID varchar(100), IN p_ApproverID VARCHAR(100))
BEGIN


  DECLARE v_TCOUNT INT DEFAULT 0;
  DECLARE v_createdBy VARCHAR(100) DEFAULT NULL;
  DECLARE v_ApprovalID VARCHAR(100) DEFAULT NULL;
  DECLARE v_FeedbackID VARCHAR(100) DEFAULT NULL;
  DECLARE v_ActionTaken VARCHAR(100) DEFAULT NULL;
  DECLARE v_NewID INT DEFAULT 0;
  DECLARE v_ApproverUserIDCNT INT DEFAULT 0;
  DECLARE v_TrailCount INT DEFAULT NULL;
  DECLARE v_ApproverLevel TEXT;
  DECLARE v_PartnetOnboading_ID TEXT;
-- SET @PartnetOnboading_ID=REPLACE(@PartnetOnboading_ID,'APIGW0000000000','')
	-- SET @PartnetOnboading_ID=REPLACE(@PartnetOnboading_ID,'APIGW','')

	/*SET @PartnetOnboading_ID=(CASE WHEN @PartnetOnboading_ID LIKE '%APIGW%' AND LEN(@PartnetOnboading_ID) <= 5 THEN 'APIGW0000000000' + RIGHT('00000' + CAST(@PartnetOnboading_ID AS VARCHAR), 5)
                                   ELSE CAST(@PartnetOnboading_ID AS VARCHAR) END)*/

	-- SET @PartnetOnboading_ID=(CASE WHEN @PartnetOnboading_ID LIKE '%APIGW%' THEN RIGHT(@PartnetOnboading_ID, LEN(@PartnetOnboading_ID) - 13) ELSE @PartnetOnboading_ID END)
	

	SET v_TCOUNT = CAST(RIGHT(p_PartnetOnboading_ID,4) AS signed);

	SET v_PartnetOnboading_ID =(CASE WHEN p_PartnetOnboading_ID LIKE '%APIGW%' AND v_TCOUNT>999 THEN RIGHT(p_PartnetOnboading_ID, CHAR_LENGTH(p_PartnetOnboading_ID) - 12)
			 WHEN p_PartnetOnboading_ID LIKE '%APIGW%' AND v_TCOUNT>99 THEN RIGHT(p_PartnetOnboading_ID, CHAR_LENGTH(p_PartnetOnboading_ID) - 13)
			 WHEN p_PartnetOnboading_ID LIKE '%APIGW%' AND v_TCOUNT>9 THEN RIGHT(p_PartnetOnboading_ID, CHAR_LENGTH(p_PartnetOnboading_ID) - 14)
			 WHEN p_PartnetOnboading_ID LIKE '%APIGW%' AND v_TCOUNT<9 THEN RIGHT(p_PartnetOnboading_ID, CHAR_LENGTH(p_PartnetOnboading_ID) - 15) ELSE p_PartnetOnboading_ID END );

	

	SELECT (SELECT ApproverLevel FROM tbl_API_HUNT_MstPartnerCaseApprovalMetrix WHERE ApproverUserID=p_ApproverUserID) INTO v_ApproverLevel;
	-- SET @ApproverLevel=(SELECT TOP 1 Role FROM tbl_API_hunt_USER WHERE EmpCode=@ApproverUserID)

	IF EXISTS (SELECT * FROM tbl_API_HUNT_Partner_Onboarding WHERE PartnetOnboading_ID = p_PartnetOnboading_ID) AND p_IdentFlag='UpdatePartnerboarding' THEN

		UPDATE tbl_API_HUNT_Partner_Onboarding
		SET Partner_Name=p_Partner_Name,
			Project_Description=p_Project_Description,
			TentativeGoLive_Date=p_TentativeGoLive_Date,
			PartnerType=p_PartnerType,
			PartnerEntityType=p_PartnerEntityType,
			PartnerTPRM_Application=p_PartnerTPRM_Application,
			Partnerrisk_score=p_Partnerrisk_score,
			Partnerrisk=p_Partnerrisk,
			API_name=p_API_name,
			API_risk=p_API_risk,
			APIrisk_score=p_APIrisk_score
		WHERE PartnetOnboading_ID = p_PartnetOnboading_ID;
	
ELSEIF (p_IdentFlag='AddPartnerboarding') THEN

		INSERT INTO tbl_API_HUNT_Partner_Onboarding(Partner_Name,Project_Description,TentativeGoLive_Date,PartnerType,PartnerEntityType,PartnerTPRM_Application,Partnerrisk_score,Partnerrisk,API_name,API_risk,APIrisk_score,statusCode)
		SELECT p_Partner_Name,p_Project_Description,p_TentativeGoLive_Date,p_PartnerType,p_PartnerEntityType,p_PartnerTPRM_Application,p_Partnerrisk_score,p_Partnerrisk,p_API_name,p_API_risk,p_APIrisk_score,1;

		SELECT (SELECT MAX(PartnetOnboading_ID) FROM tbl_API_HUNT_Partner_Onboarding) INTO v_PartnetOnboading_ID;

		INSERT INTO tbl_API_HUNT_caseapprovalMatrix (caseId,HOPP_FH,HOPP_VH,HOPP_GH,HOB_FH,HOB_VH,HOB_GH,HODB_FH,HODB_VH,HODB_GH,HOISG_FH,HOISG_VH,HOISG_GH,HOITDRM_FH,HOITDRM_VH,HOITDRM_GH)
		SELECT p_PartnetOnboading_ID,p_HOPP_FH,p_HOPP_VH,p_HOPP_GH,p_HOB_FH,p_HOB_VH,p_HOB_GH,p_HODB_FH,p_HODB_VH,p_HODB_GH,p_HOISG_FH,p_HOISG_VH,p_HOISG_GH,p_HOITDRM_FH,p_HOITDRM_VH,p_HOITDRM_GH;
	
ELSEIF p_IdentFlag='GetNewCaseId' THEN

		SELECT MAX(PartnetOnboading_ID)+1 AS CaseID FROM tbl_API_HUNT_Partner_Onboarding;
	
ELSEIF p_IdentFlag='AddPartner' THEN

		
		SELECT (SELECT MAX(PartnetOnboading_ID)+1 FROM tbl_API_HUNT_Partner_Onboarding) INTO v_NewID;

		-- INSERT INTO tbl_API_HUNT_Partner_Onboarding(PartnetOnboading_ID,Partner_Name,Project_Description,TentativeGoLive_Date,PartnerType,PartnerEntityType,PartnerTPRM_Application,Partnerrisk_score,Partnerrisk,statusCode,created_By,created_date,API_risk,
-- 												 AttachedJourneyDocuments,APIRiskAssessment,OtherDocument)
-- 		SELECT v_NewID,TBL.COL.value('PartnerName[1]','VARCHAR(100)') AS PartnerName,
-- 		TBL.COL.value('projectDescription[1]','VARCHAR(100)') AS projectDescription,
-- 		CASE 
-- 			WHEN TBL.COL.value('TentativeGoLiveDate[1]','VARCHAR(100)')='0001-01-01T00:00:00' THEN NULL 
-- 			ELSE TBL.COL.value('TentativeGoLiveDate[1]','DATE') 
-- 		END AS TentativeGoLiveDate,
-- 		TBL.COL.value('PartnerType[1]','VARCHAR(100)') AS PartnerType,
-- 		TBL.COL.value('PartnerEntityType[1]','VARCHAR(100)') AS PartnerEntityType,
-- 		TBL.COL.value('PartnerTPRMAssesmetApplicability[1]','VARCHAR(100)') AS PartnerTPRMAssesmetApplicability,
-- 		TBL.COL.value('PartnerRiskScore[1]','VARCHAR(100)') AS PartnerRiskScore,
-- 		TBL.COL.value('PartnerRisk[1]','VARCHAR(100)') AS PartnerRisk,12,
-- 		TBL.COL.value('createdBy[1]','VARCHAR(100)') AS createdBy,
-- 		NOW(),
-- 		TBL.COL.value('APIRisk[1]','VARCHAR(100)') AS APIRisk,
-- 		TBL.COL.value('AttachedJourneyDocuments[1]','VARCHAR(150)') AS AttachedJourneyDocuments,
-- 		TBL.COL.value('APIRiskAssessmentSheet[1]','VARCHAR(150)') AS APIRiskAssessmentSheet,
-- 		TBL.COL.value('OtherDocument[1]','VARCHAR(150)') AS OtherDocument
-- 		from p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL);

-- 		SELECT (SELECT MAX(PartnetOnboading_ID) FROM tbl_API_HUNT_Partner_Onboarding) INTO v_PartnetOnboading_ID;
-- 		SET v_PartnetOnboading_ID =v_NewID;

		-- INSERT INTO tbl_API_HUNT_PO_ApiDeatil(CaseId,APIName,APIRisk,APIRiskScore)
-- 		SELECT p_PartnetOnboading_ID,TBL.COL.value('APIName[1]','varchar(100)') AS APIName,
-- 		TBL.COL.value('APIRisk[1]','Varchar(100)') AS APIRisk ,
-- 		TBL.COL.value('APIRiskScore[1]','Varchar(100)') AS APIRiskScore
-- 		from p_PartnerXMl.nodes('/PartnerOnboarding/lstApiDeatil/ApiDeatil') AS TBL(COL);

		-- INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence)
-- 		SELECT p_PartnetOnboading_ID,
-- 		TBL.COL.value('Department[1]','VARCHAR(100)') AS Department,
-- 		TBL.COL.value('ApproverLevel[1]','VARCHAR(100)') AS ApproverLevel,
-- 		TBL.COL.value('ApproverUserID[1]','VARCHAR(100)') AS ApproverUserID,
-- 		TBL.COL.value('Sequence[1]','VARCHAR(100)') AS Sequence
-- 		from p_PartnerXMl.nodes('/PartnerOnboarding/PartnerApproval/AddPartnerCaseApprovalMetrixList') AS TBL(COL);

		-- INSERT INTO tbl_API_HUNT_Audit_log(CaseID,ApprovalID,status,createdBy,createdDate)
-- 		SELECT p_PartnetOnboading_ID,NULL,
-- 		TBL.COL.value('Action[1]','VARCHAR(100)') AS Action,
-- 		TBL.COL.value('createdBy[1]','VARCHAR(100)') AS createdBy,
-- 		NOW()
-- 		from p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL);

		SELECT p_PartnetOnboading_ID AS CaseID;
	
ELSEIF p_IdentFlag='GetPartnerType' THEN

		select DISTINCT PartnerType from tbl_API_HUNT_MSTPartnerType;
	
ELSEIF p_IdentFlag='GetPartnerCaseApprovalMetrix' THEN

		SELECT PartnerRiskClassification,APIRiskClassification,ApproverType,ApproverLevel,ApproverUserID,ApproverName 
		FROM tbl_API_HUNT_MstPartnerCaseApprovalMetrix;
		-- WHERE PartnerRiskClassification=@PartnerRiskClassification AND APIRiskClassification=@APIRiskClassification AND ApproverType=@ApproverType AND ApproverLevel=@ApproverLevel;
	
ELSEIF p_IdentFlag='GetPartnerCaseApprovalMetrix' THEN

		SELECT PartnerRiskClassification,APIRiskClassification,ApproverType,ApproverLevel,ApproverUserID,ApproverName 
		FROM tbl_API_HUNT_MstPartnerCaseApprovalMetrix;
		-- WHERE PartnerRiskClassification=@PartnerRiskClassification AND APIRiskClassification=@APIRiskClassification AND ApproverType=@ApproverType AND ApproverLevel=@ApproverLevel;
	
ELSEIF p_IdentFlag='GetPartnerList' THEN

		
			SELECT /*(CASE WHEN LEN(PO.PartnetOnboading_ID) <= 5 THEN 'APIGW0000000000' + RIGHT('00000' + CAST(PO.PartnetOnboading_ID AS VARCHAR), 5)
                    ELSE CAST(PO.PartnetOnboading_ID AS VARCHAR) END) AS PartnetOnboading_ID,*/
					-- 'APIGW'+REPLACE(CONVERT(VARCHAR,PO.created_date,3),'/','')+'00'+CAST(PO.PartnetOnboading_ID AS VARCHAR) AS PartnetOnboading_ID,
					CONCAT('APIGW', REPLACE(DATE_FORMAT(PO.created_date, '%d/%m/%y'),'/',''), (CASE WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=1 THEN '0000' 
					WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=2 THEN '000' WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=3 THEN '00' WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=4 THEN '0' END))+
					CAST(PO.PartnetOnboading_ID AS CHAR) AS PartnetOnboading_ID,PO.PartnetOnboading_ID AS CaseID,
					PO.Partner_Name,
			-- ST.STATUS,
			CASE 
				WHEN FEEDCOUNT.FeedbackCount > 0 AND FEEDCOUNT.FeedbackReplyCount > 0 AND FEEDCOUNT.FeedbackCount != FEEDCOUNT.FeedbackReplyCount THEN 'Awaiting For Reply'
				-- WHEN TCOUNT.ApprovalCount = 9 AND TCOUNT.ApprovedCount = 9 THEN 'In Progress'
				WHEN TCOUNT.ApprovalCount=TCOUNT.ApprovedCount THEN 'Approved'
				WHEN TCOUNT.RejectCount > 0 THEN 'Reject'
				WHEN TCOUNT.FeedbackCount > 0 AND TCOUNT.FeedbackReplyCount > 0 AND TCOUNT.FeedbackCount = TCOUNT.FeedbackReplyCount THEN 'Feedback'
				WHEN TCOUNT.FeedbackReplyCount < TCOUNT.FeedbackCount THEN 'Feedback Pending'
				WHEN ST.STATUS = 'Drafted' OR ST.STATUS = 'Awaiting For Reply' THEN ST.STATUS -- modify this line
				WHEN TCOUNT.ApprovalCount = 0 THEN 'Created'
				ELSE 'In Progress' 
			END AS StatusDescription,
			IFNULL(PO.created_By, '') AS createdBy,
			IFNULL(CONCAT(DATE_FORMAT(PO.created_date, '%d/%m/%Y'), ' ', CAST(PO.created_date AS CHAR)), '') AS createdDate,
			IFNULL(CONCAT(DATE_FORMAT(PO.Updated_date, '%d/%m/%Y'), ' ', CAST(PO.Updated_date AS CHAR)), '') AS UpdatedDate,
			IFNULL(DATEDIFF(PO.Updated_date, PO.created_date), 0) AS Ageing
		FROM tbl_API_HUNT_Partner_Onboarding PO
		LEFT JOIN LATERAL (
			SELECT (CASE
					WHEN T.status='Request For Approval' OR T.status='Edited' THEN 'Created'
					WHEN T.status='Approved' THEN 'Approved'
					WHEN T.status='Draft' THEN 'Drafted'
					WHEN T.status='Reject' THEN 'Rejected'
					WHEN T.status='Feedback' THEN 'Awaiting For Reply'
					WHEN T.status='Feedback Reply' THEN 'Feedback Replyed'
					ELSE T.status END) AS STATUS
			FROM tbl_API_HUNT_Audit_log T
			WHERE T.CaseID=PO.PartnetOnboading_ID AND T.status IS NOT NULL
			ORDER BY T.createdDate DESC
			LIMIT 1
		) ST ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				COUNT(*) AS ApprovalCount,
				SUM(CASE WHEN TCOUNT.Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
				SUM(CASE WHEN TCOUNT.Status = 'Reject' THEN 1 ELSE 0 END) AS RejectCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_PO_ApprovalTrailTable TCOUNT
			WHERE TCOUNT.CaseId = PO.PartnetOnboading_ID
		) TCOUNT ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_Audit_log TCOUNT
			WHERE TCOUNT.CaseId = PO.PartnetOnboading_ID
		) FEEDCOUNT ON TRUE
		LEFT JOIN TBL_API_statusMaster SM ON SM.statusCode = IFNULL(PO.statusCode, '1')
		WHERE ST.STATUS IS NOT NULL
		ORDER BY/* StatusDescription,*/ PO.PartnetOnboading_ID DESC;
	
ELSEIF p_IdentFlag='GetPartnerList1' THEN

			SELECT (CASE WHEN CHAR_LENGTH(PO.PartnetOnboading_ID) <= 5 THEN CONCAT('APIGW0000000000', RIGHT(CONCAT('00000', CAST(PO.PartnetOnboading_ID AS CHAR)), 5))
                    ELSE CAST(PO.PartnetOnboading_ID AS CHAR) END) AS PartnetOnboading_ID,PO.Partner_Name,
			-- ST.STATUS,
			CASE 
				WHEN FEEDCOUNT.FeedbackCount > 0 AND FEEDCOUNT.FeedbackReplyCount > 0 AND FEEDCOUNT.FeedbackCount != FEEDCOUNT.FeedbackReplyCount THEN 'Awaiting For Reply'
				-- WHEN TCOUNT.ApprovalCount = 9 AND TCOUNT.ApprovedCount = 9 THEN 'In Progress'
				WHEN TCOUNT.ApprovalCount=TCOUNT.ApprovedCount THEN 'Approved'
				WHEN TCOUNT.RejectCount > 0 THEN 'Reject'
				WHEN TCOUNT.FeedbackCount > 0 AND TCOUNT.FeedbackReplyCount > 0 AND TCOUNT.FeedbackCount = TCOUNT.FeedbackReplyCount THEN 'Feedback'
				WHEN TCOUNT.FeedbackReplyCount < TCOUNT.FeedbackCount THEN 'Feedback Pending'
				WHEN ST.STATUS = 'Drafted' OR ST.STATUS = 'Awaiting For Reply' THEN ST.STATUS -- modify this line
				WHEN TCOUNT.ApprovalCount = 0 THEN 'Created'
				ELSE 'In Progress' 
			END AS StatusDescription,
			IFNULL(PO.created_By, '') AS createdBy,
			IFNULL(DATE_FORMAT(PO.created_date, '%d/%m/%Y'), '') AS createdDate,
			IFNULL(DATE_FORMAT(PO.Updated_date, '%d/%m/%Y'), '') AS UpdatedDate,
			IFNULL(DATEDIFF(PO.Updated_date, PO.created_date), 0) AS Ageing
		FROM tbl_API_HUNT_Partner_Onboarding PO
		LEFT JOIN LATERAL (
			SELECT (CASE
					WHEN T.status='Request For Approval' OR T.status='Edited' THEN 'Created'
					WHEN T.status='Approved' THEN 'Approved'
					WHEN T.status='Draft' THEN 'Drafted'
					WHEN T.status='Reject' THEN 'Rejected'
					WHEN T.status='Feedback' THEN 'Awaiting For Reply'
					WHEN T.status='Feedback Reply' THEN 'Feedback Replyed'
					ELSE T.status END) AS STATUS
			FROM tbl_API_HUNT_Audit_log T
			WHERE T.CaseID=PO.PartnetOnboading_ID AND T.status IS NOT NULL
			ORDER BY T.createdDate DESC
			LIMIT 1
		) ST ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				COUNT(*) AS ApprovalCount,
				SUM(CASE WHEN TCOUNT.Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
				SUM(CASE WHEN TCOUNT.Status = 'Reject' THEN 1 ELSE 0 END) AS RejectCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_PO_ApprovalTrailTable TCOUNT
			WHERE TCOUNT.CaseId = PO.PartnetOnboading_ID
		) TCOUNT ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_Audit_log TCOUNT
			WHERE TCOUNT.CaseId = PO.PartnetOnboading_ID
		) FEEDCOUNT ON TRUE
		LEFT JOIN TBL_API_statusMaster SM ON SM.statusCode = IFNULL(PO.statusCode, '1')
		WHERE ST.STATUS IS NOT NULL
		ORDER BY StatusDescription, PO.PartnetOnboading_ID DESC;
	
ELSEIF p_IdentFlag='GetApiDeatil' THEN

		-- SELECT ISNULL(ServiceID,'') AS APIId,ISNULL(ServiceName,'') AS APIName,ISNULL(Classification,'') AS APIRisk,ISNULL(RiskScore,'') AS APIRiskScore
		-- FROM TBL_API_HUNT_ServiceDetails
		-- WHERE ServiceName like '%'+@APIName+'%'

		SELECT IFNULL(EXTERNALSERVICE_ID,'') AS APIId,IFNULL(COD_SERVICE_ID,'') AS APIName,IFNULL(FILLER_02,'') AS APIRisk,IFNULL(FILLER_03,'') AS APIRiskScore
		FROM TBL_API_EXTERNALSERVICES
		WHERE COD_SERVICE_ID like CONCAT('%', p_APIName, '%')
	
ELSEIF p_IdentFlag='GetlstApiDeatil' THEN

		-- SELECT ISNULL(ServiceID,'') AS APIId,ISNULL(ServiceName,'') AS APIName,ISNULL(Classification,'') AS APIRisk,ISNULL(RiskScore,'') AS APIRiskScore
		-- FROM TBL_API_HUNT_ServiceDetails
		-- WHERE ServiceName like '%'+@APIName+'%'

		SELECT IFNULL(EXTERNALSERVICE_ID,'') AS APIId,IFNULL(COD_SERVICE_ID,'') AS APIName,IFNULL(FILLER_02,'') AS APIRisk,IFNULL(FILLER_03,'') AS APIRiskScore
		FROM TBL_API_EXTERNALSERVICES
	
ELSEIF p_IdentFlag='GetPartnerApprovalDeatil' THEN

		SELECT Partner_Name AS PartnerName,Project_Description AS projectDescription,TentativeGoLive_Date AS TentativeGoLiveDate,PartnerType AS PartnerType,
			   PartnerEntityType AS PartnerEntityType,PartnerTPRM_Application AS PartnerTPRMAssesmetApplicability,Partnerrisk_score AS PartnerRiskScore,
			   Partnerrisk AS PartnerRisk,created_By AS createdBy,API_risk AS APIRisk,AttachedJourneyDocuments AS AttachedJourneyDocuments,
			   APIRiskAssessment AS APIRiskAssessment,OtherDocument AS OtherDocument
		FROM tbl_API_HUNT_Partner_Onboarding WHERE PartnetOnboading_ID=p_PartnetOnboading_ID
	
ELSEIF p_IdentFlag='GetPartnerApprovalAPIDeatil' THEN

		SELECT Id AS APIId,APIName,APIRisk,APIRiskScore FROM tbl_API_HUNT_PO_ApiDeatil WHERE CaseId=p_PartnetOnboading_ID
	
ELSEIF p_IdentFlag='GetApprovalTrailDeatil' THEN

		SELECT DISTINCT T.ApproverUserID, T.Sequence, T.Department, T.ApproverLevel,T.Id AS ApprovalTrialID,CONCAT(T1.ApproverUserID, ' - ', T1.ApproverName) AS ApproverName
		FROM tbl_API_HUNT_PO_ApprovalTrailTable T
		INNER JOIN tbl_API_HUNT_MstPartnerCaseApprovalMetrix T1 ON T1.ApproverUserID=T.ApproverUserID AND T1.ApproverLevel=T.ApproverLevel 
																		 AND T1.ApproverType=(CASE WHEN T.Department='HOB' THEN 'Business'
																								   WHEN T.Department='HODB' THEN 'Digital Banking'
																								   WHEN T.Department='HOISG' THEN 'ISG'
																								   WHEN T.Department='HOITDRM' THEN 'ITDRM'
																								   WHEN T.Department='HOPP' THEN 'Product processor' END)
		INNER JOIN (
			SELECT Department, CaseId FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseId=p_PartnetOnboading_ID AND ApproverUserID = p_ApproverUserID -- 'M3330'
		) AS Subquery
		ON T.Department = Subquery.Department AND T.CaseId = Subquery.CaseId
		WHERE T.CaseId = p_PartnetOnboading_ID -- AND T.ApproverUserID = 'M3330'
	
ELSEIF p_IdentFlag='GetApprovalTrailList' THEN

		-- SELECT O.PartnetOnboading_ID,O.Partner_Name,O.Project_Description
		-- FROM tbl_API_HUNT_PO_ApprovalTrailTable T WITH(NOLOCK)
		-- INNER JOIN tbl_API_HUNT_Partner_Onboarding O WITH(NOLOCK) ON T.CaseId=O.PartnetOnboading_ID
		-- WHERE T.ApproverUserID=@ApproverUserID AND T.Status IS NULL

		/*
		SELECT O.PartnetOnboading_ID, O.Partner_Name, O.Project_Description,O.API_risk,O.Partnerrisk
		FROM tbl_API_HUNT_PO_ApprovalTrailTable T WITH(NOLOCK)
		INNER JOIN tbl_API_HUNT_Partner_Onboarding O WITH(NOLOCK) ON T.CaseId = O.PartnetOnboading_ID
		LEFT JOIN tbl_API_HUNT_Audit_log A WITH(NOLOCK) ON A.CaseID=T.CaseId AND A.ApprovalID=T.Id AND A.status='Feedback'
		LEFT JOIN tbl_API_HUNT_POFeedbackReply_history FH WITH(NOLOCK) ON FH.CaseID=T.CaseId AND FH.ApprovalId=T.Id AND FH.FeedbackID=A.id
		WHERE T.ApproverUserID = @ApproverUserID AND T.Status IS NULL AND T.ApproverLevel=@ApproverLevel 
		AND (
		(ApproverLevel = 'FH' AND (T.Status IS NULL OR T.Status='Feedback'))
		OR
		(ApproverLevel = 'VH' AND (ApproverLevel = 'FH' AND T.Status IS NOT NULL))
		OR
		(ApproverLevel = 'GH' AND (ApproverLevel = 'VH' AND T.Status IS NOT NULL))
		)
		AND (CASE WHEN A.id IS NOT NULL AND FH.FeedbackID IS NOT NULL THEN '1'
				  WHEN A.id IS NULL AND FH.FeedbackID IS NULL THEN '1'
				  ELSE '0' END)='1'
		*/;

		SELECT DISTINCT /*(CASE WHEN LEN(O.PartnetOnboading_ID) <= 5 THEN 'APIGW0000000000' + RIGHT('00000' + CAST(O.PartnetOnboading_ID AS VARCHAR), 5)
                    ELSE CAST(O.PartnetOnboading_ID AS VARCHAR) END) AS PartnetOnboading_ID,*/
					-- 'APIGW'+REPLACE(CONVERT(VARCHAR,O.created_date,3),'/','')+'00'+CAST(O.PartnetOnboading_ID AS VARCHAR) AS PartnetOnboading_ID,
					CONCAT('APIGW', REPLACE(DATE_FORMAT(O.created_date, '%d/%m/%y'),'/',''), (CASE WHEN CHAR_LENGTH(O.PartnetOnboading_ID)=1 THEN '0000' 
					WHEN CHAR_LENGTH(O.PartnetOnboading_ID)=2 THEN '000' WHEN CHAR_LENGTH(O.PartnetOnboading_ID)=3 THEN '00' WHEN CHAR_LENGTH(O.PartnetOnboading_ID)=4 THEN '0' END))+
					CAST(O.PartnetOnboading_ID AS CHAR) AS PartnetOnboading_ID,
					O.Partner_Name,O.Project_Description,O.API_risk,O.Partnerrisk,ApproverLevel,T.Status,
			CASE 
				WHEN FEEDCOUNT.FeedbackCount > 0 AND FEEDCOUNT.FeedbackReplyCount > 0 AND FEEDCOUNT.FeedbackCount != FEEDCOUNT.FeedbackReplyCount THEN 'Awaiting For Reply'
				WHEN TCOUNT.ApprovalCount = 9 AND TCOUNT.ApprovedCount = 9 THEN 'In Progress'
				WHEN TCOUNT.ApprovalCount=TCOUNT.ApprovedCount THEN 'Approved'
				WHEN TCOUNT.RejectCount > 0 THEN 'Reject'
				WHEN TCOUNT.FeedbackCount > 0 AND TCOUNT.FeedbackReplyCount > 0 AND TCOUNT.FeedbackCount = TCOUNT.FeedbackReplyCount THEN 'Feedback'
				WHEN TCOUNT.FeedbackReplyCount < TCOUNT.FeedbackCount THEN 'Feedback Pending'
				WHEN ST.STATUS = 'Drafted' OR ST.STATUS = 'Awaiting For Reply' THEN ST.STATUS -- modify this line
				WHEN TCOUNT.ApprovalCount = 0 THEN 'Created'
				ELSE 'In Progress' 
			END AS StatusDescription
		FROM 
			tbl_API_HUNT_PO_ApprovalTrailTable T
			INNER JOIN tbl_API_HUNT_Partner_Onboarding O ON T.CaseId = O.PartnetOnboading_ID
			LEFT JOIN tbl_API_HUNT_Audit_log A ON A.CaseID = T.CaseId AND A.ApprovalID = T.Id AND A.status = 'Feedback'
			LEFT JOIN tbl_API_HUNT_POFeedbackReply_history FH ON FH.CaseID = T.CaseId AND FH.ApprovalId = T.Id AND FH.FeedbackID = A.id
		LEFT JOIN LATERAL (
			SELECT (CASE
					WHEN T.status='Request For Approval' OR T.status='Edited' THEN 'Created'
					WHEN T.status='Approved' THEN 'Approved'
					WHEN T.status='Draft' THEN 'Drafted'
					WHEN T.status='Reject' THEN 'Rejected'
					WHEN T.status='Feedback' THEN 'Awaiting For Reply'
					WHEN T.status='Feedback Reply' THEN 'Feedback Replyed'
					ELSE T.status END) AS STATUS
			FROM tbl_API_HUNT_Audit_log T
			WHERE T.CaseID=O.PartnetOnboading_ID AND T.status IS NOT NULL
			ORDER BY T.createdDate DESC
			LIMIT 1
		) ST ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				COUNT(*) AS ApprovalCount,
				SUM(CASE WHEN TCOUNT.Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
				SUM(CASE WHEN TCOUNT.Status = 'Reject' THEN 1 ELSE 0 END) AS RejectCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_PO_ApprovalTrailTable TCOUNT
			WHERE TCOUNT.CaseId = O.PartnetOnboading_ID
		) TCOUNT ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_Audit_log TCOUNT
			WHERE TCOUNT.CaseId = O.PartnetOnboading_ID
		) FEEDCOUNT ON TRUE
		WHERE 
			T.ApproverUserID = p_ApproverUserID 
			AND T.Status IS NULL 
			AND T.ApproverLevel = p_ApproverLevel 
			AND (
				(ApproverLevel = 'FH' AND (T.Status IS NULL OR T.Status = 'Feedback'))
				OR
				( ApproverLevel = 'VH' AND 
					EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable T2 
										WHERE T2.CaseID = T.CaseID AND T2.Department = T.Department AND T2.ApproverLevel = 'FH' AND T2.Status IS NOT NULL )
				)
				OR
				(
					ApproverLevel = 'GH' AND 
					EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable T3 
										WHERE T3.CaseID = T.CaseID AND T3.Department = T.Department AND T3.ApproverLevel = 'VH' AND T3.Status IS NOT NULL )
				) )
			AND ( (CASE WHEN A.id IS NOT NULL AND FH.FeedbackID IS NOT NULL THEN '1'
					   WHEN A.id IS NULL AND FH.FeedbackID IS NULL THEN '1'
					   ELSE '0' END) = '1' );
	
ELSEIF p_IdentFlag='GetDepartmentWiseStatus' THEN

		WITH Departments AS (
			SELECT 'HOPP' AS Department UNION ALL
			SELECT 'HOB' UNION ALL
			SELECT 'HODB' UNION ALL
			SELECT 'HOISG' UNION ALL
			SELECT 'HOITDRM'
		);
		SELECT d.Department AS Department,COALESCE(s.OverallStatus, 'Pending') AS ApprovalStatus
		FROM Departments d
		LEFT JOIN (
			SELECT Department,
				CASE WHEN COUNT(CASE WHEN status IS NULL THEN 1 END) = COUNT(*) THEN 'Pending'
					 WHEN COUNT(CASE WHEN status = 'Approved' OR status = 'Rejected' THEN 1 END) = COUNT(*) THEN 'Approved'
					 ELSE 'Pending'
				END AS OverallStatus
			FROM tbl_API_HUNT_PO_ApprovalTrailTable
			WHERE CaseId = p_PartnetOnboading_ID
			GROUP BY Department
		) AS s
		ON d.Department = s.Department;
	
ELSEIF p_IdentFlag='SaveAddPartnerApproval' THEN

		

		SELECT p_PartnetOnboading_ID=TBL.COL.value('CaseID[1]','VARCHAR(100)'),
			   v_createdBy=TBL.COL.value('CurrentApproverUserID[1]','VARCHAR(100)')
		from p_PartnerXMl.nodes('/PartnerApprovalModel') AS TBL(COL);

		SELECT COUNT(*) INTO v_ApproverUserIDCNT FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseId=p_PartnetOnboading_ID AND ApproverUserID=v_createdBy;

		IF v_ApproverUserIDCNT>1 THEN

			INSERT INTO tbl_API_HUNT_Audit_log(CaseID,ApprovalID,status,createdBy,createdDate,Feedback);
			SELECT T.CaseId,T.Id AS ApprovalTrialID,
					TBL.COL.value('status[1]','VARCHAR(100)') AS status,
					v_createdBy,NOW(),
					TBL.COL.value('Feedback[1]','VARCHAR(250)') AS Feedback
					from p_PartnerXMl.nodes('/PartnerApprovalModel') AS TBL(COL)
					INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId=p_PartnetOnboading_ID AND T.ApproverUserID=v_createdBy;
			
			INSERT INTO tbl_API_HUNT_Approval_trace_trial(CaseID,ApprovalTrialID,Department,ApprovalLevel,ApprovalUserID,Sequence,Status,Feedback,AssignTo,AssignToLevel,AssignToDept,createdBy,createdDate,UpdatedBy,updatedDate);
			SELECT  T.CaseId,T.Id AS ApprovalTrialID,T.Department AS Department,
					TBL.COL.value('CurrentApproverLevel[1]','VARCHAR(50)') AS ApprovalLevel,
					TBL.COL.value('CurrentApproverUserID[1]','VARCHAR(100)') AS ApprovalUserID,
					TBL.COL.value('CurrentSequence[1]','VARCHAR(100)') AS Sequence,
					TBL.COL.value('status[1]','VARCHAR(100)') AS status,
					TBL.COL.value('Feedback[1]','VARCHAR(250)') AS Feedback,
					-- TBL.COL.value('AssignTo[1]','VARCHAR(100)') AS AssignTo,
					AST.ApproverUserID AS AssignTo,
					TBL.COL.value('AssignToLevel[1]','VARCHAR(100)') AS AssignToLevel,
					T.Department AS AssignToDept,
					TBL.COL.value('createdBy[1]','VARCHAR(100)') AS createdBy,
					NOW(),
					TBL.COL.value('createdBy[1]','VARCHAR(100)') AS createdBy,
					NOW()
					from p_PartnerXMl.nodes('/PartnerApprovalModel') AS TBL(COL)
					INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId=p_PartnetOnboading_ID AND T.ApproverUserID=v_createdBy
					-- TODO: OUTER APPLY converted - XML parsing needed in application layer
					-- OUTER APPLY(
					-- 	SELECT AST.ApproverUserID
					-- 	FROM tbl_API_HUNT_PO_ApprovalTrailTable AST
					-- 	WHERE AST.CaseId=p_PartnetOnboading_ID AND AST.Department=T.Department
					-- 	AND AST.ApproverLevel=(CASE WHEN TBL.COL.value('CurrentApproverLevel[1]','VARCHAR(50)')='FH' THEN 'VH'
					-- 								WHEN TBL.COL.value('CurrentApproverLevel[1]','VARCHAR(50)')='VH' THEN 'GH' END)
					-- )AST;
			
			-- SELECT A.Status,x.Status,A.Id,X.createdBy,(CASE WHEN x.Status='Approved' THEN x.Status WHEN x.Status='Reject' THEN x.Status ELSE NULL END)
			UPDATE A
			SET A.Status=(CASE WHEN x.Status='Approved' THEN x.Status WHEN x.Status='Reject' THEN x.Status ELSE NULL END)
			FROM tbl_API_HUNT_PO_ApprovalTrailTable A
			INNER JOIN (
				SELECT TBL.COL.value('CurrentApprovalTrialID[1]','VARCHAR(100)') AS ApprovalTrialID,
					   TBL.COL.value('status[1]','VARCHAR(100)') AS status,
						TBL.COL.value('CaseID[1]','VARCHAR(100)') AS CaseID,
						TBL.COL.value('createdBy[1]','VARCHAR(100)') AS createdBy
				FROM p_PartnerXMl.nodes('/PartnerApprovalModel') AS TBL(COL)
			) X
			ON A.CaseId=x.CaseID AND A.ApproverUserID=X.createdBy
		
ELSE

			INSERT INTO tbl_API_HUNT_Audit_log(CaseID,ApprovalID,status,createdBy,createdDate,Feedback);
			SELECT TBL.COL.value('CaseID[1]','VARCHAR(100)') AS CaseID,
					TBL.COL.value('CurrentApprovalTrialID[1]','VARCHAR(100)') AS ApprovalTrialID,
					TBL.COL.value('status[1]','VARCHAR(100)') AS status,
					TBL.COL.value('createdBy[1]','VARCHAR(100)') AS createdBy,
					NOW(),
					TBL.COL.value('Feedback[1]','VARCHAR(250)') AS Feedback
					from p_PartnerXMl.nodes('/PartnerApprovalModel') AS TBL(COL);

			INSERT INTO tbl_API_HUNT_Approval_trace_trial(CaseID,ApprovalTrialID,Department,ApprovalLevel,ApprovalUserID,Sequence,Status,Feedback,AssignTo,AssignToLevel,AssignToDept,createdBy,createdDate,UpdatedBy,updatedDate);
			SELECT TBL.COL.value('CaseID[1]','VARCHAR(100)') AS CaseID,
					TBL.COL.value('CurrentApprovalTrialID[1]','VARCHAR(100)') AS ApprovalTrialID,
					TBL.COL.value('CurrentDepartment[1]','VARCHAR(100)') AS Department,
					TBL.COL.value('CurrentApproverLevel[1]','VARCHAR(50)') AS ApprovalLevel,
					TBL.COL.value('CurrentApproverUserID[1]','VARCHAR(100)') AS ApprovalUserID,
					TBL.COL.value('CurrentSequence[1]','VARCHAR(100)') AS Sequence,
					TBL.COL.value('status[1]','VARCHAR(100)') AS status,
					TBL.COL.value('Feedback[1]','VARCHAR(250)') AS Feedback,
					TBL.COL.value('AssignTo[1]','VARCHAR(100)') AS AssignTo,
					TBL.COL.value('AssignToLevel[1]','VARCHAR(100)') AS AssignToLevel,
					TBL.COL.value('AssignToDept[1]','VARCHAR(100)') AS AssignToDept,
					TBL.COL.value('createdBy[1]','VARCHAR(100)') AS createdBy,
					NOW(),
					TBL.COL.value('createdBy[1]','VARCHAR(100)') AS createdBy,
					NOW()
					from p_PartnerXMl.nodes('/PartnerApprovalModel') AS TBL(COL);

			UPDATE A
			SET A.Status=(CASE WHEN x.Status='Approved' THEN x.Status WHEN x.Status='Reject' THEN x.Status ELSE NULL END)
			FROM tbl_API_HUNT_PO_ApprovalTrailTable A
			INNER JOIN (
				SELECT TBL.COL.value('CurrentApprovalTrialID[1]','VARCHAR(100)') AS ApprovalTrialID,
					   TBL.COL.value('status[1]','VARCHAR(100)') AS status,
						TBL.COL.value('CaseID[1]','VARCHAR(100)') AS CaseID
				FROM p_PartnerXMl.nodes('/PartnerApprovalModel') AS TBL(COL)
			) X
			ON A.Id = X.ApprovalTrialID AND A.CaseId=x.CaseID
		
END IF;
	
ELSEIF p_IdentFlag='FeedbackHistory' THEN

		SELECT DISTINCT U.EmpName AS EmpName,(CASE WHEN U1.Role='FH' THEN 'Functional Head'
										  WHEN U1.Role='VH' THEN 'Vertical Head'
										   WHEN U1.Role='GH' THEN 'Group Head'
										  ELSE U1.Role END) AS Role,
		IFNULL(T.Feedback,(CASE WHEN T.Feedback IS NULL AND T.Status='Approved' THEN CONCAT('Approved By ', U.EmpName)
								WHEN T.Feedback IS NULL AND T.Status='Reject' THEN CONCAT('Rejected By ', U.EmpName)
								ELSE T.Feedback END)) AS Feedback_Details,T.Status AS Status,CONCAT(DATE_FORMAT(T.createdDate, '%d/%m/%Y'), ' ', CAST(T.createdDate AS CHAR)) AS Created_Date,T.createdDate
		FROM tbl_API_HUNT_Audit_log T
		LEFT JOIN UserMaster U ON U.EmpCode=T.createdBy
		LEFT JOIN tbl_API_hunt_USER U1 ON U1.EmpCode=T.createdBy
		WHERE T.CaseID=p_PartnetOnboading_ID AND T.createdBy IS NOT NULL
		ORDER BY T.createdDate
	
ELSEIF p_IdentFlag='GetEditTrailDeatil' THEN

		SELECT DISTINCT T.ApproverUserID, T.Sequence, T.Department, T.ApproverLevel,T.Id AS ApprovalTrialID,CONCAT(T1.ApproverUserID, ' - ', T1.ApproverName) AS ApproverName,T.Status
		FROM tbl_API_HUNT_PO_ApprovalTrailTable T
		INNER JOIN tbl_API_HUNT_MstPartnerCaseApprovalMetrix T1 ON T1.ApproverUserID=T.ApproverUserID AND T1.ApproverLevel=T.ApproverLevel 
																		 AND T1.ApproverType=(CASE WHEN T.Department='HOB' THEN 'Business'
																								   WHEN T.Department='HODB' THEN 'Digital Banking'
																								   WHEN T.Department='HOISG' THEN 'ISG'
																								   WHEN T.Department='HOITDRM' THEN 'ITDRM'
																								   WHEN T.Department='HOPP' THEN 'Product processor' END)
		INNER JOIN (
			SELECT Department, CaseId FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseId=p_PartnetOnboading_ID -- AND ApproverUserID = @ApproverUserID -- 'M3330'
		) AS Subquery
		ON T.Department = Subquery.Department AND T.CaseId = Subquery.CaseId
		WHERE T.CaseId = p_PartnetOnboading_ID -- AND T.ApproverUserID = 'M3330'
	
ELSEIF p_IdentFlag='GetFeedbackDeatil' THEN

		SELECT DISTINCT A.ApprovalID,A.Feedback,T.Department,T.ApproverLevel,T.CaseId,A.createdDate,DATE_FORMAT(A.createdDate, '%d/%m/%Y') AS Created_Date,
						U.EmpName AS EmpName,(CASE WHEN U1.Role='FH' THEN 'Functional Head'
												   WHEN U1.Role='VH' THEN 'Vertical Head'
												   WHEN U1.Role='GH' THEN 'Group Head'
												   ELSE U1.Role END) AS Role,
						IFNULL(A.Feedback,(CASE WHEN A.Feedback IS NULL AND A.Status='Approved' THEN CONCAT('Approved By ', U.EmpName)
												WHEN A.Feedback IS NULL AND A.Status='Reject' THEN CONCAT('Rejected By ', U.EmpName)
												ELSE A.Feedback END)) AS Feedback_Details,A.status AS Status,A.createdBy AS FeedbackBy,A1.Feedback_Reply AS FeedbackReply,
												A.id AS FeedbackID
		FROM tbl_API_HUNT_PO_ApprovalTrailTable T
		INNER JOIN tbl_API_HUNT_Audit_log A ON A.CaseID=T.CaseId AND A.ApprovalID=T.Id AND A.Feedback IS NOT NULL
		LEFT JOIN tbl_API_HUNT_POFeedbackReply_history A1 ON A1.CaseID=T.CaseId AND A1.ApprovalID=T.Id AND A1.FeedbackID=A.id
		LEFT JOIN UserMaster U ON U.EmpCode=A.createdBy
		LEFT JOIN tbl_API_hunt_USER U1 ON U1.EmpCode=A.createdBy
		WHERE T.CaseId=p_PartnetOnboading_ID AND A.status='Feedback'
		ORDER BY A.createdDate
	
ELSEIF p_IdentFlag='EditPartner' THEN

		

		SELECT p_PartnetOnboading_ID=TBL.COL.value('CaseId[1]','varchar(100)'),
			   v_createdBy=TBL.COL.value('(../../createdBy)[1]', 'varchar(100)'),
			   v_ApprovalID=TBL.COL.value('ApprovalID[1]','varchar(100)'),
			   v_FeedbackID=TBL.COL.value('FeedbackID[1]','Varchar(250)')
		FROM p_PartnerXMl.nodes('/PartnerOnboarding/lstPOFeedbackHistory/POFeedbackHistory') AS TBL(COL);
		-- WHERE TBL.COL.value('FeedbackAction[1]', 'varchar(100)') = 'New'

		IF p_PartnetOnboading_ID IS NULL THEN

			SELECT p_PartnetOnboading_ID=TBL.COL.value('CaseID[1]','varchar(100)'),v_createdBy=TBL.COL.value('createdBy[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
		
END IF;

		SELECT (SELECT COUNT(*) AS TrailCount FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID) INTO v_TrailCount;
		-- SET @ActionTaken=(CASE WHEN @TrailCount>1 THEN 'Edited' ELSE 'Created' END )
		SET v_ActionTaken ='Created';

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_POFeedbackReply_history WHERE CaseID = p_PartnetOnboading_ID AND ApprovalID = v_ApprovalID AND FeedbackID = v_FeedbackID ) THEN

			INSERT INTO tbl_API_HUNT_POFeedbackReply_history(CaseID,ApprovalId,Department,approvalLevel,feedbackBy,Role,Status,CreatedBy,CreatedDate,Feedback_Reply,feedbackReplyBy,Feedback,FeedbackID);
			SELECT  TBL.COL.value('CaseId[1]','varchar(100)') AS CaseId,
					TBL.COL.value('ApprovalID[1]','varchar(100)') AS ApprovalID,
					TBL.COL.value('Department[1]','Varchar(100)') AS Department,
					TBL.COL.value('ApproverLevel[1]','Varchar(100)') AS ApproverLevel,
					TBL.COL.value('FeedbackBy[1]', 'varchar(100)') AS feedbackBy,
					TBL.COL.value('Role[1]', 'varchar(100)') AS Role,
					TBL.COL.value('Status[1]', 'varchar(100)') AS Status,
					TBL.COL.value('(../../createdBy)[1]', 'varchar(100)') AS createdBy,
					NOW(),
					TBL.COL.value('FeedbackReply[1]', 'varchar(100)') AS FeedbackReply,
					TBL.COL.value('(../../createdBy)[1]', 'varchar(100)') AS feedbackReplyBy,
					TBL.COL.value('Feedback[1]','Varchar(250)') AS Feedback,
					TBL.COL.value('FeedbackID[1]','Varchar(250)') AS FeedbackID
					FROM p_PartnerXMl.nodes('/PartnerOnboarding/lstPOFeedbackHistory/POFeedbackHistory') AS TBL(COL);
					-- WHERE TBL.COL.value('FeedbackAction[1]', 'varchar(100)') = 'New'

			INSERT INTO tbl_API_HUNT_Audit_log(CaseID,ApprovalID,status,createdBy,createdDate,Feedback);
			SELECT  TBL.COL.value('CaseId[1]','varchar(100)') AS CaseId,
					TBL.COL.value('ApprovalID[1]','varchar(100)') AS ApprovalID,
					'Feedback Reply',
					TBL.COL.value('(../../createdBy)[1]', 'varchar(100)') AS createdBy,
					NOW(),
					TBL.COL.value('FeedbackReply[1]', 'varchar(100)') AS Feedback
					FROM p_PartnerXMl.nodes('/PartnerOnboarding/lstPOFeedbackHistory/POFeedbackHistory') AS TBL(COL);
					-- WHERE TBL.COL.value('FeedbackAction[1]', 'varchar(100)') = 'New'

			UPDATE tbl_API_HUNT_Partner_Onboarding
			SET Updated_By=v_createdBy,
				Updated_date=NOW()
			WHERE PartnetOnboading_ID=p_PartnetOnboading_ID
		
END IF;

		IF v_createdBy IS NOT NULL AND IFNULL(v_FeedbackID,'')='' THEN

			INSERT INTO tbl_API_HUNT_Audit_log(CaseID,ApprovalID,status,createdBy,createdDate);
			SELECT p_PartnetOnboading_ID,NULL,v_ActionTaken,v_createdBy,NOW();

			UPDATE tbl_API_HUNT_Partner_Onboarding
			SET Updated_By=v_createdBy,
				Updated_date=NOW()
			WHERE PartnetOnboading_ID=p_PartnetOnboading_ID
		
END IF;

		/* START EDIT FOR APPROVAL TRAIL TABLE */
		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOPP' AND ApproverLevel='FH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOPP','FH',TBL.COL.value('HOPP_FH[1]','varchar(100)') AS HOPP_FH,'1'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOPP_FH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOPP_FH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOPP' AND T.ApproverLevel='FH' AND T.Status IS NULL AND TBL.COL.value('HOPP_FH[1]','varchar(100)') IS NOT NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOPP' AND ApproverLevel='VH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOPP','VH',TBL.COL.value('HOPP_VH[1]','varchar(100)') AS HOPP_VH,'2'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOPP_VH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOPP_VH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOPP' AND T.ApproverLevel='VH' AND T.Status IS NULL AND TBL.COL.value('HOPP_VH[1]','varchar(100)') IS NOT NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOPP' AND ApproverLevel='GH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOPP','GH',TBL.COL.value('HOPP_GH[1]','varchar(100)') AS HOPP_GH,'3'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOPP_GH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOPP_GH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOPP' AND T.ApproverLevel='GH' AND T.Status IS NULL AND TBL.COL.value('HOPP_GH[1]','varchar(100)') IS NOT NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOB' AND ApproverLevel='FH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOB','FH',TBL.COL.value('HOB_FH[1]','varchar(100)') AS HOB_FH,'1'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOB_FH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOB_FH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOB' AND T.ApproverLevel='FH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOB' AND ApproverLevel='VH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOB','VH',TBL.COL.value('HOB_VH[1]','varchar(100)') AS HOB_VH,'2'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOB_VH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOB_VH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOB' AND T.ApproverLevel='VH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOB' AND ApproverLevel='GH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOB','GH',TBL.COL.value('HOB_GH[1]','varchar(100)') AS HOB_GH,'3'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOB_GH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOB_GH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOB' AND T.ApproverLevel='GH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HODB' AND ApproverLevel='FH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HODB','FH',TBL.COL.value('HODB_FH[1]','varchar(100)') AS HODB_FH,'1'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HODB_FH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HODB_FH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HODB' AND T.ApproverLevel='FH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HODB' AND ApproverLevel='VH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HODB','VH',TBL.COL.value('HODB_VH[1]','varchar(100)') AS HODB_VH,'2'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HODB_VH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HODB_VH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HODB' AND T.ApproverLevel='VH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HODB' AND ApproverLevel='GH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HODB','GH',TBL.COL.value('HODB_GH[1]','varchar(100)') AS HODB_GH,'3'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HODB_GH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HODB_GH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HODB' AND T.ApproverLevel='GH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOISG' AND ApproverLevel='FH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOISG','FH',TBL.COL.value('HOISG_FH[1]','varchar(100)') AS HOISG_FH,'1'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOISG_FH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOISG_FH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOISG' AND T.ApproverLevel='FH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOISG' AND ApproverLevel='VH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOISG','VH',TBL.COL.value('HOISG_VH[1]','varchar(100)') AS HOISG_VH,'2'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOISG_VH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOISG_VH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOISG' AND T.ApproverLevel='VH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOISG' AND ApproverLevel='GH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOISG','GH',TBL.COL.value('HOISG_GH[1]','varchar(100)') AS HOISG_GH,'3'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOISG_GH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOISG_GH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOISG' AND T.ApproverLevel='GH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOITDRM' AND ApproverLevel='FH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOITDRM','FH',TBL.COL.value('HOITDRM_FH[1]','varchar(100)') AS HOITDRM_FH,'1'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOITDRM_FH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOITDRM_FH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOITDRM' AND T.ApproverLevel='FH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOITDRM' AND ApproverLevel='VH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOITDRM','VH',TBL.COL.value('HOITDRM_VH[1]','varchar(100)') AS HOITDRM_VH,'2'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOITDRM_VH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOITDRM_VH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOITDRM' AND T.ApproverLevel='VH' AND T.Status IS NULL
		
END IF;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOITDRM' AND ApproverLevel='GH') THEN

			INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
			SELECT p_PartnetOnboading_ID,'HOITDRM','GH',TBL.COL.value('HOITDRM_GH[1]','varchar(100)') AS HOITDRM_GH,'3'
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			WHERE TBL.COL.value('HOITDRM_GH[1]','varchar(100)') IS NOT NULL
		
ELSE

			UPDATE T
			SET ApproverUserID=TBL.COL.value('HOITDRM_GH[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
			WHERE T.Department ='HOITDRM' AND T.ApproverLevel='GH' AND T.Status IS NULL
		
END IF;
		/* END EDIT FOR APPROVAL TRAIL TABLE */
	
ELSEIF p_IdentFlag='GetFeedbackReply' THEN

		SELECT Feedback_Reply AS FeedbackReply FROM tbl_API_HUNT_POFeedbackReply_history WHERE CaseID=p_PartnetOnboading_ID AND ApprovalId=p_ApproverID
	
ELSEIF p_IdentFlag='GetMstPartnerType' THEN

		SELECT PartnerType,PartnerEntityType,TPRMAapplicable
		FROM TBL_API_HUNT_MstPartnerType
		WHERE PartnerType=p_PartnerType
	
	
ELSEIF p_IdentFlag='SaveFeedbackReply' THEN

		SELECT p_PartnetOnboading_ID=TBL.COL.value('CaseId[1]','varchar(100)'),
			   v_createdBy=TBL.COL.value('createdBy[1]', 'varchar(100)'),
			   v_ApprovalID=TBL.COL.value('ApprovalID[1]','varchar(100)'),
			   v_FeedbackID=TBL.COL.value('FeedbackID[1]','Varchar(250)')
		FROM p_PartnerXMl.nodes('/POFeedbackHistory') AS TBL(COL);

		SELECT (SELECT COUNT(*) AS TrailCount FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID) INTO v_TrailCount;

		IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_POFeedbackReply_history WHERE CaseID = p_PartnetOnboading_ID AND ApprovalID = v_ApprovalID AND FeedbackID = v_FeedbackID ) THEN

			INSERT INTO tbl_API_HUNT_POFeedbackReply_history(CaseID,ApprovalId,Department,approvalLevel,feedbackBy,Role,Status,CreatedBy,CreatedDate,Feedback_Reply,feedbackReplyBy,Feedback,FeedbackID);
			SELECT  TBL.COL.value('CaseId[1]','varchar(100)') AS CaseId,
					TBL.COL.value('ApprovalID[1]','varchar(100)') AS ApprovalID,
					TBL.COL.value('Department[1]','Varchar(100)') AS Department,
					TBL.COL.value('ApproverLevel[1]','Varchar(100)') AS ApproverLevel,
					TBL.COL.value('FeedbackBy[1]', 'varchar(100)') AS feedbackBy,
					TBL.COL.value('Role[1]', 'varchar(100)') AS Role,
					TBL.COL.value('Status[1]', 'varchar(100)') AS Status,
					TBL.COL.value('createdBy[1]', 'varchar(100)') AS createdBy,
					NOW(),
					TBL.COL.value('FeedbackReply[1]', 'varchar(100)') AS FeedbackReply,
					TBL.COL.value('createdBy[1]', 'varchar(100)') AS feedbackReplyBy,
					TBL.COL.value('Feedback[1]','Varchar(250)') AS Feedback,
					TBL.COL.value('FeedbackID[1]','Varchar(250)') AS FeedbackID
					FROM p_PartnerXMl.nodes('/POFeedbackHistory') AS TBL(COL);
			
			INSERT INTO tbl_API_HUNT_Audit_log(CaseID,ApprovalID,status,createdBy,createdDate,Feedback);
			SELECT  TBL.COL.value('CaseId[1]','varchar(100)') AS CaseId,
					TBL.COL.value('ApprovalID[1]','varchar(100)') AS ApprovalID,
					'Feedback Reply',
					TBL.COL.value('createdBy[1]', 'varchar(100)') AS createdBy,
					NOW(),
					TBL.COL.value('FeedbackReply[1]', 'varchar(100)') AS Feedback
					FROM p_PartnerXMl.nodes('/POFeedbackHistory') AS TBL(COL);

			UPDATE tbl_API_HUNT_Partner_Onboarding
			SET Updated_By=v_createdBy,
				Updated_date=NOW()
			WHERE PartnetOnboading_ID=p_PartnetOnboading_ID
		
END IF;
	
ELSEIF p_IdentFlag='PartnerOffboardingDetails' THEN

		SELECT IFNULL(PARTNER_NAME,'') AS PARTNER_NAME,IFNULL(PARTNETONBOADING_ID,'') AS PARTNETONBOADING_ID  FROM TBL_API_HUNT_PARTNER_ONBOARDING
	
ELSEIF p_IdentFlag='getPOFapiname' THEN

		SELECT IFNULL(APIName,'') AS API_NAME,IFNULL(Id,'') AS APIId,IFNULL(CaseId,'') AS PARTNETONBOADING_ID FROM tbl_API_HUNT_PO_ApiDeatil
	
ELSEIF p_IdentFlag='GetNewIdPOFB' THEN

		SELECT MAX(ID) AS CaseID FROM tbl_API_HUNT_PartnerOffboardning_getdetails 
	
ELSEIF p_IdentFlag='DeletePartner' THEN

		DELETE FROM tbl_API_HUNT_Partner_Onboarding WHERE PartnetOnboading_ID=p_PartnetOnboading_ID;
		DELETE FROM tbl_API_HUNT_PO_ApiDeatil WHERE CaseId=p_PartnetOnboading_ID;
		DELETE FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseId=p_PartnetOnboading_ID;
		DELETE FROM tbl_API_HUNT_Audit_log WHERE CaseId=p_PartnetOnboading_ID;
		DELETE FROM tbl_API_HUNT_Approval_trace_trial WHERE CaseId=p_PartnetOnboading_ID;
		DELETE FROM tbl_API_HUNT_POFeedbackReply_history WHERE CaseId=p_PartnetOnboading_ID 
	
ELSEIF p_IdentFlag='EditPartnerDraft' THEN

			SELECT p_PartnetOnboading_ID=TBL.COL.value('CaseID[1]','varchar(100)'),v_createdBy=TBL.COL.value('createdBy[1]','varchar(100)')
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL);

			SET v_ActionTaken ='Created';

			INSERT INTO tbl_API_HUNT_Audit_log(CaseID,ApprovalID,status,createdBy,createdDate);
			-- SELECT @PartnetOnboading_ID,NULL,@ActionTaken,@createdBy,GETDATE()
			SELECT p_PartnetOnboading_ID,NULL,
			TBL.COL.value('Action[1]','VARCHAR(100)') AS Action,v_createdBy,NOW()
			from p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL);

			UPDATE T
			SET T.Partner_Name=IFNULL(TBL.COL.value('PartnerName[1]','varchar(200)'),T.Partner_Name),
				T.Project_Description=IFNULL(TBL.COL.value('projectDescription[1]','varchar(200)'),T.Project_Description),
				T.TentativeGoLive_Date=(CASE WHEN TBL.COL.value('TentativeGoLiveDate[1]','VARCHAR(100)')='0001-01-01T00:00:00' THEN T.TentativeGoLive_Date 
										ELSE TBL.COL.value('TentativeGoLiveDate[1]','DATE') END),
				T.PartnerType=IFNULL(TBL.COL.value('PartnerType[1]','VARCHAR(100)'),T.PartnerType),
				T.PartnerEntityType=IFNULL(TBL.COL.value('PartnerEntityType[1]','VARCHAR(100)'),T.PartnerEntityType),
				T.PartnerTPRM_Application=IFNULL(TBL.COL.value('PartnerTPRMAssesmetApplicability[1]','VARCHAR(100)'),T.PartnerTPRM_Application),
				T.Partnerrisk_score=IFNULL(TBL.COL.value('PartnerRiskScore[1]','VARCHAR(100)'),T.Partnerrisk_score),
				T.Partnerrisk=IFNULL(TBL.COL.value('PartnerRisk[1]','VARCHAR(100)'),T.Partnerrisk),
				T.Updated_By=v_createdBy,
				T.Updated_date=NOW(),
				T.API_risk=IFNULL(TBL.COL.value('APIRisk[1]','VARCHAR(100)'),T.API_risk),
				T.AttachedJourneyDocuments=IFNULL(TBL.COL.value('AttachedJourneyDocuments[1]','VARCHAR(150)'),T.AttachedJourneyDocuments),
				T.APIRiskAssessment=IFNULL(TBL.COL.value('APIRiskAssessmentSheet[1]','VARCHAR(150)'),T.APIRiskAssessment),
				T.OtherDocument=IFNULL(TBL.COL.value('OtherDocument[1]','VARCHAR(150)'),T.OtherDocument)
			FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
			INNER JOIN tbl_API_HUNT_Partner_Onboarding T ON T.PartnetOnboading_ID=p_PartnetOnboading_ID
			WHERE T.PartnetOnboading_ID=p_PartnetOnboading_ID;

			/* START EDIT FOR APPROVAL TRAIL TABLE */
			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOPP' AND ApproverLevel='FH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOPP','FH',TBL.COL.value('HOPP_FH[1]','varchar(100)') AS HOPP_FH,'1'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOPP_FH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOPP_FH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOPP' AND T.ApproverLevel='FH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOPP_FH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOPP' AND ApproverLevel='VH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOPP','VH',TBL.COL.value('HOPP_VH[1]','varchar(100)') AS HOPP_VH,'2'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOPP_VH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOPP_VH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOPP' AND T.ApproverLevel='VH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOPP_VH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOPP' AND ApproverLevel='GH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOPP','GH',TBL.COL.value('HOPP_GH[1]','varchar(100)') AS HOPP_GH,'3'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOPP_GH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOPP_GH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOPP' AND T.ApproverLevel='GH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOPP_GH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOB' AND ApproverLevel='FH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOB','FH',TBL.COL.value('HOB_FH[1]','varchar(100)') AS HOB_FH,'1'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOB_FH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOB_FH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOB' AND T.ApproverLevel='FH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOB_FH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOB' AND ApproverLevel='VH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOB','VH',TBL.COL.value('HOB_VH[1]','varchar(100)') AS HOB_VH,'2'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOB_VH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOB_VH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOB' AND T.ApproverLevel='VH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOB_VH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOB' AND ApproverLevel='GH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOB','GH',TBL.COL.value('HOB_GH[1]','varchar(100)') AS HOB_GH,'3'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOB_GH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOB_GH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOB' AND T.ApproverLevel='GH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOB_GH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HODB' AND ApproverLevel='FH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HODB','FH',TBL.COL.value('HODB_FH[1]','varchar(100)') AS HODB_FH,'1'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HODB_FH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HODB_FH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HODB' AND T.ApproverLevel='FH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HODB_FH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HODB' AND ApproverLevel='VH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HODB','VH',TBL.COL.value('HODB_VH[1]','varchar(100)') AS HODB_VH,'2'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HODB_VH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HODB_VH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HODB' AND T.ApproverLevel='VH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HODB_VH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HODB' AND ApproverLevel='GH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HODB','GH',TBL.COL.value('HODB_GH[1]','varchar(100)') AS HODB_GH,'3'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HODB_GH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HODB_GH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HODB' AND T.ApproverLevel='GH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HODB_GH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOISG' AND ApproverLevel='FH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOISG','FH',TBL.COL.value('HOISG_FH[1]','varchar(100)') AS HOISG_FH,'1'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOISG_FH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOISG_FH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOISG' AND T.ApproverLevel='FH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOISG_FH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOISG' AND ApproverLevel='VH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOISG','VH',TBL.COL.value('HOISG_VH[1]','varchar(100)') AS HOISG_VH,'2'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOISG_VH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOISG_VH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOISG' AND T.ApproverLevel='VH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOISG_VH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOISG' AND ApproverLevel='GH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOISG','GH',TBL.COL.value('HOISG_GH[1]','varchar(100)') AS HOISG_GH,'3'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOISG_GH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOISG_GH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOISG' AND T.ApproverLevel='GH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOISG_GH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOITDRM' AND ApproverLevel='FH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOITDRM','FH',TBL.COL.value('HOITDRM_FH[1]','varchar(100)') AS HOITDRM_FH,'1'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOITDRM_FH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOITDRM_FH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOITDRM' AND T.ApproverLevel='FH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOITDRM_FH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOITDRM' AND ApproverLevel='VH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOITDRM','VH',TBL.COL.value('HOITDRM_VH[1]','varchar(100)') AS HOITDRM_VH,'2'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOITDRM_VH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOITDRM_VH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOITDRM' AND T.ApproverLevel='VH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOITDRM_VH[1]','varchar(100)'),'')<>''
			
END IF;

			IF NOT EXISTS ( SELECT 1 FROM tbl_API_HUNT_PO_ApprovalTrailTable WHERE CaseID = p_PartnetOnboading_ID AND Department ='HOITDRM' AND ApproverLevel='GH') THEN

				INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable(CaseId,Department,ApproverLevel,ApproverUserID,Sequence);
				SELECT p_PartnetOnboading_ID,'HOITDRM','GH',TBL.COL.value('HOITDRM_GH[1]','varchar(100)') AS HOITDRM_GH,'3'
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				WHERE IFNULL(TBL.COL.value('HOITDRM_GH[1]','varchar(100)'),'')<>''
			
ELSE

				UPDATE T
				SET ApproverUserID=TBL.COL.value('HOITDRM_GH[1]','varchar(100)')
				FROM p_PartnerXMl.nodes('/PartnerOnboarding') AS TBL(COL)
				INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable T ON T.CaseId = p_PartnetOnboading_ID
				WHERE T.Department ='HOITDRM' AND T.ApproverLevel='GH' AND T.Status IS NULL AND IFNULL(TBL.COL.value('HOITDRM_GH[1]','varchar(100)'),'')<>''
			
END IF;
			/* END EDIT FOR APPROVAL TRAIL TABLE */

			DELETE FROM tbl_API_HUNT_PO_ApiDeatil WHERE CaseId=p_PartnetOnboading_ID;

			INSERT INTO tbl_API_HUNT_PO_ApiDeatil(CaseId,APIName,APIRisk,APIRiskScore);
			SELECT p_PartnetOnboading_ID,TBL.COL.value('APIName[1]','varchar(100)') AS APIName,
			TBL.COL.value('APIRisk[1]','Varchar(100)') AS APIRisk ,
			TBL.COL.value('APIRiskScore[1]','Varchar(100)') AS APIRiskScore
			from p_PartnerXMl.nodes('/PartnerOnboarding/lstApiDeatil/ApiDeatil') AS TBL(COL)
	
ELSEIF p_IdentFlag='GetPartnerSendMailDeatil' THEN

		IF IFNULL(p_PartnetOnboading_ID,'')='' THEN

			SELECT (SELECT MAX(PartnetOnboading_ID) AS PartnetOnboading_ID FROM tbl_API_HUNT_Partner_Onboarding) INTO v_PartnetOnboading_ID
		
END IF;
		SELECT DISTINCT CONCAT('APIGW', REPLACE(DATE_FORMAT(PO.created_date, '%d/%m/%y'),'/',''), (CASE WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=1 THEN '0000'
					WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=2 THEN '000' WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=3 THEN '00' WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=4 THEN '0' END),
					CAST(PO.PartnetOnboading_ID AS CHAR)) AS PartnetOnboading_ID,PO.PartnetOnboading_ID AS CaseID,
					PO.Partner_Name,
			-- ST.STATUS,
			CASE 
				WHEN FEEDCOUNT.FeedbackCount > 0 AND FEEDCOUNT.FeedbackReplyCount > 0 AND FEEDCOUNT.FeedbackCount != FEEDCOUNT.FeedbackReplyCount THEN 'Awaiting For Reply'
				-- WHEN TCOUNT.ApprovalCount = 9 AND TCOUNT.ApprovedCount = 9 THEN 'In Progress'
				WHEN TCOUNT.ApprovalCount=TCOUNT.ApprovedCount THEN 'Approved'
				WHEN TCOUNT.RejectCount > 0 THEN 'Reject'
				WHEN TCOUNT.FeedbackCount > 0 AND TCOUNT.FeedbackReplyCount > 0 AND TCOUNT.FeedbackCount = TCOUNT.FeedbackReplyCount THEN 'Feedback'
				WHEN TCOUNT.FeedbackReplyCount < TCOUNT.FeedbackCount THEN 'Feedback Pending'
				WHEN ST.STATUS = 'Drafted' OR ST.STATUS = 'Awaiting For Reply' THEN ST.STATUS -- modify this line
				WHEN TCOUNT.ApprovalCount = 0 THEN 'Created'
				ELSE 'In Progress' 
			END AS StatusDescription,
			IFNULL(PO.created_By, '') AS createdBy,AU.EmailId,
			CONCAT(IFNULL(DATE_FORMAT(PO.created_date, '%d/%m/%Y'), ''), ' ', IFNULL(CAST(PO.created_date AS CHAR), '')) AS createdDate,TT.ApproverUserID,U.EmpName
		FROM tbl_API_HUNT_Partner_Onboarding PO
		INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable TT ON TT.CaseId=PO.PartnetOnboading_ID AND TT.Status IS NULL
		INNER JOIN tbl_API_hunt_USER AU ON AU.EmpCode=TT.ApproverUserID
		INNER JOIN UserMaster U ON U.EmpCode=TT.ApproverUserID
		LEFT JOIN LATERAL (
			SELECT (CASE
					WHEN T.status='Request For Approval' OR T.status='Edited' THEN 'Created'
					WHEN T.status='Approved' THEN 'Approved'
					WHEN T.status='Draft' THEN 'Drafted'
					WHEN T.status='Reject' THEN 'Rejected'
					WHEN T.status='Feedback' THEN 'Awaiting For Reply'
					WHEN T.status='Feedback Reply' THEN 'Feedback Replyed'
					ELSE T.status END) AS STATUS
			FROM tbl_API_HUNT_Audit_log T
			WHERE T.CaseID=PO.PartnetOnboading_ID AND T.status IS NOT NULL
			ORDER BY T.createdDate DESC
			LIMIT 1
		) ST ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				COUNT(*) AS ApprovalCount,
				SUM(CASE WHEN TCOUNT.Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
				SUM(CASE WHEN TCOUNT.Status = 'Reject' THEN 1 ELSE 0 END) AS RejectCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_PO_ApprovalTrailTable TCOUNT
			WHERE TCOUNT.CaseId = PO.PartnetOnboading_ID
		) TCOUNT ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_Audit_log TCOUNT
			WHERE TCOUNT.CaseId = PO.PartnetOnboading_ID
		) FEEDCOUNT ON TRUE
		LEFT JOIN TBL_API_statusMaster SM ON SM.statusCode = IFNULL(PO.statusCode, '1')
		WHERE ST.STATUS IS NOT NULL AND PO.PartnetOnboading_ID=p_PartnetOnboading_ID
		LIMIT 0;

ELSEIF p_IdentFlag='GetPartnerSendMailDeatilForScheduler' THEN

		SELECT DISTINCT /*DATEDIFF(HOUR,PO.created_date,NOW()),*/CONCAT('APIGW', REPLACE(DATE_FORMAT(PO.created_date, '%d/%m/%y'),'/',''), (CASE WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=1 THEN '0000'
					WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=2 THEN '000' WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=3 THEN '00' WHEN CHAR_LENGTH(PO.PartnetOnboading_ID)=4 THEN '0' END),
					CAST(PO.PartnetOnboading_ID AS CHAR)) AS PartnetOnboading_ID,PO.PartnetOnboading_ID AS CaseID,
					PO.Partner_Name,
			-- ST.STATUS,
			CASE 
				WHEN FEEDCOUNT.FeedbackCount > 0 AND FEEDCOUNT.FeedbackReplyCount > 0 AND FEEDCOUNT.FeedbackCount != FEEDCOUNT.FeedbackReplyCount THEN 'Awaiting For Reply'
				-- WHEN TCOUNT.ApprovalCount = 9 AND TCOUNT.ApprovedCount = 9 THEN 'In Progress'
				WHEN TCOUNT.ApprovalCount=TCOUNT.ApprovedCount THEN 'Approved'
				WHEN TCOUNT.RejectCount > 0 THEN 'Reject'
				WHEN TCOUNT.FeedbackCount > 0 AND TCOUNT.FeedbackReplyCount > 0 AND TCOUNT.FeedbackCount = TCOUNT.FeedbackReplyCount THEN 'Feedback'
				WHEN TCOUNT.FeedbackReplyCount < TCOUNT.FeedbackCount THEN 'Feedback Pending'
				WHEN ST.STATUS = 'Drafted' OR ST.STATUS = 'Awaiting For Reply' THEN ST.STATUS -- modify this line
				WHEN TCOUNT.ApprovalCount = 0 THEN 'Created'
				ELSE 'In Progress' 
			END AS StatusDescription,
			IFNULL(PO.created_By, '') AS createdBy,AU.EmailId,
			CONCAT(IFNULL(DATE_FORMAT(PO.created_date, '%d/%m/%Y'), ''), ' ', IFNULL(CAST(PO.created_date AS CHAR), '')) AS createdDate,TT.ApproverUserID,U.EmpName
		FROM tbl_API_HUNT_Partner_Onboarding PO
		INNER JOIN tbl_API_HUNT_PO_ApprovalTrailTable TT ON TT.CaseId=PO.PartnetOnboading_ID AND TT.Status IS NULL
		INNER JOIN tbl_API_hunt_USER AU ON AU.EmpCode=TT.ApproverUserID
		INNER JOIN UserMaster U ON U.EmpCode=TT.ApproverUserID
		LEFT JOIN LATERAL (
			SELECT (CASE
					WHEN T.status='Request For Approval' OR T.status='Edited' THEN 'Created'
					WHEN T.status='Approved' THEN 'Approved'
					WHEN T.status='Draft' THEN 'Drafted'
					WHEN T.status='Reject' THEN 'Rejected'
					WHEN T.status='Feedback' THEN 'Awaiting For Reply'
					WHEN T.status='Feedback Reply' THEN 'Feedback Replyed'
					ELSE T.status END) AS STATUS
			FROM tbl_API_HUNT_Audit_log T
			WHERE T.CaseID=PO.PartnetOnboading_ID AND T.status IS NOT NULL
			ORDER BY T.createdDate DESC
			LIMIT 1
		) ST ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				COUNT(*) AS ApprovalCount,
				SUM(CASE WHEN TCOUNT.Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
				SUM(CASE WHEN TCOUNT.Status = 'Reject' THEN 1 ELSE 0 END) AS RejectCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_PO_ApprovalTrailTable TCOUNT
			WHERE TCOUNT.CaseId = PO.PartnetOnboading_ID
		) TCOUNT ON TRUE
		LEFT JOIN LATERAL (
			SELECT
				SUM(CASE WHEN TCOUNT.Status = 'Feedback' THEN 1 ELSE 0 END) AS FeedbackCount,
				SUM(CASE WHEN TCOUNT.Status = 'Feedback Reply' THEN 1 ELSE 0 END) AS FeedbackReplyCount
			FROM tbl_API_HUNT_Audit_log TCOUNT
			WHERE TCOUNT.CaseId = PO.PartnetOnboading_ID
		) FEEDCOUNT ON TRUE
		LEFT JOIN TBL_API_statusMaster SM ON SM.statusCode = IFNULL(PO.statusCode, '1')
		WHERE PO.created_date>'2023-11-08 00:00:00.000' AND
		TIMESTAMPDIFF(HOUR,PO.created_date,NOW())>24 AND ST.STATUS IS NOT NULL -- AND PO.PartnetOnboading_ID=101
		AND (CASE WHEN TCOUNT.RejectCount > 0 THEN 'N' 
				  WHEN TCOUNT.ApprovalCount=TCOUNT.ApprovedCount THEN 'N'
				  ELSE 'Y' END)='Y'
		ORDER BY/* StatusDescription,*/ PO.PartnetOnboading_ID DESC
		LIMIT 0;

END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS USP_Insert_Data_In_Activity_Log_Tracker_API_Hunt;
DELIMITER //
CREATE PROCEDURE USP_Insert_Data_In_Activity_Log_Tracker_API_Hunt(IN p_Emp_Code Varchar(50), IN p_Form_Name Varchar(50), IN p_Module_Name Varchar(50), IN p_Total_Count INT, IN p_Activity varchar(50), IN p_Activity_Details varchar(100))
BEGIN
INSERT INTO tbl_API_hunt_Activity_Log_Tracker
           (Emp_Code
           ,Form_Name
           ,Module_Name
           ,Total_Count
           ,Activity
           ,Activity_Details
		   ,Activity_Date
           )
     VALUES
	       (p_Emp_Code
		   ,p_Form_Name
		   ,p_Module_Name
		   ,p_Total_Count
		   ,p_Activity
		   ,p_Activity_Details
		   ,NOW()
		   );
END //
DELIMITER ;

DELIMITER ;