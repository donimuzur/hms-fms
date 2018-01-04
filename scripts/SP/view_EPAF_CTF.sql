

ALTER VIEW [dbo].[View_EPAF_CTF]
AS
select 
'Promotion' as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
'SYSTEM' as LASTUPDOPRID

from [PSFT_FMS]..SYSADM.PS_HMS_JOB_FLEET_VW 
where ACTION in ('PRO') and 
EMPLID  in (
select employee_id from mst_employee)
UNION
select 
(case ACTION_REASON  
WHEN 'RES' THEN 'Resignation'
ELSE 'Terminate by Company' END) as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
'SYSTEM' as LASTUPDOPRID

from [PSFT_FMS]..SYSADM.PS_HMS_JOB_FLEET_VW 
where ACTION in ('TER') and 
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


