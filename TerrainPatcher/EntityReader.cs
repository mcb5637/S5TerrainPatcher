using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace TerrainPatcher
{
    class Entity
    {
        public PointF Position;
        public string Type;
        public string Name;
        public float Orientation;
        public float Scale;
        public int PlayerID;
        public int AmbientSound = 0;
        public int Volume;
        public static Dictionary<string, int> AmbientSounds = new Dictionary<string, int>()
        {
            { "AS_Forest",                  1   },
            { "AS_UHU",                     2   },
            { "AS_Campfire",                3   },
            { "AS_ForestEuropeDayTime",     4   },
            { "AS_SwampFrogs",              5   },
            { "AS_WaterLakeShore",          6   },
            { "AS_WaterfallBig",            7   },
            { "AS_WindMontain",             8   },
            { "AS_WaterRiverSmall",         9   },
            { "AS_Crickets",                10  },
            { "AS_Desertwind",              11  },
            { "AS_Leaves",                  12  },
            { "AS_MoorBird",                13  },
            { "AS_MoorBranches",            14  },
            { "AS_MoorBubbles",             15  },
            { "AS_FogPeopleDrums",          16  },
            { "AS_ClickeringPebbles",       17  },
            { "AS_RushingWaterNormal",      18  },
            { "AS_RushingWaterDontFreeze",  19  },
            { "AS_Seagulls",                20  },
            { "AS_Sheep",                   21  },
            { "AS_Wolves",                  22  }

        };

        public Entity(XElement xe)
        {
            Name = xe.Element("Name").Value;
            if (Name == "")
                Name = null; 
            Type = xe.Element("Type").Value;
            Orientation = float.Parse(xe.Element("Orientation").Value);
            PlayerID = int.Parse(xe.Element("PlayerID").Value);
            XElement pos = xe.Element("Position");
            Position = new PointF(float.Parse(pos.Element("X").Value), float.Parse(pos.Element("Y").Value));
            Scale = float.Parse(xe.Element("Scale").Value);

            string ambientType = xe.Element("AmbientSoundType").Value;
            if (ambientType != "")
            {
                AmbientSound = AmbientSounds[ambientType];
                Volume = int.Parse(xe.Element("Health").Value);
                if (Name == null)
                    Name = "";
            }
        }

        public unsafe Int32 Float2Int(float value)
        {
            return *((Int32*)(&value));
        }

        public override string ToString()
        {
            return "{Entities." + Type + ","
                + Position.X.ToString() + ","
                + Position.Y.ToString() + ","
                + Orientation.ToString() + ","
                + PlayerID.ToString() + ","
                + Float2Int(Scale)
                + (Name == null ? "" : ",\"" + Name + "\"")
                + (AmbientSound == 0 ? "" : "," + AmbientSound.ToString() + "," + Volume.ToString())
                + "}";

        }

        public bool IsInBetween(PointF pLow, PointF pHigh)
        {
            return (Position.X > pLow.X &&
                    Position.Y > pLow.Y &&
                    Position.X < pHigh.X &&
                    Position.Y < pHigh.Y);
        }
    }

    class PatchRegion
    {
        public Entity PointA, PointB;
        public PointF PosA { get { return PointA.Position; } }
        public PointF PosB { get { return PointB.Position; } }
        public float XHigh { get { return Math.Max(PosA.X, PosB.X); } }
        public float XLow { get { return Math.Min(PosA.X, PosB.X); } }
        public float YHigh { get { return Math.Max(PosA.Y, PosB.Y); } }
        public float YLow { get { return Math.Min(PosA.Y, PosB.Y); } }
    }

    class EntityList
    {
        public XDocument xd;
        public List<Entity> Entities;
        public Dictionary<string, PatchRegion> PatchRegions;

        public EntityList(string filename)
        {
            Entities = new List<Entity>();
            PatchRegions = new Dictionary<string, PatchRegion>();

            xd = XDocument.Load(filename);


            foreach (var ent in xd.Descendants("Entity"))
            {
                Entity e = new Entity(ent);
                if (e.Name != null && e.Name.Length >= 6)
                {
                    if (e.Name.Substring(0, 3) == "TP_")
                    {
                        string[] parts = e.Name.Substring(3).Split('/');
                        if (!PatchRegions.ContainsKey(parts[0]))
                            PatchRegions.Add(parts[0], new PatchRegion());
                        if (parts[1][0] == 'a')
                            PatchRegions[parts[0]].PointA = e;
                        else
                            PatchRegions[parts[0]].PointB = e;
                    }
                }
                Entities.Add(e);
            }
        }

        public List<Entity> GetEntitiesInRegion(PatchRegion pr)
        {
            List<Entity> ents = new List<Entity>();

            //caching:
            PointF pLow = new PointF(pr.XLow, pr.YLow);
            PointF pHigh = new PointF(pr.XHigh, pr.YHigh);

            foreach (Entity e in Entities)
            {
                if (e.IsInBetween(pLow, pHigh))
                    ents.Add(e);
            }
            return ents;
        }
    }
}
