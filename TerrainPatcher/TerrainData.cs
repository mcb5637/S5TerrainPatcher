using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TerrainPatcher
{
    struct Quad
    {
        public byte TerrainType;
        public byte WaterType;
        public UInt16 WaterLevel;
    }

    struct VertexCol
    {
        public byte R;
        public byte G;
        public byte B;
        public byte Alpha;
    }

    class TerrainData
    {
        public UInt16[,] Heights;
        public Quad[,] Quads;
        public VertexCol[,] VertexCols;

        public TerrainData(string terrainDatafilename, string vertexColorsfilename)
        {

            //StringBuilder sb = new StringBuilder();
            FileStream fs = File.Open(terrainDatafilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader bin = new BinaryReader(fs);

            bin.ReadUInt32();
            bin.ReadUInt32();
            bin.ReadUInt32();
            bin.ReadUInt32();
            bin.ReadUInt32();
            bin.ReadUInt32();
            UInt32 quadOffset = bin.ReadUInt32();
            //bin.BaseStream.Seek(quadOffset, SeekOrigin.Current);
            bin.ReadUInt32(); //1

            bin.ReadUInt32(); //ms+1
            bin.ReadUInt32(); //ms+1
            UInt32 mX = bin.ReadUInt32(); //ms+3
            UInt32 mY = bin.ReadUInt32(); //ms+3
            //sb.Append(string.Format("terrain = \n{{\n\thCnt = {0},\n\theights = {{\n", (int)mX - 2));

            Heights = new UInt16[mY - 2, mX - 2];

            bin.ReadBytes(2 * (int)mX);
            for (int y = 0; y < mY - 2; y++)
            {
                bin.ReadBytes(2);
                for (int x = 0; x < mX - 2; x++)
                {
                    Heights[y, x] = bin.ReadUInt16();
                }

                bin.ReadBytes(2);
            }

            bin.ReadBytes(2 * (int)mX);

            bin.ReadUInt32(); // 8C 91 34 3D
            bin.ReadUInt32(); // 23 39 E0 58
            bin.ReadUInt32(); // offset to end
            bin.ReadUInt32(); // 1
            bin.ReadUInt32(); // visible X
            bin.ReadUInt32(); // visible Y
            UInt32 qX = bin.ReadUInt32();
            UInt32 qY = bin.ReadUInt32();
            //sb.Append(string.Format("\n\t}},\n\ttCnt = {0},\n\ttextures = {{\n", (int)qX - 3));
            //sb.Append(string.Format("terrain = \n{{\n\tdimension = {0},\n\ttextures = {{\n", qX));

            Quads = new Quad[qY - 2, qX - 2];

            bin.ReadBytes(4 * (int)qX);
            for (int y = 0; y < qY - 2; y++)
            {
                bin.ReadBytes(4);
                for (int x = 0; x < qX - 2; x++)
                {
                    Quad q;
                    q.TerrainType = bin.ReadByte();
                    q.WaterType = (byte)(bin.ReadByte() & 0x3F);
                    q.WaterLevel = bin.ReadUInt16();
                    Quads[y, x] = q;
                }
                bin.ReadBytes(4);
            }
            bin.ReadBytes(4 * (int)qX);
            fs.Close();

            fs = File.Open(vertexColorsfilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            bin = new BinaryReader(fs);

            bin.ReadUInt32();
            bin.ReadUInt32();
            bin.ReadUInt32();
            bin.ReadUInt32();
            bin.ReadUInt32();

            mX = bin.ReadUInt32(); //ms+3
            mY = bin.ReadUInt32(); //ms+3

            VertexCols = new VertexCol[mY - 2, mX - 2];
            bin.ReadBytes(4 * (int)mX);
            for (int y = 0; y < mY - 2; y++)
            {
                bin.ReadBytes(4);
                for (int x = 0; x < mX - 2; x++)
                {
                    VertexCols[y, x].B = bin.ReadByte();
                    VertexCols[y, x].G = bin.ReadByte();
                    VertexCols[y, x].R = bin.ReadByte();
                    VertexCols[y, x].Alpha = bin.ReadByte();
                }
                bin.ReadBytes(4);
            }
            fs.Close();
        }
    }
}
