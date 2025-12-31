-- IdentityServer DB
IF DB_ID('IdentityServerDb') IS NULL
    CREATE DATABASE IdentityServerDb;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'IdentityServer')
    CREATE LOGIN IdentityServer WITH PASSWORD = 'StrongIdentityPwd123!';
GO

USE IdentityServerDb;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'IdentityServer')
    CREATE USER IdentityServer FOR LOGIN IdentityServer;
GO

GRANT CREATE TABLE TO IdentityServer;
GRANT ALTER ON SCHEMA::dbo TO IdentityServer;
GRANT SELECT, INSERT, UPDATE, DELETE, REFERENCES ON SCHEMA::dbo TO IdentityServer;
GO


-- API DB
USE master;
GO

IF DB_ID('SportSynchroApiDb') IS NULL
    CREATE DATABASE SportSynchroApiDb;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'ApiUser')
    CREATE LOGIN ApiUser WITH PASSWORD = 'StrongApiPwd123!';
GO

USE SportSynchroApiDb;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'ApiUser')
    CREATE USER ApiUser FOR LOGIN ApiUser;
GO

GRANT CREATE TABLE TO ApiUser;
GRANT ALTER ON SCHEMA::dbo TO ApiUser;
GRANT SELECT, INSERT, UPDATE, DELETE, REFERENCES ON SCHEMA::dbo TO ApiUser;
GO
