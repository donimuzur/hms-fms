USE [FMS]
GO
/****** Object:  UserDefinedFunction [dbo].[FN_GetHoliday]    Script Date: 12/26/2017 4:10:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
END; 