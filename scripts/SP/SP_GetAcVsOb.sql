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
-- Create date: 12/12/2017 17:09
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
      SELECT MST_FLEET.VEHICLE_FUNCTION AS VEHICLE_FUNCTION, TRA_CRF.PRICE AS PRICE, TRA_CRF.CREATED_DATE AS CREATED_DATE, MST_COST_OB.OB_COST AS OB_COST
      FROM TRA_CRF
      INNER JOIN MST_FLEET ON MST_FLEET.MST_FLEET_ID = TRA_CRF.MST_FLEET_ID
      INNER JOIN MST_COST_OB ON MST_COST_OB.COST_CENTER = TRA_CRF.COST_CENTER
      WHERE TRA_CRF.IS_ACTIVE = 1
    ) AS Source
    ON (Source.PRICE = Target.ACTUAL_COST OR (Source.PRICE IS NULL AND Target.ACTUAL_COST IS NULL))
    AND (Source.OB_COST = Target.COST_OB OR (Source.OB_COST IS NULL AND Target.COST_OB IS NULL))
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
      ,Source.OB_COST
      ,'TRA_CRF'
      ,Source.VEHICLE_FUNCTION
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,GETDATE());

    -- CSF --
    MERGE AC_VS_OB_REPORT_DATA AS Target
    USING (
      SELECT TRA_CSF.CREATED_DATE AS CREATED_DATE, MST_COST_OB.OB_COST AS OB_COST
      FROM TRA_CSF
      INNER JOIN MST_COST_OB ON MST_COST_OB.COST_CENTER = TRA_CSF.COST_CENTER
      WHERE TRA_CSF.IS_ACTIVE = 1
    ) AS Source
    ON (Source.OB_COST = Target.COST_OB OR (Source.OB_COST IS NULL AND Target.COST_OB IS NULL))
    AND ('TRA_CSF' = Target.DOCUMENT_FROM_TYPE)
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
    VALUES (''
      ,Source.OB_COST
      ,'TRA_CSF'
      ,''
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,GETDATE());

    -- CTF --
    MERGE AC_VS_OB_REPORT_DATA AS Target
    USING (
      SELECT TRA_CTF.BUY_COST_TOTAL AS BUY_COST_TOTAL, TRA_CTF.CREATED_DATE AS CREATED_DATE, MST_COST_OB.OB_COST AS OB_COST
      FROM TRA_CTF
      INNER JOIN MST_COST_OB ON MST_COST_OB.COST_CENTER = TRA_CTF.COST_CENTER
      WHERE TRA_CTF.IS_ACTIVE = 1
    ) AS Source
    ON (Source.OB_COST = Target.COST_OB OR (Source.OB_COST IS NULL AND Target.COST_OB IS NULL))
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
      ,Source.OB_COST
      ,'TRA_CTF'
      ,''
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,GETDATE());

    -- Temporary --
    MERGE AC_VS_OB_REPORT_DATA AS Target
    USING (
      SELECT TRA_TEMPORARY.CREATED_DATE AS CREATED_DATE, MST_COST_OB.OB_COST AS OB_COST
      FROM TRA_TEMPORARY
      INNER JOIN MST_COST_OB ON MST_COST_OB.COST_CENTER = TRA_TEMPORARY.COST_CENTER
      WHERE TRA_TEMPORARY.IS_ACTIVE = 1
    ) AS Source
    ON (Source.OB_COST = Target.COST_OB OR (Source.OB_COST IS NULL AND Target.COST_OB IS NULL))
    AND ('TRA_TEMPORARY' = Target.DOCUMENT_FROM_TYPE)
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
    VALUES (''
      ,Source.OB_COST
      ,'TRA_TEMPORARY'
      ,''
      ,MONTH(Source.CREATED_DATE)
      ,YEAR(Source.CREATED_DATE)
      ,GETDATE());
END
GO
