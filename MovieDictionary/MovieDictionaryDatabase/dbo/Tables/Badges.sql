CREATE TABLE [dbo].[Badges] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (500) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Badges] PRIMARY KEY CLUSTERED ([Id] ASC)
);



