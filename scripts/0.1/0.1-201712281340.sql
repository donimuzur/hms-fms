/****** Object:  StoredProcedure [dbo].[GetOdometer]    Script Date: 12/28/2017 1:20:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[GetOdometer]
AS BEGIN

	DECLARE @tempOdoTable TABLE(
	policeNumber nvarchar(50),
    functionName nvarchar(50),
    region nvarchar(50),
    vehicleType nvarchar(50),
	reportMonth int,
	reportYear int,
	totalKm decimal(18,2),
    createdDate datetime);

	MERGE @tempOdoTable AS Target
		USING (
			select FO.LAST_KM, FO.POLICE_NUMBER, FO.VEHICLE_TYPE, CC.FUNCTION_NAME, LM.REGION, FO.DATE_OF_COST 
			FROM MST_FUEL_ODOMETER FO
			LEFT JOIN MST_EMPLOYEE E ON FO.EMPLOYEE_ID = E.EMPLOYEE_ID
			LEFT JOIN MST_FUNCTION_GROUP CC ON IIF(FO.VEHICLE_TYPE = 'BENEFIT', E.COST_CENTER, FO.COST_CENTER) = CC.COST_CENTER
			LEFT JOIN MST_LOCATION_MAPPING LM ON E.BASETOWN = LM.BASETOWN
		) AS Source
		On (Source.REGION = Target.REGION OR (Source.REGION IS NULL AND Target.REGION IS NULL))
		AND (Source.VEHICLE_TYPE = Target.vehicleType OR (Source.VEHICLE_TYPE IS NULL AND Target.vehicleType IS NULL))
		AND (Source.FUNCTION_NAME = Target.functionName OR (Source.FUNCTION_NAME IS NULL AND Target.functionName IS NULL))
		AND (Source.DATE_OF_COST = Target.createdDate)
		WHEN NOT MATCHED BY TARGET THEN
			INSERT (policeNumber, totalKm, vehicleType, functionName, region, reportMonth, reportYear, createdDate)
			VALUES( Source.POLICE_NUMBER, Source.LAST_KM, Source.VEHICLE_TYPE, Source.FUNCTION_NAME, Source.REGION, MONTH(Source.DATE_OF_COST), 
			YEAR(Source.DATE_OF_COST), Source.DATE_OF_COST);

	DECLARE @groupTempOdoTable TABLE(
		functionName nvarchar(50),
		region nvarchar(50),
		vehicleType nvarchar(50),
		reportMonth int,
		reportYear int,
		totalKm decimal(18,2),
		totalKmBefore decimal(18,2));

	MERGE @groupTempOdoTable AS Target
		USING (
			select policeNumber, functionName, region, vehicleType, reportMonth, reportYear, 
			max(totalKm) as Total from @tempOdoTable group by
			policeNumber, functionName, region, vehicleType, reportMonth, reportYear
		) AS Source
		On (Source.region = Target.region OR (Source.region IS NULL AND Target.region IS NULL))
		AND (Source.vehicleType = Target.vehicleType OR (Source.vehicleType IS NULL AND Target.vehicleType IS NULL))
		AND (Source.functionName = Target.functionName OR (Source.functionName IS NULL AND Target.functionName IS NULL))
		WHEN NOT MATCHED BY TARGET THEN
			INSERT (totalKm, vehicleType, functionName, region, reportMonth, reportYear)
			VALUES( Source.Total, Source.vehicleType, Source.functionName, Source.REGION, Source.reportMonth, 
			Source.reportYear);

	declare @functionName2 nvarchar(50),
		@region2 nvarchar(50),
		@vehicleType2 nvarchar(50),
		@reportMonth2 int,
		@reportYear2 int,
		@totalKm2 decimal(18,2),
		@totalKmBefore2 decimal(18,2);

	declare cursorFillOdometer cursor for
		select functionName, region, vehicleType, reportMonth, reportYear, totalKm FROM @groupTempOdoTable;

	open cursorFillOdometer;

	BEGIN TRANSACTION

	BEGIN TRY
		FETCH NEXT FROM cursorFillOdometer INTO
		@functionName2,@region2,@vehicleType2,@reportMonth2,@reportYear2,@totalKm2;

			WHILE @@FETCH_STATUS = 0
			BEGIN

			select @totalKmBefore2 = max(totalKm) from @groupTempOdoTable where functionName = @functionName2
			and region = @region2 and vehicleType = @vehicleType2 and totalKm < @totalKm2;

			update @groupTempOdoTable set totalKmBefore = @totalKmBefore2 where functionName = @functionName2
			and region = @region2 and vehicleType = @vehicleType2 and reportMonth = @reportMonth2
			and reportYear = @reportYear2 and totalKm = @totalKm2

			FETCH NEXT FROM cursorFillOdometer INTO
				@functionName2,@region2,@vehicleType2,@reportMonth2,@reportYear2,@totalKm2;

			END

			commit TRANSACTION;
			END TRY
			BEGIN CATCH
				SELECT ERROR_MESSAGE() AS ErrorMessage;
				ROLLBACK TRANSACTION
			END CATCH

			close cursorFillOdometer;
			deallocate cursorFillOdometer;

	MERGE ODOMETER_REPORT_DATA AS Target
		USING (
			SELECT functionName, region, vehicleType, reportMonth,
			reportYear, totalKm, totalKmBefore, (totalKm - IIF(totalKmBefore is NULL, totalKm, totalKmBefore)) as usage,
			IIF(totalKmBefore is NULL, 0, (totalKm / (totalKm - totalKmBefore))) as totalKmLiter
			FROM @groupTempOdoTable
		) AS Source
		On (Source.region = Target.region OR (Source.region IS NULL AND Target.region IS NULL))
		AND (Source.vehicleType = Target.VEHICLE_TYPE OR (Source.vehicleType IS NULL AND Target.VEHICLE_TYPE IS NULL))
		AND (Source.functionName = Target.[FUNCTION] OR (Source.functionName IS NULL AND Target.[FUNCTION] IS NULL))
		WHEN NOT MATCHED BY TARGET THEN
			INSERT (TOTAL_KM, VEHICLE_TYPE, [FUNCTION], REGION, REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
			VALUES( Source.totalKmLiter, Source.vehicleType, Source.functionName, Source.region, Source.reportMonth, 
			Source.reportYear, getdate());

END