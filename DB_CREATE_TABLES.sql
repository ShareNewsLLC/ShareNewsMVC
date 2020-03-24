CREATE TABLE [dbo].[Admins] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Email]      NVARCHAR (450) NULL,
    [Password]   NVARCHAR (MAX) NULL,
    [DateJoined] DATETIME       NOT NULL,
    [FullName]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Admins] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Email]
    ON [dbo].[Admins]([Email] ASC);

GO

CREATE TABLE [dbo].[Authors] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Email]            NVARCHAR (450) NULL,
    [Password]         NVARCHAR (MAX) NULL,
    [DateJoined]       DATETIME       NOT NULL,
    [isAnonymous]      BIT            NOT NULL,
    [isVerified]       BIT            NOT NULL,
    [isEmailConfirmed] BIT            NOT NULL,
    [FullName]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Authors] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Email]
    ON [dbo].[Authors]([Email] ASC);

GO

CREATE TABLE [dbo].[Avatars] (
    [Id]       INT             IDENTITY (1, 1) NOT NULL,
    [AuthorId] INT             NOT NULL,
    [Source]   VARBINARY (MAX) NULL,
    CONSTRAINT [PK_dbo.Avatars] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Categories] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (450) NULL,
    [isActive] BIT            NOT NULL,
    CONSTRAINT [PK_dbo.Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Name]
    ON [dbo].[Categories]([Name] ASC);

GO

CREATE TABLE [dbo].[EmailCodes] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [AuthorId]           INT            NOT NULL,
    [Email]              NVARCHAR (450) NULL,
    [ConfirmationNumber] INT            NOT NULL,
    [isExpired]          BIT            NOT NULL,
    CONSTRAINT [PK_dbo.EmailCodes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Pictures] (
    [Id]     INT             IDENTITY (1, 1) NOT NULL,
    [PostId] INT             NOT NULL,
    [Source] VARBINARY (MAX) NULL,
    CONSTRAINT [PK_dbo.Pictures] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Posts] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [CategoryId]   INT            NOT NULL,
    [Title]        NVARCHAR (MAX) NULL,
    [AuthorId]     INT            NOT NULL,
    [DateCreated]  DATETIME       NOT NULL,
    [LastModified] DATETIME       NOT NULL,
    [Article]      NVARCHAR (MAX) NULL,
    [isActive]     BIT            NOT NULL,
    CONSTRAINT [PK_dbo.Posts] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO