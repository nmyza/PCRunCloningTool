using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;

namespace PCRunCloningTool
{
    internal class DbManager
    {
        public static void PrepareDb()
        {
            if (!CheckDatabaseExistsMSSQL(GlobalSettings.GetConnectionStringWithoutDB(), GlobalSettings.msSqlDatabaseNameCustom))
            {
                CreateDB();
                CreateDbModel();
                CreateDbFuntions();
            }
                
        }

        public static void CopyDB(List<string> list, string reportsLocation, string domain, string project)
        {
            string location;
            string accessDbFilePath;
            Console.WriteLine("CopyDB");
            foreach (string runID in list)
            {
                if (RunExist(runID, domain, project))
                {
                    Console.WriteLine("Run exist !!!!!");
                }
                else
                {
                    location = reportsLocation + "/" + domain + "/" + project + "/" + runID + "/Reports";
                    accessDbFilePath = location + "/Results_" + runID + ".mdb";
                    CopyMetrics(GlobalSettings.GetConnectionStringCustomDB(), accessDbFilePath, runID);
                    CopySiteScopeMetrics(GlobalSettings.GetConnectionStringCustomDB(), accessDbFilePath, runID);
                    CopyRun(runID, domain, project);
                    TakeSiteScopeAspNetMetrics(GlobalSettings.GetConnectionStringCustomDB(), aspNetStatSQL);
                    TakeSiteScopeAspNetMetrics(GlobalSettings.GetConnectionStringCustomDB(), dbStatSQL);
                    TakeSiteScopeAspNetMetrics(GlobalSettings.GetConnectionStringCustomDB(), serverStatSQL);
                    Directory.Delete(location, true);
                }
            }
            
        }

        private static bool RunExist(string runID, string domain, string project)
        {
            string sql = @"
                            SELECT *
                            FROM [" + GlobalSettings.msSqlDatabaseNameCustom + @"].[dbo].[TestRuns]
                            WHERE [RunID] = " + runID;
            using (var targeDB = new SqlConnection(GlobalSettings.GetConnectionStringCustomDB()))
            using (SqlCommand command = new SqlCommand(sql, targeDB))
            {
                targeDB.Open();
                var reader = command.ExecuteReader();
                return reader.HasRows;
            }
        }
        private static void CopyRun(string runID, string domain, string project)
        {
            string runFromPcDbSQL = @"
                        SELECT '" + domain + "' as DOMAIN, '" + project + @"' as PROJECT
                              ,[RN_RUN_ID]
                              ,[RN_RUN_NAME]
                              ,[RN_EXECUTION_DATE]
                              ,[RN_EXECUTION_TIME]
                              ,[RN_STATE]
                              ,[RN_PC_START_TIME]
                              ,[RN_PC_END_TIME]
                              ,[RN_PC_RESERVATION_ID]
                          FROM [{0}_{1}_db].[td].[RUN]
                          WHERE [RN_RUN_ID] = " + runID;

            string sql = String.Format(runFromPcDbSQL, domain, project);
            using (var sourceDB = new SqlConnection(GlobalSettings.GetConnectionStringPCDB()))
            using (SqlCommand command = new SqlCommand(sql, sourceDB))
            {
                sourceDB.Open();
                var reader = command.ExecuteReader();
                using (var destinationdestinationDB = new SqlConnection(GlobalSettings.GetConnectionStringCustomDB()))
                {
                    destinationdestinationDB.Open();
                    using(var bulkCopy = new SqlBulkCopy(destinationdestinationDB))
                    {
                        bulkCopy.ColumnMappings.Add("DOMAIN", "Domain");
                        bulkCopy.ColumnMappings.Add("PROJECT", "Project");
                        bulkCopy.ColumnMappings.Add("RN_RUN_ID", "RunID");
                        bulkCopy.ColumnMappings.Add("RN_PC_START_TIME", "StartTime");
                        bulkCopy.ColumnMappings.Add("RN_PC_END_TIME", "EndTime");
                        bulkCopy.DestinationTableName = "PCAnalysisDashboard.dbo.TestRuns";
                        try
                        {
                            bulkCopy.WriteToServer(reader);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
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

                var commandSourceData = new OleDbCommand(String.Format(copyMetricsSQL, runID), sourceConnection);
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
                        bulkCopy.DestinationTableName = "PCAnalysisDashboard.dbo.TestRunStats";

                        try
                        {
                            bulkCopy.WriteToServer(reader);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
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

                var commandSourceData = new OleDbCommand(String.Format(siteScopeMetricsSQL, runID), sourceConnection);
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
                        bulkCopy.DestinationTableName = "PCAnalysisDashboard.dbo.Monitor_meter";

                        try
                        {
                            bulkCopy.WriteToServer(reader);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
        }

        private static void TakeSiteScopeAspNetMetrics(string connectionString, string sql)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var connection2 = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                connection2.Open();
                var reader = command.ExecuteReader();


                using (var bulkCopy = new SqlBulkCopy(connection2))
                {
                    bulkCopy.ColumnMappings.Add("RunID", "RunID");
                    bulkCopy.ColumnMappings.Add("StatType", "StatType");
                    bulkCopy.ColumnMappings.Add("TYPE", "TYPE");
                    bulkCopy.ColumnMappings.Add("Server", "Server");
                    bulkCopy.ColumnMappings.Add("Metrictype", "Metrictype");
                    bulkCopy.ColumnMappings.Add("Category", "Category");
                    bulkCopy.ColumnMappings.Add("Instance", "Instance");
                    bulkCopy.ColumnMappings.Add("Counter", "Counter");
                    bulkCopy.ColumnMappings.Add("Time", "Time");
                    bulkCopy.ColumnMappings.Add("Avg", "Avg");
                    bulkCopy.DestinationTableName = "PCAnalysisDashboard.dbo.SiteScopeStats";

                    try
                    {
                        bulkCopy.WriteToServer(reader);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
        }

        private static void DropDB()
        {
            ExecuteCommand(GlobalSettings.GetConnectionStringWithoutDB(), dropDbSQL);
        }
        private static void CreateDB()
        {
            ExecuteCommand(GlobalSettings.GetConnectionStringWithoutDB(), createDbSQL);
        }

        private static void CreateDbModel()
        {
            ExecuteCommand(GlobalSettings.GetConnectionStringCustomDB(), createModelSQL);
        }

        private static void CreateDbFuntions()
        {
            ExecuteCommand(GlobalSettings.GetConnectionStringCustomDB(), createFunctionIDSQL);
            ExecuteCommand(GlobalSettings.GetConnectionStringCustomDB(), createFunctionNISQL);
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

        public static List<Run> LoadRuns(string domain, string project)
        {
            string sql = String.Format(runsFromPcDbSQL, domain, project);

            List<Run> result = null;
            using (var connection = new SqlConnection(GlobalSettings.GetConnectionStringPCDB()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    SqlDataReader dataReader = command.ExecuteReader();

                    result = new List<Run>();
                    while (dataReader.Read())
                    {
                        result.Add(
                            new Run (
                                    dataReader.GetValue(0).ToString(),
                                    dataReader.GetValue(1).ToString(),
                                    dataReader.GetValue(2).ToString(),
                                    dataReader.GetValue(3).ToString(),
                                    dataReader.GetValue(4).ToString(),
                                    dataReader.GetValue(5).ToString(),
                                    dataReader.GetValue(6).ToString(),
                                    dataReader.GetValue(7).ToString()
                                )
                            );
                    }
                    dataReader.Close();
                }
            }
            return result;
        }

        private static void ExecuteCommand(string connectionString, string SQL)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(SQL, connection))
                {
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

        private static string siteScopeMetricsSQL = @"
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
                    ";

        private static string runsFromPcDbSQL = @"
                        SELECT [RN_RUN_ID]
                              ,[RN_RUN_NAME]
                              ,[RN_EXECUTION_DATE]
                              ,[RN_EXECUTION_TIME]
                              ,[RN_STATE]
                              ,[RN_PC_START_TIME]
                              ,[RN_PC_END_TIME]
                              ,[RN_PC_RESERVATION_ID]
                          FROM [{0}_{1}_db].[td].[RUN]
                    ";

        private static string dropDbSQL = @"
                If(db_id(N'PCAnalysisDashboard') IS NOT NULL) 
	                    drop database PCAnalysisDashboard;
                ";

        private static string createDbSQL = @"
                If(db_id(N'PCAnalysisDashboard') IS NULL)
	                create database PCAnalysisDashboard;
                ";

        private static string createFunctionIDSQL = @"
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
                    SET @fstindex = PCAnalysisDashboard.dbo.fnNthIndex(@Input,@Delimiter1,@Ordinal1)
	                SET @Lstindex = PCAnalysisDashboard.dbo.fnNthIndex(@Input,@Delimiter2,@Ordinal2)
                    SET @Noofchars = @Lstindex-@fstindex
                    RETURN @Noofchars-1
                  END
                ";

        private static string createFunctionNISQL = @"
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

        private static string createModelSQL = @"
                BEGIN TRANSACTION
                CREATE TABLE PCAnalysisDashboard.dbo.TestRuns
	                (
	                ID int NOT NULL IDENTITY (1,1),
	                Domain varchar(50) NOT NULL,
	                Project varchar(50) NOT NULL,
	                RunID int NOT NULL,
	                StartTime datetime NOT NULL,
	                EndTime datetime NOT NULL
	                )  ON [PRIMARY]
                
                ALTER TABLE PCAnalysisDashboard.dbo.TestRuns ADD CONSTRAINT
	                PK_TestRuns PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE PCAnalysisDashboard.dbo.TestRunStats
	                (
	                ID int NOT NULL IDENTITY (1,1),
	                RunID int NOT NULL,
	                Metric varchar(50) NOT NULL,
	                TransactionName varchar(50) NOT NULL,
	                EndTime varchar(50) NOT NULL,
	                Value varchar(50) NOT NULL
	                )  ON [PRIMARY]
                ALTER TABLE PCAnalysisDashboard.dbo.TestRunStats ADD CONSTRAINT
	                PK_TestRunStats PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE PCAnalysisDashboard.dbo.Monitor_meter
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
                ALTER TABLE PCAnalysisDashboard.dbo.Monitor_meter ADD CONSTRAINT
	                PK_Monitor_meter PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                COMMIT

                BEGIN TRANSACTION
                CREATE TABLE PCAnalysisDashboard.dbo.SiteScopeStats
	                (
	                ID int NOT NULL IDENTITY (1,1),
	                RunID int NOT NULL,
	                [StatType] varchar(50),
	                [TYPE] varchar(50),
	                [Server] varchar(50),
	                [Metrictype] varchar(50),
	                [Category] varchar(50),
	                [Instance] varchar(50),
	                [Counter] varchar(50),
	                [Time] float,
	                [Avg] float
	                )  ON [PRIMARY]
                ALTER TABLE PCAnalysisDashboard.dbo.SiteScopeStats ADD CONSTRAINT
	                PK_SiteScopeStats PRIMARY KEY CLUSTERED 
	                (
	                ID
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                COMMIT
                ";

        private static readonly String copyMetricsSQL = @"
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
                ";

        private static string aspNetStatSQL = @"
                    /**********  ASP .NET Stats ******************/
                    SELECT
                        [runID]
                        ,'Asp.Net' as [StatType]
	                    ,'TYPE'=SUBSTRING(	
						                     datatbl.[Event Name]
						                    ,[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',2)
						                    ,[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',2,3))  
	                    ,'Server'=SUBSTRING(
						                     [Event Name]
						                    ,[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',4)
						                    ,[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',4,5))
	                    ,'Metrictype'=SUBSTRING(
						                     [Event Name]
						                    ,[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5)
						                    ,[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))
	                    ,'Category' = SUBSTRING(
						                     [Event Name]
						                    ,[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',6)
						                    ,[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','\',6,1))
	                    ,'Instance' = CASE
		                    WHEN len(datatbl.[Event Name]) - len(replace(datatbl.[Event Name], '\', '')) >1 
		                    THEN 
			                     SUBSTRING(datatbl.[Event Name]
			                    ,[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'\',1)
			                    ,[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'\','\',1,2))
		                    ELSE ''
		                    END
	                    ,'Counter' = CASE 
		                    WHEN len(datatbl.[Event Name]) - len(replace(datatbl.[Event Name], '\', '')) > 1 
		                    THEN
			                     SUBSTRING(datatbl.[Event Name]
			                    ,[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'\',2)
			                    ,len(datatbl.[Event Name]))
		                    ELSE
			                    SUBSTRING(datatbl.[Event Name]
			                    ,[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'\',1)
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
		                    [PCAnalysisDashboard].[dbo].[Monitor_meter] prntbl  

                    ) datatbl
                    WHERE 
	                    SUBSTRING(datatbl.[Event Name]
	                    ,[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5)
	                    ,[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6)) in ('Processstats','Webservicestats','APPpool_Wpstats','NETAPPstats','NETCLRStats','ASPNETstats')
                ";

        private static string dbStatSQL = @"
                    select [runID]
                    ,'Database' as [StatType]
                    ,'TYPE'=SUBSTRING(datatbl.[Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',2),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',2,3))  
                    ,'Server'=SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',4),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',4,5))
                    ,'Metrictype'=SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))
                    ,'Category' = CASE 
                    WHEN  SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))= 'SQLStats' 
                    then SUBSTRING(datatbl.[Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'|',1),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'|','-',1,1))
                    END
                    ,'' as [Instance]
                    ,'Counter' = CASE 
                    WHEN  SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))= 'SQLStats' then
                    CASE WHEN SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'-',2),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'-','-',2,3))=' SQL Re' then
                    SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'-',2),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'-','-',2,4))
                    else
                    SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'-',2),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'-','-',2,3))
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
                    [PCAnalysisDashboard].[dbo].[Monitor_meter] prntbl
                    ) datatbl
                    where SUBSTRING(datatbl.[Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))= 'SQLStats'
                ";

        private static string serverStatSQL = @"
                    select [runID]
                    ,'Server' as [StatType]
                    ,'TYPE'=SUBSTRING(datatbl.[Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',2),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',2,3))  
                    ,'Server'=SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',4),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',4,5))
                    ,'Metrictype'=SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))
                    ,'Category' = SUBSTRING([Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',6),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','\',6,1))
                    ,'Instance' = CASE
                    WHEN  len(datatbl.[Event Name]) - len(replace(datatbl.[Event Name], '\', '')) >1 then
                    SUBSTRING(datatbl.[Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'\',1),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'\','\',1,2))
                    ELSE ''
                    END
                    ,'Counter' = CASE 
                    WHEN len(datatbl.[Event Name]) - len(replace(datatbl.[Event Name], '\', '')) > 1 then
                    SUBSTRING(datatbl.[Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'\',2),len(datatbl.[Event Name]))
                    else
                    SUBSTRING(datatbl.[Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'\',1),len(datatbl.[Event Name]))
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
                    [PCAnalysisDashboard].[dbo].[Monitor_meter] prntbl
                    ) datatbl
                    where  SUBSTRING(datatbl.[Event Name],[PCAnalysisDashboard].[dbo].[fnNthIndex](datatbl.[Event Name],'/',5),[PCAnalysisDashboard].[dbo].[fnIndexdiff](datatbl.[Event Name],'/','/',5,6))= 'ServerStats'
                ";
    }

    public class Run
    {
        public string RN_RUN_ID;
        public string RN_RUN_NAME;
        public string RN_EXECUTION_DATE;
        public string RN_EXECUTION_TIME;
        public string RN_STATE;
        public string RN_PC_START_TIME;
        public string RN_PC_END_TIME;
        public string RN_PC_RESERVATION_ID;

        public Run(
                string RN_RUN_ID,
                string RN_RUN_NAME,
                string RN_EXECUTION_DATE,
                string RN_EXECUTION_TIME,
                string RN_STATE,
                string RN_PC_START_TIME,
                string RN_PC_END_TIME,
                string RN_PC_RESERVATION_ID)
        {
            this.RN_RUN_ID = RN_RUN_ID;
            this.RN_RUN_NAME = RN_RUN_NAME;
            this.RN_EXECUTION_DATE = RN_EXECUTION_DATE;
            this.RN_EXECUTION_TIME = RN_EXECUTION_TIME;
            this.RN_STATE = RN_STATE;
            this.RN_PC_START_TIME = RN_PC_START_TIME;
            this.RN_PC_END_TIME = RN_PC_END_TIME;
            this.RN_PC_RESERVATION_ID = RN_PC_RESERVATION_ID;
        }
    }
}