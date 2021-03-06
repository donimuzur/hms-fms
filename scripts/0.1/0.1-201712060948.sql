CREATE TABLE [dbo].[FUEL_REPORT_DATA](
	[ID] [int] IDENTITY(1,1) NOT NULL,
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
	[LOCATION] [nvarchar](50) NULL,
	[REPORT_MONTH] [int] NULL,
	[REPORT_YEAR] [int] NULL,
	[CREATED_DATE] [datetime] NULL,
CONSTRAINT [PK_FUEL_REPORT_DATA] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

)ON [PRIMARY]

GO