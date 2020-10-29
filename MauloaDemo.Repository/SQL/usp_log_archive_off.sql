IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_log_archive_off]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_log_archive_off]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_log_archive_off](
				@log_id			int,
				@sub_agent_cd	char(6),
				@archive_by		varchar(15)
) AS 

SET NOCOUNT ON


DELETE log_change_archive 
FROM log_change_archive a 
	INNER JOIN log_change g ON (a.log_id = g.log_id) 
	INNER JOIN [login_user] u ON (u.login_id = g.login_id)
WHERE (a.log_id = @log_id)
	AND (a.sub_agent_cd = @sub_agent_cd)
	AND ( (a.archive_by = @archive_by) OR (u.sub_agent_cd <> @sub_agent_cd) )

GO


