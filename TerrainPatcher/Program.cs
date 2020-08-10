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
using System.Runtime.InteropServices;

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
                AttachConsole();
                Application.SetCompatibleTextRenderingDefault(false);
                MainIO main = new MainIO(true);
                main.readMapFile(args[0]);
                main.createRegionPatchFile(args[1], args[2], args[3], true, true, true, true, true, true);
                main.close();
            }
            else if (args.Length == 3)
            {
                AttachConsole();
                try
                {
                    Application.SetCompatibleTextRenderingDefault(false);
                    MainIO main = new MainIO(true);
                    main.readMapFile(args[0]);
                    HeightTextureMapIO ht = new HeightTextureMapIO(main.tdata);
                    bool clamp = ht.WriteHeightMap(args[1]);
                    ht.WriteTextureMap(args[2]);
                    if (clamp)
                        Console.WriteLine("some heights were out of range (>=1521, <=6978), they got clamped to the correct range.");
                    main.close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TerrainPatcherGUI());
            }
        }

        static void AttachConsole()
        {
            AttachConsole(-1);
        }

        [DllImport("kernel32", SetLastError = true)]
        static extern bool AttachConsole(int dwProcessId);
    }
}
