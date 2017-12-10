use [FMS];
Create procedure [dbo].[GetNoOfVehicleMakeType]
AS BEGIN

	MERGE NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA AS Target
	USING (
		select MANUFACTURER, BODY_TYPE, COUNT(MST_FLEET_ID) AS NO_OF_VEHICLE, 
		MONTH(CREATED_DATE) AS REPORT_MONTH, YEAR(CREATED_DATE) AS REPORT_YEAR, CREATED_DATE FROM MST_FLEET
		WHERE (IS_ACTIVE = 1 )
		AND (YEAR(END_CONTRACT) - YEAR(START_CONTRACT) > 4)
		AND (PROJECT = 1)
		AND (SUPPLY_METHOD = 'Service')
		GROUP BY MANUFACTURER, BODY_TYPE, REPORT_MONTH
	) AS Source
	On (Source.MANUFACTURER = Target.MANUFACTURER OR (Source.MANUFACTURER IS NULL AND Target.MANUFACTURER IS NULL))
	AND (Source.BODY_TYPE = Target.BODY_TYPE OR (Source.BODY_TYPE IS NULL AND Target.BODY_TYPE IS NULL))
	AND (Source.CREATED_DATE = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (MANUFACTURER, BODY_TYPE, NO_OF_VEHICLE, REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
		VALUES( Source.MANUFACTURER, Source.BODY_TYPE, Source.NO_OF_VEHICLE, Source.REPORT_MONTH, 
		Source.REPORT_YEAR, Source.CREATED_DATE);

END