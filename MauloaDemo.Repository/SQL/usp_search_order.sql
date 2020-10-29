IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_search_order]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_search_order]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_search_order](
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
				@book_status	char(1)				-- K(=OK), Q(=RQ), N(=NG), X(=CXLRQ), C(=CXL), 1=OK以外, 2=RQ/CXLRQ
) AS 

SET NOCOUNT ON

/*
	Examples: 
		DECLARE @total int;EXEC usp_search_order 0, 50, @total output, null, null, null, null, null, null, '08/01/2015', '12/31/2015', 'W', -10, null, null, nul, null
		DECLARE @total int;EXEC usp_search_order 0, 50, @total output, null, 'YAMA', null, null, null, null, '09/20/2015', '12/31/2015', 'W', -10, null, null, nul, ''
		DECLARE @total int;EXEC usp_search_order 0, 50, @total output, null, null, null, 'JTBO', null, null, '09/20/2015', '12/10/2015', 'W', -10, null, null, nul, ''
		DECLARE @total int;EXEC usp_search_order 0, 50, @total output, null, null, null, 'JTBW', null, null, '09/20/2015', '12/10/2015', 'W', -10, null, null, nul, ''
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

select @book_status = isnull(@book_status, '')

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
	item_name_jpn	nvarchar(200)	null
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
	ROW_NUMBER() OVER (ORDER BY s.create_date DESC, c.c_num) AS [row_number_C],
	ROW_NUMBER() OVER (ORDER BY s.update_date DESC, c.c_num) AS [row_number_U],
	ROW_NUMBER() OVER (ORDER BY c.wed_date, wed_time, c.c_num) AS [row_number_W],

	c.c_num,
	dbo.fnc_getCustName(c.c_num, 0) AS cust_name,
	dbo.fnc_getCustName(c.c_num, 1) AS cust_name_kanji,
	s.agent_cd,
	s.sub_agent_cd,
	c.area_cd,
	i.church_cd,
	c.wed_date,
	c.wed_time,
	c.cxl_date,
	c.final_date,
	s.create_date,
	s.update_date,
	s.op_seq,
	s.item_type,
	s.book_status,
	s.parent_op_seq,
	s.item_cd,
	i.item_name,
	i.item_name_jpn
FROM [sales] s 
	INNER JOIN [customer] c ON (s.c_num = c.c_num) 
	INNER JOIN [item] i ON (i.item_cd = s.item_cd)
	LEFT JOIN [agent_parent] ap ON (s.sub_agent_cd = ap.child_cd)

WHERE (c.c_num = @c_num OR @c_num IS NULL) 

	AND (@ignore_dates = 1 
		 OR (
				 ( (@date_type <> 'C') OR (s.create_date >= @date_from AND s.create_date < @date_to) )
			 AND ( (@date_type <> 'U') OR (s.update_date >= @date_from AND s.update_date < @date_to) )
			 AND ( (@date_type <> 'W') OR (c.wed_date IS NOT NULL AND c.wed_date >= @date_from AND c.wed_date < @date_to) )
			)
		)

	AND (s.parent_op_seq IS NULL)

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

	AND ( (i.church_cd = @church_cd) OR (@church_cd IS NULL) )

	AND ( (s.book_status = @book_status) OR (@book_status IN ('', '1', '2')) )
	AND ( (s.book_status <> 'K') OR (@book_status <> '1') ) 
	AND ( (s.book_status IN ('Q', 'X')) OR (@book_status <> '2') ) 

	AND ( (s.agent_cd = @agent_cd) OR (@agent_cd IS NULL) )

	AND ( (s.sub_agent_cd = @sub_agent_cd) 
			OR (@parent_cd IS NULL AND ap.parent_cd = @sub_agent_cd) 
			OR (@indep_flg = 1 AND ap.parent_cd = @parent_cd AND ap.agent_area_cd = @agent_area_cd) 
			OR (@sub_agent_cd IS NULL) )

	AND ( (s.item_type = @item_type) OR (@item_type IS NULL OR @item_type IN ('*PK', '*OP')) )
	AND ( (s.item_type IN ('PKG', 'PHP')) OR (@item_type IS NULL OR @item_type <> '*PK') )
	AND ( (s.item_type NOT IN ('PKG', 'PHP')) OR (@item_type IS NULL OR @item_type <> '*OP') )

	AND ( (s.item_cd = @item_cd) OR (@item_cd IS NULL) )

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


