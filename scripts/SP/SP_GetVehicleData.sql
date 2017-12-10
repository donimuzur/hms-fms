use [FMS];
Create procedure [dbo].[GetVehicleData]
AS BEGIN

	MERGE VEHICLE_REPORT_DATA AS Target
	USING (
		select MST_FLEET_ID, POLICE_NUMBER, MANUFACTURER, MODEL, SERIES, BODY_TYPE, EMPLOYEE_ID, EMPLOYEE_NAME, 
		VEHICLE_TYPE, COST_CENTER, START_RENT, END_RENT, SUPPLY_METHOD, VENDOR, VEHICLE_FUNCTION, REGIONAL, CITY, 
		TRANSMISSION, FUEL_TYPE, BRANDING, COLOR, AIRBAG, CHASIS_NUMBER, ENGINE_NUMBER, VEHICLE_STATUS, 
		ASSET_NUMBER, END_DATE, RESTITUTION, MONTHLY_HMS_INSTALLMENT, VAT, TOTAL_MONTHLY_CHARGE, PO_NUMBER, 
		PO_LINE, CREATED_DATE FROM MST_FLEET
	) AS Source
	On (Source.MST_FLEET_ID = Target.MST_FLEET_ID)
	AND (Source.CREATED_DATE = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ( MST_FLEET_ID, POLICE_NUMBER, MANUFACTURER, MODEL, SERIES, BODY_TYPE, EMPLOYEE_ID, EMPLOYEE_NAME, 
		VEHICLE_TYPE, COST_CENTER, START_RENT, END_RENT, SUPPLY_METHOD, VENDOR, [FUNCTION], REGIONAL, CITY, 
		TRANSMISSION, FUEL_TYPE, BRANDING, COLOR, AIRBAG, CHASIS_NUMBER, ENGINE_NUMBER, VEHICLE_STATUS, 
		ASSET_NUMBER, TERMINATION_DATE, RESTITUTION, MONTHLY_INSTALLMENT, VAT, TOTAL_MONTHLY_CHARGE, PO_NUMBER, 
		PO_LINE, REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
		VALUES( Source.MST_FLEET_ID, Source.POLICE_NUMBER, Source.MANUFACTURER, Source.MODEL, Source.SERIES
		, Source.BODY_TYPE, Source.EMPLOYEE_ID, Source.EMPLOYEE_NAME, Source.VEHICLE_TYPE, Source.COST_CENTER
		, Source.START_RENT, Source.END_RENT, Source.SUPPLY_METHOD, Source.VENDOR, Source.VEHICLE_FUNCTION
		, Source.REGIONAL, Source.CITY, Source.TRANSMISSION, Source.FUEL_TYPE, Source.BRANDING, Source.COLOR
		, Source.AIRBAG, Source.AIRBAG, Source.CHASIS_NUMBER, Source.ENGINE_NUMBER, Source.VEHICLE_STATUS
		, Source.ASSET_NUMBER, Source.END_DATE, Source.RESTITUTION, Source.MONTHLY_HMS_INSTALLMENT, Source.VAT, Source.TOTAL_MONTHLY_CHARGE
		, Source.PO_NUMBER, Source.PO_LINE, MONTH(Source.CREATED_DATE), YEAR(Source.CREATED_DATE), Source.CREATED_DATE);

END