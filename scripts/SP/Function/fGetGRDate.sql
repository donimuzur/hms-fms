-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION dbo.fGetGRDate 
(
	-- Add the parameters for the function here
	@start_contract as datetime,
	@current_month as int,
	@current_year as int
)
RETURNS datetime
AS
BEGIN
	-- Declare the return variable here
	DECLARE @result datetime;
	declare @curr_month as int = 1,
	@curr_year as int;

	if @current_month = 12
	begin
		set @curr_month = 1;
		set @curr_year = @current_year + 1;
	end
	else
	begin 
		set @curr_month = @current_month;
		set @curr_year = @current_year;
	end

	if DAY(@start_contract) > 3 and DAY(@start_contract) <= 20 set @result = DATEFROMPARTS(@curr_year,@curr_month,20);
	if DAY(@start_contract) > 20 and DAY(@start_contract) <= DAY(EOMONTH(@start_contract)) set @result = DATEFROMPARTS(@curr_year,@curr_month,3);
	if DAY(@start_contract) <= 3 set @result = DATEFROMPARTS(@curr_year,@curr_month,3);

	return @result;
END
