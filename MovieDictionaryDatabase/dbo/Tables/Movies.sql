CREATE TABLE [dbo].[Movies] (
    [Id]       NVARCHAR (20)   NOT NULL,
    [Title]    NVARCHAR (500)  NOT NULL,
    [Year]     INT             NOT NULL,
    [Runtime]  NVARCHAR (500)  NOT NULL,
    [Released] DATE            NULL,
    [Genre]    NVARCHAR (500)  NOT NULL,
    [Rating]   DECIMAL (19, 8) NULL,
    [Poster]   NVARCHAR (500)  NULL,
    [Plot]     NVARCHAR (MAX)  NULL,
    [Director] NVARCHAR (500)  NULL,
    [Writer]   NVARCHAR (500)  NULL,
    [Actors]   NVARCHAR (500)  NULL,
    [Awards]   NVARCHAR (500)  NULL,
    [Trailer]  NVARCHAR (200)  NULL,
    CONSTRAINT [PK_Movies] PRIMARY KEY CLUSTERED ([Id] ASC)
);











