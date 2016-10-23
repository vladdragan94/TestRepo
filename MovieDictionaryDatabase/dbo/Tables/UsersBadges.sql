CREATE TABLE [dbo].[UsersBadges] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [UserId]  NVARCHAR (128) NOT NULL,
    [BadgeId] INT            NOT NULL,
    CONSTRAINT [PK_UsersBadges] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsersBadges_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_UsersBadges_Badges] FOREIGN KEY ([BadgeId]) REFERENCES [dbo].[Badges] ([Id])
);

