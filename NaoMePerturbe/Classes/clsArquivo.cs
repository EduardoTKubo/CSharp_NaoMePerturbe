using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaoMePerturbe.Classes
{
    class clsArquivo
    {
        private class NaoMePerturbe
        {
            public string TELEFONE { get; set; }
            public string DT_CADASTRO { get; set; }
            public string EVENTO { get; set; }
            public string DT_INICIO { get; set; }
            public string ORIGEM { get; set; }
        }
        
        private static SqlConnection sqlCon;


        public static async Task<bool> ImportarArquivoAsync(string _cam)
        {
            try
            {
                clsVariaveis.GContador = 0;

                // criando datatable
                DataTable DT = new DataTable();
                DT.Columns.Add("TELEFONE"   , typeof(string));
                DT.Columns.Add("DT_CADASTRO", typeof(string));
                DT.Columns.Add("EVENTO"     , typeof(string));
                DT.Columns.Add("DT_INICIO"  , typeof(string));
                DT.Columns.Add("ORIGEM"     , typeof(string));


                SqlConnection sqlconnection = new SqlConnection();
                sqlCon = sqlconnection;
                sqlCon.ConnectionString = Classes.clsVariaveis.Conexao;
                await sqlCon.OpenAsync();


                var cmd = new SqlCommand("truncate table NAOPERTURBE", sqlCon);
                await cmd.ExecuteNonQueryAsync();


                // lendo arquivo .txt e gravando em um DataTable para depois incluir no SQL via SQLBulq
                using(StreamReader sr = new StreamReader(_cam))
                {
                    string linha;
                    while ((linha = sr.ReadLine()) != null )
                    {
                        string[] auxiliar = linha.Split(';');

                        string Lido = auxiliar[0].Replace('"', ' ').Trim();
                        if (Lido.Length > 1)
                        {
                            Lido = Lido.Substring(1, 1);
                            if (char.IsNumber(Convert.ToChar(Lido)))
                            {
                                DataRow dr = DT.NewRow();
                                dr["TELEFONE"]    = clsFuncoes.RetornaNumero(auxiliar[0].Replace('"', ' ').Trim());
                                dr["DT_CADASTRO"] = auxiliar[1].Replace('"', ' ').Trim();
                                dr["EVENTO"]      = auxiliar[2].Replace('"', ' ').Trim();
                                dr["DT_INICIO"]   = auxiliar[3].Replace('"', ' ').Trim();
                                dr["ORIGEM"]      = auxiliar[4].Replace('"', ' ').Trim();
                                DT.Rows.Add(dr);

                                clsVariaveis.GContador += 1;
                                if (clsVariaveis.GContador > 500000)
                                {
                                    bool booOK = await SqlBulkAsync(DT);
                                    clsVariaveis.GContador = 0;
                                    DT.Clear();
                                }             
                            }
                        }
                    }                    
                }

                // incluindo em [NAOPERTURBE_ARQ]
                clsVariaveis.GstrSQL = "insert into [NAOPERTURBE_PROC] ( arquivo ,data ,hora ) values ('" +
                                       clsVariaveis.GStrNomeArqZip + "','" + DateTime.Now.ToString("yyyy-MM-dd") +
                                       "','" + DateTime.Now.ToString("HH:mm:ss") + "' )";
                bool booGravar = await clsBanco.ExecuteQueryAsync(clsVariaveis.GstrSQL);


                // incluindo dados da datatable para sql via slqBulkCopy
                if (DT.Rows.Count > 0)
                {
                    return await SqlBulkAsync(DT);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Trata Atu Procon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open) sqlCon.Close();
            }
        }


        private static bool ExecuteQuery(string gstrSQL)
        {
            throw new NotImplementedException();
        }

        //private static List<NaoMePerturbe> Arq_NaoMePerturbe(string _cam)
        //{
        //    return File.ReadAllLines(_cam)
        //            .Select(a => a.Split(';'))
        //            .Select(c => new NaoMePerturbe()
        //            {
        //                TELEFONE    = c[0].Replace('"', ' ').Trim(),
        //                DT_CADASTRO = c[1].Replace('"', ' ').Trim(),
        //                EVENTO      = c[2].Replace('"', ' ').Trim(),
        //                DT_INICIO   = c[3].Replace('"', ' ').Trim(),
        //                ORIGEM      = c[4].Replace('"', ' ').Trim(),
        //            })
        //            .ToList();
        //}

        //private static List<NaoMePerturbe> Arq_NaoMePerturbe2(string _cam)
        //{
        //    List<NaoMePerturbe> lista = new List<NaoMePerturbe>();

        //    string[] array = File.ReadAllLines(_cam);

        //    for (int i = 1; i <array.Length; i++)
        //    {
        //        NaoMePerturbe c = new NaoMePerturbe();

        //        string[] auxiliar = array[i].Split(';');
        //        c.TELEFONE    = auxiliar[0];
        //        c.DT_CADASTRO = auxiliar[1];
        //        c.EVENTO      = auxiliar[2];
        //        c.DT_INICIO   = auxiliar[3];
        //        c.ORIGEM      = auxiliar[4];
        //        lista.Add(c);
        //    }

        //    return lista;
        //}


        public static async Task<bool> SqlBulkAsync(DataTable _dt)
        {
            try
            {
                // incluindo dados da datatable para sql via slqBulkCopy
                using (SqlBulkCopy s = new SqlBulkCopy(Classes.clsVariaveis.Conexao))
                {
                    s.BatchSize = 50000;
                    s.NotifyAfter = 50000;
                    s.BulkCopyTimeout = 60;  // default = 30
                     
                    s.DestinationTableName = "NAOPERTURBE";
                    s.ColumnMappings.Add("TELEFONE"   , "TELEFONE");
                    s.ColumnMappings.Add("DT_CADASTRO", "DT_CADASTRO");
                    s.ColumnMappings.Add("EVENTO"     , "EVENTO");
                    s.ColumnMappings.Add("DT_INICIO"  , "DT_INICIO");
                    s.ColumnMappings.Add("ORIGEM"     , "ORIGEM");
                    await s.WriteToServerAsync(_dt);
                }
                return true;
            }
            catch (Exception eb)
            {
                MessageBox.Show(eb.Message, "sql bulk", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }


    }
}
