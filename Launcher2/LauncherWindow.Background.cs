﻿// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using System.Drawing;
using System.IO;
using ClassicalSharp;
using ClassicalSharp.TexturePack;

namespace Launcher {

	public sealed partial class LauncherWindow {

		internal bool ClassicBackground = false;
		
		internal void TryLoadTexturePack() {
			Options.Load();
			LauncherSkin.LoadFromOptions();
			if( Options.Get( "nostalgia-classicbg" ) != null )
				ClassicBackground = Options.GetBool( "nostalgia-classicbg", false );
			else
				ClassicBackground = Options.GetBool( "mode-classic", false );
			
			string texDir = Path.Combine( Program.AppDirectory, "texpacks" );
			string texPack = Options.Get( OptionsKey.DefaultTexturePack ) ?? "default.zip";
			texPack = Path.Combine( texDir, texPack );
			
			if( !File.Exists( texPack ) )
				texPack = Path.Combine( texDir, "default.zip" );
			if( !File.Exists( texPack ) )
				return;
			
			using( Stream fs = new FileStream( texPack, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
				ZipReader reader = new ZipReader();
				
				reader.ShouldProcessZipEntry = (f) => f == "default.png" || f == "terrain.png";
				reader.ProcessZipEntry = ProcessZipEntry;
				reader.Extract( fs );
			}
		}
		
		bool useBitmappedFont;
		Bitmap terrainBmp;
		FastBitmap terrainPixels;
		int elemSize;
		
		void ProcessZipEntry( string filename, byte[] data, ZipEntry entry ) {
			MemoryStream stream = new MemoryStream( data );
			Bitmap bmp = new Bitmap( stream );
			if( filename == "default.png" ) {
				Drawer.SetFontBitmap( bmp );
				useBitmappedFont = !Options.GetBool( OptionsKey.ArialChatFont, false );
			} else if( filename == "terrain.png" ) {
				FastBitmap src = new FastBitmap( bmp, true );
				elemSize = src.Width / 16;
				
				terrainBmp = new Bitmap( elemSize * 2, elemSize );
				terrainPixels = new FastBitmap( terrainBmp, true );
				FastBitmap.MovePortion( elemSize * 1, 0, elemSize * 0, 0, src, terrainPixels, elemSize );
				FastBitmap.MovePortion( elemSize * 2, 0, elemSize * 1, 0, src, terrainPixels, elemSize );
				
				// Precompute the darkened stone background
				Rectangle rect = new Rectangle( 0, 0, elemSize, elemSize );
				Drawer2DExt.DrawScaledPixels( terrainPixels, terrainPixels, rect.Size, rect, rect, 96, 96 );
			}
		}
		
		public void MakeBackground() {
			if( Framebuffer == null || (Framebuffer.Width != Width || Framebuffer.Height != Height) ) {
				if( Framebuffer != null )
					Framebuffer.Dispose();
				Framebuffer = new Bitmap( Width, Height );
			}
			
			if( ClassicBackground ) {
				using( FastBitmap dst = new FastBitmap( Framebuffer, true ) ) {
					ClearTile( 0, 0, Width, 48, elemSize, 128, 64, dst, true );
					ClearTile( 0, 48, Width, Height - 48, 0, 96, 96, dst, false );
				}
			} else {
				ClearArea( 0, 0, Width, Height );
			}
			
			using( IDrawer2D drawer = Drawer ) {
				drawer.SetBitmap( Framebuffer );

				drawer.UseBitmappedChat = useBitmappedFont;
				DrawTextArgs args = new DrawTextArgs( "&eClassical&fSharp", logoFont, false );
				Size size = drawer.MeasureChatSize( ref args );
				int xStart = Width / 2 - size.Width / 2;
				
				args.Text = "&0Classical&0Sharp";
				drawer.DrawChatText( ref args, xStart + 4, 8 + 4 );
				args.Text = "&eClassical&fSharp";
				drawer.DrawChatText( ref args, xStart, 8 );
				drawer.UseBitmappedChat = false;
			}
			Dirty = true;
		}
		
		public void ClearArea( int x, int y, int width, int height ) {
			using( FastBitmap dst = new FastBitmap( Framebuffer, true ) )
				ClearArea( x, y, width, height, dst );
		}
		
		public void ClearArea( int x, int y, int width, int height, FastBitmap dst ) {
			if( ClassicBackground ) {
				ClearTile( x, y, width, height, 0, 96, 96, dst, false );
			} else {
				FastColour col = LauncherSkin.BackgroundCol;
				Drawer2DExt.DrawNoise( dst, new Rectangle( x, y, width, height ), col, 6 );
			}
		}
		
		void ClearTile( int x, int y, int width, int height, int srcX,
		               byte scaleA, byte scaleB, FastBitmap dst, bool scale ) {
			if( x >= Width || y >= Height ) return;
			Rectangle srcRect = new Rectangle( srcX, 0, elemSize, elemSize );
			const int tileSize = 48;
			Size size = new Size( tileSize, tileSize );
			int xOrig = x, xMax = x + width, yMax = y + height;
			
			for( ; y < yMax; y += tileSize )
				for( x = xOrig; x < xMax; x += tileSize )
			{
				int x2 = Math.Min( x + tileSize, Math.Min( xMax, Width ) );
				int y2 = Math.Min( y + tileSize, Math.Min( yMax, Height ) );
				
				Rectangle dstRect = new Rectangle( x, y, x2 - x, y2 - y );
				if( scale )
					Drawer2DExt.DrawScaledPixels( terrainPixels, dst, size, srcRect, dstRect, scaleA, scaleB );
				else
					Drawer2DExt.DrawScaledPixels( terrainPixels, dst, size, srcRect, dstRect );
			}
		}
	}
}
