USE [FMS]
GO
/****** Object:  StoredProcedure [dbo].[xmlAutoGRCreate]    Script Date: 12/21/2017 1:41:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Raditya
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[xmlAutoGRCreate] 
	-- Add the parameters for the stored procedure here
	@auto_gr_id as int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	declare @XmlOutput xml;
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT @XmlOutput =
		(SELECT 
			(SELECT 
			'PO Number :' + PO_NUMBER as HEADER_TEXT,
			CONVERT(varchar(20), CURRENT_TIMESTAMP, 104) AS DOC_DATE, 
			CONVERT(varchar(20), PO_DATE, 104) AS POSTING_DATE, 
			PO_NUMBER
			FROM AUTO_GR
			WHERE AUTO_GR_ID = @auto_gr_id and MONTH(PO_DATE) = MONTH(GETDATE()) and IS_POSTED = 0 FOR XML PATH('HEADER'), TYPE),
			(SELECT  LINE_ITEM, QTY_ITEM, 'EA' AS UNIT_ENTRY
			FROM 
			AUTO_GR
			WHERE AUTO_GR.AUTO_GR_ID = @auto_gr_id FOR XML PATH('ITEM'), TYPE) FOR XML PATH('GOODS_RECEIPT'), 
	ROOT('ROOT'))
	select @XmlOutput as XML_CON;
	
END
