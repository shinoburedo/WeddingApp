IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_find_price]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_find_price]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_find_price] 	
					@item_cd 	char(15),
					@wed_date	datetime,
					@agent_cd	char(6),
					@plan_kind	char(1)		= null,	

					@gross		decimal(10,2)		output,
					@d_net		decimal(10,2)		output,
					@y_net		int 			output,
					@tentative 	bit			output

AS 
SET NOCOUNT ON

	SELECT @plan_kind = nullif(@plan_kind, '')

	DECLARE @temp_agent	char(6)

	SELECT 	@temp_agent = CASE
					WHEN agent_fit = 'F' THEN '*FIT'
					WHEN agent_fit = 'L' THEN '*AFF'
					WHEN agent_fit = 'O' THEN '*OWN'
					WHEN agent_fit = 'D' THEN '*DEST'
					WHEN agent_fit = 'A' THEN '*AGT'
					ELSE @agent_cd
				END
	  FROM 	agent
	 WHERE 	(agent_cd = @agent_cd)


	SELECT 	TOP 1 @gross = gross, @d_net = d_net, @y_net = y_net, @tentative = 0  
		FROM 	item_price 
		WHERE 	(item_cd = @item_cd)
		AND 	(agent_cd = @agent_cd)
		AND		((plan_kind IS NULL) OR (plan_kind = @plan_kind) OR (@plan_kind IS NULL))
		AND 	(@wed_date BETWEEN eff_FROM AND eff_to)
	ORDER BY  case when (plan_kind = @plan_kind or (plan_kind is null and @plan_kind is null)) then 0 else 1 end, plan_kind DESC, eff_to DESC

			
	IF @d_net IS NULL AND @gross IS NULL AND @y_net IS NULL
		BEGIN
			SELECT 	TOP 1 @gross = gross, @d_net = d_net, @y_net = y_net, @tentative = 0  
				FROM 	item_price 
				WHERE 	(item_cd = @item_cd)
				AND 	(agent_cd = @temp_agent)
				AND		((plan_kind IS NULL) OR (plan_kind = @plan_kind) OR (@plan_kind IS NULL))
				AND 	(@wed_date BETWEEN eff_FROM AND eff_to)
			ORDER BY  case when (plan_kind = @plan_kind or (plan_kind is null and @plan_kind is null)) then 0 else 1 end, plan_kind DESC, eff_to DESC
		END


	IF @d_net IS NULL AND @gross IS NULL AND @y_net IS NULL
		BEGIN
			SELECT 	TOP 1 @gross = gross, @d_net = d_net, @y_net = y_net, @tentative = 1
				FROM 	item_price 
				WHERE 	(item_cd = @item_cd)
				AND 	(agent_cd = @agent_cd)
				AND		((plan_kind IS NULL) OR (plan_kind = @plan_kind) OR (@plan_kind IS NULL))
			ORDER BY  case when (plan_kind = @plan_kind or (plan_kind is null and @plan_kind is null)) then 0 else 1 end, plan_kind DESC, eff_to DESC
		END


	IF @d_net IS NULL AND @gross IS NULL AND @y_net IS NULL
		BEGIN
			SELECT 	TOP 1 @gross = gross, @d_net = d_net, @y_net = y_net, @tentative = 1
				FROM 	item_price 
				WHERE 	(item_cd = @item_cd)
				AND 	(agent_cd = @temp_agent)
				AND		((plan_kind IS NULL) OR (plan_kind = @plan_kind) OR (@plan_kind IS NULL))
			ORDER BY  case when (plan_kind = @plan_kind or (plan_kind is null and @plan_kind is null)) then 0 else 1 end, plan_kind DESC, eff_to DESC
		END


	IF @d_net IS NULL AND @gross IS NULL AND @y_net IS NULL
		SELECT 	@d_net = 0.00, @gross = 0.00, @y_net = 0, @tentative = 1			


	return

GO

GRANT EXECUTE ON [dbo].[usp_find_price] TO [public] AS [dbo]
GO


