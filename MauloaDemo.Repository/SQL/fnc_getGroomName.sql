/* ================================================
 
 fnc_getGroomName
 
 Example: 
	SELECT dbo.fnc_getGroomName('10075', 0)
	SELECT dbo.fnc_getGroomName('10075', 1)
	SELECT dbo.fnc_getGroomName('10075', 2)

================================================
*/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fnc_getGroomName]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[fnc_getGroomName]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnc_getGroomName]
(
	@c_num char(7),
	@lang int = 0
)
RETURNS nvarchar(80)
AS
BEGIN
	DECLARE @cust_name nvarchar(80)

	SELECT @cust_name = CASE WHEN @lang = 0 THEN 
							g_last + isnull(' ' + g_first, '')
						WHEN @lang = 1 THEN 
							g_last_kanji + isnull(' ' + g_first_kanji, '')
						ELSE
							isnull(dbo.fnc_getGroomName(@c_num, 1), dbo.fnc_getGroomName(@c_num, 0))
						END
	FROM customer 
	WHERE (c_num = @c_num)

	RETURN @cust_name
END

GO




