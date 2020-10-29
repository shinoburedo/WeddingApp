IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_log_archive_on]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_log_archive_on]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_log_archive_on](
				@log_id			int,
				@sub_agent_cd	char(6),
				@archive_by		varchar(15)
) AS 

SET NOCOUNT ON


INSERT INTO log_change_archive (
	log_id,
	sub_agent_cd,
	archive_by,
	archive_datetime
) VALUES (
	@log_id,
	@sub_agent_cd,
	@archive_by,
	GETUTCDATE()
)

GO


