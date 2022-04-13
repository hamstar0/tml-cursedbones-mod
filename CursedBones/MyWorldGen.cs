using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;


namespace CursedBones {
	partial class CursedBonesPatchesGen : GenPass {
		public static float CalculatePatchIntensityPercentAt( int tileY ) {
			var config = ModContent.GetInstance<CursedBonesConfig>();

			float gradBegWldPerc = config.Get<float>(
				nameof(config.CursedBonesPatchFrequencyTopGradientPercent)
			);
			float gradEndWldPerc = config.Get<float>(
				nameof(config.CursedBonesPatchFrequencyBotGradientPercent)
			);

			//

			int topGradTileRange = (int)((float)Main.maxTilesY * gradBegWldPerc);
			int topGradBegTileY = 0;
			int topGradEndTileY = topGradTileRange;

			int botGradTileRange = (int)((float)Main.maxTilesY * gradEndWldPerc);
			int botGradBegTileY = Main.maxTilesY - botGradTileRange;
			int botGradEndTileY = Main.maxTilesY - 1;

			//

			float tilePercInGradRange = config.Get<float>(
				nameof(config.CursedBonesPatchFrequencyGradientPercentPeak)
			);

			//

			if( tileY < topGradEndTileY ) {
				int gradRange = topGradEndTileY - topGradBegTileY;
				int tileInGradRange = tileY - topGradBegTileY;

				tilePercInGradRange *= (float)tileInGradRange / (float)gradRange;
			} else if( tileY >= botGradBegTileY ) {
				int gradRange = botGradEndTileY - botGradBegTileY;
				int tileInGradRange = tileY - botGradBegTileY;

				float invTilePercInGradRange = (float)tileInGradRange / (float)gradRange;
				tilePercInGradRange *= 1f - invTilePercInGradRange;
			}

			return tilePercInGradRange;
		}



		////////////////

		public CursedBonesPatchesGen() : base( "Cursed Bone Patches", 1f ) { }


		public override void Apply( GenerationProgress progress ) {
			var config = CursedBonesConfig.Instance;
			int minDistApart = config.Get<int>( nameof(config.CursedBonesWorldGenMinimumTileDistanceApart) );
			int maxRetries = config.Get<int>( nameof(config.CursedBonesWorldGenMaximumRetriesPerPatchUntilQuit) );

			int maxTheoreticalGens = (Main.maxTilesX * Main.maxTilesY) / (minDistApart * minDistApart);
			maxTheoreticalGens /= 10;

			//

			var gens = new HashSet<(int, int)>();

			for( int retries=0; retries<maxRetries; retries++ ) {
				bool success = this.AttemptNextPatchGen_If( minDistApart, gens );
				if( !success ) {
					continue;
				}

				//

				retries = 0;

				progress.Value = Math.Min( (float)gens.Count / (float)maxTheoreticalGens, 1f );
			}

			//

			progress.Value = 1f;
		}


		////////////////

		public bool AttemptNextPatchGen_If( int minDistApart, ISet<(int, int)> allGennedPatches ) {
			int randX = WorldGen.genRand.Next( Main.maxTilesX );
			int randY = WorldGen.genRand.Next( Main.maxTilesY );

			//

			(int x, int y)? testTile = this.ScanForFirstValidGenTileRadiatingOutward( randX, randY );
			if( !testTile.HasValue ) {
				return false;
			}

			//

			float gradY = CursedBonesPatchesGen.CalculatePatchIntensityPercentAt( randY );
			float invGradY = Math.Max( 1f - gradY, 0f );

			int minMapDim = Main.maxTilesX > Main.maxTilesY
				? Main.maxTilesY
				: Main.maxTilesX;

			// Patches are closer together with depth
			minDistApart += (int)((float)minMapDim * invGradY);

			//

			int minDistSqr = minDistApart * minDistApart;

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

			//

			this.GenPatchAt( testTile.Value.x, testTile.Value.y );

			allGennedPatches.Add( testTile.Value );

			//

			return true;
		}
	}
}