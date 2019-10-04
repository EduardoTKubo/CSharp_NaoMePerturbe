using System;
using System.IO.Compression;
using System.Threading.Tasks;

namespace NaoMePerturbe.Classes
{
    class clsExtrair
    {
        public static async Task<Boolean> Proc_UnZipFileAsync(string zipPath, string extractPath)
        {
            try
            {
                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                });

                return true;
            }
            catch (Exception e)
            {
                clsVariaveis.GStrResult = e.Message + "/ erro unZip";
                return false;
            }
        }
    }


}
