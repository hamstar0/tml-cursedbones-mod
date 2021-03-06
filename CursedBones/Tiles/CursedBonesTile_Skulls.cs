using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace CursedBones.Tiles {
	public partial class CursedBonesTile : ModTile {
		public static void RunBonesLaunchersNearby( Player player, bool syncIfServer ) {
			int bonesTileType = ModContent.TileType<CursedBonesTile>();
			var config = CursedBonesConfig.Instance;

			int attemptsPerTick = config.CursedBonesSkullAttemptsPerTick;
			int skullRad = config.CursedBonesSkullAttackTileRange;
			int midTileX = (int)player.Center.X / 16;
			int midTileY = (int)player.Center.Y / 16;

			for( int i = 0; i < attemptsPerTick; i++ ) {
				int x = midTileX + Main.rand.Next( -skullRad, skullRad );
				int y = midTileY + Main.rand.Next( -skullRad, skullRad );
				if( !WorldGen.InWorld(x, y) ) {
					continue;
				}

				Tile tile = Main.tile[x, y];
				if( tile?.active() == true && tile.type == bonesTileType ) {
					CursedBonesTile.LaunchSkull( player, x, y, syncIfServer );
				}
			}
		}


		////////////////

		public static void LaunchSkull( Player player, int tileX, int tileY, bool syncIfServer ) {
			var config = CursedBonesConfig.Instance;
			int damage = config.CursedBonesSkullDamage;

			Vector2 pos = new Vector2( tileX * 16 + 8, tileY * 16 + 8 );
			Vector2 vel = player.Center - pos;
			vel.X += (float)Main.rand.Next( -20, 21 );
			vel.Y += (float)Main.rand.Next( -20, 21 );
			vel.Normalize();
			vel *= 0.25f;

			int projWho = Projectile.NewProjectile(
				position: pos,
				velocity: vel,
				Type: ProjectileID.Skull,
				Damage: damage,
				KnockBack: 0f,
				Owner: Main.myPlayer,
				ai0: -1f,
				ai1: 0f
			);

			CursedBonesTile.InitializeSkullProjectileStats( projWho );

			if( syncIfServer && Main.netMode == NetmodeID.Server ) {
				NetMessage.SendData( MessageID.SyncProjectile, -1, -1, null, projWho );

				var packet = CursedBonesMod.Instance.GetPacket();
				packet.Write( projWho );
				packet.Send();
			}
		}


		////

		internal static void InitializeSkullProjectileStats( int projWho ) {
			Projectile proj = Main.projectile[ projWho ];
			proj.scale = 0.75f;
			proj.tileCollide = false;
			proj.timeLeft = 300;
		}
	}
}
