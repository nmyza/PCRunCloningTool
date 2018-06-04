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

            client.GetAsync(SERVER_URL + PC_AUTH_URI);

            loadDomains();
        }

        //***************** UI **********************/

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            var list = GetselectedRunList();
            REPORTS_LOCATION = folderPathBox.Text;
            DownloadResults(list);
            if (unzipCheckBox.Checked)
            {
                if (!DbManager.CheckDatabaseExistsMSSQL(GlobalSettings.GetConnectionStringWithoutDB(), GlobalSettings.msSqlDatabaseNameCustom))
                    MessageBox.Show("Database does not exist: " + GlobalSettings.msSqlDatabaseNameCustom);
                else
                {
                    UnzipDownloadedResults(list, "Reports.zip");
                    DbManager.CopyDB(list, REPORTS_LOCATION, DOMAIN, PROJECT);
                }
            }
        }

        private void pcDbConnectBtn_Click(object sender, EventArgs e)
        {
            loadDomains();
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
            // MessageBox.Show(box.Text);
        }

        private void projectComBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            PROJECT = projectComBox.Text;
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
            new SettingsForm().Show();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            LoadRuns();
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
        

        private void UnzipDownloadedResults(List<string> list, String reportName)
        {
            REPORTS_LOCATION = folderPathBox.Text;
            string location;
            string filePath;

            foreach (string runID in list)
            {
                location = REPORTS_LOCATION + "/" + DOMAIN + "/" + PROJECT + "/" + runID + "/";
                filePath = location + reportName;
                Utils.Unzip(filePath, location + "Reports");
            }

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
            foreach (string project in DbManager.LoadProjects(domain))
                projectComBox.Items.Add(project);            
        }

        private async void LoadRuns()
        { 
            if (DOMAIN == "") 
            {
                MessageBox.Show("Domain is not selected");
                return;
            }

            if (PROJECT == "")
            {
                MessageBox.Show("Project is not selected");
                return;
            }

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

        private async void DownloadResults(List<string> runIdList)
        {

            foreach (string runId in runIdList)
            {
                TestRunResults runResult = await GetRunMetaData(runId);
                DownloadReport(runId, runResult.AnalysedResults.Id, "Reports.zip");
            }
        }

        private async Task<TestRunResults> GetRunMetaData(String id)
        {
            HttpResponseMessage response = client.GetAsync(SERVER_URL + "/LoadTest/rest/domains/" + DOMAIN + "/projects/" + PROJECT + "/Runs/" + id + "/Results/").Result;
            var content = await response.Content.ReadAsStringAsync();

            
            XElement runs = XElement.Parse(content);
            TestRunResults testResults = new TestRunResults();
            foreach (XElement run in runs.Elements())
            {
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
            // MessageBox.Show("Runs count: " + testResults.);
            return testResults;
        }
                
        private void DownloadReport(String runID, String resultId, String name)
        {
            REPORTS_LOCATION = folderPathBox.Text;
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
