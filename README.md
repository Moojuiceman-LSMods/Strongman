# Strongman

This is a mod for the game Landlord's Super by Minskworks. You can get it here: https://store.steampowered.com/app/1127840/Landlords_Super/

#### What it does

Allows you to increase the amount of multi-stack items (bricks/shingles/tarred roof planks) you can hold, and the amount of weight you can carry without losing your ability to sprint and jump, or losing hygiene when picking/placing heavy items.

#### Save file backup

I try to not affect saves with my mods, but taking backups is recommended. This is also how you can share save files for deubgging.

* From the main menu of the game, click "Save Data" to open the save folder
* Make a copy of the "LandlordsSuper" folder, either to another location or as a new folder name in the same place
* If sharing the save, zip the folder for easier transfer

#### Installation (simple)

* Download MeblIkea's mod manager https://github.com/MeblIkea/Landlords-Super-Mod-Manager and follow the setup instructions

#### Installation (manual)

* Download BepInEx 5 64-bit. Here is a direct link to version 5.4.19 which works with the game https://github.com/BepInEx/BepInEx/releases/download/v5.4.19/BepInEx_x64_5.4.19.0.zip
* Unzip it into the game folder, next to LandlordsSuper.exe. Find it via Steam by right clicking the game > Manage > Browse local files, or by clicking "Streaming Assets" from the game's main menu then going up a folder
* Start the game once so BepInEx creates the necessary folders
* Grab the latest .dll from the releases page and put it in the BepInEx\plugins folder
 
#### Configuration

Mod options are stored in BepInEx\config\<Modname>.cfg

* Max Shingles - Max shingles/tarred roof planks that can be carried. Vanilla game value is 12
* Max Bricks - Max bricks that can be carried. Vanilla game value is 11
* Weight Limit - Weight that can be lifted and still sprint/jump. Vanilla game value is 10
