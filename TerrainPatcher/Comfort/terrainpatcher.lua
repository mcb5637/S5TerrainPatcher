
--- terrainpatcher 2.0
-- 
-- Benutzung in 2 Modi:
-- 
-- FreePatching: Die Patchdaten werden jederzeit in einem metatable bereitgehalten und beim neuladen eines savegames erneut geladen.
-- 		terrainPatcher.initFreePatching(_regionNames, _luaPath) Muss aus der FMA aufgerufen werden, mit den Namen aller Regionen.
-- 		terrainPatcher.patchRegion(_name)						Lädt eine Region.
-- 
-- 
-- GameLoadPatch: Die Patchdateien werden nur geladen, wenn sie auch benötigt werden.
-- 					Sind sie in einer S5x gepackt, muss diese geladen sein (automatisch in FMA und Mission_OnSaveGameLoaded)
-- 		terrainPatcher.gameLoadPatchRegion(_name)				Lädt die Regionsdaten, lädt die Region und löscht die Regionsdaten danach.
-- 
-- Benötigt:
-- 	- S5Hook (EntityIterator)
terrainPatcher = {luaPath = "data/maps/externalmap/", useLuac = false, namesuffix = ""}
function terrainPatcher.initFreePatching(_regionNames, _luaPath)
	terrainPatcher.regionNames = _regionNames
	if _luaPath then
		terrainPatcher.luaPath = _luaPath
	end
	terrainPatcher.checkMetatable()
	terrainPatcher.LoadGameOrig = Mission_OnSaveGameLoaded
	Mission_OnSaveGameLoaded = function()
		terrainPatcher.loadRegions()
		terrainPatcher.LoadGameOrig()
	end
	terrainPatcher.loadRegions()
end

function terrainPatcher.loadRegions()
	terrainPatcher.checkMetatable()
	for _, rnam in ipairs(terrainPatcher.regionNames) do
		terrainPatcher.loadRegion(rnam)
	end
end

function terrainPatcher.loadRegion(_name)
	Script.Load(terrainPatcher.luaPath.._name..terrainPatcher.namesuffix..".lua"..(terrainPatcher.useLuac and "c" or ""))
end

function terrainPatcher.checkMetatable()
	if not getmetatable(terrainPatcher) then
		setmetatable(terrainPatcher, {})
	end
end

function terrainPatcher.gameLoadPatchRegion(_name, _off, _subPatchPos)
	terrainPatcher.checkMetatable()
	terrainPatcher.loadRegion(_name)
	terrainPatcher.patchRegion(_name, _off, _subPatchPos)
	getmetatable(terrainPatcher)[_name] = nil
end

function terrainPatcher.patchRegion(_name, _off, _subPatchPos)
	getmetatable(terrainPatcher)[_name](_off, _subPatchPos)
end

function terrainPatcher.removeEntitiesInRegion(posBoundaries, off)
	for id in S5Hook.EntityIterator(Predicate.InRect(posBoundaries.low.X + off.X, posBoundaries.low.Y + off.Y, posBoundaries.high.X + off.X, posBoundaries.high.Y + off.Y)) do
		Logic.DestroyEntity(id)
	end
end

