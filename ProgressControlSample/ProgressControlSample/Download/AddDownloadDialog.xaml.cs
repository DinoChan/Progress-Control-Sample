using System;
using System.Collections.Generic;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace ProgressControlSample.Download
{
    public sealed partial class AddDownloadDialog : ContentDialog, INotifyPropertyChanged
    {
        private int _totalLinks;

        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);

        public AddDownloadDialog()
        {
            this.InitializeComponent();
        }


        [CanBeNull]
        private async Task<IEnumerable<Downloader>> AddNewDownload(IEnumerable<Uri> links)
        {
            var downlodTasks = links.Select(Downloader.Create);
            var downlodTasksArray = downlodTasks.ToArray();

            var downloads = await Task.WhenAll(downlodTasksArray);
            return downloads;
        }


        [CanBeNull]
        private async Task<IEnumerable<Downloader>> AddNewDownload2(IEnumerable<Uri> links)
        {
            var downlodTasks = links.Select(link =>
            {
                var cts = new CancellationTokenSource();
                var token = cts.Token;
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                return Downloader.Create(link, token);
            });
            var downlodTasksArray = downlodTasks.ToArray();

            var downloads = await Task.WhenAll(downlodTasksArray);
            return downloads;
        }


        [CanBeNull]
        private async Task<IEnumerable<Downloader>> AddNewDownload3(IEnumerable<Uri> links)
        {
            TotalLinks = links.Count();
            _finishedLinks = 0;
            var downlodTasks = links.Select(link =>
            {
                var cts = new CancellationTokenSource();
                var token = cts.Token;
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                var downloader= Downloader.Create(link, token);
                return downloader;
            });
            
            var downlodTasksArray = downlodTasks.ToArray();

            var downloads = await Task.WhenAll(downlodTasksArray);
            return downloads;
        }

        public int TotalLinks
        {
            get => _totalLinks;
            set
            {
                _totalLinks = value;
                OnPropertyChanged();
            }
        }


        private int _finishedLinks;

        public int FinishedLinks
        {
            get => _finishedLinks;
            set
            {
                _finishedLinks = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
