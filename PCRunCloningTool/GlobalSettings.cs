namespace PCRunCloningTool
{
    internal class GlobalSettings
    {
        public static string msSqlServerNamePC = "PCTRSQLPRODVH2";
        public static string msSqlUserNamePC = "loadtester";
        public static string msSqlPasswordPC = "5RonLzQhl36pRBpXEWB5";

        public static string msSqlServerNameCustom = "VH-MA0055026\\BUILDMASTER";
        public static string msSqlUserNameCustom = "test";
        public static string msSqlPasswordCustom = "Crown2009";
        public static string msSqlDatabaseNameCustom = "PCAnalysisDashboard";
        
        public static string GetConnectionStringWithoutDB()
        {
            return "Data Source=" + msSqlServerNameCustom + ";User ID=" + msSqlUserNameCustom + ";Password=" + msSqlPasswordCustom;
        }

        public static string GetConnectionStringCustomDB()
        {
            //"Server=[" + msSqlServerNameCustom + "];Database=[" + msSqlUserNameCustom + "];Trusted_Connection=true"
            return "Data Source=" + msSqlServerNameCustom + ";Initial Catalog=" + msSqlDatabaseNameCustom + ";User ID=" + msSqlUserNameCustom + ";Password=" + msSqlPasswordCustom;
            //return "Data Source=" + msSqlServerNameCustom + ";User ID=" + msSqlUserNameCustom + ";Password=" + msSqlPasswordCustom;
        }

        public static string GetConnectionStringPCDB()
        {
            return "Data Source=" + msSqlServerNamePC + ";Initial Catalog=pcsiteadmin_db;User ID=" + msSqlUserNamePC + ";Password=" + msSqlPasswordPC;
        }

    }
}