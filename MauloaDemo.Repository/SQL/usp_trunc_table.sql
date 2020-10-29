IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_trunc_table]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_trunc_table]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_trunc_table]


	AS SET NOCOUNT ON

	declare @tbl_name sysname

        DECLARE type_cursor CURSOR FOR

	select name from sysobjects where type = 'U' and name <> 'sysdiagrams' order by name

        OPEN type_cursor
	FETCH NEXT FROM type_cursor INTO @tbl_name

        WHILE @@fetch_status = 0
	    begin
		if @tbl_name not in 	(
						'key_number',
						'area',
						'region',
						'agent_parent',
						'hotel',
						'login_user',
						'login_user_token',
						'holiday',
						'pickup_place',
						'Agent'
					)
			begin
				exec('truncate table ' + @tbl_name)
			end

		FETCH NEXT FROM type_cursor INTO @tbl_name
	    end

      DEALLOCATE type_cursor

	return


GO


