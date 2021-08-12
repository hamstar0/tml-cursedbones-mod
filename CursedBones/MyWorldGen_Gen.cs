using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using CursedBones.Tiles;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		public void GenPatchAt( int tileX, int tileY ) {
			var config = CursedBonesConfig.Instance;
			int minPatchSize = config.CursedBonesWorldGenPatchMinimumSize;
			int maxPatchSize = config.CursedBonesWorldGenPatchMaximumSize;

			float gradY = CursedBonesPatchesGen.CalculateVerticalGradientPercentAt( tileY );

			int patchSize = WorldGen.genRand.Next( minPatchSize, maxPatchSize );
			patchSize = (int)( (float)patchSize * gradY );

			if( patchSize < minPatchSize ) {
				//return false;
				patchSize = minPatchSize;
			}

			var done = new HashSet<(int x, int y)>();
			var candidates = new HashSet<(int x, int y)>();
			int gennedTiles = 0;

			if( patchSize >= 1 ) {
				gennedTiles++;

				candidates.Add( (tileX, tileY) );
			}

			(int x, int y) pair;
			while( candidates.Count >= 1 ) {
				pair = this.PopNextPatchTileCandidate( candidates );

				done.Add( pair );

				if( this.GenPatchTileAt(pair.x, pair.y) ) {
					gennedTiles++;
					if( gennedTiles >= patchSize ) {
						break;
					}
				}

				this.GetNextPatchTilesCandidatesNear( pair.x, pair.y, candidates, done, 1 );
			}

			// Too small
			if( gennedTiles <= 1 && minPatchSize >= 2 ) {
				int bonesTile = ModContent.TileType<CursedBonesTile>();

				foreach( (int x, int y) in done ) {
					Tile tile = Main.tile[x, y];
					if( tile != null && tile.type == bonesTile ) {
						tile.type = TileID.Stone;
					}
				}
				gennedTiles = 0;
			}

			if( config.DebugModeInfo ) {
				CursedBonesMod.Instance.Logger.Info(
					"Generated Cursed Bones pile at "+tileX+", "+tileY+" with "+gennedTiles+" tiles."
				);
			}
		}


		////////////////

		private bool GenPatchTileAt(
					int tileX,
					int tileY ) {
			if( !this.IsValidGenTile(tileX, tileY, out bool hasMatter) ) {
				return false;
			}

			if( hasMatter ) {
				WorldGen.KillTile( tileX, tileY );
			}

			bool success = WorldGen.PlaceTile(
				i: tileX,
				j: tileY,
				type: ModContent.TileType<CursedBonesTile>()
			);

			if( success ) {
				if( tileX >= 1 ) {
					Main.tile[tileX - 1, tileY]?.slope( 0 );
				}
				if( tileY >= 1 ) {
					Main.tile[tileX, tileY - 1]?.slope( 0 );
				}
				if( tileY < Main.maxTilesY - 1 ) {
					Main.tile[tileX, tileY + 1]?.slope( 0 );
				}
				if( tileX < Main.maxTilesX - 1 ) {
					Main.tile[tileX + 1, tileY]?.slope( 0 );
				}

				WorldGen.SquareTileFrame( tileX, tileY );
			}

			return success;
		}


		////////////////

		private void GetNextPatchTilesCandidatesNear(
					int tileX,
					int tileY,
					ISet<(int x, int y)> candidates,
					ISet<(int x, int y)> done,
					int depth ) {
			if( !done.Contains( (tileX-1, tileY-1) ) && this.IsValidGenTile(tileX-1, tileY-1, out _) ) {
				candidates.Add( (tileX-1, tileY-1) );
			}
			if( !done.Contains( (tileX, tileY-1) ) && this.IsValidGenTile(tileX, tileY-1, out _) ) {
				candidates.Add( (tileX, tileY-1) );
			}
			if( !done.Contains( (tileX+1, tileY-1) ) && this.IsValidGenTile(tileX+1, tileY-1, out _) ) {
				candidates.Add( (tileX+1, tileY-1) );
			}
			if( !done.Contains( (tileX-1, tileY) ) && this.IsValidGenTile(tileX-1, tileY, out _) ) {
				candidates.Add( (tileX-1, tileY) );
			}
			if( !done.Contains( (tileX+1, tileY) ) && this.IsValidGenTile(tileX+1, tileY, out _) ) {
				candidates.Add( (tileX+1, tileY) );
			}
			if( !done.Contains( (tileX-1, tileY+1) ) && this.IsValidGenTile(tileX-1, tileY+1, out _) ) {
				candidates.Add( (tileX-1, tileY+1) );
			}
			if( !done.Contains( (tileX, tileY+1) ) && this.IsValidGenTile(tileX, tileY+1, out _) ) {
				candidates.Add( (tileX, tileY+1) );
			}
			if( !done.Contains( (tileX+1, tileY+1) ) && this.IsValidGenTile(tileX+1, tileY+1, out _) ) {
				candidates.Add( (tileX+1, tileY+1) );
			}

			if( depth-- >= 1 ) {
				this.GetNextPatchTilesCandidatesNear( tileX, tileY - 1, candidates, done, depth );
				this.GetNextPatchTilesCandidatesNear( tileX - 1, tileY, candidates, done, depth );
				this.GetNextPatchTilesCandidatesNear( tileX, tileY + 1, candidates, done, depth );
				this.GetNextPatchTilesCandidatesNear( tileX + 1, tileY, candidates, done, depth );
			}
		}


		////////////////

		private (int x, int y) PopNextPatchTileCandidate( ISet<(int x, int y)> candidates ) {
			(int, int) pick = this.GetNextPatchTileCandidate( candidates );

			candidates.Remove( pick );

			return pick;
		}
	}
}