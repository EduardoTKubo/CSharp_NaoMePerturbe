using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace NaoMePerturbe.Classes
{
    class clsDatabase
    {
        private static SqlConnection sqlConDest = null;

        public static async Task<bool> CopyToDbAsync(string _db, string _conDestino)
        {
            try
            {
                clsVariaveis.GStrResult = string.Empty;
                clsVariaveis.IntIncluido = 0;
                clsVariaveis.IntTotal = 0;

                // conexao ao servidor destino
                SqlConnection sqlconnection = new SqlConnection();
                sqlConDest = sqlconnection;
                sqlConDest.ConnectionString = _conDestino;
                await sqlConDest.OpenAsync();

                // limpando ATU_PROCON_1 da database do servidor de destino
                var cmd = new SqlCommand("truncate table [ATU_PROCON_1]", sqlConDest);
                await cmd.ExecuteNonQueryAsync();

                //  gerando datatable com dados da tabela ATU_PROCON_1 do servidor DB03
                clsVariaveis.GstrSQL = "select ID ,ORIGEM ,UF ,NOME ,TELEFONE ,DATA ,SITUACAO ,APARTIRDE ,DATALIMITE ,DDD ,FONE ,seq from [Atu_Procon_1] where Obs is null";
                DataTable dtProcon = await Classes.clsBanco.ConsultaAsync(clsVariaveis.GstrSQL);
                if (dtProcon.Rows.Count != 0)
                {
                    clsVariaveis.IntIncluido = dtProcon.Rows.Count;

                    // exportando Atu_Procon_1 da origem para o destino
                    if ( await SqlBulkAsync( dtProcon, _db, _conDestino))
                    {
                        // executando stored procedure : sp_InsertIntoDoNotCall / inclusao Atu_Procon_1  em  DoNotCall
                        SqlCommand sqlCom = new SqlCommand
                        {
                            Connection = sqlConDest,
                            CommandType = CommandType.Text,
                            CommandTimeout = 540,
                            CommandText = "exec sp_InsertIntoDoNotCall "
                        };
                    
                        var strResp = await sqlCom.ExecuteScalarAsync();
                        clsVariaveis.IntTotal = Convert.ToInt32(strResp.ToString());

                        if (clsVariaveis.IntTotal == 0)
                        {
                            clsVariaveis.GStrResult = _db + " | Atu_Procon_1 vazio - destino";
                            return false;
                        }
                        else
                        {
                            clsVariaveis.GStrResult = _db + " | DoNotCall Atualizado com sucesso";
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }                    
                }
                else
                {
                    clsVariaveis.GStrResult = _db + " | Atu_Procon_1 vazio - origem";
                    return false;
                }
            }
            catch (Exception e)
            {
                clsVariaveis.GStrResult = _db + " | " + e.Message;
                return false;
            }
            finally
            {
                if (sqlConDest.State == ConnectionState.Open) sqlConDest.Close();
            }
        }



        public static async Task<Boolean> SqlBulkAsync(DataTable _dt, string _db, string _conDestino)
        {
            try
            {
                // incluindo dados da datatable para sql via slqBulkCopy
                using (SqlBulkCopy s = new SqlBulkCopy(_conDestino))
                {
                    s.BatchSize = 5000;
                    s.NotifyAfter = 5000;

                    s.DestinationTableName = "ATU_PROCON_1";
                    s.ColumnMappings.Add("ID"         , "ID");
                    s.ColumnMappings.Add("ORIGEM"     , "ORIGEM");
                    s.ColumnMappings.Add("UF"         , "UF");
                    s.ColumnMappings.Add("NOME"       , "NOME");
                    s.ColumnMappings.Add("TELEFONE"   , "TELEFONE");
                    s.ColumnMappings.Add("DATA"       , "DATA");
                    s.ColumnMappings.Add("SITUACAO"   , "SITUACAO");
                    s.ColumnMappings.Add("APARTIRDE"  , "APARTIRDE");
                    s.ColumnMappings.Add("DATALIMITE" , "DATALIMITE");
                    s.ColumnMappings.Add("DDD"        , "DDD");
                    s.ColumnMappings.Add("FONE"       , "FONE");
                    s.ColumnMappings.Add("seq"        , "seq");
                    await s.WriteToServerAsync(_dt);
                }
                return true;
            }
            catch (Exception eb)
            {
                clsVariaveis.GStrResult = _db + " | " + eb.Message;
                return false;
            }

        }


        //public static async Task<Boolean> SqlBulkInclAsync(DataTable _dt, string _db, string _conDestino)
        //{
        //    try
        //    {
        //        // incluindo dados da datatable para sql via slqBulkCopy
        //        using (SqlBulkCopy s = new SqlBulkCopy(_conDestino))
        //        {
        //            s.BatchSize = 5000;
        //            s.NotifyAfter = 5000;
        //            s.BulkCopyTimeout = 60;

        //            s.DestinationTableName = "donotcall";
        //            s.ColumnMappings.Add("Procon"       , "Procon");
        //            s.ColumnMappings.Add("NaoMePerturbe", "NaoMePerturbe");
        //            s.ColumnMappings.Add("uf"           , "uf");
        //            s.ColumnMappings.Add("nome"         , "nome");
        //            s.ColumnMappings.Add("ddd"          , "ddd");
        //            s.ColumnMappings.Add("telefone"     , "telefone");
        //            s.ColumnMappings.Add("telBase"      , "telBase");
        //            s.ColumnMappings.Add("data"         , "data");
        //            s.ColumnMappings.Add("dataLimite"   , "dataLimite");
        //            s.ColumnMappings.Add("situacao"     , "situacao");
        //            await s.WriteToServerAsync(_dt);
        //        }
        //        //clsVariaveis.GStrResult = _db + " | copia realizada com sucesso";
        //        return true;
        //    }
        //    catch (Exception eb)
        //    {
        //        clsVariaveis.GStrResult = _db + " | " + eb.Message;
        //        return false;
        //    }
        //}

    }
}
