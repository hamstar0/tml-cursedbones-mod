using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		public static float CalculateVerticalGradientPercentAt( int tileY ) {
			var config = ModContent.GetInstance<CursedBonesConfig>();

			float topGradPercY = config.CursedBonesWorldGenDensityGradientFromStartPercent;
			float botGradPercY = config.CursedBonesWorldGenDensityGradientFromEndPercent;

			//

			float topGradRange = (float)Main.maxTilesY * topGradPercY;
			float topGradY = (float)tileY / topGradRange;
			topGradY = Math.Min( topGradY, 1f );

			//

			int invTileY = Main.maxTilesY - tileY;

			float botGradRange = (float)Main.maxTilesY * botGradPercY;
			float botGradY = (float)invTileY / botGradRange;
			botGradY = Math.Min( botGradY, 1f );

			//

			return topGradY * botGradY;
		}



		////////////////

		public CursedBonesPatchesGen() : base( "Cursed Bone Patches", 1f ) { }


		public override void Apply( GenerationProgress progress ) {
			var config = CursedBonesConfig.Instance;
			int minDistApart = config.CursedBonesWorldGenMinimumTileDistanceApart;
			int maxRetries = config.CursedBonesWorldGenMaximumRetriesPerPatchUntilQuit;

			int maxTheoreticalGens = (Main.maxTilesX * Main.maxTilesY) / (minDistApart * minDistApart);
			maxTheoreticalGens /= 100;
			//maxTheoreticalGens = (maxTheoreticalGens / 100000) * maxRetries;

			var gens = new HashSet<(int, int)>();

			for( int retries=0; retries<maxRetries; retries++ ) {
				if( this.AttemptNextPatchGen(minDistApart, gens) ) {
					retries = 0;

					progress.Value = Math.Min( (float)gens.Count / (float)maxTheoreticalGens, 1f );
				}
			}

			progress.Value = 1f;
		}


		////////////////

		public bool AttemptNextPatchGen( int minDistApart, ISet<(int, int)> allGennedPatches ) {
			int randX = WorldGen.genRand.Next( Main.maxTilesX );
			int randY = WorldGen.genRand.Next( Main.maxTilesY );

			//

			float gradY = CursedBonesPatchesGen.CalculateVerticalGradientPercentAt( randY );
			float invGradY = Math.Max( 1f - gradY, 0f );
			int minMapDim = Main.maxTilesX > Main.maxTilesY
				? Main.maxTilesY
				: Main.maxTilesX;

			minDistApart += (int)((float)minMapDim * invGradY);

			//

			int minDistSqr = minDistApart * minDistApart;

			//

			(int x, int y)? testTile = this.ScanForFirstValidGenTileRadiatingOutward( randX, randY );
			if( !testTile.HasValue ) {
				return false;
			}

			//

			// Any existing gens too close?
			foreach( (int x, int y) in allGennedPatches ) {
				int diffX = testTile.Value.x - x;
				int diffY = testTile.Value.y - y;
				int diffSqr = (diffX * diffX) + (diffY * diffY);

				if( diffSqr < minDistSqr ) {
					return false;
				}
			}

			this.GenPatchAt( testTile.Value.x, testTile.Value.y );

			allGennedPatches.Add( testTile.Value );

			return true;
		}
	}
}