# 🏆 Achievement Item Tracker (Blish HUD Module)

[![Guild Wars 2](https://img.shields.io/badge/Guild%20Wars%202-Addon-red.svg)](https://www.guildwars2.com/)
[![Blish HUD](https://img.shields.io/badge/Blish%20HUD-%3E%3D%201.2.0-blue.svg)](https://blishhud.com/)
[![GitHub Release](https://img.shields.io/github/v/release/Mahargip/achievement-item-tracker-gw2?color=brightgreen&label=Latest%20Release)](https://github.com/Mahargip/achievement-item-tracker-gw2/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Achievement Item Tracker** is an in-game overlay module for **[Blish HUD](https://blishhud.com/)** in **Guild Wars 2**. It allows you to search any achievement and view the complete required item checklist with live totals of your owned items aggregated across all your account's storage locations.

---

## 📖 Module Overview

When working on achievements or collections in Guild Wars 2 (such as *Legendary Weapons*, *Specialization Collections*, *Skins*, *Eater/Convergence achievements*, etc.), players frequently run into the same challenges:
- *Which items are still missing to complete this collection?*
- *Have I already stashed this item on an alt character, in my bank, or in material storage?*
- *How many copies do I already own versus how many are required?*

**Achievement Item Tracker** solves these problems by:
1. Providing an instant, local offline index of all Guild Wars 2 achievements.
2. Securely querying the official Guild Wars 2 API via Blish HUD's `Gw2ApiManager`.
3. Automatically aggregating item counts across every storage location on your account in real-time.
4. Providing a compact, draggable floating screen pin widget so you can keep track of required items without obstructing gameplay.

---

## ✨ Features

- **🔍 Instant Offline Search Index**
  - Pre-indexed local database of over **8,300+** Guild Wars 2 achievements (`ref/achievements.json`).
  - Search by name with 0ms latency and instant substring/word filtering.
  - Quick filter toggle to show only collection- and item-based achievements.

- **🎒 Cross-Account Inventory Aggregation**
  - Automatically queries and tallies item counts across:
    - 🎒 **Active & Alt Character Bags**
    - 🏦 **Account Bank**
    - 📦 **Material Storage**
    - 💼 **Shared Inventory Slots**
  - Displays real-time progress for each requirement: `[ 0 / 5 ]`, `[ 3 / 5 ]`, up to `[ 5 / 5 ✓ ]`.

- **🎨 Official Rarity Colors & Completion Status**
  - Item names styled with official Guild Wars 2 rarity colors (Junk, Basic, Fine, Masterwork, Rare, Exotic, Ascended, Legendary).
  - A green checkmark (**✓**) automatically indicates requirements or collection bits already completed on your account.

- **💡 Rich Storage Location Tooltips**
  - Hover over any item requirement row to see an exact breakdown of where each copy is stored (e.g. *"In Character Bags: 2, In Bank: 3, In Material Storage: 5"*).

- **📌 In-Game Floating Pin Widget**
  - Click **"📌 Pin to Screen"** on any achievement to pop out a compact, movable HUD overlay.
  - Keeps your item checklist visible while exploring Tyria without keeping the main search window open.
  - Pinned achievement state persists across game and Blish HUD restarts.

- **⚡ Corner Icon & Context Menu**
  - Dedicated icon in the Blish HUD top corner bar:
    - **Left-Click**: Toggle the main tracker search window.
    - **Right-Click**: Quick context menu (*Open Tracker Window*, *Sync Inventory Now*, *Toggle Screen Pin Widget*).
  - Configurable hotkey/keybind in Blish HUD module settings.

---

## 📥 Download Module

The compiled, ready-to-use `.bhm` module file is available on the GitHub **Releases** page:

👉 **[Download the Latest Achievement Item Tracker (.bhm) from Releases](https://github.com/Mahargip/achievement-item-tracker-gw2/releases)**

> Download the file with the **`.bhm`** extension (e.g., `AchievementItemTracker.bhm`) under the latest release.

---

## 📦 How to Install / Add Module in Blish HUD

Follow these simple steps to install and enable the module:

### Step 1: Download the `.bhm` File
Download **`AchievementItemTracker.bhm`** from the **[Releases](https://github.com/Mahargip/achievement-item-tracker-gw2/releases)** page.

### Step 2: Open the Blish HUD Modules Directory
Open the Blish HUD `modules` folder on your computer using either method below:

- **Automatic Method (Recommended):**
  1. Launch **Blish HUD**.
  2. Open the settings window by clicking the **Blish HUD Helmet icon** in the top-left of the screen.
  3. Navigate to the **Manage Modules** tab.
  4. Click the **Open Modules Folder** button at the top.
- **Manual Method:**
  Press `Win + R`, paste the following path, and hit **Enter**:
  ```text
  %USERPROFILE%\Documents\Guild Wars 2\addons\blishhud\modules
  ```

### Step 3: Copy the Module File
Copy the downloaded `AchievementItemTracker.bhm` file and paste it into the `modules` folder.

### Step 4: Enable the Module in Blish HUD
1. Switch back to Blish HUD (or restart Blish HUD if it was already running).
2. Go to **Manage Modules** (Helmet icon).
3. Find **Achievement Item Tracker** in the module list.
4. Toggle the switch to **Enable Module**.

### Step 5: Grant Required API Permissions
For the tracker to read your inventory items and achievement progress, ensure the following API permissions are granted:
- **`account`**: Validates connection and account information.
- **`inventories`**: Reads Bank, Material Storage, and Shared Inventory items.
- **`characters`**: Reads bag contents across all your characters.
- **`progression`**: Checks completed achievements and collection bit progress.

> [!NOTE]
> Make sure the Guild Wars 2 API Key configured in your Blish HUD settings has these permissions enabled at [account.arena.net/applications](https://account.arena.net/applications).

---

## 🎮 How to Use

1. **Open the Tracker Window**:
   - Click the **Achievement Item Tracker** icon in the Blish HUD top corner bar, or press your configured hotkey.
2. **Search for an Achievement**:
   - Type the name of any achievement in the search bar (e.g., *Ambrite Weapon Collection*, *Luminescent Armor*, *Bifrost III*).
3. **Inspect Required Items**:
   - Click any search result to view all required items alongside your current owned counts.
   - Hover over an item row to see which characters or storage locations hold that item.
4. **Pin to Screen**:
   - Click **📌 Pin to Screen** in the top-right corner to open the floating HUD widget.
   - Drag the widget to your desired position on screen.
5. **Sync Inventory**:
   - Inventory data automatically refreshes in the background every 5 minutes.
   - You can trigger an instant manual refresh anytime by right-clicking the corner icon and selecting **Sync Inventory Now**.

---

## 🛠️ Building from Source (For Developers)

If you wish to build or contribute to the module:

1. Clone this repository:
   ```shell
   git clone https://github.com/Mahargip/achievement-item-tracker-gw2.git
   ```
2. Build the solution using Visual Studio 2022 or via CLI:
   ```shell
   dotnet build -c Release
   ```
3. The custom MSBuild packaging target will automatically package the `.bhm` archive and deploy it directly to your local Blish HUD modules directory.

---

## 📜 License & Attribution

- This project is licensed under the **[MIT License](LICENSE)**.
- Guild Wars 2 and all associated assets are registered trademarks of **ArenaNet LLC** and **NCSOFT**.
- Built with **[Blish HUD](https://blishhud.com/)** and **[Gw2Sharp](https://github.com/Archomeda/Gw2Sharp)**.
