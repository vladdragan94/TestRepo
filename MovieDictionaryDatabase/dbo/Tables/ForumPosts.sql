CREATE TABLE [dbo].[ForumPosts] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (128) NOT NULL,
    [DateAdded]  DATETIME       NOT NULL,
    [Title]      NVARCHAR (MAX) NOT NULL,
    [Content]    NVARCHAR (MAX) NOT NULL,
    [PostId]     INT            NULL,
    [IsQuestion] BIT            NOT NULL,
    [IsAnswer]   BIT            CONSTRAINT [DF_ForumPosts_IsAnswer] DEFAULT ((0)) NOT NULL,
    [Votes]      INT            CONSTRAINT [DF_ForumPosts_Votes] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ForumPosts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ForumPosts_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_ForumPosts_ForumPosts] FOREIGN KEY ([PostId]) REFERENCES [dbo].[ForumPosts] ([Id])
);









