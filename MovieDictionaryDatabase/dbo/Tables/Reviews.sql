CREATE TABLE [dbo].[Reviews] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [MovieId]   NVARCHAR (20)  NOT NULL,
    [UserId]    NVARCHAR (128) NOT NULL,
    [Rating]    INT            NOT NULL,
    [DateAdded] DATETIME           NOT NULL,
    [Title]     NVARCHAR (500) NOT NULL,
    [Content]   NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Reviews_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Reviews_Movies] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([Id])
);



