INSERT INTO [dbo].[MST_MODUL]
           ([MST_MODUL_ID]
           ,[MODUL_NAME]
           ,[MODUL_URL]
           ,[MENU_NAME]
           ,[PARENT_MODUL_ID]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[IS_ACTIVE])
     VALUES
           (43
           ,'Help'
           ,'~/Help'
           ,'Help'
           ,1
           ,'SYSTEM'
           ,GETDATE()
           ,NULL
           ,NULL
           ,1)
GO


