
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/04/2017 15:46:53
-- Generated from EDMX file: D:\VTI\GIT FIX DFIS\hms-dfis\DFIS.EntitiesDAL\EDMX\DFISContextDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [dfis];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_GeneralContImproveSandBag_MasterLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneralContImproveSandBag] DROP CONSTRAINT [FK_GeneralContImproveSandBag_MasterLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_KPIInventoryAccuracy_MasterLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[KPIInventoryAccuracy] DROP CONSTRAINT [FK_KPIInventoryAccuracy_MasterLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_KPIProductivityDetail_KPIProductivity1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[KPIProductivityDetail] DROP CONSTRAINT [FK_KPIProductivityDetail_KPIProductivity1];
GO
IF OBJECT_ID(N'[dbo].[FK_Location_ParentLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MasterLocation] DROP CONSTRAINT [FK_Location_ParentLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterEquipmentType_MasterLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MasterEquipmentType] DROP CONSTRAINT [FK_MasterEquipmentType_MasterLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterRolesFunctionMapping_MasterFunction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MasterRolesFunctionMapping] DROP CONSTRAINT [FK_MasterRolesFunctionMapping_MasterFunction];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterRolesFunctionMapping_MasterRole]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MasterRolesFunctionMapping] DROP CONSTRAINT [FK_MasterRolesFunctionMapping_MasterRole];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterUserLocationMapping_MasterLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MasterUserLocationMapping] DROP CONSTRAINT [FK_MasterUserLocationMapping_MasterLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterUserLocationMapping_MasterUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MasterUserLocationMapping] DROP CONSTRAINT [FK_MasterUserLocationMapping_MasterUser];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterUserRoleMapping_MasterRole]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MasterUserRoleMapping] DROP CONSTRAINT [FK_MasterUserRoleMapping_MasterRole];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterUserRoleMapping_MasterUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MasterUserRoleMapping] DROP CONSTRAINT [FK_MasterUserRoleMapping_MasterUser];
GO
IF OBJECT_ID(N'[dbo].[FK_NotificationSystem_MasterUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NotificationSystem] DROP CONSTRAINT [FK_NotificationSystem_MasterUser];
GO
IF OBJECT_ID(N'[dbo].[FK_TransactionLog_MasterFunction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionLog] DROP CONSTRAINT [FK_TransactionLog_MasterFunction];
GO
IF OBJECT_ID(N'[dbo].[FK_TransactionLog_MasterUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionLog] DROP CONSTRAINT [FK_TransactionLog_MasterUser];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[GenBuildingFacility]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GenBuildingFacility];
GO
IF OBJECT_ID(N'[dbo].[GeneralContImproveSandBag]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeneralContImproveSandBag];
GO
IF OBJECT_ID(N'[dbo].[GeneralContImprovSS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeneralContImprovSS];
GO
IF OBJECT_ID(N'[dbo].[KPIDistributionCost]', 'U') IS NOT NULL
    DROP TABLE [dbo].[KPIDistributionCost];
GO
IF OBJECT_ID(N'[dbo].[KPIDistributionCostSV]', 'U') IS NOT NULL
    DROP TABLE [dbo].[KPIDistributionCostSV];
GO
IF OBJECT_ID(N'[dbo].[KPIInventoryAccuracy]', 'U') IS NOT NULL
    DROP TABLE [dbo].[KPIInventoryAccuracy];
GO
IF OBJECT_ID(N'[dbo].[KPIProductivity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[KPIProductivity];
GO
IF OBJECT_ID(N'[dbo].[KPIProductivityDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[KPIProductivityDetail];
GO
IF OBJECT_ID(N'[dbo].[KPIVehicleDeliveryProcess]', 'U') IS NOT NULL
    DROP TABLE [dbo].[KPIVehicleDeliveryProcess];
GO
IF OBJECT_ID(N'[dbo].[MasterConfiguration]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterConfiguration];
GO
IF OBJECT_ID(N'[dbo].[MasterConfigurationEmail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterConfigurationEmail];
GO
IF OBJECT_ID(N'[dbo].[MasterConvertionPointSS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterConvertionPointSS];
GO
IF OBJECT_ID(N'[dbo].[MasterDynamicField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterDynamicField];
GO
IF OBJECT_ID(N'[dbo].[MasterEquipmentType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterEquipmentType];
GO
IF OBJECT_ID(N'[dbo].[MasterFABrand]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterFABrand];
GO
IF OBJECT_ID(N'[dbo].[MasterFGStacking]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterFGStacking];
GO
IF OBJECT_ID(N'[dbo].[MasterFunction]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterFunction];
GO
IF OBJECT_ID(N'[dbo].[MasterGuideline]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterGuideline];
GO
IF OBJECT_ID(N'[dbo].[MasterHoliday]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterHoliday];
GO
IF OBJECT_ID(N'[dbo].[MasterIconFacility]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterIconFacility];
GO
IF OBJECT_ID(N'[dbo].[MasterList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterList];
GO
IF OBJECT_ID(N'[dbo].[MasterLoadFactorCFP]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterLoadFactorCFP];
GO
IF OBJECT_ID(N'[dbo].[MasterLocation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterLocation];
GO
IF OBJECT_ID(N'[dbo].[MasterRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterRole];
GO
IF OBJECT_ID(N'[dbo].[MasterRolesFunctionMapping]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterRolesFunctionMapping];
GO
IF OBJECT_ID(N'[dbo].[MasterTransportRegion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterTransportRegion];
GO
IF OBJECT_ID(N'[dbo].[MasterUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterUser];
GO
IF OBJECT_ID(N'[dbo].[MasterUserLocationMapping]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterUserLocationMapping];
GO
IF OBJECT_ID(N'[dbo].[MasterUserRoleMapping]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterUserRoleMapping];
GO
IF OBJECT_ID(N'[dbo].[MasterVendor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterVendor];
GO
IF OBJECT_ID(N'[dbo].[NewsHighlight]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NewsHighlight];
GO
IF OBJECT_ID(N'[dbo].[NotificationSystem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NotificationSystem];
GO
IF OBJECT_ID(N'[dbo].[tm_location]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tm_location];
GO
IF OBJECT_ID(N'[dbo].[tm_location_detail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tm_location_detail];
GO
IF OBJECT_ID(N'[dbo].[TransactionLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TransactionLog];
GO
IF OBJECT_ID(N'[DFISModelEntitiesStoreContainer].[KPIDistributionCostView]', 'U') IS NOT NULL
    DROP TABLE [DFISModelEntitiesStoreContainer].[KPIDistributionCostView];
GO
IF OBJECT_ID(N'[DFISModelEntitiesStoreContainer].[KPIInputSVView]', 'U') IS NOT NULL
    DROP TABLE [DFISModelEntitiesStoreContainer].[KPIInputSVView];
GO
IF OBJECT_ID(N'[DFISModelEntitiesStoreContainer].[KPIVehicleDeliveryProcessView]', 'U') IS NOT NULL
    DROP TABLE [DFISModelEntitiesStoreContainer].[KPIVehicleDeliveryProcessView];
GO
IF OBJECT_ID(N'[DFISModelEntitiesStoreContainer].[UserRolesSecurityView]', 'U') IS NOT NULL
    DROP TABLE [DFISModelEntitiesStoreContainer].[UserRolesSecurityView];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'MasterDynamicFields'
CREATE TABLE [dbo].[MasterDynamicFields] (
    [IDDynamicField] int IDENTITY(1,1) NOT NULL,
    [PageName] varchar(100)  NOT NULL,
    [FieldName] varchar(100)  NOT NULL,
    [FieldType] varchar(100)  NOT NULL,
    [GroupCollapse] varchar(100)  NOT NULL,
    [Ordering] int  NOT NULL,
    [NotificationPeriod] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterFunctions'
CREATE TABLE [dbo].[MasterFunctions] (
    [IDFunction] int IDENTITY(1,1) NOT NULL,
    [FunctionName] varchar(200)  NOT NULL,
    [ParentIDFunction] int  NULL,
    [Type] varchar(10)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterLists'
CREATE TABLE [dbo].[MasterLists] (
    [IDList] int IDENTITY(1,1) NOT NULL,
    [FieldName] varchar(200)  NOT NULL,
    [FieldValue] varchar(200)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'tm_location_detail'
CREATE TABLE [dbo].[tm_location_detail] (
    [id] int IDENTITY(1,1) NOT NULL,
    [id_building] int  NULL,
    [id_location] int  NULL,
    [name] varchar(50)  NULL,
    [total] int  NULL,
    [object_data] varchar(max)  NULL,
    [create_date] datetime  NULL
);
GO

-- Creating table 'GenBuildingFacilities'
CREATE TABLE [dbo].[GenBuildingFacilities] (
    [WarehouseID] varchar(100)  NOT NULL,
    [WarehouseName] varchar(200)  NOT NULL,
    [WarehouseAddress] varchar(max)  NOT NULL
);
GO

-- Creating table 'tm_location'
CREATE TABLE [dbo].[tm_location] (
    [id_building] int IDENTITY(1,1) NOT NULL,
    [name] varchar(100)  NULL,
    [total] int  NULL,
    [dataobject] varchar(max)  NULL,
    [create_date] datetime  NULL
);
GO

-- Creating table 'MasterConfigurations'
CREATE TABLE [dbo].[MasterConfigurations] (
    [IDConfiguration] int IDENTITY(1,1) NOT NULL,
    [PageName] varchar(200)  NOT NULL,
    [Description] varchar(max)  NOT NULL,
    [Value] varchar(200)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterConfigurationEmails'
CREATE TABLE [dbo].[MasterConfigurationEmails] (
    [IDConfigMail] int IDENTITY(1,1) NOT NULL,
    [PageName] varchar(200)  NOT NULL,
    [Receiver] varchar(200)  NOT NULL,
    [BodyEmail] varchar(max)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'TransactionLogs'
CREATE TABLE [dbo].[TransactionLogs] (
    [IDLog] int IDENTITY(1,1) NOT NULL,
    [IDFunction] int  NOT NULL,
    [IDUser] varchar(50)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Action] varchar(50)  NOT NULL,
    [Remarks] varchar(1000)  NOT NULL
);
GO

-- Creating table 'MasterUsers'
CREATE TABLE [dbo].[MasterUsers] (
    [IDUser] varchar(50)  NOT NULL,
    [FullName] varchar(500)  NOT NULL,
    [Email] varchar(100)  NOT NULL,
    [Address] varchar(500)  NOT NULL,
    [Phone] varchar(25)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterLoadFactorCFPs'
CREATE TABLE [dbo].[MasterLoadFactorCFPs] (
    [IDLoadFactorCFP] int IDENTITY(1,1) NOT NULL,
    [VehicleType] varchar(100)  NOT NULL,
    [KMperLiter] int  NOT NULL,
    [KgCO2perLiter] int  NOT NULL,
    [BrandCategory] varchar(100)  NOT NULL,
    [MaxQty] int  NOT NULL,
    [HJE] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterConvertionPointSSes'
CREATE TABLE [dbo].[MasterConvertionPointSSes] (
    [IDConvertionPointSS] int IDENTITY(1,1) NOT NULL,
    [ValueFrom] int  NOT NULL,
    [ValueTo] int  NOT NULL,
    [Value] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterFABrands'
CREATE TABLE [dbo].[MasterFABrands] (
    [FACode] varchar(50)  NOT NULL,
    [SpeakingCode] varchar(10)  NOT NULL,
    [Type] varchar(200)  NOT NULL,
    [StickPerBox] decimal(18,4)  NOT NULL,
    [PackPerBox] decimal(18,4)  NOT NULL,
    [StickPerPack] decimal(18,4)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterRoles'
CREATE TABLE [dbo].[MasterRoles] (
    [IDRole] int IDENTITY(1,1) NOT NULL,
    [RoleName] varchar(100)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterRolesFunctionMappings'
CREATE TABLE [dbo].[MasterRolesFunctionMappings] (
    [IDRolesFunctionMapping] int IDENTITY(1,1) NOT NULL,
    [IDRole] int  NOT NULL,
    [IDFunction] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NOT NULL
);
GO

-- Creating table 'MasterUserLocationMappings'
CREATE TABLE [dbo].[MasterUserLocationMappings] (
    [IDUserLocationMapping] int IDENTITY(1,1) NOT NULL,
    [IDUser] varchar(50)  NOT NULL,
    [IDLocation] varchar(50)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterUserRoleMappings'
CREATE TABLE [dbo].[MasterUserRoleMappings] (
    [IDUserRoleMapping] int IDENTITY(1,1) NOT NULL,
    [IDRole] int  NOT NULL,
    [IDUser] varchar(50)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterFGStackings'
CREATE TABLE [dbo].[MasterFGStackings] (
    [IDFGStacking] int IDENTITY(1,1) NOT NULL,
    [Brand] varchar(10)  NOT NULL,
    [MaxStacking] int  NOT NULL,
    [MinStacking] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterEquipmentTypes'
CREATE TABLE [dbo].[MasterEquipmentTypes] (
    [IDEquipmentType] int IDENTITY(1,1) NOT NULL,
    [IDLocation] varchar(50)  NOT NULL,
    [Type] varchar(200)  NOT NULL,
    [Name] varchar(200)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterGuidelines'
CREATE TABLE [dbo].[MasterGuidelines] (
    [IDGuidline] int IDENTITY(1,1) NOT NULL,
    [PageName] varchar(200)  NOT NULL,
    [Description] varchar(max)  NOT NULL,
    [Keywords] varchar(200)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterHolidays'
CREATE TABLE [dbo].[MasterHolidays] (
    [HolidayDate] datetime  NOT NULL,
    [Description] varchar(max)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterIconFacilities'
CREATE TABLE [dbo].[MasterIconFacilities] (
    [IDIconFacility] int IDENTITY(1,1) NOT NULL,
    [Icon] varchar(max)  NOT NULL,
    [FieldName] varchar(200)  NOT NULL,
    [Status] bit  NOT NULL,
    [Remarks] varchar(500)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL
);
GO

-- Creating table 'MasterTransportRegions'
CREATE TABLE [dbo].[MasterTransportRegions] (
    [IDTransportRegion] int IDENTITY(1,1) NOT NULL,
    [GPNumber] varchar(100)  NOT NULL,
    [TransportRegion] varchar(200)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterVendors'
CREATE TABLE [dbo].[MasterVendors] (
    [IDVendor] varchar(50)  NOT NULL,
    [VendorName] nchar(10)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'NewsHighlights'
CREATE TABLE [dbo].[NewsHighlights] (
    [IDNewsHighlight] int IDENTITY(1,1) NOT NULL,
    [Title] varchar(200)  NOT NULL,
    [Image] varchar(max)  NOT NULL,
    [FileUpload] varchar(max)  NOT NULL,
    [IsPublished] bit  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'NotificationSystems'
CREATE TABLE [dbo].[NotificationSystems] (
    [IDNotification] int IDENTITY(1,1) NOT NULL,
    [IDUser] varchar(50)  NOT NULL,
    [PageName] varchar(200)  NOT NULL,
    [Description] varchar(500)  NOT NULL,
    [IsRead] bit  NOT NULL,
    [IsOpen] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL
);
GO

-- Creating table 'UserRolesSecurityViews'
CREATE TABLE [dbo].[UserRolesSecurityViews] (
    [IDUser] varchar(50)  NOT NULL,
    [FullName] varchar(500)  NOT NULL,
    [IDRole] int  NOT NULL,
    [rolename] varchar(100)  NOT NULL,
    [IDFunction] int  NOT NULL,
    [FunctionName] varchar(200)  NOT NULL,
    [Type] varchar(10)  NOT NULL,
    [ParentIDFunction] int  NULL
);
GO

-- Creating table 'KPIProductivities'
CREATE TABLE [dbo].[KPIProductivities] (
    [ProductivityNumber] varchar(16)  NOT NULL,
    [ProductivityTitle] varchar(128)  NOT NULL,
    [Initiator] varchar(50)  NOT NULL,
    [ContactPerson] varchar(128)  NOT NULL,
    [GMSTNumber] varchar(128)  NOT NULL,
    [Description] varchar(128)  NOT NULL,
    [Category] varchar(50)  NOT NULL,
    [MonthFrom] int  NOT NULL,
    [MonthTo] int  NOT NULL,
    [YearFrom] int  NOT NULL,
    [YearTo] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'KPIVehicleDeliveryProcesses'
CREATE TABLE [dbo].[KPIVehicleDeliveryProcesses] (
    [IDVehicleDeliveryProcess] int IDENTITY(1,1) NOT NULL,
    [Year] int  NOT NULL,
    [Month] int  NOT NULL,
    [LongTermVehicle] int  NOT NULL,
    [ShortTermVehicle] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'KPIInputSVViews'
CREATE TABLE [dbo].[KPIInputSVViews] (
    [Type] varchar(200)  NOT NULL,
    [Year] int  NOT NULL,
    [Jan] int  NULL,
    [Feb] int  NULL,
    [Mar] int  NULL,
    [Apr] int  NULL,
    [May] int  NULL,
    [Jun] int  NULL,
    [Jul] int  NULL,
    [Aug] int  NULL,
    [Sep] int  NULL,
    [Oct] int  NULL,
    [Nov] int  NULL,
    [Dec] int  NULL
);
GO

-- Creating table 'KPIInventoryAccuracies'
CREATE TABLE [dbo].[KPIInventoryAccuracies] (
    [IDInventoryAccuracy] int IDENTITY(1,1) NOT NULL,
    [IDLocation] varchar(50)  NOT NULL,
    [Month] int  NOT NULL,
    [Year] int  NOT NULL,
    [Accuracy] decimal(18,4)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'KPIDistributionCostSVs'
CREATE TABLE [dbo].[KPIDistributionCostSVs] (
    [IDDistributionCostSV] int IDENTITY(1,1) NOT NULL,
    [Type] varchar(200)  NOT NULL,
    [Year] int  NOT NULL,
    [Month] int  NOT NULL,
    [Value] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'KPIProductivityDetails'
CREATE TABLE [dbo].[KPIProductivityDetails] (
    [IDProductivityDetail] int IDENTITY(1,1) NOT NULL,
    [ProductivityNumber] varchar(16)  NOT NULL,
    [Status] varchar(10)  NOT NULL,
    [Month] int  NOT NULL,
    [Year] int  NOT NULL,
    [Value] decimal(19,4)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'KPIVehicleDeliveryProcessViews'
CREATE TABLE [dbo].[KPIVehicleDeliveryProcessViews] (
    [DeliveryProcess] varchar(18)  NOT NULL,
    [Year] int  NOT NULL,
    [Jan] int  NULL,
    [Feb] int  NULL,
    [Mar] int  NULL,
    [Apr] int  NULL,
    [May] int  NULL,
    [Jun] int  NULL,
    [Jul] int  NULL,
    [Aug] int  NULL,
    [Sep] int  NULL,
    [Oct] int  NULL,
    [Nov] int  NULL,
    [Dec] int  NULL
);
GO

-- Creating table 'GeneralContImproveSandBags'
CREATE TABLE [dbo].[GeneralContImproveSandBags] (
    [IDContImprovSandBag] int IDENTITY(1,1) NOT NULL,
    [IDLocation] varchar(50)  NOT NULL,
    [Year] int  NOT NULL,
    [Month] int  NOT NULL,
    [Department] varchar(100)  NOT NULL,
    [SubDepartment] varchar(100)  NOT NULL,
    [Title] varchar(50)  NOT NULL,
    [Status] varchar(20)  NOT NULL,
    [Owner] varchar(10)  NOT NULL,
    [EmployeeStatus] varchar(20)  NOT NULL,
    [IdeaSource] varchar(50)  NOT NULL,
    [ImplementationDate] datetime  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'GeneralContImprovSSes'
CREATE TABLE [dbo].[GeneralContImprovSSes] (
    [IDContImprovSS] int IDENTITY(1,1) NOT NULL,
    [Title] varchar(200)  NOT NULL,
    [Topic] varchar(50)  NULL,
    [OwnerIdea] varchar(50)  NULL,
    [ProposalDate] datetime  NULL,
    [ImplementDate] datetime  NULL,
    [CostSavingPerMonth] decimal(19,4)  NULL,
    [EndPoint] int  NULL,
    [Voucher] decimal(19,4)  NULL,
    [ABCDAward] bit  NULL,
    [SubmittedPeriod] int  NULL,
    [Regional] varchar(50)  NULL,
    [ProductivityScore] int  NULL,
    [QualityScore] int  NULL,
    [WorkingHourScore] int  NULL,
    [SafetyScore] int  NULL,
    [EnvirontmentScore] int  NULL,
    [ErgonomicScore] int  NULL,
    [UsabilityScore] int  NULL,
    [EffortScore] int  NULL,
    [ProActivenessScore] int  NULL,
    [CreativityScore] int  NULL,
    [IdeaSource] varchar(50)  NULL,
    [LocationModifiedDate] datetime  NULL,
    [CopyBy] varchar(50)  NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'MasterLocations'
CREATE TABLE [dbo].[MasterLocations] (
    [IDLocation] varchar(50)  NOT NULL,
    [LocationName] varchar(100)  NOT NULL,
    [ParentLocation] varchar(50)  NULL,
    [Type] varchar(50)  NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- Creating table 'KPIDistributionCostViews'
CREATE TABLE [dbo].[KPIDistributionCostViews] (
    [Year] int  NOT NULL,
    [Type] varchar(100)  NOT NULL,
    [Func] varchar(100)  NOT NULL,
    [Dept] varchar(100)  NOT NULL,
    [Category] varchar(100)  NOT NULL,
    [Account] varchar(100)  NOT NULL,
    [YTD_FY] decimal(38,2)  NULL,
    [Jan] decimal(18,2)  NULL,
    [Feb] decimal(18,2)  NULL,
    [Mar] decimal(18,2)  NULL,
    [Apr] decimal(18,2)  NULL,
    [May] decimal(18,2)  NULL,
    [Jun] decimal(18,2)  NULL,
    [Jul] decimal(18,2)  NULL,
    [Aug] decimal(18,2)  NULL,
    [Sep] decimal(18,2)  NULL,
    [Oct] decimal(18,2)  NULL,
    [Nov] decimal(18,2)  NULL,
    [Dec] decimal(18,2)  NULL
);
GO

-- Creating table 'KPIDistributionCosts'
CREATE TABLE [dbo].[KPIDistributionCosts] (
    [IDDistributionCost] int IDENTITY(1,1) NOT NULL,
    [Type] varchar(100)  NOT NULL,
    [Func] varchar(100)  NOT NULL,
    [Dept] varchar(100)  NOT NULL,
    [Category] varchar(100)  NOT NULL,
    [Account] varchar(100)  NOT NULL,
    [Year] int  NOT NULL,
    [Jan] decimal(18,2)  NULL,
    [Feb] decimal(18,2)  NULL,
    [Mar] decimal(18,2)  NULL,
    [Apr] decimal(18,2)  NULL,
    [May] decimal(18,2)  NULL,
    [Jun] decimal(18,2)  NULL,
    [Jul] decimal(18,2)  NULL,
    [Aug] decimal(18,2)  NULL,
    [Sep] decimal(18,2)  NULL,
    [Oct] decimal(18,2)  NULL,
    [Nov] decimal(18,2)  NULL,
    [Des] decimal(18,2)  NULL,
    [Value] decimal(18,2)  NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] varchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedBy] varchar(128)  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [Remarks] varchar(500)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [IDDynamicField] in table 'MasterDynamicFields'
ALTER TABLE [dbo].[MasterDynamicFields]
ADD CONSTRAINT [PK_MasterDynamicFields]
    PRIMARY KEY CLUSTERED ([IDDynamicField] ASC);
GO

-- Creating primary key on [IDFunction] in table 'MasterFunctions'
ALTER TABLE [dbo].[MasterFunctions]
ADD CONSTRAINT [PK_MasterFunctions]
    PRIMARY KEY CLUSTERED ([IDFunction] ASC);
GO

-- Creating primary key on [IDList] in table 'MasterLists'
ALTER TABLE [dbo].[MasterLists]
ADD CONSTRAINT [PK_MasterLists]
    PRIMARY KEY CLUSTERED ([IDList] ASC);
GO

-- Creating primary key on [id] in table 'tm_location_detail'
ALTER TABLE [dbo].[tm_location_detail]
ADD CONSTRAINT [PK_tm_location_detail]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [WarehouseID] in table 'GenBuildingFacilities'
ALTER TABLE [dbo].[GenBuildingFacilities]
ADD CONSTRAINT [PK_GenBuildingFacilities]
    PRIMARY KEY CLUSTERED ([WarehouseID] ASC);
GO

-- Creating primary key on [id_building] in table 'tm_location'
ALTER TABLE [dbo].[tm_location]
ADD CONSTRAINT [PK_tm_location]
    PRIMARY KEY CLUSTERED ([id_building] ASC);
GO

-- Creating primary key on [IDConfiguration] in table 'MasterConfigurations'
ALTER TABLE [dbo].[MasterConfigurations]
ADD CONSTRAINT [PK_MasterConfigurations]
    PRIMARY KEY CLUSTERED ([IDConfiguration] ASC);
GO

-- Creating primary key on [IDConfigMail] in table 'MasterConfigurationEmails'
ALTER TABLE [dbo].[MasterConfigurationEmails]
ADD CONSTRAINT [PK_MasterConfigurationEmails]
    PRIMARY KEY CLUSTERED ([IDConfigMail] ASC);
GO

-- Creating primary key on [IDLog] in table 'TransactionLogs'
ALTER TABLE [dbo].[TransactionLogs]
ADD CONSTRAINT [PK_TransactionLogs]
    PRIMARY KEY CLUSTERED ([IDLog] ASC);
GO

-- Creating primary key on [IDUser] in table 'MasterUsers'
ALTER TABLE [dbo].[MasterUsers]
ADD CONSTRAINT [PK_MasterUsers]
    PRIMARY KEY CLUSTERED ([IDUser] ASC);
GO

-- Creating primary key on [IDLoadFactorCFP] in table 'MasterLoadFactorCFPs'
ALTER TABLE [dbo].[MasterLoadFactorCFPs]
ADD CONSTRAINT [PK_MasterLoadFactorCFPs]
    PRIMARY KEY CLUSTERED ([IDLoadFactorCFP] ASC);
GO

-- Creating primary key on [IDConvertionPointSS] in table 'MasterConvertionPointSSes'
ALTER TABLE [dbo].[MasterConvertionPointSSes]
ADD CONSTRAINT [PK_MasterConvertionPointSSes]
    PRIMARY KEY CLUSTERED ([IDConvertionPointSS] ASC);
GO

-- Creating primary key on [FACode] in table 'MasterFABrands'
ALTER TABLE [dbo].[MasterFABrands]
ADD CONSTRAINT [PK_MasterFABrands]
    PRIMARY KEY CLUSTERED ([FACode] ASC);
GO

-- Creating primary key on [IDRole] in table 'MasterRoles'
ALTER TABLE [dbo].[MasterRoles]
ADD CONSTRAINT [PK_MasterRoles]
    PRIMARY KEY CLUSTERED ([IDRole] ASC);
GO

-- Creating primary key on [IDRolesFunctionMapping] in table 'MasterRolesFunctionMappings'
ALTER TABLE [dbo].[MasterRolesFunctionMappings]
ADD CONSTRAINT [PK_MasterRolesFunctionMappings]
    PRIMARY KEY CLUSTERED ([IDRolesFunctionMapping] ASC);
GO

-- Creating primary key on [IDUserLocationMapping] in table 'MasterUserLocationMappings'
ALTER TABLE [dbo].[MasterUserLocationMappings]
ADD CONSTRAINT [PK_MasterUserLocationMappings]
    PRIMARY KEY CLUSTERED ([IDUserLocationMapping] ASC);
GO

-- Creating primary key on [IDUserRoleMapping] in table 'MasterUserRoleMappings'
ALTER TABLE [dbo].[MasterUserRoleMappings]
ADD CONSTRAINT [PK_MasterUserRoleMappings]
    PRIMARY KEY CLUSTERED ([IDUserRoleMapping] ASC);
GO

-- Creating primary key on [IDFGStacking] in table 'MasterFGStackings'
ALTER TABLE [dbo].[MasterFGStackings]
ADD CONSTRAINT [PK_MasterFGStackings]
    PRIMARY KEY CLUSTERED ([IDFGStacking] ASC);
GO

-- Creating primary key on [IDEquipmentType] in table 'MasterEquipmentTypes'
ALTER TABLE [dbo].[MasterEquipmentTypes]
ADD CONSTRAINT [PK_MasterEquipmentTypes]
    PRIMARY KEY CLUSTERED ([IDEquipmentType] ASC);
GO

-- Creating primary key on [IDGuidline] in table 'MasterGuidelines'
ALTER TABLE [dbo].[MasterGuidelines]
ADD CONSTRAINT [PK_MasterGuidelines]
    PRIMARY KEY CLUSTERED ([IDGuidline] ASC);
GO

-- Creating primary key on [HolidayDate] in table 'MasterHolidays'
ALTER TABLE [dbo].[MasterHolidays]
ADD CONSTRAINT [PK_MasterHolidays]
    PRIMARY KEY CLUSTERED ([HolidayDate] ASC);
GO

-- Creating primary key on [IDIconFacility] in table 'MasterIconFacilities'
ALTER TABLE [dbo].[MasterIconFacilities]
ADD CONSTRAINT [PK_MasterIconFacilities]
    PRIMARY KEY CLUSTERED ([IDIconFacility] ASC);
GO

-- Creating primary key on [IDTransportRegion] in table 'MasterTransportRegions'
ALTER TABLE [dbo].[MasterTransportRegions]
ADD CONSTRAINT [PK_MasterTransportRegions]
    PRIMARY KEY CLUSTERED ([IDTransportRegion] ASC);
GO

-- Creating primary key on [IDVendor] in table 'MasterVendors'
ALTER TABLE [dbo].[MasterVendors]
ADD CONSTRAINT [PK_MasterVendors]
    PRIMARY KEY CLUSTERED ([IDVendor] ASC);
GO

-- Creating primary key on [IDNewsHighlight] in table 'NewsHighlights'
ALTER TABLE [dbo].[NewsHighlights]
ADD CONSTRAINT [PK_NewsHighlights]
    PRIMARY KEY CLUSTERED ([IDNewsHighlight] ASC);
GO

-- Creating primary key on [IDNotification] in table 'NotificationSystems'
ALTER TABLE [dbo].[NotificationSystems]
ADD CONSTRAINT [PK_NotificationSystems]
    PRIMARY KEY CLUSTERED ([IDNotification] ASC);
GO

-- Creating primary key on [IDUser], [FullName], [IDRole], [rolename], [IDFunction], [FunctionName], [Type] in table 'UserRolesSecurityViews'
ALTER TABLE [dbo].[UserRolesSecurityViews]
ADD CONSTRAINT [PK_UserRolesSecurityViews]
    PRIMARY KEY CLUSTERED ([IDUser], [FullName], [IDRole], [rolename], [IDFunction], [FunctionName], [Type] ASC);
GO

-- Creating primary key on [ProductivityNumber] in table 'KPIProductivities'
ALTER TABLE [dbo].[KPIProductivities]
ADD CONSTRAINT [PK_KPIProductivities]
    PRIMARY KEY CLUSTERED ([ProductivityNumber] ASC);
GO

-- Creating primary key on [IDVehicleDeliveryProcess] in table 'KPIVehicleDeliveryProcesses'
ALTER TABLE [dbo].[KPIVehicleDeliveryProcesses]
ADD CONSTRAINT [PK_KPIVehicleDeliveryProcesses]
    PRIMARY KEY CLUSTERED ([IDVehicleDeliveryProcess] ASC);
GO

-- Creating primary key on [Type], [Year] in table 'KPIInputSVViews'
ALTER TABLE [dbo].[KPIInputSVViews]
ADD CONSTRAINT [PK_KPIInputSVViews]
    PRIMARY KEY CLUSTERED ([Type], [Year] ASC);
GO

-- Creating primary key on [IDInventoryAccuracy] in table 'KPIInventoryAccuracies'
ALTER TABLE [dbo].[KPIInventoryAccuracies]
ADD CONSTRAINT [PK_KPIInventoryAccuracies]
    PRIMARY KEY CLUSTERED ([IDInventoryAccuracy] ASC);
GO

-- Creating primary key on [IDDistributionCostSV] in table 'KPIDistributionCostSVs'
ALTER TABLE [dbo].[KPIDistributionCostSVs]
ADD CONSTRAINT [PK_KPIDistributionCostSVs]
    PRIMARY KEY CLUSTERED ([IDDistributionCostSV] ASC);
GO

-- Creating primary key on [IDProductivityDetail] in table 'KPIProductivityDetails'
ALTER TABLE [dbo].[KPIProductivityDetails]
ADD CONSTRAINT [PK_KPIProductivityDetails]
    PRIMARY KEY CLUSTERED ([IDProductivityDetail] ASC);
GO

-- Creating primary key on [DeliveryProcess], [Year] in table 'KPIVehicleDeliveryProcessViews'
ALTER TABLE [dbo].[KPIVehicleDeliveryProcessViews]
ADD CONSTRAINT [PK_KPIVehicleDeliveryProcessViews]
    PRIMARY KEY CLUSTERED ([DeliveryProcess], [Year] ASC);
GO

-- Creating primary key on [IDContImprovSandBag] in table 'GeneralContImproveSandBags'
ALTER TABLE [dbo].[GeneralContImproveSandBags]
ADD CONSTRAINT [PK_GeneralContImproveSandBags]
    PRIMARY KEY CLUSTERED ([IDContImprovSandBag] ASC);
GO

-- Creating primary key on [IDContImprovSS] in table 'GeneralContImprovSSes'
ALTER TABLE [dbo].[GeneralContImprovSSes]
ADD CONSTRAINT [PK_GeneralContImprovSSes]
    PRIMARY KEY CLUSTERED ([IDContImprovSS] ASC);
GO

-- Creating primary key on [IDLocation] in table 'MasterLocations'
ALTER TABLE [dbo].[MasterLocations]
ADD CONSTRAINT [PK_MasterLocations]
    PRIMARY KEY CLUSTERED ([IDLocation] ASC);
GO

-- Creating primary key on [Year], [Type], [Func], [Dept], [Category], [Account] in table 'KPIDistributionCostViews'
ALTER TABLE [dbo].[KPIDistributionCostViews]
ADD CONSTRAINT [PK_KPIDistributionCostViews]
    PRIMARY KEY CLUSTERED ([Year], [Type], [Func], [Dept], [Category], [Account] ASC);
GO

-- Creating primary key on [IDDistributionCost] in table 'KPIDistributionCosts'
ALTER TABLE [dbo].[KPIDistributionCosts]
ADD CONSTRAINT [PK_KPIDistributionCosts]
    PRIMARY KEY CLUSTERED ([IDDistributionCost] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [IDFunction] in table 'TransactionLogs'
ALTER TABLE [dbo].[TransactionLogs]
ADD CONSTRAINT [FK_TransactionLog_MasterFunction]
    FOREIGN KEY ([IDFunction])
    REFERENCES [dbo].[MasterFunctions]
        ([IDFunction])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TransactionLog_MasterFunction'
CREATE INDEX [IX_FK_TransactionLog_MasterFunction]
ON [dbo].[TransactionLogs]
    ([IDFunction]);
GO

-- Creating foreign key on [IDUser] in table 'TransactionLogs'
ALTER TABLE [dbo].[TransactionLogs]
ADD CONSTRAINT [FK_TransactionLog_MasterUser]
    FOREIGN KEY ([IDUser])
    REFERENCES [dbo].[MasterUsers]
        ([IDUser])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TransactionLog_MasterUser'
CREATE INDEX [IX_FK_TransactionLog_MasterUser]
ON [dbo].[TransactionLogs]
    ([IDUser]);
GO

-- Creating foreign key on [IDFunction] in table 'MasterRolesFunctionMappings'
ALTER TABLE [dbo].[MasterRolesFunctionMappings]
ADD CONSTRAINT [FK_MasterRolesFunctionMapping_MasterFunction]
    FOREIGN KEY ([IDFunction])
    REFERENCES [dbo].[MasterFunctions]
        ([IDFunction])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterRolesFunctionMapping_MasterFunction'
CREATE INDEX [IX_FK_MasterRolesFunctionMapping_MasterFunction]
ON [dbo].[MasterRolesFunctionMappings]
    ([IDFunction]);
GO

-- Creating foreign key on [IDRole] in table 'MasterRolesFunctionMappings'
ALTER TABLE [dbo].[MasterRolesFunctionMappings]
ADD CONSTRAINT [FK_MasterRolesFunctionMapping_MasterRole]
    FOREIGN KEY ([IDRole])
    REFERENCES [dbo].[MasterRoles]
        ([IDRole])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterRolesFunctionMapping_MasterRole'
CREATE INDEX [IX_FK_MasterRolesFunctionMapping_MasterRole]
ON [dbo].[MasterRolesFunctionMappings]
    ([IDRole]);
GO

-- Creating foreign key on [IDRole] in table 'MasterUserRoleMappings'
ALTER TABLE [dbo].[MasterUserRoleMappings]
ADD CONSTRAINT [FK_MasterUserRoleMapping_MasterRole]
    FOREIGN KEY ([IDRole])
    REFERENCES [dbo].[MasterRoles]
        ([IDRole])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterUserRoleMapping_MasterRole'
CREATE INDEX [IX_FK_MasterUserRoleMapping_MasterRole]
ON [dbo].[MasterUserRoleMappings]
    ([IDRole]);
GO

-- Creating foreign key on [IDUser] in table 'MasterUserLocationMappings'
ALTER TABLE [dbo].[MasterUserLocationMappings]
ADD CONSTRAINT [FK_MasterUserLocationMapping_MasterUser]
    FOREIGN KEY ([IDUser])
    REFERENCES [dbo].[MasterUsers]
        ([IDUser])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterUserLocationMapping_MasterUser'
CREATE INDEX [IX_FK_MasterUserLocationMapping_MasterUser]
ON [dbo].[MasterUserLocationMappings]
    ([IDUser]);
GO

-- Creating foreign key on [IDUser] in table 'MasterUserRoleMappings'
ALTER TABLE [dbo].[MasterUserRoleMappings]
ADD CONSTRAINT [FK_MasterUserRoleMapping_MasterUser]
    FOREIGN KEY ([IDUser])
    REFERENCES [dbo].[MasterUsers]
        ([IDUser])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterUserRoleMapping_MasterUser'
CREATE INDEX [IX_FK_MasterUserRoleMapping_MasterUser]
ON [dbo].[MasterUserRoleMappings]
    ([IDUser]);
GO

-- Creating foreign key on [IDUser] in table 'NotificationSystems'
ALTER TABLE [dbo].[NotificationSystems]
ADD CONSTRAINT [FK_NotificationSystem_MasterUser]
    FOREIGN KEY ([IDUser])
    REFERENCES [dbo].[MasterUsers]
        ([IDUser])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NotificationSystem_MasterUser'
CREATE INDEX [IX_FK_NotificationSystem_MasterUser]
ON [dbo].[NotificationSystems]
    ([IDUser]);
GO

-- Creating foreign key on [ProductivityNumber] in table 'KPIProductivityDetails'
ALTER TABLE [dbo].[KPIProductivityDetails]
ADD CONSTRAINT [FK_KPIProductivityDetail_KPIProductivity1]
    FOREIGN KEY ([ProductivityNumber])
    REFERENCES [dbo].[KPIProductivities]
        ([ProductivityNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KPIProductivityDetail_KPIProductivity1'
CREATE INDEX [IX_FK_KPIProductivityDetail_KPIProductivity1]
ON [dbo].[KPIProductivityDetails]
    ([ProductivityNumber]);
GO

-- Creating foreign key on [IDLocation] in table 'GeneralContImproveSandBags'
ALTER TABLE [dbo].[GeneralContImproveSandBags]
ADD CONSTRAINT [FK_GeneralContImproveSandBag_MasterLocation]
    FOREIGN KEY ([IDLocation])
    REFERENCES [dbo].[MasterLocations]
        ([IDLocation])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralContImproveSandBag_MasterLocation'
CREATE INDEX [IX_FK_GeneralContImproveSandBag_MasterLocation]
ON [dbo].[GeneralContImproveSandBags]
    ([IDLocation]);
GO

-- Creating foreign key on [IDLocation] in table 'KPIInventoryAccuracies'
ALTER TABLE [dbo].[KPIInventoryAccuracies]
ADD CONSTRAINT [FK_KPIInventoryAccuracy_MasterLocation]
    FOREIGN KEY ([IDLocation])
    REFERENCES [dbo].[MasterLocations]
        ([IDLocation])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KPIInventoryAccuracy_MasterLocation'
CREATE INDEX [IX_FK_KPIInventoryAccuracy_MasterLocation]
ON [dbo].[KPIInventoryAccuracies]
    ([IDLocation]);
GO

-- Creating foreign key on [IDLocation] in table 'MasterEquipmentTypes'
ALTER TABLE [dbo].[MasterEquipmentTypes]
ADD CONSTRAINT [FK_MasterEquipmentType_MasterLocation]
    FOREIGN KEY ([IDLocation])
    REFERENCES [dbo].[MasterLocations]
        ([IDLocation])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterEquipmentType_MasterLocation'
CREATE INDEX [IX_FK_MasterEquipmentType_MasterLocation]
ON [dbo].[MasterEquipmentTypes]
    ([IDLocation]);
GO

-- Creating foreign key on [ParentLocation] in table 'MasterLocations'
ALTER TABLE [dbo].[MasterLocations]
ADD CONSTRAINT [FK_Location_ParentLocation]
    FOREIGN KEY ([ParentLocation])
    REFERENCES [dbo].[MasterLocations]
        ([IDLocation])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Location_ParentLocation'
CREATE INDEX [IX_FK_Location_ParentLocation]
ON [dbo].[MasterLocations]
    ([ParentLocation]);
GO

-- Creating foreign key on [IDLocation] in table 'MasterUserLocationMappings'
ALTER TABLE [dbo].[MasterUserLocationMappings]
ADD CONSTRAINT [FK_MasterUserLocationMapping_MasterLocation]
    FOREIGN KEY ([IDLocation])
    REFERENCES [dbo].[MasterLocations]
        ([IDLocation])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterUserLocationMapping_MasterLocation'
CREATE INDEX [IX_FK_MasterUserLocationMapping_MasterLocation]
ON [dbo].[MasterUserLocationMappings]
    ([IDLocation]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------