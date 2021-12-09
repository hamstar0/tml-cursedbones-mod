using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace CursedBones {
	class CursedBonesItem : GlobalItem {
		public override void ModifyTooltips( Item item, List<TooltipLine> tooltips ) {
			if( item.pick > 0 ) {
				var config = CursedBonesConfig.Instance;
				int pickPow = config.Get<int>( nameof(config.CursedBonesTilePickaxeStrength) );

				if( pickPow > 0 && item.pick >= pickPow ) {
					string modname = "[c/FFFF88:Cursed Bones] - ";

					var tip = new TooltipLine( this.mod, "CursedBonesPickaxe", modname+"Able to mine Cursed Bones" );

					tooltips.Add( tip );
				}
			}
		}
	}
}