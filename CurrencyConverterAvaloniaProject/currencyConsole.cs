using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyConverterAvaloniaProject
{
    public abstract class CurrencyConsole
    {
        
        public static async Task<object> ShowAvailableCurrencies(HttpClient client, bool returnList = false)
        {
            List<string> currencyDescriptions = new List<string>();
            string myReturnString = "";
            try
            {
                string url = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies.json";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                Dictionary<string, string> currencies = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

                Console.WriteLine("Available currencies:");
                foreach (var currency in currencies)
                {
                    string formattedCurrency = $"{currency.Key.ToUpper()}: {currency.Value}";
                    currencyDescriptions.Add(formattedCurrency);
                    myReturnString += $"{formattedCurrency}\n";
                    Console.WriteLine(formattedCurrency);
                }

                // Return as list if returnList is true, otherwise return as string
                return returnList ? currencyDescriptions : myReturnString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching currencies: {ex.Message}");
                return ex.Message;
            }
        }

                public static async Task<object> ShowCurrencyRates(HttpClient client, 
            string currencyInput, string? dateInput = null, List<string>? targetCurrencies = null, bool returnList = false)
        {
            List<string> currencyPairs = new List<string>();
            string myReturnString = "";
            try
            {
                dateInput ??= DateTime.Now.ToString("yyyy-MM-dd");
                string url = $"https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@" +
                             $"{dateInput}/v1/currencies/{currencyInput.ToLower()}.json";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDoc = JsonDocument.Parse(jsonString);

                JsonElement root = jsonDoc.RootElement;
                if (root.TryGetProperty(currencyInput.ToLower(), out JsonElement currencyData))
                {
                    if (targetCurrencies == null)
                    {
                        // Collect all pairs if no specific target currencies are provided
                        foreach (JsonProperty pair in currencyData.EnumerateObject())
                        {
                            string formattedPair = $"{pair.Name.ToUpper()}: {pair.Value}";
                            currencyPairs.Add(formattedPair);
                            myReturnString += $"\n{formattedPair}\n";
                        }
                    }
                    else
                    {
                        // Only collect specified target currencies
                        foreach (string targetCurrency in targetCurrencies)
                        {
                            if (currencyData.TryGetProperty(targetCurrency.ToLower(), out JsonElement rate))
                            {
                                string formattedPair = $"{targetCurrency.ToUpper()}: {rate}";
                                currencyPairs.Add(formattedPair);
                                myReturnString += $"\n{formattedPair}\n";
                            }
                            else
                            {
                                Console.WriteLine($"{targetCurrency.ToUpper()}: Rate not available.");
                            }
                        }
                    }
                    
                    // Return as list if returnList is true, otherwise return as string
                    return returnList ? currencyPairs : myReturnString;
                }
                else
                {
                    Console.WriteLine("Currency data not found.");
                    return "Currency data not found.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching currency rates: {ex.Message}");
                return ex.Message;
            }
        }
                
                public static async Task<List<string>> ConvertCurrenciesAsync(HttpClient client, 
            string fromCurrency, List<string> targetCurrencies, DateTime date)
        {
            List<string> conversionResults = new List<string>();
            try
            {
                string formattedDate = date.ToString("yyyy-MM-dd");
                string url = $"https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@{formattedDate}/v1/currencies/{fromCurrency.ToLower()}.json";
                
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDoc = JsonDocument.Parse(jsonString);

                JsonElement root = jsonDoc.RootElement;
                if (root.TryGetProperty(fromCurrency.ToLower(), out JsonElement currencyData))
                {
                    foreach (string targetCurrency in targetCurrencies)
                    {
                        if (currencyData.TryGetProperty(targetCurrency.ToLower(), out JsonElement rate))
                        {
                            string formattedPair = $"{targetCurrency.ToUpper()}: {rate}";
                            conversionResults.Add(formattedPair);
                        }
                        else
                        {
                            // If a rate is not available, show as unavailable
                            conversionResults.Add($"{targetCurrency.ToUpper()}: Rate not available");
                        }
                    }
                }
                else
                {
                    conversionResults.Add("Currency data not found for the specified date.");
                }
            }
            catch (Exception ex)
            {
                conversionResults.Add($"Error fetching conversion rates: {ex.Message}");
            }

            return conversionResults;
        }
    }
}
