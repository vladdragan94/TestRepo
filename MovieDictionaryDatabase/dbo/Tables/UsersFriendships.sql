CREATE TABLE [dbo].[UsersFriendships] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [FirstUser]  NVARCHAR (128) NOT NULL,
    [SecondUser] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_UsersFriendships] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsersFriendships_AspNetUsers] FOREIGN KEY ([FirstUser]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_UsersFriendships_AspNetUsers1] FOREIGN KEY ([SecondUser]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

