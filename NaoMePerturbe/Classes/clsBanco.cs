﻿
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaoMePerturbe.Classes
{
    class clsBanco
    {
        private static SqlConnection sqlCon = null;
        private static SqlCommand sqlCom = null;
        //private static SqlDataAdapter sqlAdapter = null;
        private static DataTable Dt = null;


        //public static DataTable Consulta(string select)
        //{
        //    try
        //    {
        //        sqlCon = new SqlConnection
        //        {
        //            ConnectionString = Classes.clsVariaveis.Conexao
        //        };
        //        sqlCon.Open();

        //        sqlCom = new SqlCommand
        //        {
        //            Connection = sqlCon,
        //            CommandType = CommandType.Text,
        //            CommandText = select,
        //            CommandTimeout = 540
        //        };

        //        sqlAdapter = new SqlDataAdapter(sqlCom);
        //        Dt = new DataTable();
        //        sqlAdapter.Fill(Dt);

        //        return Dt;
        //    }
        //    catch (System.Exception e)
        //    {
        //        MessageBox.Show(e.Message, "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return null;
        //    }
        //}


        public static async Task<DataTable> ConsultaAsync(string select)
        {
            try
            {
                sqlCon = new SqlConnection
                {
                    ConnectionString = Classes.clsVariaveis.Conexao
                };
                await sqlCon.OpenAsync();

                sqlCom = new SqlCommand
                {
                    Connection = sqlCon,
                    CommandType = CommandType.Text,
                    CommandText = select,
                    CommandTimeout = 120
                };

                Dt = new DataTable();
                SqlDataReader reader = await sqlCom.ExecuteReaderAsync();
                Dt.Load(reader);
                return Dt;
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "ConsultaAsync", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open) sqlCon.Close();
            }
        }


        public static async Task<bool> ExecuteQueryAsync(string Comando)
        {
            try
            {
                sqlCon = new SqlConnection
                {
                    ConnectionString = Classes.clsVariaveis.Conexao
                };
                await sqlCon.OpenAsync();

                sqlCom = new SqlCommand
                {
                    Connection = sqlCon,
                    CommandType = CommandType.Text,
                    CommandTimeout = 120,
                    CommandText = Comando
                };
                await sqlCom.ExecuteNonQueryAsync();

                return true;
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "ExecuteQueryAsync", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open) sqlCon.Close();
            }
        }


        public static async Task<DataTable> ExecuteQueryRetornoAsync(string Comando)
        {
            try
            {
                sqlCon = new SqlConnection
                {
                    ConnectionString = Classes.clsVariaveis.Conexao
                };
                await sqlCon.OpenAsync();

                sqlCom = new SqlCommand
                {
                    Connection = sqlCon,
                    CommandType = CommandType.Text,
                    CommandTimeout = 120,
                    CommandText = Comando
                };

                Dt = new DataTable();
                SqlDataReader reader = await sqlCom.ExecuteReaderAsync();
                Dt.Load(reader);
                return Dt;
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "ExecuteQueryRetornoAsync", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open) sqlCon.Close();
            }
        }


        //public static async Task<DataTable> Consulta_DBAsync(string select ,string Conexao)
        //{
        //    try
        //    {
        //        sqlCon = new SqlConnection
        //        {
        //            ConnectionString = Conexao
        //        };
        //        await sqlCon.OpenAsync();

        //        sqlCom = new SqlCommand
        //        {
        //            Connection = sqlCon,
        //            CommandType = CommandType.Text,
        //            CommandText = select,
        //            CommandTimeout = 540
        //        };

        //        //sqlAdapter = new SqlDataAdapter(sqlCom);
        //        //Dt = new DataTable();
        //        //sqlAdapter.Fill(Dt);

        //        Dt = new DataTable();
        //        SqlDataReader reader = await sqlCom.ExecuteReaderAsync();
        //        Dt.Load(reader);

        //        return Dt;
        //    }
        //    catch (System.Exception e)
        //    {
        //        MessageBox.Show(e.Message, "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return null;
        //    }
        //    finally
        //    {
        //        if (sqlCon.State == ConnectionState.Open) sqlCon.Close();
        //    }
        //}


    }
}
