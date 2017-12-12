use [FMS];
Create procedure [dbo].[GetLeaseCostData]
AS BEGIN

	MERGE LEASE_COST_REPORT_DATA AS Target
	USING (
		select SUM(COST) AS TOTAL_FUEL_COST, VEHICLE_TYPE, [FUNCTION], REGION, MONTH(CREATED_DATE) AS REPORT_MONTH, 
		YEAR(CREATED_DATE) AS REPORT_YEAR, CREATED_DATE FROM MST_FLEET
		WHERE (IS_ACTIVE = 1 )
		AND (YEAR(END_CONTRACT) - YEAR(START_CONTRACT) > 4)
		AND (PROJECT = 1)
		AND (SUPPLY_METHOD = 'Service')
		GROUP BY [FUNCTION], REPORT_MONTH
	) AS Source