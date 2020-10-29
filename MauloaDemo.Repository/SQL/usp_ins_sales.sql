IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ins_sales]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ins_sales]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_ins_sales](
					@c_num 			char(7), 	
					@item_cd 		char(15),
					@cust_collect 		bit			= 0,	
					@agent_cd		char(6),
					@sub_agent_cd		char(6),
					@inv_agent		char(6)			= null,
					@staff			nvarchar(50),
					@branch_staff	nvarchar(50)	= null,
					@quantity		smallint,
					@book_status		char(1),
					@price_date		datetime,
					@parent_op_seq		int			= null,
					@last_person 		varchar(15)
	) AS 

    SET NOCOUNT ON

	DECLARE	@gross 			decimal(10,2), 
		@d_net 			decimal(10,2), 
		@y_net 			int, 
		@tentative 		bit, 
		@price 			decimal(10,2), 
		@item_type 		char(3),
		@book_proc_date		datetime,
		@book_proc_by		varchar(15),
		@tmp_seq		int,
		@plan_kind		char(1)

	SELECT @inv_agent = nullif(@inv_agent, '')

	if not exists(select * from item where item_cd = @item_cd and discon_date is null)
	begin
		if exists(select * from item where item_cd = @item_cd and discon_date < @price_date)
			return(-1)
	end
	
	select  @item_type = item_type from item where item_cd = @item_cd


	if isnull(@parent_op_seq, 0) <> 0
		select	@price = 0.00, @tentative = 0
	else
		begin
			select	@plan_kind = null

			if @item_type not in ('PKG','PHP')
				begin
					if exists(select * from sales where c_num = @c_num and item_type = 'PKG' and book_status = 'K')
						select	@plan_kind = 'W'
					
					if @plan_kind = null and exists(select * from sales where c_num = @c_num and item_type = 'PHP' and book_status = 'K')
						select	@plan_kind = 'P'
				end

			execute usp_find_price 	@item_cd,
						@price_date,
						@agent_cd,
						@plan_kind,
						@gross		output,
						@d_net		output,
						@y_net		output,
						@tentative	output

			if @gross is null and @d_net is null and @y_net is null
				select 	@price = 0.00
			else begin
				select @price = isnull(case when @cust_collect = 1 then @gross else @d_net end, 0)
			end
		end


	if @book_status = 'K'
		select	@book_proc_date = getutcdate(), @book_proc_by = @last_person
	else
		select	@book_proc_date = null, @book_proc_by = null

	
	insert 	sales  (	
				c_num,
				item_type,
				item_cd,
				agent_cd,
				sub_agent_cd,
				inv_agent,
				staff,
				branch_staff,
				quantity,
				parent_op_seq,
				cust_collect,
				tentative_price,
				org_price,
				price,
				amount,
				book_status,
				book_proc_date,
				book_proc_by,
				sales_post_date,
				create_by,
				create_date,
				last_person,
				update_date
			)
		values 	(	
				@c_num,
				@item_type,
				@item_cd,
				@agent_cd,
				@sub_agent_cd,
				@inv_agent,
				@staff,
				@branch_staff,
				@quantity,
				@parent_op_seq,
				@cust_collect,
				@tentative,
				@price,
				@price,
				@price * @quantity,
				@book_status,
				@book_proc_date,
				@book_proc_by,
				@price_date,
				@last_person,
				getutcdate(),
				@last_person,
				getutcdate()
			)


	select @tmp_seq = scope_identity()


	if @book_status = 'K'
		exec usp_ins_arrangement @tmp_seq, @last_person


	return(@tmp_seq)


GO

GRANT EXECUTE ON [dbo].[usp_ins_sales] TO [public] AS [dbo]
GO


