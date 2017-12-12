-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Teguh Pratama
-- Create date: 12/12/2017
-- Description:	For AC_VS_OB_REPORT_DATA table, mohon update/edit jika keliru, thanks.
-- =============================================
CREATE PROCEDURE [dbo].[GetAcVsOb]
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
      SELECT MST_FLEET.TOTAL_MONTHLY_CHARGE AS TOTAL_MONTHLY_CHARGE, MST_FLEET.VEHICLE_FUNCTION AS VEHICLE_FUNCTION, TRA_CRF.PRICE AS PRICE, TRA_CRF.CREATED_DATE AS CREATED_DATE FROM TRA_CRF
      INNER JOIN MST_FLEET ON MST_FLEET.MST_FLEET_ID = TRA_CRF.MST_FLEET_ID
      WHERE TRA_CRF.IS_ACTIVE = 1
    ) AS Source
    ON (Source.PRICE = Target.ACTUAL_COST OR (Source.PRICE IS NULL AND Target.ACTUAL_COST IS NULL))
    AND (Source.TOTAL_MONTHLY_CHARGE = Target.COST_OB OR (Source.TOTAL_MONTHLY_CHARGE IS NULL AND Target.COST_OB IS NULL))
    AND ('TRA_CRF' = Target.DOCUMENT_FROM_TYPE)
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
      ,Source.TOTAL_MONTHLY_CHARGE
      ,'TRA_CRF'
      ,Source.VEHICLE_FUNCTION
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,GETDATE());

    -- CSF --
    MERGE AC_VS_OB_REPORT_DATA AS Target
    USING (
      SELECT CREATED_DATE FROM TRA_CSF
      WHERE IS_ACTIVE = 1
    ) AS Source
    ON ('TRA_CSF' = Target.DOCUMENT_FROM_TYPE)
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
    VALUES (NULL
      ,NULL
      ,'TRA_CSF'
      ,NULL
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,GETDATE());

    -- CTF --
    MERGE AC_VS_OB_REPORT_DATA AS Target
    USING (
      SELECT BUY_COST, BUY_COST_TOTAL, CREATED_DATE FROM TRA_CTF
      WHERE IS_ACTIVE = 1
    ) AS Source
    ON (Source.BUY_COST = Target.COST_OB OR (Source.BUY_COST IS NULL AND Target.COST_OB IS NULL))
    AND (Source.BUY_COST_TOTAL = Target.ACTUAL_COST OR (Source.BUY_COST_TOTAL IS NULL AND Target.ACTUAL_COST IS NULL))
    AND ('TRA_CTF' = Target.DOCUMENT_FROM_TYPE)
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
      ,Source.BUY_COST
      ,'TRA_CTF'
      ,NULL
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,GETDATE());
END
GO
