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
		private (int x, int y) GetNextPatchTileCandidate( ISet<(int x, int y)> candidates ) {
			var config = CursedBonesConfig.Instance;
			int porousity = config.CursedBonesWorldGenPatchPorousityDegree;

			var candidateCandidates = new Dictionary<int, (int, int)>();
			(int, int)[] candidatesArray = candidates.ToArray();

			for( int i=0; i<porousity; i++ ) {
				int check = WorldGen.genRand.Next( candidatesArray.Length );
				candidateCandidates[ check ] = candidatesArray[ check ];
			}

			return this.PickPriorityPatchTileCandidate( candidateCandidates.Values.ToArray() );
		}

		////

		private (int x, int y) PickPriorityPatchTileCandidate( (int x, int y)[] candidates ) {
			// Prioritize ground tiles
			foreach( (int x, int y) in candidates ) {
				if( this.IsNearValidAttachTile(x, y) ) {
					return (x, y);
				}
			}

			// Prioritize porousity
			foreach( (int x, int y) in candidates ) {
				if( this.CountAdjacentBones(x, y) < 3 ) {
					return (x, y);
				}
			}

			// Prioritize adjacency
			foreach( (int x, int y) in candidates ) {
				if( this.CountAdjacentBones(x, y) >= 1 ) {
					return (x, y);
				}
			}

			return candidates.FirstOrDefault();
		}


		////////////////

		private bool IsNearValidAttachTile( int tileX, int tileY ) {
			if( this.IsValidAttachTile(tileX, tileY-1) ) {
				return true;
			}
			if( this.IsValidAttachTile(tileX-1, tileY) ) {
				return true;
			}
			if( this.IsValidAttachTile(tileX, tileY+1) ) {
				return true;
			}
			if( this.IsValidAttachTile(tileX+1, tileY) ) {
				return true;
			}
			return false;
		}

		private int CountAdjacentBones( int tileX, int tileY ) {
			if( tileX <= 1 || tileX >= Main.maxTilesX-1 || tileY <= 1 || tileY >= Main.maxTilesY-1 ) {
				return 0;
			}

			int bonesTile = ModContent.TileType<CursedBonesTile>();
			int count = 0;

			if( Main.tile[tileX, tileY-1]?.type == bonesTile ) {
				count++;
			}
			if( Main.tile[tileX, tileY+1]?.type == bonesTile ) {
				count++;
			}
			if( Main.tile[tileX-1, tileY]?.type == bonesTile ) {
				count++;
			}
			if( Main.tile[tileX+1, tileY]?.type == bonesTile ) {
				count++;
			}

			return count;
		}
	}
}