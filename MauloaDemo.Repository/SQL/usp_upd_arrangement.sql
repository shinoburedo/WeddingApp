IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_upd_arrangement]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_upd_arrangement]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_upd_arrangement]	@arrangement_id 	int,
					@vendor_cd 		char(8), 
					@cfmd 			bit,
					@cfmd_by 		nvarchar(100) 	= null,	
					@cfmd_date 		datetime 	= null,
					@cxl 			bit,			
					@cxl_vend_by 	nvarchar(100) 	= null,
					@cxl_date 		datetime 	= null,	
					@note 			nvarchar(max) 	= null,
					@last_person 	varchar(15)



        AS 
        SET NOCOUNT ON


	declare	@op_seq		int,
		@c_num		char(7),
		@wed_date	datetime,
		@church_cd	char(5),
		@quantity	smallint,
		@item_cd	char(15),
		@cost		decimal(10,2),
		@a_cost		decimal(10,2)



	select	@op_seq = op_seq, @c_num = c_num, @a_cost = (cost/quantity) from arrangement where arrangement_id = @arrangement_id

	select 	@quantity = quantity, @item_cd = item_cd from sales where op_seq = @op_seq

	select	@wed_date = wed_date, @church_cd = church_cd from customer where c_num = @c_num

	if not exists(select * from arrangement where arrangement_id = @arrangement_id and vendor_cd = @vendor_cd)
		exec	usp_find_cost 	@item_cd,
					@wed_date,
					@vendor_cd,
					@church_cd,
					@a_cost		output

	select	@cost = @a_cost * @quantity


	update	arrangement 
	   set 	vendor_cd	= @vendor_cd,
		cfmd		= @cfmd,
		cfmd_by		= @cfmd_by,
		cfmd_date	= @cfmd_date,
		cxl		= @cxl,
		cxl_vend_by	= @cxl_vend_by,
		cxl_date	= @cxl_date,
		note		= @note,
		cost		= @cost,
		quantity	= @quantity,
		last_person	= @last_person, 
		update_date	= getutcdate()
	where arrangement_id = @arrangement_id


	return(0)



GO


