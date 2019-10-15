USE [procon]
GO

/****** Object:  StoredProcedure [dbo].[sp_InsertIntoDoNotCall]    Script Date: 11/10/2019 11:51:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sp_InsertIntoDoNotCall] 
AS
	declare @qtd as numeric(9,0) 

		set @qtd = ( select COUNT(*) as qtd from ATU_PROCON_1 )

		if @qtd > 0 	
			begin
				insert into DoNotCall ( Procon ,NaoMePerturbe ,uf ,nome ,ddd ,telefone ,telBase ,data ,dataLimite ,situacao )
				select 0 ,1 ,uf ,nome ,ddd ,fone ,telefone ,data ,apartirde ,situacao
				from ATU_PROCON_1 where obs is null;

				update d set  d.NaoMePerturbe = 1
				from    DoNotCall       as d
				inner join ATU_PROCON_1 as a  on a.uf = d.uf  and  a.ddd = d.ddd and  a.fone = d.telefone          
				where a.obs = 'SO PROCON';

				truncate table ATU_PROCON_1;

				set @qtd = ( select COUNT(*) as qtd from DoNotCall )
			end

		select @qtd
		return


GO


