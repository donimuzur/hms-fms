Create procedure [dbo].[GetNoOfVehicle]
AS BEGIN

	MERGE NO_OF_VEHICLE_REPORT_DATA AS Target
	USING (
		select VEHICLE_TYPE, SUPPLY_METHOD, VEHICLE_FUNCTION, COUNT(CHASIS_NUMBER) AS NO_OF_VEHICLE, 
		MONTH(CREATED_DATE) AS REPORT_MONTH, YEAR(CREATED_DATE) AS REPORT_YEAR, CREATED_DATE FROM MST_FLEET
		WHERE (IS_ACTIVE = 1 )
		AND (SUPPLY_METHOD = 'Lease' OR SUPPLY_METHOD = 'Service' OR SUPPLY_METHOD = 'Temporary')
		GROUP BY VEHICLE_TYPE, SUPPLY_METHOD, VEHICLE_FUNCTION, CREATED_DATE, MONTH(CREATED_DATE)
	) AS Source
	ON (Source.VEHICLE_TYPE = Target.VEHICLE_TYPE OR (Source.VEHICLE_TYPE IS NULL AND Target.VEHICLE_TYPE IS NULL))
	AND (Source.SUPPLY_METHOD = Target.SUPPLY_METHOD OR (Source.SUPPLY_METHOD IS NULL AND Target.SUPPLY_METHOD IS NULL))
	AND (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
	AND (Source.CREATED_DATE = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (VEHICLE_TYPE, SUPPLY_METHOD, [FUNCTION], NO_OF_VEHICLE, REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
		VALUES( Source.VEHICLE_TYPE, Source.SUPPLY_METHOD, Source.VEHICLE_FUNCTION, Source.NO_OF_VEHICLE, MONTH(Source.CREATED_DATE), 
		YEAR(Source.CREATED_DATE), Source.CREATED_DATE);

END