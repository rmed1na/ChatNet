/* Database creation and initial setup */
USE master

IF NOT EXISTS(SELECT * FROM sys.databases WHERE [name] = 'ChatNet')
BEGIN
	CREATE DATABASE [ChatNet]

	EXEC('
		IF NOT EXISTS(SELECT * FROM sys.syslogins WHERE [name] = ''cn_usr'')
		BEGIN
			CREATE LOGIN [cn_usr] WITH PASSWORD = ''123456789''
		END
	')

	EXEC('
		USE [ChatNet]

		IF NOT EXISTS(SELECT * FROM sys.database_principals WHERE [name] = N''cn_usr'')
		BEGIN
			CREATE USER [cn_usr] FOR LOGIN [cn_usr]
			EXEC sp_addrolemember N''db_owner'', N''cn_usr''
		END

		IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''Users'')
		BEGIN
			CREATE TABLE [Users]
			(
				[UserId] INT IDENTITY NOT NULL,
				[CreatedDate] DATETIME NOT NULL,
				[UpdatedDate] DATETIME NULL,
				[Username] NVARCHAR(64) NOT NULL,
				[FirstName] NVARCHAR(32) NULL,
				[LastName] NVARCHAR(32) NULL,
				[Password] NVARCHAR(1024) NOT NULL,
				CONSTRAINT [PK_Users] PRIMARY KEY (UserId)
			)
			INSERT INTO [Users] ([CreatedDate], [Username], [Password]) VALUES (GETDATE(), ''admin'', ''$2b$10$tQz8Tz0r9s/ozQv1jpGrouqRcJDfy5L1yUXDCV9OMQLOzsghm.iom'')
		END

		IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''ChatRooms'')
		BEGIN
			CREATE TABLE [ChatRooms]
			(
				[ChatRoomId] INT IDENTITY NOT NULL,
				[CreatedDate] DATETIME NOT NULL,
				[UpdatedDate] DATETIME NULL,
				[Name] NVARCHAR(64) NOT NULL,
				[StatusCode] INT NOT NULL,
				CONSTRAINT [PK_ChatRooms] PRIMARY KEY (ChatRoomId)
			)
			INSERT INTO ChatRooms([CreatedDate], [Name], [StatusCode]) VALUES (GETDATE(), ''General'', 1)
		END

		IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''ChatRoomPosts'')
		BEGIN
			CREATE TABLE [ChatRoomPosts]
			(
				[ChatRoomPostId] INT IDENTITY NOT NULL,
				[CreatedDate] DATETIME NOT NULL,
				[UpdatedDate] DATETIME NULL,
				[ChatRoomId] INT NOT NULL,
				[UserId] INT NOT NULL,
				[Message] NVARCHAR(2048) NOT NULL,
				CONSTRAINT [PK_ChatRoomPosts] PRIMARY KEY (ChatRoomPostId),
				CONSTRAINT [FK_ChatRoomPosts_ChatRoomId] FOREIGN KEY (ChatRoomId) REFERENCES ChatRooms(ChatRoomId),
				CONSTRAINT [PK_ChatRoomPosts_UserId] FOREIGN KEY (UserId) REFERENCES Users(UserId)
			)
		END
		')
END