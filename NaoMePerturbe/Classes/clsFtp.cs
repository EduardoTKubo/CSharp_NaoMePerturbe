using System;
using System.Data;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;
using System.Threading.Tasks;

namespace NaoMePerturbe.Classes
{
    class clsFtp
    {
        private static string pathOrigem = string.Empty;
        private static string pathDestino = string.Empty;
        

        public static async Task<Boolean> P02_DownloadFTPAsync()
        {
            clsVariaveis.GStrNomeArqZip = "";
            clsVariaveis.GStrCamArqZip = "";

            try
            {
                // conexao via sftp
                using (SftpClient sftp = new SftpClient("sftp.net.com.br", "globocabo", "kUsTeHEspa46"))
                {
                    sftp.Connect();

                    var arquivos = sftp.ListDirectory("/app/sftp/globocabo/inbox/");
                    foreach (var arquivo in arquivos)
                    {
                        if (arquivo.FullName.Length > 30)
                        {                        
                            if (arquivo.FullName.Substring(26, 21).Equals("NAO_PERTURBE_" + clsVariaveis.GDtBase.ToString("yyyyMMdd")))
                            {
                                if (arquivo.FullName.Substring(arquivo.FullName.Length - 4, 4).Equals(".zip"))
                                {
                                    pathOrigem = arquivo.FullName;

                                    clsVariaveis.GStrNomeArqZip = arquivo.Name;                 // nome do arquivo desejado
                                    clsVariaveis.GStrCamArqZip = Application.StartupPath + @"\arquivo\" + clsVariaveis.GStrNomeArqZip;   // caminho do arquivo .zip 
                                    pathDestino = clsVariaveis.GStrCamArqZip;

                                    // verifica se o arquivo ja foi processado
                                    clsVariaveis.GstrSQL = "select * from [NAOPERTURBE_PROC] where Arquivo = '" + clsVariaveis.GStrNomeArqZip + "' and Ativo = 1 ";
                                    DataTable dtArq = await Classes.clsBanco.ConsultaAsync(clsVariaveis.GstrSQL);
                                    if (dtArq.Rows.Count == 0)
                                    {
                                        // se arquivo .zip existir --> deletar
                                        if (clsFuncoes.ArquivoExiste(pathDestino))
                                        {
                                            string strDel = clsFuncoes.DeletarArquivo(pathDestino);
                                        }

                                        // se arquivo .txt existir --> deletar
                                        if (clsFuncoes.ArquivoExiste(pathDestino.Replace(".zip", ".txt")))
                                        {
                                            string strDel = clsFuncoes.DeletarArquivo(pathDestino.Replace(".zip", ".txt"));
                                        }

                                        // fazer o download do arquivo desejado para pasta informada
                                        using (Stream fileStream = File.OpenWrite(pathDestino))
                                        {
                                            await Task.Run(() =>
                                            {
                                                sftp.DownloadFile(pathOrigem, fileStream);
                                            });
                                        }
                                    }
                                    else
                                    {
                                        // o arquivo ja foi processado
                                        clsVariaveis.GStrResult     = clsVariaveis.GStrNomeArqZip + " : já foi processado";
                                        clsVariaveis.GStrNomeArqZip = "";
                                        clsVariaveis.GStrCamArqZip  = "";
                                        clsVariaveis.GProcDia       = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    sftp.Disconnect();
                }

                if(clsVariaveis.GStrCamArqZip != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
                    
            }
            catch (Exception e)
            {
                clsVariaveis.GStrResult = e.Message + "/ erro dowload ftp";
                return false;
            }
        }

    }
}
