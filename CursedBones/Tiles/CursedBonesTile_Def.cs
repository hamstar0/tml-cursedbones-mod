using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace CursedBones.Tiles {
	public delegate bool CustomItemDrop( int tileX, int tileY );




	public partial class CursedBonesTile : ModTile {
		public const int MaxRealFrames = 4;

		public static int DarkFrames { get; private set; } = 8;

		public static int FrameDuration { get; private set; } = 4;


		////////////////

		private static int CurrentFrame = 0;



		////////////////

		public static int GetRandomFrame( int x, int y ) {
			double randRange = (double)( ( x + ( y * 65536 ) ).ToString().GetHashCode() );
			randRange /= (double)Int32.MaxValue;
			randRange = Math.Abs( randRange );
			double maxFrames = CursedBonesTile.MaxRealFrames + CursedBonesTile.DarkFrames;
			randRange *= maxFrames;

			int myFrame = CursedBonesTile.CurrentFrame + (int)randRange;
			myFrame %= CursedBonesTile.MaxRealFrames + CursedBonesTile.DarkFrames;

			if( myFrame >= CursedBonesTile.MaxRealFrames ) {
				myFrame = CursedBonesTile.MaxRealFrames - 1;
			}

			return myFrame;
		}



		////////////////

		private ISet<CustomItemDrop> CustomItemDrops;



		////////////////
		
		public void AddCustomItemDropHook( CustomItemDrop hook ) {
			var mytile = ModContent.GetInstance<CursedBonesTile>();
			if( mytile.CustomItemDrops == null ) {
				mytile.CustomItemDrops = new HashSet<CustomItemDrop>();
			}

			mytile.CustomItemDrops.Add( hook );
		}


		////////////////

		public override void SetDefaults() {
			Main.tileLighted[ this.Type ] = true;
			Main.tileSolid[ this.Type ] = true;

			ModTranslation name = this.CreateMapEntryName();
			name.SetDefault( "Cursed Bones" );
			this.AddMapEntry( new Color(0, 255, 255), name );

			this.animationFrameHeight = 90;
		}

		public override bool CanExplode( int i, int j ) {
			//return Main.tile[i, j]?.type == ModContent.TileType<CursedBonesTile>();
			return false;
		}


		////////////////

		public override bool Drop( int i, int j ) {
			var mytile = ModContent.GetInstance<CursedBonesTile>();

			foreach( CustomItemDrop hook in mytile.CustomItemDrops ) {
				if( !hook.Invoke(i, j) ) {
					return false;
				}
			}

			return true;
		}


		////////////////

		public override void ModifyLight( int x, int y, ref float r, ref float g, ref float b ) {
			float bright = 0.35f;
			int myFrame = CursedBonesTile.GetRandomFrame( x, y );
			float lit;

			switch( myFrame ) {
			case 1:
				lit = 1f;
				break;
			case 0:
			case 2:
				lit = 0.5f;
				break;
			default:
				lit = 0.15f;
				break;
			}

			r = 0.001f;
			g = bright * lit;
			b = bright * lit;
		}

		////

		public override void AnimateIndividualTile( int type, int x, int y, ref int frameXOffset, ref int frameYOffset ) {
			frameYOffset = CursedBonesTile.GetRandomFrame(x, y) * this.animationFrameHeight;
		}


		////////////////

		public override void AnimateTile( ref int frame, ref int frameCounter ) {
			frameCounter++;

			if( frameCounter >= CursedBonesTile.FrameDuration ) {
				frameCounter = 0;

				CursedBonesTile.CurrentFrame++;

				if( CursedBonesTile.CurrentFrame >= (CursedBonesTile.MaxRealFrames + CursedBonesTile.DarkFrames) ) {
					CursedBonesTile.CurrentFrame = 0;
				}
			}

			frame = CursedBonesTile.CurrentFrame >= CursedBonesTile.MaxRealFrames
				? CursedBonesTile.MaxRealFrames - 1
				: CursedBonesTile.CurrentFrame;
		}
	}
}
