DROP PROCEDURE [dbo].[usp_c_get_church_avail]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO




CREATE PROCEDURE [dbo].[usp_c_get_church_avail]
						@date_from_p	char(8),
						@date_to_p	char(8),
						@item_cd	char(15) = null,
						@church_cd	char(5) = null,
						@item_type	varchar(3) = null,
						@area_cd	char(3) = null,
						@rtn_mode	smallint = 0


	AS 
	SET NOCOUNT ON
	
	declare	@date_from	datetime, 
			@date_to	datetime,
			@tmp_date	datetime,
			@tmp_time	datetime,
			@tmp_range	smallint,
			@tmp_seq	int,
			@rtn_val	int,
			@loop_from	datetime,
			@loop_to	datetime,
			@order_date	datetime,
			@exists_pkg_church bit,
			@close_sunday bit	

	select @date_from = CONVERT(datetime, @date_from_p,1)
	select @date_to = CONVERT(datetime, @date_to_p,1)
	select @tmp_date = @date_from, @loop_from = null, @loop_to = null

	if @item_cd is null
		return
		
	while @tmp_date <= @date_to
		begin
			if exists	(	select	*  
							  from	item i inner join item_price p on (i.item_cd = p.item_cd)
								 inner join c_item c on (i.item_cd = c.item_cd)
							 where	i.item_cd = @item_cd
							   and	c.open_to_cust = 1
							   and	@tmp_date between eff_from and eff_to
						)
				begin
					if @loop_from is null
						select	@loop_from = @tmp_date, @loop_to = @tmp_date
					else
						select	@loop_to = @tmp_date
				end
	
			select @tmp_date = dateadd(day, 1, @tmp_date)
		end

	if (@loop_from is null) or (@loop_to is null)
		return
	else
		select	@date_from = @loop_from, @date_to = @loop_to
		
	if @church_cd is null
	begin
		select	@church_cd = church_cd 
		  from	item 
		 where	item_cd = @item_cd
		   
		if @church_cd is null
			return
	end

	--select @close_sunday = isnull(close_sunday,0) from church where church_cd = @church_cd

	CREATE TABLE #tmp_wt_get_church_avail_by_day
	(
		church_cd	char(5)		null,
		block_date	datetime	null,
		[status]	nvarchar(4) null	
	)

	--CREATE TABLE #tmp_wt_get_church_avail
	--(
	--	church_cd	char(5)		null,
	--	block_date	datetime	null,
	--	[day]		varchar(3)	null,
	--	range_seq	smallint	null,
	--	start_time	datetime	null,
	--	agent_cd	char(6)		null,
	--	[seq]		int			null,
	--	[status]	nvarchar(4) null	
	--)

	CREATE TABLE #tmp_wt_get_church_avail (
		church_cd		char(5) NOT NULL,
		block_date		datetime NOT NULL,
		[day]		varchar(3)	null,
		start_time		datetime NOT NULL,
		agent_cd		char(6) NULL,
		sub_agent_cd	char(6) NULL,
		block_status	char(6)	NULL,
		book_status		char(1) NULL
	)


	insert into #tmp_wt_get_church_avail
		(
			church_cd,
			block_date,
			[day],
			start_time,
			agent_cd,
			sub_agent_cd,
			block_status,
			book_status
		)
	exec usp_get_church_blocks	@church_cd, @date_from, @date_to, @church_cd

	select @tmp_date = @date_from

	while @tmp_date <= @date_to
		begin
			declare by_time_cursor cursor for

			select start_time from church_time where church_cd = @church_cd order by start_time					

			open by_time_cursor
			fetch next from by_time_cursor into @tmp_time

			while @@fetch_status = 0
			begin
				if not exists(select * from #tmp_wt_get_church_avail where block_date = @tmp_date and start_time = @tmp_time)
					begin
						--if @close_sunday = 1 and datename(dw, @tmp_date) = 'Sunday'
						--	insert #tmp_wt_get_church_avail values(@church_cd, @tmp_date, substring(datename(weekday, @tmp_date),1,3), @tmp_time, null, null, 'C', 'C')
						--else
						--	insert #tmp_wt_get_church_avail values(@church_cd, @tmp_date, substring(datename(weekday, @tmp_date),1,3), @tmp_time, null, null, 'U', 'U')
							insert #tmp_wt_get_church_avail values(@church_cd, @tmp_date, substring(datename(weekday, @tmp_date),1,3), @tmp_time, null, null, 'U', 'U')
					end
				--else 
				--	begin
				--		if exists(select * from #tmp_wt_get_church_avail where block_date = @tmp_date and range_seq = @tmp_range and status = 'A') and @item_cd is not null and @church_cd is not null
				--		begin
				--			select	@rtn_val = 0
						
				--			exec @rtn_val = usp_getpkgoptionstatus 'XOXOXOX', @church_cd, @tmp_date, @tmp_time, @item_cd, 'WEBAG'
						
				--			if @rtn_val >= 10
				--				update #tmp_wt_get_church_avail set status = 'C' where church_cd = @church_cd and block_date = @tmp_date and range_seq = @tmp_range
				--		end
				--		else
				--		begin
				--			if @close_sunday = 1 and datename(dw, @tmp_date) = 'Sunday'
				--				update #tmp_wt_get_church_avail set status = 'C' where church_cd = @church_cd and block_date = @tmp_date and range_seq = @tmp_range
				--		end
				--	end

				--if @close_sunday = 1 and datename(dw, @tmp_date) = 'Sunday'
				--	update #tmp_wt_get_church_avail set status = 'C' where church_cd = @church_cd and block_date = @tmp_date and range_seq = @tmp_range
				
				fetch next from by_time_cursor into @tmp_range, @tmp_time, @tmp_seq
			end

			deallocate by_time_cursor
			
			select @tmp_date = dateadd(day, 1, @tmp_date)
		end
		
	select @tmp_date = @date_from

	--if exists(select p.*
	--				from pkg_item p 
	--					inner join church_tariff t on (p.item_cd = t.item_cd)
	--					inner join church c on (c.church_cd = t.church_cd)
	--				where p.pkg_cd = @trf_item_cd)
	--	begin
	--		select @exists_pkg_church = 1
	--	end

	if @rtn_mode = 1
		select 	church_cd,
			block_date,
			start_time,
			status = case
					when [block_status] is null then '〇'
					else '×'
				 end
		  from 	#tmp_wt_get_church_avail
		 order by block_date, seq
	else
		begin
			CREATE TABLE #a1(
				item_cd		varchar(15)
			)
			select @order_date = dbo.fnc_getJapanDate()
			while @tmp_date <= @date_to
				begin

					DELETE FROM #a1
					INSERT INTO #a1(
						item_cd
					)
					EXEC usp_c_get_item_list null, null, @item_cd, null, 1, 1, null, @church_cd, @tmp_date

					if not exists(
						select *
							from	#a1 p)
								insert #tmp_wt_get_church_avail_by_day values(@church_cd, @tmp_date, '×')
					--else if @exists_pkg_church = 1 and not exists(
					--	select p.*
					--		from pkg_item p 
					--			inner join church_tariff t on (p.item_cd = t.item_cd)
					--			inner join church c on (c.church_cd = t.church_cd)
					--		where p.pkg_cd = @trf_item_cd and
					--			(c.open_month = 0 or
					--			c.open_month = 12 or
					--			c.open_month >= 18 or
					--			DATEADD(month, c.open_month, Convert(NVARCHAR, dbo.fnc_getdate(), 111)) >= @tmp_date))
					--			insert #tmp_wt_get_church_avail_by_day values(@church_cd, @tmp_date, '×')
					else
						begin
							if exists(select * from #tmp_wt_get_church_avail where block_date = @tmp_date and [block_status] is null)
								insert #tmp_wt_get_church_avail_by_day values(@church_cd, @tmp_date, '〇')
							else 
							begin
								insert #tmp_wt_get_church_avail_by_day values(@church_cd, @tmp_date, '×')
								--if exists(select * from #tmp_wt_get_church_avail where block_date = @tmp_date and status = 'U')
								--	insert #tmp_wt_get_church_avail_by_day values(@church_cd, @tmp_date, '△')
								--else
								--	insert #tmp_wt_get_church_avail_by_day values(@church_cd, @tmp_date, '×')
							end
						end
					select @tmp_date = dateadd(day, 1, @tmp_date)
				end

			select	* from #tmp_wt_get_church_avail_by_day
		end


	return






GO


