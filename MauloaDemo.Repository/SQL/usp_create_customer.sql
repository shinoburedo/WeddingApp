IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_create_customer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_create_customer]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_create_customer] (
		@sub_agent_cd			char(6)
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
		, @create_by			varchar(15)
) AS	

DECLARE @c_num char(7), @int_c_num int, @utc_now datetime, @agent_cd char(6)

SET @utc_now = getutcdate()

-- sub_agent_cdからagent_cdを取得
SELECT @agent_cd = isnull(parent_cd, child_cd) 
FROM agent_parent WHERE child_cd = @sub_agent_cd


-- 新しいカスタマー番号を採番
SELECT @int_c_num = next_number FROM [key_number] WHERE (data_key = 'c_num')
SET @c_num = convert(char(7), @int_c_num)
UPDATE [key_number] SET next_number = @int_c_num + 1 WHERE (data_key = 'c_num')

-- customerテーブルに挿入。
INSERT INTO customer (
		  c_num
		, agent_cd				
		, sub_agent_cd			
		, g_last				
		, g_first				
		, b_last				
		, b_first				
		, g_last_kana			
		, g_first_kana			
		, b_last_kana			
		, b_first_kana			
		, g_last_kanji			
		, g_first_kanji		
		, b_last_kanji			
		, b_first_kanji		
		, area_cd	
		, create_date			
		, create_by			
		, last_person
		, update_date
) VALUES (
		  @c_num
		, @agent_cd				
		, @sub_agent_cd			
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
		, @utc_now
		, @create_by			
		, @create_by
		, @utc_now
)

-- 戻り値としてカスタマー番号を整数値で返す。
RETURN @int_c_num

GO


