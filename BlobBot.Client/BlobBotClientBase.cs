using BlobBot.Shared.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlobBot.Client
{
    /// <summary>
    /// queries for new blobs added by Azure EventGrid, and downloads them for offline processing,
    /// executing whatever process you need to
    /// </summary>
    public abstract class BlobBotClientBase : BackgroundService
    {
        private readonly string _machineName;
        private readonly ILogger<BlobBotClientBase> _logger;

        private bool _running;

        public BlobBotClientBase(ILogger<BlobBotClientBase> logger, string machineName)
        {
            _machineName = machineName;            
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            _running = true;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _running = false;
        }

        protected abstract Task<IEnumerable<IBlobInfo>> GetNewBlobsAsync();

        protected abstract Task UpdateBlobInfoAsync(IBlobInfo blobInfo);

        /// <summary>
        /// whatever work you need to do with blob goes here,
        /// conversions, OCR, workflow or database updates, whatever
        /// </summary>
        protected abstract Task ProcessBlobAsync(string localFile, IBlobInfo blobInfo);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (_running)
            {
                if (stoppingToken.IsCancellationRequested) break;

                var blobs = await GetNewBlobsAsync();

                foreach (var blob in blobs)
                {
                    blob.Status = Status.Downloading;                    
                    blob.ProcessedBy = _machineName;
                    blob.StatusDateTime = DateTime.UtcNow;
                    await UpdateBlobInfoAsync(blob);

                    var download = await DownloadAsync(blob, stoppingToken);

                    if (download.success)
                    {
                        blob.Status = Status.Processing;
                        blob.StatusDateTime = DateTime.UtcNow;
                        await UpdateBlobInfoAsync(blob);

                        try
                        {
                            await ProcessBlobAsync(download.localFile, blob);
                            blob.Status = Status.Succeeded;                            
                        }
                        catch (Exception exc)
                        {
                            blob.Status = Status.ProcessFailed;     
                            blob.ErrorMessage = exc.Message;
                        }
                    }
                    else
                    {
                        blob.Status = Status.DownloadFailed;
                        blob.ErrorMessage = download.errorMessage;
                    }

                    blob.StatusDateTime = DateTime.UtcNow;
                    await UpdateBlobInfoAsync(blob);
                }
            }
        }

        private Task<(bool success, string errorMessage, string localFile)> DownloadAsync(IBlobInfo blob, CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}