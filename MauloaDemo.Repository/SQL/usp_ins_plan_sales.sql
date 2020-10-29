IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ins_plan_sales]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ins_plan_sales]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_ins_plan_sales]	
					@c_num 			char(7), 	
					@item_cd 		char(15),
					@cust_collect 		bit			= 0,	
					@agent_cd		char(6),
					@sub_agent_cd		char(6),
					@inv_agent		char(6)			= null,
					@church_cd		char(5),
					@wed_date		datetime,
					@wed_time		datetime,
					@staff			nvarchar(50),
					@branch_staff	nvarchar(50)	= null,
					@book_status		char(1),
					@note			nvarchar(max)		= null,
					@last_person 		varchar(15),
					@return_mess 		varchar(10) 		= null output,
					@is_irregular_time	bit = 0

AS 
SET NOCOUNT ON 

DECLARE	@op_seq		int,
	@pickup		datetime,
	@pickup_min	smallint,
	@item_type char(3),
	@plan_kind char(1)

SELECT @item_type = item_type FROM item WHERE item_cd = @item_cd
SELECT @plan_kind = plan_kind FROM church WHERE church_cd = @church_cd


	select	@return_mess = 'ok', @pickup = null


	if @book_status = 'K'
	begin
		IF @item_type = 'PHP'
		BEGIN
			-- フォトプランの場合
			-- 全てのフォトプラン&同じ日時で既にOK予約が入っていればRQにする。
			if exists(select * from customer c inner join church h on (c.church_cd = h.church_cd)
					  where (h.plan_kind = 'P' OR h.plan_kind IS NULL)
						and (c.wed_date = @wed_date) 
						and (convert(varchar(5), c.wed_time, 8) = convert(varchar(5), @wed_time, 8))
					 )
			begin
				select	@return_mess = 'booked', @book_status = 'Q'


				/* この10時枠2件までOKという仕様は、一旦無しで立上げスタートする。

				-- ただし10時枠については2件までOK可能とする。
				IF convert(varchar(5), @wed_time, 8) = '10:00'
				BEGIN
					if (select count(*) from customer c inner join church h on (c.church_cd = h.church_cd)
 					   	 where (h.plan_kind = 'P' OR h.plan_kind IS NULL)
   					 	   and (c.wed_date = @wed_date)
   					 	   and (convert(varchar(5), c.wed_time, 8) = '10:00')
						 ) < 2
					begin
						select	@return_mess = 'ok', @book_status = 'K'
					end
				END

				*/
			end
		ELSE 
			-- 挙式プランの場合
			-- 同じ教会&日時で既にOK予約が入っていればRQにする。
			if exists(select * from customer 
					  where (church_cd = @church_cd) 
						and (wed_date = @wed_date) 
						and (convert(varchar(5), wed_time, 8) = convert(varchar(5), @wed_time, 8))
					  )
			begin
				select	@return_mess = 'booked', @book_status = 'Q'
			end
		END


		if @book_status = 'K'
		begin
			-- ブロックがClosedの場合はRQにする。
			if exists(select * from church_block where church_cd = @church_cd and block_date = @wed_date and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @wed_time, 8)) = 0 and block_status = 'X')
				select	@return_mess = 'closed', @book_status = 'Q'	
		end

		if @book_status = 'K'
		begin
			-- ブロックが他のエージェント用に確保されている場合はRQにする。
			if exists(select * from church_block where church_cd = @church_cd and block_date = @wed_date and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @wed_time, 8)) = 0 and block_status is not null and block_status <> @agent_cd)
				select	@return_mess = 'agent', @book_status = 'Q'	
		end

		if @book_status = 'K'
		begin
			-- 教会マスターからピックアップ時刻を計算。
			select	@pickup_min = default_pickup from church where church_cd = @church_cd
			if @pickup_min < 0
				select	@pickup = dateadd(mi, @pickup_min, @wed_time)

			-- OKステータスで保存する場合はカスタマーの情報も更新する。
			update	customer
				set	church_cd 	= @church_cd,
				wed_date	= @wed_date,
				wed_time	= @wed_time,
				htl_pick	= @pickup,
				last_person	= @last_person,
				update_date	= getutcdate()
				where	c_num = @c_num	
		end
	end

	-- salesにレコードを挿入。
	exec	@op_seq = usp_ins_sales	@c_num, 
					@item_cd, 
					@cust_collect, 	
					@agent_cd,
					@sub_agent_cd,
					@inv_agent,
					@staff,
					@branch_staff,
					1,
					@book_status,
					@wed_date,
					null,		-- parent_op_seq
					@last_person

	-- wed_infoにレコードを挿入。
	insert 	wed_info(	
			op_seq,
			c_num,
			req_church_cd,
			req_wed_date,
			req_wed_time,
			is_irregular_time,
			note,
			create_by,
			create_date,
			last_person,
			update_date
	) values (	
			@op_seq,
			@c_num,
			@church_cd,
			@wed_date,
			@wed_time,
			@is_irregular_time,
			@note,
			@last_person,
			getutcdate(),
			@last_person,
			getutcdate()
	)


	return(@op_seq)



GO


