using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace PCRunCloningTool
{
    public partial class Form1 : Form
    {
        
        private static String DOMAIN = "";
        private static String PROJECT = "";
        private static readonly String SERVER_URL = "http://pctrappprodvh2";
        private static readonly String PC_AUTH_URI = "/LoadTest/rest/authentication-point/authenticate";
        //private static readonly String PC_LOGOUT_URI = "/LoadTest/rest/authentication-point/logout";
        private static String REPORTS_LOCATION = "//mir/qa/temp/nazamyz";
        private static readonly XNamespace NS = "http://www.hp.com/PC/REST/API";

        private static readonly CookieContainer cookies = new CookieContainer();
        private static readonly HttpClientHandler handler = new HttpClientHandler();
        private static readonly HttpClient client = null;
        private static readonly String RUN_TABLE_NAME = "TestRuns";
        private static readonly String FOLDERS_TABLE_NAME = "Folders";
        private static readonly String RUN_STATE_FINISHED = "Finished";
        private DataSet runsDataSet = new DataSet();
        private DataSet foldersDataSet = new DataSet();

        static int workingCounter = 0;
        static int workingLimit = 10;
        static int processedCounter = 0;


        static Form1() {
            handler.CookieContainer = cookies;
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic bG9hZHRlc3RlcjpDcm93bjIwMDk=");
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

        }
        public Form1()
        {
            InitializeComponent();

            AuthorizePC();

            loadDomains();

            Logger.InitLogger();
            Logger.Log.Info("Application has started!");
        }

        private static void AuthorizePC()
        {
            client.GetAsync(SERVER_URL + PC_AUTH_URI);
        }

        //***************** UI **********************/

        public void Form1_Shown(object sender, EventArgs e)
        {
            if (!DbManager.CheckDatabaseExistsMSSQL(GlobalSettings.GetConnectionStringWithoutDB(), GlobalSettings.msSqlDatabaseNameCustom))
            {
                var form = new SettingsForm();
                form.Show();
                form.Focus();
            }
        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            var list = GetselectedRunList();
            REPORTS_LOCATION = folderPathBox.Text;
            
            if (unzipCheckBox.Checked)
            {
                CopyRuns2(list);
            }
        }

        private void downloadAllBtn_Click(object sender, EventArgs e)
        {
            List<string> result = new List<string>();
            foreach (DataRow row in runsDataSet.Tables[RUN_TABLE_NAME].Rows)
            {
                result.Add(row["RN_RUN_ID"].ToString());
            }
            CopyRuns2(result);
        }

        private async void CopyRuns(List<string> runs)
        {
            if (!DbManager.CheckDatabaseExistsMSSQL(GlobalSettings.GetConnectionStringWithoutDB(), GlobalSettings.msSqlDatabaseNameCustom))
                MessageBox.Show("Database does not exist: " + GlobalSettings.msSqlDatabaseNameCustom);
            else
            {
                foreach (string runID in runs)
                    if (!IsRunFinished(runID))
                    {
                        Logs("Run ID: " + runID + " is not Finished");
                        continue;
                    }
                    else
                    if (!DbManager.RunExist(runID, DOMAIN, PROJECT))
                    {
                        await CopyRun(runID);
                    }
                    else
                    {
                        Logs("Run is copied. ID: " + runID);
                        Console.WriteLine("Run is already in DB. ID: " + runID + DbManager.GetIdOfCopiedRun(runID, DOMAIN, PROJECT));
                    }
            }
        }


        private async void CopyRunupd(object runID)
        {
            try
            {
                string strrunID = Convert.ToString(runID);
                if (!IsRunFinished(strrunID))
                {
                    Logs("Run ID: " + strrunID + " is not Finished");
                }
                else
                if (!DbManager.RunExist(strrunID, DOMAIN, PROJECT))
                {
                    await CopyRun(strrunID);
                }
                else
                {
                    Logs("Run is copied. ID: " + runID);
                    Console.WriteLine("Run is already in DB. ID: " + runID);
                }
            }
            catch (Exception ex)
            {
                //handle your exception...
                string exMsg = ex.Message;
            }
            finally
            {
                Interlocked.Decrement(ref workingCounter);
                Interlocked.Increment(ref processedCounter);
            }

        }

        private void CopyRuns2(List<string> runs)
        {

            if (!DbManager.CheckDatabaseExistsMSSQL(GlobalSettings.GetConnectionStringWithoutDB(), GlobalSettings.msSqlDatabaseNameCustom))
                MessageBox.Show("Database does not exist: " + GlobalSettings.msSqlDatabaseNameCustom);
            else
            {
                int checkCount = runs.Count;
                foreach (string runID in runs)
                {

                    while (workingCounter >= workingLimit)
                    {
                        Thread.Sleep(100);
                    }

                    workingCounter += 1;
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(CopyRunupd);
                    Thread th = new Thread(pts);
                    th.Start(runID);

                }
                while (processedCounter < checkCount)
                {
                    Thread.Sleep(100);
                }
                Console.WriteLine("Work completed!");
            }
        }




        private void Logs(string message)
        {
            //consoleBox.Text += message + Environment.NewLine;
            // set the current caret position to the end
            //consoleBox.SelectionStart = consoleBox.Text.Length;
            // scroll it automatically
            //consoleBox.ScrollToCaret();
        }

        private Task CopyRun(string ID)
        {
            return Task.Run(async () =>
            {
                TestRunResults run = await DownloadResults(ID);
                UnzipDownloadedResults(ID, run.AnalysedResults.Name);
                DbManager.CopyDB(run, REPORTS_LOCATION, DOMAIN, PROJECT, GetProjectsMap(DOMAIN)[PROJECT]);
                Console.WriteLine("Copied successful. ID: " + ID);
            });
            
        }

        private void domainComBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DOMAIN = domainComBox.Text;
            checkedListBox1.Items.Clear();
            if (DOMAIN != "")
            {
                loadProjects(DOMAIN);
                projectComBox.Enabled = true;
            }
            DisableComboBoxes(firstLevelFolderComBox, secondLevelFolderComBox, threeLevelFolderComBox, fourLevelFolderComBox);
            DisableComboBoxLabels(labelProduct, labelRelease, labelVersion, labelSubVersion);
        }

        private void projectComBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            PROJECT = projectComBox.Text;
            firstLevelFolderComBox.Enabled = true;

            foldersDataSet.Clear();
            DbManager.GetFolderStructure(foldersDataSet, FOLDERS_TABLE_NAME, DOMAIN, PROJECT, GetProjectsMap(DOMAIN)[PROJECT]);
            FillFirstLevelFolderComBox(foldersDataSet, FOLDERS_TABLE_NAME);
            DisableComboBoxes(secondLevelFolderComBox, threeLevelFolderComBox, fourLevelFolderComBox);
            DisableComboBoxLabels(labelRelease, labelVersion, labelSubVersion);
        }

        private void firstLevelFolderComBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillSecondLevelFolderComBox(foldersDataSet, FOLDERS_TABLE_NAME, GetFolderID(firstLevelFolderComBox.Text));
            DisableComboBoxes(threeLevelFolderComBox, fourLevelFolderComBox);
            DisableComboBoxLabels(labelVersion, labelSubVersion);
        }

        private void secondLevelFolderComBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillThreeLevelFolderComBox(foldersDataSet, FOLDERS_TABLE_NAME, GetFolderID(secondLevelFolderComBox.Text));
            DisableComboBoxes(fourLevelFolderComBox);
            DisableComboBoxLabels(labelSubVersion);
        }

        private void DisableComboBoxes(params ComboBox [] comboBoxes)
        {
            foreach (ComboBox box in comboBoxes)
            {
                box.Items.Clear();
                box.Text = "";
                box.Enabled = false;
                box.Visible = false;
            }
        }

        private void DisableComboBoxLabels(params Label [] labels)
        {
            foreach (Label label in labels)
            {
                label.Visible = false;
            }
        }

        private void folderDialogBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderPathBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            var form = new SettingsForm();
            if (Application.OpenForms[form.Name] == null)
                form.ShowDialog();
            else
                Application.OpenForms[form.Name].Focus();
        }

        private void loadRunsBtn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(DOMAIN)) { MessageBox.Show("Domain is not selected"); return;}
            if (String.IsNullOrEmpty(PROJECT)) { MessageBox.Show("Project is not selected"); return;}

            string parrentFolderIDs;
            if (!String.IsNullOrEmpty(fourLevelFolderComBox.Text))
                parrentFolderIDs = CalcFolderIDs(fourLevelFolderComBox.Text);
            else if (!String.IsNullOrEmpty(threeLevelFolderComBox.Text))
                parrentFolderIDs = CalcFolderIDs(threeLevelFolderComBox.Text);
            else if (!String.IsNullOrEmpty(secondLevelFolderComBox.Text))
                parrentFolderIDs = CalcFolderIDs(secondLevelFolderComBox.Text);
            else if (!String.IsNullOrEmpty(firstLevelFolderComBox.Text))
                parrentFolderIDs = CalcFolderIDs(firstLevelFolderComBox.Text);
            else
                parrentFolderIDs = "";

            runsDataSet.Clear();
            DbManager.LoadRunsFromDB(runsDataSet, RUN_TABLE_NAME, DOMAIN, PROJECT, parrentFolderIDs, GetProjectsMap(DOMAIN)[PROJECT]);

            ShowRuns(runsDataSet, RUN_TABLE_NAME);
            
            testRunsGridView.DataSource = runsDataSet;
            testRunsGridView.DataMember = RUN_TABLE_NAME;
        }

        private string CalcFolderIDs(string rootFolderName)
        {
            var rootFolderID = GetFolderID(rootFolderName);
            LinkedList<string> list = new LinkedList<string>();
            list.AddFirst(rootFolderID);
            LinkedListNode<string> node = list.First;
            while (node != null)
            {
                foreach (DataRow row in foldersDataSet.Tables[FOLDERS_TABLE_NAME].Rows) // AL_ITEM_ID, AL_FATHER_ID, AL_DESCRIPTION 
                {
                    if (node.Value.Equals(row["AL_FATHER_ID"].ToString()))
                        list.AddLast(row["AL_ITEM_ID"].ToString());
                }
                node = node.Next;
            }

            // MessageBox.Show(String.Join(",", list.ToArray()));
            return String.Join(",", list.ToArray());
        }

        private string GetFolderID(string name)
        {
            foreach(DataRow row in foldersDataSet.Tables[FOLDERS_TABLE_NAME].Rows) // AL_ITEM_ID, AL_FATHER_ID, AL_DESCRIPTION
            {
                if (name.Equals(row["AL_DESCRIPTION"].ToString()))
                    return row["AL_ITEM_ID"].ToString();
            }
            return "2";
        }

        private void FillFirstLevelFolderComBox(DataSet data, string member)
        {
            FillComBox(firstLevelFolderComBox, labelProduct, data, member, "2");
        }

        private void FillSecondLevelFolderComBox(DataSet data, string member, string parrentFolderID)
        {
            FillComBox(secondLevelFolderComBox, labelRelease, data, member, parrentFolderID);
        }

        private void FillThreeLevelFolderComBox(DataSet data, string member, string parrentFolderID)
        {
            FillComBox(threeLevelFolderComBox, labelVersion, data, member, parrentFolderID);
        }

        private void FillComBox(ComboBox comboBox, Label label, DataSet data, string member, string parrentFolderID)
        {
            if ("0".Equals(parrentFolderID))
            {
                DisableComboBoxes(comboBox);
                DisableComboBoxLabels(label);
                return;
            }
            comboBox.Items.Clear();
            comboBox.Text = "";
            comboBox.Items.Add("");
            foreach (DataRow row in data.Tables[member].Rows)
            {
                var folderName = row["AL_DESCRIPTION"].ToString();
                if (parrentFolderID.Equals(row["AL_FATHER_ID"].ToString()) && !"Scenarios".Equals(folderName) && !"Scripts".Equals(folderName))
                {
                    comboBox.Items.Add(folderName);
                }
            }

            comboBox.Enabled = comboBox.Items.Count > 1;
            comboBox.Visible = comboBox.Items.Count > 1;
            label.Visible = comboBox.Items.Count > 1;
        }

        private bool IsRunFinished(string ID)
        {
            foreach (DataRow row in runsDataSet.Tables[RUN_TABLE_NAME].Rows)
            {
                if (ID.Equals(row["RN_RUN_ID"].ToString()))
                {
                    return RUN_STATE_FINISHED.Equals(row["RN_STATE"].ToString());
                }
            }
            return false;
        }

        private void ShowRuns(DataSet data, string tableName)
        {
            checkedListBox1.Items.Clear();
            foreach (DataRow row in data.Tables[tableName].Rows)
            {
                checkedListBox1.Items.Add(row["RN_RUN_ID"].ToString());
            }
        }

        //************* Controller *******************/

        private List<string> GetselectedRunList()
        {
            List<string> runIdList = new List<string>();
            if (checkedListBox1.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBox1.CheckedItems.Count - 1; x++)
                {
                    runIdList.Add(checkedListBox1.CheckedItems[x].ToString());
                }
            }
            return runIdList;
        }
        
        private static void UnzipDownloadedResults(string runID, String reportName)
        {
            // REPORTS_LOCATION = folderPathBox.Text;
            string location;
            string filePath;
            location = REPORTS_LOCATION + "/" + DOMAIN + "/" + PROJECT + "/" + runID + "/";
            filePath = location + reportName;
            Utils.Unzip(filePath, location + "Reports");
        }

        private void loadDomains()
        {
                foreach (string domain in DbManager.LoadDomains())
                {
                    domainComBox.Items.Add(domain);
                }
        }

        private void loadProjects(string domain)
        {
            projectComBox.Items.Clear();
            projectComBox.Text = "";
            firstLevelFolderComBox.Items.Clear();
            firstLevelFolderComBox.Text = "";
            secondLevelFolderComBox.Items.Clear();
            secondLevelFolderComBox.Text = "";
            //foreach (string project in DbManager.LoadProjects(domain))
            foreach (string project in GetProjectList(domain))
                projectComBox.Items.Add(project);            
        }

        Dictionary<string, Dictionary<string, string>> domainProjetcDatabaseMap = new Dictionary<string, Dictionary<string, string>>();

        private List<string> GetProjectList(string domain)
        {
            return GetProjectsMap(domain).Keys.ToList();
        }

        private Dictionary<string, string> GetProjectsMap(string domain)
        {
            if (!domainProjetcDatabaseMap.TryGetValue(domain, out Dictionary<string, string> projects))
            {
                projects = LoadProjectsData(domain);
                domainProjetcDatabaseMap.Add(domain, projects);
            }
            return projects;
        }

        private static Dictionary<string, string> LoadProjectsData(string domain)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            List<List<string>> projects = DbManager.LoadProjectsData(domain);
            foreach(List<string> project in projects)
            {
                result.Add(project[0], project[1]);
            }
            return result;
        }

        private async void LoadRunsByREST()
        {
            AuthorizePC();
            HttpResponseMessage response = client.GetAsync(SERVER_URL + "/LoadTest/rest/domains/" + DOMAIN + "/projects/" + PROJECT + "/Runs").Result;
            var content = await response.Content.ReadAsStringAsync();

            XNamespace ns = "http://www.hp.com/PC/REST/API";
            XElement runs = XElement.Parse(content);
            checkedListBox1.Items.Clear();
            foreach (XElement run in runs.Elements())
            {
                checkedListBox1.Items.Add(run.Element(ns + "ID").Value);
            }
        }

        public static async Task<TestRunResults> DownloadResults(string runID)
        {
            TestRunResults runResult = await GetRunMetaData(runID);
            Console.WriteLine("RunResult " + runResult.ToString());
            DownloadReport(runID, runResult.AnalysedResults.Id, runResult.AnalysedResults.Name);
            return runResult;
        }

        private static async Task<TestRunResults> GetRunMetaData(String id)
        {
            AuthorizePC();
            HttpResponseMessage response = client.GetAsync(SERVER_URL + "/LoadTest/rest/domains/" + DOMAIN + "/projects/" + PROJECT + "/Runs/" + id + "/Results/").Result;
            var content = await response.Content.ReadAsStringAsync();
            XElement runs = XElement.Parse(content);
            TestRunResults testResults = new TestRunResults();
            foreach (XElement run in runs.Elements())
            {
                Console.WriteLine("Run: " + run.ToString());
                TestRunReport testRunReport = new TestRunReport(
                    run.Element(NS + "ID").Value,
                    run.Element(NS + "Name").Value,
                    run.Element(NS + "Type").Value,
                    run.Element(NS + "RunID").Value);

                switch (run.Element(NS + "Type").Value)
                {
                    case "OUTPUT LOG":
                        if ("output.mdb.zip" == run.Element(NS + "Name").Value) {
                            testResults.OutputLogMdb = testRunReport;
                        } else {
                            testResults.OutputLogTxt = testRunReport;
                        }
                        break;
                    case "RAW RESULTS":
                        testResults.RawResults = testRunReport;
                        break;
                    case "ANALYZED RESULT":
                        testResults.AnalysedResults = testRunReport;
                        break;
                    case "HTML REPORT":
                        testResults.HtmlReport = testRunReport;
                        break;
                }
            }
            return testResults;
        }
                
        private static void DownloadReport(String runID, String resultId, String name)
        {
            AuthorizePC();
            string location = REPORTS_LOCATION + "/" + DOMAIN + "/" + PROJECT + "/" + runID + "/";
            string filePath = location + name;
            string reportURI = SERVER_URL + "/LoadTest/rest/domains/" + DOMAIN + "/projects/" + PROJECT + "/Runs/" + runID + "/Results/" + resultId + "/data";

            if (File.Exists(filePath)) return;
            
            if (!Directory.Exists(location))
                Directory.CreateDirectory(location);
            
            WebClient myWebClient = new WebClient();
            myWebClient.Headers.Add("User-Agent", ".NET Foundation Repository Reporter");

            var cookiesStr = "";

            Uri uri = new Uri(SERVER_URL + PC_AUTH_URI);
            IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
            {
                if (cookiesStr != "") cookiesStr = cookiesStr + ";";
                cookiesStr += cookie.Name + "=" + cookie.Value;
            }
            myWebClient.Headers.Add(HttpRequestHeader.Cookie, cookiesStr);

            myWebClient.DownloadFile(reportURI, filePath);
        }

    }

    public class TestRunReport
    {
        public string Id;
        public string Name;
        public string Type;
        public string RunId;

        public TestRunReport(string id, string name, string type, string runId)
        {
            Id = id;
            Name = name;
            Type = type;
            RunId = runId;
        }
    }

    public class TestRunResults
    {

        public TestRunReport OutputLogMdb { get; set; }
        public TestRunReport OutputLogTxt;
        public TestRunReport RawResults;
        public TestRunReport AnalysedResults;
        public TestRunReport HtmlReport;
    }
}
