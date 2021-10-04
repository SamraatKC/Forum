USE [ForumDB]
GO

/****** Object: Table [dbo].[MainTopics] Script Date: 10/3/2021 16:41:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MainTopics] (
    [MainTopicId]     INT            IDENTITY (1, 1) NOT NULL primary key,
    [ThemeIdFK]       INT            NOT NULL,
    [ParentIdFK]      INT            NOT NULL,
    [ReferenceLink]   NVARCHAR (MAX) NULL,
    [Title]           NVARCHAR (MAX) NULL,
    [Description]     NVARCHAR (MAX) NULL,
    [TopicIcon]       NVARCHAR (MAX) NULL,
    [DisplayOrder]    INT            NOT NULL,
    [CreatedDate]     DATETIME2 (7)  NULL,
    [CreatedBy]       NVARCHAR (MAX) NULL,
    [LastUpdatedDate] DATETIME2 (7)  NULL,
    [LastUpdatedBy]   NVARCHAR (MAX) NULL,
    [Status]          NVARCHAR (MAX) NULL,
    [Moderator]       NVARCHAR (MAX) NULL
);
GO

/****** Object: Table [dbo].[MainTopicPosts] Script Date: 10/3/2021 16:40:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MainTopicPosts] (
    [MainTopicPostId] INT            IDENTITY (1, 1) NOT NULL,
    [MainTopicsIdFK]  INT            NOT NULL,
    [Title]           NVARCHAR (MAX) NULL,
    [Description]     NVARCHAR (MAX) NULL,
    [TopicIcon]       NVARCHAR (MAX) NULL,
    [CreatedDate]     DATETIME2 (7)  NULL,
    [CreatedBy]       NVARCHAR (MAX) NULL,
    [LastUpdatedDate] DATETIME2 (7)  NULL,
    [LastUpdatedBy]   NVARCHAR (MAX) NULL,
    [Status]          NVARCHAR (MAX) NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_MainTopicPosts_MainTopicsIdFK]
    ON [dbo].[MainTopicPosts]([MainTopicsIdFK] ASC);


GO
ALTER TABLE [dbo].[MainTopicPosts]
    ADD CONSTRAINT [PK_MainTopicPosts] PRIMARY KEY CLUSTERED ([MainTopicPostId] ASC);


GO
ALTER TABLE [dbo].[MainTopicPosts]
    ADD CONSTRAINT [FK_MainTopicPosts_MainTopics_MainTopicsIdFK] FOREIGN KEY ([MainTopicsIdFK]) REFERENCES [dbo].[MainTopics] ([MainTopicId]) ON DELETE CASCADE;





