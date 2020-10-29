IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_find_cost]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_find_cost]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_find_cost] 	@item_cd 	char(15),
				@wed_date	datetime,
				@vendor_cd	char(8),
				@church_cd	char(5),
				@cost		decimal(10,2)	output


        AS 
        SET NOCOUNT ON


	select	@cost = null


	select 	@cost = cost  
	  from 	item_cost
	 where 	item_cd = @item_cd
	   and 	vendor_cd = @vendor_cd
	   and	church_cd = @church_cd
	   and 	@wed_date between eff_from and eff_to

			
	if @cost is null
		select 	@cost = cost  
		  from 	item_cost
		 where 	item_cd = @item_cd
		   and 	vendor_cd = @vendor_cd
		   and	church_cd = '***'
		   and 	@wed_date between eff_from and eff_to


	if @cost is null
		select 	@cost = 0.00			


	return

GO

GRANT EXECUTE ON [dbo].[usp_find_cost] TO [public] AS [dbo]
GO


