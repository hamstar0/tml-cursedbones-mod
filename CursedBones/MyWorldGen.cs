using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.World.Generation;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		public CursedBonesPatchesGen() : base( "Cursed Bone Patches", 1f ) { }


		public override void Apply( GenerationProgress progress ) {
			var config = CursedBonesConfig.Instance;
			int minDist = config.CursedBonesWorldGenMinimumTileDistanceApart;
			int maxRetries = config.CursedBonesWorldGenMaximumRetriesPerPatchUntilQuit;

			int maxTheoreticalGens = (Main.maxTilesX * Main.maxTilesY) / (minDist * minDist);
			maxTheoreticalGens = (maxTheoreticalGens / 100000) * maxRetries;
			var gens = new HashSet<(int, int)>();

			for( int retries=0; retries<maxRetries; retries++ ) {
				if( this.AttemptNextPatchGen(minDist, gens) ) {
					retries = 0;

					progress.Value = Math.Min( (float)gens.Count / (float)maxTheoreticalGens, 1f );
				}
			}

			progress.Value = 1f;
		}


		////////////////

		public bool AttemptNextPatchGen( int minDist, ISet<(int, int)> genTiles ) {
			int minDistSqr = minDist * minDist;
			int randX = WorldGen.genRand.Next( Main.maxTilesX );
			int randY = WorldGen.genRand.Next( Main.maxTilesX );

			(int x, int y)? testTile = this.ScanForValidRawGenSpot( randX, randY );
			if( !testTile.HasValue ) {
				return false;
			}
			
			// Any existing gens too close?
			foreach( (int x, int y) in genTiles ) {
				int diffX = testTile.Value.x - x;
				int diffY = testTile.Value.y - y;
				int diffSqr = diffX * diffX + diffY * diffY;

				if( diffSqr < minDistSqr ) {
					return false;
				}
			}

			this.GenPatchAt( testTile.Value.x, testTile.Value.y );

			genTiles.Add( testTile.Value );

			return true;
		}
	}
}