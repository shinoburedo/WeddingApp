IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ins_arrangement]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ins_arrangement]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_ins_arrangement]	@op_seq		int,
					@last_person 	char(6)


        AS 
        SET NOCOUNT ON

	declare	@wed_date 	datetime, 
		@church_cd 	char(5),
		@c_num 		char(7),
		@jnl_started	bit,
		@quantity	smallint,
		@item_cd	char(15),
		@vendor_cd	char(8),
		@cost		decimal(10,2)


	select	@c_num = c_num, @item_cd = item_cd, @jnl_started = jnl_started, @quantity = quantity from sales where op_seq = @op_seq

	select	@wed_date = wed_date, @church_cd = church_cd from customer where c_num = @c_num

	if @wed_date is null
		return(-1)

	if @church_cd is null
		select	@church_cd = '***'


	if exists(select * from item_vendor where item_cd = @item_cd and default_vendor = 1)
		begin
		        declare vendor_cursor cursor for 

			select 	vendor_cd from item_vendor where item_cd = @item_cd and default_vendor = 1

		        open vendor_cursor 
			fetch next from vendor_cursor into @vendor_cd
		end
	else
		begin
		        declare vendor_cursor cursor for 

			select 	vendor_cd from item_vendor where item_cd = @item_cd

		        open vendor_cursor 
			fetch next from vendor_cursor into @vendor_cd
		end
	
        while @@fetch_status = 0
		begin
			select	@cost = null

			exec	usp_find_cost 	@item_cd,
						@wed_date,
						@vendor_cd,
						@church_cd,
						@cost		output

			insert	arrangement
				(
					op_seq,
					c_num,
					vendor_cd,
					quantity,
					cost,
					jnl_started,
					create_by,
					create_date,
					last_person,
					update_date
				)
			values	(
					@op_seq,
					@c_num,
					@vendor_cd,
					1,
					@cost,
					@jnl_started,
					@last_person,
					getutcdate(),
					@last_person,
					getutcdate()
				)

			fetch next from vendor_cursor into @vendor_cd

		end

        deallocate vendor_cursor    


	return(0)


GO

GRANT EXECUTE ON [dbo].[usp_ins_arrangement] TO [public] AS [dbo]
GO


