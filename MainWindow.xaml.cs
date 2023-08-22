using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web;
using System.Reflection.Emit;
using System.Data;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Media.Converters;
using System.Runtime.CompilerServices;

using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Net.WebRequestMethods;

using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Controls.WindowsForms;
using Microsoft.WindowsAPICodePack.Shell;

namespace MiteneLoader
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // "https://mitene.us/f/bO6hd7QDdb4"
        DataTable miteneData;
        string Page_Source;
        int page_count = 1;
        string nowDownloadPath = "";
        readonly CountdownEvent condition = new CountdownEvent(1);
        bool isDownload = false;
        bool endofData = false;
        bool readStart = false;
        bool isMiteneDataPage = false;
        bool isMiteneLoginPage = false;

        int DuplicateCount = 0;
        CoreWebView2DownloadOperation downloadOperation;

        public MainWindow()
        {
            InitializeComponent();


            loadSetting();


            InitAsync();

            miteneData = new DataTable();
            miteneData.Columns.Add("downloadUrl");
            miteneData.Columns.Add("id");
            miteneData.Columns.Add("uuid");
            miteneData.Columns.Add("userId");
            miteneData.Columns.Add("mediaType");
            miteneData.Columns.Add("originalHash");
            miteneData.Columns.Add("hasComment");
            miteneData.Columns.Add("comments");
            miteneData.Columns.Add("footprints");
            miteneData.Columns.Add("tookAt");
            miteneData.Columns.Add("audienceType");
            miteneData.Columns.Add("mediaWidth");
            miteneData.Columns.Add("mediaHeight");
            miteneData.Columns.Add("mediaOrientation");
            miteneData.Columns.Add("latitude");
            miteneData.Columns.Add("longitude");
            miteneData.Columns.Add("mediaDeviceModel");
            miteneData.Columns.Add("deviceFilePath");
            miteneData.Columns.Add("videoDuration");
            miteneData.Columns.Add("contentType");
            miteneData.Columns.Add("origin");
            miteneData.Columns.Add("thumbnailGenerated");
            miteneData.Columns.Add("expiringUrl");
            miteneData.Columns.Add("expiringVideoUrl");
            miteneData.Columns.Add("fileExist",typeof(bool));



    }
    private async void InitAsync()
        {
            await MiteneWebView.EnsureCoreWebView2Async();

            // MiteneWebView2 Runtime Version
            Debug.Print($"BrowserVersionString = {MiteneWebView.CoreWebView2.Environment.BrowserVersionString}");

            //// MiteneWebView
            //MiteneWebView.ContentLoading += MiteneWebViewOnContentLoading;
            //MiteneWebView.CoreWebView2InitializationCompleted += MiteneWebViewOnCoreWebView2InitializationCompleted;
            //MiteneWebView.NavigationCompleted += MiteneWebViewOnNavigationCompleted;
            //MiteneWebView.NavigationStarting += MiteneWebViewOnNavigationStarting;
            //MiteneWebView.SourceChanged += MiteneWebViewOnSourceChanged;
            //MiteneWebView.WebMessageReceived += MiteneWebViewOnWebMessageReceived;
            //MiteneWebView.ZoomFactorChanged += MiteneWebViewOnZoomFactorChanged;

            //// MiteneWebView.CoreWebView2

            //MiteneWebView.CoreWebView2.BasicAuthenticationRequested += CoreWebView2OnBasicAuthenticationRequested;
            //MiteneWebView.CoreWebView2.ClientCertificateRequested += CoreWebView2OnClientCertificateRequested;
            //MiteneWebView.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2OnContainsFullScreenElementChanged;
            //MiteneWebView.CoreWebView2.ContentLoading += CoreWebView2OnContentLoading;
            //MiteneWebView.CoreWebView2.ContextMenuRequested += CoreWebView2OnContextMenuRequested;
            //MiteneWebView.CoreWebView2.DocumentTitleChanged += CoreWebView2OnDocumentTitleChanged;
            //MiteneWebView.CoreWebView2.DOMContentLoaded += CoreWebView2OnDOMContentLoaded;
            MiteneWebView.CoreWebView2.DownloadStarting += CoreWebView2OnDownloadStarting;
            //MiteneWebView.CoreWebView2.FrameCreated += CoreWebView2OnFrameCreated;
            //MiteneWebView.CoreWebView2.FrameNavigationCompleted += CoreWebView2OnFrameNavigationCompleted;
            //MiteneWebView.CoreWebView2.FrameNavigationStarting += CoreWebView2OnFrameNavigationStarting;
            //MiteneWebView.CoreWebView2.HistoryChanged += CoreWebView2OnHistoryChanged;
            //MiteneWebView.CoreWebView2.IsDefaultDownloadDialogOpenChanged += CoreWebView2OnIsDefaultDownloadDialogOpenChanged;
            //MiteneWebView.CoreWebView2.IsDocumentPlayingAudioChanged += CoreWebView2OnIsDocumentPlayingAudioChanged;
            //MiteneWebView.CoreWebView2.IsMutedChanged += CoreWebView2OnIsMutedChanged;
            MiteneWebView.CoreWebView2.NavigationCompleted += CoreWebView2OnNavigationCompleted;
            //MiteneWebView.CoreWebView2.NavigationStarting += CoreWebView2OnNavigationStarting;
            //MiteneWebView.CoreWebView2.NewWindowRequested += CoreWebView2OnNewWindowRequested;
            //MiteneWebView.CoreWebView2.PermissionRequested += CoreWebView2OnPermissionRequested;
            //MiteneWebView.CoreWebView2.ProcessFailed += CoreWebView2OnProcessFailed;
            //MiteneWebView.CoreWebView2.ScriptDialogOpening += CoreWebView2OnScriptDialogOpening;
            //MiteneWebView.CoreWebView2.ServerCertificateErrorDetected += CoreWebView2OnServerCertificateErrorDetected;
            //MiteneWebView.CoreWebView2.SourceChanged += CoreWebView2OnSourceChanged;
            //MiteneWebView.CoreWebView2.StatusBarTextChanged += CoreWebView2OnStatusBarTextChanged;
            //MiteneWebView.CoreWebView2.WebMessageReceived += CoreWebView2OnWebMessageReceived;
            //MiteneWebView.CoreWebView2.WebResourceRequested += CoreWebView2OnWebResourceRequested;
            //MiteneWebView.CoreWebView2.WebResourceResponseReceived += CoreWebView2OnWebResourceResponseReceived;
            //MiteneWebView.CoreWebView2.WindowCloseRequested += CoreWebView2OnWindowCloseRequested;

            //var evr = MiteneWebView.CoreWebView2.GetDevToolsProtocolEventReceiver("Page.downloadProgress");
            //evr.DevToolsProtocolEventReceived += (object sender, CoreWebView2DevToolsProtocolEventReceivedEventArgs e) =>
            //{
            //    Debug.WriteLine(e.ParameterObjectAsJson);
            //};

            try
            {
                MiteneWebView.Source = new Uri(Page_Source);
            }
            catch
            {

            }

          

        }


        private void loadSetting()
        {
            TxtSharedURL.Text = Properties.Settings.Default.Shared_URL;
            TxtFolderPath.Text = Properties.Settings.Default.Storage_Folder;
            ChkYearMonthFolder.IsChecked = (Properties.Settings.Default.SubFolder_Type == 1);
            Page_Source = TxtSharedURL.Text;

            string dirPath = TxtFolderPath.Text;
            if (!Directory.Exists(dirPath))
            {
                MessageEx.ShowWarningDialog("指定の保存フォルダーが存在しません。\n保存フォルダーを選択してください。", Window.GetWindow(this));
                return;
            }
            else
            {
                try
                {
                    FileBrowser.Navigate(ShellFileSystemFolder.FromFolderPath(Properties.Settings.Default.Storage_Folder));  // フォルダーがないとエラー
                }
                catch
                {
                    MessageEx.ShowErrorDialog("ファイルディレクトリがありません");
                }

            }


        }

        private void saveSetting()
        {
            Properties.Settings.Default.Shared_URL = TxtSharedURL.Text;
            Properties.Settings.Default.Storage_Folder = TxtFolderPath.Text;
            Page_Source = TxtSharedURL.Text;

            if ((bool)ChkYearMonthFolder.IsChecked)
            {
                Properties.Settings.Default.SubFolder_Type = 1;
            }
            else
            {
                Properties.Settings.Default.SubFolder_Type = 0;
            }

            Properties.Settings.Default.Save();

        }

        // MiteneWebView2

        private void MiteneWebViewOnContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e)
        {
            Debug.Print($"MiteneWebView.ContentLoading: {nameof(e.IsErrorPage)} = {e.IsErrorPage}, {nameof(e.NavigationId)} = {e.NavigationId}");
        }

        private void MiteneWebViewOnCoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2InitializationCompleted: {nameof(e.IsSuccess)} = {e.IsSuccess}, {nameof(e.InitializationException)} = {e.InitializationException}");
        }

        private void MiteneWebViewOnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.Print($"MiteneWebView.NavigationCompleted: {nameof(e.NavigationId)} = {e.NavigationId}, {nameof(e.IsSuccess)} = {e.IsSuccess}, {nameof(e.HttpStatusCode)} = {e.HttpStatusCode}, {nameof(e.WebErrorStatus)} = {e.WebErrorStatus}");
        }

        private void MiteneWebViewOnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            Debug.Print($"MiteneWebView.NavigationStarting: {nameof(e.NavigationId)} = {e.NavigationId}, {e.Uri}");
        }

        private void MiteneWebViewOnSourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            Debug.Print($"MiteneWebView.SourceChanged: {nameof(e.IsNewDocument)} = {e.IsNewDocument}");
        }

        private void MiteneWebViewOnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            Debug.Print($"MiteneWebView.WebMessageReceived: {nameof(e.Source)} = {e.WebMessageAsJson}");
        }

        private void MiteneWebViewOnZoomFactorChanged(object sender, EventArgs e)
        {
            Debug.Print("MiteneWebView.ZoomFactorChanged:");
        }

        // MiteneWebView.CoreWebView2

        private void CoreWebView2OnBasicAuthenticationRequested(object sender, CoreWebView2BasicAuthenticationRequestedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.BasicAuthenticationRequested: {nameof(e.Uri)} = {e.Uri}");
        }

        private void CoreWebView2OnClientCertificateRequested(object sender, CoreWebView2ClientCertificateRequestedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.ClientCertificateRequested: {nameof(e.Host)} = {e.Host}");
        }

        private void CoreWebView2OnContainsFullScreenElementChanged(object sender, object e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.ContainsFullScreenElementChanged: {e}");
        }

        private void CoreWebView2OnContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.ContentLoading {nameof(e.IsErrorPage)} = {e.IsErrorPage}, {nameof(e.NavigationId)} = {e.NavigationId}");
        }

        private void CoreWebView2OnContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.ContextMenuRequested: {nameof(e.ContextMenuTarget)} = {e.ContextMenuTarget.SelectionText}");
        }

        private void CoreWebView2OnDocumentTitleChanged(object sender, object e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.DocumentTitleChanged: {(sender as CoreWebView2)?.DocumentTitle}");
        }

        private void CoreWebView2OnDOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.DOMContentLoaded: {nameof(e.NavigationId)} = {e.NavigationId}");
        }

        private void CoreWebView2OnDownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.DownloadStarting: {nameof(e.ResultFilePath)} = {e.ResultFilePath}");
            string download_path = e.ResultFilePath;
            string newpath = getSavePath(download_path);
            if (!System.IO.File.Exists(newpath))
             
            {
                var DownloadOperation = e.DownloadOperation;
                e.ResultFilePath = newpath;

                downloadOperation = e.DownloadOperation; // Store the 'DownloadOperation' for later use in events
                                                         //downloadOperation.BytesReceivedChanged += DownloadOperation_BytesReceivedChanged; // Subscribe to BytesReceivedChanged event
                                                         //downloadOperation.EstimatedEndTimeChanged += DownloadOperation_EstimatedEndTimeChanged; // Subsribe to EstimatedEndTimeChanged event
                downloadOperation.StateChanged += DownloadOperation_StateChanged;
            } 
            else
            {
                e.Cancel = true;
                //シグナル初期化
                condition.Signal();
                System.Threading.Thread.Sleep(1);
                condition.Reset();
            }
        }

        private void DownloadOperation_EstimatedEndTimeChanged(object sender, object e)
        {
            var ans = downloadOperation.EstimatedEndTime.ToString(); // Show the progress
            var fi = downloadOperation.ResultFilePath.ToString();
            Debug.Print("MiteneWebView.CoreWebView2.DownloadOperation_EstimatedEndTimeChanged " + ans + "(" + fi +")");
        }

        private void DownloadOperation_BytesReceivedChanged(object sender, object e)
        {
            var recive  = downloadOperation.BytesReceived.ToString(); // Show the progress
            var size = downloadOperation.TotalBytesToReceive.ToString();
            var fi = downloadOperation.ResultFilePath.ToString();
            Debug.Print("MiteneWebView.CoreWebView2.DownloadOperation_BytesReceivedChanged " + recive + "/" + size + "(" + fi + ")");
        }

        private void DownloadOperation_StateChanged(object sender, object e)
        {
            var state = downloadOperation.State.ToString(); // Show the progress
            var fi = downloadOperation.ResultFilePath.ToString();
            Debug.Print("MiteneWebView.CoreWebView2.DownloadOperation_StateChanged " + state + "(" + fi + ")");

            if (state == "Completed")
            {
                //シグナル初期化
                condition.Signal();
                System.Threading.Thread.Sleep(1);
                condition.Reset();
            }

        }

        private void CoreWebView2OnFrameCreated(object sender, CoreWebView2FrameCreatedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.FrameCreated: {nameof(e.Frame)} = {e.Frame.Name}");
        }

        private void CoreWebView2OnFrameNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.FrameNavigationCompleted: {nameof(e.NavigationId)} = {e.NavigationId}, {nameof(e.IsSuccess)} = {e.IsSuccess}, {nameof(e.HttpStatusCode)} = {e.HttpStatusCode}, {nameof(e.WebErrorStatus)} = {e.WebErrorStatus}");
        }

        private void CoreWebView2OnFrameNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.FrameNavigationStarting: {nameof(e.NavigationId)} = {e.NavigationId}, {nameof(e.Uri)} = {e.Uri}");
        }

        private void CoreWebView2OnHistoryChanged(object sender, object e)
        {
            Debug.Print("MiteneWebView.CoreWebView2.HistoryChanged:");
        }

        private void CoreWebView2OnIsDefaultDownloadDialogOpenChanged(object sender, object e)
        {
            Debug.Print("MiteneWebView.CoreWebView2.IsDefaultDownloadDialogOpenChanged:");
        }

        private void CoreWebView2OnIsDocumentPlayingAudioChanged(object sender, object e)
        {
            Debug.Print("MiteneWebView.CoreWebView2.IsDocumentPlayingAudioChanged");
        }

        private void CoreWebView2OnIsMutedChanged(object sender, object e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.IsMutedChanged:");
        }

        private async void CoreWebView2OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.NavigationCompleted: {nameof(e.NavigationId)} = {e.NavigationId}, {nameof(e.IsSuccess)} = {e.IsSuccess}");

            checkMitenePage();
            if (isMiteneDataPage && !isDownload && !endofData && readStart)
            {
                ReadData();
                if (!endofData)
                {
                    nextPage();
                }
            }
        }

        private void CoreWebView2OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.NavigationStarting: {nameof(e.NavigationId)} = {e.NavigationId}, {e.Uri}");
        }

        private void CoreWebView2OnNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.NewWindowRequested: {nameof(e.Name)} = {e.Name}, {nameof(e.Uri)} = {e.Uri}");
        }

        private void CoreWebView2OnPermissionRequested(object sender, CoreWebView2PermissionRequestedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.PermissionRequested: {nameof(e.PermissionKind)} = {e.PermissionKind}, {nameof(e.State)} = {e.State}, {nameof(e.Uri)} = {e.Uri}");
        }

        private void CoreWebView2OnProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.ProcessFailed: {nameof(e.ExitCode)} = {e.ExitCode}, {nameof(e.Reason)} = {e.Reason}, {nameof(e.ProcessDescription)} = {e.ProcessDescription}");
        }

        private void CoreWebView2OnScriptDialogOpening(object sender, CoreWebView2ScriptDialogOpeningEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.ScriptDialogOpening: {nameof(e.Kind)} = {e.Kind}, {nameof(e.ResultText)} = {e.ResultText}, {nameof(e.DefaultText)} = {e.DefaultText}, {nameof(e.Message)} = {e.Message}, {nameof(e.Uri)} = {e.Uri}");
        }

        private void CoreWebView2OnServerCertificateErrorDetected(object sender, CoreWebView2ServerCertificateErrorDetectedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.ServerCertificateErrorDetected: {nameof(e.Action)} = {e.Action}, {nameof(e.ErrorStatus)} = {e.ErrorStatus}, {nameof(e.ServerCertificate.DisplayName)} = {e.ServerCertificate.DisplayName}, {nameof(e.RequestUri)} = {e.RequestUri}");
        }

        private void CoreWebView2OnSourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.SourceChanged: {nameof(e.IsNewDocument)} = {e.IsNewDocument}");
        }

        private void CoreWebView2OnStatusBarTextChanged(object sender, object e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.StatusBarTextChanged: {(sender as CoreWebView2)?.StatusBarText}");
        }

        private void CoreWebView2OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.WebMessageReceived: {nameof(e.Source)} = {e.Source}, {nameof(e.WebMessageAsJson)} = {e.WebMessageAsJson}");
        }

        private void CoreWebView2OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.WebResourceRequested: {nameof(e.ResourceContext)} = {e.ResourceContext}, {nameof(e.Response.StatusCode)} = {e.Response.StatusCode}, {nameof(e.Request.Uri)} = {e.Request.Uri}");
        }

        private async void CoreWebView2OnWebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.WebResourceResponseReceived: {nameof(e.Response.StatusCode)} = {e.Response.StatusCode}, {nameof(e.Request.Uri)} = {e.Request.Uri}");

            var requestContent = "";
            var responstContent = "";
            Exception requestContentException = null;
            Exception responseContentException = null;

            try
            {
                if (e.Request.Content != null)
                {
                    var req = new StreamReader(e.Request.Content);
                    requestContent = await req.ReadToEndAsync();
                }
            }
            catch (Exception exception)
            {
                requestContentException = exception;
            }

            try
            {

                var stream = await e.Response.GetContentAsync();

                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    responstContent = await reader.ReadToEndAsync();
                }
            }
            catch (Exception exception)
            {
                responseContentException = exception;
            }

            // Debugger.Break();
        }

        private void CoreWebView2OnWindowCloseRequested(object sender, object e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.WindowCloseRequested:");
        }


        private async void ReadData()
        {
            var html = await MiteneWebView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML");

            string src = WebUtility.UrlDecode(html);
            string defaultStr = src;

            src = src.Replace("\\u003C", "<");
            src = src.Replace("\\\"", "\"");
            if (string.IsNullOrEmpty(src)) return;
            int top = src.IndexOf("CDATA");
            if (top == -1)
            {
                endofData = true;
                doDownload();
                return;
            }
            src = src.Substring(top, src.Length - top);
            top = src.IndexOf("{\"id");
            if (top == -1)
            {
                endofData = true;
                doDownload();
                return;
            }

            src = src.Substring(top, src.Length - top);
            var pattern = @"\},\{(""id"":\d{4,15},""uuid"")";
            src = Regex.Replace(src, pattern, "\n$1");
            top = src.IndexOf("}]};gon");
            src = src.Substring(0, top);
            src = src.Replace("{\"id", "\"id");
            src = src.Replace("},", "");

            string[] lines = { };


            System.IO.StringReader rs = new System.IO.StringReader(src);

            int line_count = 0;
            while (rs.Peek() > -1)
            {
                //一行読み込んで表示する
                string line = rs.ReadLine();
                if (line.StartsWith("\"id"))
                {
                    Array.Resize(ref lines, lines.Length + 1);
                    MiteneStruct data = new MiteneStruct(line);
                    DataRow dr = miteneData.NewRow();
                    dr["id"] = data.id;
                    dr["uuid"] = data.uuid;
                    dr["userId"] = data.userId;
                    dr["mediaType"] = data.mediaType;
                    dr["originalHash"] = data.originalHash;
                    dr["hasComment"] = data.hasComment;
                    dr["comments"] = data.comments;
                    dr["footprints"] = data.footprints;
                    dr["tookAt"] = getUniqtookAt(data.tookAt);
                    dr["audienceType"] = data.audienceType;
                    dr["mediaWidth"] = data.mediaWidth;
                    dr["mediaHeight"] = data.mediaHeight;
                    dr["mediaOrientation"] = data.mediaOrientation;
                    dr["latitude"] = data.latitude;
                    dr["longitude"] = data.longitude;
                    dr["mediaDeviceModel"] = data.mediaDeviceModel;
                    dr["deviceFilePath"] = data.deviceFilePath;
                    dr["videoDuration"] = data.videoDuration;
                    dr["contentType"] = data.contentType;
                    dr["origin"] = data.origin;
                    dr["thumbnailGenerated"] = data.thumbnailGenerated;
                    dr["expiringUrl"] = data.expiringUrl;
                    dr["expiringVideoUrl"] = data.expiringVideoUrl;
                    dr["downloadUrl"] = this.Page_Source + "/media_files/" + data.uuid + "/download";
                    dr["fileExist"] = FileExistCheckPath(data.uuid, dr["tookAt"].ToString());
                    miteneData.Rows.Add(dr);
                    line_count++;
                }
            }

            string count = "count:" + line_count + "/Total:" + miteneData.Rows.Count;
            Debug.Print("MiteneWebView.DataLoad: " + count);

        }

        private string getUniqtookAt(string tookAt)
        {
            string newName = tookAt;
            string selectStr = "tookAt= '" + newName + "'";

            DataRow[] drs = miteneData.Select(selectStr);
            if (drs.Length > 0)
            {
                for (uint i = 0; i < uint.MaxValue - 1; i++)
                {
                    newName = $"{tookAt} ({i + 1})";
                    selectStr = "tookAt= '" + newName + "'";
                    DataRow[] drss = miteneData.Select(selectStr);
                    if (drss.Length < 1)
                    {
                        return newName;
                    }
                }
            }
            return newName;

        }

        private void nextPage()
        {
            page_count++;
            string nextPage = Page_Source + "?page=" + page_count;
            MiteneWebView.CoreWebView2.Navigate(nextPage);
        }

        private void FirstPage()
        {
            page_count = 1;
            string url = Page_Source;
            ;
            MiteneWebView.Source = new Uri(url);
        }


        private string getSavePath(string download_path)
        {
            string uuid = System.IO.Path.GetFileNameWithoutExtension(download_path);
            string ex = System.IO.Path.GetExtension(download_path);

            if (string.IsNullOrEmpty(ex)) ex = ".*";

            DataRow row = getMiteneDataByUuid(uuid);
            string file_name = "";
            if (row == null)
            {
                file_name = uuid + ex;
            }
            else
            {
                file_name = getFileName(row, ex);
            }

            string folder_path = getFoldrPath();

            return folder_path + @"\" + file_name;
        }

        private bool FileExistCheckPath(string uuid, string tookAt)
        {
            string date = tookAt.Replace(":", "-").Substring(0, 19);
            string folder_path = getFoldrPath();
            string extension = ".*";
            string check_file_name = date + "(" + uuid + ")" + extension;

            var fileList1 = Directory.GetFiles(folder_path, check_file_name);

            if (fileList1.Length > 0)
            {
                return true;
            }
            return false;
        }

        private string getFileName(DataRow row,string extension)
        {
            if (row == null) return "";
            string tookAT = row["tookAt"].ToString();
            string uuid = row["uuid"].ToString();
            string date = tookAT.Replace(":", "-").Substring(0,19);
            if (string.IsNullOrEmpty(extension)) extension = ".*";

            return date+"("+ uuid +")" + extension;
        }



        private DataRow getMiteneDataByUuid(string uuid)
        {
            string selectStr = "uuid= '" + uuid + "'";
            DataRow[] drs = miteneData.Select(selectStr);
            if (drs.Length > 0)
            {
                return drs[0];
            }
            return null;
        }


        private string getFoldrPath()
        {
            //return @"C:\WorkSpace\Mitene";
            return TxtFolderPath.Text;
        }

        private async void doDownload()
        {
            progressBar.Visibility = Visibility.Visible;
            progressText.Visibility = Visibility.Visible;
            progressBar.Width = this.Width - 30;
            string folder_path = getFoldrPath();
            isDownload = true;
            string result = "";
            progressBar.Maximum = miteneData.Rows.Count;

            DataRow[] foundRows;

            string selectStr = "fileExist = 'False'";
            foundRows = miteneData.Select(selectStr);

            int download_count = foundRows.Length;

            progressBar.Value = miteneData.Rows.Count - download_count;


            progressText.Text = progressBar.Value + "/" + progressBar.Maximum;

            //for (int index = 0; index < miteneData.Rows.Count; index++)
            foreach (DataRow row in foundRows)
            {

                string dPage = row["downloadUrl"].ToString();

                MiteneWebView.CoreWebView2.Navigate(dPage);
                await Task.Run(() =>
                {
                    //読み込み完了まで待機
                    if (condition.Wait(600000))
                        result = "ok";
                    else
                        result = "timeout";
                });
                progressBar.Value++;
                progressText.Text = progressBar.Value + "/" + progressBar.Maximum;

            }

            if (progressBar.Value >= progressBar.Maximum)
            {
                progressText.Text = progressBar.Value + "/" + progressBar.Maximum + " ファイル取込み完了";
                progressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                progressText.Text = progressBar.Value + "/" + progressBar.Maximum;
            }

            //MiteneWebView.CoreWebView2.
            if (MiteneWebView.CoreWebView2.IsDefaultDownloadDialogOpen)
            {
                MiteneWebView.CoreWebView2.CloseDefaultDownloadDialog();
            }

            MiteneWebView.CoreWebView2.Navigate(Page_Source);
            string message = progressBar.Value + "/" + progressBar.Maximum + " ファイル処理完了";
            MessageEx.ShowInformationDialog(message, Window.GetWindow(this));

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            progressBar.Width = this.Width - 30;
        }


        private async void checkMitenePage()
        {
            string mitene_url = "https://mitene.us";
            bool is_mitene_url = false;

            string url = MiteneWebView.Source.ToString();
            if(url.Length >= mitene_url.Length)
            {
                url = url.Substring(0, mitene_url.Length);
                if (url.Equals(mitene_url, StringComparison.OrdinalIgnoreCase))
                {
                    is_mitene_url = true;
                }
            }

            if (is_mitene_url)
            {
                var html = await MiteneWebView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML");

                string src = WebUtility.UrlDecode(html);
                src = src.Replace("\\u003C", "<");
                src = src.Replace("\\\"", "\"");

                var pattern = @"""id"":\d{4,15},""uuid""";

                var login_pattern = "input type=\"password\" name=\"session\\[password\\]\" id=\"session_password\"";
                isMiteneDataPage = Regex.IsMatch(src, pattern);
                isMiteneLoginPage = Regex.IsMatch(src, login_pattern);


                if (isMiteneDataPage)
                {
                    Menu_DoStart.Visibility = Visibility.Visible;
                }
                else
                {
                    Menu_DoStart.Visibility = Visibility.Collapsed;
                }

            }
            else
            {
                isMiteneDataPage = false;
                isMiteneLoginPage = false;
                Menu_DoStart.Visibility = Visibility.Collapsed;
            }


        }

        private void Btn_Min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Btn_Max_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Menu_MitenePageCheck_Click(object sender, RoutedEventArgs e)
        {
            checkMitenePage();

            if (isMiteneDataPage)
            {
                MessageEx.ShowInformationDialog("みてねの共有URLデータページです。", Window.GetWindow(this));
            }
            else if(isMiteneLoginPage)
            {

                MessageEx.ShowInformationDialog("みてねの共有URLログインページです。", Window.GetWindow(this));
            }
            else
            {
                MessageEx.ShowWarningDialog("みてねの共有URLページではりません。", Window.GetWindow(this));
            }
        }

        private void Menu_CookieDelete_Click(object sender, RoutedEventArgs e)
        {
            MiteneWebView.CoreWebView2.CookieManager.DeleteAllCookies();
        }

        private void Menu_DoStart_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("MiteneWebView.ReadStart:");
            FirstPage(); //最初のページへ戻してから処理する
            System.Threading.Thread.Sleep(1000);
            readStart = true;
            ReadData();
            if (!endofData)
            {
                nextPage();
            }
        }

        private void Menu_Configration_Click(object sender, RoutedEventArgs e)
        {
            loadSetting();
            MainPanel.Visibility = Visibility.Collapsed;
            SettingPanel.Visibility = Visibility.Visible;
        }

        private void BtnSettingSave_Click(object sender, RoutedEventArgs e)
        {
            saveSetting();

            string dirPath = TxtFolderPath.Text;
            if (!Directory.Exists(dirPath))
            {
                MessageEx.ShowWarningDialog("指定の保存フォルダーが存在しません。\n保存フォルダーを選択してください。", Window.GetWindow(this));
                return;
            }

            if (string.IsNullOrEmpty(Page_Source))
            {
                MessageEx.ShowWarningDialog("共有URLが設定されていません。\n共有URLを設定してください。", Window.GetWindow(this));
                return;
            }


            MiteneWebView.Source = new Uri(Page_Source);

            checkMitenePage();
            if(!isMiteneDataPage && !isMiteneLoginPage)
            {
                MessageEx.ShowWarningDialog("みてねの共有URLではありません。\n正しい共有URLを設定してください。", Window.GetWindow(this));
                return;
            }
            SettingPanel.Visibility = Visibility.Collapsed;
            MainPanel.Visibility = Visibility.Visible;


        }

        private void BtnFolder_Click(object sender, RoutedEventArgs e)
        {

            var cofd = new CommonOpenFileDialog();

            cofd.Title = "フォルダを選択してください";
            if (string.IsNullOrEmpty(TxtFolderPath.Text))
            {
                cofd.InitialDirectory = @"C:";
            }
            else
            {
                cofd.InitialDirectory = TxtFolderPath.Text;

            }
            cofd.IsFolderPicker = true;

                if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                TxtFolderPath.Text = cofd.FileName;
        }

        private void Menu_FileBrowse_Click(object sender, RoutedEventArgs e)
        {
            FileBrowsePanel.Visibility = Visibility.Visible;
        }
    }

    public class MiteneStruct
    {
        public string id;
        public string uuid;
        public string userId;
        public string mediaType;
        public string originalHash;
        public string hasComment;
        public string comments;
        public string footprints;
        public string tookAt;
        public string audienceType;
        public string mediaWidth;
        public string mediaHeight;
        public string mediaOrientation;
        public string latitude;
        public string longitude;
        public string mediaDeviceModel;
        public string deviceFilePath;
        public string videoDuration;
        public string contentType;
        public string origin;
        public string thumbnailGenerated;
        public string expiringUrl;
        public string expiringVideoUrl;


        public MiteneStruct(string src)
        {
            get_value(src);
        }

        private void get_value(string src)
        {
            src = src.Replace(",", "\n");
            System.IO.StringReader rs = new System.IO.StringReader(src);
            while (rs.Peek() > -1)
            {
                //一行読み込んで表示する
                string line = rs.ReadLine();
                int top = line.IndexOf(":");
                if (top == -1) continue;

                int top2 = top + 1;
                string name = line.Substring(0, top);
                string value = line.Substring(top2 , line.Length - top2);

                name = name.Replace("\"", "");
                value = value.Replace("\"", "");

                switch (name)
                {
                    case "id":
                        this.id = value;
                        break;
                    case "uuid":
                        this.uuid = value;
                        break;
                    case "userId":
                        this.userId = value;
                        break;
                    case "mediaType":
                        this.mediaType = value;
                        break;
                    case "originalHash":
                        this.originalHash = value;
                        break;
                    case "hasComment":
                        this.hasComment = value;
                        break;
                    case "comments":
                        this.comments = value;
                        break;
                    case "footprints":
                        this.footprints = value;
                        break;
                    case "tookAt":
                        this.tookAt = value;
                        break;
                    case "audienceType":
                        this.audienceType = value;
                        break;
                    case "mediaWidth":
                        this.mediaWidth = value;
                        break;
                    case "mediaHeight":
                        this.mediaHeight = value;
                        break;
                    case "mediaOrientation":
                        this.mediaOrientation = value;
                        break;
                    case "latitude":
                        this.latitude = value;
                        break;
                    case "longitude":
                        this.longitude = value;
                        break;
                    case "mediaDeviceModel":
                        this.mediaDeviceModel = value;
                        break;
                    case "deviceFilePath":
                        this.deviceFilePath = value;
                        break;
                    case "videoDuration":
                        this.videoDuration = value;
                        break;
                    case "contentType":
                        this.contentType = value;
                        break;
                    case "origin":
                        this.origin = value;
                        break;
                    case "thumbnailGenerated":
                        this.thumbnailGenerated = value;
                        break;
                    case "expiringUrl":
                        this.expiringUrl = value;
                        break;
                    case "expiringVideoUrl":
                        this.expiringVideoUrl = value;
                        break;
                    default:
                        break;
                }



            }

            // file extension
            string extention = "";
            int extention_top = this.contentType.IndexOf('/');
            if (extention_top != -1)
            {
                extention_top++;
                extention = this.contentType.Substring(extention_top, this.contentType.Length - extention_top);
            }


        }

    }




}

