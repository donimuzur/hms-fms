USE [FMS]
GO

/****** Object:  View [dbo].[View_EPAF_CTF]    Script Date: 11/30/2017 3:44:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[View_EPAF_CTF]
AS
select 
(case ACTION  
WHEN 'TER' THEN 'Terminate by Company'
ELSE 'Promotion' END) as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
'SYSTEM' as LASTUPDOPRID

from [PSFT_FMS]..SYSADM.PS_HMS_JOB_FLEET_VW 
where ACTION in ('TER','PRO') and 
EMPLID  in (
select employee_id from mst_employee)
UNION
select (case ACTION_REASON  
WHEN 'STA' THEN 'STA Out'
ELSE 'IA Out' END) as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,

'SYSTEM' as LASTUPDOPRID
from [PSFT_FMS]..SYSADM.PS_HMS_JOB_FLEET_VW 
where ACTION = 'TAS' and ACTION_REASON in ('STA','IA') and 
EMPLID  in (
select employee_id from mst_employee);




GO


