CREATE TABLE [dbo].[PO_REPORT_DATA](
	[POLICE_NUMBER] [nvarchar](50) NULL,
	[SUPPLY_METHOD] [nvarchar](50) NULL,
	[EMPLOYEE_NAME] [nvarchar](100) NULL,
	[COST_CENTER] [char](10) NULL,
	[MANUFACTURER] [nvarchar](50) NULL,
	[MODEL] [nvarchar](50) NULL,
	[SERIES] [nvarchar](50) NULL,
	[BODY_TYPE] [nvarchar](50) NULL,
	[COLOR] [nvarchar](50) NULL,
	[CHASIS_NUMBER] [nvarchar](50) NULL,
	[ENGINE_NUMBER] [nvarchar](50) NULL,
	[VEHICLE_TYPE] [nvarchar](50) NULL,
	[VEHICLE_USAGE] [nvarchar](50) NULL,
	[PO_NUMBER] [nvarchar](50) NULL,
	[PO_LINE] [nvarchar](50) NULL,
	[REPORT_MONTH] [int] NULL,
	[REPORT_YEAR] [int] NULL
		
) ON [PRIMARY]

GO