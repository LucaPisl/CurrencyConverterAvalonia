# Currency Converter

A cross-platform currency conversion application built with Avalonia UI and .NET 8.

## Description

Currency Converter is a desktop application that allows users to convert between different currencies using up-to-date exchange rates. The application supports historical conversion rates from March 2, 2024 onwards and allows converting from one currency to multiple currencies simultaneously.

## Features

- Convert from one currency to multiple currencies at once
- Access historical conversion rates (available from 2024-03-02 onwards)
- Simple and intuitive user interface
- Cross-platform support (Windows, Linux, macOS)

## Screenshots

![Screenshot1](https://github.com/LucaPisl/CurrencyConverterAvalonia/blob/master/CurrencyConverterAvaloniaProject/Screenshots/CurrencyConverterSS1.png?raw=true)

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Compatible with Windows, Linux, and macOS

### Installation

#### From Source

1. Clone the repository:
   ```bash
   git clone https://github.com/LucaPisl/CurrencyConverterAvaloniaProject.git
   ```

2. Navigate to the project directory:
   ```bash
   cd CurrencyConverterAvaloniaProject
   ```

3. Build and run the application:
   ```bash
   dotnet build
   dotnet run --project CurrencyConverterAvaloniaProject
   ```

## Usage

1. Select the source currency from the left panel
2. Select one or more target currencies from the middle panel
3. Enter the desired date for conversion in the format yyyy-MM-dd
4. Click the Convert button to see the results in the right panel
5. Use the Save button to export your conversion results

## Technologies Used

- [.NET 8](https://dotnet.microsoft.com/en-us/)
- [Avalonia UI](https://avaloniaui.net/) - Cross-platform .NET UI framework

## Project Structure

- UI components defined in XAML (MainWindow.axaml)
- Currency conversion logic in currencyConsole.cs
- Cross-platform application setup in Program.cs and App.axaml.cs

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. This was more of a quick practice project, but it can always be improved!

## License

This project is licensed under version 3 of the GNU General Public License - see the LICENSE file for details.

## Author

Made in Europe, Romania by Luca Pîslaru

---
