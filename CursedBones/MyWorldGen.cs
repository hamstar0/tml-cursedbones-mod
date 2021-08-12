using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		public static float CalculateGradientPercentAt( int tileY ) {
			float gradPerc = 0f;
			var config = ModContent.GetInstance<CursedBonesConfig>();

			float topGradPercY = config.CursedBonesWorldGenDensityGradientFromStartPercent;

			float topGradY = (float)tileY / ( (float)Main.maxTilesY * topGradPercY );
			topGradY = 1f - Math.Min( topGradY, 1f );

			gradPerc += topGradY;

			//

			float botGradPercY = config.CursedBonesWorldGenDensityGradientFromEndPercent;
			int invTileY = Main.maxTilesY - tileY;

			float botGradY = (float)invTileY / ( (float)Main.maxTilesY * botGradPercY );
			botGradY = 1f - Math.Min( botGradY, 1f );

			gradPerc += botGradY;

			//

			return gradPerc;
		}



		////

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
			int minMapDim = Main.maxTilesX > Main.maxTilesY
				? Main.maxTilesY
				: Main.maxTilesX;

			//

			int randX = WorldGen.genRand.Next( Main.maxTilesX );
			int randY = WorldGen.genRand.Next( Main.maxTilesY );

			//

			float gradY = CursedBonesPatchesGen.CalculateGradientPercentAt( randY );

			minDist += (int)((float)minMapDim * gradY);

			//

			int minDistSqr = minDist * minDist;

			//

			(int x, int y)? testTile = this.ScanForValidRawGenSpot( randX, randY );
			if( !testTile.HasValue ) {
				return false;
			}

			//

			// Any existing gens too close?
			foreach( (int x, int y) in genTiles ) {
				int diffX = testTile.Value.x - x;
				int diffY = testTile.Value.y - y;
				int diffSqr = (diffX * diffX) + (diffY * diffY);

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