using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ProgressControlSample.Download
{
  public  class DownloadServiceClient
    {
        public async Task Download(Progress<double> progress = null)
        {
            HttpResponse r;r.
            var downloader = new Windows.Networking.BackgroundTransfer.BackgroundDownloader();
            Task s;
            
            // Create a new download operation.
         var   ownload = downloader.CreateDownload(null, null,null).StartAsync().AsTask().w;

            // Start the download and persist the promise to be able to cancel the download.
            promise = ownload.Progress....startAsync().then(complete, error, progress);
        }
    }
}
 