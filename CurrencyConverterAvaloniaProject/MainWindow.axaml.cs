using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace CurrencyConverterAvaloniaProject
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _webClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            // Call an async initialization method after constructing the window
            InitializeCurrenciesAsync();
            DateCurrencyConversionTextBox.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private async Task InitializeCurrenciesAsync()
        {
            try
            {
                var currenciesResult = await CurrencyConsole.ShowAvailableCurrencies(_webClient, returnList: true);

                if (currenciesResult is List<string> currenciesList)
                {
                    foreach (string currency in currenciesList)
                    {
                        AvailableCurrenciesListBox.Items.Add(currency);
                        ToConvertCurrenciesListBox.Items.Add(currency);
                    }
                }
                else if (currenciesResult is string errorMessage)
                {
                    var box = MessageBoxManager
                        .GetMessageBoxStandard("Error", $"Failed to load currencies: {errorMessage}", ButtonEnum.Ok);
                    await box.ShowAsPopupAsync(this);
                }
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error", $"An error occurred: {ex.Message}", ButtonEnum.Ok);
                await box.ShowAsPopupAsync(this);
            }
        }

        public void CloseButtonClickHandler(object? sender, RoutedEventArgs e)
        {
            _webClient.Dispose();
            Close();
        }

        public void RefreshButtonClickHandler(object? sender, RoutedEventArgs e)
        {
            AvailableCurrenciesListBox.Items.Clear();
            ToConvertCurrenciesListBox.Items.Clear();
            ResultCurrencyTextBox.Clear();
            
            
            
            InitializeCurrenciesAsync();
            try
            {
                if (CalcButton is null)
                {
                    throw new ArgumentNullException("CalcButton", "Id Argument cannot be null");
                }
            }
            catch (ArgumentNullException arErr)
            {
                Console.WriteLine(arErr.Message);
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error!", "You need to input a value!", ButtonEnum.Ok);

                var result = box.ShowAsPopupAsync(this);
            }
            catch (FormatException fEx)
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error!", "You need to input a valid value!", ButtonEnum.Ok);

                var result = box.ShowAsPopupAsync(this);
            }
        }

        private async void ConvertButton_OnClickButtonClickHandler(object? sender, RoutedEventArgs e)
        {
            ResultCurrencyTextBox.Text = string.Empty;
            try
            {
                // Step 1: Retrieve selected currencies
                var selectedCurrencies = ToConvertCurrenciesListBox.SelectedItems
                    .Cast<string>()
                    .Select(item => item.Split(':')[0].Trim())  // Extract the code before ":"
                    .ToList();

                if (!selectedCurrencies.Any())
                {
                    var box = MessageBoxManager
                        .GetMessageBoxStandard("Error", "Please select at least one currency to convert into.", ButtonEnum.Ok);
                    await box.ShowAsPopupAsync(this);
                    return;
                }

                // Step 2: Parse and validate date
                if (!DateTime.TryParseExact(DateCurrencyConversionTextBox.Text, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                {
                    var box = MessageBoxManager
                        .GetMessageBoxStandard("Error", "Invalid date format. Please enter a date in yyyy-MM-dd format.", ButtonEnum.Ok);
                    await box.ShowAsPopupAsync(this);
                    return;
                }

                // Step 3: Perform the conversion
                var fromCurrency = AvailableCurrenciesListBox.SelectedItem?.ToString().Split(':')[0].Trim();
                if (fromCurrency == null)
                {
                    var box = MessageBoxManager
                        .GetMessageBoxStandard("Error", "Please select a currency to convert from.", ButtonEnum.Ok);
                    await box.ShowAsPopupAsync(this);
                    return;
                }

                var conversionResults = await CurrencyConsole.ConvertCurrenciesAsync(_webClient, fromCurrency, selectedCurrencies, date);

                // Step 4: Display results
                ResultCurrencyTextBox.Text = string.Join(Environment.NewLine, conversionResults);
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error", $"An error occurred during conversion: {ex.Message}", ButtonEnum.Ok);
                await box.ShowAsPopupAsync(this);
            }
        }

        private async void SaveButton_OnClickClickHandler(object? sender, RoutedEventArgs e)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this);

            // Start async operation to open the dialog.
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save as File"
            });

            if (file is not null)
            {
                // Open writing stream from the file.
                await using var stream = await file.OpenWriteAsync();
                using var streamWriter = new StreamWriter(stream);
                // Write some content to the file.
                await streamWriter.WriteLineAsync(ResultCurrencyTextBox.Text);
            }
        }
    }
}
