IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_trans_info]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_trans_info]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_trans_info]	@info_id		int		= 0,
					@op_seq			int,
					@pickup_date		datetime	= null,
					@pickup_time		datetime	= null,
					@pickup_hotel		char(3)	= null,
					@pickup_place		nvarchar(100)	= null,
					@dropoff_time		datetime	= null,
					@dropoff_hotel		char(3)	= null,
					@dropoff_place		nvarchar(100)	= null,
					@note			nvarchar(max)	= null,
					@last_person 		varchar(15)


        AS 
        SET NOCOUNT ON

	DECLARE	@c_num		char(7),
		@tmp_seq	int

	select	@c_num = c_num from sales where op_seq = @op_seq


	if @info_id = 0
		begin
			insert	trans_info
				(
					op_seq,
					c_num,
					pickup_date,
					pickup_time,
					pickup_hotel,
					pickup_place,
					dropoff_time,
					dropoff_hotel,
					dropoff_place,
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
					@pickup_date,
					@pickup_time,
					@pickup_hotel,
					@pickup_place,
					@dropoff_time,
					@dropoff_hotel,
					@dropoff_place,
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
		update	trans_info
		   set	pickup_date		= @pickup_date,
			pickup_time		= @pickup_time,
			pickup_hotel		= @pickup_hotel,
			pickup_place		= @pickup_place,
			dropoff_time		= @dropoff_time,
			dropoff_hotel		= @dropoff_hotel,
			dropoff_place		= @dropoff_place,
			note			= @note,
			last_person		= @last_person,
			update_date		= getutcdate()
		 where	info_id = @info_id


	return(0)


GO

GRANT EXECUTE ON [dbo].[usp_trans_info] TO [public] AS [dbo]
GO
