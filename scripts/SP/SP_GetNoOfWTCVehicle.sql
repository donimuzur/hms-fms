Create procedure [dbo].[GetNoOfWTCVehicle]
AS BEGIN

	MERGE NO_OF_WTC_VEHICLE_REPORT_DATA AS Target
	USING (
		select VEHICLE_FUNCTION, COUNT(MST_FLEET_ID) AS NO_OF_VEHICLE, REGIONAL,
		MONTH(CREATED_DATE) AS REPORT_MONTH, YEAR(CREATED_DATE) AS REPORT_YEAR, CREATED_DATE FROM MST_FLEET
		WHERE (IS_ACTIVE = 1 )
		AND (VEHICLE_FUNCTION = 'SALES' OR VEHICLE_FUNCTION = 'MARKETING')
		GROUP BY VEHICLE_FUNCTION, REGIONAL, CREATED_DATE, MONTH(CREATED_DATE)
	) AS Source
	On (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
	AND (Source.CREATED_DATE = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([FUNCTION], NO_OF_VEHICLE, REGIONAL, REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
		VALUES( Source.VEHICLE_FUNCTION, Source.NO_OF_VEHICLE, Source.REGIONAL, Source.REPORT_MONTH, 
		Source.REPORT_YEAR, Source.CREATED_DATE);

END