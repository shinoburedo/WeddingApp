IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_get_sales_children]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_get_sales_children]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_get_sales_children] (
				@op_seq		int,
				@item_cd	char(15)
) AS 

/*
	Example: 
		EXEC usp_get_sales_children 0, 'CELESTWDGCALVY'
		EXEC usp_get_sales_children 61, null
*/


IF @op_seq IS NULL OR @op_seq = 0
BEGIN
	SELECT 
		0 AS op_seq,
		null AS c_num,
		ch.item_type,
		ch.item_cd,
		null AS agent_cd, 
		null AS sub_agent_cd, 
		null AS staff,
		null AS branch_staff,
		convert(smallint, 1) AS quantity,
		null AS note,
		0 AS parent_op_seq,
		0 AS upgrade_op_seq,
		convert(bit, 0) AS cust_collect,
		convert(bit, 0) AS tentative_price,
		convert(decimal(10, 2), 0) AS org_price,
		convert(decimal(10, 2), 0) AS price,
		convert(decimal(10, 2), 0) AS amount,
		convert(bit, 0) AS price_changed,
		null AS price_change_reason,
		null AS book_status,
		null AS book_proc_date,
		null AS book_proc_by,
		convert(bit, 0) AS jpn_cfm,
		null AS jpn_cfm_date,
		null AS jpn_cfm_by,
		convert(decimal(10, 2), 0) AS cxl_charge,
		null AS inv_seq,
		null AS sales_post_date, 
		convert(bit, 0) AS jnl_started,
		null AS create_by,
		GETUTCDATE() AS create_date,
		null AS last_person,
		GETUTCDATE() AS update_date,
		ch.item_name,
		ch.item_name_jpn,
		t.info_type
	FROM item_option op
		INNER JOIN item ch ON (op.child_cd = ch.item_cd)
		INNER JOIN item_type t ON (ch.item_type = t.item_type)
	WHERE (op.item_cd = @item_cd)
	ORDER BY ch.item_type, ch.item_cd
END ELSE 
BEGIN
	SELECT 
		ch.op_seq,
		ch.c_num,
		ch.item_type,
		ch.item_cd,
		ch.agent_cd, 
		ch.sub_agent_cd, 
		ch.staff,
		ch.branch_staff,
		ch.quantity,
		ch.note,
		ch.parent_op_seq,
		ch.upgrade_op_seq,
		ch.cust_collect,
		ch.tentative_price,
		ch.org_price,
		ch.price,
		ch.amount,
		ch.price_changed,
		ch.price_change_reason,
		ch.book_status,
		ch.book_proc_date,
		ch.book_proc_by,
		ch.jpn_cfm,
		ch.jpn_cfm_date,
		ch.jpn_cfm_by,
		ch.cxl_charge,
		ch.inv_seq,
		ch.sales_post_date, 
		ch.jnl_started,
		ch.create_by,
		ch.create_date,
		ch.last_person,
		ch.update_date,
		i.item_name,
		i.item_name_jpn,
		t.info_type
	FROM sales s
		INNER JOIN sales ch ON (ch.parent_op_seq = s.op_seq)
		INNER JOIN item i ON (i.item_cd = ch.item_cd)
		INNER JOIN item_type t ON (i.item_type = t.item_type)
	WHERE (s.op_seq = @op_seq)
	ORDER BY ch.item_type, ch.item_cd, ch.op_seq
END


RETURN
GO


