using ChainResourceService;
using ChainResourceService.Storages;
using NUnit.Framework;
using System;

namespace ExchangeRateInfoTests
{
    public class Tests
    {
        private const string FileSystemName = "exchangeRates.json";
        private const string AppId = "4c9aa2ecaeb44cc39ccb875e253ac959";
        private const string BaseCurrency = "USD";
        private const string ApiUrl = $"https://openexchangerates.org/latest.json?app_id={AppId}&base={BaseCurrency}";

        [Test]
        public async Task GetValue_WebServiceStorage_FirstCall_ReturnsExchangeRateList()
        {
            var webServiceStorage = new WebServiceStorage<ExchangeRateList>(ApiUrl);

            var chainResource = new ChainResource<ExchangeRateList>(webServiceStorage);

            var exchangeRateList = await chainResource.GetValue();

            Assert.IsNotNull(exchangeRateList);
        }

        [Test]
        public async Task GetValue_WebServiceStorage_SecondCall_WhereMemoryAndFSExpired_ReturnsExchangeRateList()
        {
            var memoryStorage = new MemoryStorage<ExchangeRateList>(0);
            var fileSystemStorage = new FileSystemStorage<ExchangeRateList>(FileSystemName, 0);
            var webServiceStorage = new WebServiceStorage<ExchangeRateList>(ApiUrl);

            var chainResource = new ChainResource<ExchangeRateList>(memoryStorage, fileSystemStorage, webServiceStorage);

            await chainResource.GetValue();

            // Sleep for 1 sec to simulate Memory and FS storages expiration
            Thread.Sleep(1000);

            var exchangeRateList = await chainResource.GetValue();

            Assert.IsNotNull(exchangeRateList);
        }

        [Test]
        public async Task GetValue_MemoryStorage_SecondCall_ReturnsExchangeRateList()
        {
            var memoryStorage = new MemoryStorage<ExchangeRateList>(1);
            var webServiceStorage = new WebServiceStorage<ExchangeRateList>(ApiUrl);

            var chainResource = new ChainResource<ExchangeRateList>(memoryStorage, webServiceStorage);

            // Perform the first call to initialize the file system storage
            await chainResource.GetValue();

            // Perform the second call to get data from Memory storage
            var exchangeRateList = await chainResource.GetValue();

            Assert.IsNotNull(exchangeRateList);
        }

        [Test]
        public async Task GetValue_FileSystemStorage_SecondCall_ReturnsExchangeRateList()
        {
            var fileSystemStorage = new FileSystemStorage<ExchangeRateList>(FileSystemName, 4);
            var webServiceStorage = new WebServiceStorage<ExchangeRateList>(ApiUrl);

            var chainResource = new ChainResource<ExchangeRateList>(fileSystemStorage, webServiceStorage);

            // Perform the first call to initialize the FS storage
            await chainResource.GetValue();

            // Perform the second call to get data from FS storage
            var exchangeRateList = await chainResource.GetValue();

            Assert.IsNotNull(exchangeRateList);
        }

        [Test]
        public async Task GetValue_FileSystemStorage_SecondCall_WhereMemoryExpired_ReturnsExchangeRateList()
        {
            var memoryStorage = new MemoryStorage<ExchangeRateList>(0);
            var fileSystemStorage = new FileSystemStorage<ExchangeRateList>(FileSystemName, 4);
            var webServiceStorage = new WebServiceStorage<ExchangeRateList>(ApiUrl);

            var chainResource = new ChainResource<ExchangeRateList>(memoryStorage, fileSystemStorage, webServiceStorage);

            // Perform the first call to initialize the Memory and FS storages
            await chainResource.GetValue();

            // Sleep for 1 sec to simulate Memory storage expiration
            Thread.Sleep(1000);

            var exchangeRateList = await chainResource.GetValue();

            Assert.IsNotNull(exchangeRateList);
        }
    }
}