IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'GetPOData')
BEGIN
    DROP PROCEDURE [dbo].[GetPOData]
END
GO
CREATE procedure [dbo].[GetPOData]
AS BEGIN

	MERGE PO_REPORT_DATA AS Target
	USING (
		select a.POLICE_NUMBER, a.SUPPLY_METHOD, a.EMPLOYEE_NAME, a.COST_CENTER, a.MANUFACTURER, a.MODEL, a.SERIES, 
		a.BODY_TYPE, a.COLOR, a.CHASIS_NUMBER, a.ENGINE_NUMBER, a.VEHICLE_TYPE, a.VEHICLE_USAGE, a.START_CONTRACT, 
		a.END_CONTRACT, a.VENDOR_NAME, a.MONTHLY_HMS_INSTALLMENT, 
		ISNULL(IIF (((a.VAT_DECIMAL is null)OR(a.VAT_DECIMAL <> 0)), MONTHLY_HMS_INSTALLMENT * 0.1, 0), 0 )  AS GST, 
		ISNULL((IIF (((a.VAT_DECIMAL is null)OR(a.VAT_DECIMAL <> 0)), MONTHLY_HMS_INSTALLMENT * 0.1, 0) + a.MONTHLY_HMS_INSTALLMENT), 0 )  AS TOTAL_MONTHLY_INSTALLMENT, 
		a.PO_NUMBER, a.PO_LINE, a.CREATED_DATE, a.MST_FLEET_ID,  ISNULL(a.EMPLOYEE_ID, '-' )as EMPLOYEE_ID, a.VEHICLE_FUNCTION
		FROM MST_FLEET a, MST_EMPLOYEE b where b.EMPLOYEE_ID = a.EMPLOYEE_ID
	) AS Source
	On (Source.PO_NUMBER = Target.PO_NUMBER)
	AND (Source.PO_LINE = Target.PO_LINE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (POLICE_NUMBER, SUPPLY_METHOD, EMPLOYEE_NAME, COST_CENTER, MANUFACTURER, MODEL, SERIES, BODY_TYPE, 
		COLOR, CHASIS_NUMBER, ENGINE_NUMBER, VEHICLE_TYPE, VEHICLE_USAGE, START_CONTRACT, END_CONTRACT, VENDOR, 
		MONTHLY_INSTALLMENT, GST, TOTAL_MONTHLY_INSTALLMENT, PO_NUMBER, PO_LINE, REPORT_MONTH, REPORT_YEAR, CREATED_DATE, EMPLOYEE_ID, MST_FlEET_ID, VEHICLE_FUNCTION)
		VALUES( Source.POLICE_NUMBER, Source.SUPPLY_METHOD, Source.EMPLOYEE_NAME, Source.COST_CENTER, Source.MANUFACTURER, 
		Source.MODEL, Source.SERIES, Source.BODY_TYPE, Source.COLOR, Source.CHASIS_NUMBER, Source.ENGINE_NUMBER,
		Source.VEHICLE_TYPE, Source.VEHICLE_USAGE, Source.START_CONTRACT, Source.END_CONTRACT, Source.VENDOR_NAME, Source.MONTHLY_HMS_INSTALLMENT, 
		Source.GST, Source.TOTAL_MONTHLY_INSTALLMENT, Source.PO_NUMBER, Source.PO_LINE, MONTH(Source.CREATED_DATE), YEAR(Source.CREATED_DATE), Source.CREATED_DATE, Source.EMPLOYEE_ID, Source.MST_FlEET_ID, Source.VEHICLE_FUNCTION);

END