using System.IO;
using Terraria;
using Terraria.ModLoader;
using CursedBones.Tiles;


namespace CursedBones {
	public class CursedBonesMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-cursedbones-mod";


		////////////////

		public static CursedBonesMod Instance { get; private set; }



		////////////////

		public override void Load() {
			CursedBonesMod.Instance = this;
		}

		public override void Unload() {
			CursedBonesMod.Instance = null;
		}


		////////////////

		public override void HandlePacket( BinaryReader reader, int whoAmI ) {
			CursedBonesTile.InitializeSkullProjectileStats( reader.ReadInt32() );
		}
	}
}