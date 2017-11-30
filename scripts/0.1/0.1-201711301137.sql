ALTER TABLE dbo.MST_GS ADD
	MANUFACTURER nvarchar(50) NULL,
	MODEL nvarchar(50) null,
	SERIES nvarchar(50) null,
	TRANSMISSION nvarchar(50) null,
	[YEAR] Int,
	GS_MANUFACTURER nvarchar(50) null,
	GS_MODEL nvarchar(50) null,
	GS_SERIES nvarchar(50) null,
	GS_TRANSMISSION nvarchar(50) null,
	LEAD_TIME datetime null
GO