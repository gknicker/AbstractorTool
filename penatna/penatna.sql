USE [penatna]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Table [dbo].[Admin]    Script Date: 12/12/2013 4:30:28 PM ******/
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Admin](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[LoginName] [varchar](20) NOT NULL,
	[PasswordHash] [char](64) NOT NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Admin] UNIQUE NONCLUSTERED 
(
	[LoginName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[User]    Script Date: 12/12/2013 4:17:42 PM ******/
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[User](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[LoginName] [nvarchar](50) NOT NULL,
	[Password] [varchar](64) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[InvoicePrefix] [varchar](10) NOT NULL,
	[NextInvoiceNumber] [int] NOT NULL,
	[SearchFee] [smallmoney] NOT NULL,
	[CopyFee] [smallmoney] NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[MiddleInitial] [char](1) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[Address] [varchar](100) NOT NULL,
	[City] [varchar](50) NOT NULL,
	[StateAbbr] [char](2) NOT NULL,
	[Zip] [decimal](5, 0) NOT NULL,
	[ZipPlus4] [decimal](4, 0) NULL,
	[Phone] [varchar](20) NULL,
	[DateCreated] [smalldatetime] NOT NULL,
	[DateUpdated] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_User] UNIQUE NONCLUSTERED 
(
	[LoginName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_User_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[Order]    Script Date: 12/12/2013 4:17:42 PM ******/
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Order](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CaseNumber] [int] NOT NULL,
	[TaskID] [int] NOT NULL,
	[DateAssigned] [date] NOT NULL,
	[DateImported] [smalldatetime] NOT NULL,
	[DateCompleted] [smalldatetime] NULL,
	[AbstractFilePath] [varchar](400) NULL,
	[SearchFee] [smallmoney] NOT NULL,
	[CopyFee] [smallmoney] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Address] [nvarchar](100) NOT NULL,
	[County] [varchar](30) NOT NULL,
	[State] [char](2) NOT NULL,
	[InstrumentDate] [date] NULL,
	[RecordingDate] [date] NULL,
	[MortgageAmount] [money] NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Order] UNIQUE NONCLUSTERED 
(
	[TaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_User_Order] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_User_Order]
GO

/****** Object:  Table [dbo].[Page]    Script Date: 12/12/2013 4:20:00 PM ******/
CREATE TABLE [dbo].[Page](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[OrderId] [uniqueidentifier] NOT NULL,
	[PageNumber] [tinyint] NOT NULL,
	[PageFilePath] [varchar](400) NOT NULL,
 CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Page]  WITH CHECK ADD  CONSTRAINT [FK_Order_Page] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
GO

ALTER TABLE [dbo].[Page] CHECK CONSTRAINT [FK_Order_Page]
GO

/****** Object:  Table [dbo].[Abstract]    Script Date: 12/12/2013 4:19:28 PM ******/
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Abstract](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[OrderId] [uniqueidentifier] NOT NULL,
	[PageNumber] [tinyint] NOT NULL,
	[FieldLabel] [varchar](100) NOT NULL,
	[LabelTopPixels] [smallint] NOT NULL,
	[LabelLeftPixels] [smallint] NOT NULL,
	[LabelHeightPixels] [smallint] NOT NULL,
	[LabelWidthPixels] [smallint] NOT NULL,
	[ValueTopPixels] [smallint] NOT NULL,
	[ValueLeftPixels] [smallint] NOT NULL,
	[ValueHeightPixels] [smallint] NOT NULL,
	[ValueWidthPixels] [smallint] NOT NULL,
	[FieldValue] [varchar](MAX) NOT NULL,
	[LastUpdated] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_Abstract] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Abstract] UNIQUE NONCLUSTERED 
(
	[OrderId] ASC,
	[PageNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Abstract]  WITH CHECK ADD  CONSTRAINT [FK_Order_Abstract] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
GO

ALTER TABLE [dbo].[Abstract] CHECK CONSTRAINT [FK_Order_Abstract]
GO

/****** Object:  Table [dbo].[Invoice]    Script Date: 12/12/2013 4:20:39 PM ******/
CREATE TABLE [dbo].[Invoice](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[OrderId] [uniqueidentifier] NOT NULL,
	[Number] [int] NOT NULL,
	[Amount] [smallmoney] NOT NULL,
	[SearchFee] [smallmoney] NOT NULL,
	[CopyFee] [smallmoney] NOT NULL,
	[PageCount] [tinyint] NOT NULL,
	[DateCreated] [smalldatetime] NOT NULL,
	[DateBilled] [smalldatetime] NULL,
	[DatePaid] [smalldatetime] NULL,
	[DateVoided] [smalldatetime] NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Order_Invoice] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
GO

ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Order_Invoice]
GO

