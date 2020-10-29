IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_rpt_finalinfo_sheet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_rpt_finalinfo_sheet]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_rpt_finalinfo_sheet]( 
						@c_num char(7),
						@english bit = 0,
						@agent_cd char(6) = null
					)
AS 
SET NOCOUNT ON

/*
	Examples: 
		EXEC usp_rpt_finalinfo_sheet '10080', 0, null
		EXEC usp_rpt_finalinfo_sheet '10080', 0, 'HISJ'
		EXEC usp_rpt_finalinfo_sheet '10080', 1, null
		EXEC usp_rpt_finalinfo_sheet '10080', 1, 'HISJ'
*/


SELECT @agent_cd = nullif(@agent_cd, '')

CREATE TABLE #t (
	c_num			char(7),
	cust_name		nvarchar(50) null,
	cust_name_kanji	nvarchar(50) null,
	cust_phone		nvarchar(20) null,

	wed_date		datetime null,
	wed_time		datetime null,
	church_cd		char(5)	null,
	church_name		nvarchar(100) null,
	hotel_cd		char(5) null,
	hotel_name		nvarchar(100) null,

	plan_op_seq		int	null,
	plan_item_type	char(3) null,
	plan_item_cd	char(15) null,
	plan_item_name	nvarchar(200) null,
	plan_date		datetime null,
	plan_time		datetime null,

	op_seq			int,
	is_option		bit,
	info_date		datetime null,
	info_time		datetime null,
	item_type		char(3),
	item_cd			char(15) null,
	item_name		nvarchar(200) null,
	quantity		smallint,
	vendor_cd		char(8) null,
	vendor_name		nvarchar(100) null,
	note			nvarchar(max) null
)

INSERT INTO #t
SELECT 
	@c_num AS c_num,
	dbo.fnc_getCustName(@c_num, 0) AS cust_name,
	dbo.fnc_getCustName(@c_num, 1) AS cust_name_jpn,

	(SELECT TOP 1 isnull(a.cell_tel, a.home_tel) 
	 FROM address_info a 
	 WHERE (a.c_num = c.c_num) AND (a.cell_tel IS NOT NULL OR a.home_tel IS NOT NULL) 
	 ORDER BY info_id DESC) AS cust_phone,

	c.wed_date,
	c.wed_time,
	c.church_cd,
	(SELECT TOP 1 CASE WHEN @english = 1 THEN ch.church_name ELSE isnull(ch.church_name_jpn, ch.church_name) END 
	 FROM [church] ch
	 WHERE (ch.church_cd = c.church_cd)) AS church_name,

	c.hotel_cd,
	(SELECT TOP 1 CASE WHEN @english = 1 THEN h.hotel_name ELSE isnull(h.hotel_name_jpn, h.hotel_name) END 
	 FROM [hotel] h
	 WHERE (h.hotel_cd = c.hotel_cd)) AS hotel_name,

	s2.op_seq AS plan_op_seq,
	s2.item_type AS plan_item_type,
	s2.item_cd AS plan_item_cd,
	'' AS plan_item_name,
	null AS plan_date,
	null AS plan_time,

	s.op_seq,
	CASE WHEN s.item_type NOT IN ('PKG', 'PHP') AND isnull(s2.item_type, '') NOT IN ('PKG', 'PHP') THEN 1 ELSE 0 END AS is_option,
	null AS info_date,
	null AS info_time,
	s.item_type,
	s.item_cd,
	(SELECT TOP 1 CASE WHEN @english = 1 THEN i.item_name ELSE isnull(i.item_name_jpn, i.item_name) END 
	 FROM [item] i
	 WHERE (i.item_cd = s.item_cd)) AS item_name,
	s.quantity,
	
	'' AS vendor_cd, 
	'' AS vendor_name,
	s.note 
	
FROM [sales] s
	INNER JOIN [customer] c ON (s.c_num = c.c_num)
	LEFT JOIN [sales] s2 ON (s2.op_seq = s.parent_op_seq AND s2.item_type IN ('PKG', 'PHP'))
WHERE (s.c_num = @c_num)
	AND (s.agent_cd = @agent_cd OR @agent_cd IS NULL)
	AND (s.book_status = 'K')
	AND (s.item_type NOT IN ('PKG', 'PHP'))


-- プランの名称をセット。
UPDATE #t
SET plan_item_name = CASE WHEN @english = 1 THEN i.item_name ELSE isnull(i.item_name_jpn, i.item_name) END
FROM #t INNER JOIN item i ON (#t.plan_item_cd = i.item_cd)

-- プランの日時をセット。
UPDATE #t
SET plan_date = i.req_wed_date,
	plan_time = i.req_wed_time
FROM #t INNER JOIN wed_info i ON (#t.plan_op_seq = i.op_seq)


--UPDATE #t
--SET info_date = i.delivery_date, 
--	info_time = i.delivery_time
--FROM #t INNER JOIN [cos_info] i ON (#t.op_seq = i.op_seq)

UPDATE #t
SET info_date = i.delivery_date, 
	info_time = i.delivery_time,
	note = i.note
FROM #t INNER JOIN [delivery_info] i ON (#t.op_seq = i.op_seq)

UPDATE #t
SET info_date = i.make_date, 
	info_time = i.make_time,
	note = i.note
FROM #t INNER JOIN [make_info] i ON (#t.op_seq = i.op_seq)

UPDATE #t
SET info_date = i.party_date, 
	info_time = i.party_time,
	note = i.note
FROM #t INNER JOIN [rcp_info] i ON (#t.op_seq = i.op_seq)

UPDATE #t
SET info_date = i.shoot_date, 
	info_time = i.shoot_time,
	note = i.note
FROM #t INNER JOIN [shoot_info] i ON (#t.op_seq = i.op_seq)

UPDATE #t
SET info_date = i.pickup_date, 
	info_time = i.pickup_time,
	note = i.note
FROM #t INNER JOIN [trans_info] i ON (#t.op_seq = i.op_seq)


-- 結果を返す。
SELECT * FROM #t 
ORDER BY is_option, plan_op_seq, item_type, info_date, info_time, item_cd


GO


