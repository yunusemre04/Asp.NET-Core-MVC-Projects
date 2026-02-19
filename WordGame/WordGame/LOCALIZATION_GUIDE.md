# Localization Guide for WordGame

## Overview
The WordGame application now supports multiple languages using ASP.NET Core's built-in localization with IViewLocalizer.

## Supported Languages
- English (en-US) - Default
- Turkish (tr-TR)

## How It Works

### 1. Configuration
Localization is configured in [Program.cs](Program.cs):
- Supported cultures: en-US, tr-TR
- Default culture: en-US
- Resources path: Resources folder
- View localization format: Suffix (e.g., Index.en-US.resx)

### 2. IViewLocalizer Injection
The `Localizer` is injected globally in [_ViewImports.cshtml](Views/_ViewImports.cshtml):
```csharp
@inject IViewLocalizer Localizer
```

### 3. Using Localization in Views

#### Basic Usage
Replace hardcoded text with localized strings:
```csharp
// Before
<h1>Welcome</h1>

// After
<h1>@Localizer["Welcome"]</h1>
```

#### With Parameters
Use string.Format for dynamic content:
```csharp
<p>@string.Format(Localizer["WelcomeGreeting"], userName)</p>
```

### 4. Resource Files Structure
Resource files are located in the `Resources/Views/` folder and follow this naming convention:
```
Resources/
  Views/
    Shared/
      _Layout.en-US.resx  (English)
      _Layout.tr-TR.resx  (Turkish)
    Home/
      Index.en-US.resx    (English)
      Index.tr-TR.resx    (Turkish)
    Auth/
      Login.en-US.resx    (English)
      Login.tr-TR.resx    (Turkish)
```

Each .resx file contains key-value pairs:
```xml
<data name="Welcome" xml:space="preserve">
  <value>Welcome to WordGame</value>
</data>
```

### 5. Language Switcher
A language switcher is available in the navbar that allows users to switch between languages:
- Click the language icon (🌐) in the navigation bar
- Select "English" or "Türkçe"
- The language preference is persisted via query strings

## Adding Localization to a New View

### Step 1: Create Resource Files
Create two resource files in `Resources/Views/[ControllerName]/`:
- `ViewName.en-US.resx` (English)
- `ViewName.tr-TR.resx` (Turkish)

Example for `Views/Word/Add.cshtml`:
- `Resources/Views/Word/Add.en-US.resx`
- `Resources/Views/Word/Add.tr-TR.resx`

### Step 2: Add Localized Strings
Add your key-value pairs to both resource files:

**Add.en-US.resx:**
```xml
<data name="PageTitle" xml:space="preserve">
  <value>Add New Word</value>
</data>
<data name="WordLabel" xml:space="preserve">
  <value>Word</value>
</data>
```

**Add.tr-TR.resx:**
```xml
<data name="PageTitle" xml:space="preserve">
  <value>Yeni Kelime Ekle</value>
</data>
<data name="WordLabel" xml:space="preserve">
  <value>Kelime</value>
</data>
```

### Step 3: Update the View
Replace hardcoded text with `@Localizer["KeyName"]`:
```csharp
<h2>@Localizer["PageTitle"]</h2>
<label>@Localizer["WordLabel"]</label>
```

## Example Views Already Localized
- [Views/Shared/_Layout.cshtml](Views/Shared/_Layout.cshtml) - Navigation menu
- [Views/Home/Index.cshtml](Views/Home/Index.cshtml) - Home page

## Additional Resource Files Created
Example resource files have been created for:
- Auth/Login view (Login.en-US.resx, Login.tr-TR.resx)

You can use these as templates for localizing other views.

## Best Practices
1. Use descriptive key names (e.g., `WelcomeTitle` instead of `Text1`)
2. Group related translations in the same resource file
3. Keep keys consistent across all language files
4. Use string.Format for dynamic content with parameters
5. Test both languages after adding new translations

## Testing
To test the localization:
1. Run the application
2. Click the language icon in the navbar
3. Switch between English and Turkish
4. Verify all text is properly translated

## Notes
- The default language is English (en-US)
- Language preference persists while navigating the site
- All views must have resource files for both supported languages
