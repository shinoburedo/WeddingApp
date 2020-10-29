IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_rcp_info]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_rcp_info]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_rcp_info]	@info_id		int		= 0,
					@op_seq			int,
					@party_date		datetime	= null,
					@party_time		datetime	= null,
					@rest_cd		nvarchar(100)	= null,
					@note			nvarchar(max)	= null,
					@last_person 		varchar(15)


        AS 
        SET NOCOUNT ON

	DECLARE	@c_num		char(7),
		@tmp_seq	int

	select	@c_num = c_num from sales where op_seq = @op_seq


	if @info_id = 0
		begin
			insert	rcp_info
				(
					op_seq,
					c_num,
					party_date,
					party_time,
					rest_cd,
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
					@party_date,
					@party_time,
					@rest_cd,
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
		update	rcp_info
		   set	party_date		= @party_date,
			party_time		= @party_time,
			rest_cd		= @rest_cd,
			note			= @note,
			last_person		= @last_person,
			update_date		= getutcdate()
		 where	info_id = @info_id


	return(0)


GO

GRANT EXECUTE ON [dbo].[usp_rcp_info] TO [public] AS [dbo]
GO


