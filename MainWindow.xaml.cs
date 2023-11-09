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
using System.Drawing;
using System.Net.NetworkInformation;
using MiteneLoader.Properties;
using System.Configuration;
using System.Windows.Forms;

namespace MiteneLoader
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // "https://mitene.us/f/bO6hd7QDdb4"
        DataTable miteneData;
        int page_count = 1;
        readonly CountdownEvent condition = new CountdownEvent(1);

        bool DataDownloadComplete = false;
        bool inDownloadProsess = false;
        bool DataReadComplete = false;
        bool inDataReadProsess = false;

        bool isMiteneDataPage = false;
        bool isMiteneLoginPage = false;
        bool isMitenePage = false;

        int Select_User = 0;
        string Select_Name;
        string Shared_URL;
        string Storage_Folder;
        string Password;
        bool useYearMonthFolder;
        bool Login_Cookie_Clear;
        bool Finished_Page_Cancel;

        bool isInternetConnected = false;
        bool initOK = false;

        CoreWebView2DownloadOperation downloadOperation;

        public MainWindow()
        {
            InitializeComponent();

            loadSetting();
            showBrank();
            InitMiteneTable();  //みてねのDataTable構成を定義します

            isInternetConnected = isNetworkAvailable();//インターネットへの接続を確認します
            if (isInternetConnected)
            {
                InitAsync();
            }
            else
            {
                showFileBrowser();
            }

            initOK = true;

        }


        /// <summary>
        /// みてねのDataTabeleの構成を定義します。
        /// </summary>
        private void InitMiteneTable()
        {
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
            miteneData.Columns.Add("fileExist", typeof(bool));
        }

        /// <summary>
        /// WebView2の初期化の完了を待って、その後の処理を行う
        /// </summary>
        private async void InitAsync()
        {
            await MiteneWebView.EnsureCoreWebView2Async();
            setMiteneWebViewEvent();
            PageLoading();
        }

        /// <summary>
        /// WebView2のイベントハンドラーを設定する
        /// ref https://note.dokeep.jp/post/wpf-webview2-events/
        /// </summary>
        private void setMiteneWebViewEvent()
        {
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

        }

        /// <summary>
        /// 設定されている内容で接続状況を確認しどのパネルを表示するか決定します。
        /// </summary>
        private void PageLoading()
        {
            bool isConfigRequired = false;
            string mess = "";

            if (isInternetConnected)
            {
                if (string.IsNullOrEmpty(Shared_URL))
                {
                    isConfigRequired = true;
                    mess = "共有URLが設定されていません。共有URLを設定してください。";
                }
                else
                {
                    try
                    {

                        Debug.Print("MiteneWebView.Source = new Uri(" + Shared_URL + ");");
                        //MiteneWebView.Source = new Uri(Shared_URL);
                        MiteneWebView.CoreWebView2.Navigate(Shared_URL);
                    }
                    catch
                    {
                        isConfigRequired = true;
                        mess = "指定URLが開けません。設定を確認してください。";
                    }

                }

            }

            if (!Directory.Exists(Storage_Folder))
            {
                isConfigRequired = true;
                if (mess.Length > 0) mess = mess + "\n";
                mess = mess + "指定の保存フォルダーが存在しません。保存フォルダーを設定してください。";

                Menu_FileBrowse.Visibility = Visibility.Collapsed;
            }
            else
            {
                try
                {
                    FileBrowser.Navigate(ShellFileSystemFolder.FromFolderPath(Storage_Folder));  // フォルダーがないとエラー
                    Menu_FileBrowse.Visibility = Visibility.Visible;
                }
                catch
                {
                    isConfigRequired = true;

                    if (mess.Length > 0) mess = mess + "\n";

                    mess = mess + "指定の保存フォルダーが存在しません。保存フォルダーを設定してください。";
                    Menu_FileBrowse.Visibility = Visibility.Collapsed;
                }

            }

            if (isConfigRequired)
            {
                MessageEx.ShowWarningDialog(mess, Window.GetWindow(this));
                showSetting();
                return;
            }

            if (isInternetConnected)
            {
                showWebBrowser();
            }
            else
            {
                mess = "インターネットに接続していないためWebブラウザは使用できません。";
                MessageEx.ShowWarningDialog(mess, Window.GetWindow(this));

                showFileBrowser();
            }
        }

        /// <summary>
        /// 表示しているWebページがみてねのものであるか確認し
        /// 以下のプロパティを設定します。
        ///     bool isMiteneDataPage       //みてねのログイン後ページ
        ///     bool isMiteneLoginPage      //みてねのログインページ
        ///     bool isMitenePage           //みてねのページ
        /// </summary>
        /// <returns></returns>
        private async Task<bool> checkMitenePage()
        {
            string mitene_url = "https://mitene.us";
            bool is_mitene_url = false;

            string url = MiteneWebView.Source.ToString();

            if (url.Length >= mitene_url.Length)
            {
                url = url.Substring(0, mitene_url.Length);
                is_mitene_url = url.Equals(mitene_url, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                is_mitene_url = false;
            }


            if (is_mitene_url)
            {
                var html = await MiteneWebView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML");

                string src = WebUtility.UrlDecode(html);
                src = src.Replace("\\u003C", "<");
                src = src.Replace("\\\"", "\"");

                var data_page_pattern = @"""id"":\d{4,15},""uuid""";
                var login_pattern = "input type=\"password\" name=\"session\\[password\\]\" id=\"session_password\"";
                var mitene_page_pattarn = "Mitene.Utils.FollowerAccessTokenManager.setToken";

                isMiteneDataPage = Regex.IsMatch(src, data_page_pattern);
                isMiteneLoginPage = Regex.IsMatch(src, login_pattern);
                isMitenePage = Regex.IsMatch(src, mitene_page_pattarn);

                if (!isMiteneDataPage && !isMiteneLoginPage && !isMitenePage && !inDownloadProsess && !inDataReadProsess && !DataDownloadComplete)
                {
                    return false;
                }

                return true;
            }
            else
            {
                isMiteneDataPage = false;
                isMiteneLoginPage = false;
                isMitenePage = false;

                return false;
            }

        }

        /// <summary>
        /// インターネットに正常に接続しているかを返す
        /// </summary>
        /// <returns></returns>
        private bool isNetworkAvailable()
        {
            Debug.Print("isNetworkAvailable()");

            //ネットワークに接続されているか調べる
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                //インターネットに接続されているか確認する
                string host = "http://www.yahoo.com";

                System.Net.HttpWebRequest webreq = null;
                System.Net.HttpWebResponse webres = null;
                try
                {
                    //HttpWebRequestの作成
                    webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(host);
                    //メソッドをHEADにする
                    webreq.Method = "HEAD";
                    //受信する
                    webres = (System.Net.HttpWebResponse)webreq.GetResponse();
                    //応答ステータスコードを表示
                    Console.WriteLine(webres.StatusCode);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    if (webres != null)
                        webres.Close();
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Webブラウザパネルを表示します。
        /// </summary>
        private void showWebBrowser()
        {
            MainPanel.Visibility = Visibility.Visible;
            SettingPanel.Visibility = Visibility.Collapsed;
            FileBrowsePanel.Visibility = Visibility.Collapsed;
            if (isMiteneDataPage || isMiteneLoginPage)
            {
                Menu_DoStart.Visibility = Visibility.Visible;
                Menu_WebBrowse.Visibility = Visibility.Collapsed;

            }
            else
            {
                Menu_DoStart.Visibility = Visibility.Collapsed;
                Menu_WebBrowse.Visibility = Visibility.Visible;

            }
            setProgressText();
        }

        /// <summary>
        /// Fileブラウザパネルを表示します。
        /// </summary>
        private void showFileBrowser()
        {
            MainPanel.Visibility = Visibility.Hidden;
            SettingPanel.Visibility = Visibility.Collapsed;
            FileBrowsePanel.Visibility = Visibility.Visible;
            if (isInternetConnected)
            {
                Menu_WebBrowse.Visibility = Visibility.Visible;
            }
            else
            {
                Menu_WebBrowse.Visibility = Visibility.Collapsed;
            }
            Menu_DoStart.Visibility = Visibility.Collapsed;

            //ここで最新の状態に更新して表示したい

            // FileBrowser.Refresh();  <-- NG

            //  ↓　NG

            //try
            //{
            //    FileBrowser.Navigate(ShellFileSystemFolder.FromFolderPath(Storage_Folder));  // フォルダーがないとエラー
            //}
            //catch
            //{
            //    string mess = "指定の保存フォルダーが存在しません。保存フォルダーを設定してください。";
            //    MessageEx.ShowWarningDialog(mess, Window.GetWindow(this));
            //    showSetting();
            //    return;
            //}

        }

        /// <summary>
        /// 設定パネルを表示します。
        /// </summary>
        private void showSetting()
        {
            MainPanel.Visibility = Visibility.Hidden;
            SettingPanel.Visibility = Visibility.Visible;
            FileBrowsePanel.Visibility = Visibility.Collapsed;

            if (isInternetConnected)
            {
                Menu_WebBrowse.Visibility = Visibility.Visible;
            }
            else
            {
                Menu_WebBrowse.Visibility = Visibility.Collapsed;
            }
            Menu_DoStart.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// すべてのパネルを非表示にします。
        /// </summary>
        private void showBrank()
        {
            MainPanel.Visibility = Visibility.Hidden;
            SettingPanel.Visibility = Visibility.Collapsed;
            FileBrowsePanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 表示している、みてね共有URLからファイル情報を取得する
        /// </summary>
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
                inDataReadProsess = false;
                DataReadComplete = true;
                doDownload();
                return;
            }
            src = src.Substring(top, src.Length - top);
            top = src.IndexOf("{\"id");
            if (top == -1)
            {
                inDataReadProsess = false;
                DataReadComplete = true;
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
            int exist_count = 0;
            while (rs.Peek() > -1)
            {
                //一行読み込んで表示する
                string line = rs.ReadLine();
                if (line.StartsWith("\"id"))
                {
                    Array.Resize(ref lines, lines.Length + 1);
                    MiteneStruct data = new MiteneStruct(line);
                    DataRow dr = miteneData.NewRow();
                    bool exist = false;
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
                    dr["downloadUrl"] = this.Shared_URL + "/media_files/" + data.uuid + "/download";
                    exist = FileExistCheckPath(data.uuid, dr["tookAt"].ToString());
                    dr["fileExist"] = exist;
                    miteneData.Rows.Add(dr);
                    line_count++;
                    if (exist == true) exist_count++;
                }
            }
            string count = "count:" + line_count + "/Total:" + miteneData.Rows.Count;
            Debug.Print("MiteneWebView.DataLoad: " + count);
            setProgressText();

            if (line_count > 0 && line_count == exist_count)
            {
                if (Finished_Page_Cancel)
                {
                    inDataReadProsess = false;
                    DataReadComplete = true;
                    doDownload();
                    return;
                }
            }

        }

        /// <summary>
        /// ダウンロード処理を行う
        /// </summary>
        private async void doDownload()
        {
            progressBar.Visibility = Visibility.Visible;
            progressBar.Width = this.Width - 30;
            inDownloadProsess = true;

            DataRow[] foundRows;

            string selectStr = "fileExist = 'False'";
            foundRows = miteneData.Select(selectStr);

            int download_count = foundRows.Length;

            progressBar.Maximum = download_count;
            progressBar.Value = 0;
            setProgressText();

            foreach (DataRow row in foundRows)
            {
                bool ReadOK = false;

                string dPage = row["downloadUrl"].ToString();

                MiteneWebView.CoreWebView2.Navigate(dPage);
                await Task.Run(() =>
                {

                    //読み込み完了まで待機
                    if (condition.Wait(600000))
                        ReadOK = true;
                    else
                        ReadOK = false;
                });

                if (!ReadOK) return;
                progressBar.Value++;
                setProgressText();

                //download history clear
                ClearDownloadHistoryData();
            }

            if (progressBar.Value >= progressBar.Maximum)
            {
                progressBar.Visibility = Visibility.Collapsed;
            }
            setProgressText();

            //MiteneWebView.CoreWebView2.
            if (MiteneWebView.CoreWebView2.IsDefaultDownloadDialogOpen)
            {
                MiteneWebView.CoreWebView2.CloseDefaultDownloadDialog();
            }

            MiteneWebView.CoreWebView2.Navigate(Shared_URL);
            string message = "";
            if (progressBar.Maximum > 0)
            {
                message = "処理終了しました。\n新たに " + progressBar.Maximum + "件のファイルをダウンロードしました。";
            }
            else
            {
                message = "処理終了しました。\n新たにダウンロードが必要なファイルはありませんでした。";
            }
            MessageEx.ShowInformationDialog(message, Window.GetWindow(this));

            inDataReadProsess = false;
            DataReadComplete = false;
            inDataReadProsess = false;
            DataDownloadComplete = true;
            inDownloadProsess = false;

            setAllMenuEnable(true);
            setProgressText();

        }

        /// <summary>
        /// ダウンロード履歴のクリア
        /// </summary>
        private async void ClearDownloadHistoryData()
        {
            // ref https://github.com/MicrosoftEdge/WebView2Feedback/issues/2582

            CoreWebView2Profile profile;
            if (this.MiteneWebView.CoreWebView2 != null)
            {
                profile = this.MiteneWebView.CoreWebView2.Profile;
                //MessageBox.Show($"{profile}");

                //CoreWebView2BrowsingDataKinds dataKinds = (CoreWebView2BrowsingDataKinds)
                //                         (CoreWebView2BrowsingDataKinds.GeneralAutofill |
                //                      CoreWebView2BrowsingDataKinds.PasswordAutosave |
                //                      CoreWebView2BrowsingDataKinds.AllDomStorage |
                //                      CoreWebView2BrowsingDataKinds.DownloadHistory |
                //                      CoreWebView2BrowsingDataKinds.BrowsingHistory);
                CoreWebView2BrowsingDataKinds dataKinds = (CoreWebView2BrowsingDataKinds)
                         (CoreWebView2BrowsingDataKinds.DownloadHistory);

                await profile.ClearBrowsingDataAsync(dataKinds);
            }
        }

        /// <summary>
        /// みてねの次ページを表示する
        /// </summary>
        private void nextPage()
        {
            page_count++;
            string nextPage = Shared_URL + "?page=" + page_count;
            MiteneWebView.CoreWebView2.Navigate(nextPage);
        }

        /// <summary>
        /// みてねの最初のページを表示する
        /// </summary>
        private void FirstPage()
        {
            page_count = 1;
            string url = Shared_URL;
            ;
            MiteneWebView.Source = new Uri(url);
        }

        /// <summary>
        /// menu有効・無効を設定する
        /// <param name="isEnable">有効・無効</param>
        /// </summary>
        private void setAllMenuEnable(bool isEnable)
        {
            Debug.Print("setAllMenuEnable()");

            Menu_Configration.IsEnabled = isEnable;
            Menu_DoStart.IsEnabled = isEnable;
            Menu_FileBrowse.IsEnabled = isEnable;
            Menu_WebBrowse.IsEnabled = isEnable;
            Menu_Setting.IsEnabled = isEnable;

        }

        #region User Setting Read Write *******************************************

        /// <summary>
        /// ユーザー毎のSetting情報を読み込みます。
        /// </summary>
        private void loadSetting()
        {
            initCmbUser();
            Select_User = Properties.Settings.Default.User_Num;

            UserSettingLoad(Select_User);
        }

        /// <summary>
        /// ユーザー選択コンボを初期化。
        /// </summary>

        private void initCmbUser()
        {
            CmbUser.Items.Clear();
            CmbUser.Items.Add(Properties.Settings.Default.Name_0);
            CmbUser.Items.Add(Properties.Settings.Default.Name_1);
            CmbUser.Items.Add(Properties.Settings.Default.Name_2);
            CmbUser.Items.Add(Properties.Settings.Default.Name_3);
            CmbUser.Items.Add(Properties.Settings.Default.Name_4);
            CmbUser.SelectedIndex = Select_User;
        }

        /// <summary>
        /// ユーザー毎のSetting情報を読み込みます。
        /// </summary>

        private void UserSettingLoad(int select_User)
        {
            switch (select_User)
            {
                case 0:
                    Select_Name = Properties.Settings.Default.Name_0;
                    Password = Properties.Settings.Default.Password_0;
                    Shared_URL = Properties.Settings.Default.Shared_URL_0;
                    Storage_Folder = Properties.Settings.Default.Storage_Folder_0;
                    useYearMonthFolder = (Properties.Settings.Default.SubFolder_Type_0 == 1);
                    Login_Cookie_Clear = Properties.Settings.Default.Login_Cookie_Clear_0;
                    Finished_Page_Cancel = Properties.Settings.Default.Finished_Page_Cancel_0;
                    break;
                case 1:
                    Select_Name = Properties.Settings.Default.Name_1;
                    Password = Properties.Settings.Default.Password_1;
                    Shared_URL = Properties.Settings.Default.Shared_URL_1;
                    Storage_Folder = Properties.Settings.Default.Storage_Folder_1;
                    useYearMonthFolder = (Properties.Settings.Default.SubFolder_Type_1 == 1);
                    Login_Cookie_Clear = Properties.Settings.Default.Login_Cookie_Clear_1;
                    Finished_Page_Cancel = Properties.Settings.Default.Finished_Page_Cancel_1;
                    break;
                case 2:
                    Select_Name = Properties.Settings.Default.Name_2;
                    Password = Properties.Settings.Default.Password_2;
                    Shared_URL = Properties.Settings.Default.Shared_URL_2;
                    Storage_Folder = Properties.Settings.Default.Storage_Folder_2;
                    useYearMonthFolder = (Properties.Settings.Default.SubFolder_Type_2 == 1);
                    Login_Cookie_Clear = Properties.Settings.Default.Login_Cookie_Clear_2;
                    Finished_Page_Cancel = Properties.Settings.Default.Finished_Page_Cancel_2;
                    break;
                case 3:
                    Select_Name = Properties.Settings.Default.Name_3;
                    Password = Properties.Settings.Default.Password_3;
                    Shared_URL = Properties.Settings.Default.Shared_URL_3;
                    Storage_Folder = Properties.Settings.Default.Storage_Folder_3;
                    useYearMonthFolder = (Properties.Settings.Default.SubFolder_Type_3 == 1);
                    Login_Cookie_Clear = Properties.Settings.Default.Login_Cookie_Clear_3;
                    Finished_Page_Cancel = Properties.Settings.Default.Finished_Page_Cancel_3;
                    break;
                case 4:
                    Select_Name = Properties.Settings.Default.Name_4;
                    Password = Properties.Settings.Default.Password_4;
                    Shared_URL = Properties.Settings.Default.Shared_URL_4;
                    Storage_Folder = Properties.Settings.Default.Storage_Folder_4;
                    useYearMonthFolder = (Properties.Settings.Default.SubFolder_Type_4 == 1);
                    Login_Cookie_Clear = Properties.Settings.Default.Login_Cookie_Clear_4;
                    Finished_Page_Cancel = Properties.Settings.Default.Finished_Page_Cancel_4;
                    break;
                default:
                    Select_Name = Properties.Settings.Default.Name_0;
                    Password = Properties.Settings.Default.Password_0;
                    Shared_URL = Properties.Settings.Default.Shared_URL_0;
                    Storage_Folder = Properties.Settings.Default.Storage_Folder_0;
                    useYearMonthFolder = (Properties.Settings.Default.SubFolder_Type_0 == 1);
                    Login_Cookie_Clear = Properties.Settings.Default.Login_Cookie_Clear_0;
                    Finished_Page_Cancel = Properties.Settings.Default.Finished_Page_Cancel_0;
                    break;
            }
            CmbUser.SelectedValue =
            TxtName.Text = Select_Name;
            TxtPass.Text = Password;
            TxtSharedURL.Text = Shared_URL;
            TxtFolderPath.Text = Storage_Folder;
            ChkYearMonthFolder.IsChecked = useYearMonthFolder;
            ChkClearCookie.IsChecked = Login_Cookie_Clear;
            ChkFinishedPage.IsChecked = Finished_Page_Cancel;

            Title.Content = "みてねローダー " + Select_Name;
        }

        /// <summary>
        /// ユーザー毎のSetting情報を書き込みます。
        /// </summary>
        private void saveSetting()
        {
            int select_user = (int)CmbUser.SelectedIndex;
            int sub_folder_type;

            Select_User = select_user;
            Select_Name = TxtName.Text;
            Password = TxtPass.Text;
            Shared_URL = TxtSharedURL.Text;
            Storage_Folder = TxtFolderPath.Text;
            useYearMonthFolder = (bool)ChkYearMonthFolder.IsChecked;
            if (useYearMonthFolder)
            {
                sub_folder_type = 1;
            }
            else
            {
                sub_folder_type = 0;
            }
            Login_Cookie_Clear = (bool)ChkClearCookie.IsChecked;
            Finished_Page_Cancel = (bool)ChkFinishedPage.IsChecked;

            switch (select_user)
            {
                case 0:
                    Properties.Settings.Default.Name_0 = Select_Name;
                    Properties.Settings.Default.Password_0 = Password;
                    Properties.Settings.Default.Shared_URL_0 = Shared_URL;
                    Properties.Settings.Default.Storage_Folder_0 = Storage_Folder;
                    Properties.Settings.Default.SubFolder_Type_0 = sub_folder_type;
                    Properties.Settings.Default.Login_Cookie_Clear_0 = Login_Cookie_Clear;
                    Properties.Settings.Default.Finished_Page_Cancel_0 = Finished_Page_Cancel;
                    break;
                case 1:
                    Properties.Settings.Default.Name_1 = Select_Name;
                    Properties.Settings.Default.Password_1 = Password;
                    Properties.Settings.Default.Shared_URL_1 = Shared_URL;
                    Properties.Settings.Default.Storage_Folder_1 = Storage_Folder;
                    Properties.Settings.Default.SubFolder_Type_1 = sub_folder_type;
                    Properties.Settings.Default.Login_Cookie_Clear_1 = Login_Cookie_Clear;
                    Properties.Settings.Default.Finished_Page_Cancel_1 = Finished_Page_Cancel;
                    break;
                case 2:
                    Properties.Settings.Default.Name_2 = Select_Name;
                    Properties.Settings.Default.Password_2 = Password;
                    Properties.Settings.Default.Shared_URL_2 = Shared_URL;
                    Properties.Settings.Default.Storage_Folder_2 = Storage_Folder;
                    Properties.Settings.Default.SubFolder_Type_2 = sub_folder_type;
                    Properties.Settings.Default.Login_Cookie_Clear_2 = Login_Cookie_Clear;
                    Properties.Settings.Default.Finished_Page_Cancel_2 = Finished_Page_Cancel;
                    break;
                case 3:
                    Properties.Settings.Default.Name_3 = Select_Name;
                    Properties.Settings.Default.Password_3 = Password;
                    Properties.Settings.Default.Shared_URL_3 = Shared_URL;
                    Properties.Settings.Default.Storage_Folder_3 = Storage_Folder;
                    Properties.Settings.Default.SubFolder_Type_3 = sub_folder_type;
                    Properties.Settings.Default.Login_Cookie_Clear_3 = Login_Cookie_Clear;
                    Properties.Settings.Default.Finished_Page_Cancel_3 = Finished_Page_Cancel;
                    break;
                case 4:
                    Properties.Settings.Default.Name_4 = Select_Name;
                    Properties.Settings.Default.Password_4 = Password;
                    Properties.Settings.Default.Shared_URL_4 = Shared_URL;
                    Properties.Settings.Default.Storage_Folder_4 = Storage_Folder;
                    Properties.Settings.Default.SubFolder_Type_4 = sub_folder_type;
                    Properties.Settings.Default.Login_Cookie_Clear_4 = Login_Cookie_Clear;
                    Properties.Settings.Default.Finished_Page_Cancel_4 = Finished_Page_Cancel;
                    break;
                default:
                    Properties.Settings.Default.Name_0 = Select_Name;
                    Properties.Settings.Default.Password_0 = Password;
                    Properties.Settings.Default.Shared_URL_0 = Shared_URL;
                    Properties.Settings.Default.Storage_Folder_0 = Storage_Folder;
                    Properties.Settings.Default.SubFolder_Type_0 = sub_folder_type;
                    Properties.Settings.Default.Login_Cookie_Clear_0 = Login_Cookie_Clear;
                    Properties.Settings.Default.Finished_Page_Cancel_0 = Finished_Page_Cancel;
                    break;
            }
            Properties.Settings.Default.User_Num = select_user;
            Properties.Settings.Default.Save();

        }

        #endregion
        //
        #region Parsing Page Data Function *******************************************

        /// <summary>
        /// ダウンローダが返す保存ファイル名(uuid)でみてねデータを検索し
        /// TookAt(uuid)形式の保存フォルダーを含むフルパスとして返す。
        /// </summary>
        /// <param name="download_path">ダウンローダが返す保存ファイル名</param>
        /// <returns></returns>
        private string getRenameSavePath(string download_path)
        {
            string uuid = System.IO.Path.GetFileNameWithoutExtension(download_path);
            string ex = System.IO.Path.GetExtension(download_path);

            if (string.IsNullOrEmpty(ex)) ex = ".*";

            DataRow row = getMiteneDataByUuid(uuid);
            string tookAT = "";



            string file_name = "";
            if (row == null)
            {
                file_name = uuid + ex;
            }
            else
            {
                tookAT = row["tookAt"].ToString();
                string uuid2 = row["uuid"].ToString();
                string date = tookAT.Replace(":", "-").Substring(0, 19);

                file_name = date + "(" + uuid + ")" + ex;
            }
            string folder_path = getFoldrPath(tookAT);
            return folder_path + @"\" + file_name;
        }

        /// <summary>
        /// みてねデータが保存済みかどうかを調べる
        /// TookAt(uuid)形式の保存フォルダーを含むフルパスとして返す。
        /// </summary>
        /// <param name="uuid,">みてねデータのuuid</param>
        /// <param name="tookAt,">みてねデータのtookAt</param>
        /// <returns></returns>
        private string getFileName(DataRow row, string extension)
        {
            if (row == null) return "";
            string tookAT = row["tookAt"].ToString();
            string uuid = row["uuid"].ToString();
            string date = tookAT.Replace(":", "-").Substring(0, 19);
            if (string.IsNullOrEmpty(extension)) extension = ".*";

            return date + "(" + uuid + ")" + extension;
        }

        /// <summary>
        /// みてねデータが保存済みかどうかを調べる
        /// TookAt(uuid)形式の保存フォルダーを含むフルパスとして返す。
        /// </summary>
        /// <param name="uuid,">みてねデータのuuid</param>
        /// <param name="tookAt,">みてねデータのtookAt</param>
        /// <returns></returns>
        private bool FileExistCheckPath(string uuid, string tookAt)
        {
            //年月フォルダーと保存ルートフォルダーの両方をしらべ
            //ファイルが存在する場合は、
            //useYearMonthFolderの設定に従い移動処理を行いtrueを返す。
            //ファイルがない場合は、falseを返す

            string normal_path = Storage_Folder;
            string year_month_path = getYearMonthFoldrPath(tookAt);

            string date = tookAt.Replace(":", "-").Substring(0, 19);
            string extension = ".*";
            string check_file_name = date + "(" + uuid + ")" + extension;

            string[] normal_files = null;
            string[] year_month_files = null;

            normal_files = Directory.GetFiles(normal_path, check_file_name);
            bool normal_found = (normal_files.Length > 0);

            bool year_month_found = false;
            if (System.IO.Directory.Exists(year_month_path))
            {
                year_month_files = Directory.GetFiles(year_month_path, check_file_name);
                year_month_found = (year_month_files.Length > 0);
            }


            //ファイル移動処理
            if (useYearMonthFolder)
            {
                //年月サブフォルダー使用時サブフォルダーがなければ作成
                if (!System.IO.Directory.Exists(year_month_path))
                {
                    Directory.CreateDirectory(year_month_path);
                }


                if (normal_found)
                {
                    foreach (string from_file in normal_files)
                    {
                        string file_name = System.IO.Path.GetFileName(from_file);
                        string to_file = year_month_path + @"\" + file_name;
                        if (!System.IO.File.Exists(to_file))
                        {
                            System.IO.File.Move(from_file, to_file);
                        }
                        else
                        {
                            System.IO.File.Delete(from_file);
                        }
                    }
                }
            }
            else
            {
                if (year_month_found)
                {
                    foreach (string from_file in year_month_files)
                    {
                        string file_name = System.IO.Path.GetFileName(from_file);
                        string to_file = normal_path + @"\" + file_name;
                        if (!System.IO.File.Exists(to_file))
                        {
                            System.IO.File.Move(from_file, to_file);
                        }
                        else
                        {
                            System.IO.File.Delete(from_file);
                        }
                    }
                }

                //サブフォルダー内残ファイル移動及びサブフォルダー削除

                string[] sub_dirs = Directory.GetDirectories(normal_path);
                foreach (string sub_dir in sub_dirs)
                {
                    IEnumerable<string> files = System.IO.Directory.EnumerateFiles(sub_dir, "*", System.IO.SearchOption.AllDirectories);

                    foreach (string from_file in files)
                    {
                        string file_name = System.IO.Path.GetFileName(from_file);
                        string to_file = normal_path + @"\" + file_name;
                        System.IO.File.Move(from_file, to_file);
                    }
                    Directory.Delete(sub_dir, true);
                }
            }



            if (normal_found || year_month_found)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// uuidを指定して、みてねDatatableから該当するDataRowを取得する
        /// </summary>
        /// <param name="uuid,">みてねデータのuuid</param>
        /// <returns></returns>
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


        private string getFoldrPath(string TookAt)
        {
            if (useYearMonthFolder)
            {
                return getYearMonthFoldrPath(TookAt);
            }
            return Storage_Folder;
        }

        private string getYearMonthFoldrPath(string TookAt)
        {
            if (string.IsNullOrEmpty(TookAt)) return Storage_Folder;

            string matchStr = @"^\d{4}-\d{2}-\d{2}";

            bool result = Regex.IsMatch(TookAt, matchStr);

            if (!result) return Storage_Folder;

            string date = TookAt.Replace(":", "-").Substring(0, 19);
            string Year = date.Substring(0, 4);
            string Month = date.Substring(5, 2);
            return Storage_Folder + @"\" + Year + @"\" + Month;
        }

        /// <summary>
        /// 重複しないTookAtデータを(を取得する　例2023-08-22T19:08:25 09:00(1)
        /// <param name="tookAt">みてねtookAt</param>
        /// </summary>
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

        #endregion
        //
        #region Window_Event *******************************************

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

        private void BtnSettingSave_Click(object sender, RoutedEventArgs e)
        {
            saveSetting();

            string dirPath = TxtFolderPath.Text;
            if (!Directory.Exists(Storage_Folder))
            {
                MessageEx.ShowWarningDialog("指定の保存フォルダーが存在しません。\n保存フォルダーを選択してください。", Window.GetWindow(this));
                return;
            }

            if (string.IsNullOrEmpty(Shared_URL))
            {
                MessageEx.ShowWarningDialog("共有URLが設定されていません。\n共有URLを設定してください。", Window.GetWindow(this));
                return;
            }


            //            MiteneWebView.Source = new Uri(Shared_URL);
            //MiteneWebView.na
            initCmbUser();

            PageLoading();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            progressBar.Width = this.Width - 30;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Grid_Title.Margin = new Thickness(8, 8, 8, 0);
                Grid_Main.Margin = new Thickness(12, 0, 12, 12);
                Btn_Max.Content = FindResource("ImageWinStateNormal");
            }
            else
            {
                Grid_Title.Margin = new Thickness(0, 0, 0, 0);
                Grid_Main.Margin = new Thickness(4, 0, 4, 4);
                Btn_Max.Content = FindResource("ImageWinStateMax");
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Login_Cookie_Clear)
            {
                MiteneWebView.CoreWebView2.CookieManager.DeleteAllCookies();
            }
        }

        #endregion
        //
        #region Menu_Click_Event **************************************
        private async void Menu_MitenePageCheck_Click(object sender, RoutedEventArgs e)
        {
            bool isCorrectPage = await checkMitenePage();

            if (isMiteneDataPage)
            {
                MessageEx.ShowInformationDialog("みてねの共有URLデータページです。", Window.GetWindow(this));
            }
            else if (isMiteneLoginPage)
            {

                MessageEx.ShowInformationDialog("みてねの共有URLログインページです。", Window.GetWindow(this));
            }
            else
            {
                MessageEx.ShowWarningDialog("みてねの共有URLページではりません。", Window.GetWindow(this));
            }
        }

        private void Menu_DoStart_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("MiteneWebView.inDataReadProsess:");
            FirstPage(); //最初のページへ戻してから処理する
            System.Threading.Thread.Sleep(1000);
            inDataReadProsess = true;
            setProgressText();
            setAllMenuEnable(false);
            miteneData.Clear();
            ReadData();
            if (!DataReadComplete)
            {
                nextPage();
            }
        }

        private void setProgressText()
        {
            if (inDataReadProsess)
            {
                progressText.Text = "みてねページ" + page_count + " データ解析中 処理データ数=" + miteneData.Rows.Count.ToString();
            }
            else if (inDownloadProsess && !DataDownloadComplete)
            {
                if (progressBar.Value >= progressBar.Maximum)
                {
                    progressText.Text = "みてねデータ " + progressBar.Value + "/" + progressBar.Maximum + " ダウンロード完了";
                }
                else
                {
                    progressText.Text = "みてねデータ " + progressBar.Value + "/" + progressBar.Maximum + " ダウンロード中";
                }
            }
            else
            {
                //AssemblyTitleの取得
                System.Reflection.AssemblyTitleAttribute asmttl =
                    (System.Reflection.AssemblyTitleAttribute)
                    Attribute.GetCustomAttribute(
                        System.Reflection.Assembly.GetExecutingAssembly(),
                        typeof(System.Reflection.AssemblyTitleAttribute));

                //AssemblyProductの取得
                System.Reflection.AssemblyProductAttribute asmprd =
                    (System.Reflection.AssemblyProductAttribute)
                    Attribute.GetCustomAttribute(
                    System.Reflection.Assembly.GetExecutingAssembly(),
                    typeof(System.Reflection.AssemblyProductAttribute));
                Console.WriteLine(asmprd.Product);


                //AssemblyCopyrightの取得
                System.Reflection.AssemblyCopyrightAttribute asmcpy =
                    (System.Reflection.AssemblyCopyrightAttribute)
                    Attribute.GetCustomAttribute(
                    System.Reflection.Assembly.GetExecutingAssembly(),
                    typeof(System.Reflection.AssemblyCopyrightAttribute));

                //AssemblyCompanyの取得
                System.Reflection.AssemblyCompanyAttribute asmcmp =
                    (System.Reflection.AssemblyCompanyAttribute)
                    Attribute.GetCustomAttribute(
                    System.Reflection.Assembly.GetExecutingAssembly(),
                    typeof(System.Reflection.AssemblyCompanyAttribute));
                Console.WriteLine(asmcmp.Company);

                //自分自身のAssemblyを取得
                System.Reflection.Assembly asm =
                    System.Reflection.Assembly.GetExecutingAssembly();
                //バージョンの取得
                System.Version ver = asm.GetName().Version;

                progressText.Text = asmprd.Product + " Ver." + ver + " " + asmcpy.Copyright + " " + asmcmp.Company;

            }
        }


        private void Menu_Configration_Click(object sender, RoutedEventArgs e)
        {
            showSetting();
        }


        private void Menu_FileBrowse_Click(object sender, RoutedEventArgs e)
        {
            showFileBrowser();
        }

        private void Menu_WebBrowse_Click(object sender, RoutedEventArgs e)
        {
            showWebBrowser();
        }
        #endregion
        //
        #region WebView2_Event **************************************
        // MiteneWebView2
        // ref https://note.dokeep.jp/post/wpf-webview2-events/

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
        #endregion
        //
        #region CoreView2_Event **************************************

        // MiteneWebView.CoreWebView2


        private async void CoreWebView2OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.NavigationCompleted: {nameof(e.NavigationId)} = {e.NavigationId}, {nameof(e.IsSuccess)} = {e.IsSuccess}, {nameof(e.HttpStatusCode)} = {e.HttpStatusCode}, {nameof(e.WebErrorStatus)} = {e.WebErrorStatus}");

            if (!e.IsSuccess && e.HttpStatusCode == 0) return;

            bool isCorrectPage = await checkMitenePage();

            if (isMiteneDataPage)
            {
                Menu_DoStart.Visibility = Visibility.Visible;
                Menu_WebBrowse.Visibility = Visibility.Collapsed;
            }
            else
            {
                Menu_DoStart.Visibility = Visibility.Collapsed;
                Menu_WebBrowse.Visibility = Visibility.Visible;
            }

            if (isCorrectPage)
            {
                //データ読込処理中の場合
                if (inDataReadProsess)
                {
                    ReadData();
                    if (!DataReadComplete)
                    {
                        nextPage();
                    }
                }
                if (DataDownloadComplete) DataDownloadComplete = false;
            }
            else
            {
                MessageEx.ShowWarningDialog("みてねの共有URLではありません。正しい共有URLを設定してください。", Window.GetWindow(this));
                showSetting();
            }
        }

        private void CoreWebView2OnDownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
        {
            Debug.Print($"MiteneWebView.CoreWebView2.DownloadStarting: {nameof(e.ResultFilePath)} = {e.ResultFilePath}");
            string download_path = e.ResultFilePath;
            string newpath = getRenameSavePath(download_path);
            if (!System.IO.File.Exists(newpath))
            {
                e.ResultFilePath = newpath;

                // Ref https://stackoverflow.com/questions/67537998/webview2-download-progress
                var DownloadOperation = e.DownloadOperation;

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

        #endregion
        //
        #region DownloadOperation_Event **************************************

        // Ref https://stackoverflow.com/questions/67537998/webview2-download-progress

        private void DownloadOperation_EstimatedEndTimeChanged(object sender, object e)
        {
            var ans = downloadOperation.EstimatedEndTime.ToString(); // Show the progress
            var fi = downloadOperation.ResultFilePath.ToString();
            Debug.Print("MiteneWebView.CoreWebView2.DownloadOperation_EstimatedEndTimeChanged " + ans + "(" + fi + ")");
        }

        private void DownloadOperation_BytesReceivedChanged(object sender, object e)
        {
            var recive = downloadOperation.BytesReceived.ToString(); // Show the progress
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
        #endregion
        //
        #region Not Use CoreView2_Event **************************************
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
        #endregion

        private void CmbUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initOK) return;
            UserSettingLoad(CmbUser.SelectedIndex);
            PageLoading();
        }

        private void BtnSettingExport_Click(object sender, RoutedEventArgs e)
        {
            // ref https://qiita.com/minoru-nagasawa/items/7d173274b4468fd9629a
            // ファイルを選択
            string fullPath;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = "mitene.config";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                sfd.Filter = "設定ファイル(*.config)|*.config";
                sfd.FilterIndex = 1;
                sfd.Title = "エクスポート先のファイルを選択してください";
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                fullPath = sfd.FileName;
            }

            // ファイルをコピー
            try
            {
                // user.configのパスを取得
                string userConfigPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

                // ファイルが無ければSave()して生成する
                if (!System.IO.File.Exists(userConfigPath))
                {
                    Settings.Default.Save();
                }

                // エクスポートはファイルをコピーするだけ
                System.IO.File.Copy(userConfigPath, fullPath, true);
                MessageEx.ShowInformationDialog("エクスポートしました", Window.GetWindow(this));
            }
            catch (Exception ex)
            {
                //MessageBox.Show().Show(ex.ToString(), "エクスポート失敗", MessageBoxButtons.OK);
                string message = ex.Message + "\nエクスポート失敗";
                MessageEx.ShowErrorDialog(message, Window.GetWindow(this));
            }
        }

        private void BtnSettingInport_Click(object sender, RoutedEventArgs e)
        {
            // ファイル選択
            string exportfilefullPath = "";
            using (var ofd = new OpenFileDialog())
            {
                ofd.FileName = "mitene.config";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                ofd.Filter = "設定ファイル(*.config)|*.config";
                ofd.FilterIndex = 1;
                ofd.Title = "インポートするファイルを選択してください";
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                exportfilefullPath = ofd.FileName;
            }

            try
            {

                // user.configのパスを取得
                string userConfigPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                // ファイルが無ければSave()して生成する
                if (!System.IO.File.Exists(userConfigPath))
                {
                    saveSetting();
                }
                // インポートはファイルをコピーするだけ
                System.IO.File.Copy(exportfilefullPath, userConfigPath, true);
                MessageEx.ShowInformationDialog("インポートしました", Window.GetWindow(this));
                Settings.Default.Reload();
                UserSettingLoad(Select_User);
            }
            catch (Exception ex)
            {
                //MessageBox.Show().Show(ex.ToString(), "エクスポート失敗", MessageBoxButtons.OK);
                string message = ex.Message + "\nインポート失敗";
                MessageEx.ShowErrorDialog(message, Window.GetWindow(this));
            }


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
                string value = line.Substring(top2, line.Length - top2);

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

