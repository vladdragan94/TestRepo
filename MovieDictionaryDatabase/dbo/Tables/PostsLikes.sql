CREATE TABLE [dbo].[PostsLikes] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [PostId] INT            NOT NULL,
    [UserId] NVARCHAR (128) NOT NULL,
    [Liked]  BIT            NOT NULL,
    CONSTRAINT [PK_PostsLikes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PostsLikes_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_PostsLikes_ForumPosts] FOREIGN KEY ([PostId]) REFERENCES [dbo].[ForumPosts] ([Id])
);

