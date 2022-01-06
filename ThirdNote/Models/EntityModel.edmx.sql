
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/05/2021 14:37:20
-- Generated from EDMX file: C:\Users\Sina\Documents\ThirdNote\ThirdNote\Models\EntityModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ThirdNotesDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Notes'
CREATE TABLE [dbo].[Notes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NULL,
    [Text] nvarchar(max)  NULL,
    [CreatedDate] datetime  NOT NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Label] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Metas'
CREATE TABLE [dbo].[Metas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Hidden] bit  NOT NULL,
    [Markdown] bit  NOT NULL,
    [Pin] bit  NOT NULL,
    [Actionable] bit  NOT NULL,
    [Note_Id] int  NOT NULL
);
GO

-- Creating table 'Rates'
CREATE TABLE [dbo].[Rates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Score] int  NOT NULL,
    [Date] datetime  NOT NULL,
    [Note_Id] int  NOT NULL
);
GO

-- Creating table 'NoteTag'
CREATE TABLE [dbo].[NoteTag] (
    [Tags_Id] int  NOT NULL,
    [Notes_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Notes'
ALTER TABLE [dbo].[Notes]
ADD CONSTRAINT [PK_Notes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Metas'
ALTER TABLE [dbo].[Metas]
ADD CONSTRAINT [PK_Metas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Rates'
ALTER TABLE [dbo].[Rates]
ADD CONSTRAINT [PK_Rates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Tags_Id], [Notes_Id] in table 'NoteTag'
ALTER TABLE [dbo].[NoteTag]
ADD CONSTRAINT [PK_NoteTag]
    PRIMARY KEY CLUSTERED ([Tags_Id], [Notes_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Tags_Id] in table 'NoteTag'
ALTER TABLE [dbo].[NoteTag]
ADD CONSTRAINT [FK_TagNote_Tag]
    FOREIGN KEY ([Tags_Id])
    REFERENCES [dbo].[Tags]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Notes_Id] in table 'NoteTag'
ALTER TABLE [dbo].[NoteTag]
ADD CONSTRAINT [FK_TagNote_Note]
    FOREIGN KEY ([Notes_Id])
    REFERENCES [dbo].[Notes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TagNote_Note'
CREATE INDEX [IX_FK_TagNote_Note]
ON [dbo].[NoteTag]
    ([Notes_Id]);
GO

-- Creating foreign key on [Note_Id] in table 'Rates'
ALTER TABLE [dbo].[Rates]
ADD CONSTRAINT [FK_NoteRate]
    FOREIGN KEY ([Note_Id])
    REFERENCES [dbo].[Notes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NoteRate'
CREATE INDEX [IX_FK_NoteRate]
ON [dbo].[Rates]
    ([Note_Id]);
GO

-- Creating foreign key on [Note_Id] in table 'Metas'
ALTER TABLE [dbo].[Metas]
ADD CONSTRAINT [FK_NoteMeta]
    FOREIGN KEY ([Note_Id])
    REFERENCES [dbo].[Notes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NoteMeta'
CREATE INDEX [IX_FK_NoteMeta]
ON [dbo].[Metas]
    ([Note_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------