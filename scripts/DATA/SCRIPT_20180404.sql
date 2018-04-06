USE [FMS]
GO

UPDATE MST_FLEET
SET PO_NUMBER = '4501463118', PO_LINE='1', START_CONTRACT= '2018-01-01', END_CONTRACT='2018-12-31'
WHERE POLICE_NUMBER ='L9273GH'AND IS_ACTIVE=1
GO

UPDATE MST_FLEET
SET PO_NUMBER = '4501463119', PO_LINE='1', START_CONTRACT= '2018-01-01', END_CONTRACT='2018-12-31'
WHERE POLICE_NUMBER ='L7862G'AND IS_ACTIVE=1
GO

INSERT INTO [dbo].[AUTO_GR]
           ([PO_NUMBER]
           ,[PO_DATE]
           ,[CREATED_DATE]
           ,[IS_POSTED]
           ,[LINE_ITEM]
           ,[QTY_ITEM]
           ,[TERMINATION_DATE])
     VALUES
           ('4501463118'
            ,'2018-01-04 04:00:00'
           ,'2018-01-04 04:00:00'
           ,1
           ,1
           ,3
           ,NULL)
GO


INSERT INTO [dbo].[AUTO_GR]
           ([PO_NUMBER]
           ,[PO_DATE]
           ,[CREATED_DATE]
           ,[IS_POSTED]
           ,[LINE_ITEM]
           ,[QTY_ITEM]
           ,[TERMINATION_DATE])
     VALUES
           ('4501463119'
           ,'2018-01-04 04:00:00'
           ,'2018-01-04 04:00:00'
           ,1
           ,1
           ,3
           ,NULL)
GO



