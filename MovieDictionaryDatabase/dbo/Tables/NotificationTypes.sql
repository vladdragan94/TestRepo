CREATE TABLE [dbo].[NotificationTypes] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (500) NULL,
    CONSTRAINT [PK_NotificationTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

