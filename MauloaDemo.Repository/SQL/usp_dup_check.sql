IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_dup_check]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_dup_check]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_dup_check](
				@skip			int = 0,
				@take			int = 10,
				@total			int output,
				@date_from		datetime,
				@include_dup_check_done	bit = 0
		) AS 

SET NOCOUNT ON

/*
	Examples: 
		declare @total int;	EXEC usp_dup_check 0, 20, @total, '09/20/2014', 0
		declare @total int; EXEC usp_dup_check 0, 20, @total, '10/16/2014', 1
*/

select @total = 0

create table #t (
	[row_number]	int,
	dup_check_done	bit,
	c_num			char(7),
	cust_name		nvarchar(80)	null,
	cust_name_kanji	nvarchar(80)	null,
	agent_cd		char(6),
	sub_agent_cd	char(6),
	area_cd			char(3),
	church_cd		char(5)			null,
    wed_date		datetime		null,
    wed_time		datetime		null,
    cxl_date		datetime		null,
	final_date		datetime		null,
    create_date		datetime,
    update_date		datetime

	--op_seq			int				null,
	--item_type		char(3)			null,
	--book_status		char(1)			null,
	--parent_op_seq	int				null,

	--item_cd			char(15)		null,
	--item_name		varchar(100)	null,
	--item_name_jpn	nvarchar(200)	null
)

-- 念のため日付パラメータから時刻部分を除去。
SELECT @date_from = convert(datetime, convert(varchar(10), @date_from, 111))


INSERT INTO #t
SELECT 
	ROW_NUMBER() OVER (ORDER BY wed_date, wed_time, c_num) AS [row_number],
	c.dup_check_done,
	c.c_num,
	dbo.fnc_getCustName(c.c_num, 0) AS cust_name,
	dbo.fnc_getCustName(c.c_num, 1) AS cust_name_kanji,
	c.agent_cd,
	c.sub_agent_cd,
	c.area_cd,
	c.church_cd,
	c.wed_date,
	c.wed_time,
	c.cxl_date,
	c.final_date,
	c.create_date,
	c.update_date
	--op_seq = null,
	--item_type = null,
	--book_status = null,
	--parent_op_seq = null,
	--item_cd = null,
	--item_name = null,
	--item_name_jpn = null
FROM [customer] c 
WHERE (c.wed_date IS NULL OR c.wed_date >= @date_from)
	AND (c.dup_check_done = 0 OR @include_dup_check_done = 1)
	AND EXISTS(SELECT * FROM customer c2 
			 WHERE (c2.c_num <> c.c_num)
			   AND (c2.g_last = c.g_last)
			   AND (c2.g_first = c.g_first)
			   AND (c2.b_last = c.b_last)
			   AND (c2.b_first = c.b_first)
			)

-- 件数を返す。
SELECT @total = (select COUNT(*) from #t)

-- 結果を返す。
SELECT * FROM #t 
	WHERE ([row_number] > @skip) AND ([row_number] <= (@skip + @take))
	ORDER BY wed_date, wed_time, c_num


GO


