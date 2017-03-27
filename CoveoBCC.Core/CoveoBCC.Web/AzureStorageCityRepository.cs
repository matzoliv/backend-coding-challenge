using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CoveoBCC.Core
{
    public class AzureStorageCityRepository : ICityRepository
    {
        private readonly List<City> m_cities;

        public AzureStorageCityRepository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse( @"" );

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference( "cities-big" );
            var blob = blobContainer.GetBlobReference( "cities_canada-usa.tsv.gz" );

            var memoryStream = new MemoryStream();
            blob.DownloadToStream( memoryStream );
            memoryStream.Flush();
            memoryStream.Position = 0;

            m_cities = new List<City>();

            using ( GZipStream decompressionStream = new GZipStream( memoryStream, CompressionMode.Decompress ) )
            {
                using ( var streamReader = new System.IO.StreamReader( decompressionStream ) )
                {
                    streamReader.ReadLine();

                    while ( !streamReader.EndOfStream )
                    {
                        m_cities.Add( City.FromTabbedSeparatedLine( streamReader.ReadLine() ) );
                    }
                }
            }
        }

        public IEnumerable<City> GetAllCities ()
        {
            return m_cities;
        }
    }
}
