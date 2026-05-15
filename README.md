# SmartWallet

A personal finance tracking application built with .NET MAUI. Allows users to track income and expenses, view statistics by category and time period, and manage transaction history.

> **Note:** This application was developed and optimized for **iOS** as a mobile application. It can also be run on Android, Windows, and macOS — all features work correctly on every platform, however the UI is not fully adapted for non-iOS layouts. On desktop platforms (Windows, macOS) the interface may not look perfect, as it was designed for a mobile screen size.

## Features

- PIN-based authentication (create and change PIN)
- Add, edit and delete income/expense transactions
- Filter transactions by category, time period, or search text
- Statistics with pie charts grouped by category
- Export and import transaction history as JSON

## Requirements

| Platform | Requirement |
|----------|-------------|
| Windows  | Windows 10 version 10.0.19041.0 or higher, Visual Studio 2022 with MAUI workload |
| macOS    | macOS 15+, Visual Studio for Mac or Rider with MAUI workload |
| Android  | Android emulator or physical device (API 21+), Visual Studio / Rider |
| iOS      | macOS with Xcode 16+, physical iPhone or simulator |

## How to Run

### Windows
1. Install [Visual Studio 2022](https://visualstudio.microsoft.com/) with the **.NET MAUI** workload
2. Clone the repository
3. Open `SmartWallet.sln`
4. Select `net10.0-windows10.0.19041.0` as the target framework
5. Run the project

### macOS
1. Install [Rider](https://www.jetbrains.com/rider/) or Visual Studio for Mac with MAUI support
2. Install Xcode from the App Store
3. Clone the repository
4. Open `SmartWallet.sln`
5. Select `net10.0-maccatalyst` or `net10.0-ios` as the target framework
6. Run the project

### Android (any OS)
1. Install Visual Studio or Rider with the **.NET MAUI** workload
2. Set up an Android emulator (API 21+) via Android SDK Manager
3. Clone the repository
4. Open `SmartWallet.sln`
5. Select `net10.0-android` as the target framework
6. Select your emulator or connected device and run

## First Launch

On first launch, you will be prompted to create a 6-digit PIN. This PIN is required to access the application on subsequent launches.

## Project Structure

```
SmartWallet/
├── Models/
│   ├── Entities/        # Database entity classes (User, Transaction)
│   ├── Interfaces/      # Service interfaces
│   └── Services/        # Business logic and data access
├── ViewModels/          # MVVM ViewModels
├── Views/               # MAUI Pages and XAML
└── Resources/           # Fonts, images, styles
```

## Technologies Used

- .NET 10 MAUI
- SQLite (sqlite-net-pcl)
- CommunityToolkit.Mvvm
- LiveChartsCore (pie charts)
- BCrypt.Net (PIN hashing)
- System.Text.Json (import/export)

## Author

Tomáš Olbert
