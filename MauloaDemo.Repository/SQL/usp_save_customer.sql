IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_save_customer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_save_customer]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_save_customer] (
		  @c_num				char(7)
		, @sub_agent_cd			char(6)
		, @g_last				varchar(20)
		, @g_first				varchar(20)
		, @b_last				varchar(20)
		, @b_first				varchar(20)
		, @g_last_kana			nvarchar(20)
		, @g_first_kana			nvarchar(20)
		, @b_last_kana			nvarchar(20)
		, @b_first_kana			nvarchar(20)
		, @g_last_kanji			nvarchar(20)
		, @g_first_kanji		nvarchar(20)
		, @b_last_kanji			nvarchar(20)
		, @b_first_kanji		nvarchar(20)

		, @area_cd				char(3)
		, @tour_cd				nvarchar(20)
		, @church_cd			char(5)
		, @wed_date				datetime
		, @wed_time				datetime
		, @htl_pick				datetime

		, @bf_date				datetime
		, @bf_time				datetime
		, @bf_place				nvarchar(100)
		, @ft_date				datetime
		, @ft_time				datetime
		, @ft_place				nvarchar(100)

		, @in_flight			varchar(10)
		, @in_dep				char(3)
		, @in_dep_date			datetime
		, @in_dep_time			datetime
		, @in_arr				char(3)
		, @in_arr_date			datetime
		, @in_arr_time			datetime

		, @out_flight			varchar(10)
		, @out_dep				char(3)
		, @out_dep_date			datetime
		, @out_dep_time			datetime
		, @out_arr				char(3)
		, @out_arr_date			datetime
		, @out_arr_time			datetime

		, @hotel_cd				char(5)
		, @room_number			varchar(10)
		, @checkin_date			datetime
		, @checkout_date		datetime

		, @note					nvarchar(MAX)
		, @staff_note			nvarchar(MAX)

		, @attend_count			smallint
		, @attend_name			nvarchar(100)
		, @attend_memo			nvarchar(MAX)

		, @last_person			varchar(15)
) AS	

DECLARE @int_c_num int, @utc_now datetime, @agent_cd char(6)

SET @int_c_num = NULL
SET @utc_now = getutcdate()

-- sub_agent_cdからagent_cdを取得
SELECT @agent_cd = isnull(parent_cd, child_cd) 
FROM agent_parent WHERE (child_cd = @sub_agent_cd)

IF @agent_cd IS NULL
BEGIN
	  RAISERROR ('Agent code is invalid.', 16, 1);
END

IF @area_cd IS NULL OR NOT EXISTS(select * from area where area_cd = @area_cd)
BEGIN
	  RAISERROR ('Area code is invalid.', 16, 1);
END

--IF @church_cd IS NOT NULL AND NOT EXISTS(select * from church where church_cd = @church_cd)
--BEGIN
--	  RAISERROR ('Church code is invalid.', 16, 1);
--END

IF @hotel_cd IS NOT NULL AND NOT EXISTS(select * from hotel where hotel_cd = @hotel_cd)
BEGIN
	  RAISERROR ('Hotel code is invalid.', 16, 1);
END

	

IF @c_num IS NULL
BEGIN
	-- カスタマー番号を採番してcustomerテーブルに新規追加。(基本項目のみ)
	EXEC @int_c_num = usp_create_customer 
						  @sub_agent_cd
						, @g_last
						, @g_first
						, @b_last
						, @b_first
						, @g_last_kana
						, @g_first_kana
						, @b_last_kana
						, @b_first_kana
						, @g_last_kanji
						, @g_first_kanji
						, @b_last_kanji
						, @b_first_kanji
						, @area_cd
						, @last_person

	SET @c_num = convert(char(7), @int_c_num)
END 
ELSE BEGIN
	-- customerテーブルを更新。(基本項目のみ)
	UPDATE customer SET
			  @int_c_num = convert(int, @c_num)		-- 見つかった場合のみ戻り値を返す。
			, agent_cd = @agent_cd
			, sub_agent_cd = @sub_agent_cd
			, g_last = @g_last
			, g_first = @g_first	
			, b_last = @b_last	
			, b_first= @b_first	
			, g_last_kana = @g_last_kana	
			, g_first_kana = @g_first_kana
			, b_last_kana = @b_last_kana			
			, b_first_kana = @b_first_kana
			, g_last_kanji = @g_last_kanji
			, g_first_kanji = @g_first_kanji
			, b_last_kanji = @b_last_kanji
			, b_first_kanji = @b_first_kanji
			, area_cd = @area_cd
			, wed_date = @wed_date
			, wed_time = @wed_time
			, last_person = @last_person
			, update_date = @utc_now
	WHERE (c_num = @c_num)
END


IF @c_num IS NOT NULL 
BEGIN
	-- customerテーブルを更新。(基本項目以外)
	UPDATE customer SET
			tour_cd = @tour_cd
			, htl_pick = @htl_pick
			, bf_date = @bf_date
			, bf_time = @bf_time
			, bf_place = @bf_place
			, ft_date = @ft_date
			, ft_time = @ft_time
			, ft_place = @ft_place
			, in_flight = @in_flight
			, in_dep = @in_dep
			, in_dep_date = @in_dep_date
			, in_dep_time = @in_dep_time
			, in_arr = @in_arr
			, in_arr_date = @in_arr_date
			, in_arr_time = @in_arr_time
			, out_flight = @out_flight
			, out_dep = @out_dep
			, out_dep_date = @out_dep_date
			, out_dep_time = @out_dep_time
			, out_arr = @out_arr
			, out_arr_date = @out_arr_date
			, out_arr_time = @out_arr_time
			, hotel_cd = @hotel_cd
			, room_number = @room_number
			, checkin_date = @checkin_date
			, checkout_date = @checkout_date
			, note = @note
			, staff_note = @staff_note
			, attend_count = @attend_count
			, attend_name = @attend_name
			, attend_memo = @attend_memo
	WHERE (c_num = @c_num)
END


-- 戻り値としてカスタマー番号を整数値で返す。
RETURN @int_c_num

GO


