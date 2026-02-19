# 🎮 WordGame - Interactive Language Learning Platform

A comprehensive ASP.NET Core MVC web application designed to help users learn and practice English vocabulary through engaging games, quizzes, and mnemonic techniques.

## 📋 Overview

WordGame is an interactive vocabulary learning platform that combines multiple learning methods to enhance language acquisition. Users can add words, create mnemonics, take quizzes, play word puzzles, and track their progress - all in a bilingual interface supporting both English and Turkish.

## ✨ Features

### 🔐 User Management
- **User Registration & Authentication**: Secure account creation and login system
- **Password Management**: Change password and forgot password functionality
- **Session Management**: Persistent user sessions across the application

### 📚 Word Management
- **Add Words**: Create word entries with English/Turkish translations and images
- **Sample Sentences**: Add up to 3 example sentences for each word
- **Word List**: View all your saved words in an elegant card layout
- **Word Details**: Detailed view of words with all associated information
- **Delete Words**: Remove words and associated learning progress

### 🧠 Mnemonic System
- **Memory Pegs**: Create custom mnemonics with images and notes
- **Visual Learning**: Upload mnemonic images to enhance memory retention

### 📝 Quiz System
- **Customizable Quizzes**: Select number of questions (5, 10, 15, or 20)
- **Progress Tracking**: Automatic tracking of completed words
- **Detailed Results**: Review answers with correct/incorrect indicators
- **Performance Feedback**: Get personalized feedback based on quiz scores

### 🎯 Puzzle Game
- **Word Puzzle**: Wordle-style letter guessing game
- **Visual Feedback**: Color-coded hints for letter positions
- **Difficulty Levels**: Words filtered by length (3-8 letters)
- **Score Tracking**: Monitor your puzzle-solving performance

### 📊 Reports & Analytics
- **Learning Progress**: View statistics on learned vs. unlearned words
- **Daily Word Limit**: Track words learned per day
- **User Statistics**: Comprehensive overview of your learning journey

### ⚙️ Settings
- **Profile Management**: Update personal information
- **Daily Word Limits**: Set custom learning goals
- **Preferences**: Customize your learning experience

### 🌐 Localization
- **Bilingual Support**: Full English and Turkish language support
- **Auto Language Detection**: Automatically detects browser language on first visit
- **Language Persistence**: Saves language preference via cookies
- **Easy Language Switching**: Toggle between languages with one click

## 🛠️ Technologies Used

### Backend
- **ASP.NET Core MVC** (.NET 8.0)
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Relational database management
- **Identity Framework** - User authentication and authorization

### Frontend
- **Razor Pages** - Server-side rendering
- **Bootstrap 5** - Responsive UI framework
- **Font Awesome** - Icon library
- **JavaScript** - Client-side interactivity

### Localization
- **IViewLocalizer** - View localization interface
- **Resource Files (.resx)** - Bilingual resource management
- **RequestLocalization Middleware** - Language detection and switching

### Language Switching

Click the language switcher in the navigation bar to toggle between English and Turkish. Your preference will be saved automatically.

## 📁 Project Structure

```
WordGame/
├── Controllers/          # MVC Controllers
│   ├── AuthController.cs
│   ├── HomeController.cs
│   ├── WordController.cs
│   ├── QuizController.cs
│   ├── PuzzleController.cs
│   ├── MnemonicController.cs
│   ├── ReportController.cs
│   └── SettingsController.cs
├── Models/              # Data models and ViewModels
│   ├── Entities/
│   └── ViewModels/
├── Views/               # Razor views
│   ├── Auth/
│   ├── Word/
│   ├── Quiz/
│   ├── Puzzle/
│   ├── Mnemonic/
│   ├── Report/
│   ├── Settings/
│   └── Shared/
├── Data/                # Database context
├── Resources/           # Localization resource files
│   └── Views/
├── Services/            # Business logic services
├── wwwroot/            # Static files (CSS, JS, images)
└── Migrations/         # EF Core migrations
```

## 🔧 Configuration

### Localization Settings

The application supports English (en-US) and Turkish (tr-TR) with the following configuration in `Program.cs`:

```csharp
var supportedCultures = new[] { "en-US", "tr-TR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
```

### Session Configuration

Session timeout and cookie settings can be modified in `Program.cs`.

## 🙏 Acknowledgments

- Bootstrap for the responsive UI framework
- Font Awesome for the comprehensive icon library
- Microsoft for ASP.NET Core and Entity Framework Core
- The open-source community for inspiration and support

⭐ If you found this project helpful, please consider giving it a star!
