using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Luxcinder
{
    public class LuxcinderWorld : ModSystem
    {
        public static bool downedFrostBitingWorm = false; // Changed to static

        public override void OnWorldLoad()
        {
            downedFrostBitingWorm = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedFrostBitingWorm)
            {
                tag["downedFrostBitingWorm"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedFrostBitingWorm = tag.ContainsKey("downedFrostBitingWorm");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = downedFrostBitingWorm;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedFrostBitingWorm = flags[0];
        }
    }
}