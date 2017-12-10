use [FMS];
Create procedure [dbo].[GetFuelData]
AS BEGIN

	MERGE FUEL_COST_REPORT_DATA AS Target
	USING (
		select SUM(COST) AS TOTAL_FUEL_COST, VEHICLE_TYPE, [FUNCTION], REGION, CREATED_DATE FROM MST_FUEL_ODOMETER
		WHERE (IS_ACTIVE = 1 )
		AND (YEAR(END_CONTRACT) - YEAR(START_CONTRACT) > 4)
		AND (PROJECT = 1)
		AND (SUPPLY_METHOD = 'Service')
		GROUP BY MANUFACTURER, BODY_TYPE, CREATED_DATE
	) AS Source
	
	//TODO
	On (Source.MANUFACTURER = Target.MANUFACTURER OR (Source.MANUFACTURER IS NULL AND Target.MANUFACTURER IS NULL))
	AND (Source.BODY_TYPE = Target.BODY_TYPE OR (Source.BODY_TYPE IS NULL AND Target.BODY_TYPE IS NULL))
	AND (Source.CREATED_DATE = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (TOTAL_FUEL_COST, VEHICLE_TYPE, , REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
		VALUES( Source.MANUFACTURER, Source.BODY_TYPE, Source.NO_OF_VEHICLE, MONTH(Source.CREATED_DATE), 
		YEAR(Source.CREATED_DATE), Source.CREATED_DATE);

END