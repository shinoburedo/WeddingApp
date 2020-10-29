IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_make_info]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_make_info]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_make_info]	@info_id		int		= 0,
					@op_seq			int,
					@make_date		datetime	= null,
					@make_time		datetime	= null,
					@make_place		nvarchar(100)	= null,
					@make_in_time		datetime	= null,
					@note			nvarchar(max)	= null,
					@last_person 		varchar(15)


        AS 
        SET NOCOUNT ON

	DECLARE	@c_num		char(7),
		@tmp_seq	int

	select	@c_num = c_num from sales where op_seq = @op_seq


	if @info_id = 0
		begin
			insert	make_info
				(
					op_seq,
					c_num,
					make_date,
					make_time,
					make_place,
					make_in_time,
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
					@make_date,
					@make_time,
					@make_place,
					@make_in_time,
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
		update	make_info
		   set	make_date		= @make_date,
			make_time		= @make_time,
			make_place		= @make_place,
			make_in_time		= @make_in_time,
			note			= @note,
			last_person		= @last_person,
			update_date		= getutcdate()
		 where	info_id = @info_id


	return(0)


GO

GRANT EXECUTE ON [dbo].[usp_make_info] TO [public] AS [dbo]
GO


