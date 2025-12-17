using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class CurrencyConverter : MonoBehaviour
{
    private const string API_URL = "https://api.exchangerate-api.com/v4/latest/USD"; // API URL
    [SerializeField] private Dictionary<string, float> exchangeRates = new Dictionary<string, float>();
    [SerializeField] private List<string> lstKey = new List<string>();

    public static CurrencyConverter Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Lấy tỷ giá từ API
    public async Task<bool> FetchExchangeRates()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(API_URL))
        {
            Debug.Log("Fetching exchange rates...");
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield(); // Chờ kết quả từ API

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log($"Response JSON: {json}");

                try
                {
                    // Parse JSON
                    var rawResponse = JsonUtility.FromJson<RawResponse>(json);
                    Debug.Log(rawResponse.rates.Count);
                    // Nếu dữ liệu trả về hợp lệ
                    if (rawResponse != null && rawResponse.rates != null)
                    {
                        // Chuyển đổi rates thành Dictionary
                        exchangeRates.Clear();
                        foreach (var rate in rawResponse.rates)
                        {
                            exchangeRates[rate.currency] = rate.value;
                        }

                        // Làm mới danh sách key
                        lstKey.Clear();
                        foreach (var key in exchangeRates.Keys)
                        {
                            lstKey.Add(key);
                        }

                        Debug.Log("Exchange rates updated!");
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse exchange rates.");
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error parsing JSON: {ex.Message}");
                    return false;
                }
            }
            else
            {
                Debug.LogError($"Error fetching exchange rates: {request.error}");
                return false;
            }
        }
    }

    // Chuyển đổi tiền tệ
    public float Convert(string fromCurrency, string toCurrency, float amount)
    {
        if (exchangeRates.ContainsKey(fromCurrency) && exchangeRates.ContainsKey(toCurrency))
        {
            float fromRate = exchangeRates[fromCurrency];
            float toRate = exchangeRates[toCurrency];
            return (amount / fromRate) * toRate;
        }
        else
        {
            Debug.LogWarning($"Currency {fromCurrency} or {toCurrency} not found!");
            return -1f;
        }
    }
}

// Lớp để parse JSON gốc
[System.Serializable]
public class RawResponse
{
    public string @base; // "base" trong JSON
    public List<Rate> rates; // Đổi sang List để hỗ trợ JSON parsing

    [System.Serializable]
    public class Rate
    {
        public string currency;
        public float value;
    }
}
