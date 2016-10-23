CREATE TABLE [dbo].[Log] (
    [Id]        INT             IDENTITY (1, 1) NOT NULL,
    [Date]      DATETIME        NOT NULL,
    [Thread]    NVARCHAR (255)  NOT NULL,
    [Level]     NVARCHAR (50)   NOT NULL,
    [Logger]    NVARCHAR (255)  NOT NULL,
    [Message]   NVARCHAR (4000) NOT NULL,
    [Exception] NVARCHAR (4000) NULL
);



