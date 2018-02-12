using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TerrainPatcher
{
    class MainIO
    {
        private string tmpPath;
        private bool console;
        private TerrainData tdata;
        private EntityList elist;

        public MainIO(bool console)
        {
            this.console = console;
            
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            tmpPath = Path.GetTempPath() + "terrainPatcher-" + Guid.NewGuid().ToString("N").Substring(24);
            Directory.CreateDirectory(tmpPath);
            UnpackHelperFiles();
        }

        private void UnpackHelperFiles()
        {
            Assembly localAssembly = Assembly.GetExecutingAssembly();
            string[] resourceFiles = localAssembly.GetManifestResourceNames();

            try
            {
                foreach (string resourceName in resourceFiles)
                {
                    string[] nameParts = resourceName.Split('.');
                    if (nameParts[1] != "Helper")
                        continue;

                    Stream resourceStream = localAssembly.GetManifestResourceStream(resourceName);

                    string storeFilename = tmpPath + '/'
                        + nameParts[nameParts.Length - 2]
                        + '.' + nameParts[nameParts.Length - 1];

                    using (FileStream fs = new FileStream(storeFilename, FileMode.Create))
                    {
                        resourceStream.CopyTo(fs);
                    }
                }
            }
            catch
            {
                if (console)
                {
                    Console.WriteLine("internal error unpack!");
                    Environment.Exit(1);
                }
                else
                {
                    MessageBox.Show("A problem occured while creating the Helperfiles", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
        }

        public void readMapFile(string f)
        {
            if (!File.Exists(f))
            {
                if (console)
                {
                    Console.WriteLine("mapfile not found!");
                    Environment.Exit(1);
                }
                else
                {
                    MessageBox.Show("Datei nicht gefunden!");
                }
                return;
            }

            try
            {
                File.Copy(f, tmpPath + "/src.s5x", true);

                ProcessStartInfo psi = new ProcessStartInfo(tmpPath + "/bbaTool.exe", "\"" + tmpPath + "/src.s5x\"");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process proc = Process.Start(psi);
                proc.WaitForExit();


                elist = new EntityList(tmpPath + "/src.s5x.unpacked/maps/externalmap/mapdata.xml");
                tdata = new TerrainData(tmpPath + "/src.s5x.unpacked/maps/externalmap/terraindata.bin", tmpPath + "/src.s5x.unpacked/maps/externalmap/mapvertexcolors.bin");

            }
            catch (Exception ex) {
                if (console)
                {
                    Console.WriteLine("Error:\n" + ex.ToString());
                    Environment.Exit(1);
                }
                else
                {
                    MessageBox.Show("Error:\n" + ex.ToString());
                }
            }
        }

        public Dictionary<string, PatchRegion> getRegions()
        {
            return elist.PatchRegions;
        }

        public void createRegionPatchFile(string region, string f, string regonname, bool entities, bool textures, bool heights, bool vertexColors, bool waterTypes, bool waterLevels)
        {
            if (!elist.PatchRegions.ContainsKey(region))
            {
                Console.WriteLine("region not found! " + region);
                Environment.Exit(1);
            }

            PatchRegion pr = elist.PatchRegions[region];

            StringBuilder sb = new StringBuilder();
            Point heightHigh = new Point((int)pr.XHigh / 100, (int)pr.YHigh / 100);
            Point heightLow = new Point((int)pr.XLow / 100, (int)pr.YLow / 100);
            Point quadHigh = new Point((int)pr.XHigh / 400, (int)pr.YHigh / 400);
            Point quadLow = new Point((int)pr.XLow / 400, (int)pr.YLow / 400);
            PointF middlePos = new PointF((pr.XHigh - pr.XLow) / 2 + pr.XLow, (pr.YHigh - pr.YLow) / 2 + pr.YLow);
            float radius = (float)Math.Sqrt(((pr.XHigh - pr.XLow) / 2) * ((pr.XHigh - pr.XLow) / 2) + ((pr.YHigh - pr.YLow) / 2) * ((pr.YHigh - pr.YLow) / 2));

            // region definition
            sb.Append(string.Format(@"getmetatable(terrainPatcher).{12} = function()
local terrainData = {{
    heightBoundaries = {{
        high = {{ X = {0}, Y = {1} }},
        low = {{ X = {2}, Y = {3} }}
    }},
    quadBoundaries = {{
        high = {{ X = {4}, Y = {5} }},
        low = {{ X = {6}, Y = {7} }}
    }},
    posBoundaries = {{
        high = {{ X = {8}, Y = {9} }},
        low = {{ X = {10}, Y = {11} }},
    }},", heightHigh.X, heightHigh.Y, heightLow.X, heightLow.Y, quadHigh.X, quadHigh.Y, quadLow.X, quadLow.Y, pr.XHigh, pr.YHigh, pr.XLow, pr.YLow, regonname));

            // entities
            if (entities)
            {
                sb.Append("\n\t\tnewEntities = {");
                foreach (Entity ent in elist.GetEntitiesInRegion(pr))
                {
                    sb.Append(ent);
                    sb.Append(',');
                }
                sb.Append("},");
            }

            // textures
            if (textures)
            {
                sb.Append("\n\t\ttextures = {");

                XYLoop(quadLow, quadHigh, sb, delegate (int x, int y)
                {
                    sb.Append(tdata.Quads[y, x].TerrainType);
                    sb.Append(',');
                });
                sb.Append("},");
            }

            // water type
            if (waterTypes)
            {
                sb.Append("\n\t\twaterTypes = {");

                XYLoop(quadLow, quadHigh, sb, delegate (int x, int y)
                {
                    sb.Append(tdata.Quads[y, x].WaterType);
                    sb.Append(',');
                });
                sb.Append("},");
            }

            // water level
            if (waterLevels)
            {
                sb.Append("\n\t\twaterLevels = {");

                XYLoop(quadLow, quadHigh, sb, delegate (int x, int y)
                {
                    sb.Append(tdata.Quads[y, x].WaterLevel);
                    sb.Append(',');
                });
                sb.Append("},");
            }

            // terrain level
            if (heights)
            {
                sb.Append("\n\t\theights = {");

                XYLoop(heightLow, heightHigh, sb, delegate (int x, int y)
                {
                    sb.Append(tdata.Heights[y, x]);
                    sb.Append(',');
                });
                sb.Append("},");
            }

            // vertex colors
            if (vertexColors)
            {
                sb.Append("\n\t\tvertexColors = {");

                XYLoop(heightLow, heightHigh, sb, delegate (int x, int y)
                {
                    sb.Append("{");
                    sb.Append(tdata.VertexCols[y, x].R);
                    sb.Append(',');
                    sb.Append(tdata.VertexCols[y, x].G);
                    sb.Append(',');
                    sb.Append(tdata.VertexCols[y, x].B);
                    sb.Append("},");
                });
                sb.Append("},");
            }

            // generator script

            //score luaerr fix
            sb.Append(@"
}
TB_Score_OnBuildingConstructionCompleteBkp = Score.OnBuildingConstructionComplete
Score.OnBuildingConstructionComplete = function() end");

            // remove old entities
            if (entities)
                sb.Append(@"
terrainPatcher.removeEntitiesInRegion(terrainData.posBoundaries)");

            // terrain height & vertex color
            if (heights || vertexColors)
            {
                sb.Append(@"

for x = terrainData.heightBoundaries.low.X, terrainData.heightBoundaries.high.X do
  for y = terrainData.heightBoundaries.low.Y, terrainData.heightBoundaries.high.Y do");
                if (heights)
                    sb.Append(@"
    Logic.SetTerrainNodeHeight(x, y, terrainData.heights[y-terrainData.heightBoundaries.low.Y+1][x-terrainData.heightBoundaries.low.X+1])");
                if (vertexColors)
                    sb.Append(@"
    Logic.SetTerrainVertexColor(x, y, 
    terrainData.vertexColors[y-terrainData.heightBoundaries.low.Y+1][x-terrainData.heightBoundaries.low.X+1][1], 
    terrainData.vertexColors[y-terrainData.heightBoundaries.low.Y+1][x-terrainData.heightBoundaries.low.X+1][2], 
    terrainData.vertexColors[y-terrainData.heightBoundaries.low.Y+1][x-terrainData.heightBoundaries.low.X+1][3])");
                sb.Append(@"
  end
end");
            }

            // terrain texture & water height % water type
            if (textures || waterLevels || waterTypes)
            {
                sb.Append(@"

for x = terrainData.quadBoundaries.low.X, terrainData.quadBoundaries.high.X do
  for y = terrainData.quadBoundaries.low.Y, terrainData.quadBoundaries.high.Y do");
                if (textures)
                    sb.Append(@"
    Logic.SetTerrainNodeType(x*4, y*4, terrainData.textures[y-terrainData.quadBoundaries.low.Y+1][x-terrainData.quadBoundaries.low.X+1]) ");
                if (waterTypes)
                    sb.Append(@"
    Logic.ExchangeWaterType(x*4, y*4, x*4+1, y*4+1, 0, terrainData.waterTypes[y-terrainData.quadBoundaries.low.Y+1][x-terrainData.quadBoundaries.low.X+1])");
                if (waterLevels)
                    sb.Append(@"
    Logic.WaterSetAbsoluteHeight(x*4, y*4, x*4+1, y*4+1, terrainData.waterLevels[y-terrainData.quadBoundaries.low.Y+1][x-terrainData.quadBoundaries.low.X+1]*4)");
                sb.Append(@"
  end
end");
            }

            // blocking update
            sb.Append(@"

--Logic.UpdateBlocking(terrainData.heightBoundaries.low.X, terrainData.heightBoundaries.low.Y, terrainData.heightBoundaries.high.X, terrainData.heightBoundaries.high.Y)");

            // entities
            if (entities)
                sb.Append(@"

for i = 1, table.getn(terrainData.newEntities) do
  local eId = Logic.CreateEntity(terrainData.newEntities[i][1], terrainData.newEntities[i][2], terrainData.newEntities[i][3], terrainData.newEntities[i][4], terrainData.newEntities[i][5])
  Logic.SetEntityScriptingValue(eId, -33, terrainData.newEntities[i][6])
  if terrainData.newEntities[i][7] then
    if terrainData.newEntities[i][8] then
      Logic.SetEntityScriptingValue(eId, 9, terrainData.newEntities[i][8])
      Logic.SetEntityScriptingValue(eId, -8, terrainData.newEntities[i][9])
    end
    if terrainData.newEntities[i][7] ~= """" then
      Logic.SetEntityName(eId, terrainData.newEntities[i][7])
    end
  end
end");

            // finishing generators
            sb.Append(@"

GUI.RebuildMinimapTerrain()
terrainData = nil
Score.OnBuildingConstructionComplete = TB_Score_OnBuildingConstructionCompleteBkp
CollectGarbage = function() collectgarbage(); return true; end
StartSimpleHiResJob(""CollectGarbage"")
end");

            // save
            FileStream fs = File.Create(f);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
            sw.Close();
        }

        static void XYLoop(Point low, Point high, StringBuilder sb, Action<int, int> action)
        {
            for (int y = low.Y; y <= high.Y; y++)
            {
                sb.Append('{');
                for (int x = low.X; x <= high.X; x++)
                    action(x, y);
                sb.Append("},");
            }
        }

        public string getTmpPath()
        {
            return tmpPath;
        }

        public void close()
        {
            try
            {
                Directory.Delete(tmpPath, true);
            }
            catch { }
        }
    }
}
