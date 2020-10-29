DROP PROCEDURE [dbo].[usp_c_get_item_list]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_c_get_item_list]
					@language 	varchar(5),  
					@item_type	char(3),
					@item_cd varchar(15),			-- charでなくvarcharにしないとLIKE検索が出来ない。
					@item_name	nvarchar(200),
					@prepare_to_cust bit = 0,
					@open_to_cust bit = 1,
					@agent_cd char(6),
					@church_cd char(5) = null,
					@wed_date Datetime = null

					

/*
	Example: 

	 EXEC [dbo].[usp_wt_item_list] 'J', 'PCKG', 'PKG', 'LIGHT', '', '', 'HNL', 40, 1, 0 
	 EXEC [dbo].[usp_wt_item_list] 'J', 'PCKG', 'PKG', 'LIGHT', '', '', 'HNL', 40, 1, 1 
	 EXEC [dbo].[usp_wt_item_list] 'J', 'PCKG', 'PKG', 'LIGHT', 'H1C', '', 'HNL', 40, 0, 1
	 EXEC [dbo].[usp_wt_item_list] 'J', 'PCKG', 'PKG', '', '', 'アクア', 'HNL', 40, 0, 1
	 EXEC [dbo].[usp_wt_item_list] 'E', 'PCKG', 'PKG', '', '', 'aloha', 'HNL', 40, 0, 1

*/
        				
       				
AS SET NOCOUNT on

SET @language = isnull(@language, 'J')

SELECT 
	   @item_type = nullif(@item_type, ''),
	   @item_cd = nullif(@item_cd, ''),
	   @item_name = nullif(@item_name, '')

CREATE TABLE #t (
	item_type		char(3),
	item_cd			char(15),
	item_name		nvarchar(200),
	item_info	nvarchar(max),
	price_note	nvarchar(max),
	cxl_info	nvarchar(max),
	disp_order	smallint,
	open_to_cust	bit,
	movie_url		[varchar](300),
	product_detail			nvarchar(max),
	image_count			tinyint,
	image_upload_date			datetime,
	gross			decimal(10, 2) null,
	d_net			decimal(10, 2) null,
	y_net			int null,
	tentative		bit 
)
	
INSERT INTO #t
SELECT
	item_type,
	i.item_cd,
	--rest_cd,
	item_name = 
		CASE WHEN @language = 'J' THEN item_name_jpn 
		ELSE item_name
		END,
	item_info = 
		CASE WHEN @language = 'J' THEN item_info_jpn 
		ELSE item_info_eng 
		END,
	price_note = 
		CASE WHEN @language = 'J' THEN price_note_jpn 
		ELSE price_note_eng 
		END,
	cxl_info = 
		CASE WHEN @language = 'J' THEN cxl_info_jpn 
		ELSE cxl_info_eng 
		END,
	--jpn_collect,
	--jpn_display,
	disp_order,
	--reg_add,
	open_to_cust,
	movie_url,
	product_detail = 
		CASE WHEN @language = 'J' THEN product_detail_jpn 
		ELSE product_detail_eng 
		END,
	--last_person,
	--update_date,
	image_count,
	image_upload_date,
	gross = null,
	d_net = null,
	y_net = null,
	tentative = 1
FROM item i inner join c_item c on (i.item_cd = c.item_cd)
WHERE ((prepare_to_cust = 1 AND @prepare_to_cust = 1) OR (open_to_cust = 1 AND @open_to_cust = 1) )
	AND	(@item_type IS NULL OR i.item_type = @item_type)
	AND	(@item_cd IS NULL OR i.item_cd LIKE @item_cd + '%')
	AND	(@item_name IS NULL OR (i.item_name LIKE '%' + @item_name + '%' 
								 OR i.item_name_jpn LIKE '%' + @item_name + '%'))

	AND (@church_cd IS NULL OR
			i.church_cd = @church_cd
		)

	AND (@wed_date IS NULL OR
			(exists(select *
						from	item_price p 
						where p.agent_cd = @agent_cd
							and p.item_cd = i.item_cd
							and (p.eff_from <= @wed_date AND p.eff_to >= @wed_date)
				)
			)
		)

	AND (@wed_date IS NULL OR
			(i.discon_date IS NULL OR i.discon_date > @wed_date)
		)


DECLARE @tmp_item_cd char(15), @gross decimal(10,2), @d_net decimal(10,2), @y_net int, @tentative bit

-- レコード毎にループして挙式日、エージェント等に応じた価格を取得。
DECLARE mycursor CURSOR FOR 
SELECT item_cd FROM #t;
OPEN mycursor;
FETCH NEXT FROM mycursor INTO @tmp_item_cd;

WHILE @@FETCH_STATUS = 0
BEGIN
	SELECT @gross= null, @d_net = null, @y_net = null, @tentative = 1;

	EXEC usp_find_price @tmp_item_cd, @wed_date, @agent_cd, null,
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
	SELECT * FROM #t 
	ORDER BY disp_order, item_cd




RETURN







GO


