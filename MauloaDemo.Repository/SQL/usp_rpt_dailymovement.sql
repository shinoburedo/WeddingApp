IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_rpt_dailymovement]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_rpt_dailymovement]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE  proc [dbo].[usp_rpt_dailymovement]
@wed_date varchar(8) = null, @church_cd varchar(10)= null, 
@agent_cd varchar(10) = null
as
		
	select @church_cd = rtrim(@church_cd)
	select @agent_cd = rtrim(@agent_cd)

	select 
	    c.c_num,
		c.church_cd,
		c.wed_date,
		wed_time= CONVERT(char(5),c.wed_time,8),
		htl_pick = CONVERT(char(5),c.htl_pick,8), 
		c.agent_cd,
		gname = g_last + ', ' +g_first, 
		bname = b_last + ', ' +b_first, 
		c.hotel_cd,
		c.room_number, 
		c.checkin_date,
		c.checkout_date,
		c.attend_count,
		pkg_name = (select top 1 i.item_name from item i, sales s 
			where i.item_cd = s.item_cd and 
				s.c_num = c.c_num and 
				s.book_status IN ('K', 'Q') and
				i.item_type in ('PKG', 'PHP')),
		s.item_type,
		s.item_cd,
		s.op_seq,
		i.item_name,
		s.quantity,
		a.vendor_cd,
		c.note
		from customer c
			 INNER JOIN sales s ON (c.c_num = s.c_num)
			 INNER JOIN item i ON (s.item_cd = i.item_cd)
			 LEFT JOIN arrangement a ON (s.op_seq = a.op_seq)
		where wed_date = @wed_date
			and (@agent_cd is null or c.agent_cd = @agent_cd)
			and (@church_cd is null or c.church_cd like @church_cd)
			and s.book_status IN ('K', 'Q')
			and a.cxl = 0
		order by wed_time,
		  c_num,
		  case s.item_type 
		  when 'PKG' then 1 
		  when 'PHP' then 2
		  else 3 end,
		  s.item_type,
		  s.item_cd
		
		
return(0)
		
/**************************************************************************************************************************/

/****** Object:  StoredProcedure [dbo].[usp_rptEmpSalesByItem]    Script Date: 05/03/2016 14:02:18 ******/
SET ANSI_NULLS ON



GO


