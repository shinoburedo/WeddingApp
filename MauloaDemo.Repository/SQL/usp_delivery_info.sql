IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_delivery_info]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_delivery_info]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_delivery_info]	@info_id		int		= 0,
					@op_seq			int,
					@delivery_date		datetime	= null,
					@delivery_time		datetime	= null,
					@delivery_place		nvarchar(100)	= null,
					@note			nvarchar(max)	= null,
					@last_person 		varchar(15)


        AS 
        SET NOCOUNT ON

	DECLARE	@c_num		char(7),
		@tmp_seq	int

	select	@c_num = c_num from sales where op_seq = @op_seq


	if @info_id = 0
		begin
			insert	delivery_info
				(
					op_seq,
					c_num,
					delivery_date,
					delivery_time,
					delivery_place,
					note,
					create_by,
					create_date,
					last_person,
					update_date
				)
			values
				(
					@op_seq,
					@c_num,
					@delivery_date,
					@delivery_time,
					@delivery_place,
					@note,
					@last_person,
					getutcdate(),
					@last_person,
					getutcdate()
				)

			select @tmp_seq = scope_identity()

			return(@tmp_seq)
		end
	else
		update	delivery_info
		   set	delivery_date		= @delivery_date,
			delivery_time		= @delivery_time,
			delivery_place		= @delivery_place,
			note			= @note,
			last_person		= @last_person,
			update_date		= getutcdate()
		 where	info_id = @info_id


	return(0)


GO

GRANT EXECUTE ON [dbo].[usp_delivery_info] TO [public] AS [dbo]
GO


