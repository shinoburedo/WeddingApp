IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_search_customer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_search_customer]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_search_customer](
				@skip			int = 0,
				@take			int = 10,
				@total			int output,
				@c_num			char(7),
				@cust_name		nvarchar(50),
				@area_cd		char(3),
				@agent_cd		char(6), 
				@sub_agent_cd	char(6),
				@church_cd		char(5),			
				@date_from		datetime,
				@date_to		datetime,
				@date_type		char(1),			-- W=wed_date, C=create_date, U=update_date
				@time_zone		int,
				@item_type		char(3), 
				@item_cd		varchar(15),		-- charではなくvarcharにしないとLIKE検索が出来ない。
				@item_name		nvarchar(200),
				@not_finalized_only	 bit = 0
) AS 

SET NOCOUNT ON

/*
	Examples: 
		DECLARE @total int;EXEC usp_search_customer 0, 50, @total output, null, null, null, null, null, null, '09/01/2015', '12/31/2015', 'W', -10, null, null, null
		DECLARE @total int;EXEC usp_search_customer 0, 50, @total output, null, 'YAMA', null, null, null, null, '09/20/2015', '12/31/2015', 'W', -10, null, null, null
		DECLARE @total int;EXEC usp_search_customer 0, 50, @total output, null, null, null, 'HISJ', null, null, '09/20/2015', '12/10/2015', 'W', -10, null, null, null
		DECLARE @total int;EXEC usp_search_customer 0, 50, @total output, null, null, null, 'JTBO', null, null, '09/20/2015', '12/10/2015', 'W', -10, null, null, null
		SELECT @total;
*/

select @total = 0

-- 空文字列のパラメータをNULLに変換。
select @c_num = nullif(@c_num, ''), 
	   @cust_name = nullif(@cust_name, ''), 
	   @area_cd = nullif(@area_cd, ''), 
	   @agent_cd = nullif(@agent_cd, ''), 
	   @sub_agent_cd = nullif(@sub_agent_cd, ''), 
	   @church_cd = nullif(@church_cd, ''),
	   @item_type = nullif(@item_type, ''),
	   @item_cd = nullif(@item_cd, ''),
	   @item_name = nullif(@item_name, '')

-- @sub_agent_cdが指定されている場合はagent_parentテーブルからparent_cdとindep_flgを取得。
DECLARE @parent_cd char(6), @indep_flg bit, @agent_area_cd char(5)
SELECT @parent_cd = NULL, @indep_flg = 0, @agent_area_cd = NULL
IF @sub_agent_cd IS NOT NULL
BEGIN
	SELECT @parent_cd = ap.parent_cd, @indep_flg = ap.indep_flg, @agent_area_cd = ap.agent_area_cd 
	FROM agent_parent ap
	WHERE (ap.child_cd = @sub_agent_cd)
END

-- @c_numか@cust_nameが指定されている場合は日付の条件を無視する。
DECLARE @ignore_dates bit
SELECT @ignore_dates = CASE WHEN @c_num IS NOT NULL OR @cust_name IS NOT NULL THEN 1 ELSE 0 END


CREATE TABLE #t (
	[row_number_C]	int,
	[row_number_U]	int,
	[row_number_W]	int,
	c_num			char(7),
	cust_name		nvarchar(80)	null,
	cust_name_kanji	nvarchar(80)	null,
	g_last			varchar(20),
	agent_cd		char(6),
	sub_agent_cd	char(6),
	area_cd			char(3),
	church_cd		char(5)			null,
    wed_date		datetime		null,
    wed_time		datetime		null,
    cxl_date		datetime		null,
    final_date		datetime		null,
    create_date		datetime,
    update_date		datetime,

	op_seq			int				null,
	item_type		char(3)			null,
	book_status		char(1)			null,
	parent_op_seq	int				null,

	item_cd			char(15)		null,
	item_name		varchar(100)	null,
	item_name_jpn	nvarchar(200)	null,

	is_irregular_time bit		NOT NULL
)


-- 念のため日付パラメータから時刻部分を除去。
SELECT @date_from = convert(datetime, convert(varchar(10), @date_from, 111))
SELECT @date_to = convert(datetime, convert(varchar(10), @date_to, 111))

-- To日付は「<」で比較するため1日加算する。
SELECT @date_to = DATEADD(dd, 1, @date_to)

-- 日付タイプが登録日または更新日の場合はUTC時刻で比較。
IF @date_type <> 'W'
BEGIN
	SELECT @date_from = DATEADD(hh, @time_zone * -1, @date_from),
		   @date_to = DATEADD(hh, @time_zone * -1, @date_to)
END


INSERT INTO #t
SELECT 
	ROW_NUMBER() OVER (ORDER BY c.create_date DESC, c_num) AS [row_number_C],
	ROW_NUMBER() OVER (ORDER BY c.update_date DESC, c_num) AS [row_number_U],
	ROW_NUMBER() OVER (ORDER BY c.wed_date, wed_time, c_num) AS [row_number_W],

	c.c_num,
	dbo.fnc_getCustName(c.c_num, 0) AS cust_name,
	dbo.fnc_getCustName(c.c_num, 1) AS cust_name_kanji,
	c.g_last,
	c.agent_cd,
	c.sub_agent_cd,
	c.area_cd,
	c.church_cd,
	c.wed_date,
	c.wed_time,
	c.cxl_date,
	c.final_date,
	c.create_date,
	c.update_date,
	op_seq = null,
	item_type = null,
	book_status = null,
	parent_op_seq = null,
	item_cd = null,
	item_name = null,
	item_name_jpn = null,
	is_irregular_time = 0
FROM [customer] c 
	LEFT JOIN [agent_parent] ap ON (c.sub_agent_cd = ap.child_cd)

WHERE (c.c_num = @c_num OR @c_num IS NULL) 

	AND (@ignore_dates = 1 
		 OR (
				 ( (@date_type <> 'C') OR (c.create_date >= @date_from AND c.create_date < @date_to) )
			 AND ( (@date_type <> 'U') OR (c.update_date >= @date_from AND c.update_date < @date_to) )
			 AND ( (@date_type <> 'W') OR (c.wed_date IS NOT NULL AND c.wed_date >= @date_from AND c.wed_date < @date_to) )
			)
		)

	AND ( (@cust_name IS NULL)
		  OR (c.g_last LIKE @cust_name + '%' ) 
		  OR (c.g_first LIKE @cust_name + '%' ) 
		  OR (c.b_last LIKE @cust_name + '%' ) 
		  OR (c.b_first LIKE @cust_name + '%' ) 
		  --OR (c.g_last_kana LIKE @cust_name + '%' ) 
		  --OR (c.g_first_kana LIKE @cust_name + '%' ) 
		  --OR (c.b_last_kana LIKE @cust_name + '%' ) 
		  --OR (c.b_first_kana LIKE @cust_name + '%' ) 
		  OR (c.g_last_kanji LIKE @cust_name + '%' ) 
		  OR (c.g_first_kanji LIKE @cust_name + '%' ) 
		  OR (c.b_last_kanji LIKE @cust_name + '%' ) 
		  OR (c.b_first_kanji LIKE @cust_name + '%' ) 
		)

	AND ( (c.area_cd = @area_cd) OR (@area_cd IS NULL) )

	AND ( (c.church_cd = @church_cd) OR (@church_cd IS NULL) )

	AND ( (c.agent_cd = @agent_cd) OR (@agent_cd IS NULL) )

	AND ( (c.sub_agent_cd = @sub_agent_cd) 
			OR (@parent_cd IS NULL AND ap.parent_cd = @sub_agent_cd) 
			OR (@indep_flg = 1 AND ap.parent_cd = @parent_cd AND ap.agent_area_cd = @agent_area_cd) 
			OR (@sub_agent_cd IS NULL) )

	AND ( (@not_finalized_only = 0)
			OR
			-- Finalize済を除外
		  (c.final_by IS NULL)
		)


-- カスタマー毎にOKまたはCXLRQ中のプランの情報をセット
UPDATE #t 
SET item_type = s.item_type,
	book_status = s.book_status,
	is_irregular_time = w.is_irregular_time,
	item_cd = s.item_cd,
	item_name = i.item_name,
	item_name_jpn = i.item_name_jpn
FROM #t  
	INNER JOIN wed_info w ON (w.c_num = #t.c_num 
							AND w.req_church_cd = #t.church_cd 
							AND w.req_wed_date = #t.wed_date 
							AND w.req_wed_time = #t.wed_time)
	INNER JOIN sales s ON (s.op_seq = w.op_seq AND s.book_status IN ('K', 'X'))
	INNER JOIN item i ON (s.item_cd = i.item_cd)



-- 件数を返す。
SELECT @total = (select COUNT(*) from #t)


-- 結果を返す。
IF @date_type = 'C' 
	SELECT * FROM #t 
	WHERE ([row_number_C] > @skip) AND ([row_number_C] <= (@skip + @take))
	ORDER BY create_date DESC, c_num

ELSE IF @date_type = 'U'
	SELECT * FROM #t 
	WHERE ([row_number_U] > @skip) AND ([row_number_U] <= (@skip + @take))
	ORDER BY update_date DESC, c_num

ELSE
	SELECT * FROM #t 
	WHERE ([row_number_W] > @skip) AND ([row_number_W] <= (@skip + @take))
	ORDER BY wed_date, wed_time, c_num


GO


