IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fnc_split]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[fnc_split]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnc_split]( @str nvarchar(max), @delimiter nchar(1) )
RETURNS @return TABLE (value NVARCHAR(max) NULL)
AS
BEGIN

/*
  Example: 
 
     SELECT value FROM [dbo].[fnc_split]('aaa,bbb,ccc, ddd, eeee ,,g,hhhhhhhhhhh,テスト,です。', ',')
     SELECT value FROM [dbo].[fnc_split]('aaa,bbb,ccc, ddd, eeee ,,g,hhhhhhhhhhh,テスト,です。', ',')
     SELECT value FROM [dbo].[fnc_split]('あいうえお', ',')
     SELECT value FROM [dbo].[fnc_split]('あいうえお, かきくけこ', ',')
 
 */


	DECLARE @ix_start int, @ix_end int
	DECLARE @buf as nvarchar(max)

	SET @ix_start = 1
	SET @ix_end = CHARINDEX(@delimiter, @str) -1

	WHILE @ix_end >= 0
	BEGIN
		SET @buf = RTRIM(LTRIM(SUBSTRING(@str, @ix_start, @ix_end)))
		INSERT INTO @return VALUES(@buf)

		SET @ix_start = @ix_start + @ix_end + 1 
		SET @ix_end = CHARINDEX(@delimiter, @str, @ix_start) - @ix_start 
	END 

	SET @ix_end = len(@str) 
	SET @buf = RTRIM(LTRIM(SUBSTRING(@str, @ix_start, @ix_end)))
	INSERT INTO @return VALUES(@buf)

	RETURN
END

GO


