CREATE TABLE [dbo].[LatestMovies] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [MovieId]   NVARCHAR (20) UNIQUE NOT NULL,
    [DateAdded] DATE          NOT NULL,
    CONSTRAINT [PK_LatestMovies] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LatestMovies_Movies] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([Id])
);

