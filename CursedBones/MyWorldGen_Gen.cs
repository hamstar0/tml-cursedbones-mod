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

			ISet<(int x, int y)> genAttempts = new HashSet<(int, int)>();
			ISet<(int x, int y)> skips = new HashSet<(int, int)>();
			ISet<(int x, int y)> candidates = new HashSet<(int, int)>();
			int gennedTiles = 0;

			if( patchSize >= 1 ) {
				gennedTiles++;

				candidates.Add( (tileX, tileY) );
			}

			while( candidates.Count >= 1 ) {
				(int x, int y)? maybePair = this.PickAndRemoveNextTileFromCandidates( candidates );
				if( !maybePair.HasValue ) {
					break;
				}

				(int x, int y) pair = maybePair.Value;
				genAttempts.Add( pair );

				if( this.GenPatchTileAt(pair.x, pair.y) ) {
					gennedTiles++;
					if( gennedTiles >= patchSize ) {
						break;
					}
				}

				// Add to the overall list of viable "candidate" tiles with viable tiles near the current tile
				this.GetNextPatchTilesCandidatesNear(
					tileX: pair.x,
					tileY: pair.y,
					genAttempts: genAttempts,
					candidates: ref candidates,
					skips: ref skips,
					depth: 1
				);
			}

			// Too small
			if( gennedTiles <= 1 && minPatchSize >= 2 ) {
				int bonesTile = ModContent.TileType<CursedBonesTile>();

				foreach( (int x, int y) in genAttempts ) {
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
					ISet<(int x, int y)> genAttempts,
					ref ISet<(int x, int y)> candidates,
					ref ISet<(int x, int y)> skips,
					int depth ) {
			void checkCandidate( int x, int y, ref ISet<(int, int)> mycandidates, ref ISet<(int, int)> myskips ) {
				if( genAttempts.Contains( (x, y) ) || myskips.Contains( (x, y) ) ) {
					return;
				}

				if( this.IsValidGenTile(x, y, out bool hasMatter) ) {
					mycandidates.Add( (x, y) );
				} else if( hasMatter ) {
					myskips.Add( (x, y) );
				}
			}

			//

			checkCandidate( tileX - 1, tileY - 1, ref candidates, ref skips );
			checkCandidate( tileX,     tileY - 1, ref candidates, ref skips );
			checkCandidate( tileX + 1, tileY - 1, ref candidates, ref skips );

			checkCandidate( tileX - 1, tileY, ref candidates, ref skips );
			checkCandidate( tileX + 1, tileY, ref candidates, ref skips );

			checkCandidate( tileX - 1, tileY + 1, ref candidates, ref skips );
			checkCandidate( tileX,     tileY + 1, ref candidates, ref skips );
			checkCandidate( tileX + 1, tileY + 1, ref candidates, ref skips );

			if( depth-- >= 1 ) {
				this.GetNextPatchTilesCandidatesNear( tileX, tileY-1, genAttempts, ref candidates, ref skips, depth );
				this.GetNextPatchTilesCandidatesNear( tileX-1, tileY, genAttempts, ref candidates, ref skips, depth );
				this.GetNextPatchTilesCandidatesNear( tileX, tileY+1, genAttempts, ref candidates, ref skips, depth );
				this.GetNextPatchTilesCandidatesNear( tileX+1, tileY, genAttempts, ref candidates, ref skips, depth );
			}
		}
	}
}