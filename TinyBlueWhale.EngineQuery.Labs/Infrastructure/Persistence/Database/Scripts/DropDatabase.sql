USE [master];
GO
IF DB_ID(N'TinyBlueWhaleEngineQueryLabs') IS NOT NULL
BEGIN
    ALTER DATABASE [TinyBlueWhaleEngineQueryLabs] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TinyBlueWhaleEngineQueryLabs];
END;
GO
