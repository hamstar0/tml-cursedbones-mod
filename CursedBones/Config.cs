using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;


namespace CursedBones {
	public class CursedBonesConfig : ModConfig {
		public static CursedBonesConfig Instance => ModContent.GetInstance<CursedBonesConfig>();



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;



		////////////////

		public bool DebugModeInfo { get; set; } = false;


		////

		[Range(0, 1000)]
		[DefaultValue(10)]
		public int CursedBonesSkullAttemptsPerTick { get; set; } = 10;

		[Range(0, 500)]
		[DefaultValue(30)]
		public int CursedBonesSkullAttackTileRange { get; set; } = 30;

		[Range(1, 9999)]
		[DefaultValue(10)]
		public int CursedBonesSkullDamage { get; set; } = 10;


		////

		[DefaultValue( true )]
		public bool CursedBonesWorldGenEnabled { get; set; } = true;


		[Range( 0f, 1f )]
		[DefaultValue( 0.35f )]
		public float CursedBonesWorldGenDensityGradientFromStartPercent { get; set; } = 0.35f;

		[Range( 0f, 1f )]
		[DefaultValue( 0.075f )]
		public float CursedBonesWorldGenDensityGradientFromEndPercent { get; set; } = 0.075f;


		[Range( 5, 9999 )]
		[DefaultValue( 100 )]
		public int CursedBonesWorldGenMinimumTileDistanceApart { get; set; } = 100;

		[Range( 1, 1000000 )]
		[DefaultValue( 500 )]
		public int CursedBonesWorldGenMaximumRetriesPerPatchUntilQuit { get; set; } = 500;
		

		[Range( 1, 200 )]
		[DefaultValue( 4 )]
		public int CursedBonesWorldGenPatchMinimumSize {
			get => this._CursedBonesWorldGenPatchMinimumSize;
			set {
				if( value >= this._CursedBonesWorldGenPatchMaximumSize ) {
					this._CursedBonesWorldGenPatchMinimumSize = this._CursedBonesWorldGenPatchMaximumSize - 1;
				} else {
					this._CursedBonesWorldGenPatchMinimumSize = value;
				}
			}
		}

		[Range( 1, 200 )]
		[DefaultValue( 28 )]
		public int CursedBonesWorldGenPatchMaximumSize {
			get => this._CursedBonesWorldGenPatchMaximumSize;
			set {
				if( value <= this._CursedBonesWorldGenPatchMinimumSize ) {
					this._CursedBonesWorldGenPatchMaximumSize = this._CursedBonesWorldGenPatchMinimumSize + 1;
				} else {
					this._CursedBonesWorldGenPatchMaximumSize = value;
				}
			}
		}

		private int _CursedBonesWorldGenPatchMinimumSize = 4;
		private int _CursedBonesWorldGenPatchMaximumSize = 28;


		[Range( 1, 100 )]
		[DefaultValue( 5 )]
		public int CursedBonesWorldGenPatchDensityDegree { get; set; } = 5;
	}
}