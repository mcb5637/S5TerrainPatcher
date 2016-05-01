TerrainPatcher v0.3

Anwendung:
  1. Schritt:
    Kopiere die Map welche eine Änderung erfahren soll und gestalte den zu verändernden Bereich(e) im Editor neu.
    Platziere 2 Skript-Entitäten auf der Map die den Bereich in einem Rechteck einschließen. (Grid einschalten hilft)
    Benenne die Entitäten nach diesem Schema: TP_NameDesBereichs/a und TP_NameDesBereichs/b
    Wobei NameDesBereichs ein beliebiger Name ist und a bzw. b den jeweiligen Eckpunkt definieren.
    In einer Map sind auch mehrere Bereiche auf einmal möglich. Speichere die Map nun ab.
  
  2. Schritt: 
    Betätige nun den Button "Comfort-Skript abspeichern" um das benötigten Comfort-Skript zu bekommen.
    Es kann entweder die erhaltene Lua Datei eingebunden oder einfach nur die Funktionen irgendwo ins Skript deiner
    URSPRÜNGLICHEN Map kopiert werden.
   
  3. Schritt:
    Lass nun die VERÄNDERTE Map über das Tool einlesen und wähle den von dir gewünschten Bereich aus der resultierenden Liste.
    Ergebnis ist ein Lua-File an dem es KEINE Veränderungen vorzunehmen gibt.
    Dieses Lua-File muss nun in die URSPRÜNGLICHE Map geladen werden. (bbaTool, S5Tools)
  
  4. Schritt:
    Um die Veränderung in der Map herbeizuführen muss:
      1. In der FMA alle Bereiche initialisiert werden. Dies geschieht folgendermaßen:
        InitTerrainPatching({"namedesbereichs.lua", "namedeszweitenbereichs.lua"}, luaPath)
        Argument 1: Ein Table mit allen Lua Files die Patch-Bereiche enthalten.
        Argument 2: Optional, der Pfad aus dem die Lua Files ausgelesen werden (standard: data\maps\externalmap)
      
      2. An beliebiger Stelle diese Funktion aufgerufen werden:
        PatchRegion("NameDesBereichs")
        Argument 1: Der Name welcher über die Script-Entitäten vergeben wurde.


Usage:
  Step 1:
    Create a copy of the complete map and reshape the region(s) you want to patch in the new file.
    Place two ScriptEntities on the map to define the rectangular region which will be patched. Turn on the grid!
    Name the two entities according to this format: TP_NameOfYourRegion/a and TP_NameOfYourRegion/b
    NameOfYourRegion is an arbitrary identifier and a and b is used to distinguish between the edges.
    You can define multiple regions on a single map. Finally, save the map.

  Step 2:
    Press the "Comfort-Skript abspeichern" button to save the neccessary comfort functions for your script. 
    You can either integrate the lua file or copy the functions into the mapscript of your original map.
  
  Step 3:
    Open the reshaped map using the tool and load with "Einlesen". Then choose "Speichern" for each of the regions to 
    generate a lua file with the changes. Do not make changes to this file.
    Integrate this file into your original map. (using S5Tools or bbaTool)
    
  Step 4:
    To patch a region, you have to:
      1. Call this function in the FirstMapAction at the very beginning:
        InitTerrainPatching({"nameofyourregion.lua", "nameofyoursecondregion.lua"}, luaPath)
        Argument 1: A table containing all filenames for your regions
        Argument 2: optional, sets the path to load lua files during debugging
    
      2. Call this function to do the patching:
        PatchRegion("NameOfYourRegion")
        Argument 1: The region identifier, as defined by the ScriptEntitites
      
    

CHANGELOG:
  v0.3: 
    - Added Scale and AmbientSounds
    - Error checking
  v0.2: Support for Windows XP 32Bit
  v0.1: Initial release