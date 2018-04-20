using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProgressControlSample.Annotations;
using System.Threading;

namespace ProgressControlSample
{
    public class Downloader : INotifyPropertyChanged
    {
        private static int _count;

        public static async Task<Downloader> Create(Uri uri, CancellationToken cancellationToken)
        {
            var downloader = new Downloader(uri);
            await downloader.LoadInformation(cancellationToken);
            return downloader;
        }

        public static async Task<Downloader> Create(Uri uri)
        {
            var downloader = new Downloader(uri);
            await downloader.LoadInformation();
            return downloader;
        }


        private Downloader(Uri uri)
        {
            Uri = uri;
        }

        public Uri Uri { get; }

        public string Name { get; private set; }

        public long TotalBytes { get; private set; }


        private long _receivedBytes;

        /// <summary>
        /// 获取或设置 ReceivedBytes 的值
        /// </summary>
        public long ReceivedBytes
        {
            get => _receivedBytes;
            set
            {
                if (_receivedBytes == value)
                    return;

                _receivedBytes = value;
                OnPropertyChanged();
            }
        }

        public async Task StartDownload(IProgress<long> progress, CancellationToken cancellationToken)
        {
            var random = new Random();
            while (ReceivedBytes < TotalBytes)
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    await Task.Delay(100, cts.Token);
                    var bytesReceived = random.Next(1024 * 1024);
                    ReceivedBytes += bytesReceived;
                    progress?.Report(bytesReceived);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        private async Task LoadInformation(CancellationToken cancellationToken)
        {

            await Task.Delay(TimeSpan.FromSeconds(_count++ * 0.5), cancellationToken);
            Name = Uri.AbsoluteUri;

            const int maxValue = 10 * 1024 * 1024;
            var random = new Random();
            TotalBytes = random.Next(maxValue);

            if (Name.Contains("timeout"))
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            if (Name.Contains("error"))
                throw new Exception("Something error");
        }

        private async Task LoadInformation()
        {
            await Task.Delay(1000);

            Name = Uri.AbsoluteUri;
            var random = new Random();
            const int maxValue = 10 * 1024 * 1024;
            TotalBytes = random.Next(maxValue);

            if (Name.Contains("error"))
                throw new Exception("Something error");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
