using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace TerrainPatcher
{
    class HeightTextureMapIO
    {
        private TerrainData TerrainData;

        public HeightTextureMapIO(TerrainData td)
        {
            TerrainData = td;
        }

        public bool WriteHeightMap(string filename)
        {
            Bitmap bm = new Bitmap((int) TerrainData.MapSize, (int) TerrainData.MapSize, PixelFormat.Format8bppIndexed);
            int highestVal = 6978;// 10000; // 6978;
            int lowestval = 1521; // 1521;
            bool hadToClamp = false;
            //for (int y = 0; y < TerrainData.MapSize; y++)
            //{
            //    for (int x = 0; x < TerrainData.MapSize; x++)
            //    {
            //        int v = TerrainData.Heights[y, x];
            //        highestVal = Math.Max(highestVal, v);
            //        lowestval = Math.Min(lowestval, v);
            //    }
            //}
            //double b = Math.Log(1.0 / (0xFF - 1)) / (lowestval - highestVal);
            //double a = 1 / Math.Pow(Math.E, b * lowestval);
            ColorPalette palette = bm.Palette;
            for (int x = 0; x < 256; x++)
            {
                palette.Entries[x] = Color.FromArgb(x, x, x);
            }
            bm.Palette = palette;
            BitmapData data = bm.LockBits(new Rectangle(Point.Empty, bm.Size), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            int datasize = data.Stride * data.Height;
            byte[] rawdata = new byte[datasize];
            for (int y = 0; y<TerrainData.MapSize; y++)
            {
                for (int x = 0; x < TerrainData.MapSize; x++)
                {
                    double v = TerrainData.Heights[y, x];
                    if (v > highestVal || v < lowestval)
                        hadToClamp = true;
                    v = Math.Max(Math.Min(v, highestVal), lowestval);
                    v = (v - lowestval) / (highestVal - lowestval);
                    v = (0xFF - 1) * v;
                    byte h = (byte)v;
                    h++;
                    //byte h = (byte) (a * Math.Pow(Math.E, v * b));
                    //bm.SetPixel(x, y, Color.FromArgb(h, h, h));
                    rawdata[(TerrainData.MapSize-y-1) * data.Stride + x] = h;
                }
            }
            Marshal.Copy(rawdata, 0, data.Scan0, datasize);
            bm.UnlockBits(data);
            bm.Save(filename, ImageFormat.Bmp);
            return hadToClamp;
        }

        public Dictionary<int, int> ReadTexturePalette(out List<Color> cols)
        {
            Dictionary<int, int> terrToColIndex = new Dictionary<int, int>();
            cols = new List<Color>();
            int i = 0;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TerrainPatcher.Palette.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string line = null;
                do
                {
                    line = reader.ReadLine();
                    if (line == null)
                        continue;
                    if (int.TryParse(line, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rgb))
                    {
                        Color c = Color.FromArgb(rgb);
                        cols.Add(c);
                    }
                    else
                        Console.WriteLine("broken color, skipping " + i);
                } while (line != null);
                i++;
            }
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TerrainPatcher.TerrainColors.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string line = null;
                do
                {
                    line = reader.ReadLine();
                    if (line == null)
                        continue;
                    string[] comps = line.Split(' ');
                    if (comps.Length != 3)
                    {
                        Console.WriteLine("broken line, skipping");
                        continue;
                    }
                    if (int.TryParse(comps[1], out int terr) && int.TryParse(comps[2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rgb))
                    {
                        Color c = Color.FromArgb(rgb);
                        if (!cols.Contains(c))
                            Console.WriteLine("missing color, skipping");
                        else
                            terrToColIndex.Add(terr, cols.IndexOf(c));
                    }
                    else
                        Console.WriteLine("broken ints, skipping");
                } while (line != null);
            }
            return terrToColIndex;
        }

        public void WriteTextureMap(string filename)
        {
            Dictionary<int, int> terrToColIndex = ReadTexturePalette(out List<Color> cols);

            Bitmap bm = new Bitmap((int)TerrainData.MapSize, (int)TerrainData.MapSize, PixelFormat.Format8bppIndexed);
            ColorPalette pal = bm.Palette;
            for (int i = 0; i < cols.Count; i++)
                pal.Entries[i] = cols[i];
            bm.Palette = pal;
            BitmapData data = bm.LockBits(new Rectangle(Point.Empty, bm.Size), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            int datasize = data.Stride * data.Height;
            byte[] rawdata = new byte[datasize];
            for (int y = 0; y < TerrainData.MapSize; y++)
            {
                for (int x = 0; x < TerrainData.MapSize; x++)
                {
                    byte t = TerrainData.Quads[y / 4, x / 4].TerrainType;
                    int ci = terrToColIndex[t];
                    rawdata[(TerrainData.MapSize - y - 1) * data.Stride + x] = (byte)ci;
                }
            }
            Marshal.Copy(rawdata, 0, data.Scan0, datasize);
            bm.UnlockBits(data);
            bm.Save(filename, ImageFormat.Bmp);
        }
    }
}
