USE [FMS]
GO
/****** Object:  UserDefinedFunction [dbo].[fGetGRQTY]    Script Date: 2/14/2018 3:18:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[fGetGRQTY] 
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
		if DAY(@start_contract) > 3 and DAY(@start_contract) <= 20 set @result =1 - convert(decimal(18,2), (31 - DAY(@start_contract)) )/ 30;
		if DAY(@start_contract) > 20 and DAY(@start_contract) <= DAY(EOMONTH(@start_contract))
		begin
			if DATEDIFF(month, @start_contract, DATEFROMPARTS(@current_year,@current_month,1)) = 1
			begin 
				set @result = 1;
			end 
			else
			begin
				set @result = 1  - ( convert(decimal(18,2),DATEDIFF(day,@start_contract,DATEADD(day,3,EOMONTH(@start_contract)))) / 30) ;
			end
			
		end

		return @result;
		
	end
	
	if DATEDIFF(month, @start_contract, DATEFROMPARTS(@current_year,@current_month,1)) = 1
	begin
		set @result = 1;

		
		if DAY(@start_contract) > 20 and DAY(@start_contract) <= DAY(EOMONTH(@start_contract))
		begin
			set @result =( convert(decimal(18,2),DATEDIFF(day,@start_contract,DATEADD(day,3,EOMONTH(@start_contract)))) / 30) + 1;
		end

				

		return @result;
	end

	if MONTH(@start_contract) = @current_month and YEAR(@start_contract) = @current_year
	begin
		set @result = 0;

		if DAY(@start_contract) > 3 and DAY(@start_contract) <= 20 set @result = convert(decimal(18,2), (31 - DAY(@start_contract)) )/ 30;
		if DAY(@start_contract) < 3 set @result = 1;

		return @result;
	end

	
	return @result;
	

	
	

END
