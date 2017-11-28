USE [FMS]
GO

/****** Object:  View [PMI\ywicakso].[View_EPAF_CSF]    Script Date: 11/24/2017 2:08:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW dbo.[View_EPAF_CSF]
AS
SELECT case ACTION 
WHEN 'HIR' THEN 'New Hire' 
ELSE 'Rehire' END as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
NULL as EXPAT,
NULL as LETTER_SEND,
CITY,
LOCATION_DESC as BASETOWN,
'SYSTEM' as LASTUPDOPRID
 FROM [PSFT_GEN]..SYSADM.PS_HMS_JOB_FLEET_VW
where ACTION in ('HIR','REH')

UNION

SELECT 'Promotion' as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
NULL as EXPAT,
NULL as LETTER_SEND,
CITY,
LOCATION_DESC as BASETOWN,
'SYSTEM' as LASTUPDOPRID
 FROM [PSFT_GEN]..SYSADM.PS_HMS_JOB_FLEET_VW
where ACTION in ('PRO')

UNION
SELECT ACTION_REASON as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
NULL as EXPAT,
NULL as LETTER_SEND,
CITY,
LOCATION_DESC as BASETOWN,
'SYSTEM' as LASTUPDOPRID
 FROM [PSFT_GEN]..SYSADM.PS_HMS_JOB_FLEET_VW
where ACTION in ('TAS') and ACTION_REASON in ('STA','IA');

GO


