using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.Text;
using System.Globalization;

namespace TerrainPatcher
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            if (args.Length == 4)
            {
                Application.SetCompatibleTextRenderingDefault(false);
                MainIO main = new MainIO(true);
                main.readMapFile(args[0]);
                main.createRegionPatchFile(args[1], args[2], args[3], true, true, true, true, true, true);
                main.close();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
        
    }
}
