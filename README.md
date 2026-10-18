# 🏆 Achievement Item Tracker (Blish HUD Module)

[![Guild Wars 2](https://img.shields.io/badge/Guild%20Wars%202-Addon-red.svg)](https://www.guildwars2.com/)
[![Blish HUD](https://img.shields.io/badge/Blish%20HUD-%3E%3D%201.2.0-blue.svg)](https://blishhud.com/)
[![GitHub Release](https://img.shields.io/github/v/release/Mahargip/achievement-item-tracker-gw2?color=brightgreen&label=Latest%20Release)](https://github.com/Mahargip/achievement-item-tracker-gw2/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Achievement Item Tracker** adalah modul in-game overlay untuk **[Blish HUD](https://blishhud.com/)** di game **Guild Wars 2**. Modul ini dirancang khusus untuk mempermudah pemain dalam mencari achievement dan melacak seluruh kebutuhan item koleksi secara instan dengan menghitung total item yang kamu miliki di seluruh tempat penyimpanan akunmu.

---

## 📖 Penjelasan Modul (Overview)

Saat mengerjakan achievement atau koleksi di Guild Wars 2 (seperti *Legendary Weapons*, *Specialization Collections*, *Skins*, *Eater/Convergence achievements*, dsb.), pemain sering kali kesulitan mengetahui:
- Item apa saja yang masih dibutuhkan untuk menyelesaikan achievement tersebut?
- Apakah item tersebut sudah pernah disimpan di bank, material storage, atau tas karakter lain (*alt characters*)?
- Berapa jumlah item yang sudah terkumpul dan berapa kekurangannya?

**Achievement Item Tracker** menjawab masalah tersebut dengan:
1. Mengindeks seluruh achievement Guild Wars 2 secara lokal (offline) agar pencarian berlangsung cepat tanpa jeda.
2. Membaca data API resmi Guild Wars 2 secara aman melalui Blish HUD.
3. Mengagregasi (menjumlahkan) seluruh item yang relevan dari semua lokasi penyimpanan akunmu secara real-time.
4. Menyediakan widget *floating pin* yang dapat ditempel di layar saat bermain tanpa menghalangi pandangan.

---

## ✨ Fitur Utama (Features)

- **🔍 Pencarian Cepat & Instan (Instant Offline Search)**
  - Dilengkapi database lokal berisi lebih dari **8.300+** achievement Guild Wars 2 (`ref/achievements.json`).
  - Pencarian nama achievement tanpa delay (0ms) dengan fitur filter khusus koleksi item.

- **🎒 Cross-Account Inventory Aggregation (Pengecekan Lintas Karakter & Penyimpanan)**
  - Secara otomatis memindai dan menghitung total item yang kamu miliki di:
    - 🎒 **Tas Karakter Aktif maupun Karakter Alt** (*Character Bags*)
    - 🏦 **Bank Akun** (*Account Bank*)
    - 📦 **Penyimpanan Material** (*Material Storage*)
    - 💼 **Shared Inventory Slots**
  - Menampilkan progress yang jelas: `[ 0 / 5 ]`, `[ 3 / 5 ]`, hingga `[ 5 / 5 ✓ ]`.

- **🎨 Indikator Warna Rarity & Status Selesai**
  - Nama item diformat dengan warna rarity resmi Guild Wars 2 (Junk, Basic, Fine, Masterwork, Rare, Exotic, Ascended, Legendary).
  - Tanda centang hijau (**✓**) otomatis muncul jika syarat item/bit achievement telah terpenuhi di akunmu.

- **💡 Tooltip Detail Lokasi Item**
  - Cukup arahkan kursor mouse (*hover*) ke salah satu item untuk melihat rincian di mana saja item tersebut disimpan (contoh: *"In Character Bags: 2, In Bank: 3, In Material Storage: 5"*).

- **📌 In-Game Floating Pin Widget (Sematkan ke Layar)**
  - Klik tombol **"📌 Pin to Screen"** pada achievement mana saja untuk memunculkan overlay mini yang ringkas dan bisa digeser (*draggable*).
  - Memudahkanmu terus memantau daftar kebutuhan item saat menjelajahi Tyria tanpa perlu membuka-tutup jendela utama.
  - Pilihan achievement yang di-pin tetap tersimpan meskipun game atau Blish HUD ditutup.

- **⚡ Corner Icon & Menu Cepat**
  - Ikon khusus di bar atas Blish HUD:
    - **Klik Kiri**: Membuka / menutup jendela pencarian.
    - **Klik Kanan**: Menu pintas untuk *Open Tracker Window*, *Sync Inventory Now*, dan *Toggle Screen Pin Widget*.
  - Dukungan tombol pintas (*Keybind / Hotkey*) yang dapat dikonfigurasi melalui menu pengaturan Blish HUD.

---

## 📥 Download Modul

File modul resmi yang siap pakai (`.bhm`) tersedia di halaman **Releases**:

👉 **[Download Achievement Item Tracker Terbaru di Menu Releases](https://github.com/Mahargip/achievement-item-tracker-gw2/releases)**

> Unduh file dengan ekstensi **`.bhm`** (contoh: `AchievementItemTracker.bhm`) pada rilis versi terbaru.

---

## 📦 Cara Menambahkan / Memasang Modul di Blish HUD

Ikuti langkah-langkah mudah berikut untuk memasang modul ini:

### Langkah 1: Unduh File Modul
Unduh file **`AchievementItemTracker.bhm`** dari halaman **[Releases](https://github.com/Mahargip/achievement-item-tracker-gw2/releases)**.

### Langkah 2: Buka Folder Modules Blish HUD
Buka folder `modules` Blish HUD di komputer Anda. Anda bisa membukanya dengan salah satu cara berikut:

- **Cara Otomatis (Rekomendasi):**
  1. Jalankan **Blish HUD**.
  2. Buka menu pengaturan dengan mengklik ikon **Helm Blish HUD** di kiri atas layar.
  3. Pilih tab **Manage Modules**.
  4. Klik tombol **Open Modules Folder** di bagian atas.
- **Cara Manual:**
  Tekan `Windows + R`, lalu salin dan tempel alamat berikut kemudian tekan **Enter**:
  ```text
  %USERPROFILE%\Documents\Guild Wars 2\addons\blishhud\modules
  ```

### Langkah 3: Salin File Modul
Salin (*copy*) file `AchievementItemTracker.bhm` yang telah diunduh, lalu tempel (*paste*) ke dalam folder `modules` tersebut.

### Langkah 4: Aktifkan Modul di Blish HUD
1. Buka kembali Blish HUD (atau restart Blish HUD jika sedang berjalan).
2. Masuk ke **Manage Modules** (ikon Helm).
3. Cari **Achievement Item Tracker** di daftar modul.
4. Klik tombol toggle switch ke posisi **Enable Module**.

### Langkah 5: Berikan Izin API (API Permissions)
Modul ini memerlukan izin API agar dapat membaca inventaris dan progres achievement akun Anda secara akurat:
- **`account`**: Memvalidasi koneksi akun GW2.
- **`inventories`**: Membaca isi Bank, Material Storage, dan Shared Inventory Slots.
- **`characters`**: Membaca isi tas dari semua karakter Anda.
- **`progression`**: Mengecek status penyelesaian achievement dan koleksi.

> [!NOTE]
> Pastikan API Key Guild Wars 2 yang terdaftar di Blish HUD Anda telah dicentang izin di atas melalui website [account.arena.net/applications](https://account.arena.net/applications).

---

## 🎮 Cara Menggunakan Modul

1. **Membuka Jendela Pencarian**:
   - Klik ikon **Achievement Item Tracker** di pojok bar atas Blish HUD, atau gunakan Hotkey yang sudah diatur di Settings.
2. **Mencari Achievement**:
   - Ketikkan nama achievement pada kotak pencarian (contoh: *Ambrite Weapon Collection*, *Luminescent Armor*, *Bifrost III*, dll.).
3. **Melihat Kebutuhan Item**:
   - Klik salah satu hasil pencarian untuk melihat daftar item yang diperlukan beserta jumlah yang sudah kamu punya.
   - Arahkan kursor ke item untuk melihat lokasi penyimpanan spesifik.
4. **Menyematkan ke Layar (Pin)**:
   - Klik tombol **📌 Pin to Screen** di kanan atas jendela.
   - Pindahkan widget overlay kecil ke posisi layar yang kamu sukai.
5. **Memperbarui Data Inventaris (Sync)**:
   - Data inventaris otomatis diperbarui setiap 5 menit.
   - Anda juga dapat melakukan sinkronisasi manual kapan saja dengan klik kanan pada ikon corner bar -> **Sync Inventory Now**.

---

## 🛠️ Untuk Pengembang (Build from Source)

Jika Anda ingin memodifikasi atau meng-compile modul ini sendiri:

1. Clone repositori ini:
   ```shell
   git clone https://github.com/Mahargip/achievement-item-tracker-gw2.git
   ```
2. Buka solusi menggunakan Visual Studio 2022 atau via terminal:
   ```shell
   dotnet build -c Release
   ```
3. Target MSBuild otomatis mengemas file `.bhm` dan menyalinnya langsung ke folder modul Blish HUD lokal Anda.

---

## 📜 Lisensi & Atribusi

- Modul ini dirilis di bawah lisensi **[MIT License](LICENSE)**.
- Guild Wars 2 dan seluruh aset game terkait merupakan merek dagang terdaftar milik **ArenaNet LLC** & **NCSOFT**.
- Modul ini menggunakan framework **[Blish HUD](https://blishhud.com/)** dan pustaka **[Gw2Sharp](https://github.com/Archomeda/Gw2Sharp)**.
