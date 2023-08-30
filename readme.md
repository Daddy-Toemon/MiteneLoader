# みてねローダ

"みてねローダ" は「[みてね](https://mitene.us/)」ブラウザ版の共有URLを設定することで写真や動画をパソコンに一括ダウンロードするWindowsアプリケーションです。

# デモ


https://github.com/BB-Sawabe/MiteneLoader/assets/88169524/39941627-191e-4c6f-ab1d-ab71ebc10528

注意　デモ動画は早送り動画です。

# 特徴

でっちあげアプリなのでソースは汚いです。

デバッグ環境では動作しますが、実行環境では、  
System.DllNotFoundException Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateCoreWebView2EnvironmentWithOptionsのエラーが発生する問題は解決されていません


# 使用パッケージ
このアプリの作成には以下のパッケージを利用しています。

* [Microsoft.Web.WebView2](https://learn.microsoft.com/ja-jp/microsoft-edge/webview2/) (Wpf:WebView2)
* [Microsoft-WindowsAPICodePack-Shell](https://github.com/contre/Windows-API-Code-Pack-1.1) (wfc:ExplorerBrowser)
* [Microsoft-WindowsAPICodePack-Core](https://github.com/contre/Windows-API-Code-Pack-1.1) (wfc:ExplorerBrowser)
* [.NET Framework 4.7.2](https://dotnet.microsoft.com/ja-jp/download/dotnet-framework/net472)
* [MessageBoxWPF](https://github.com/mikihiro-t/MessageBoxWPF/tree/master)

# インストール

インストーラはありません。　Visual Studio 2022のプロジェクトファイルです

# 使用法
**設定・システム構成画面**

* みてねの共有URL　ｈttps://mitene.us/f/****** を設定します。
* 保存ディレクトリ（保存ディレクトリは先に作成しておく必要があります)
* フォルダー構成  年・月フォルダーに分ける  
有効にすると、処理開始の際に、年・月フォルダーに分けたフォルダーにデータが保存されます。(途中変更にも対応します)
* ログイン　終了時ログイン状態をクリアする  
有効にすると、終了時にLogin Cookieがクリアされアプリケーション実行毎にログインが必要となります。  


# 備考

みてねローダは、Windows11でしか動作確認していません。

# 作者

* Toemon

# 謝辞
作成に当たり、以下のサイトを参考にさせていただきました。　ありがとうございます。

[Unable to clear user data with ClearBrowsingDataAsync](https://github.com/MicrosoftEdge/WebView2Feedback/issues/2582)  
[WebView2 で自分好みのブラウザを作る](https://qiita.com/so_nkbys/items/a03242f5089a2c2c8a66)  
[[WPF] WebView2 のイベントをデバッグで確認してみる](https://note.dokeep.jp/post/wpf-webview2-events/)  
[WebView2 Download progress](https://stackoverflow.com/questions/67537998/webview2-download-progress)  
[[WPF]フォルダ選択ダイアログは一択です[Windows]](https://threeshark3.com/commonopenfiledialog/)  
[[WPF＋C#]フォルダエクスプローラーを組み込む](https://resanaplaza.com/2022/04/29/%E3%80%90wpf%EF%BC%8Bc%E3%80%91%E3%83%95%E3%82%A9%E3%83%AB%E3%83%80%E3%82%A8%E3%82%AF%E3%82%B9%E3%83%97%E3%83%AD%E3%83%BC%E3%83%A9%E3%83%BC%E3%82%92%E7%B5%84%E3%81%BF%E8%BE%BC%E3%82%80%EF%BC%88wndows/)  
[Microsoft Edge WebView2ランタイムを手動でインストールする](https://community.f-secure.com/total-ja/kb/articles/9218-microsoft-edge-webview2%E3%83%A9%E3%83%B3%E3%82%BF%E3%82%A4%E3%83%A0%E3%82%92%E6%89%8B%E5%8B%95%E3%81%A7%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB%E3%81%99%E3%82%8B)  
[いい感じのMessageBox for WPF](https://qiita.com/hiro_t/items/5a2637179d6f580738de)  

# ライセンス

Copyright (c) 2023 Toemon
Released under the MIT license  
https://opensource.org/license/mit/ (English)  
https://licenses.opensource.jp/MIT/MIT.html (日本語)  

「みてね」の一括ダウンロードにご利用ください

ありがとう
