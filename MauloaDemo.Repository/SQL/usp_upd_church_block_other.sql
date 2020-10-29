IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_upd_church_block_other]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_upd_church_block_other]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_upd_church_block_other]	@start_date	datetime,
						@end_date	datetime


	AS SET NOCOUNT ON

	declare @tmp_date	datetime,
		@start_time	datetime

	select	@tmp_date = @start_date


	while @tmp_date <= @end_date
		begin
        		DECLARE time_cursor CURSOR FOR

			select start_time from church_time where church_cd = 'OTHER' order by start_time

			OPEN time_cursor
			FETCH NEXT FROM time_cursor INTO @start_time

			WHILE @@fetch_status = 0
				begin
					if datediff(mi, convert(varchar(8), @start_time, 8), convert(varchar(8), '10:00', 8)) = 0
						insert church_block (church_cd, block_date, start_time, block_status, last_person)		
						values ('OTHER', @tmp_date, @start_time, 'X', 'kazu')

					if datediff(mi, convert(varchar(8), @start_time, 8), convert(varchar(8), '12:00', 8)) = 0
						insert church_block (church_cd, block_date, start_time, block_status, last_person)		
						values ('OTHER', @tmp_date, @start_time, 'X', 'kazu')

					if datediff(mi, convert(varchar(8), @start_time, 8), convert(varchar(8), '15:00', 8)) = 0
						insert church_block (church_cd, block_date, start_time, block_status, last_person)		
						values ('OTHER', @tmp_date, @start_time, 'X', 'kazu')

					if datediff(mi, convert(varchar(8), @start_time, 8), convert(varchar(8), '16:00', 8)) = 0
						insert church_block (church_cd, block_date, start_time, block_status, last_person)		
						values ('OTHER', @tmp_date, @start_time, 'X', 'kazu')


					FETCH NEXT FROM time_cursor INTO @start_time
				end

			DEALLOCATE time_cursor

			select	@tmp_date = dateadd(dd, 1, @tmp_date)

		end


	return


GO

GRANT EXECUTE ON [dbo].[usp_upd_church_block_other] TO [public] AS [dbo]
GO


