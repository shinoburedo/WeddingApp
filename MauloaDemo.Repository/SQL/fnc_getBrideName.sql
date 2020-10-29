/* ================================================
 
 fnc_getBrideName
 
 Example: 
	SELECT dbo.fnc_getBrideName('10075', 0)
	SELECT dbo.fnc_getBrideName('10075', 1)
	SELECT dbo.fnc_getBrideName('10075', 2)

================================================
*/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fnc_getBrideName]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[fnc_getBrideName]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnc_getBrideName]
(
	@c_num char(7),
	@lang int = 0
)
RETURNS nvarchar(80)
AS
BEGIN
	DECLARE @cust_name nvarchar(80)

	SELECT @cust_name = CASE WHEN @lang = 0 THEN 
							b_last + isnull(' ' + b_first, '')
						WHEN @lang = 1 THEN 
							b_last_kanji + isnull(' ' + b_first_kanji, '')
						ELSE
							isnull(dbo.fnc_getBrideName(@c_num, 1), dbo.fnc_getBrideName(@c_num, 0))
						END
	FROM customer 
	WHERE (c_num = @c_num)

	RETURN @cust_name
END

GO




