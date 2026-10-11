# Validation

Validated on macOS with .NET SDK 9.0.102 and MAUI 9.0.14.

- Mac Catalyst arm64 build: **0 warnings, 0 errors**.
- Core regression executable: **17 checks passed** against a temporary SQLite database.
- Native welcome/menu layout inspected at **390×800**, **1200×840**, and **1920×1080** window dimensions. These are native point-sized windows, not stretched screenshots or a claim of testing every external monitor.
- Narrow layout: stacked welcome photo, readable name field, full-width CTA, single-column menu, scrollable page/cart, visible checkout fields/buttons.
- Large layout: centered, bounded content; capped headline size; native Mac text sizing; full button backgrounds with no compact gray bezel.
- Native ordering checks: entree selection, large combo at the existing 10% discount, appetizer addition, and selected cart deletion preserving the other item.
- Native checkout checks: 7% tax, decimal tip, negative-tip validation, clearing a tip, demo confirmation, clearing the order, returning to welcome.
- Native administration checks: category filtering, loading the selected item's name/price for editing, cancel returning to dashboard, and new-item navigation resetting the form.

The UI checks used `EAGLESNEST_DATA_DIR` so they did not alter the installed menu. Android, iOS devices, Windows, and physical transitions between displays with different DPI settings were not exercised here.
