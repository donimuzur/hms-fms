CREATE TABLE [dbo].[CFM_IDLE_REPORT_DATA](
	[POLICE_NUMBER] [nvarchar](50) NULL,
	[MANUFACTURER] [nvarchar](50) NULL,
	[MODEL] [nvarchar](50) NULL,
	[SERIES] [nvarchar](50) NULL,
	[BODY_TYPE] [nvarchar](50) NULL,
	[COLOR] [nvarchar](50) NULL,
	[GROUP_LEVEL] [nvarchar](50) NULL,
	[START_CONTRACT] [date] NULL,
	[END_CONTRACT] [date] NULL,
	[SUPPLY_METHOD] [nvarchar](50) NULL,
	[VENDOR] [nvarchar](100) NULL,
	[COST_CENTER] [char](10) NULL,
	[TRANSMISSION] [nvarchar](50) NULL,
	[FUEL_TYPE] [nvarchar](50) NULL,
	[START_IDLE] [date] NULL,
	[END_IDLE] [date] NULL,
	[MONTHLY_INSTALLMENT] [decimal](18,2) NULL
		
) ON [PRIMARY]

GO