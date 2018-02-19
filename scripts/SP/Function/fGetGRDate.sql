USE [FMS]
GO
/****** Object:  UserDefinedFunction [dbo].[fGetGRDate]    Script Date: 2/19/2018 4:00:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[fGetGRDate] 
(
	-- Add the parameters for the function here
	@start_contract as datetime,
	@end_contract as datetime,
	@current_month as int,
	@current_year as int
)
RETURNS datetime
AS
BEGIN

	
	-- Declare the return variable here
	DECLARE @result datetime;
	declare @gr_exist as int =0;
	declare @curr_month as int = 1,
	@curr_year as int;

	set @curr_month = @current_month;
	set @curr_year = @current_year;

	if MONTH(@end_contract) = @current_month and YEAR(@end_contract) = @current_year
	begin
		set @result = DATEFROMPARTS(@curr_year,@curr_month,3);
	end
	else if MONTH(@start_contract) = @current_month and YEAR(@start_contract) = @current_year
	begin
		if DAY(@start_contract) < 3 set @result = DATEFROMPARTS(@curr_year,@curr_month,3);
		else if DAY(@start_contract) >= 3 and DAY(@start_contract) <= 20 set @result = DATEFROMPARTS(@curr_year,@curr_month,20);
	end
	else 
	begin 
		set @result = DATEFROMPARTS(@curr_year,@curr_month,3);
	end
	--if DAY(@start_contract) > 20 and DAY(@start_contract) <= DAY(EOMONTH(@start_contract)) 
	--begin
	--	if @current_month = 12
	--	begin
	--		set @curr_month = 1;
	--		set @curr_year = @current_year + 1;
	--	end
	--	set @result = DATEFROMPARTS(@curr_year,@curr_month,3);
	return @result;
END


