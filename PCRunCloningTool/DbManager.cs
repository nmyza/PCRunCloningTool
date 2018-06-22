using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Data;

namespace PCRunCloningTool
{
    internal class DbManager
    {
        private static int SQL_BULK_COPY_TIMEOUT = 120;
        private static int SQL_COMMAND_TIMEOUT = 120;
        public static void PrepareDb()
        {
            if (!CheckDatabaseExistsMSSQL(GlobalSettings.GetConnectionStringWithoutDB(), GlobalSettings.msSqlDatabaseNameCustom))
            {
                CreateDB();
                CreateDbModel();
                CreateDbFuntions();
            }
                
        }

        public static void CopyDB(TestRunResults run, string reportsLocation, string domain, string project)
        {
            string runFolderLocation;
            string accessDbFilePath;
            Console.WriteLine("CopyDB");
            if (RunExist(run.AnalysedResults.RunId, domain, project))
            {
                Console.WriteLine("Run exist!");
            }
            else
            {
                runFolderLocation = reportsLocation + "/" + domain + "/" + project + "/" + run.AnalysedResults.RunId;
                accessDbFilePath = runFolderLocation + "/Reports/" + run.AnalysedResults.Name.Replace(".zip", ".mdb");
                CopyMetrics(GlobalSettings.GetConnectionStringCustomDB(), accessDbFilePath, run.AnalysedResults.RunId);
                CopyTransactionSummary(GlobalSettings.GetConnectionStringCustomDB(), accessDbFilePath, run.AnalysedResults.RunId);
                CopySiteScopeMetrics(GlobalSettings.GetConnectionStringCustomDB(), accessDbFilePath, run.AnalysedResults.RunId);
                CopyRun(run.AnalysedResults.RunId, domain, project);
                CopyRunFolders(domain, project);
                TakeSiteScopeMetrics(GlobalSettings.GetConnectionStringCustomDB(), AspNetStat_SQL(run.AnalysedResults.RunId), run.AnalysedResults.RunId, "SiteScope_AspNet_Stats");
                TakeSiteScopeMetrics(GlobalSettings.GetConnectionStringCustomDB(), DbStat_SQL(run.AnalysedResults.RunId), run.AnalysedResults.RunId, "SiteScope_Database_Stats");
                TakeSiteScopeMetrics(GlobalSettings.GetConnectionStringCustomDB(), ServerStat_SQL(run.AnalysedResults.RunId), run.AnalysedResults.RunId, "SiteScope_Server_Stats");
                DeleteTemporaryMetricData(run.AnalysedResults.RunId);
                Directory.Delete(runFolderLocation, true);
            }
        }

        private static void DeleteTemporaryMetricData(string runID)
        {
            ExecuteCommand(GlobalSettings.GetConnectionStringCustomDB(), DeleteTemporaryMetricData_SQL(runID));
        }

        public static bool RunExist(string runID, string domain, string project)
        {
            string sql = @"
                            SELECT *
                            FROM [" + GlobalSettings.msSqlDatabaseNameCustom + @"].[dbo].[TestRuns]
                            WHERE [RN_RUN_ID] = " + runID;
            return HasRowsInCustomDB(sql);
        }

        private static bool RunFoldersExist(string domain, string project)
        {
            string sql = String.Format(@"
                            SELECT *
                            FROM [" + GlobalSettings.msSqlDatabaseNameCustom + @"].[dbo].[ALL_LISTS]
                            WHERE [DOMAIN] = '{0}' AND [PROJECT] = '{1}'", domain, project);
            return HasRowsInCustomDB(sql); 
        }

        private static bool HasRowsInCustomDB(string sql)
        {
            using (var targeDB = new SqlConnection(GlobalSettings.GetConnectionStringCustomDB()))
            using (SqlCommand command = new SqlCommand(sql, targeDB))
            {
                targeDB.Open();
                command.CommandTimeout = SQL_COMMAND_TIMEOUT;
                var reader = command.ExecuteReader();
                return reader.HasRows;
            }
        }

        private static void CopyRun(string runID, string domain, string project)
        {
            string sql = RunByIdFromPcDB_SQL(domain, project, runID);
            using (var sourceDB = new SqlConnection(GlobalSettings.GetConnectionStringPCDB()))
            using (SqlCommand command = new SqlCommand(sql, sourceDB))
            {
                sourceDB.Open();
                command.CommandTimeout = SQL_COMMAND_TIMEOUT;
                var reader = command.ExecuteReader();
                using (var destinationdestinationDB = new SqlConnection(GlobalSettings.GetConnectionStringCustomDB()))
                {
                    destinationdestinationDB.Open();
                    using(var bulkCopy = new SqlBulkCopy(destinationdestinationDB))
                    {
                        bulkCopy.ColumnMappings.Add("DOMAIN", "DOMAIN");
                        bulkCopy.ColumnMappings.Add("PROJECT", "PROJECT");
                        bulkCopy.ColumnMappings.Add("RN_TEST_ID", "RN_TEST_ID");
                        bulkCopy.ColumnMappings.Add("TS_SUBJECT", "TS_SUBJECT");
                        bulkCopy.ColumnMappings.Add("RN_TEST_CONFIG_ID", "RN_TEST_CONFIG_ID");
                        bulkCopy.ColumnMappings.Add("RN_RUN_ID", "RN_RUN_ID");
                        bulkCopy.ColumnMappings.Add("RN_RUN_NAME", "RN_RUN_NAME");
                        bulkCopy.ColumnMappings.Add("RN_EXECUTION_DATE", "RN_EXECUTION_DATE");
                        bulkCopy.ColumnMappings.Add("RN_EXECUTION_TIME", "RN_EXECUTION_TIME");
                        bulkCopy.ColumnMappings.Add("RN_DURATION", "RN_DURATION");
                        bulkCopy.ColumnMappings.Add("RN_STATE", "RN_STATE");
                        bulkCopy.ColumnMappings.Add("RN_PC_START_TIME", "RN_PC_START_TIME");
                        bulkCopy.ColumnMappings.Add("RN_PC_END_TIME", "RN_PC_END_TIME");
                        bulkCopy.ColumnMappings.Add("RN_PC_CONTROLLER_NAME", "RN_PC_CONTROLLER_NAME");
                        bulkCopy.ColumnMappings.Add("RN_PC_LOAD_GENERATORS", "RN_PC_LOAD_GENERATORS");
                        bulkCopy.ColumnMappings.Add("RN_PC_VUSERS_INVOLVED", "RN_PC_VUSERS_INVOLVED");
                        bulkCopy.ColumnMappings.Add("RN_PC_VUSERS_MAX", "RN_PC_VUSERS_MAX");
                        bulkCopy.ColumnMappings.Add("RN_PC_VUSERS_AVERAGE", "RN_PC_VUSERS_AVERAGE");
                        bulkCopy.ColumnMappings.Add("RN_PC_TOTAL_TRANSACT_PASSED", "RN_PC_TOTAL_TRANSACT_PASSED");
                        bulkCopy.ColumnMappings.Add("RN_PC_TOTAL_TRANSACT_FAILED", "RN_PC_TOTAL_TRANSACT_FAILED");
                        bulkCopy.ColumnMappings.Add("RN_PC_TOTAL_ERRORS", "RN_PC_TOTAL_ERRORS");
                        bulkCopy.ColumnMappings.Add("RN_PC_HITS_SEC_AVERAGE", "RN_PC_HITS_SEC_AVERAGE");
                        bulkCopy.ColumnMappings.Add("RN_PC_THROUGHPUT_AVERAGE", "RN_PC_THROUGHPUT_AVERAGE");
                        bulkCopy.ColumnMappings.Add("RN_PC_TRANSACT_SEC_AVERAGE", "RN_PC_TRANSACT_SEC_AVERAGE");
                        bulkCopy.DestinationTableName = GlobalSettings.msSqlDatabaseNameCustom + ".dbo.TestRuns";

                        bulkCopy.BulkCopyTimeout = SQL_BULK_COPY_TIMEOUT;
                        try
                        {
                            Utils.StartMeasure("TestRuns");
                            bulkCopy.WriteToServer(reader);
                            Utils.StopMeasure("TestRuns");
                        }
                        catch (Exception ex)
                        {
                            Utils.StopMeasure("TestRuns");
                            Console.WriteLine("Run ID: " + runID + Environment.NewLine + ex.Message);
                            Logger.Log.Error("Data copying to TestRuns was failed. Run ID: " + runID + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
        }

        private static void CopyRunFolders(string domain, string project)
        {
            if (RunFoldersExist(domain, project)) return;

            string sql = RunFoldersPCDB_SQL(domain, project);
            using (var sourceDB = new SqlConnection(GlobalSettings.GetConnectionStringPCDB()))
            using (SqlCommand command = new SqlCommand(sql, sourceDB))
            {
                sourceDB.Open();
                command.CommandTimeout = SQL_COMMAND_TIMEOUT;
                var reader = command.ExecuteReader();
                using (var destinationdestinationDB = new SqlConnection(GlobalSettings.GetConnectionStringCustomDB()))
                {
                    destinationdestinationDB.Open();
                    using (var bulkCopy = new SqlBulkCopy(destinationdestinationDB))
                    {
                        bulkCopy.ColumnMappings.Add("DOMAIN", "DOMAIN");
                        bulkCopy.ColumnMappings.Add("PROJECT", "PROJECT");
                        bulkCopy.ColumnMappings.Add("AL_ITEM_ID", "AL_ITEM_ID");
                        bulkCopy.ColumnMappings.Add("AL_FATHER_ID", "AL_FATHER_ID");
                        bulkCopy.ColumnMappings.Add("AL_DESCRIPTION", "AL_DESCRIPTION");
                        bulkCopy.DestinationTableName = GlobalSettings.msSqlDatabaseNameCustom + ".dbo.ALL_LISTS";

                        bulkCopy.BulkCopyTimeout = SQL_BULK_COPY_TIMEOUT;
                        try
                        {
                            bulkCopy.WriteToServer(reader);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Logger.Log.Error("Data copying to ALL_LISTS was failed. " + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
        }

        private static void CopyMetrics(string connectionString, string accessDbFilePath, string runID)
        {
            // string accessConnStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data source= C:\\tmp\\Reports\\Results_59.mdb";
            string accessConnStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data source=" + accessDbFilePath;
            using (var sourceConnection = new OleDbConnection(accessConnStr))
            {
                sourceConnection.Open();

                var commandSourceData = new OleDbCommand(CopyMetrics_SQL(runID), sourceConnection);
                commandSourceData.CommandTimeout = SQL_COMMAND_TIMEOUT;
                var reader = commandSourceData.ExecuteReader();

                using (var destinationConnection = new SqlConnection(connectionString))
                {
                    destinationConnection.Open();

                    using (var bulkCopy = new SqlBulkCopy(destinationConnection))
                    {
                        bulkCopy.ColumnMappings.Add("RunID", "RunID");
                        bulkCopy.ColumnMappings.Add("Metric", "Metric");
                        bulkCopy.ColumnMappings.Add("TransactionName", "TransactionName");
                        bulkCopy.ColumnMappings.Add("EndTime", "EndTime");
                        bulkCopy.ColumnMappings.Add("Value", "Value");
                        bulkCopy.DestinationTableName = GlobalSettings.msSqlDatabaseNameCustom + ".dbo.TestRunStats";

                        bulkCopy.BulkCopyTimeout = SQL_BULK_COPY_TIMEOUT;
                        try
                        {
                            Utils.StartMeasure("TestRunStats");
                            bulkCopy.WriteToServer(reader);
                            Utils.StopMeasure("TestRunStats");
                        }
                        catch (Exception ex)
                        {
                            Utils.StopMeasure("TestRunStats");
                            Console.WriteLine("Run ID: " + runID + Environment.NewLine + ex.Message);
                            Logger.Log.Error("Data copying to TestRunStats was failed. Run ID: " + runID + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
        }

        private static void CopySiteScopeMetrics(string connectionString, string accessDbFilePath, string runID)
        {
            // string accessConnStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data source= C:\\tmp\\Reports\\Results_59.mdb";
            string accessConnStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data source=" + accessDbFilePath;
            using (var sourceConnection = new OleDbConnection(accessConnStr))
            {
                sourceConnection.Open();

                var commandSourceData = new OleDbCommand(SiteScopeMetrics_SQL(runID), sourceConnection);
                var reader = commandSourceData.ExecuteReader();

                using (var destinationConnection = new SqlConnection(connectionString))
                {
                    destinationConnection.Open();

                    using (var bulkCopy = new SqlBulkCopy(destinationConnection))
                    {
                        bulkCopy.ColumnMappings.Add("RunID", "RunID");
                        bulkCopy.ColumnMappings.Add("Event Name", "Event Name");
                        bulkCopy.ColumnMappings.Add("Event Type", "Event Type");
                        bulkCopy.ColumnMappings.Add("EndTime", "EndTime");
                        bulkCopy.ColumnMappings.Add("Avg", "Avg");
                        bulkCopy.ColumnMappings.Add("Max", "Max");
                        bulkCopy.ColumnMappings.Add("Min", "Min");
                        bulkCopy.DestinationTableName = GlobalSettings.msSqlDatabaseNameCustom + ".dbo.Monitor_meter";

                        bulkCopy.BulkCopyTimeout = SQL_BULK_COPY_TIMEOUT;
                        try
                        {
                            Utils.StartMeasure("Monitor_meter");
                            bulkCopy.WriteToServer(reader);
                            Utils.StopMeasure("Monitor_meter");

                        }
                        catch (Exception ex)
                        {
                            Utils.StopMeasure("Monitor_meter");
                            Console.WriteLine("Run ID: " + runID + Environment.NewLine + ex.Message);
                            Logger.Log.Error("Data copying to Monitor_meter was failed. Run ID: " + runID + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
        }

        private static void TakeSiteScopeMetrics(string connectionString, string sql, string runID, string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var connection2 = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                connection2.Open();
                command.CommandTimeout = SQL_COMMAND_TIMEOUT;
                var reader = command.ExecuteReader();


                using (var bulkCopy = new SqlBulkCopy(connection2))
                {
                    bulkCopy.ColumnMappings.Add("RunID", "RunID");
                    //bulkCopy.ColumnMappings.Add("StatType", "StatType");
                    bulkCopy.ColumnMappings.Add("TYPE", "TYPE");
                    bulkCopy.ColumnMappings.Add("Server", "Server");
                    bulkCopy.ColumnMappings.Add("Metrictype", "Metrictype");
                    bulkCopy.ColumnMappings.Add("Category", "Category");
                    bulkCopy.ColumnMappings.Add("Instance", "Instance");
                    bulkCopy.ColumnMappings.Add("Counter", "Counter");
                    bulkCopy.ColumnMappings.Add("Time", "Time");
                    bulkCopy.ColumnMappings.Add("Avg", "Avg");
                    //bulkCopy.DestinationTableName = GlobalSettings.msSqlDatabaseNameCustom + ".dbo.SiteScopeStats";
                    bulkCopy.DestinationTableName = GlobalSettings.msSqlDatabaseNameCustom + ".dbo." + tableName;

                    bulkCopy.BulkCopyTimeout = SQL_BULK_COPY_TIMEOUT;
                    try
                    {
                        Utils.StartMeasure(tableName);
                        bulkCopy.WriteToServer(reader);
                        Utils.StopMeasure(tableName);
                    }
                    catch (Exception ex)
                    {
                        Utils.StopMeasure(tableName);
                        Console.WriteLine("Run ID: " + runID + Environment.NewLine + ex.Message);
                        Logger.Log.Error("Data copying to SiteScopeStats was failed. Run ID: " + runID + Environment.NewLine + ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
        }

        private static void CopyTransactionSummary(string connectionString, string accessDbFilePath, string runID)
        {
            // string accessConnStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data source= C:\\tmp\\Reports\\Results_59.mdb";
            string accessConnStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data source=" + accessDbFilePath;
            using (var sourceConnection = new OleDbConnection(accessConnStr))
            {
                sourceConnection.Open();

                var commandSourceData = new OleDbCommand(TransactionSummary_SQL(runID), sourceConnection);
                var reader = commandSourceData.ExecuteReader();

                using (var destinationConnection = new SqlConnection(connectionString))
                {
                    destinationConnection.Open();

                    using (var bulkCopy = new SqlBulkCopy(destinationConnection))
                    {
                        bulkCopy.ColumnMappings.Add("RunID", "RunID");
                        bulkCopy.ColumnMappings.Add("Event ID", "Event ID");
                        bulkCopy.ColumnMappings.Add("TransactionName", "TransactionName");
                        bulkCopy.ColumnMappings.Add("RT50Perc", "RT50Perc");
                        bulkCopy.ColumnMappings.Add("RT85Perc", "RT85Perc");
                        bulkCopy.ColumnMappings.Add("RTmin", "RTmin");
                        bulkCopy.ColumnMappings.Add("RTmax", "RTmax");
                        bulkCopy.ColumnMappings.Add("RTavg", "RTavg");
                        bulkCopy.ColumnMappings.Add("pass", "pass");
                        bulkCopy.ColumnMappings.Add("fail", "fail"); 
                        bulkCopy.ColumnMappings.Add("SLA50Perc", "SLA50Perc");
                        bulkCopy.ColumnMappings.Add("SLA85Perc", "SLA85Perc");
                        bulkCopy.DestinationTableName = GlobalSettings.msSqlDatabaseNameCustom + ".dbo.TestTransactionSummary";

                        bulkCopy.BulkCopyTimeout = SQL_BULK_COPY_TIMEOUT;
                        try
                        {
                            Utils.StartMeasure("TestTransactionSummary");
                            bulkCopy.WriteToServer(reader);
                            Utils.StopMeasure("TestTransactionSummary");

                        }
                        catch (Exception ex)
                        {
                            Utils.StopMeasure("TestTransactionSummary");
                            Console.WriteLine("Run ID: " + runID + Environment.NewLine + ex.Message);
                            Logger.Log.Error("Data copying to TestTransactionSummary was failed. Run ID: " + runID + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
        }


        private static void DropDB()
        {
            ExecuteCommand(GlobalSettings.GetConnectionStringWithoutDB(), DropDb_SQL());
        }

        private static void CreateDB()
        {
            ExecuteCommand(GlobalSettings.GetConnectionStringWithoutDB(), CreateDb_SQL());
        }

        private static void CreateDbModel()
        {
            ExecuteCommand(GlobalSettings.GetConnectionStringCustomDB(), CreateModel_SQL());
        }

        private static void CreateDbFuntions()
        {
            ExecuteCommand(GlobalSettings.GetConnectionStringCustomDB(), CreateFunctionID_SQL());
            ExecuteCommand(GlobalSettings.GetConnectionStringCustomDB(), CreateFunctionNI_SQL());
        }

        public static bool CheckDatabaseExistsMSSQL(string connectionString, string databaseName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand($"SELECT db_id('{databaseName}')", connection))
                {
                    connection.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }       

        public static List<string> LoadDomains()
        {
            string sql = "Select [DOMAIN_NAME] From [pcsiteadmin_db].[td].[DOMAINS]";
            return ExecuteDataRequest(sql, GlobalSettings.GetConnectionStringPCDB());
        }
        public static List<string> LoadProjects(string domain)
        {
            string sql = "Select [PROJECT_NAME] From [pcsiteadmin_db].[td].[PROJECTS] WHERE [DOMAIN_NAME] LIKE '" + domain + "'";
            return ExecuteDataRequest(sql, GlobalSettings.GetConnectionStringPCDB());
        }

        private static void ExecuteCommand(string connectionString, string SQL)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(SQL, connection))
                {
                    command.CommandTimeout = SQL_COMMAND_TIMEOUT;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private static List<string> ExecuteDataRequest(string sql, string connetionString)
        {
            List<string> result = null;
            using (var connection = new SqlConnection(connetionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandTimeout = SQL_COMMAND_TIMEOUT;
                    SqlDataReader dataReader = command.ExecuteReader();

                    result = new List<string>();
                    while (dataReader.Read())
                    {
                        result.Add(dataReader.GetValue(0).ToString());
                    }
                    dataReader.Close();
                }
            }
            return result;
        }

        public static void LoadRunsFromDB(DataSet dataSet, string member, string domain, string project, string parrentFolderIDs)
        {
            string where = "";
            if (!String.IsNullOrEmpty(parrentFolderIDs))
                where = String.Format("WHERE [TS_SUBJECT] in ({0})", parrentFolderIDs);

            LoadDataSetFromDB(
                dataSet,
                member,
                GlobalSettings.GetConnectionStringPCDB(),
                RunsFromPcDB_SQL(domain, project) + where
                );
        }

        public static void GetFolderStructure(DataSet data, string member, string domain, string project)
        {
            LoadDataSetFromDB(
                data,
                member,
                GlobalSettings.GetConnectionStringCustomDB(),
                RunFoldersCustomDB_SQL(domain, project)
            );
            if (data.Tables[member].Rows.Count == 0)
            {
                CopyRunFolders(domain, project);
                LoadDataSetFromDB(
                    data,
                    member,
                    GlobalSettings.GetConnectionStringCustomDB(),
                    RunFoldersCustomDB_SQL(domain, project)
                );
            }
        }

        private static void LoadDataSetFromDB(DataSet dataSet, string member, string connectionString, string sql)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                connection.Open();
                adapter.Fill(dataSet, member);
            }
        }

        /*********************************************/
        /***************  SQL Partion ****************/
        /*********************************************/

        private static string SiteScopeMetrics_SQL(string runID)
        {
            return String.Format(@"
                        SELECT
                            '{0}' as [RunID]
	                        ,evmp.[Event Name]
	                        ,evmp.[Event Type]
	                        ,Int([End Time]/60) as [Endtime]
	                        ,avg([Value]) as [Avg]
	                        ,max([Amaximum]) as [Max]
	                        ,min([Aminimum]) as [Min]
                        FROM [Monitor_meter] monmr
                        INNER JOIN [Event_map] evmp on monmr.[Event ID]=evmp.[Event ID] 
                        WHERE evmp.[Event Name] not like '%countersInError%' and evmp.[Event Type]='SiteScope' 
                        GROUP BY evmp.[Event Name],evmp.[Event Type],Int([End Time]/60)
                    ", runID);
        }

        private static string RunByIdFromPcDB_SQL(string domain, string project, string runID)
        {
            return String.Format(@"
                        SELECT   '{0}' as DOMAIN
                                ,'{1}' as PROJECT
                                ,[RN_TEST_ID]
                                ,[TS_SUBJECT]
                                ,[RN_TEST_CONFIG_ID]
                                ,[RN_RUN_ID]
                                ,[RN_RUN_NAME]
                                ,[RN_EXECUTION_DATE]
                                ,[RN_EXECUTION_TIME]
                                ,[RN_DURATION]
                                ,[RN_STATE]
                                ,[RN_PC_START_TIME]
                                ,[RN_PC_END_TIME]
                                ,[RN_PC_CONTROLLER_NAME]
                                ,[RN_PC_LOAD_GENERATORS]
                                ,[RN_PC_VUSERS_INVOLVED]
                                ,[RN_PC_VUSERS_MAX]
                                ,[RN_PC_VUSERS_AVERAGE]
                                ,[RN_PC_TOTAL_TRANSACT_PASSED]
                                ,[RN_PC_TOTAL_TRANSACT_FAILED]
                                ,[RN_PC_TOTAL_ERRORS]
                                ,[RN_PC_HITS_SEC_AVERAGE]
                                ,[RN_PC_THROUGHPUT_AVERAGE]
                                ,[RN_PC_TRANSACT_SEC_AVERAGE]
                          FROM 
                            [{0}_{1}_db].[td].[RUN] INNER JOIN [{0}_{1}_db].[td].[TEST] ON [RN_TEST_ID] = [TS_TEST_ID]
                          WHERE 
                            [RN_RUN_ID] = {2}
                        ", domain, project, runID);
        }

        private static string RunsFromPcDB_SQL(string domain, string project)
        {
            return String.Format(@"
                        SELECT   '{0}' as DOMAIN
                                ,'{1}' as PROJECT
                                ,[RN_TEST_ID]
                                ,[TS_SUBJECT]
                                ,[RN_TEST_CONFIG_ID]
                                ,[RN_RUN_ID]
                                ,[RN_RUN_NAME]
                                ,[RN_EXECUTION_DATE]
                                ,[RN_EXECUTION_TIME]
                                ,[RN_DURATION]
                                ,[RN_STATE]
                                ,[RN_PC_START_TIME]
                                ,[RN_PC_END_TIME]
                                ,[RN_PC_CONTROLLER_NAME]
                                ,[RN_PC_LOAD_GENERATORS]
                                ,[RN_PC_VUSERS_INVOLVED]
                                ,[RN_PC_VUSERS_MAX]
                                ,[RN_PC_VUSERS_AVERAGE]
                                ,[RN_PC_TOTAL_TRANSACT_PASSED]
                                ,[RN_PC_TOTAL_TRANSACT_FAILED]
                                ,[RN_PC_TOTAL_ERRORS]
                                ,[RN_PC_HITS_SEC_AVERAGE]
                                ,[RN_PC_THROUGHPUT_AVERAGE]
                                ,[RN_PC_TRANSACT_SEC_AVERAGE]
                          FROM 
                            [{0}_{1}_db].[td].[RUN] INNER JOIN [{0}_{1}_db].[td].[TEST] ON [RN_TEST_ID] = [TS_TEST_ID]
                    ", domain, project);
        }

        private static string DeleteTemporaryMetricData_SQL(string runID)
        {
            return String.Format("DELETE FROM[{1}].[dbo].[Monitor_meter] WHERE[RunID] = {0}", runID, GlobalSettings.msSqlDatabaseNameCustom);
        }

        private static string DropDb_SQL()
        {
            return String.Format(@"
                If(db_id(N'{0}') IS NOT NULL) 
	                    drop database {0};
                ", GlobalSettings.msSqlDatabaseNameCustom);
        }

        private static string CreateDb_SQL()
        {
            return String.Format(@"
                If(db_id(N'{0}') IS NULL)
	                create database {0};
                ", GlobalSettings.msSqlDatabaseNameCustom);
        }

        private static string CreateFunctionID_SQL()
        {
            return String.Format(@"
                CREATE FUNCTION [dbo].[fnIndexdiff]
                               (@Input     VARCHAR(8000),
                                @Delimiter1 CHAR(5),
				                @Delimiter2 CHAR(5),
                                @Ordinal1   INT,
				                @Ordinal2   INT)
                RETURNS INT
                AS
                  BEGIN
                    DECLARE  @fstindex INT,
                             @Lstindex    INT,
                             @Noofchars   INT
                    SET @fstindex = {0}.dbo.fnNthIndex(@Input,@Delimiter1,@Ordinal1)
	                SET @Lstindex = {0}.dbo.fnNthIndex(@Input,@Delimiter2,@Ordinal2)
                    SET @Noofchars = @Lstindex-@fstindex
                    RETURN @Noofchars-1
                  END
                ", GlobalSettings.msSqlDatabaseNameCustom);
        }

        private static string CreateFunctionNI_SQL()
        {
            return @"
                CREATE FUNCTION [dbo].[fnNthIndex]
                               (@Input     VARCHAR(8000),
                                @Delimiter CHAR(1),
                                @Ordinal   INT)
                RETURNS INT
                AS
                  BEGIN
                    DECLARE  @Pointer INT,
                             @Last    INT,
                             @Count   INT
                    SET @Pointer = 1
                    SET @Last = 0
                    SET @Count = 1
                    WHILE (2 > 1)
                      BEGIN
                        SET @Pointer = CHARINDEX(@Delimiter,@Input,@Pointer)
                        IF @Pointer = 0
                          BREAK
                        IF @Count = @Ordinal
                          BEGIN
                            SET @Last = @Pointer
                            BREAK
                          END
                        SET @Count = @Count + 1
                        SET @Pointer = @Pointer + 1
                      END
                    RETURN @Last+1
                  END
                ";
        }

        private static string CreateModel_SQL()
        {
            return String.Format(@"
                BEGIN TRANSACTION
                CREATE TABLE {0}.dbo.TestRuns
                    (
                    ID int NOT NULL IDENTITY (1,1),
                    DOMAIN varchar(50) NOT NULL,
                    PROJECT varchar(50) NOT NULL,
				    RN_TEST_ID int,
                    TS_SUBJECT int,
                    RN_TEST_CONFIG_ID int,
                    RN_RUN_ID int NOT NULL,
                    RN_RUN_NAME varchar(255),
                    RN_EXECUTION_DATE datetime NOT NULL,
                    RN_EXECUTION_TIME datetime NOT NULL,
                    RN_DURATION int,
                    RN_STATE varchar(100),
                    RN_PC_START_TIME datetime NOT NULL,
                    RN_PC_END_TIME datetime NOT NULL,
                    RN_PC_CONTROLLER_NAME varchar(256),
                    RN_PC_LOAD_GENERATORS varchar(4000),
                    RN_PC_VUSERS_INVOLVED int,
                    RN_PC_VUSERS_MAX int,
                    RN_PC_VUSERS_AVERAGE int,
                    RN_PC_TOTAL_TRANSACT_PASSED int,
                    RN_PC_TOTAL_TRANSACT_FAILED int,
                    RN_PC_TOTAL_ERRORS int,
                    RN_PC_HITS_SEC_AVERAGE int,
                    RN_PC_THROUGHPUT_AVERAGE int,
                    RN_PC_TRANSACT_SEC_AVERAGE int
                    )  ON [PRIMARY]
                
                ALTER TABLE {0}.dbo.TestRuns ADD CONSTRAINT
	                PK_TestRuns PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                CREATE NONCLUSTERED INDEX IX_TestRun_RunID   
                    ON {0}.dbo.TestRuns (RN_RUN_ID);

                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE {0}.dbo.TestRunStats
	                (
	                ID int NOT NULL IDENTITY (1,1),
	                RunID int NOT NULL,
	                Metric varchar(50) NOT NULL,
	                TransactionName varchar(50) NOT NULL,
	                EndTime varchar(50) NOT NULL,
	                Value varchar(50) NOT NULL
	                )  ON [PRIMARY]
                ALTER TABLE {0}.dbo.TestRunStats ADD CONSTRAINT
	                PK_TestRunStats PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                CREATE NONCLUSTERED INDEX IX_TestRunStats_RunID   
                    ON {0}.dbo.TestRunStats (RunID);
                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE {0}.dbo.Monitor_meter
	                (
	                ID int NOT NULL IDENTITY (1,1),
	                RunID int NOT NULL,
	                [Event Name] varchar(250) NOT NULL,
	                [Event Type] varchar(50) NOT NULL,
	                EndTime float NOT NULL,
	                [Avg] float NOT NULL,
	                [Max] float NOT NULL,
	                [Min] float NOT NULL
	                )  ON [PRIMARY]
                ALTER TABLE {0}.dbo.Monitor_meter ADD CONSTRAINT
	                PK_Monitor_meter PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE {0}.dbo.TestTransactionSummary
	                (
	                ID int NOT NULL IDENTITY (1,1)
	                ,RunID int NOT NULL
                    ,[Event ID] int NOT NULL
	                ,[TransactionName] varchar(50)
	                ,[RT50Perc] float
					,[RT85Perc] float
                    ,[SLA50Perc] varchar(10)
                    ,[SLA85Perc] varchar(10)
					,[RTmin] float
					,[RTmax] float
					,[RTavg] float
					,[pass] int
					,[fail] int
	                )  ON [PRIMARY]
                ALTER TABLE {0}.dbo.TestTransactionSummary ADD CONSTRAINT
	                PK_TestTransactionSummary PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                CREATE NONCLUSTERED INDEX IX_TestTransactionSummary_RunID   
                    ON {0}.dbo.TestTransactionSummary (RunID);
                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE {0}.dbo.SiteScope_AspNet_Stats
	                (
	                ID int NOT NULL IDENTITY (1,1),
	                RunID int NOT NULL,
	                [TYPE] varchar(50),
	                [Server] varchar(50),
	                [Metrictype] varchar(50),
	                [Category] varchar(50),
	                [Instance] varchar(50),
	                [Counter] varchar(50),
	                [Time] float,
	                [Avg] float
	                )  ON [PRIMARY]
                ALTER TABLE {0}.dbo.SiteScope_AspNet_Stats ADD CONSTRAINT
	                PK_SiteScope_AspNet_Stats PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                CREATE NONCLUSTERED INDEX IX_SiteScope_AspNet_Stats_RunID   
                    ON {0}.dbo.SiteScope_AspNet_Stats (RunID);
                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE {0}.dbo.SiteScope_Server_Stats
	                (
	                ID int NOT NULL IDENTITY (1,1),
	                RunID int NOT NULL,
	                [TYPE] varchar(50),
	                [Server] varchar(50),
	                [Metrictype] varchar(50),
	                [Category] varchar(50),
	                [Instance] varchar(50),
	                [Counter] varchar(50),
	                [Time] float,
	                [Avg] float
	                )  ON [PRIMARY]
                ALTER TABLE {0}.dbo.SiteScope_Server_Stats ADD CONSTRAINT
	                PK_SiteScope_Server_Stats PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                CREATE NONCLUSTERED INDEX IX_SiteScope_Server_Stats_RunID   
                    ON {0}.dbo.SiteScope_Server_Stats (RunID);
                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE {0}.dbo.SiteScope_Database_Stats
	                (
	                ID int NOT NULL IDENTITY (1,1),
	                RunID int NOT NULL,
	                [TYPE] varchar(50),
	                [Server] varchar(50),
	                [Metrictype] varchar(50),
	                [Category] varchar(50),
	                [Instance] varchar(50),
	                [Counter] varchar(50),
	                [Time] float,
	                [Avg] float
	                )  ON [PRIMARY]
                ALTER TABLE {0}.dbo.SiteScope_Database_Stats ADD CONSTRAINT
	                PK_SiteScope_Database_Stats PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                CREATE NONCLUSTERED INDEX IX_SiteScope_Database_Stats_RunID   
                    ON {0}.dbo.SiteScope_Database_Stats (RunID);
                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE {0}.dbo.ALL_LISTS
	                (
                    DOMAIN varchar(50) NOT NULL,
                    PROJECT varchar(50) NOT NULL,
	                AL_ITEM_ID int NOT NULL,
	                AL_FATHER_ID int NOT NULL,
	                AL_DESCRIPTION varchar(255)
	                )  ON [PRIMARY]
                COMMIT
                ", GlobalSettings.msSqlDatabaseNameCustom);
        }

        private static string CopyMetrics_SQL(string runID)
        {
            return String.Format(@"
                     SELECT 
	                    '{0}' as [RunID]
	                    ,'Response Time' as [Metric] 
	                    ,evmp.[Event Name] as [TransactionName]
	                    ,[End time] as [EndTime]
	                    ,[Value]
                    FROM [Event_meter] evmr
                    INNER JOIN [Event_map] evmp ON evmr.[Event ID]=evmp.[Event ID]
                    WHERE  evmp.[Event Type]='Transaction'

                    union all

                    SELECT 
	                    '{0}' as [RunID]
	                    ,'Hits/Sec' as [Metric] 
	                    ,'Scenario' as [TransactionName]
	                    ,otbl.Endtime*10 as [EndTime]
	                    ,otbl.[Hits/Sec] as [Value] 
                    FROM (
	                    SELECT 
		                    Int([End Time]/10) as [Endtime]
		                    ,round(sum(AsumSq)/10,0) as [Hits/Sec]
	                    FROM [WebEvent_meter] 
	                    WHERE [Event ID]=(
		                    SELECT 
			                    [Event ID] 
		                    FROM [Event_map]  
		                    WHERE [Event Name]='HTTP_200'
	                    )
	                    GROUP BY Int([End Time]/10)
                    ) otbl 

                    union all

                    SELECT 
	                     '{0}' as [RunID]
	                    ,'Running Users' as [Metric] 
	                    ,'Scenario' as [TransactionName]
	                    ,otbl.Endtime as [EndTime]
	                    ,otbl.Users as [Value] 
                    FROM (
	                    SELECT itbl.[End Time] as [Endtime],count ([Vuser ID]) as [Users] 
	                    FROM [VuserEvent_meter] as itbl 
	                    WHERE [Vuser Status ID]=2 
	                    GROUP BY itbl.[End Time] 
	                    ) otbl 
                ", runID);
        }

        private static string AspNetStat_SQL(string runID)
        {
            return String.Format(@"
                    /**********  ASP .NET Stats ******************/
                    SELECT
                        [runID]
                        /*,'Asp.Net' as [StatType]*/
	                    ,'TYPE'=SUBSTRING(	
						                     datatbl.[Event Name]
						                    ,[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',2)
						                    ,[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',2,3))  
	                    ,'Server'=SUBSTRING(
						                     [Event Name]
						                    ,[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',4)
						                    ,[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',4,5))
	                    ,'Metrictype'=SUBSTRING(
						                     [Event Name]
						                    ,[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5)
						                    ,[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))
	                    ,'Category' = SUBSTRING(
						                     [Event Name]
						                    ,[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',6)
						                    ,[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','\',6,1))
	                    ,'Instance' = CASE
		                    WHEN len(datatbl.[Event Name]) - len(replace(datatbl.[Event Name], '\', '')) >1 
		                    THEN 
			                     SUBSTRING(datatbl.[Event Name]
			                    ,[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'\',1)
			                    ,[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'\','\',1,2))
		                    ELSE ''
		                    END
	                    ,'Counter' = CASE 
		                    WHEN len(datatbl.[Event Name]) - len(replace(datatbl.[Event Name], '\', '')) > 1 
		                    THEN
			                     SUBSTRING(datatbl.[Event Name]
			                    ,[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'\',2)
			                    ,len(datatbl.[Event Name]))
		                    ELSE
			                    SUBSTRING(datatbl.[Event Name]
			                    ,[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'\',1)
			                    ,len(datatbl.[Event Name]))
		                    END
	                    ,datatbl.[Endtime] as [Time]
	                    ,datatbl.[Avg] as [Avg]
                    FROM (
	                    SELECT 
							 runID
		                    ,prntbl.[Event Name]
		                    ,prntbl.Endtime
		                    ,prntbl.[Min]
		                    ,prntbl.[avg]
		                    ,prntbl.[Max] 
	                    FROM 
		                    [{0}].[dbo].[Monitor_meter] prntbl  
                        WHERE 
                            prntbl.runID = {1}
                    ) datatbl
                    WHERE 
	                    SUBSTRING(datatbl.[Event Name]
	                    ,[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5)
	                    ,[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6)) in ('Processstats','Webservicestats','APPpool_Wpstats','NETAPPstats','NETCLRStats','ASPNETstats')
                ", GlobalSettings.msSqlDatabaseNameCustom, runID);
        }

        private static string DbStat_SQL(string runID)
        {
            return String.Format(@"
                    select [runID]
                    /*,'Database' as [StatType]*/
                    ,'TYPE'=SUBSTRING(datatbl.[Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',2),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',2,3))  
                    ,'Server'=SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',4),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',4,5))
                    ,'Metrictype'=SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))
                    ,'Category' = CASE 
                    WHEN  SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))= 'SQLStats' 
                    then SUBSTRING(datatbl.[Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'|',1),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'|','-',1,1))
                    END
                    ,'' as [Instance]
                    ,'Counter' = CASE 
                    WHEN  SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))= 'SQLStats' then
                    CASE WHEN SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'-',2),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'-','-',2,3))=' SQL Re' then
                    SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'-',2),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'-','-',2,4))
                    else
                    SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'-',2),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'-','-',2,3))
                    END
                    END
                    ,datatbl.[Endtime] as [Time]
                    ,datatbl.[Avg] as [Avg]
                    from (
                    SELECT 
                    prntbl.runID
                    ,prntbl.[Event Name]
                    ,prntbl.Endtime
                    ,prntbl.[Min]
                    ,prntbl.[avg]
                    ,prntbl.[Max] 
                    FROM 
                        [{0}].[dbo].[Monitor_meter] prntbl
                    WHERE 
                        prntbl.runID = {1}
                    ) datatbl
                    where SUBSTRING(datatbl.[Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))= 'SQLStats'
                ", GlobalSettings.msSqlDatabaseNameCustom, runID);
        }

        private static string ServerStat_SQL(string runID)
        {
            return String.Format(@"
                    select [runID]
                    /*,'Server' as [StatType]*/
                    ,'TYPE'=SUBSTRING(datatbl.[Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',2),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',2,3))  
                    ,'Server'=SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',4),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',4,5))
                    ,'Metrictype'=SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))
                    ,'Category' = SUBSTRING([Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',6),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','\',6,1))
                    ,'Instance' = CASE
                    WHEN  len(datatbl.[Event Name]) - len(replace(datatbl.[Event Name], '\', '')) >1 then
                    SUBSTRING(datatbl.[Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'\',1),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'\','\',1,2))
                    ELSE ''
                    END
                    ,'Counter' = CASE 
                    WHEN len(datatbl.[Event Name]) - len(replace(datatbl.[Event Name], '\', '')) > 1 then
                    SUBSTRING(datatbl.[Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'\',2),len(datatbl.[Event Name]))
                    else
                    SUBSTRING(datatbl.[Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'\',1),len(datatbl.[Event Name]))
                    END
                    ,datatbl.[Endtime] as [Time]
                    ,datatbl.[Avg] as [Avg]
                    from (
                    SELECT 
                    prntbl.runID
                    ,prntbl.[Event Name]
                    ,prntbl.Endtime
                    ,prntbl.[Min]
                    ,prntbl.[avg]
                    ,prntbl.[Max] 
                    FROM 
                        [{0}].[dbo].[Monitor_meter] prntbl
                    WHERE 
                        prntbl.runID = {1}
                    ) datatbl
                    where  SUBSTRING(datatbl.[Event Name],[{0}].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[{0}].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))= 'ServerStats'
                ", GlobalSettings.msSqlDatabaseNameCustom, runID);
        }

        private static string RunFoldersPCDB_SQL(string domain, string project)
        {
            return String.Format(@"
                        SELECT
                             '{0}' as DOMAIN
                            ,'{1}' as PROJECT
                            ,[AL_ITEM_ID]
                            ,[AL_FATHER_ID]
                            ,[AL_DESCRIPTION]
                        FROM [{0}_{1}_db].[td].[ALL_LISTS]
                    ", domain, project);
        }

        private static string RunFoldersCustomDB_SQL(string domain, string project)
        {
            return String.Format(@"
                        SELECT
                             [DOMAIN]
                            ,[PROJECT]
                            ,[AL_ITEM_ID]
                            ,[AL_FATHER_ID]
                            ,[AL_DESCRIPTION]
                        FROM [{2}].[dbo].[ALL_LISTS]
                        WHERE [DOMAIN] = '{0}' AND [PROJECT] = '{1}'
                    ", domain, project, GlobalSettings.msSqlDatabaseNameCustom);
        }

        private static string TransactionSummary_SQL(string runID)
        {
            return String.Format(@"
                        SELECT 
	                            '{0}' as [RunID],
                                data50.[Event ID],data50.[Event Name] as [TransactionName],[RT50Perc],[RT85Perc],[RTmin],[RTmax],[RTavg],[pass],[fail]
                                ,IIF(InStr(data50.[Event Name], 'API'), IIF([RT50Perc]<=0.300, 'Passed', 'Failed'), IIF([RT50Perc]<=0.500, 'Passed', 'Failed')) as [SLA50Perc]
                                ,IIF(InStr(data50.[Event Name], 'API'), IIF([RT85Perc]<=0.500, 'Passed', 'Failed'), IIF([RT85Perc]<=0.800, 'Passed', 'Failed')) as [SLA85Perc]
                        FROM ((
                            SELECT 
	                             TOP 1 round(Value,4) AS [RT50Perc],irdata.[Event ID],[Event Name]
	                        FROM ( 
		                        SELECT 
			                            TOP 50 PERCENT Value,[Event ID]
		                        FROM 
			                        [Event_meter]  
		                        WHERE 
			                        [Tree Path ID]>=0
		                        ORDER BY Value ASC
	                        ) irdata INNER JOIN [Event_map] map ON irdata.[Event ID]=map.[Event ID]
	                        WHERE  map.[Event Type]='Transaction'
                        ) data50
                        INNER JOIN (
	                        SELECT 
		                         TOP 1 round(irdata.Value,4) AS [RT85Perc],irdata.[Event ID],[Event Name]
		                    FROM ( 
			                    SELECT 
				                        TOP 85 PERCENT Value,[Event ID]
			                    FROM 
				                    [Event_meter]  
			                    WHERE 
				                    [Tree Path ID]>=0
			                    ORDER BY Value ASC
		                    ) irdata INNER JOIN [Event_map] map ON irdata.[Event ID]=map.[Event ID]
		                    WHERE  map.[Event Type]='Transaction'
                        ) data85 ON data50.[Event ID] = data85.[Event ID])
                        INNER JOIN (
	                        SELECT
		                        [Event ID],round(MIN(Value),4) as [RTmin],round(MAX(Value),4) as [RTmax],round(AVG(Value),4) as [RTavg]
                                ,COUNT(Switch(Status1=1, 'Passed')) as [pass]
                                ,COUNT(Switch(Status1=0, 'Failed')) as [fail]
	                        FROM 
		                        [Event_meter] 
	                        WHERE 
		                        [Tree Path ID]>=0 and [Status1]=1
	                        GROUP BY
		                        [Event ID]
                        ) dataStat ON data50.[Event ID] = dataStat.[Event ID]
                    ", runID);
        }
    }
}