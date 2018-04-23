using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ProgressControlSample.Annotations;
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

        public ObservableCollection<DownloaderModel> Downloads { get; } = new ObservableCollection<DownloaderModel>();

        private async void OnAddLinks(object sender, RoutedEventArgs e)
        {
            var dialog = new AddDownloadDialog();
            await dialog.ShowAsync();
            if (dialog.Downloads == null)
                return;

            foreach (var item in dialog.Downloads)
            {
                var model = new DownloaderModel { Downloader = item, State = ProgressState.Started };
                Downloads.Add(model);
                model.DownloadedData += OnDownloadData;
                model.StartDownload();
            }
        }

        private void OnDownloadData(object sender, int e)
        {
        }
    }


    public class DownloaderModel : INotifyPropertyChanged
    {
        public Downloader Downloader { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;


        private ProgressState _state;

        /// <summary>
        /// 获取或设置 State 的值
        /// </summary>
        public ProgressState State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                    return;

                _state = value;
                OnPropertyChanged();
            }
        }


        public async Task StartDownload()
        {
            var progress = new Progress<int>();
            progress.ProgressChanged += (s, e) =>
            {
                DownloadedData?.Invoke(this, e);
                OnPropertyChanged("Downloader");
            };
            var cancellationToken = new CancellationToken();
            await Downloader.StartDownload(progress, cancellationToken);
            State = ProgressState.Completed;
        }

        public event EventHandler<int> DownloadedData;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
