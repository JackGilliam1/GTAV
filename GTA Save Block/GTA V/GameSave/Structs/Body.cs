using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Methods;

namespace GTA.V.Formats.Save.Structs
{
    public class Body
    {
        public List<Entry> Entries { get; set; }
        public Body(Stream xIn)
        {
            this.Entries = new List<Entry>();
            while (xIn.Position < xIn.Length)
            {
                Entry newEnt = new Entry(xIn);
                if (newEnt.Name != null)
                    Entries.Add(newEnt);
            }
        }
        public void Write(Stream xOut)
        {
            for (int i = 0; i < Entries.Count; i++)
                Entries[i].Write(xOut);
        }
    }
}