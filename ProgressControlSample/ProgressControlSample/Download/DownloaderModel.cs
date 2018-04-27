using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProgressControlSample.Annotations;

namespace ProgressControlSample.Download
{
    public class DownloaderModel : INotifyPropertyChanged
    {
        public Downloader Downloader { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private CancellationTokenSource _cancellationTokenSource;

        private ProgressState _state;
        private bool _isDownloaeding;

        /// <summary>
        /// 获取或设置 State 的值
        /// </summary>
        public ProgressState State
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                _state = value;
                OnPropertyChanged();
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public async Task StartDownload()
        {
            if (_isDownloaeding)
                return;

            _isDownloaeding = true;
            try
            {
                State = ProgressState.Started;
                var progress = new Progress<int>();
                progress.ProgressChanged += (s, e) =>
                {
                    DownloadedData?.Invoke(this, e);
                    OnPropertyChanged(nameof(Downloader));
                };
                _cancellationTokenSource = new CancellationTokenSource();
                await Downloader.StartDownload(progress, _cancellationTokenSource.Token);
                State = ProgressState.Completed;
            }
            finally
            {
                _isDownloaeding = false;
            }
        }

        public event EventHandler<int> DownloadedData;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
