-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION fGetGRQTY 
(
	-- Add the parameters for the function here
	@start_contract as date,
	@end_contract as date,
	@current_month as int,
	@current_year as int
)
RETURNS decimal(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @result as decimal(18,2) = 1;
	
	

	if MONTH(@end_contract) = @current_month and YEAR(@end_contract) = @current_year
	begin
		set @result = 1;

		if DAY(@start_contract) > 3 and DAY(@start_contract) <= 20 set @result = convert(decimal(18,2), DAY(@start_contract)) / 30;
		if DAY(@start_contract) > 20 and DAY(@start_contract) <= DAY(EOMONTH(@start_contract))
			set @result = 2 - convert(decimal(18,2),DATEDIFF(day,@start_contract,DATEADD(day,3,EOMONTH(@start_contract)))) / 30;
		

		return @result;
		
	end
	
	if DATEDIFF(month, @start_contract, DATEFROMPARTS(@current_year,@current_month,1)) = 1
	begin
		set @result = 1;

		
		if DAY(@start_contract) > 20 and DAY(@start_contract) <= DAY(EOMONTH(@start_contract))
			set @result = convert(decimal(18,2),DATEDIFF(day,@start_contract,DATEADD(day,3,EOMONTH(@start_contract)))) / 30;
		

		return @result;
	end

	if MONTH(@start_contract) = @current_month
	begin
		set @result = 0;

		if DAY(@start_contract) > 3 and DAY(@start_contract) <= 20 set @result = convert(decimal(18,2), DAY(@start_contract)) / 30;
		

		return @result;
	end

	
	return @result;
	

	
	

END
GO

