CREATE TABLE [dbo].[ReviewsLikes] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [ReviewId] INT            NOT NULL,
    [UserId]   NVARCHAR (128) NOT NULL,
    [Liked]    BIT            NOT NULL,
    CONSTRAINT [PK_ReviewsLikes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReviewsLikes_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_ReviewsLikes_Reviews] FOREIGN KEY ([ReviewId]) REFERENCES [dbo].[Reviews] ([Id])
);

