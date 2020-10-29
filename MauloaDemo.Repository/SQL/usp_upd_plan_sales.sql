DROP PROCEDURE [dbo].[usp_upd_plan_sales]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[usp_upd_plan_sales](
					@op_seq			int,
					@agent_cd		char(6),
					@sub_agent_cd		char(6),
					@inv_agent		char(6)			= null,
					@staff			nvarchar(50),
					@branch_staff	nvarchar(50)	= null,
					@book_status		char(1),
					@note			nvarchar(max)		= null,
					@last_person 		varchar(15),
					@return_mess 		varchar(10) 		= null output,
					@with_children	bit = 0
	) 
AS 

SET NOCOUNT ON 


	SELECT @inv_agent = nullif(@inv_agent, '')


	select	@return_mess = 'invalid'

	if @op_seq < 1
		return(0)

	select	@return_mess = 'ok'

	--==================================================
	-- book_statusが変更されていない場合
	if exists(select * from sales where op_seq = @op_seq and book_status = @book_status)
	begin
		update	wed_info 
			set	note = @note,
			last_person = @last_person,
			update_date = getutcdate()
			where	op_seq = @op_seq
			
		update	sales
			set	agent_cd = @agent_cd,
			sub_agent_cd = @sub_agent_cd,
			inv_agent = @inv_agent,
			staff		= @staff,
			branch_staff	= @branch_staff,
			cust_collect	= CASE WHEN @inv_agent IS NULL THEN 1 ELSE 0 END,
			last_person = @last_person,
			update_date = getutcdate()
			where	op_seq = @op_seq

		return(0)
	end

	--==================================================
	-- booking_statusが変更されている場合のみ下記を実行。

	declare	@status		char(1), 
		@church_cd	char(5),
		@wed_date	datetime,
		@wed_time	datetime,
		@pickup		datetime,
		@pickup_min	smallint,
		@c_num		char(7),
		@item_type  char(3)

	select	@return_mess = 'status'

	select	@c_num = c_num, @status = book_status, @agent_cd = agent_cd, @item_type = item_type 
	from sales where op_seq = @op_seq
	
	-- 現在のステータスと変更後のステータスを比較してルールに合致しない場合は処理を中止する。
	if (@status = 'K' and @book_status not in ('X', 'C')) or (@book_status = 'K' and @status <> 'Q')
		return(0)

	select	@return_mess = 'ok', @pickup = null

	select	@church_cd = req_church_cd, @wed_date = req_wed_date, @wed_time = req_wed_time 
	from wed_info where op_seq = @op_seq

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


	update	sales
	   set	book_status	= @book_status,
		book_proc_date	= getutcdate(),
		book_proc_by	= @last_person,
		agent_cd 	= @agent_cd,
		sub_agent_cd 	= @sub_agent_cd,
		staff		= @staff,
		branch_staff	= @branch_staff,
		inv_agent 	= @inv_agent,
		cust_collect	= CASE WHEN @inv_agent IS NULL THEN 1 ELSE 0 END,
		last_person	= @last_person,
		update_date	= getutcdate()
	 where	op_seq = @op_seq


	update	wed_info 
	   set	note = @note,
		last_person = @last_person,
		update_date = getutcdate()
	 where	op_seq = @op_seq


	-- キャンセルされた場合
	if @book_status = 'C'
	begin
		-- プラン自身のarragementをキャンセル。
		update	arrangement
		 	set	cxl		= 1,
			cxl_vend_by	= @last_person,
			cxl_date	= getutcdate(),
			last_person	= @last_person,
			update_date	= getutcdate()
			where	op_seq = @op_seq

		IF @with_children = 1 
		BEGIN
			-- プランに含まれる商品のarrangementをキャンセル。
			update	arrangement
			set cxl			= 1,
				cxl_vend_by	= @last_person,
				cxl_date	= getutcdate(),
				last_person	= @last_person,
				update_date	= getutcdate()
			  from sales s inner join arrangement a on (s.op_seq = a.op_seq) 
			 where (s.parent_op_seq = @op_seq)

			-- プランに含まれる商品のsalesをキャンセル。
			update sales
			set	book_status	= @book_status,
				last_person		= @last_person,
				update_date		= getutcdate()
			where (parent_op_seq = @op_seq)

			-- プランに含まれる商品の孫の代のarrangementをキャンセル。
			update	arrangement
			set cxl			= 1,
				cxl_vend_by	= @last_person,
				cxl_date	= getutcdate(),
				last_person	= @last_person,
				update_date	= getutcdate()
			  from sales s inner join arrangement a on (s.op_seq = a.op_seq) 
			where (parent_op_seq in (select op_seq from sales where parent_op_seq = @op_seq))

			-- プランに含まれる商品の孫の代のsalesをキャンセル。
			update sales
			set	book_status	= @book_status,
				last_person		= @last_person,
				update_date		= getutcdate()
			where (parent_op_seq in (select op_seq from sales where parent_op_seq = @op_seq))
		END


		-- customerの挙式情報をクリア。
		update	customer
		set	church_cd 	= null,
			wed_date	= null,
			wed_time	= null,
			htl_pick	= null,
			last_person	= @last_person,
			update_date	= getutcdate()
		where	(c_num = @c_num)
			AND (wed_date = @wed_date)
			AND (wed_time = @wed_time)
			AND (church_cd = @church_cd)

			-- 同じ教会・日付・時刻で別のOK申込みがある場合はクリアしない。
			AND NOT EXISTS(select * from wed_info w inner join sales s on(w.op_seq = s.op_seq) 
						   where w.c_num = @c_num 
							 and w.op_seq <> @op_seq 
							 and w.req_church_cd = @church_cd
							 and w.req_wed_date = @wed_date
							 and w.req_wed_time = @wed_time
							 and s.book_status = 'K')
	end


	return(0)

GO

GRANT EXECUTE ON [dbo].[usp_upd_plan_sales] TO [public] AS [dbo]
GO



