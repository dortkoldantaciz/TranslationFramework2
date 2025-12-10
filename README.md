# TranslationFramework2

**Translation Framework 2** is an application developed to make it easier to translate different games.

## How It Works

When creating a new translation, the application asks the user to select the game they want to translate and the folder where it is installed.  
After that, the application automatically finds and extracts (if necessary) the files that contain text, so the user can focus only on translating.

When the user finishes the translation, the application rebuilds the files.

---

## Supported Games

### Yakuza 0
- Text translation
- Texture replacement
- Font replacement
- Patch to use 1-byte encoding instead of Shift-JIS

### Yakuza 0 Director’s Cut (BETA)
- Text translation *(cutscenes cannot be opened yet)*
- Texture replacement
- Font replacement *(only from `.dds` files)*
- Patch to use 1-byte encoding instead of Shift-JIS

### Yakuza Kiwami
- Text translation
- Texture replacement
- Font replacement
- Patch to use 1-byte encoding instead of Shift-JIS

### Ryū ga Gotoku Ishin! (PS3)
- Text translation *(partial support)*

### Shining Resonance Refrain
- Text translation
- Texture replacement
- Font replacement

### The MISSING: J.J. Macfield and the Island of Memories
- Text translation
- Texture replacement
- Font replacement

### Phoenix Wright: Ace Attorney Trilogy
- Text translation
- Script editing *(not tested)*
- Texture replacement *(`.sprite` files are not modified, so images are cut)*

> **NOTE:** Does not work correctly. Do not use until further notice.

### The Legend of Heroes: Trails in the Sky
- Text translation
- Texture replacement
- Font replacement

### UnderRail
- Text translation

> **NOTE:** To make it work, you must copy the following files into the `plugins` folder:  
> `underrail.exe`, `GalaxyCSharp.dll`, `sfmlnet-audio-2.dll`, `sfmlnet-window-2.dll`,  
> `Microsoft.Xna.Framework.dll`, `Microsoft.Xna.Framework.Graphics.dll`

### AI - The Somnium Files
- Text translation
- Texture replacement

### Disco Elysium
- Text translation
- Texture replacement

### Hardcore Mecha
- Text translation
- Texture replacement

### Love Esquire
- Text translation

> **NOTE:** To make it work, you must modify the `vntext.sq` and `Assembly-CSharp.dll` files beforehand.

### NightCry
- Text translation
- Texture replacement
- Font replacement

---

## Credits

- **Malaquito** and **Hazardous** from ClanDLAN for daring to make translations using my tools xD
- **Rick Gibbed** for his tool to extract Yakuza files
- **Clarence1996**, **andonovmarko**, and **M-1618** for their game icons
- **flamethrower** for the Falcom compression algorithm
- **Zhenjian Yang** for his research on *The Legend of Heroes: Trails in the Sky* scripts
- **DragonZH** for the UnityEX tool
- **0xd4d** for dnlib and dnSpy — they are amazing!!!
- **Tradusquare** and all its members
- And everyone else I forgot to mention
