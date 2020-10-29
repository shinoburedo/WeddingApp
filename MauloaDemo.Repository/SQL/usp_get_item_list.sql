IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_get_item_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_get_item_list]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_get_item_list](
				@plan_type		char(1),			-- 'W'=挙式プラン、'P'=フォトプラン、'O'=オプション、それ以外=0件を返す。
				@church_cd		char(5),			
				@item_type		char(3), 
				@item_cd		varchar(15),		-- charではなくvarcharにしないとLIKE検索が出来ない。
				@item_name		nvarchar(200), 
				@wed_date		datetime, 
				@agent_cd		char(6), 
				@plan_kind		char(1),			-- オプションの価格取得対象のプラン区分。'W'=挙式プランあり、'P'=フォトプランあり、null=プラン無し.
				@open_to_japan_only bit = 0
) AS 

SET NOCOUNT ON

/*
	Examples: 
		EXEC usp_get_item_list 'O', null, null, null, null, '06/15/2014', 'HIS', 'P', 1
		EXEC usp_get_item_list 'O', null, null, null, null, '05/30/2014', 'HIS', 'W', 1
		EXEC usp_get_item_list 'O', null, 'ALB', null, null, '06/15/2014', 'HIS', 'P', 1
		EXEC usp_get_item_list 'O', null, 'ALB', null, null, '05/30/2014', 'HIS', 'W', 1
		EXEC usp_get_item_list 'O', null, null, 'F', null, '06/15/2014', 'HIS', 'P', 1
		EXEC usp_get_item_list 'O', null, null, null, 'アルル', '06/15/2014', 'HIS', 'P', 1
		EXEC usp_get_item_list 'P', 'ALAMO', null, null, null, '10/15/2014', 'HIS', 'P', 1
		EXEC usp_get_item_list 'P', null, null, null, null, '05/30/2014', 'HIS', null, 1
		EXEC usp_get_item_list 'W', null, null, null, null, '05/30/2014', 'JTB', null, 1
		EXEC usp_get_item_list 'P', null, null, null, null, '05/30/2014', 'JTB', null, 1
		EXEC usp_get_item_list null, null, null, null, null, '05/30/2014', 'HIS', null, 1
*/

-- 空文字列のパラメータをNULLに変換。
SELECT @plan_type = nullif(@plan_type, ''), 
	   @item_type = nullif(@item_type, ''), 
	   @item_cd = nullif(@item_cd, ''), 
	   @item_name = nullif(@item_name, ''), 
	   @agent_cd = nullif(@agent_cd, ''), 
	   @plan_kind = nullif(@plan_kind, '')

CREATE TABLE #t (
	info_type		char(3),
	item_type		char(3),
	church_cd		char(5),
	item_cd			char(15),
	item_name		varchar(100),
	item_name_jpn	nvarchar(200),
	rq_default		bit,
	abbrev			char(5),
	gross			decimal(10, 2) null,
	d_net			decimal(10, 2) null,
	y_net			int null,
	tentative		bit 
)

INSERT INTO #t
SELECT 
	t.info_type,
	i.item_type,
	i.church_cd,
	i.item_cd,
	i.item_name,
	i.item_name_jpn,
	i.rq_default,
	i.abbrev,
	gross = null,
	d_net = null,
	y_net = null,
	tentative = 1
FROM [item] i
	INNER JOIN [item_type] t ON (i.item_type = t.item_type)
WHERE ((i.item_type = 'PKG' AND @plan_type = 'W') 
		OR (i.item_type = 'PHP' AND @plan_type = 'P')  
		OR (i.item_type NOT IN ('PKG', 'PHP') AND @plan_type = 'O')
		OR (i.item_type = '-' AND @plan_type NOT IN ('W', 'P', 'O'))
	  )
	AND ( (i.church_cd = @church_cd) OR (@church_cd IS NULL) )
	AND ( (i.item_type = @item_type) OR (@item_type IS NULL) )
	AND ( (i.item_cd LIKE @item_cd + '%') OR (@item_cd IS NULL) ) 
	AND ( (i.item_name LIKE '%' + @item_name + '%') OR (i.item_name_jpn LIKE '%' + @item_name + '%') OR (@item_name IS NULL) ) 
	AND ( (i.discon_date IS NULL OR i.discon_date > @wed_date) OR (@wed_date IS NULL) )
	AND ( (i.open_to_japan = 1 OR @open_to_japan_only = 0))


-- @agent_cdで絞り込む。
IF (@agent_cd IS NOT NULL)	
BEGIN
	DELETE #t WHERE ( NOT EXISTS(select * from item_price p where (p.item_cd = #t.item_cd) and (p.agent_cd = @agent_cd)) )
END

-- @plan_kindで絞り込む。
IF @plan_kind IS NOT NULL
BEGIN
	DELETE #t WHERE ( NOT EXISTS(select * from item_price p where (p.item_cd = #t.item_cd) and (p.agent_cd = @agent_cd) and (p.plan_kind = @plan_kind OR p.plan_kind IS NULL)) )
END 
--*** 2015/09/22 Mike. コメントアウト。オプションオンリーの場合にplan_kindが'P'または'W'のレコードがある場合はそれを適用するため。
--ELSE BEGIN
	--DELETE #t WHERE ( NOT EXISTS(select * from item_price p where (p.item_cd = #t.item_cd) and (p.agent_cd = @agent_cd) and (p.plan_kind IS NULL)) )
--END


DECLARE @tmp_item_cd char(15), @gross decimal(10,2), @d_net decimal(10,2), @y_net int, @tentative bit

-- レコード毎にループして挙式日、エージェント等に応じた価格を取得。
DECLARE mycursor CURSOR FOR 
SELECT item_cd FROM #t;
OPEN mycursor;
FETCH NEXT FROM mycursor INTO @tmp_item_cd;

WHILE @@FETCH_STATUS = 0
BEGIN
	SELECT @gross= null, @d_net = null, @y_net = null, @tentative = 1;

	EXEC usp_find_price @tmp_item_cd, @wed_date, @agent_cd, @plan_kind,
			@gross output, 
			@d_net output, 
			@y_net output, 
			@tentative output

	UPDATE #t SET gross = @gross, 
				  d_net = @d_net,
				  y_net = @y_net,
				  tentative = @tentative
	WHERE (item_cd = @tmp_item_cd)

   FETCH NEXT FROM mycursor INTO @tmp_item_cd;
END

CLOSE mycursor;
DEALLOCATE mycursor;

-- 結果を返す。
IF @plan_type IN ('W', 'P')
BEGIN
	SELECT * FROM #t 
	ORDER BY item_type, item_cd
END ELSE BEGIN
	SELECT * FROM #t 
	ORDER BY item_type, church_cd, item_cd
END


GO


