IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_address_info]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_address_info]
GO

SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[usp_address_info]	@info_id		int		= 0,
					@c_num			char(7),
					@pax_type		char(1),
					@pax_name		nvarchar(100),
					@jpn_zip		varchar(10)	= null,
					@addr_kana1		nvarchar(100)	= null,
					@addr_kana2		nvarchar(100)	= null,
					@addr_kana3		nvarchar(100)	= null,
					@addr_kanji1		nvarchar(100)	= null,
					@addr_kanji2		nvarchar(100)	= null,
					@addr_kanji3		nvarchar(100)	= null,
					@home_tel		varchar(20)	= null,
					@work_tel		varchar(20)	= null,
					@cell_tel		varchar(20)	= null,
					@e_mail			varchar(50)	= null,
					@last_person 		varchar(15)


        AS 
        SET NOCOUNT ON

	declare	@tmp_seq	int


	if @info_id = 0
		begin
			insert	address_info
				(
					c_num,
					pax_type,
					pax_name,
					jpn_zip,
					addr_kana1,
					addr_kana2,
					addr_kana3,
					addr_kanji1,
					addr_kanji2,
					addr_kanji3,
					home_tel,
					work_tel,
					cell_tel,
					e_mail,
					create_by,
					create_date,
					last_person,
					update_date
				)
			values
				(
					@c_num,
					@pax_type,
					@pax_name,
					@jpn_zip,
					@addr_kana1,
					@addr_kana2,
					@addr_kana3,
					@addr_kanji1,
					@addr_kanji2,
					@addr_kanji3,
					@home_tel,
					@work_tel,
					@cell_tel,
					@e_mail,
					@last_person,
					getutcdate(),
					@last_person,
					getutcdate()
				)


			select @tmp_seq = scope_identity()

			return(@tmp_seq)
		end
	else
		update	address_info
		   set	pax_type	= @pax_type,
			pax_name	= @pax_name,
			jpn_zip		= @jpn_zip,
			addr_kana1	= @addr_kana1,
			addr_kana2	= @addr_kana2,
			addr_kana3	= @addr_kana3,
			addr_kanji1	= @addr_kanji1,
			addr_kanji2	= @addr_kanji2,
			addr_kanji3	= @addr_kanji3,
			home_tel	= @home_tel,
			work_tel	= @work_tel,
			cell_tel	= @cell_tel,
			e_mail		= @e_mail,
			last_person	= @last_person,
			update_date	= getutcdate()
		 where	info_id = @info_id


	return(0)


GO


