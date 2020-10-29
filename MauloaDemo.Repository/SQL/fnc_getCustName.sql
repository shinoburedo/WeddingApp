/* ================================================
 
 fnc_getCustName
 
 Example: 
	SELECT dbo.fnc_getCustName('10075', 0)
	SELECT dbo.fnc_getCustName('10075', 1)
	SELECT dbo.fnc_getCustName('10075', 2)

================================================
*/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fnc_getCustName]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[fnc_getCustName]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnc_getCustName]
(
	@c_num char(7),
	@lang int = 0
)
RETURNS nvarchar(80)
AS
BEGIN
	DECLARE @cust_name nvarchar(80), @groom_name nvarchar(40), @bride_name nvarchar(40)

	SELECT @groom_name = dbo.fnc_getGroomName(@c_num, @lang)
	SELECT @bride_name = dbo.fnc_getBrideName(@c_num, @lang)

	SELECT @cust_name = @groom_name + isnull(' / ' + @bride_name, '')
						
	FROM customer 
	WHERE (c_num = @c_num)

	RETURN @cust_name
END

GO




