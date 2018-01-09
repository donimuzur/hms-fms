Create procedure [dbo].[GetNoOfVehicle]
AS BEGIN

	MERGE NO_OF_VEHICLE_REPORT_DATA AS Target
	USING (
		select VEHICLE_TYPE, SUPPLY_METHOD, VEHICLE_FUNCTION, COUNT(CHASIS_NUMBER) AS NO_OF_VEHICLE, 
		MONTH(START_CONTRACT) AS REPORT_MONTH, YEAR(START_CONTRACT) AS REPORT_YEAR, START_CONTRACT, REGIONAL FROM MST_FLEET
		WHERE (IS_ACTIVE = 1 )
		AND (SUPPLY_METHOD = 'Lease' OR SUPPLY_METHOD = 'Services' OR SUPPLY_METHOD = 'Temporary' OR SUPPLY_METHOD = 'Extend')
		GROUP BY VEHICLE_TYPE, SUPPLY_METHOD, VEHICLE_FUNCTION, START_CONTRACT, REGIONAL, MONTH(CREATED_DATE)
	) AS Source
	ON (Source.VEHICLE_TYPE = Target.VEHICLE_TYPE OR (Source.VEHICLE_TYPE IS NULL AND Target.VEHICLE_TYPE IS NULL))
	AND (Source.SUPPLY_METHOD = Target.SUPPLY_METHOD OR (Source.SUPPLY_METHOD IS NULL AND Target.SUPPLY_METHOD IS NULL))
	AND (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
	AND (Source.START_CONTRACT = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (VEHICLE_TYPE, SUPPLY_METHOD, [FUNCTION], NO_OF_VEHICLE, REPORT_MONTH, REPORT_YEAR, CREATED_DATE, REGION)
		VALUES( Source.VEHICLE_TYPE, Source.SUPPLY_METHOD, Source.VEHICLE_FUNCTION, Source.NO_OF_VEHICLE, MONTH(Source.START_CONTRACT), 
		YEAR(Source.START_CONTRACT), Source.START_CONTRACT, Source.REGIONAL);

END