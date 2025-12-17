using PS.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExchangeRates
{
    public string baseCurrency;
    public string date;
    public Dictionary<string, float> rates;
}

public class CurrencyExchange : Singleton<CurrencyExchange>
{
    private const string jsonData = @"
    {
        'base': 'USD',
        'date': '2025-01-08',
        'rates': {
            'USD': 1,
            'AED': 3.67,
            'AFN': 70.99,
            'ALL': 94.59,
            'AMD': 396.63,
            'ANG': 1.79,
            'AOA': 923.25,
            'ARS': 1036.25,
            'AUD': 1.6,
            'AWG': 1.79
            // Add more currencies as needed
        }
    }";

    private ExchangeRates exchangeRates;

    void Start()
    {
        ParseExchangeRates(jsonData);
        PrintExchangeRates();
    }

    // Parse the JSON data into a C# object
    private void ParseExchangeRates(string json)
    {
        exchangeRates = JsonUtility.FromJson<ExchangeRates>(json);
    }

    // Print out the exchange rates for USD
    private void PrintExchangeRates()
    {
        Debug.Log($"Base Currency: {exchangeRates.baseCurrency}, Date: {exchangeRates.date}");
        foreach (var rate in exchangeRates.rates)
        {
            Debug.Log($"{rate.Key}: {rate.Value}");
        }
    }
}
