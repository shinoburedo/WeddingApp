IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_save_church_avail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_save_church_avail]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[usp_save_church_avail]
@church_cd char(5) = null, @block_date datetime = null,
@block char(6) = null,    @start_time datetime = null,	@last_person  char(6) = null
AS

	SET NOCOUNT OFF

	if not exists(select * from church_time
			where church_cd = @church_cd
			  and start_time = @start_time)
			return(0)


	--if exists(select * from church_block 
	--	       where church_cd = @church_cd
	--		 and block_date = @block_date
 -- 			 and range_seq = @range_seq
	--		 )
	--	if not exists(select * from church_block 
	--	       where church_cd = @church_cd
	--		 and block_date = @block_date
 -- 			 and range_seq = @range_seq
	--		 and booked = @booked)
	--	return(0)

	if  exists(select * from church_block where church_cd = @church_cd 
		   and start_time = @start_time and block_date = @block_date)
		begin 

		if  @block is null or @block = ''
		begin 

			delete from church_block 
			where church_cd = @church_cd 
			   and start_time = @start_time 
			   and block_date = @block_date
		end
		else
		begin 
			update church_block
			   set 
				block_status = @block,
				last_person = @last_person,
				update_date = getutcdate()
			where church_cd = @church_cd 
			   and start_time = @start_time 
			   and block_date = @block_date
		end
		end
	else
		begin 
		if  @block is not null and @block <> ''
			begin 
			Insert into church_block
			values(@church_cd,@block_date, @start_time,  @block, @last_person, getutcdate())
			end
		end
	
	return (0)


GO


