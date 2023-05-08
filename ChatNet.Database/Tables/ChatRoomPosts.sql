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