ALTER TABLE [dbo].[ODOMETER_REPORT_DATA] ADD
	[VEHCILE_TYPE] [nvarchar](50) NULL

ALTER TABLE [dbo].[ACCIDENT_REPORT_DATA] ADD
	[FUNCTION] [nvarchar](50) NULL

ALTER TABLE [dbo].[PO_REPORT_DATA] ADD
	[MST_FLEET_ID] [bigint] NULL

ALTER TABLE [dbo].[SALES_BY_REGION_REPORT_DATA] ADD
	[REGION] [nvarchar](50) NULL

GO