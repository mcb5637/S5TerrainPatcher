using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading;
using System.Windows.Forms;

namespace TerrainPatcher
{
    public partial class Form1 : Form
    {
        private string tmpPath;
        private EntityList elSrc;
        private TerrainData tdSrc;

        public Form1()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            btnChooseFileSrc.Tag = tbSrcMap;
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
                MessageBox.Show("A problem occured while creating the Helperfiles", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            TextBox outTb = (sender as Button).Tag as TextBox;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Siedler 5 Maps (*.s5x)|*.s5x|Alle Dateien (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                outTb.Text = ofd.FileName;
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (!File.Exists(tbSrcMap.Text))
            {
                MessageBox.Show("Datei nicht gefunden!");
                return;
            }

            try
            {
                File.Copy(tbSrcMap.Text, tmpPath + "/src.s5x", true);

                ProcessStartInfo psi = new ProcessStartInfo(tmpPath + "/bbaTool.exe", "\"" + tmpPath + "/src.s5x\"");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process proc = Process.Start(psi);
                proc.WaitForExit();


                elSrc = new EntityList(tmpPath + "/src.s5x.unpacked/maps/externalmap/mapdata.xml");
                tdSrc = new TerrainData(tmpPath + "/src.s5x.unpacked/maps/externalmap/terraindata.bin", tmpPath + "/src.s5x.unpacked/maps/externalmap/mapvertexcolors.bin");

                pnlRegions.Controls.Clear();
                if (elSrc.PatchRegions.Count == 0)
                {
                    Label lblRegion = new Label();
                    lblRegion.AutoSize = true;
                    lblRegion.Text = "Keine Regionen gefunden!";
                    lblRegion.Location = new Point(12, 11);
                    pnlRegions.Controls.Add(lblRegion);
                    return;
                }
                int num = 0;
                foreach (KeyValuePair<string, PatchRegion> kvp in elSrc.PatchRegions)
                {
                    Label lblRegion = new Label();
                    lblRegion.AutoSize = true;
                    lblRegion.Text = kvp.Key;
                    lblRegion.Location = new Point(12, 11 + 25 * num);
                    Button btnSave = new Button();
                    btnSave.Text = "Speichern";
                    btnSave.Tag = kvp.Key;
                    btnSave.Click += btnSave_Click;
                    btnSave.Location = new Point(220, 6 + 25 * num);
                    pnlRegions.Controls.Add(lblRegion);
                    pnlRegions.Controls.Add(btnSave);

                    num++;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error:\n" + ex.ToString()); }
        }

        void XYLoop(Point low, Point high, StringBuilder sb, Action<int, int> action)
        {
            for (int y = low.Y; y <= high.Y; y++)
            {
                sb.Append('{');
                for (int x = low.X; x <= high.X; x++)
                    action(x, y);
                sb.Append("},");
            }
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            string prKey = (sender as Button).Tag as string;
            PatchRegion pr = elSrc.PatchRegions[prKey];

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Lua Dateien (*.lua)|*.lua|Alle Dateien (*.*)|*.*";
            sfd.FileName = prKey.ToLower() + ".lua";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            
            StringBuilder sb = new StringBuilder();
            Point heightHigh = new Point((int)pr.XHigh / 100, (int)pr.YHigh / 100);
            Point heightLow = new Point((int)pr.XLow / 100, (int)pr.YLow / 100);
            Point quadHigh = new Point((int)pr.XHigh / 400, (int)pr.YHigh / 400);
            Point quadLow = new Point((int)pr.XLow / 400, (int)pr.YLow / 400);
            PointF middlePos = new PointF((pr.XHigh - pr.XLow) / 2 + pr.XLow, (pr.YHigh - pr.YLow) / 2 + pr.YLow);
            float radius = (float)Math.Sqrt(((pr.XHigh - pr.XLow) / 2) * ((pr.XHigh - pr.XLow) / 2) + ((pr.YHigh - pr.YLow) / 2) * ((pr.YHigh - pr.YLow) / 2));
            sb.Append(string.Format(@"patchRegions.{12} = function()
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
    }},", heightHigh.X, heightHigh.Y, heightLow.X, heightLow.Y, quadHigh.X, quadHigh.Y, quadLow.X, quadLow.Y, pr.XHigh, pr.YHigh, pr.XLow, pr.YLow, prKey));
            bool entities = cbEntities.Checked, textures = cbTextures.Checked, heights = cbHeights.Checked, vertexColors = cbVertexColors.Checked, waterTypes = cbWaterTypes.Checked, waterLevels = cbWaterLevels.Checked;
            if (entities)
            {
                sb.Append("\n\t\tnewEntities = {");
                foreach (Entity ent in elSrc.GetEntitiesInRegion(pr))
                {
                    sb.Append(ent);
                    sb.Append(',');
                }
                sb.Append("},");
            }

            if (textures)
            {
                sb.Append("\n\t\ttextures = {");

                XYLoop(quadLow, quadHigh, sb, delegate(int x, int y)
                {
                    sb.Append(tdSrc.Quads[y, x].TerrainType);
                    sb.Append(',');
                });
                sb.Append("},");
            }

            if (waterTypes)
            {
                sb.Append("\n\t\twaterTypes = {");

                XYLoop(quadLow, quadHigh, sb, delegate(int x, int y)
                {
                    sb.Append(tdSrc.Quads[y, x].WaterType);
                    sb.Append(',');
                });
                sb.Append("},");
            }

            if (waterLevels)
            {
                sb.Append("\n\t\twaterLevels = {");

                XYLoop(quadLow, quadHigh, sb, delegate(int x, int y)
                {
                    sb.Append(tdSrc.Quads[y, x].WaterLevel);
                    sb.Append(',');
                });
                sb.Append("},");
            }

            if (heights)
            {
                sb.Append("\n\t\theights = {");

                XYLoop(heightLow, heightHigh, sb, delegate(int x, int y)
                {
                    sb.Append(tdSrc.Heights[y, x]);
                    sb.Append(',');
                });
                sb.Append("},");
            }

            if (vertexColors)
            {
                sb.Append("\n\t\tvertexColors = {");

                XYLoop(heightLow, heightHigh, sb, delegate(int x, int y)
                {
                    sb.Append("{");
                    sb.Append(tdSrc.VertexCols[y, x].R);
                    sb.Append(',');
                    sb.Append(tdSrc.VertexCols[y, x].G);
                    sb.Append(',');
                    sb.Append(tdSrc.VertexCols[y, x].B);
                    sb.Append("},");
                });
                sb.Append("},");
            }
            sb.Append(@"
}
TB_Score_OnBuildingConstructionCompleteBkp = Score.OnBuildingConstructionComplete
Score.OnBuildingConstructionComplete = function() end");
            if (entities)
                sb.Append(@"
RemoveEntitiesInRegion(terrainData.posBoundaries)");
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
            sb.Append(@"

Logic.UpdateBlocking(terrainData.heightBoundaries.low.X, terrainData.heightBoundaries.low.Y, terrainData.heightBoundaries.high.X, terrainData.heightBoundaries.high.Y)
GUI.RebuildMinimapTerrain()
terrainData = nil
Score.OnBuildingConstructionComplete = TB_Score_OnBuildingConstructionCompleteBkp
CollectGarbage = function() collectgarbage(); return true; end
StartSimpleHiResJob(""CollectGarbage"")
end");

            FileStream fs = File.Create(sfd.FileName);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
            sw.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Directory.Delete(tmpPath, true);
            }
            catch { }
        }

        private void btnGetLuaFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Lua Dateien (*.lua)|*.lua|Alle Dateien (*.*)|*.*";
            sfd.FileName = "terrainpatcher.lua";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            Assembly localAssembly = Assembly.GetExecutingAssembly();

            try
            {
                Stream resourceStream = localAssembly.GetManifestResourceStream("TerrainPatcher.Comfort.terrainpatcher.lua");
                using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                {
                    resourceStream.CopyTo(fs);
                }
            }
            catch { }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(tmpPath + "/readme.txt"));
        }




    }


    public static class MyExtensions
    {
        // Only useful before .NET 4
        public static void CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}
