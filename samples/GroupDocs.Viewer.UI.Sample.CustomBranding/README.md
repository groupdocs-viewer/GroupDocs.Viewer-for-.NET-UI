# Custom Branding Sample

This sample demonstrates how to white-label GroupDocs.Viewer.UI with custom colors, logo, and application title using the built-in branding API.

## What This Sample Shows

- Replacing the default logo image and text with custom SVG files via `SetLogoImage()` and `SetLogoText()`
- Setting a custom page title via `UITitle`
- Overriding the color scheme using CSS variables via `AddCustomStylesheet()`
- Replacing the header application name text via CSS `::after` pseudo-element
- Hiding the init-screen branding (logo and "Powered by" links)

## Prerequisites

- .NET 6.0 SDK or later

## Running

```bash
cd samples/GroupDocs.Viewer.UI.Sample.CustomBranding
dotnet run
```

Open your browser at `https://localhost:5001` (or the URL shown in the console output).

## Configuration

### Logo Replacement

Custom SVG logos are placed in the `./Logos` directory and configured in `Program.cs`:

```csharp
options.SetLogoImage("./Logos/logo-image.svg");  // Replaces the icon logo
options.SetLogoText("./Logos/logo-text.svg");     // Replaces the text logo
```

To hide a logo instead of replacing it, use `HideLogoImage()` or `HideLogoText()`.

### Page Title

```csharp
options.UITitle = "Acme Corp Document Viewer";
```

### Custom Stylesheet

The `./Styles/custom-branding.css` file overrides CSS variables to change the color scheme:

```css
:root {
    --c-bg-brand: #0d9488;           /* Primary brand background */
    --c-bg-brand-hover: #0f766e;     /* Brand hover state */
    --c-text-brand: #0d9488;         /* Brand text color */
    /* ... more variables */
}
```

The stylesheet also demonstrates replacing the header app name text and hiding init-screen branding elements.

## Project Structure

```
├── Program.cs                     # App setup with branding configuration
├── Styles/
│   └── custom-branding.css        # CSS variable overrides and text replacement
├── Logos/
│   ├── logo-image.svg             # Custom icon logo (teal square with "A")
│   └── logo-text.svg              # Custom text logo ("Acme corp")
└── Files/                         # Documents to view
```
