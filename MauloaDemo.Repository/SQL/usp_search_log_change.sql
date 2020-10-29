IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_search_log_change]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_search_log_change]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_search_log_change](
				@skip			int = 0,
				@take			int = 10,
				@total			int output,
				@c_num			char(7),
				@cust_name		nvarchar(50),
				@area_cd		char(3),
				@agent_cd		char(6), 
				@sub_agent_cd	char(6),
				@church_cd		char(5),			
				@item_type		char(3), 
				@item_cd		varchar(15),		-- charではなくvarcharにしないとLIKE検索が出来ない。
				@item_name		nvarchar(200),
				@book_status	char(1),			-- K(=OK), Q(=RQ), N(=NG), X(=CXLRQ), C(=CXL), 1=OK以外, 2=RQ/CXLRQ
				@include_archived bit,				-- アーカイブ済レコードを含めるか否か。
				@viewer_login_id varchar(15),		-- 検索を実行しているユーザーのログインID。
				@action			char(1) = null
) AS 

SET NOCOUNT ON

/*
	Examples: 
		DECLARE @total int;
		EXEC usp_search_log_change 0, 20, @total output, null, null, null, null, null, null, null, null, null, null, 0, 'pmtest'
		EXEC usp_search_log_change 20, 20, @total output, null, null, null, null, null, null, null, null, null, '2', 0, 'pmtest'
		EXEC usp_search_log_change 0, 10, @total output, null, 'Yama', null, null, null, null, null, null, null, '', 0, 'pmtest'
		EXEC usp_search_log_change 0, 20, @total output, null, null, null, 'JTBO', null, null, null, null, null, '', 0, 'pmtest'
		EXEC usp_search_log_change 0, 20, @total output, '10078', null, null, null, null, null, null, null, null, '', 0, 'pmtest'
		EXEC usp_search_log_change 0, 20, @total output, null, null, null, null, null, null, null, null, 'PHOTO', '', 0, 'pmtest'
		SELECT @total;
*/


select @total = 0


DECLARE @viewer_sub_agent_cd char(6), @is_staff bit

SELECT @viewer_sub_agent_cd = sub_agent_cd, 
	   @is_staff = (CASE WHEN user_type = 'STF' THEN 1 ELSE 0 END)
FROM login_user 
WHERE (login_id = @viewer_login_id)
IF (@viewer_sub_agent_cd IS NULL)
BEGIN
	RAISERROR ('Invalid viewer_login_id.', 16, 1)
	return
END

-- 空文字列のパラメータをNULLに変換。
select @c_num = nullif(@c_num, ''), 
	   @cust_name = nullif(@cust_name, ''), 
	   @area_cd = nullif(@area_cd, ''), 
	   @agent_cd = nullif(@agent_cd, ''), 
	   @sub_agent_cd = nullif(@sub_agent_cd, ''), 
	   @church_cd = nullif(@church_cd, ''),
	   @item_type = nullif(@item_type, ''),
	   @item_cd = nullif(@item_cd, ''),
	   @item_name = nullif(@item_name, ''),
	   @action = nullif(@action, '')

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




create table #t (
	log_id			int,
	log_datetime	datetime,
	log_by			varchar(15),
	log_by_sub_agent_cd char(6)		null,
	c_num			char(7)			null,
	table_name		varchar(50),
	[key_id]		int				null,
	[key_cd]		varchar(20)		null,
	[action]		char(1),
	[changes]		nvarchar(max)	null,

	cust_name		nvarchar(80)	null,
	cust_name_kanji	nvarchar(80)	null,
	agent_cd		char(6)			null,
	sub_agent_cd	char(6)			null,
	area_cd			char(3)			null,
	church_cd		char(5)			null,
    wed_date		datetime		null,
    wed_time		datetime		null,

	op_seq			int				null,
	item_type		char(3)			null,
	book_status		char(1)			null,
	parent_op_seq	int				null,

	item_cd			char(15)		null,
	item_name		varchar(100)	null,
	item_name_jpn	nvarchar(200)	null,

	archived		bit,
	archive_by		varchar(15)		null,
	archive_datetime datetime		null
)




INSERT INTO #t
SELECT 
	g.log_id,
	g.log_datetime,	
	g.login_id AS log_by,
	u.sub_agent_cd AS log_by_sub_agent_cd,
	g.c_num,
	g.table_name,
	g.[key_id],
	g.[key_cd],
	g.[action],
	g.[changes],

	dbo.fnc_getCustName(g.c_num, 0) AS cust_name,
	dbo.fnc_getCustName(g.c_num, 1) AS cust_name_kanji,

	c.agent_cd,
	c.sub_agent_cd,
	c.area_cd,
	c.church_cd,
	c.wed_date,
	c.wed_time,

	NULL AS op_seq,
	NULL AS item_type,
	NULL AS book_status,
	NULL AS parent_op_seq,

	NULL AS item_cd,
	NULL AS item_name,
	NULL AS item_name_jpn,

	0 AS archived,
	NULL AS archive_by,
	NULL AS archive_datetime

FROM [log_change] g  
	INNER JOIN [customer] c ON (g.c_num = c.c_num) 
	LEFT JOIN [login_user] u ON (u.login_id = g.login_id)
	LEFT JOIN [agent_parent] ap ON (c.sub_agent_cd = ap.child_cd)
WHERE (g.c_num IS NOT NULL)

	-- 自分自身のログは除外。
	AND (g.login_id <> @viewer_login_id)

	-- action='I'(New) は除外。
	AND ( (g.[action] IN ('U', 'D') AND g.[table_name] <> 'sales')
			OR ( g.[action] IN ('U', 'D') AND g.[table_name] = 'sales' 
						AND EXISTS(select * from sales s where s.op_seq = g.[key_id] and s.parent_op_seq IS NULL) 
			   )
			OR (g.[action] = 'I' AND g.[table_name] IN ('wed_info', 'cos_info', 'schedule_phrase', 'file') )
			OR (g.[action] = 'I' AND g.[table_name] = 'sales' 
						AND EXISTS(select * from sales s where s.op_seq = g.[key_id] and s.parent_op_seq IS NULL) ) 
		)

	AND ( (g.c_num = @c_num) OR (@c_num IS NULL) )

	AND ( (@cust_name IS NULL)
		  OR (c.g_last LIKE @cust_name + '%' ) 
		  OR (c.g_first LIKE @cust_name + '%' ) 
		  OR (c.b_last LIKE @cust_name + '%' ) 
		  OR (c.b_first LIKE @cust_name + '%' ) 
		  --OR (c.g_last_kana LIKE '%' + @cust_name + '%' ) 
		  --OR (c.g_first_kana LIKE '%' + @cust_name + '%' ) 
		  --OR (c.b_last_kana LIKE '%' + @cust_name + '%' ) 
		  --OR (c.b_first_kana LIKE '%' + @cust_name + '%' ) 
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

	AND ( (@include_archived = 1)
			OR
			-- アーカイブ済のログを除外
		  (NOT EXISTS(SELECT * FROM [log_change_archive] a 
					   WHERE (g.log_id = a.log_id) 
						 AND ( (a.archive_by = @viewer_login_id) 
							   OR 
							   (a.sub_agent_cd = @viewer_sub_agent_cd AND u.sub_agent_cd <> @viewer_sub_agent_cd)
							 ) 
					  ) 
		  )
		)

	AND ( (g.[action] = @action) OR (@action IS NULL) )


IF (@include_archived = 1)
BEGIN
	UPDATE #t
	SET archived = 1,
		archive_by = (SELECT TOP 1 archive_by 
					  FROM [log_change_archive] a 
					  WHERE (#t.log_id = a.log_id) AND (a.sub_agent_cd = @viewer_sub_agent_cd)
					  ORDER BY archive_id
					 ),
		archive_datetime = (SELECT TOP 1 archive_datetime
					  FROM [log_change_archive] a 
					  WHERE (#t.log_id = a.log_id) AND (a.sub_agent_cd = @viewer_sub_agent_cd)
					  ORDER BY archive_id
					 )
	WHERE EXISTS(SELECT * FROM [log_change_archive] a 
							INNER JOIN log_change g ON (a.log_id = g.log_id) 
							INNER JOIN [login_user] u ON (u.login_id = g.login_id)
				 WHERE (#t.log_id = a.log_id) 
					AND ( (a.archive_by = @viewer_login_id) 
						  OR 
						  (a.sub_agent_cd = @viewer_sub_agent_cd AND u.sub_agent_cd <> @viewer_sub_agent_cd)
					    )
				)
END


-- Salesのステータス等を取得。
UPDATE #t SET 
	op_seq = s.op_seq,
	item_type = s.item_type,
	book_status = s.book_status,
	parent_op_seq =s.parent_op_seq,
	item_cd = s.item_cd
FROM #t INNER JOIN sales s ON (#t.[key_id] = s.op_seq)
	INNER JOIN item i ON (i.item_cd = s.item_cd) 
WHERE (#t.table_name = 'sales')

UPDATE #t SET 
	op_seq = s.op_seq,
	item_type = s.item_type,
	book_status = s.book_status,
	parent_op_seq =s.parent_op_seq,
	item_cd = s.item_cd
FROM #t INNER JOIN arrangement a ON (#t.[key_id] = a.arrangement_id)
	INNER JOIN sales s ON (a.op_seq = s.op_seq)
	INNER JOIN item i ON (i.item_cd = s.item_cd) 
WHERE (#t.table_name = 'arrangement')



UPDATE #t SET 
	op_seq = s.op_seq,
	item_type = s.item_type,
	book_status = s.book_status,
	parent_op_seq =s.parent_op_seq,
	item_cd = s.item_cd,
	wed_date = w.req_wed_date, 
	wed_time = w.req_wed_time,
	church_cd = w.req_church_cd
FROM #t INNER JOIN wed_info w ON (#t.[key_id] = w.info_id)
	INNER JOIN sales s ON (w.op_seq = s.op_seq)
	INNER JOIN item i ON (i.item_cd = s.item_cd) 
WHERE (#t.table_name = 'wed_info')

UPDATE #t SET 
	item_type = 'COS',
	item_name	  = CASE w.pax_type WHEN 'G' THEN 'Groom' WHEN 'B' THEN 'Bride' ELSE 'Attendant' END + ' Costume',
	item_name_jpn = CASE w.pax_type WHEN 'G' THEN 'Groom' WHEN 'B' THEN 'Bride' ELSE 'Attendant' END + ' Costume'
FROM #t INNER JOIN cos_info w ON (#t.[key_id] = w.info_id)
WHERE (#t.table_name = 'cos_info') AND (#t.[action] = 'I')

UPDATE #t SET 
	op_seq = s.op_seq,
	item_type = s.item_type,
	book_status = s.book_status,
	parent_op_seq =s.parent_op_seq,
	item_cd = s.item_cd
FROM #t INNER JOIN delivery_info w ON (#t.[key_id] = w.info_id)
	INNER JOIN sales s ON (w.op_seq = s.op_seq)
	INNER JOIN item i ON (i.item_cd = s.item_cd) 
WHERE (#t.table_name = 'delivery_info')

UPDATE #t SET 
	op_seq = s.op_seq,
	item_type = s.item_type,
	book_status = s.book_status,
	parent_op_seq =s.parent_op_seq,
	item_cd = s.item_cd
FROM #t INNER JOIN make_info w ON (#t.[key_id] = w.info_id)
	INNER JOIN sales s ON (w.op_seq = s.op_seq)
	INNER JOIN item i ON (i.item_cd = s.item_cd) 
WHERE (#t.table_name = 'make_info')

UPDATE #t SET 
	op_seq = s.op_seq,
	item_type = s.item_type,
	book_status = s.book_status,
	parent_op_seq =s.parent_op_seq,
	item_cd = s.item_cd
FROM #t INNER JOIN rcp_info w ON (#t.[key_id] = w.info_id)
	INNER JOIN sales s ON (w.op_seq = s.op_seq)
	INNER JOIN item i ON (i.item_cd = s.item_cd) 
WHERE (#t.table_name = 'rcp_info')

UPDATE #t SET 
	op_seq = s.op_seq,
	item_type = s.item_type,
	book_status = s.book_status,
	parent_op_seq =s.parent_op_seq,
	item_cd = s.item_cd
FROM #t INNER JOIN shoot_info w ON (#t.[key_id] = w.info_id)
	INNER JOIN sales s ON (w.op_seq = s.op_seq)
	INNER JOIN item i ON (i.item_cd = s.item_cd) 
WHERE (#t.table_name = 'shoot_info')

UPDATE #t SET 
	op_seq = s.op_seq,
	item_type = s.item_type,
	book_status = s.book_status,
	parent_op_seq =s.parent_op_seq,
	item_cd = s.item_cd
FROM #t INNER JOIN trans_info w ON (#t.[key_id] = w.info_id)
	INNER JOIN sales s ON (w.op_seq = s.op_seq)
	INNER JOIN item i ON (i.item_cd = s.item_cd) 
WHERE (#t.table_name = 'trans_info')



-- Schedule PhraseのNewについてChangesを更新
UPDATE #t SET 
	item_name = 'New schedule',
	item_name_jpn = 'スケジュール作成'
WHERE ([action] = 'I') 
	AND (table_name = 'schedule_phrase')

-- Schedule PhraseのNewを1行(log_idが最小のもの)だけに絞る。
DELETE #t
WHERE ([action] = 'I') 
	AND (table_name = 'schedule_phrase')
	AND (EXISTS(select * from #t t2 
				where #t.c_num = t2.c_num and #t.log_id < t2.log_id 
					and t2.[action] = #t.[action] and t2.table_name = #t.table_name 
					--and ( NOT EXISTS(SELECT * FROM [log_change_archive] a 
					--				 WHERE (t2.log_id = a.log_id) AND (a.sub_agent_cd = @viewer_sub_agent_cd)) )
			    )
		)

-- プランに含まれるアイテムの新規追加のログ（actionが'I'(New)でparent_op_seqがあるもの）は除外。
DELETE #t 
WHERE ([action] = 'I') AND (parent_op_seq IS NOT NULL)


-- 新規追加のログ（actionが'I'(New)で最新ステータスがOK/RQ以外のものは除外。
DELETE #t 
WHERE ([action] = 'I') AND (#t.book_status NOT IN ('K', 'Q'))


-- Salesのステータスでフィルタリング。
IF (@book_status IS NOT NULL AND @book_status <> '') 
BEGIN
	DELETE #t
	WHERE ( (#t.book_status <> @book_status) AND (@book_status NOT IN ('1', '2')) )
		OR ( (#t.book_status = 'K') AND (@book_status = '1') ) 
		OR ( (#t.book_status NOT IN ('Q', 'X')) AND (@book_status = '2') ) 
END

-- Salesのitem_typeでフィルタリング。
IF (@item_type IS NOT NULL AND @item_type <> '')
BEGIN
	DELETE #t
	WHERE ( ( (#t.item_type <> @item_type OR #t.item_type IS NULL) AND (@item_type NOT IN ('*PK', '*OP'))) --特定のitem_typeが指定された時
			OR ( (#t.item_type NOT IN ('PHP', 'PKG') OR #t.item_type IS NULL) AND (@item_type = '*PK'))	--「プランのみ」が指定された時
			OR ( (#t.item_type IN ('PHP', 'PKG') OR #t.item_type IS NULL) AND (@item_type = '*OP'))		--「オプションのみ」が指定された時
		  )
END

-- Salesのitem_cdでフィルタリング。
IF (@item_cd IS NOT NULL AND @item_cd <> '')
BEGIN
	DELETE #t
	WHERE (#t.item_cd <> @item_cd OR #t.item_cd IS NULL)
END



-- アイテム名を取得。
UPDATE #t SET 
	item_name = i.item_name,
	item_name_jpn = i.item_name_jpn
FROM #t INNER JOIN item i ON (i.item_cd = #t.item_cd) 
WHERE (#t.item_cd IS NOT NULL)



-- 件数を返す。
SELECT @total = (select COUNT(*) from #t)

-- 行番号を付ける。
SELECT 
	ROW_NUMBER() OVER (ORDER BY log_datetime DESC, c_num) AS [row_number],
	log_id, 
	log_datetime,
	log_by,
	log_by_sub_agent_cd,
	c_num,
	table_name,
	[action],
	[changes],

	cust_name,
	cust_name_kanji,
	agent_cd,
	sub_agent_cd,
	area_cd,
	church_cd,
    wed_date,
    wed_time,

	op_seq,
	item_type,
	book_status,
	parent_op_seq,

	item_cd,
	item_name,
	item_name_jpn,

	archived,
	archive_by,
	archive_datetime
INTO #t2
FROM #t

-- 指定ページ内のレコードのみを抽出して返す。
SELECT *
FROM #t2 
WHERE ([row_number] > @skip) AND ([row_number] <= (@skip + @take))
ORDER BY log_datetime DESC, c_num

GO


