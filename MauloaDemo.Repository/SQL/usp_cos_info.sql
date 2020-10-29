IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_cos_info]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_cos_info]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_cos_info]	@info_id		int		= 0,
					@c_num			char(7),
					@pax_type		char(1),
					@height			nvarchar(15)	= null,
					@chest			nvarchar(15)	= null,
					@waist			nvarchar(15)	= null,
					@cloth_size		nvarchar(15)	= null,
					@shoe_size		nvarchar(15)	= null,
					@note			nvarchar(max)	= null,
					@last_person 		varchar(15)


        AS 
        SET NOCOUNT ON

	declare	@tmp_seq	int


	if @info_id = 0
		begin
			insert	cos_info
				(
					c_num,
					pax_type,
					height,
					chest,
					waist,
					cloth_size,
					shoe_size,
					note,
					create_by,
					create_date,
					last_person,
					update_date
				)
			values
				(
					@c_num,
					@pax_type,
					@height,
					@chest,
					@waist,
					@cloth_size,
					@shoe_size,
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
		update	cos_info
		   set	pax_type		= @pax_type,
			height			= @height,
			chest			= @chest,
			waist			= @waist,
			cloth_size		= @cloth_size,
			shoe_size		= @shoe_size,
			note			= @note,
			last_person		= @last_person,
			update_date		= getutcdate()
		 where	info_id = @info_id


	return(0)


GO


