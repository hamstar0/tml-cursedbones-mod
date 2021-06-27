using Terraria.ID;
using Terraria.ModLoader;


namespace CursedBones.Items.Placeable {
	public class CursedBonesItem : ModItem {
		//public override void SetStaticDefaults() {
		//	ItemID.Sets.SortingPriorityMaterials[ this.item.type ] = 58;
		//}

		public override void SetDefaults() {
			this.item.useStyle = ItemUseStyleID.SwingThrow;
			this.item.useTurn = true;
			this.item.useAnimation = 15;
			this.item.useTime = 10;
			this.item.autoReuse = true;
			this.item.maxStack = 999;
			this.item.consumable = true;
			this.item.createTile = ModContent.TileType<Tiles.CursedBonesTile>();
			this.item.width = 12;
			this.item.height = 12;
			this.item.value = 3000;
		}
	}
}
