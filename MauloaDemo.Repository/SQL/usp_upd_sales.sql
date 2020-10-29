﻿IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_upd_sales]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_upd_sales]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[usp_upd_sales](
					@op_seq			int,
					@agent_cd		char(6),
					@sub_agent_cd		char(6),
					@inv_agent		char(6)			= null,
					@staff			nvarchar(50),
					@branch_staff	nvarchar(50)	= null,
					@price			decimal(10,2),
					@price_changed		bit,
					@price_change_reason	nvarchar(200)	= null,
					@quantity		smallint,
					@book_status		char(1),
					@cxl_charge		decimal(10,2)	= 0,
					@last_person 		varchar(15),
					@return_mess 		varchar(10) 		= null output
	) AS 

	SET NOCOUNT ON

	SELECT @inv_agent = nullif(@inv_agent, '')
	SELECT @return_mess = 'invalid'

	if @op_seq < 1
		return(0)

	declare	@status		char(1), 
		@c_num		char(7)

	select	@return_mess = 'status'

	select	@c_num = c_num, @status = book_status from sales where op_seq = @op_seq
	
	if @status <> @book_status and ((@status = 'K' and @book_status not in ('X', 'C')) or (@book_status = 'K' and @status <> 'Q'))
		return(0)

	select	@return_mess = 'ok'

	if not exists(select * from sales where op_seq = @op_seq and book_status = @book_status)
		update	sales
		   set	book_status	= @book_status,
			book_proc_date	= getutcdate(), 
			book_proc_by 	= @last_person
		 where	op_seq = @op_seq


	update	sales
	   set	staff			= @staff,
		branch_staff	= @branch_staff,
		agent_cd 		= @agent_cd,
		sub_agent_cd 		= @sub_agent_cd,
		inv_agent 		= @inv_agent,
		cust_collect	= CASE WHEN @inv_agent IS NULL THEN 1 ELSE 0 END,
		price			= @price,
		amount			= @price * @quantity,
		price_changed		= @price_changed,
		price_change_reason	= @price_change_reason,
		quantity		= @quantity,
		cxl_charge		= @cxl_charge,
		last_person		= @last_person,
		update_date		= getutcdate()
	 where	op_seq = @op_seq

	
	if @book_status = 'K' and not exists(select * from arrangement where op_seq = @op_seq)
		exec usp_ins_arrangement @op_seq, @last_person
	else
		begin
			if exists(select * from arrangement where op_seq = @op_seq)
				begin
					update	arrangement
	   				   set	quantity	= @quantity,
						cost		= (cost/quantity) * @quantity,
						last_person	= @last_person,
						update_date	= getutcdate()
					 where	op_seq = @op_seq


					--if @book_status = 'C'
					--begin
					--	update	arrangement
	 	  --				   set	cxl		= 1,
					--		cxl_vend_by	= @last_person,
					--		cxl_date	= getutcdate(),
					--		last_person	= @last_person,
					--		update_date	= getutcdate()
		 		--		 where	op_seq = @op_seq

					--	-- 子供のarrangementをキャンセル。
					--	update	arrangement
					--	set cxl			= 1,
					--		cxl_vend_by	= @last_person,
					--		cxl_date	= getutcdate(),
					--		last_person	= @last_person,
					--		update_date	= getutcdate()
					--	  from sales s inner join arrangement a on (s.op_seq = a.op_seq) 
					--	 where (s.parent_op_seq = @op_seq)

					--	-- 子供のsalesをキャンセル。
					--	update sales
					--	set	book_status	= @book_status,
					--		last_person		= @last_person,
					--		update_date		= getutcdate()
					--	where (parent_op_seq = @op_seq)

					--	-- 孫の代のarrangementをキャンセル。
					--	update	arrangement
					--	set cxl			= 1,
					--		cxl_vend_by	= @last_person,
					--		cxl_date	= getutcdate(),
					--		last_person	= @last_person,
					--		update_date	= getutcdate()
					--	  from sales s inner join arrangement a on (s.op_seq = a.op_seq) 
					--	where (parent_op_seq in (select op_seq from sales where parent_op_seq = @op_seq))

					--	-- 孫の代のsalesをキャンセル。
					--	update sales
					--	set	book_status	= @book_status,
					--		last_person		= @last_person,
					--		update_date		= getutcdate()
					--	where (parent_op_seq in (select op_seq from sales where parent_op_seq = @op_seq))
					--end
				end
		end


	return(0)



GO

GRANT EXECUTE ON [dbo].[usp_upd_sales] TO [public] AS [dbo]
GO


