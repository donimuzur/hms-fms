CREATE procedure dbo.GetCC
AS BEGIN

	MERGE MST_FUNCTION_GROUP AS Target
	USING (SELECT DISTINCT txt_acc_group, txt_cc
	FROM [HMSSQLFWOPRD.ID.PMI\PRD03].[db_Intranet_Acc_Code_P1].[dbo].[tbl_CC]) AS Source
	ON (Source.txt_acc_group = Target.FUNCTION_NAME OR (Source.txt_acc_group IS NULL AND Target.FUNCTION_NAME IS NULL))
	AND (Source.txt_cc = Target.COST_CENTER OR (Source.txt_cc IS NULL AND Target.COST_CENTER IS NULL))
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (FUNCTION_NAME, COST_CENTER, CREATED_BY, CREATED_DATE, IS_ACTIVE)
		VALUES( Source.txt_acc_group, Source.txt_cc, 'SYSTEM', GETDATE(), 1);

END