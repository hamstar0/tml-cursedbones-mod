using System;
using System.Collections.Generic;
using Terraria;
using Terraria.World.Generation;
using Terraria.ModLoader;
using CursedBones.Tiles;


namespace CursedBones {
	class CursedBonesWorld : ModWorld {
		public override void PreUpdate() {
			int max = Main.player.Length;
			for( int i=0; i<max; i++ ) {
				Player plr = Main.player[i];
				if( plr?.active != true || plr.dead ) {
					continue;
				}

				CursedBonesTile.RunBonesLaunchersNearby( plr );
			}
		}


		public override void ModifyWorldGenTasks( List<GenPass> tasks, ref float totalWeight ) {
			if( CursedBonesConfig.Instance.CursedBonesWorldGenEnabled ) {
				tasks.Add( new CursedBonesPatchesGen() );
			}
		}
	}
}