CREATE TABLE [dbo].[UsersRecommendations] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [SenderId]   NVARCHAR (128) NOT NULL,
    [ReceiverId] NVARCHAR (128) NOT NULL,
    [MovieId]    NVARCHAR (20)  NOT NULL,
    [Liked]      BIT            CONSTRAINT [DF_UsersRecommendations_Liked] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_UsersRecommendations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsersRecommendations_AspNetUsers] FOREIGN KEY ([SenderId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_UsersRecommendations_AspNetUsers1] FOREIGN KEY ([ReceiverId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_UsersRecommendations_Movies] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([Id])
);



