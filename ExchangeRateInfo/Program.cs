using ChainResourceService;
using ChainResourceService.Storages;

string appId = "4c9aa2ecaeb44cc39ccb875e253ac959";
string _base = "USD";
var memoryStorage = new MemoryStorage<ExchangeRateList>(1);
var fileSystemStorage = new FileSystemStorage<ExchangeRateList>("exchangeRates.json", 4);
var webServiceStorage = new WebServiceStorage<ExchangeRateList>($"https://openexchangerates.org/latest.json?app_id={appId}&base={_base}");

var chainResource = new ChainResource<ExchangeRateList>(memoryStorage, fileSystemStorage, webServiceStorage);

var exchangeRateList = await chainResource.GetValue();

if (exchangeRateList != null)
{
    Console.WriteLine($"Disclaimer={exchangeRateList.Disclaimer},\n" +
                      $"License={exchangeRateList.License},\n" +
                      $"Timestamp={exchangeRateList.Timestamp},\n" +
                      $"Base={exchangeRateList.Base},\n" +
                      $"Rates:\n");
    foreach (var rate in exchangeRateList?.Rates)
    {
        Console.WriteLine($"{rate.Key}:{rate.Value},\n");
    }
}

Console.ReadKey();

public class ExchangeRateList
{
    public string Disclaimer { get; set; }
    public string License { get; set; }
    public long Timestamp { get; set; }
    public string Base { get; set; }
    public Dictionary<string, double> Rates { get; set; }
}
