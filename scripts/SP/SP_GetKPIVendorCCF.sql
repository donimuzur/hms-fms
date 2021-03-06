USE [FMS]
GO
/****** Object:  StoredProcedure [dbo].[KPIVendor]    Script Date: 12/26/2017 3:59:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
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
