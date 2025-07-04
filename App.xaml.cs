using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MiteneLoader
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (MiteneLoader.Properties.Settings.Default.MustUpgrade)
            {
                MiteneLoader.Properties.Settings.Default.Upgrade(); // 前バージョンの設定を引き継ぐ
                MiteneLoader.Properties.Settings.Default.MustUpgrade = false; // 2回目以降は実行しない
                MiteneLoader.Properties.Settings.Default.Save(); // 保存
            }

            // メインウィンドウ起動
            //var mainWindow = new MainWindow();
            //mainWindow.Show();
        }

    }
}
