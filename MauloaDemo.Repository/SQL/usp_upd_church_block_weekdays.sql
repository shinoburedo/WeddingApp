IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_upd_church_block_weekdays]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_upd_church_block_weekdays]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_upd_church_block_weekdays]	@start_date	datetime,
						@end_date	datetime


	AS SET NOCOUNT ON

	declare @tmp_date	datetime,
		@church_cd	char(5),
		@start_time	datetime,
		@bom_date	datetime,
		@3rd_sat	datetime

	select	@tmp_date = @start_date


	while @tmp_date <= @end_date
		begin
			select 	@bom_date = dateadd(day, day(@tmp_date) * - 1 + 1, @tmp_date)

			select 	@3rd_sat = dateadd(dd, 21 - datepart(dw, @bom_date), @bom_date)


        		DECLARE time_cursor CURSOR FOR

			select c.church_cd, t.start_time from church c, church_time t where c.church_cd = t.church_cd and c.plan_kind = 'W' order by c.church_cd, t.start_time

			OPEN time_cursor
			FETCH NEXT FROM time_cursor INTO @church_cd, @start_time

			WHILE @@fetch_status = 0
				begin
					if @church_cd = 'CALVY' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'fri' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '14:00', 8)) < 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
						 	 and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'CUCS' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sat' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '11:00', 8)) > 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'CUCS' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sun' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '13:00', 8)) > 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'CUCA' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sun' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '14:00', 8)) > 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'KAWC' and lower(substring(datename(dw, @tmp_date), 1, 3)) not in ('sat','sun') and ((datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '10:00', 8)) = 0) or (datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '13:00', 8)) < 0))
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'KAWC' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sat' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '12:00', 8)) < 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'KAWC' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sun' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '13:00', 8)) > 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'NUCC' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sun' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '15:00', 8)) > 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'MOCC' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sat' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '15:00', 8)) = 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'MOCC' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sun' 
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'HLNC' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'wed' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '15:00', 8)) = 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'HLNC' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sun' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '15:00', 8)) > 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'STCLE' and lower(substring(datename(dw, @tmp_date), 1, 3)) in ('mon', 'tue', 'wed', 'fri') 
					   and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '13:00', 8)) <= 0
					   and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '15:00', 8)) >= 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'STCLE' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'wed' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '11:00', 8)) >= 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'STCLE' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'thu' and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '13:00', 8)) <= 0
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'STCLE' and lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sat' and (
					   ((datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '10:00', 8)) < 0 and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '11:00', 8)) > 0)) or
					   ((datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '11:00', 8)) < 0 and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '12:00', 8)) > 0)) or
					   ((datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '12:00', 8)) < 0 and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '13:00', 8)) > 0)) or
					   ((datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '15:00', 8)) < 0 and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '16:00', 8)) > 0)) or
					   ((datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '16:00', 8)) < 0 and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '17:00', 8)) > 0)))
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					if @church_cd = 'STCLE' 
					   and ((lower(substring(datename(dw, @tmp_date), 1, 3)) = 'sun') or (datediff(dd, @3rd_sat, @tmp_date) = 0)) 
					   and (
					   ((datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '14:00', 8)) > 0)) or
					   ((datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '15:00', 8)) < 0 and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '16:00', 8)) > 0)) or
					   ((datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '16:00', 8)) < 0 and datediff(mi, convert(varchar(5), @start_time, 8), convert(varchar(5), '17:00', 8)) > 0)))
					   and not exists(select * from church_block where church_cd = @church_cd and datediff(dd, convert(varchar(8), block_date, 1), convert(varchar(8), @tmp_date, 1)) = 0
							  and datediff(mi, convert(varchar(5), start_time, 8), convert(varchar(5), @start_time, 8)) = 0 and block_status = 'X')
						begin
							insert church_block (church_cd, block_date, start_time, block_status, last_person)		
							values (@church_cd, @tmp_date, @start_time, 'X', 'kazu')
						end


					FETCH NEXT FROM time_cursor INTO @church_cd, @start_time
				end

			DEALLOCATE time_cursor

			select	@tmp_date = dateadd(dd, 1, @tmp_date)

		end


	return


GO

GRANT EXECUTE ON [dbo].[usp_upd_church_block_weekdays] TO [public] AS [dbo]
GO


