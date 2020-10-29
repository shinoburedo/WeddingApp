IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_get_options]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_get_options]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_get_options] (
				@c_num	char(7),
				@agent_cd char(6) = null
) AS 

/*
	Example: 
		EXEC usp_get_options '10080', null
		EXEC usp_get_options '10080', 'HISJ'
*/

SELECT @agent_cd = nullif(@agent_cd, '')


SELECT 
	s.op_seq,
	s.c_num,
	s.item_type,
	s.item_cd,
	s.agent_cd, 
	s.sub_agent_cd, 
	s.staff,
	s.quantity,
	s.note,
	s.parent_op_seq,
	s.upgrade_op_seq,
	s.cust_collect,
	s.tentative_price,
	s.org_price,
	s.price,
	s.amount,
	s.price_changed,
	s.price_change_reason,
	s.book_status,
	s.book_proc_date,
	s.book_proc_by,
	s.jpn_cfm,
	s.jpn_cfm_date,
	s.jpn_cfm_by,
	s.cxl_charge,
	s.inv_seq,
	s.sales_post_date, 
	s.jnl_started,
	s.create_by,
	s.create_date,
	s.last_person,
	s.update_date,
	i.item_name,
	i.item_name_jpn,
	t.info_type,
	isnull(s.parent_op_seq, s.op_seq) AS group_op_seq
FROM sales s
	INNER JOIN item i ON (i.item_cd = s.item_cd)
	INNER JOIN item_type t ON (i.item_type = t.item_type)
	LEFT JOIN sales p ON (p.op_seq = s.parent_op_seq)
WHERE (s.c_num = @c_num)
	AND (s.agent_cd = @agent_cd OR @agent_cd IS NULL)
	AND (s.item_type NOT IN ('PKG', 'PHP'))
	AND (p.op_seq IS NULL OR p.item_type NOT IN ('PKG','PHP'))
ORDER BY isnull(s.parent_op_seq, s.op_seq), s.parent_op_seq, s.op_seq


RETURN
GO


