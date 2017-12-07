CREATE TABLE [dbo].[VEHICLE_HISTORY_REPORT_DATA](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DATE] [date] NULL,
	[EMPLOYEE] [nvarchar](50) NULL,
	[DESCRIPTION] [nvarchar](MAX) NULL,
	[REPORT_MONTH] [int] NULL,
	[REPORT_YEAR] [int] NULL,
	[CREATED_DATE] [datetime] NOT NULL,
CONSTRAINT [PK_VEHICLE_HISTORY_REPORT_DATA] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

) ON [PRIMARY]

GO