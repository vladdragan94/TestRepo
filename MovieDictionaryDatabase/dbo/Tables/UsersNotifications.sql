CREATE TABLE [dbo].[UsersNotifications] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [UserId]             NVARCHAR (128) NOT NULL,
    [Seen]               BIT            CONSTRAINT [DF_UsersNotifications_Seen] DEFAULT ((0)) NOT NULL,
    [NotificationTypeId] INT            NOT NULL,
    [DateAdded]          DATETIME       NOT NULL,
    [MovieId]            NVARCHAR (20)  NULL,
    [BadgeId]            INT            NULL,
    [PostId]             INT            NULL,
    CONSTRAINT [PK_UsersNotifications] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsersNotifications_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_UsersNotifications_Badges] FOREIGN KEY ([BadgeId]) REFERENCES [dbo].[Badges] ([Id]),
    CONSTRAINT [FK_UsersNotifications_ForumPosts] FOREIGN KEY ([PostId]) REFERENCES [dbo].[ForumPosts] ([Id]),
    CONSTRAINT [FK_UsersNotifications_Movies] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([Id]),
    CONSTRAINT [FK_UsersNotifications_NotificationTypes] FOREIGN KEY ([NotificationTypeId]) REFERENCES [dbo].[NotificationTypes] ([Id])
);







