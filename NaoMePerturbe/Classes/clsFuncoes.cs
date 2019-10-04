using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NaoMePerturbe.Classes
{
    class clsFuncoes
    {
        public static bool ArquivoExiste(string _cam)
        {
            FileInfo arq = new FileInfo(_cam);
            if (arq.Exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string DeletarArquivo(string _cam)
        {
            FileInfo arq = new FileInfo(_cam);
            try
            {
                arq.Delete();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string RetornaCelular(string _ddd, string _fone)
        {
            string result = _fone;

            if (result.Length == 8)
            {
                switch (_ddd)
                {
                    case var s when new[] { "11", "12", "13", "14", "15", "16", "17", "18", "19", "21", "22", "24", "27", "28", "31", "32", "33", "34", "35", "37", "38", "41", "42", "43", "44", "45", "46", "47", "48", "49", "51", "53", "54", "55", "61", "62", "63", "64", "65", "66", "67", "68", "69", "71", "73", "74", "75", "77", "79", "81", "82", "83", "84", "85", "86", "87", "88", "89", "91", "92", "93", "94", "95", "96", "97", "98", "99" }.Contains(s):

                        switch (_fone.Substring(0, 1))
                        {
                            case var s1 when new[] { "6", "7", "8", "9" }.Contains(s1):
                                result = "9" + result;
                                break;

                            default:
                                break;
                        }

                        break;

                    default:
                        break;
                }
            }

            return result;
        }


        public static string RetornaNumero(string _texto)
        {
            Regex r = new Regex(@"\d+");

            string result = "";

            foreach (Match m in r.Matches(_texto))
            {
                result += m.Value;
            }

            if (result != "")
            {
                Double i = Convert.ToDouble(result);

                result = i.ToString();
            }
            else
            {
                result = "0";
            }

            return result;
        }

    }
}
