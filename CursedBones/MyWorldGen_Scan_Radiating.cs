using System;
using Terraria;
using Terraria.World.Generation;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		public (int x, int y)? ScanForFirstValidGenTileRadiatingOutward(
					int startTileX,
					int startTileY,
					int range=100 ) {
			for( int i=0; i<range; i++ ) {
				int xL = startTileX - i;
				int xR = startTileX + i;
				int yT = startTileY - i;
				int yB = startTileY + i;

				if( this.IsValidGenTile(xL, startTileY, out _) ) {
					return (xL, startTileY);
				}
				if( this.IsValidGenTile(xR, startTileY, out _) ) {
					return (xR, startTileY);
				}
				if( this.IsValidGenTile(startTileX, yT, out _) ) {
					return (startTileX, yT);
				}
				if( this.IsValidGenTile(startTileX, yT, out _) ) {
					return (startTileX, yB);
				}
			}

			return null;
		}
	}
}