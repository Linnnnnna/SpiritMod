using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.Items.Pins
{
	// This just exists to store the pin locations on the world
	// There's no data management here - please don't flood with unnecessary bloat
	public class PinWorld : ModSystem
	{
		public TagCompound pins = new();

		public void SetPin(string name, Vector2 pos) => pins[name] = pos;

		public void RemovePin(string name) => pins.Remove(name);

		public override void SaveWorldData(TagCompound tag) => tag.Add("pins", pins);
		public override void LoadWorldData(TagCompound tag) => pins = tag.Get<TagCompound>("pins");

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(pins.Count);
			foreach (var pair in pins)
			{
				writer.Write(pair.Key);
				writer.WriteVector2(pins.Get<Vector2>(pair.Key));
			}
		}

		public override void NetReceive(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
				pins[reader.ReadString()] = reader.ReadVector2();
		}
	}
}