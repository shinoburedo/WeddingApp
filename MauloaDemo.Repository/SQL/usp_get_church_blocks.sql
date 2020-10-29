IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_get_church_blocks]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_get_church_blocks]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_get_church_blocks](
				@church_cd		char(5),			
				@start_date		datetime, 
				@end_date		datetime,
				@location		char(5),
				@agent_cd		char(6) = null
) AS 

SET NOCOUNT ON

/*
	Examples: 
		EXEC usp_get_church_blocks 'CALVY', '12/01/2015', '12/24/2015', null
		EXEC usp_get_church_blocks 'CALVY', '10/01/2015', '10/31/2015', null
		EXEC usp_get_church_blocks 'PHOTO', '10/01/2015', '10/10/2015', 'TREE', 'JTBO'
		EXEC usp_get_church_blocks 'MAGIC', '11/01/2015', '11/15/2015', null
*/

IF @start_date > @end_date
BEGIN
	declare @tmp datetime
	SELECT @tmp = @start_date
	SELECT @start_date = @end_date
	SELECT @end_date = @tmp
END

DECLARE @plan_kind char(1), @is_photo bit
SELECT @plan_kind = isnull(plan_kind, '') FROM church WHERE (church_cd = @church_cd)
SELECT @is_photo = CASE WHEN @plan_kind IN ('S', 'P', '') THEN 1 ELSE 0 END

SET @location = isnull(@location, @church_cd)


CREATE TABLE #d (
	block_date		datetime NOT NULL
)

CREATE TABLE #t (
	row_id			int NOT NULL IDENTITY (1, 1),
	church_cd		char(5) NOT NULL,
	block_date		datetime NOT NULL,
	start_time		datetime NOT NULL,
	start_time_s	char(5) NOT NULL,

	pickup_time		datetime NULL,
	pickup_time_s		char(5) NULL,

	is_sunset		bit		NOT NULL,
	is_irregular_time bit	NOT NULL,
	block_status	char(6)	NULL,
	book_status		char(1) NULL,

	agent_cd		char(6) NULL,
	sub_agent_cd	char(6) NULL,
	c_num			char(7) NULL,
	g_last			varchar(20) NULL,
	g_last_kanji	nvarchar(20) NULL,
	is_finalized	bit		NULL
)

--***** 一時テーブルに指定範囲の日付分のレコードを挿入。
DECLARE @dt datetime
SET @dt = @start_date
WHILE @dt <= @end_date
BEGIN
	INSERT INTO #d (block_date) VALUES (@dt) 
	SELECT @dt = DATEADD(dd, 1, @dt)
END

--***** 日付毎にその教会の既存ブロックを取得。
INSERT INTO #t (
	church_cd, 
	block_date, 
	start_time, 
	start_time_s, 
	is_sunset, 
	is_irregular_time, 
	block_status)
SELECT 
	@location, 
	#d.block_date, 
	b.start_time, 
	start_time_s = convert(char(5), b.start_time, 108), 
	is_sunset = 0,
	is_irregular_time = 0,
	b.block_status
FROM #d INNER JOIN church_block b ON (b.church_cd = @location AND b.block_date = #d.block_date)


--***** 日付毎にその教会のデフォルト時間枠を取得。(既存ブロックが無い場合)
INSERT INTO #t (
	church_cd, 
	block_date, 
	start_time, 
	start_time_s, 
	is_sunset, 
	is_irregular_time, 
	block_status)
SELECT 
	@location, 
	#d.block_date, 
	ct.start_time, 
	start_time_s = convert(char(5), ct.start_time, 108), 
	is_sunset = 0,
	is_irregular_time = 0,
	block_status = null
FROM #d CROSS JOIN church_time ct 
WHERE (ct.church_cd = @location)
	AND NOT EXISTS(select * from #t 
				   where #t.church_cd = @location 
					 and #t.block_date = #d.block_date 
					 and #t.start_time_s = convert(char(5), ct.start_time, 108))


--***** 「挙式プラン」または「フォトプラン」の予約内容を反映。
-- 挙式プランの場合は指定の教会で絞る。
-- フォトプランの場合はitem_type='PHP'の予約を全て反映する。
UPDATE #t 
SET block_status = '*BKD*',
	book_status = s.book_status,
	church_cd = w.req_church_cd, 
	agent_cd = c.agent_cd,
	sub_agent_cd = c.sub_agent_cd, 
	c_num = c.c_num, 
	g_last = c.g_last,
	g_last_kanji = c.g_last_kanji,
	is_finalized = CASE WHEN c.final_date IS NOT NULL THEN 1 ELSE 0 END
FROM #t 
	INNER JOIN wed_info w ON ( ((w.req_church_cd = #t.church_cd) OR (@is_photo = 1))
								AND (w.req_wed_date = #t.block_date)
								AND (convert(char(5), w.req_wed_time, 108) = #t.start_time_s)
						     )
	INNER JOIN sales s ON (s.op_seq = w.op_seq AND s.book_status IN ('K', 'X'))
	INNER JOIN customer c ON (s.c_num = c.c_num)
WHERE (@is_photo <> 1 OR (@is_photo = 1 AND s.item_type = 'PHP' 
									--	AND #t.start_time_s <> '10:00' 
						 )
	  )


----***** フォトプランの10時枠の予約を取得。
--IF @is_photo = 1
--BEGIN
--	declare @bk_agent_cd char(6), @bk_sub_agent_cd char(6), @bk_c_num char(7), 
--			@bk_g_last varchar(20), @bk_g_last_kanji nvarchar(20), @is_finalized bit,
--			@bk_wed_date datetime, @bk_wed_time_s char(5), @bk_op_seq int, @bk_church_cd char(5), @book_status,
--			@row_id int, @count int

--	DECLARE mycursor CURSOR
--    FOR SELECT DISTINCT s.agent_cd, s.sub_agent_cd, s.c_num, c.g_last, c.g_last_kanji, CASE WHEN c.final_date IS NOT NULL THEN 1 ELSE 0 END
--			  w.req_wed_date, convert(char(5), w.req_wed_time, 108), s.op_seq, w.req_church_cd, s.book_status
--		FROM #t 
--			INNER JOIN wed_info w ON ( ((w.req_church_cd = #t.church_cd) OR (@is_photo = 1)) 
--										AND (w.req_wed_date = #t.block_date)
--										AND (convert(char(5), w.req_wed_time, 108) = #t.start_time_s)
--									 )
--			INNER JOIN sales s ON (s.op_seq = w.op_seq AND s.book_status IN ('K', 'X'))
--			INNER JOIN customer c ON (s.c_num = c.c_num)
--		WHERE (s.item_type = 'PHP' AND #t.start_time_s = '10:00')
--		ORDER BY s.op_seq
--	OPEN mycursor

--	FETCH NEXT FROM mycursor 
--	INTO @bk_agent_cd, @bk_sub_agent_cd, @bk_c_num, @bk_g_last, @bk_g_last_kanji, @is_finalized, 
--				@bk_wed_date, @bk_wed_time_s, @bk_op_seq, @bk_church_cd, @book_status

--	SET @count = 0

--	WHILE @@FETCH_STATUS = 0
--	BEGIN
--		SET @row_id = 0
--		SET @count = @count + 1

--		SELECT TOP 1 @row_id = row_id
--		FROM #t 
--		WHERE (#t.block_status IS NULL)
--			AND (#t.block_date = @bk_wed_date) 
--			AND (#t.start_time_s = @bk_wed_time_s)
--			AND ( (@location <> 'TREE')
--				OR (@count >= 2 OR @bk_church_cd = 'TREE')		-- TREEの場合のみ、TREE自身に予約が入っているか、または他のロケーションで2件以上の予約が入っている場合のみ「*BKD*」にする。
--				)
--		ORDER BY row_id

--		IF (@row_id <> 0) 			
--		BEGIN
--			UPDATE #t 
--			SET block_status = '*BKD*',
--				book_status = @book_status,
--				church_cd = @bk_church_cd, 
--				agent_cd = @bk_agent_cd,
--				sub_agent_cd = @bk_sub_agent_cd, 
--				c_num = @bk_c_num, 
--				g_last = @bk_g_last,
--				g_last_kanji = @bk_g_last_kanji, 
--				is_finalized = @is_finalized
--			FROM #t 
--			WHERE (#t.row_id = @row_id)
--		END

--		FETCH NEXT FROM mycursor 
--		INTO @bk_agent_cd, @bk_sub_agent_cd, @bk_c_num, @bk_g_last, @bk_g_last_kanji, @is_finalized, 
--					@bk_wed_date, @bk_wed_time_s, @bk_op_seq, @bk_church_cd, @book_status
--	END
--	CLOSE mycursor;
--	DEALLOCATE mycursor;
--END


--***** イレギュラーで入力された予約を取得
INSERT INTO #t (
	church_cd,
	block_date,
	start_time,
	start_time_s,
	is_sunset,
	is_irregular_time,
	block_status,
	book_status,
	agent_cd,
	sub_agent_cd,
	c_num,
	g_last,
	g_last_kanji,
	is_finalized
)
SELECT 
	church_cd =w.req_church_cd,
	block_date = w.req_wed_date,
	start_time = convert(datetime, convert(char(5), w.req_wed_time, 108)),
	start_time_s = convert(char(5), w.req_wed_time, 108), 
	is_sunset = 0,
	is_irregular_time = 1,
	block_status = '*BKD*',
	s.book_status,
	s.agent_cd,
	s.sub_agent_cd,
	s.c_num,
	c.g_last,
	c.g_last_kanji,
	is_finalized = CASE WHEN c.final_date IS NOT NULL THEN 1 ELSE 0 END
FROM sales s 
	INNER JOIN wed_info w ON (s.op_seq = w.op_seq) 
	INNER JOIN customer c ON (c.c_num = s.c_num)
WHERE (w.is_irregular_time = 1)
	AND (s.book_status IN ('K', 'X'))
	AND (w.req_wed_date >= @start_date)
	AND (w.req_wed_date <= @end_date)
	AND ((@is_photo = 1) OR (@church_cd IS NULL) OR (w.req_church_cd = @church_cd))	-- 挙式プランはchurch_cdで絞る。
	AND (@is_photo <> 1 OR (s.item_type = 'PHP'))					-- フォトプランはitem_typeで絞る。
	AND NOT EXISTS(select * from #t t where t.c_num = s.c_num 
										and t.church_cd = w.req_church_cd
										and t.block_date = w.req_wed_date
										and t.start_time_s = convert(char(5), w.req_wed_time, 108)
				   )



--***** Pick Up時間を取得
IF @is_photo = 1 AND @location <> 'PHOTO'
BEGIN
	UPDATE #t
	SET pickup_time = DATEADD(n, 
							(SELECT TOP 1 pl.min_offset
							FROM schedule_pattern_item p
								INNER JOIN item i ON (i.item_cd= p.item_cd)
								INNER JOIN schedule_pattern_line pl ON (p.sch_pattern_id = pl.sch_pattern_id)
							WHERE (i.item_type = 'PHP')
								and (i.church_cd = @location)
								and (pl.item_type = 'TRN')
								AND EXISTS(select * from item_price pr 
										   where (pr.item_cd = i.item_cd AND pr.agent_cd = @agent_cd
												AND @start_date >= pr.eff_from AND @start_date <= pr.eff_to
												 )
										  )
							ORDER BY pl.min_offset
							)
						, #t.start_time)

	UPDATE #t
	SET	pickup_time_s = convert(char(5), pickup_time, 108)
	WHERE (pickup_time IS NOT NULL)
END

-- is_finalizedがNULLのレコードを0に更新。
UPDATE #t SET is_finalized = isnull(is_finalized, 0)

--***** 結果を返す。
SELECT * FROM #t ORDER BY block_date, start_time, church_cd


GO

GRANT EXECUTE ON [dbo].[usp_get_church_blocks] TO [public] AS [dbo]
GO


