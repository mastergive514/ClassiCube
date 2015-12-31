﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Threading;
using ClassicalSharp;
using ClassicalSharp.Network;
using OpenTK;
using OpenTK.Graphics;

namespace Launcher2 {

	public sealed partial class LauncherWindow {
		
		/// <summary> Underlying native window instance. </summary>
		public NativeWindow Window;
		
		/// <summary> Platform specific class used to draw 2D elements,
		/// such as text, rounded rectangles and lines. </summary>
		public IDrawer2D Drawer;
		
		/// <summary> Currently active screen. </summary>
		public LauncherScreen Screen;
		
		/// <summary> Whether the client drawing area needs to be redrawn/presented to the screen. </summary>
		public bool Dirty;
		
		/// <summary> Currently active logged in session with classicube.net. </summary>
		public ClassicubeSession Session = new ClassicubeSession();
		
		/// <summary> Queue used to download resources asynchronously. </summary>
		public AsyncDownloader Downloader;
		
		/// <summary> Returns the width of the client drawing area. </summary>
		public int Width { get { return Window.Width; } }
		
		/// <summary> Returns the height of the client drawing area. </summary>
		public int Height { get { return Window.Height; } }
		
		/// <summary> Bitmap that contains the entire array of pixels that describe the client drawing area. </summary>
		public Bitmap Framebuffer;
		
		/// <summary> Whether at the next tick, the launcher window should proceed to stop displaying frames and subsequently exit. </summary>
		public bool ShouldExit;
		
		/// <summary> Contains metadata attached for different screen instances,
		/// typically used to save 'last text entered' text when a screen is disposed. </summary>
		public Dictionary<string, Dictionary<string, object>> ScreenMetadata =
			new Dictionary<string, Dictionary<string, object>>();
		
		internal ResourceFetcher fetcher;
		
		Font logoFont;
		PlatformDrawer platformDrawer;
		public void Init() {
			Window.Resize += Resize;
			Window.FocusedChanged += FocusedChanged;
			Window.WindowStateChanged += Resize;
			logoFont = new Font( "Arial", 30, FontStyle.Regular );
			string path = Assembly.GetExecutingAssembly().Location;
			Window.Icon = Icon.ExtractAssociatedIcon( path );
			                                         
			if( Configuration.RunningOnWindows )
				platformDrawer = new WinPlatformDrawer();
			else if( Configuration.RunningOnX11 )
				platformDrawer = new X11PlatformDrawer();
			else if( Configuration.RunningOnMacOS )
				platformDrawer = new OSXPlatformDrawer();
		}

		void FocusedChanged( object sender, EventArgs e ) {
			MakeBackground();
			Screen.Resize();
		}

		void Resize( object sender, EventArgs e ) {
			platformDrawer.Resize( Window.WindowInfo );
			MakeBackground();
			Screen.Resize();
		}
		
		public void SetScreen( LauncherScreen screen ) {
			if( this.Screen != null )
				this.Screen.Dispose();
			
			MakeBackground();
			this.Screen = screen;
			screen.Init();
		}
		
		public bool ConnectToServer( List<ServerListEntry> publicServers, string hash ) {
			if( String.IsNullOrEmpty( hash ) ) return false;
			
			ClientStartData data = null;
			foreach( ServerListEntry entry in publicServers ) {
				if( entry.Hash == hash ) {
					data = new ClientStartData( Session.Username, entry.Mppass,
					                           entry.IPAddress, entry.Port );
					Client.Start( data, true, ref ShouldExit );
					return true;
				}
			}
			
			// Fallback to private server handling
			try {
				data = Session.GetConnectInfo( hash );
			} catch( WebException ex ) {
				ErrorHandler.LogError( "retrieving server information", ex );
				return false;
			} catch( ArgumentOutOfRangeException ) {
				return false;
			}
			Client.Start( data, true, ref ShouldExit );
			return true;
		}
		
		public void Run() {			
			Window = new NativeWindow( 640, 400, Program.AppName, 0,
			                          GraphicsMode.Default, DisplayDevice.Default );
			Window.Visible = true;
			Drawer = new GdiPlusDrawer2D( null );
			Init();
			TryLoadTexturePack();
			platformDrawer.Init( Window.WindowInfo );
			
			fetcher = new ResourceFetcher();
			fetcher.CheckResourceExistence();
			if( !fetcher.AllResourcesExist ) {
				SetScreen( new ResourcesScreen( this ) );
			} else {
				SetScreen( new MainScreen( this ) );
			}
			
			while( true ) {
				Window.ProcessEvents();
				if( !Window.Exists ) break;
				if( ShouldExit ) {
					if( Screen != null ) 
						Screen.Dispose();
					break;
				}
				
				Screen.Tick();
				if( Dirty || Screen.Dirty )
					Display();
				Thread.Sleep( 1 );
			}
			
			if( Options.Load() ) {
				LauncherSkin.SaveToOptions();
				Options.Save();
			}
			Window.Close();
		}
		
		void Display() {
			Dirty = false;
			Screen.Dirty = false;
			platformDrawer.Draw( Window.WindowInfo, Framebuffer );
		}
		
		public void Dispose() {
			logoFont.Dispose();
		}
	}
}
