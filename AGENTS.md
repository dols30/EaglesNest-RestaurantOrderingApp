# Eagles Nest Restaurant

## Project
This is a native .NET 9 MAUI ordering kiosk, not a website. Keep the MAUI application and SQLite menu storage. The app lives in `backendKiosk/backendKiosk`.

## Design
Use the shared resources in `Resources/Styles`. Navy (#101B2D), orange (#FF9C51), soft white (#F6F7F9), Open Sans. Keep touch targets at least 44 points, descriptive accessible labels, and visible selected/disabled states. Welcome and menu screens must adapt to narrow windows; scroll long content instead of clipping. Keep existing Shell routes and menu prices; combos retain the 10% discount and checkout retains the 7% tax. Photography is illustrative, generated locally, and bundled for offline use.

## Work and validation
- Inspect relevant XAML and code-behind before changing a screen.
- Keep cart IDs unique per instance, copy menu items into orders, and use decimal values for checkout totals.
- Use asynchronous database methods; never block the UI with Task.Wait or Task.Result.
- Build on macOS: `dotnet build backendKiosk/backendKiosk/backendKiosk.csproj -f net9.0-maccatalyst -r maccatalyst-arm64 -p:TargetFrameworks=net9.0-maccatalyst`.
- Run core regression checks: `dotnet run --project tests/EaglesNest.CoreChecks`.
- Check the actual running app for welcome, category selection, combo creation, cart deletion, and checkout; capture portfolio images from the native app.
- Do not claim Android/iOS/Windows validation from a Mac Catalyst build.
- This is a local ordering demo. Do not claim payment processing or kitchen integration.

## Desktop responsiveness
Mac Catalyst must use `UIDeviceFamily=6` for native Mac sizing instead of scaled iPad rendering. Do not introduce Picker, Stepper, or RefreshView on Mac; their UIKit primitives are unsupported in the Mac idiom. Use scrollable selections instead. Cap main content at 1440 points, item/checkout reading areas around 1040, and the headline at 68 points. Never scale an entire page or rasterize text to fit a monitor. Reflow welcome below 780 points, stack the cart below the menu there, and show a single category column below 500 points.
