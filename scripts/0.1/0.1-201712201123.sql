ALTER TABLE TRA_TEMPORARY ADD
	COMMENTS NVARCHAR(255) NULL,
	PRICE decimal(18, 2) NULL,
	VAT_DECIMAL decimal(18, 2) NULL;
	
ALTER TABLE TRA_CSF ADD
	COMMENTS NVARCHAR(255) NULL,
	PRICE decimal(18, 2) NULL,
	VAT_DECIMAL decimal(18, 2) NULL;
	
ALTER TABLE MST_FLEET ADD
	PRICE decimal(18, 2) NULL;