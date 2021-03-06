USE [FMS]
GO
/****** Object:  StoredProcedure [dbo].[KPICoordinator]    Script Date: 12/26/2017 3:47:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Dona Doni
-- Create date: 29/11/2017
-- Description:	KPICCF
-- =============================================
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
