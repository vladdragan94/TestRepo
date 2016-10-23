CREATE TABLE [dbo].[ApplicationConfigurations] (
    [Id]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR (500) NOT NULL,
    [Value] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Configurations] PRIMARY KEY CLUSTERED ([Id] ASC)
);

