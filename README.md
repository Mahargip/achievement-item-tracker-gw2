# Achievement Item Tracker (Blish HUD Module)

A **Guild Wars 2** Blish HUD overlay module that allows you to search any achievement and view the complete required item checklist with live totals of your owned items across all storage locations.

---

## ✨ Features

- **Instant Offline Search Index**:
  - Pre-indexed database of all **8,357** Guild Wars 2 achievements (`ref/achievements.json`).
  - Search by name in 0ms with instant substring/word filtering.
  - Option to filter only collection/item-based achievements.

- **Cross-Account Inventory Aggregation**:
  - Automatically queries the official Guild Wars 2 API via Blish HUD's `Gw2ApiManager`.
  - Scans and sums item counts across:
    - **Active & Alt Character Bags**
    - **Account Bank**
    - **Material Storage**
    - **Shared Inventory Slots**
  - Displays owned items vs. required items (e.g. `[ 0 / 5 ]`, `[ 3 / 5 ]`, `[ 5 / 5 ✓ ]`).

- **Rich Tooltips & Item Breakdown**:
  - Hover over any item row to see where each copy is stored (e.g. *"In Character Bags: 2, In Bank: 3, In Material Storage: 5"*).
  - Item names styled with official Guild Wars 2 rarity colors (Exotic, Ascended, Rare, etc.).
  - Shows achievement bit completion status if checked off on your account.

- **In-Game Floating Pin Widget**:
  - Click **"📌 Pin to Screen"** on any achievement to pop out a compact, movable HUD overlay.
  - Keeps your item checklist on screen while playing without blocking your view.
  - Persists your pinned achievement across game restarts.

- **Corner Icon & Context Menu**:
  - Quick-access corner icon in the Blish HUD top menu.
  - Left-click toggles the main window.
  - Right-click menu for quick actions:
    - *Open Tracker Window*
    - *Sync Inventory Now*
    - *Toggle Screen Pin Widget*
  - Configurable hotkey in module settings.

---

## 🛠️ Project Structure

```
AchievementItemTracker/
├── AchievementItemTracker.sln         # Visual Studio Solution
├── AchievementItemTracker.csproj      # SDK-style .NET 4.8 project with auto .bhm packaging
├── AchievementItemTrackerModule.cs    # Blish HUD module lifecycle entry point
├── manifest.json                      # Blish HUD metadata & GW2 API permissions
├── Models/
│   ├── AchievementData.cs             # Index and bit data models
│   ├── AchievementItemRequirement.cs  # Item count, rarity color, and completion logic
│   └── AccountInventorySnapshot.cs    # Storage snapshot (Bags, Bank, Materials, Shared)
├── Services/
│   ├── AchievementIndexService.cs     # In-memory search across 8,000+ achievements
│   └── Gw2DataService.cs              # Gw2Sharp API client & inventory aggregator
├── UI/
│   ├── Controls/
│   │   ├── ItemRequirementRow.cs      # Item row with icon, name, count badge & tooltip
│   │   └── SearchResultRow.cs         # Clickable achievement result entry
│   └── Views/
│       ├── TrackerWindow.cs           # Main search & item breakdown window
│       └── PinnedTrackerWidget.cs     # Compact floating HUD overlay
└── ref/
    ├── achievements.json              # Prebuilt complete GW2 achievement index
    ├── icon.png                       # Module corner icon
    └── icon_hover.png                 # Corner icon hover state
```

---

## 🚀 How to Build & Test

### Automatic Deployment
Whenever you build the project in Visual Studio (`Ctrl + Shift + B`) or via the command line:
```shell
dotnet build
```
The custom MSBuild target will automatically:
1. Compile the C# binaries.
2. Package the `.bhm` archive with `manifest.json` and assets.
3. Deploy `AchievementItemTracker.bhm` directly to:
   `%USERPROFILE%\Documents\Guild Wars 2\addons\blishhud\modules\`

### Enabling in Blish HUD
1. Start **Guild Wars 2** and **Blish HUD**.
2. Open the Blish HUD settings window (helmet icon).
3. Navigate to **Manage Modules**.
4. Find **Achievement Item Tracker** in the list and toggle **Enable Module**.
5. Grant API permissions (*Inventories*, *Characters*, *Progression*).
6. Click the new **Achievement Item Tracker** icon in the top corner bar to search!
