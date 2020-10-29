IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_upd_church_block_holiday]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_upd_church_block_holiday]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_upd_church_block_holiday]	@church_cd	char(5),
						@block_date	datetime



	AS SET NOCOUNT ON

	declare @start_time	datetime


        DECLARE time_cursor CURSOR FOR

	select start_time from church_time where church_cd = @church_cd order by start_time

        OPEN time_cursor
	FETCH NEXT FROM time_cursor INTO @start_time

        WHILE @@fetch_status = 0
		begin
			insert church_block (church_cd, block_date, start_time, block_status, last_person)
			values (@church_cd, @block_date, @start_time, 'X', 'kazu')


			FETCH NEXT FROM time_cursor INTO @start_time
	    	end

        DEALLOCATE time_cursor


	return


GO

GRANT EXECUTE ON [dbo].[usp_upd_church_block_holiday] TO [public] AS [dbo]
GO


