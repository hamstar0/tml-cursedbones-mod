using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.World.Generation;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		private (int x, int y)? PickAndRemoveNextTileFromCandidates( ISet<(int x, int y)> candidates ) {
			var config = CursedBonesConfig.Instance;
			int compactness = config.Get<int>( nameof(config.CursedBonesPatchCompactnessDegree) );

			(int, int)[] sample = this.PickRandomTilesFromCandidates( candidates, compactness );
			(int, int)? pick = this.PickPriorityTileFromCandidates( sample );	// prioritizes denser patches

			if( pick.HasValue ) {
				candidates.Remove( pick.Value );
			}

			return pick;
		}


		////////////////

		private (int x, int y)[] PickRandomTilesFromCandidates(
					ISet<(int x, int y)> candidates,
					int sampleSize ) {
			var candidateCandidates = new HashSet<(int, int)>();
			(int, int)[] candidatesArray = candidates.ToArray();

			for( int i=0; i<sampleSize; i++ ) {
				int check = WorldGen.genRand.Next( candidatesArray.Length );
				candidateCandidates.Add( candidatesArray[check] );
			}

			return candidateCandidates.ToArray();
		}

		////

		private (int x, int y)? PickPriorityTileFromCandidates( (int x, int y)[] candidates ) {
			// Crawl outwardly across the ground, if possible (75% of the time)
			if( WorldGen.genRand.NextFloat() >= 0.75f ) {
				foreach( (int x, int y) in candidates ) {
					if( !CursedBonesPatchesGen.HasCardinallyAdjacentValidAttachableTileForBone(x, y) ) { continue; }
					if( CursedBonesPatchesGen.CountCardinallyAdjacentBones(x, y) == 0 ) { continue; }

					return (x, y);
				}
			}

			// Grow from nearby bones, if not too dense
			foreach( (int x, int y) in candidates ) {
				int nearbyBones = CursedBonesPatchesGen.CountCardinallyAdjacentBones( x, y );
				if( nearbyBones >= 1 && nearbyBones <= 2 ) {
					return (x, y);
				}
			}

			/*// Grow from nearby tiles, regardless of bones
			foreach( (int x, int y) in candidates ) {
				if( this.IsNearValidAttachTile(x, y) ) {	<- redundant?
					return (x, y);
				}
			}*/

			// Otherwise, grow upon any random candidate tile nearby
			return candidates.FirstOrDefault();
		}
	}
}