using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ProgressControlSample.Download;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace ProgressControlSample
{
    public sealed partial class DownloadPage : UserControl
    {
        public DownloadPage()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<Downloader> Downloads { get; } = new ObservableCollection<Downloader>();

        private async void OnAddLinks(object sender, RoutedEventArgs e)
        {
            var dialog = new AddDownloadDialog();
            await dialog.ShowAsync();
            if (dialog.Downloads == null)
                return;

            foreach (var item in dialog.Downloads)
            {
                Downloads.Add(item);
            }

        }
    }
}
