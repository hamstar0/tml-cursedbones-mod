using System;
using Terraria;
using Terraria.World.Generation;
using ModLibsCore.Libraries.Debug;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		private static bool HasOpenAirNearby( int tileX, int tileY, int rad=2 ) {
			int minX = tileX - rad <= 0 ? 1 : tileX - rad;
			int maxX = tileX + rad >= Main.maxTilesX ? Main.maxTilesX - 1 : tileX + rad;
			int minY = tileY - rad <= 0 ? 1 : tileY - rad;
			int maxY = tileY + rad >= Main.maxTilesY ? Main.maxTilesY - 1 : tileY + rad;

			for( int x = minX; x < maxX; x++ ) {
				for( int y = minY; y < maxY; y++ ) {
					Tile tile = Main.tile[x, y];
					if( tile?.active() == true ) {
						continue;
					}
					if( tile != null && tile.wall != 0 ) {
						continue;
					}

					return true;
				}
			}

			return false;
		}



		////////////////

		private bool IsValidGenTile( int x, int y, out bool hasMatter ) {
			if( x <= 1 || x >= Main.maxTilesX - 1 || y <= 1 || y >= Main.maxTilesY - 1 ) {
				hasMatter = true;
				return false;
			}

			Tile tile = Main.tile[x, y];

			hasMatter = tile?.active() == true;

			if( hasMatter ) {
				return false;
				/*if( CursedBonesPatchesGen.IsEarthType(tile.type) ) {
					// 15% chance to allow solid 'ground' to be replaced
					if( WorldGen.genRand.NextFloat() > 0.15f ) {
						return false;
					}
				} else {
					// Exclude all else
					return false;
				}*/
			}

			// No 'open air' tiles on surface
			if( y < WorldGen.worldSurface ) {	//Main.worldSurface?
				if( CursedBonesPatchesGen.HasOpenAirNearby(x, y) ) {
					return false;
				}
			}

			return CursedBonesPatchesGen.HasCardinallyAdjacentValidAttachableTileForBone( x, y );
		}
	}
}