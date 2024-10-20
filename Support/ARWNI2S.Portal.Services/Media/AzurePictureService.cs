﻿using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Services.Configuration;
using ARWNI2S.Node.Services.Logging;
using ARWNI2S.Portal.Services.Entities.Media;
using ARWNI2S.Portal.Services.Seo;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ARWNI2S.Portal.Services.Media
{
    /// <summary>
    /// Picture service for Windows Azure
    /// </summary>
    public partial class AzurePictureService : PictureService
    {
        #region Fields

        private static BlobContainerClient _blobContainerClient;
        private static BlobServiceClient _blobServiceClient;
        private static bool _azureBlobStorageAppendContainerName;
        private static bool _isInitialized;
        private static string _azureBlobStorageConnectionString;
        private static string _azureBlobStorageContainerName;
        private static string _azureBlobStorageEndPoint;

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly MediaSettings _mediaSettings;

        private readonly object _locker = new();

        #endregion

        #region Ctor

        public AzurePictureService(NI2SSettings ni2sSettings,
            IDownloadService downloadService,
            IHttpContextAccessor httpContextAccessor,
            ILogService logger,
            IEngineFileProvider fileProvider,
            IRepository<Picture> pictureRepository,
            IRepository<PictureBinary> pictureBinaryRepository,
            //IRepository<TitlePicture> titlePictureRepository,
            //IRepository<QuestPicture> questPictureRepository,
            //IRepository<TournamentPicture> tournamentPictureRepository,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            MediaSettings mediaSettings)
            : base(downloadService,
                  httpContextAccessor,
                  logger,
                  fileProvider,
                  pictureRepository,
                  pictureBinaryRepository,
                  //titlePictureRepository,
                  //questPictureRepository,
                  //tournamentPictureRepository,
                  settingService,
                  urlRecordService,
                  webHelper,
                  mediaSettings)
        {
            _staticCacheManager = staticCacheManager;
            _mediaSettings = mediaSettings;

            OneTimeInit(ni2sSettings);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Initialize cloud container
        /// </summary>
        /// <param name="ni2sSettings">App settings</param>
        protected void OneTimeInit(NI2SSettings ni2sSettings)
        {
            if (_isInitialized)
                return;

            if (string.IsNullOrEmpty(ni2sSettings.Get<AzureBlobConfig>().ConnectionString))
                throw new NodeException("Azure connection string for Blob is not specified");

            if (string.IsNullOrEmpty(ni2sSettings.Get<AzureBlobConfig>().ContainerName))
                throw new NodeException("Azure container name for Blob is not specified");

            if (string.IsNullOrEmpty(ni2sSettings.Get<AzureBlobConfig>().EndPoint))
                throw new NodeException("Azure end point for Blob is not specified");

            lock (_locker)
            {
                if (_isInitialized)
                    return;

                _azureBlobStorageAppendContainerName = ni2sSettings.Get<AzureBlobConfig>().AppendContainerName;
                _azureBlobStorageConnectionString = ni2sSettings.Get<AzureBlobConfig>().ConnectionString;
                _azureBlobStorageContainerName = ni2sSettings.Get<AzureBlobConfig>().ContainerName.Trim().ToLowerInvariant();
                _azureBlobStorageEndPoint = ni2sSettings.Get<AzureBlobConfig>().EndPoint.Trim().ToLowerInvariant().TrimEnd('/');

                _blobServiceClient = new BlobServiceClient(_azureBlobStorageConnectionString);
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_azureBlobStorageContainerName);

                CreateCloudBlobContainer().GetAwaiter().GetResult();

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Create cloud Blob container
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task CreateCloudBlobContainer()
        {
            await _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        }

        /// <summary>
        /// Get picture (thumb) local path
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the local picture thumb path
        /// </returns>
        protected override Task<string> GetThumbLocalPathAsync(string thumbFileName)
        {
            var path = _azureBlobStorageAppendContainerName ? $"{_azureBlobStorageContainerName}/" : string.Empty;

            return Task.FromResult($"{_azureBlobStorageEndPoint}/{path}{thumbFileName}");
        }

        /// <summary>
        /// Get picture (thumb) URL 
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <param name="serverLocation">Node location URL; null to use determine the current server location automatically</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the local picture thumb path
        /// </returns>
        protected override async Task<string> GetThumbUrlAsync(string thumbFileName, string serverLocation = null)
        {
            return await GetThumbLocalPathAsync(thumbFileName);
        }

        /// <summary>
        /// Initiates an asynchronous operation to delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task DeletePictureThumbsAsync(Picture picture)
        {
            //create a string containing the Blob name prefix
            var prefix = $"{picture.Id:0000000}";

            var tasks = new List<Task>();
            await foreach (var blob in _blobContainerClient.GetBlobsAsync(BlobTraits.All, BlobStates.All, prefix))
            {
                tasks.Add(_blobContainerClient.DeleteBlobIfExistsAsync(blob.Name, DeleteSnapshotsOption.IncludeSnapshots));
            }
            await Task.WhenAll(tasks);

            await _staticCacheManager.RemoveByPrefixAsync(MediaServicesDefaults.ThumbsExistsPrefix);
        }

        /// <summary>
        /// Initiates an asynchronous operation to get a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected override async Task<bool> GeneratedThumbExistsAsync(string thumbFilePath, string thumbFileName)
        {
            try
            {
                var key = _staticCacheManager.PrepareKeyForDefaultCache(MediaServicesDefaults.ThumbExistsCacheKey, thumbFileName);

                return await _staticCacheManager.GetAsync(key, async () =>
                {
                    return await _blobContainerClient.GetBlobClient(thumbFileName).ExistsAsync();
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Initiates an asynchronous operation to save a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <param name="mimeType">MIME type</param>
        /// <param name="binary">Picture binary</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
        {
            var blobClient = _blobContainerClient.GetBlobClient(thumbFileName);
            await using var ms = new MemoryStream(binary);

            //set mime type
            BlobHttpHeaders headers = null;
            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                headers = new BlobHttpHeaders
                {
                    ContentType = mimeType
                };
            }

            //set cache control
            if (!string.IsNullOrWhiteSpace(_mediaSettings.AzureCacheControlHeader))
            {
                headers ??= new BlobHttpHeaders();
                headers.CacheControl = _mediaSettings.AzureCacheControlHeader;
            }

            if (headers is null)
                //We must explicitly indicate through the parameter that the object needs to be overwritten if it already exists
                //See more: https://github.com/Azure/azure-sdk-for-net/issues/9470
                await blobClient.UploadAsync(ms, overwrite: true);
            else
                await blobClient.UploadAsync(ms, new BlobUploadOptions { HttpHeaders = headers });

            await _staticCacheManager.RemoveByPrefixAsync(MediaServicesDefaults.ThumbsExistsPrefix);
        }

        #endregion
    }
}