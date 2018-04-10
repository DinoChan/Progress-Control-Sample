using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProgressControlSample.Annotations;
using System.Threading;
using System.Web;

namespace ProgressControlSample
{
    public class Downloader : INotifyPropertyChanged
    {
        public static async Task<Downloader> Create(Uri uri)
        {
            var downloader = new Downloader(uri);
            await downloader.LoadInformation();
            return downloader;
        }

        private Downloader(Uri uri)
        {
            _uri = uri;
        }

        private readonly Uri _uri;

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

        public async Task StartDownload(IProgress<double> progress, CancellationToken cancellationToken)
        {

        }

        private async Task LoadInformation()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            Name = _uri.AbsoluteUri;
            var random = new Random();
            const int maxValue = 10 * 1024 * 1024;
            TotalBytes = random.Next(maxValue);

            if (Name.Contains("Timeout"))
                await Task.Delay(TimeSpan.FromSeconds(10));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
