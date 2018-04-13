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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace ProgressControlSample.Download
{
    public sealed partial class AddDownloadDialog : ContentDialog, INotifyPropertyChanged
    {
        private static int _count;

        private int _totalLinks;

        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);

        public AddDownloadDialog()
        {
            this.InitializeComponent();
            //ProgressControl.IsEnabled = false;
            //Links.CollectionChanged+= (s, e) => ProgressControl.IsEnabled = Links.Any();
           
        }

        

        public ObservableCollection<Uri> Links { get; } = new ObservableCollection<Uri>();
        private readonly List<Downloader> _downloads = new List<Downloader>();
        private CancellationTokenSource _cancellationTokenSource;


        public IEnumerable<Downloader> Downloads { get; private set; }

        private async Task<IEnumerable<Downloader>> AddNewDownload(IEnumerable<Uri> links, CancellationToken cancellationToken)
        {
            var downlodTasks = links.Select(Downloader.Create);
            var downlodTasksArray = downlodTasks.ToArray();
            var downloads = await Task.WhenAll(downlodTasksArray);
            return downloads;
        }


        private async Task<IEnumerable<Downloader>> AddNewDownload2(IEnumerable<Uri> links, CancellationToken cancellationToken)
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


        private async Task<IEnumerable<Downloader>> AddNewDownload3(IEnumerable<Uri> links, CancellationToken cancellationToken)
        {
            TotalLinks = Links.Count();
            _finishedTasks = _downloads.Count;

            Task<Downloader> Selector(Uri link)
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    var token = cts.Token;
                    cts.CancelAfter(TimeSpan.FromSeconds(5));
                    var downloader = Downloader.Create(link, token);
                    return downloader;
                }
            }

            var downlodTasks = links.Select(Selector);

            var progressTasks = downlodTasks.Select(async t =>
            {
                var result = await t;
                await _mutex.WaitAsync(cancellationToken);
                try
                {
                    FinishedTasks++;
                    _downloads.Add(t.Result);
                }
                finally
                {
                    _mutex.Release();
                }

                return result;
            }).ToArray();


            var downloads = await Task.WhenAll(progressTasks);
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


        private int _finishedTasks;

        public int FinishedTasks
        {
            get => _finishedTasks;
            set
            {
                _finishedTasks = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private async void OnStateChanged(object sender, ProgressStateEventArgs e)
        {
            switch (e.NewValue)
            {
                case ProgressState.Ready:
                    if (e.OldValue == ProgressState.Paused)
                        _downloads.Clear();
                    break;
                case ProgressState.Started:
                    {
                        try
                        {
                            _cancellationTokenSource = new CancellationTokenSource();
                            await AddNewDownload(_cancellationTokenSource.Token);
                            Downloads = _downloads;
                            ProgressControl.State = ProgressState.Completed;
                            await Task.Delay(TimeSpan.FromSeconds(2));
                            Hide();
                        }
                        catch (OperationCanceledException ex)
                        {
                            InAppNotification.Show("Task Paused:" + ex.Message, 1000);
                        }
                        catch (Exception ex)
                        {
                            ProgressControl.State = ProgressState.Faulted;
                            InAppNotification.Show("Task Error:" + ex.Message, 1000);
                        }
                    }
                    break;
                case ProgressState.Completed:
                    break;
                case ProgressState.Faulted:
                    _downloads.Clear();
                    break;
                case ProgressState.Paused:
                    _cancellationTokenSource?.Cancel();
                    break;
                default:
                    break;
            }
        }


        private async Task AddNewDownload(CancellationToken ccancellationToken)
        {
            var links = Links.Where(l => _downloads.Select(d => d.Uri).Contains(l) == false).ToList();
            await AddNewDownload(links, ccancellationToken);
        }

        private void OnAddNormalLink(object sender, RoutedEventArgs e)
        {
            Links.Add(new Uri("http://Link" + _count++, UriKind.RelativeOrAbsolute));
        }

        private void OnAddTimeOutLink(object sender, RoutedEventArgs e)
        {
            Links.Add(new Uri("http://timeoutLink" + _count++, UriKind.RelativeOrAbsolute));
        }

        private void OnAddErrorLink(object sender, RoutedEventArgs e)
        {
            Links.Add(new Uri("http://errorLink" + _count++, UriKind.RelativeOrAbsolute));
        }

       
    }
}
