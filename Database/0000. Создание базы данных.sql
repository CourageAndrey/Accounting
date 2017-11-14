USE [master]
GO
/****** Object:  Database [ComfortIsland]    Script Date: 11/08/2017 16:10:06 ******/
CREATE DATABASE [ComfortIsland] ON  PRIMARY 
( NAME = N'ComfortIsland', FILENAME = N'%PATH%\ComfortIsland.mdf' , SIZE = 6144KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ComfortIsland_log', FILENAME = N'%PATH%\ComfortIsland_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ComfortIsland] SET COMPATIBILITY_LEVEL = 100
GO
ALTER DATABASE [ComfortIsland] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [ComfortIsland] SET ANSI_NULLS OFF
GO
ALTER DATABASE [ComfortIsland] SET ANSI_PADDING OFF
GO
ALTER DATABASE [ComfortIsland] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [ComfortIsland] SET ARITHABORT OFF
GO
ALTER DATABASE [ComfortIsland] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [ComfortIsland] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [ComfortIsland] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [ComfortIsland] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [ComfortIsland] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [ComfortIsland] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [ComfortIsland] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [ComfortIsland] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [ComfortIsland] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [ComfortIsland] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [ComfortIsland] SET  DISABLE_BROKER
GO
ALTER DATABASE [ComfortIsland] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [ComfortIsland] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [ComfortIsland] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [ComfortIsland] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [ComfortIsland] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [ComfortIsland] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [ComfortIsland] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [ComfortIsland] SET  READ_WRITE
GO
ALTER DATABASE [ComfortIsland] SET RECOVERY FULL
GO
ALTER DATABASE [ComfortIsland] SET  MULTI_USER
GO
ALTER DATABASE [ComfortIsland] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [ComfortIsland] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'ComfortIsland', N'ON'
GO
USE [ComfortIsland]
GO
/****** Object:  Table [dbo].[Balance]    Script Date: 11/08/2017 16:10:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Balance](
	[PriceID] [bigint] NOT NULL,
	[Count] [bigint] NOT NULL,
 CONSTRAINT [PK_Balance] PRIMARY KEY CLUSTERED 
(
	[PriceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentType]    Script Date: 11/08/2017 16:10:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentType](
	[ID] [smallint] NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
 CONSTRAINT [PK_DocumentType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Unit]    Script Date: 11/08/2017 16:10:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Unit](
	[ID] [smallint] NOT NULL,
	[Name] [nvarchar](32) NOT NULL,
	[ShortName] [nvarchar](8) NOT NULL,
 CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 11/08/2017 16:10:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ID] [bigint] NOT NULL,
	[Code] [nvarchar](32) NULL,
	[Name] [nvarchar](512) NOT NULL,
	[UnitID] [smallint] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Document]    Script Date: 11/08/2017 16:10:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Document](
	[ID] [bigint] NOT NULL,
	[Number] [nvarchar](64) NULL,
	[Date] [datetime] NULL,
	[TypeID] [smallint] NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IsPartOf]    Script Date: 11/08/2017 16:10:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IsPartOf](
	[ParentID] [bigint] NOT NULL,
	[ChildID] [bigint] NOT NULL,
	[Count] [bigint] NOT NULL,
 CONSTRAINT [PK_IsPartOf] PRIMARY KEY CLUSTERED 
(
	[ParentID] ASC,
	[ChildID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Price]    Script Date: 11/08/2017 16:10:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Price](
	[ID] [bigint] NOT NULL,
	[ProductID] [bigint] NOT NULL,
	[Price] [decimal](18, 0) NOT NULL,
 CONSTRAINT [PK_Price] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Position]    Script Date: 11/08/2017 16:10:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Position](
	[DocumentID] [bigint] NOT NULL,
	[PriceID] [bigint] NOT NULL,
	[Count] [bigint] NOT NULL,
 CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED 
(
	[DocumentID] ASC,
	[PriceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Version] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Version](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[Number] [bigint] NOT NULL,
	[Description] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  ForeignKey [FK_Product_Unit]    Script Date: 11/08/2017 16:10:07 ******/
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Unit] FOREIGN KEY([UnitID])
REFERENCES [dbo].[Unit] ([ID])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Unit]
GO
/****** Object:  ForeignKey [FK_Document_DocumentType]    Script Date: 11/08/2017 16:10:07 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_DocumentType] FOREIGN KEY([TypeID])
REFERENCES [dbo].[DocumentType] ([ID])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_DocumentType]
GO
/****** Object:  ForeignKey [FK_IsPartOf_Product]    Script Date: 11/08/2017 16:10:07 ******/
ALTER TABLE [dbo].[IsPartOf]  WITH CHECK ADD  CONSTRAINT [FK_IsPartOf_Product] FOREIGN KEY([ParentID])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[IsPartOf] CHECK CONSTRAINT [FK_IsPartOf_Product]
GO
/****** Object:  ForeignKey [FK_IsPartOf_Product1]    Script Date: 11/08/2017 16:10:07 ******/
ALTER TABLE [dbo].[IsPartOf]  WITH CHECK ADD  CONSTRAINT [FK_IsPartOf_Product1] FOREIGN KEY([ChildID])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[IsPartOf] CHECK CONSTRAINT [FK_IsPartOf_Product1]
GO
/****** Object:  ForeignKey [FK_Balance_Price]    Script Date: 11/08/2017 16:10:07 ******/
ALTER TABLE [dbo].[Balance]  WITH CHECK ADD  CONSTRAINT [FK_Balance_Price] FOREIGN KEY([PriceID])
REFERENCES [dbo].[Price] ([ID])
GO
ALTER TABLE [dbo].[Balance] CHECK CONSTRAINT [FK_Balance_Price]
GO
/****** Object:  ForeignKey [FK_Price_Product]    Script Date: 11/08/2017 16:10:07 ******/
ALTER TABLE [dbo].[Price]  WITH CHECK ADD  CONSTRAINT [FK_Price_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[Price] CHECK CONSTRAINT [FK_Price_Product]
GO
/****** Object:  ForeignKey [FK_Position_Document]    Script Date: 11/08/2017 16:10:07 ******/
ALTER TABLE [dbo].[Position]  WITH CHECK ADD  CONSTRAINT [FK_Position_Document] FOREIGN KEY([DocumentID])
REFERENCES [dbo].[Document] ([ID])
GO
ALTER TABLE [dbo].[Position] CHECK CONSTRAINT [FK_Position_Document]
GO
/****** Object:  ForeignKey [FK_Position_Price]    Script Date: 11/08/2017 16:10:07 ******/
ALTER TABLE [dbo].[Position]  WITH CHECK ADD  CONSTRAINT [FK_Position_Price] FOREIGN KEY([PriceID])
REFERENCES [dbo].[Price] ([ID])
GO
ALTER TABLE [dbo].[Position] CHECK CONSTRAINT [FK_Position_Price]
GO

USE [ComfortIsland]
GO

-- типы документов
INSERT INTO [dbo].[DocumentType]
           ([ID]
           ,[Name])
     VALUES
           (1
           ,'Приход')
INSERT INTO [dbo].[DocumentType]
           ([ID]
           ,[Name])
     VALUES
           (2
           ,'Расход')

-- единицы измерения
INSERT INTO [dbo].[Unit]
           ([ID]
           ,[Name]
           ,[ShortName])
     VALUES
           (1
           ,'штука'
           ,'шт.')
INSERT INTO [dbo].[Unit]
           ([ID]
           ,[Name]
           ,[ShortName])
     VALUES
           (2
           ,'метр погонный'
           ,'м/п')


INSERT INTO [dbo].[Version]
           ([TimeStamp]
           ,[Number]
           ,[Description])
     VALUES
           (getdate()
           ,1
           ,'Создана база данных.')
