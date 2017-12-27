CREATE FUNCTION [dbo].[fGetHoliday](@StartDate datetime, @EndDate datetime)  
RETURNS int   
AS   
BEGIN  
    DECLARE @periode int;  
    DECLARE @i int;  
    DECLARE @totalSunday int;  
    DECLARE @totalHoliday int;  

   --Begin Sunday
		set @totalSunday = 0
		set @totalHoliday = 0
		set @i = 0
		set @periode = DATEDIFF(Day,@StartDate,@EndDate)
		while @i <= @periode
		begin
			if DATENAME(WEEKDAY,(dateadd(DAY,@i,@StartDate))) in (N'Saturday',N'Sunday')
			select @totalSunday = @totalSunday + 1
			set @i = @i + 1
		end
	--End Sunday  

	--Begin Holiday
		select @totalHoliday = count(b.MST_HOLIDAY_DATE) from MST_HOLIDAY_CALENDAR b where b.MST_HOLIDAY_DATE >= @StartDate and b.MST_HOLIDAY_DATE <= @EndDate;
	--End Holiday
	SET @totalHoliday = @totalHoliday + @totalSunday;
    RETURN @totalHoliday;  
END
GO

CREATE TABLE [dbo].[PO_REPORT_DATA](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[POLICE_NUMBER] [nvarchar](50) NULL,
	[SUPPLY_METHOD] [nvarchar](50) NULL,
	[EMPLOYEE_NAME] [nvarchar](100) NULL,
	[COST_CENTER] [char](10) NULL,
	[MANUFACTURER] [nvarchar](50) NULL,
	[MODEL] [nvarchar](50) NULL,
	[SERIES] [nvarchar](50) NULL,
	[BODY_TYPE] [nvarchar](50) NULL,
	[COLOR] [nvarchar](50) NULL,
	[CHASIS_NUMBER] [nvarchar](50) NULL,
	[ENGINE_NUMBER] [nvarchar](50) NULL,
	[VEHICLE_TYPE] [nvarchar](50) NULL,
	[VEHICLE_USAGE] [nvarchar](50) NULL,
	[PO_NUMBER] [nvarchar](50) NULL,
	[PO_LINE] [nvarchar](50) NULL,
	[REPORT_MONTH] [int] NULL,
	[REPORT_YEAR] [int] NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[MST_FLEET_ID] [bigint] NULL,
	[START_CONTRACT] [datetime] NULL,
	[END_CONTRACT] [datetime] NULL,
	[VENDOR] [nvarchar](100) NULL,
	[MONTHLY_INSTALLMENT] [decimal](18, 2) NULL,
	[GST] [decimal](18, 2) NULL,
	[TOTAL_MONTHLY_INSTALLMENT] [decimal](18, 2) NULL,
	[EMPLOYEE_ID] [nvarchar](9) NULL,
	[VEHICLE_FUNCTION] [nvarchar](50) NULL,
 CONSTRAINT [PK_PO_REPORT_DATA] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PO_REPORT_DATA]  WITH CHECK ADD FOREIGN KEY([EMPLOYEE_ID])
REFERENCES [dbo].[MST_EMPLOYEE] ([EMPLOYEE_ID])
GO


/****** Object:  StoredProcedure [dbo].[GetFuelCostData]    Script Date: 26/12/2017 10:24:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[GetFuelCostData]
AS BEGIN

	MERGE FUEL_COST_BY_FUNC_REPORT_DATA AS Target
	USING (
		select SUM(FO.COST) AS TOTAL_FUEL_COST, FO.VEHICLE_TYPE, CC.FUNCTION_NAME, LM.REGION, 
		FO.DATE_OF_COST 
		FROM MST_FUEL_ODOMETER FO
		LEFT JOIN MST_EMPLOYEE E ON FO.EMPLOYEE_ID = E.EMPLOYEE_ID
		LEFT JOIN MST_FUNCTION_GROUP CC ON IIF(FO.VEHICLE_TYPE = 'BENEFIT', E.COST_CENTER, FO.COST_CENTER) = CC.COST_CENTER
		LEFT JOIN MST_LOCATION_MAPPING LM ON E.BASETOWN = LM.BASETOWN
		GROUP BY FO.VEHICLE_TYPE, CC.FUNCTION_NAME, LM.REGION, FO.DATE_OF_COST
	) AS Source
	
	On (Source.VEHICLE_TYPE = Target.VEHICLE_TYPE OR (Source.VEHICLE_TYPE IS NULL AND Target.VEHICLE_TYPE IS NULL))
	AND (Source.FUNCTION_NAME = Target.[FUNCTION] OR (Source.FUNCTION_NAME IS NULL AND Target.[FUNCTION] IS NULL))
	AND (Source.REGION = Target.REGION OR (Source.REGION IS NULL AND Target.REGION IS NULL))
	AND (Source.DATE_OF_COST = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (TOTAL_FUEL_COST, VEHICLE_TYPE, [FUNCTION], REGION, REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
		VALUES( Source.TOTAL_FUEL_COST, Source.VEHICLE_TYPE, Source.FUNCTION_NAME, Source.REGION, 
		MONTH(Source.DATE_OF_COST), YEAR(Source.DATE_OF_COST), Source.DATE_OF_COST);

END

/****** Object:  StoredProcedure [dbo].[GetLeaseCostData]    Script Date: 26/12/2017 21:59:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[GetLeaseCostData]
AS BEGIN

	MERGE LEASE_COST_BY_FUNC_REPORT_DATA AS Target
	USING (
		select VEHICLE_FUNCTION, SUM(TOTAL_MONTHLY_CHARGE) AS TOTAL_LEASE_COST, START_CONTRACT, REGIONAL FROM MST_FLEET
		WHERE (IS_ACTIVE = 1 )
		AND (PROJECT = 1)
		GROUP BY VEHICLE_FUNCTION, START_CONTRACT, REGIONAL
	) AS Source
	ON (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
	AND (Source.START_CONTRACT = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([FUNCTION], TOTAL_LEASE_COST, REPORT_MONTH, REPORT_YEAR, CREATED_DATE, REGION)
		VALUES( Source.VEHICLE_FUNCTION, Source.TOTAL_LEASE_COST, MONTH(Source.START_CONTRACT), 
		YEAR(Source.START_CONTRACT), Source.START_CONTRACT, Source.REGIONAL);

END
GO

-- =============================================
-- Author:		Teguh Pratama
-- Create date: 12/12/2017 18:18
-- Description:	For AC_VS_OB_REPORT_DATA table, mohon update/edit jika keliru, thanks.
-- =============================================
ALTER PROCEDURE [dbo].[GetAcVsOb]
	-- Add the parameters for the stored procedure here


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    -- CRF --
    MERGE AC_VS_OB_REPORT_DATA AS Target
    USING (
      SELECT MST_FLEET.VEHICLE_FUNCTION AS VEHICLE_FUNCTION, TRA_CRF.PRICE AS PRICE, TRA_CRF.CREATED_DATE AS CREATED_DATE, MST_COST_OB.OB_COST AS OB_COST
      FROM TRA_CRF
      INNER JOIN MST_FLEET ON MST_FLEET.MST_FLEET_ID = TRA_CRF.MST_FLEET_ID
      INNER JOIN MST_COST_OB ON MST_COST_OB.COST_CENTER = TRA_CRF.COST_CENTER_NEW
      WHERE TRA_CRF.IS_ACTIVE = 1
    ) AS Source
    ON (Source.PRICE = Target.ACTUAL_COST OR (Source.PRICE IS NULL AND Target.ACTUAL_COST IS NULL))
      AND (Source.OB_COST = Target.COST_OB OR (Source.OB_COST IS NULL AND Target.COST_OB IS NULL))
      AND ('TRA_CRF' = Target.DOCUMENT_FROM_TYPE)
      AND (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
      AND (MONTH(Source.CREATED_DATE) = Target.REPORT_MONTH OR (Source.CREATED_DATE IS NULL AND Target.REPORT_MONTH IS NULL))
      AND (YEAR(Source.CREATED_DATE) = Target.REPORT_YEAR OR (Source.CREATED_DATE IS NULL AND Target.REPORT_YEAR IS NULL))
    WHEN NOT MATCHED BY Target THEN
    INSERT ([ACTUAL_COST]
      ,[COST_OB]
      ,[DOCUMENT_FROM_TYPE]
      ,[FUNCTION]
      ,[REPORT_MONTH]
      ,[REPORT_YEAR]
      ,[CREATED_DATE])
    VALUES (Source.PRICE
      ,Source.OB_COST
      ,'TRA_CRF'
      ,Source.VEHICLE_FUNCTION
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,Source.CREATED_DATE);

    -- CSF --
    MERGE AC_VS_OB_REPORT_DATA AS Target
    USING (
      SELECT TRA_CSF.CREATED_DATE AS CREATED_DATE, MST_COST_OB.OB_COST AS OB_COST, MST_FLEET.VEHICLE_FUNCTION AS VEHICLE_FUNCTION, MST_PRICELIST.PRICE AS PRICE
      FROM TRA_CSF
      INNER JOIN MST_COST_OB ON MST_COST_OB.COST_CENTER = TRA_CSF.COST_CENTER
      INNER JOIN MST_FLEET ON MST_FLEET.ENGINE_NUMBER = TRA_CSF.VENDOR_ENGINE_NUMBER
        AND MST_FLEET.CHASIS_NUMBER = TRA_CSF.VENDOR_CHASIS_NUMBER
      INNER JOIN MST_PRICELIST ON MST_PRICELIST.MANUFACTURER = MST_FLEET.MANUFACTURER
        AND MST_PRICELIST.MODEL = MST_FLEET.MODEL
        AND MST_PRICELIST.SERIES = MST_FLEET.SERIES
        AND MST_PRICELIST.YEAR = MST_FLEET.VEHICLE_YEAR
      WHERE TRA_CSF.IS_ACTIVE = 1
    ) AS Source
    ON (Source.PRICE = Target.ACTUAL_COST OR (Source.PRICE IS NULL AND Target.ACTUAL_COST IS NULL))
      AND (Source.OB_COST = Target.COST_OB OR (Source.OB_COST IS NULL AND Target.COST_OB IS NULL))
      AND ('TRA_CSF' = Target.DOCUMENT_FROM_TYPE)
      AND (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
      AND (MONTH(Source.CREATED_DATE) = Target.REPORT_MONTH OR (Source.CREATED_DATE IS NULL AND Target.REPORT_MONTH IS NULL))
      AND (YEAR(Source.CREATED_DATE) = Target.REPORT_YEAR OR (Source.CREATED_DATE IS NULL AND Target.REPORT_YEAR IS NULL))
    WHEN NOT MATCHED BY Target THEN
    INSERT ([ACTUAL_COST]
      ,[COST_OB]
      ,[DOCUMENT_FROM_TYPE]
      ,[FUNCTION]
      ,[REPORT_MONTH]
      ,[REPORT_YEAR]
      ,[CREATED_DATE])
    VALUES (Source.PRICE
      ,Source.OB_COST
      ,'TRA_CSF'
      ,Source.VEHICLE_FUNCTION
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,Source.CREATED_DATE);

    -- CTF --
    MERGE AC_VS_OB_REPORT_DATA AS Target
    USING (
      SELECT TRA_CTF.BUY_COST_TOTAL AS BUY_COST_TOTAL, TRA_CTF.CREATED_DATE AS CREATED_DATE, MST_COST_OB.OB_COST AS OB_COST, MST_FLEET.VEHICLE_FUNCTION AS VEHICLE_FUNCTION
      FROM TRA_CTF
      INNER JOIN MST_COST_OB ON MST_COST_OB.COST_CENTER = TRA_CTF.COST_CENTER
      INNER JOIN MST_FLEET ON MST_FLEET.POLICE_NUMBER = TRA_CTF.POLICE_NUMBER
      WHERE TRA_CTF.IS_ACTIVE = 1
    ) AS Source
    ON (Source.BUY_COST_TOTAL = Target.ACTUAL_COST OR (Source.BUY_COST_TOTAL IS NULL AND Target.ACTUAL_COST IS NULL))
      AND (Source.OB_COST = Target.COST_OB OR (Source.OB_COST IS NULL AND Target.COST_OB IS NULL))
      AND ('TRA_CTF' = Target.DOCUMENT_FROM_TYPE)
      AND (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
      AND (MONTH(Source.CREATED_DATE) = Target.REPORT_MONTH OR (Source.CREATED_DATE IS NULL AND Target.REPORT_MONTH IS NULL))
      AND (YEAR(Source.CREATED_DATE) = Target.REPORT_YEAR OR (Source.CREATED_DATE IS NULL AND Target.REPORT_YEAR IS NULL))
    WHEN NOT MATCHED BY Target THEN
    INSERT ([ACTUAL_COST]
      ,[COST_OB]
      ,[DOCUMENT_FROM_TYPE]
      ,[FUNCTION]
      ,[REPORT_MONTH]
      ,[REPORT_YEAR]
      ,[CREATED_DATE])
    VALUES (Source.BUY_COST_TOTAL
      ,Source.OB_COST
      ,'TRA_CTF'
      ,''
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,Source.CREATED_DATE);

    -- Temporary --
    MERGE AC_VS_OB_REPORT_DATA AS Target
    USING (
      SELECT TRA_TEMPORARY.CREATED_DATE AS CREATED_DATE, MST_COST_OB.OB_COST AS OB_COST, MST_FLEET.VEHICLE_FUNCTION AS VEHICLE_FUNCTION, MST_PRICELIST.PRICE AS PRICE
      FROM TRA_TEMPORARY
      INNER JOIN MST_COST_OB ON MST_COST_OB.COST_CENTER = TRA_TEMPORARY.COST_CENTER
      INNER JOIN MST_FLEET ON MST_FLEET.ENGINE_NUMBER = TRA_TEMPORARY.VENDOR_ENGINE_NUMBER
        AND MST_FLEET.CHASIS_NUMBER = TRA_TEMPORARY.VENDOR_CHASIS_NUMBER
      INNER JOIN MST_PRICELIST ON MST_PRICELIST.MANUFACTURER = MST_FLEET.MANUFACTURER
        AND MST_PRICELIST.MODEL = MST_FLEET.MODEL
        AND MST_PRICELIST.SERIES = MST_FLEET.SERIES
        AND MST_PRICELIST.YEAR = MST_FLEET.VEHICLE_YEAR
      WHERE TRA_TEMPORARY.IS_ACTIVE = 1
    ) AS Source
    ON (Source.PRICE = Target.ACTUAL_COST OR (Source.PRICE IS NULL AND Target.ACTUAL_COST IS NULL))
      AND (Source.OB_COST = Target.COST_OB OR (Source.OB_COST IS NULL AND Target.COST_OB IS NULL))
      AND ('TRA_TEMPORARY' = Target.DOCUMENT_FROM_TYPE)
      AND (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
      AND (MONTH(Source.CREATED_DATE) = Target.REPORT_MONTH OR (Source.CREATED_DATE IS NULL AND Target.REPORT_MONTH IS NULL))
      AND (YEAR(Source.CREATED_DATE) = Target.REPORT_YEAR OR (Source.CREATED_DATE IS NULL AND Target.REPORT_YEAR IS NULL))
    WHEN NOT MATCHED BY Target THEN
    INSERT ([ACTUAL_COST]
      ,[COST_OB]
      ,[DOCUMENT_FROM_TYPE]
      ,[FUNCTION]
      ,[REPORT_MONTH]
      ,[REPORT_YEAR]
      ,[CREATED_DATE])
    VALUES (Source.PRICE
      ,Source.OB_COST
      ,'TRA_TEMPORARY'
      ,Source.VEHICLE_FUNCTION
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,Source.CREATED_DATE);
END
GO

ALTER PROCEDURE [dbo].[KPICoordinator] 
	-- Add the parameters for the stored procedure here
	@TraCCFId int
AS
BEGIN
	DECLARE 
	@RoolType varchar(10),
	@CoordinatorKpi int,
	@CoorPromiseDate date,
	@CoorResponDate date,
	@VendorPromiseDate date,
	@VendorResponDate date,
	@TotDetil int,
	@n int,
	@tanggalAwal as datetime,
	@tanggalakhir as datetime,
	@totalHoliday as int

	-- Add the T-SQL statements to compute the return value here
	select @RoolType = a.ROLE_TYPE from MST_COMPLAINT_CATEGORY a, TRA_CCF b where a.MST_COMPLAINT_CATEGORY_ID = b.COMPLAINT_CATEGORY and b.TRA_CCF_ID = @TraCCFId
	if (@RoolType = 'HR')  
	-- If HR
	  begin
	   select @TotDetil = count(a.TRA_CCF_DETAIL_ID) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
	   if (@TotDetil = 1)
		   begin
				select @tanggalAwal = a.COMPLAINT_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				select @tanggalakhir = a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				set @totalHoliday = dbo.fGetHoliday(@tanggalAwal,@tanggalakhir);

				select @CoordinatorKpi = DateDiff(Day,a.COMPLAINT_DATE,a.COORDINATOR_RESPONSE_DATE) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				set @CoordinatorKpi = @CoordinatorKpi - @totalHoliday
		   end
		else
		   begin
		   -- First Row
			set @n = 1
			while (@n <= @TotDetil)
				begin
					select @CoorResponDate = t1.COORDINATOR_RESPONSE_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
					select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1
					set @totalHoliday = dbo.fGetHoliday(@CoorPromiseDate,@CoorResponDate)

					select @CoordinatorKpi = DateDiff(Day,@CoorPromiseDate,@CoorResponDate)
					set @CoordinatorKpi = @CoordinatorKpi - @totalHoliday
					set @n = @n + 1
				end
		   end
	 end
	 else
	 -- If Fleet
	  begin
	   select @TotDetil = count(a.TRA_CCF_DETAIL_ID) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
	   if (@TotDetil = 1)
			begin
				select @tanggalAwal = a.COMPLAINT_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				select @tanggalakhir = a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				set @totalHoliday = dbo.fGetHoliday(@tanggalAwal,@tanggalakhir)

				select @CoordinatorKpi = DateDiff(Day,a.COMPLAINT_DATE,a.COORDINATOR_RESPONSE_DATE) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				set @CoordinatorKpi = @CoordinatorKpi -@totalHoliday
			end
			else
			begin
				set @n = 1
				while (@n <= @TotDetil)
					begin
						select @CoorResponDate = t1.COORDINATOR_RESPONSE_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
						select @VendorPromiseDate = t1.VENDOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1
						select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1

						if (@n = 2)
						begin
						-- Second Row More Until Last Row -1
							if(@VendorPromiseDate = null)
							begin
								set @totalHoliday = dbo.fGetHoliday(@CoorPromiseDate,@CoorResponDate)
								select @CoordinatorKpi = DateDiff(Day,@CoorPromiseDate,@CoorResponDate)
								set @CoordinatorKpi = @CoordinatorKpi - @totalHoliday
							end
							else
							begin
								set @totalHoliday = dbo.fGetHoliday(@VendorPromiseDate,@CoorResponDate)
								select @CoordinatorKpi = DateDiff(Day,@VendorPromiseDate,@CoorResponDate)
								set @CoordinatorKpi = @CoordinatorKpi - @totalHoliday
							end
						end
						else if (@n > 2)
						begin
						-- Last Row
							select @VendorPromiseDate = t1.VENDOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
							select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
							set @totalHoliday = dbo.fGetHoliday(@VendorPromiseDate,@CoorResponDate)

							select @CoordinatorKpi = DateDiff(Day,@VendorPromiseDate,@CoorResponDate)
							set @CoordinatorKpi = @CoordinatorKpi - @totalHoliday
						end
						set @n = @n + 1
					end
			end
	  end
	  update TRA_CCF set COORDINATOR_KPI = @CoordinatorKpi where TRA_CCF_ID = @TraCCFId
END
GO


ALTER PROCEDURE [dbo].[KPIVendor] 
	-- Add the parameters for the stored procedure here
	@TraCCFId int
AS
BEGIN
	DECLARE 
	@RoolType varchar(10),
	@VendorKpi int,
	@CoorPromiseDate date,
	@CoorResponDate date,
	@VendorPromiseDate date,
	@VendorResponDate date,
	@TotDetil int,
	@n int,

	@tanggalAwal as datetime,
	@tanggalakhir as datetime,
	@totalHoliday as int

	-- Add the T-SQL statements to compute the return value here
	select @TotDetil = count(a.TRA_CCF_DETAIL_ID) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
	if (@TotDetil = 1)
	begin
		-- First Row
		select @tanggalAwal = a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
		select @tanggalakhir = a.VENDOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
		set @totalHoliday = dbo.fGetHoliday(@tanggalAwal,@tanggalakhir);

		select @VendorKpi = DateDiff(Day,a.COORDINATOR_RESPONSE_DATE,a.VENDOR_RESPONSE_DATE) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
		set @VendorKpi = @VendorKpi - @totalHoliday
	end
	else
	begin
		set @n = 1
		while (@n <= @TotDetil)
		begin
		select @CoorResponDate = t1.COORDINATOR_RESPONSE_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
		select @VendorPromiseDate = t1.VENDOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1
		select @VendorResponDate = t1.VENDOR_RESPONSE_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
		select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1
		if (@n = 2)
			begin
			-- Second Row More Until Last Row -1
			set @totalHoliday = dbo.fGetHoliday(@VendorPromiseDate,@VendorResponDate);

			select @VendorKpi = DateDiff(Day,@VendorPromiseDate,@VendorResponDate)
			set @VendorKpi = @VendorKpi - @totalHoliday
			end
		else if (@n > 2)
			begin
			-- Last Row
			select @VendorPromiseDate = t1.VENDOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
			select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
			set @totalHoliday = dbo.fGetHoliday(@VendorPromiseDate,@CoorPromiseDate);

			select @VendorKpi = DateDiff(Day,@VendorPromiseDate,@CoorPromiseDate)
			set @VendorKpi = @VendorKpi - @totalHoliday
			end
		set @n = @n + 1
		end
	end
	update TRA_CCF set VENDOR_KPI = @VendorKpi where TRA_CCF_ID = @TraCCFId
END
GO