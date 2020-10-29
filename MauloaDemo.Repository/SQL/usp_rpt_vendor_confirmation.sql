IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_rpt_vendor_confirmation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_rpt_vendor_confirmation]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_rpt_vendor_confirmation](
				@vendor_cd			char(8),
				@date_from		datetime,
				@date_to		datetime
) AS 

/*
	EXAMPLE: 
		EXEC usp_rpt_vendor_confirmation 'ISSEI', '10/01/2014', '10/31/2014'
*/



SET NOCOUNT ON

-- 空文字列のパラメータをNULLに変換。
select @vendor_cd = nullif(@vendor_cd, '')

create table #t (
	c_num			char(7),
	g_last			varchar(20),
    g_first			varchar(20)		null,
    b_last			varchar(20)		null,
    b_first			varchar(20)		null,
    wed_date		datetime		null,
    wed_time		datetime		null,

	hotel_cd		char(5)			null,
	hotel_name		varchar(100)			null,

	church_cd		char(5)			null,
	church_name		varchar(100)			null,

	op_seq			int				null,
	item_type		char(3)			null,
	info_type		char(3)			null,
	note			nvarchar(max)			null,

	vendor_cd		char(8)			null,
	vendor_name		varchar(100)			null,
	op_tel		varchar(100)			null,
	op_fax		varchar(100)			null,

	item_cd			char(15)		null,
	item_name		varchar(100)	null,

	delivery_date	datetime		null,
	delivery_time	datetime		null,
	delivery_place	nvarchar(100)		null,
	delivery_note	nvarchar(max)		null,

	make_date	datetime		null,
	make_time	datetime		null,
	make_place	nvarchar(100)		null,
	make_in_time	datetime		null,
	make_note	nvarchar(max)		null,

	party_date	datetime		null,
	party_time	datetime		null,
	rest_cd	nvarchar(100)		null,
	rcp_note	nvarchar(max)		null,

	shoot_date	datetime		null,
	shoot_time	datetime		null,
	shoot_place	nvarchar(100)		null,
	shoot_note	nvarchar(max)		null,

	pickup_date	datetime		null,
	pickup_time	datetime		null,
	pickup_place	nvarchar(100)		null,
	dropoff_time	datetime		null,
	dropoff_place	nvarchar(100)		null,
	pickup_hotel	char(3)		null,
	dropoff_hotel	char(3)		null,
	trans_note	nvarchar(max)		null

)


-- 念のため日付パラメータから時刻部分を除去。
SELECT @date_from = convert(datetime, convert(varchar(10), @date_from, 111))
SELECT @date_to = convert(datetime, convert(varchar(10), @date_to, 111))

-- To日付は「<」で比較するため1日加算する。
SELECT @date_to = DATEADD(dd, 1, @date_to)

INSERT INTO #t
SELECT 
	c.c_num,
	c.g_last,
	c.g_first,
	c.b_last,
	c.b_first,
	c.wed_date,
	c.wed_time,
	c.hotel_cd,
	h.hotel_name,
	c.church_cd,
	ch.church_name,
	a.op_seq,
	s.item_type,
	item_type.info_type,
	s.note,
	a.vendor_cd,
	v.vendor_name,
	v.op_tel,
	v.op_fax,
	s.item_cd,
	i.item_name,
	delivery_info.delivery_date,
	delivery_info.delivery_time,
	delivery_info.delivery_place,
	delivery_info.note as delivery_note,
	make_info.make_date,
	make_info.make_time,
	make_info.make_place,
	make_info.make_in_time,
	make_info.note as make_note,
	rcp_info.party_date,
	rcp_info.party_time,
	rcp_info.rest_cd,
	rcp_info.note as rcp_note,
	shoot_info.shoot_date,
	shoot_info.shoot_time,
	shoot_info.shoot_place,
	shoot_info.note as shoot_note,
	trans_info.pickup_date,
	trans_info.pickup_time,
	trans_info.pickup_place,
	trans_info.dropoff_time,
	trans_info.dropoff_place,
	trans_info.pickup_hotel as pickup_hotel,
	trans_info.dropoff_hotel as dropoff_hotel,
	trans_info.note as trans_note
FROM [arrangement] a
	INNER JOIN [sales]s ON (a.op_seq = s.op_seq) 
	INNER JOIN [customer] c ON (s.c_num = c.c_num) 
	INNER JOIN [item] i ON (i.item_cd = s.item_cd)
	INNER JOIN [item_type] ON (s.item_type = item_type.item_type)
	INNER JOIN [vendor] v ON (a.vendor_cd = v.vendor_cd)
	LEFT JOIN [hotel] h ON (c.hotel_cd = h.hotel_cd)
	LEFT JOIN [church] ch ON (c.church_cd = ch.church_cd)
	LEFT JOIN [delivery_info] ON (a.op_seq = delivery_info.op_seq)
	LEFT JOIN [make_info] ON (a.op_seq = make_info.op_seq)
	LEFT JOIN [rcp_info] ON (a.op_seq = rcp_info.op_seq)
	LEFT JOIN [shoot_info] ON (a.op_seq = shoot_info.op_seq)
	LEFT JOIN [trans_info] ON (a.op_seq = trans_info.op_seq)
WHERE ( (a.vendor_cd = @vendor_cd) OR (@vendor_cd IS NULL))	
	AND (s.book_status = 'k')
	AND (s.sales_post_date >= @date_from AND s.sales_post_date < @date_to)



-- 結果を返す。
IF @vendor_cd IS NULL
	SELECT * FROM #t ORDER BY vendor_cd, info_type, wed_date, wed_time, c_num
ELSE
	SELECT * FROM #t ORDER BY info_type, wed_date, wed_time, c_num




GO


