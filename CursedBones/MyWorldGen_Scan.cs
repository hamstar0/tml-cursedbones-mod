using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.World.Generation;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		private bool IsValidGenTile( int x, int y, out bool hasMatter ) {
			if( x <= 1 || x >= Main.maxTilesX - 1 || y <= 1 || y >= Main.maxTilesY - 1 ) {
				hasMatter = true;
				return false;
			}

			Tile tile = Main.tile[x, y];

			hasMatter = tile?.active() == true;

			// No sky tiles
			if( y < Main.worldSurface ) {
				if( tile.wall == 0 ) {
					return false;
				}
			}

			if( hasMatter ) {
				if( CursedBonesPatchesGen.IsEarthType(tile.type) ) {
					// 15% chance to allow solid 'ground' to be replaced
					if( WorldGen.genRand.NextFloat() > 0.15f ) {
						return false;
					}
				} else {
					// Exclude all else
					return false;
				}
			}

			return CursedBonesPatchesGen.HasCardinallyAdjacentValidAttachableTileForBone( x, y );
		}
	}
}