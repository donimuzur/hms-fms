use [FMS];

Create procedure dbo.GetEpafCTF
AS BEGIN

	MERGE MST_EPAF AS Target
	USING (select ACTION_TYPE, EMPLID, NAME_FORMAL, CC, EFFDT, GROUP_LEVEL
	FROM View_EPAF_CTF) AS Source
	ON (Source.ACTION_TYPE = Target.EPAF_ACTION OR (Source.ACTION_TYPE IS NULL AND Target.EPAF_ACTION IS NULL))
	AND (Source.CC = Target.COST_CENTER OR (Source.CC IS NULL AND Target.COST_CENTER IS NULL))
	AND (Source.EFFDT = Target.EFFECTIVE_DATE OR (Source.EFFDT IS NULL AND Target.EFFECTIVE_DATE IS NULL))
	AND (Source.GROUP_LEVEL = Target.GROUP_LEVEL OR (Source.GROUP_LEVEL IS NULL AND Target.GROUP_LEVEL IS NULL))
	AND (Target.DOCUMENT_TYPE = 6)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (DOCUMENT_TYPE, EPAF_ACTION, EMPLOYEE_ID, EMPLOYEE_NAME, COST_CENTER,
		EFFECTIVE_DATE, GROUP_LEVEL, CREATED_BY, CREATED_DATE, IS_ACTIVE)
		VALUES( 6, Source.ACTION_TYPE, Source.EMPLID, Source.NAME_FORMAL, Source.CC, 
		Source.EFFDT, Source.GROUP_LEVEL, 'SYSTEM', GETDATE(), 1);

END