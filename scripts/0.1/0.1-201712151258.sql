BEGIN TRANSACTION
GO
ALTER TABLE dbo.AUTO_GR ADD
	LINE_ITEM int NULL,
	QTY_ITEM decimal(18, 2) NULL,
	TERMINATION_DATE datetime NULL
GO
ALTER TABLE dbo.AUTO_GR SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

