CREATE TABLE [dbo].[UsersWatchlists] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [UserId]    NVARCHAR (128) NOT NULL,
    [MovieId]   NVARCHAR (20)  NOT NULL,
    [DateAdded] DATETIME       CONSTRAINT [DF_UsersWatchlists_DateAdded] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_UsersWatchlists] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsersWatchlists_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_UsersWatchlists_Movies] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([Id])
);



