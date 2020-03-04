CREATE TABLE [dbo].[Table1] (
    [Id]           BIGINT          NOT NULL,
    [Value1]       SMALLINT        NULL,
    [Value2]       BIGINT          NULL,
    [Value3]       DECIMAL (18, 3) NULL,
    [Value4]       DATETIME        NULL,
	[Value5]       VARCHAR(50)        NULL,
    [DateCreation] DATETIME        NULL,
    CONSTRAINT [PK_Table1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

