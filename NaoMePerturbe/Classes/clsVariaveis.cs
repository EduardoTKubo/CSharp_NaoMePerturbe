using System;

namespace NaoMePerturbe.Classes
{
    class clsVariaveis
    {
        //// srv_db01
        //private static readonly string conexao = @"Data Source=10.0.32.59;Initial Catalog=PROCON; User ID=sa; Password=SRV@admin2012;";
        
        // srv_db03
        private static readonly string conexao = @"Data Source=10.0.32.63;Initial Catalog=PROCON; User ID=sa; Password=SRV@admin2012;";


        public static string Conexao
        {
            get { return conexao; }
        }
        
        private static string gstrSQL = string.Empty;
        public static string GstrSQL
        {
            get { return gstrSQL; }
            set { gstrSQL = value; }
        }

        private static bool gProcDia = false;
        public static bool GProcDia
        {
            get { return gProcDia; }
            set { gProcDia = value; }
        }

        private static DateTime gDtBase = DateTime.Now;
        public static DateTime GDtBase
        {
            get { return gDtBase; }
            set { gDtBase = value; }
        }

        private static string gStrResult = string.Empty;
        public static string GStrResult
        {
            get { return gStrResult; }
            set { gStrResult = value; }
        }

        private static int gContador = 0;
        public static int GContador
        {
            get { return gContador; }
            set { gContador = value; }
        }

        private static string gStrCamArqZip = string.Empty;
        public static string GStrCamArqZip
        {
            get { return gStrCamArqZip; }
            set { gStrCamArqZip = value; }
        }

        private static string gStrNomeArqZip = string.Empty;
        public static string GStrNomeArqZip
        {
            get { return gStrNomeArqZip; }
            set { gStrNomeArqZip = value; }
        }
        
        private static int intIncluido = 0;
        public static int IntIncluido
        {
            get { return intIncluido; }
            set { intIncluido = value; }
        }
        private static int intTotal = 0;
        public static int IntTotal
        {
            get { return intTotal; }
            set { intTotal = value; }
        }


    }
}
