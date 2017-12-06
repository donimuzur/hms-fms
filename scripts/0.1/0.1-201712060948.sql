CREATE TABLE [dbo].[FUEL_REPORT_DATA](
	[POLICE_NUMBER] [nvarchar](50) NULL,
	[LITER] [int] NULL,
	[ODOMETER] [decimal](18,2) NULL,
	[COST] [decimal](18,2) NULL,
	[FUEL_TYPE] [nvarchar](50) NULL,
	[COST_CENTER] [char](10) NULL,
	[FUNCTION] [nvarchar](50) NULL,
	[MANUFACTURER] [nvarchar](50) NULL,
	[MODEL] [nvarchar](50) NULL,
	[SERIES] [nvarchar](50) NULL,
	[BODY_TYPE] [nvarchar](50) NULL,
	[VEHICLE_TYPE] [nvarchar](50) NULL,
	[VEHICLE_USAGE] [nvarchar](50) NULL,
	[LOCATION] [nvarchar](50) NULL

)ON [PRIMARY]

GO