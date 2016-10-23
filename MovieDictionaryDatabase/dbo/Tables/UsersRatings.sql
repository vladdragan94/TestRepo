CREATE TABLE [dbo].[UsersRatings] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [UserId]    NVARCHAR (128) NOT NULL,
    [MovieId]   NVARCHAR (20)  NOT NULL,
    [Rating]    INT            NOT NULL,
    [DateAdded] DATETIME       CONSTRAINT [DF_UsersRatings_DateAdded] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_UsersRatings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsersRatings_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_UsersRatings_Movies] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([Id])
);



