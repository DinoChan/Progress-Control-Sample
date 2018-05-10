using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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

        private DateTime _now;
        public DownloadPage()
        {
            this.InitializeComponent();
            _semaphore = new SemaphoreSlim(5);
            var progress = new Progress<int>();
            _progress = progress;
            var reports = Observable.FromEventPattern<int>(handler => progress.ProgressChanged += handler, handler => progress.ProgressChanged -= handler);
            reports.Buffer(TimeSpan.FromSeconds(1)).Subscribe(async x =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                 {
                     SpeedElement.Text = string.Format("{0} Bytes/S", x.Sum(s => s.EventArgs).ToString("N0"));
                 });
            });
        }

        private readonly SemaphoreSlim _semaphore;
        private readonly IProgress<int> _progress;

        public ObservableCollection<DownloaderModel> Downloads { get; } = new ObservableCollection<DownloaderModel>();

        private async void OnAddLinks(object sender, RoutedEventArgs e)
        {
            var dialog = new AddDownloadDialog();
            await dialog.ShowAsync();
            if (dialog.Downloads == null)
                return;
            _now = DateTime.Now;
            var tasks = dialog.Downloads.Select(async item =>
            {
                var model = new DownloaderModel { Downloader = item };
                Downloads.Add(model);
                model.DownloadedData += OnDownloadData;
                await _semaphore.WaitAsync();
                try
                {
                    await model.StartDownloadAsync();
                }
                catch (OperationCanceledException)
                {
                    //do nothing
                }
                finally
                {
                    _semaphore.Release();
                }
            }).ToArray();
            await Task.WhenAll(tasks);
        }

        private void OnDownloadData(object sender, int e)
        {
            _progress.Report(e);
        }

        private async void OnDownloadStateChanged(object sender, ProgressStateEventArgs e)
        {
            var model = (sender as FrameworkElement)?.DataContext as DownloaderModel;
            switch (e.NewValue)
            {
                case ProgressState.Ready:
                    break;
                case ProgressState.Started:
                    try
                    {
                        await model?.StartDownloadAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        //do nothing
                    }
                    break;
                case ProgressState.Completed:
                    break;
                case ProgressState.Faulted:
                    break;
                case ProgressState.Paused:
                    model?.Stop();
                    break;
                default:
                    break;
            }
        }


    }


   
}
