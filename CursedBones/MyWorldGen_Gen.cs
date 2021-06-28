using System;
using System.Collections.Generic;
using System.Linq;
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
			int patchSize = WorldGen.genRand.Next( minPatchSize, maxPatchSize );

			var done = new HashSet<(int x, int y)>();
			var candidates = new HashSet<(int x, int y)> { (tileX, tileY) };
			int gennedTiles = 0;

			(int x, int y) pair;
			do {
				pair = this.PopNextPatchTileCandidate( candidates );

				done.Add( pair );

				if( this.GenPatchTileAt(pair.x, pair.y) ) {
					gennedTiles++;

					if( gennedTiles >= patchSize ) {
						break;
					}
				}

				this.GetNextPatchTilesCandidatesNear( pair.x, pair.y, candidates, done, 1 );
			} while( candidates.Count > 0 );

			if( config.DebugModeInfo ) {
				CursedBonesMod.Instance.Logger.Info(
					"Generated Cursed Bones pile at " + tileX + ", " + tileY
					+ " with " + gennedTiles + " tiles."
				);
			}
		}


		////////////////

		private bool GenPatchTileAt(
					int tileX,
					int tileY ) {
			if( !this.IsValidGenTile(tileX, tileY) ) {
				return false;
			}

			bool success = WorldGen.PlaceTile(
				i: tileX,
				j: tileY,
				type: ModContent.TileType<CursedBonesTile>()
			);
			if( success ) {
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
			if( !done.Contains( (tileX-1, tileY-1) ) && this.IsValidGenTile(tileX-1, tileY-1) ) {
				candidates.Add( (tileX-1, tileY-1) );
			}
			if( !done.Contains( (tileX, tileY-1) ) && this.IsValidGenTile(tileX, tileY-1) ) {
				candidates.Add( (tileX, tileY-1) );
			}
			if( !done.Contains( (tileX+1, tileY-1) ) && this.IsValidGenTile(tileX+1, tileY-1) ) {
				candidates.Add( (tileX+1, tileY-1) );
			}
			if( !done.Contains( (tileX-1, tileY) ) && this.IsValidGenTile(tileX-1, tileY) ) {
				candidates.Add( (tileX-1, tileY) );
			}
			if( !done.Contains( (tileX+1, tileY) ) && this.IsValidGenTile(tileX+1, tileY) ) {
				candidates.Add( (tileX+1, tileY) );
			}
			if( !done.Contains( (tileX-1, tileY+1) ) && this.IsValidGenTile(tileX-1, tileY+1) ) {
				candidates.Add( (tileX-1, tileY+1) );
			}
			if( !done.Contains( (tileX, tileY+1) ) && this.IsValidGenTile(tileX, tileY+1) ) {
				candidates.Add( (tileX, tileY+1) );
			}
			if( !done.Contains( (tileX+1, tileY+1) ) && this.IsValidGenTile(tileX+1, tileY+1) ) {
				candidates.Add( (tileX+1, tileY+1) );
			}

			if( depth-- > 0 ) {
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