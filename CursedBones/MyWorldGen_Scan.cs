using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.World.Generation;


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

		private bool IsValidGenTile( int x, int y, out bool hasMatter ) {
			if( x <= 1 || x >= Main.maxTilesX - 1 || y <= 1 || y >= Main.maxTilesY - 1 ) {
				hasMatter = true;
				return false;
			}

			Tile tile = Main.tile[x, y];

			hasMatter = tile?.active() == true;

			if( hasMatter ) {
				if( CursedBonesPatchesGen.IsEarthType(tile.type) ) {
					// 25% chance to allow solid 'ground' to be replaced
					if( WorldGen.genRand.NextFloat() > 0.25f ) {
						return false;
					}
				} else {
					// Exclude all else
					return false;
				}
			}

			return this.HasAdjacentAttachArea( x, y );
		}

		private bool HasAdjacentAttachArea( int x, int y ) {
			bool scan( int x2, int y2 ) {
				return this.IsValidAttachArea( x, y, x2, y2, new HashSet<(int, int)>() );
			}

			if( scan( x - 1, y ) ) { return true; }
			if( scan( x + 1, y ) ) { return true; }
			if( scan( x, y - 1 ) ) { return true; }
			if( scan( x, y + 1 ) ) { return true; }
			return false;
		}

		private bool IsValidAttachArea( int primeX, int primeY, int x, int y, ISet<(int x, int y)> scanned ) {
			if( scanned.Count >= 10 ) {
				return true;
			}

			bool scan( int x2, int y2, ISet<(int x, int y)> scanned2 ) {
				if( x2 == primeX && y2 == primeY ) {
					return false;
				}
				if( scanned2.Contains( (x2, y2) ) ) {
					return false;
				}
				if( !this.IsValidAttachTile( x2, y2 ) ) {
					return false;
				}

				scanned2.Add( (x2, y2) );

				return this.IsValidAttachArea( x, y, x2, y2, scanned2 );
			}

			if( scan(x - 1, y, scanned) ) { return true; }
			if( scan(x + 1, y, scanned) ) { return true; }
			if( scan(x, y - 1, scanned) ) { return true; }
			if( scan(x, y + 1, scanned) ) { return true; }
			return false;
		}

		private bool IsValidAttachTile( int x, int y ) {
			if( !WorldGen.InWorld(x, y) ) {
				return false;
			}

			Tile tile = Main.tile[x, y];
			if( tile?.active() != true ) {
				return false;
			}
			
			if( CursedBonesPatchesGen.IsEarthType(tile.type) ) {
				return true;
			}

			return false;
		}


		////////////////

		public (int x, int y)? ScanForValidRawGenSpot( int startTileX, int startTileY ) {
			for( int i=0; i<100; i++ ) {
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