using Microsoft.Xna.Framework;
using SpiritMod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.NPCs
{
	public class SnowBreathGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public override void PostAI(NPC npc)
		{
			Player closest = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];

			if (closest.ZoneSnow || closest.ZoneNormalSpace)
			{
				if (npc.townNPC && Main.rand.NextBool(27))
				{
					var spawnPos = new Vector2(npc.position.X + 8 * npc.direction, npc.Center.Y - 13f);
					int d = Dust.NewDust(spawnPos, npc.width, 10, ModContent.DustType<FrostBreath>(), 1.5f * npc.direction, 0f, 100, default, Main.rand.NextFloat(.20f, 0.75f));
					Main.dust[d].velocity.Y *= 0f;
				}
			}
		}
	}
}