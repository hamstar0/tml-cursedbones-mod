using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using CursedBones.Tiles;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		public static bool IsEarthType( int tileType ) {
			switch( tileType ) {
			case TileID.Dirt:
			case TileID.Stone:
			case TileID.Grass:
			case TileID.Mud:
			case TileID.SnowBlock:
			case TileID.IceBlock:
			case TileID.Sand:
			case TileID.Sandstone:
			case TileID.HardenedSand:
			case TileID.JungleGrass:
			case TileID.Granite:
			case TileID.Marble:
			case TileID.MushroomGrass:
			//
			case TileID.CorruptGrass:
			case TileID.FleshGrass:
			case TileID.Ebonstone:
			case TileID.Crimstone:
			case TileID.Pearlstone:
			case TileID.Ebonsand:
			case TileID.Crimsand:
			case TileID.CorruptSandstone:
			case TileID.CrimsonSandstone:
			case TileID.CorruptHardenedSand:
			case TileID.CrimsonHardenedSand:
			case TileID.HallowSandstone:
			case TileID.HallowHardenedSand:
			case TileID.CorruptIce:
			case TileID.FleshIce:
			case TileID.HallowedIce:
				return true;
			default:
				return false;
			}
		}



		////////////////

		public static bool IsValidAttachableTileForBone( int tileX, int tileY ) {
			if( !WorldGen.InWorld(tileX, tileY) ) {
				return false;
			}

			Tile tile = Main.tile[tileX, tileY];
			if( tile?.active() != true ) {
				return false;
			}

			return CursedBonesPatchesGen.IsEarthType( tile.type );
		}


		////////////////

		private static bool HasCardinallyAdjacentValidAttachableTileForBone( int tileX, int tileY ) {
			if( CursedBonesPatchesGen.IsValidAttachableTileForBone( tileX, tileY - 1 ) ) {
				return true;
			}
			if( CursedBonesPatchesGen.IsValidAttachableTileForBone( tileX - 1, tileY ) ) {
				return true;
			}
			if( CursedBonesPatchesGen.IsValidAttachableTileForBone( tileX, tileY + 1 ) ) {
				return true;
			}
			if( CursedBonesPatchesGen.IsValidAttachableTileForBone( tileX + 1, tileY ) ) {
				return true;
			}
			return false;
		}


		////////////////

		public static int CountCardinallyAdjacentBones( int tileX, int tileY ) {
			if( tileX <= 1 || tileX >= Main.maxTilesX - 1 || tileY <= 1 || tileY >= Main.maxTilesY - 1 ) {
				return 0;
			}

			int bonesTile = ModContent.TileType<CursedBonesTile>();
			int count = 0;

			if( Main.tile[tileX, tileY - 1]?.type == bonesTile ) {
				count++;
			}
			if( Main.tile[tileX, tileY + 1]?.type == bonesTile ) {
				count++;
			}
			if( Main.tile[tileX - 1, tileY]?.type == bonesTile ) {
				count++;
			}
			if( Main.tile[tileX + 1, tileY]?.type == bonesTile ) {
				count++;
			}

			return count;
		}
	}
}