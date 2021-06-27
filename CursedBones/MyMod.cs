using Terraria;
using Terraria.ModLoader;


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
	}
}