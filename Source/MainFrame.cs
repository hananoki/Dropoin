using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Diagnostics;
using System.Reflection;
using System.Linq;

using AppCommon;



namespace Dropoin {
	public partial class MainFrame : Form {
		
		[Flags]
		public enum PlaySoundFlags : int {
			SND_SYNC = 0x0000,
			SND_ASYNC = 0x0001,
			SND_NODEFAULT = 0x0002,
			SND_MEMORY = 0x0004,
			SND_LOOP = 0x0008,
			SND_NOSTOP = 0x0010,
			SND_NOWAIT = 0x00002000,
			SND_ALIAS = 0x00010000,
			SND_ALIAS_ID = 0x00110000,
			SND_FILENAME = 0x00020000,
			SND_RESOURCE = 0x00040004,
			SND_PURGE = 0x0040,
			SND_APPLICATION = 0x0080
		}
		[System.Runtime.InteropServices.DllImport( "winmm.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto )]
		private static extern bool PlaySound( string pszSound, IntPtr hmod, PlaySoundFlags fdwSound );



		internal enum AccentState {
			ACCENT_DISABLED = 0,
			ACCENT_ENABLE_GRADIENT = 1,
			ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
			ACCENT_ENABLE_BLURBEHIND = 3,
			ACCENT_INVALID_STATE = 4
		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct AccentPolicy {
			public AccentState AccentState;
			public int AccentFlags;
			public int GradientColor;
			public int AnimationId;
		}
		//マウスのクリック位置を記憶
		private Point m_mousePoint;

		string  m_msgDropEntry = "ファイルをドロップ";
		string  m_msgDropProcess = "処理を実行中";

		string m_appName;
		string  m_appPath;

		bool dontSetting=false;

		public string CONFIG_PATH {
			get { return $"{m_appPath}\\{m_appName}.json"; }
		}
		//string m_settingInit;

		static Setting m_settingInstance;

		Setting m_setting {
			get {
				if( m_settingInstance == null ) {
					if( File.Exists( CONFIG_PATH ) ) {
						using( var st = new StreamReader( CONFIG_PATH ) ) {
							var ss = st.ReadToEnd();
							if( !string.IsNullOrEmpty( ss ) ) {
								var s = LitJson.JsonMapper.ToObject<Setting>( ss );
								m_settingInstance = s;
								Debug.Log( $"Setting: {CONFIG_PATH}から読み込みました。" );
							}
						}
					}
				}
				if( m_settingInstance == null ) {
					m_settingInstance = new Setting();
					Debug.Log( "Setting: 新規作成します" );
				}
				return m_settingInstance;
			}
			set {
				m_settingInstance = value;
			}
		}


		int m_listindex {
			get { return m_setting.list_idx; }
			set { m_setting.list_idx = value; }
		}

		CommandInfo m_currentCommand {
			get { return m_setting.m_cmdList[ m_listindex ]; }
		}
		List<CommandInfo> m_commandList {
			get { return m_setting.m_cmdList; }
		}
		List<string> m_envList {
			get { return m_setting.envList; }
		}


		public MainFrame() {
			InitializeComponent();

			//BackColor = Color.Black;
			bool DwmEnabled=false;
			//win32api.DwmIsCompositionEnabled( out DwmEnabled );
			if( DwmEnabled == true ) {
				Win32.MARGINS margin;
				margin.leftWidth = -1;
				margin.rightWidth = -1;
				margin.topHeight = 0;
				margin.bottomHeight = 0;
				Win32.DwmExtendFrameIntoClientArea( this.Handle, ref margin );
			}

			//ホイールイベントの追加
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.Form1_MouseWheel );

			//AccentPolicy policy;
			//policy.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
			//policy.AccentFlags = 0;
			//policy.GradientColor= 0;
			//policy.AnimationId = 0;
			//WindowCompositionAttributeData data;
			//data.Attribute=WindowCompositionAttribute.WCA_ACCENT_POLICY;
			//IntPtr sysTimePtr = Marshal.AllocCoTaskMem( Marshal.SizeOf( policy ) );
			//Marshal.StructureToPtr( policy, sysTimePtr, false );
			//data.Data = sysTimePtr;
			//data.SizeOfData = Marshal.SizeOf( policy );
			//SetWindowCompositionAttribute( this.Handle, ref data );
			////タスクバーに表示しない
			//ShowInTaskbar = false;
		}


		/// <summary>
		/// フォームのロード時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load( object sender, EventArgs e ) {
			Font = SystemFonts.IconTitleFont;

			var location = Assembly.GetExecutingAssembly().Location;
			m_appName = location.GetBaseName();

			var exePath = Directory.GetParent( location );
			m_appPath = exePath.FullName;
			Debug.Log( $"{m_appPath} : {m_appName}" );

			m_appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			m_appPath = Path.GetDirectoryName( m_appPath );

#if DEBUG
			Win32.AllocConsole();
#endif

			//if( Debugger.IsAttached ) {
			//	m_appPath = Directory.GetCurrentDirectory();
			//}


			m_pictureBox1.Visible = false;
			m_picReady.Visible = true;

			//

			//if( File.Exists( configPath ) ) {
			//	try {
			//		using( var st = new StreamReader( configPath ) ) {
			//			m_settingInit = st.ReadToEnd();
			//			m_setting = LitJson.JsonMapper.ToObject<Setting>( m_settingInit );
						
			//			//throw new Exception("err");
			//			foreach( var c in m_commandList ) {
			//				console.log( "{0}: {1} {2}".format( c.cmdName, c.cmdExe, c.cmdArg ) );
			//			}
			//		}
			//	}
			//	catch( Exception ee ) {
			//		console.log( ee.ToString() );
			//		m_setting = null;
			//		m_settingInit = null;
			//	}
			//}

			//ディスプレイの高さ
			int disph = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
			//ディスプレイの幅
			int dispw = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
			while( dispw <= m_setting.posx ) {
				m_setting.posx -= dispw;
			}
			if( m_setting.posx <= 0 ) {
				m_setting.posx = 120;
			}
			while( disph <= m_setting.posy ) {
				m_setting.posy -= disph;
			}
			if( m_setting.posy <= 0 ) {
				m_setting.posy = 120;
			}

			this.Location = new Point( m_setting.posx, m_setting.posy );
			if( m_envList.Count == 0 ) {
				m_envList.Add( "" );
				m_envList.Add( "" );
			}


			//if( m_setting == null ) {
			//	var result = MessageBox.Show( "設定ファイルがありませんでした。\n\n> 新規作成しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question );
			//	if( result == DialogResult.Yes ) {
			//		m_setting = new Setting();
			//		m_setting.posx = this.Location.X;
			//		m_setting.posy = this.Location.Y;
			//		m_setting.m_cmdList.Add( new CommandInfo( "", "", "" ) );
			//		m_envList.Add( "" );
			//		m_envList.Add( "" );
			//	}
			//	else {
			//		dontSetting = true;
			//		label2.Text = "使用できません";
			//		label2.ForeColor = Color.Red;
			//	}
			//}

			rt.setEnv( "PATH", "", EnvironmentVariableTarget.Process );
			var paths = m_envList.
				Where( x => !string.IsNullOrEmpty( x ) )
				//.Select( x => Path.GetDirectoryName( x ) )
				.ToArray();

			Helper.SetEnvironmentPath( string.Join( ";", paths ) );

			ShowScriptLabel();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_FormClosing( object sender, FormClosingEventArgs e ) {
			if( dontSetting ) return;

			try {
				using( StreamWriter writer = new StreamWriter( CONFIG_PATH ) ) {
					m_setting.posx = this.Location.X;
					m_setting.posy = this.Location.Y;
					string json = JsonUtils.ToJson( m_setting );
					writer.Write( json );
				}
			}
			catch( Exception ee ) {
				Debug.Log( ee.ToString() );
			}
		}


		void lockDragDrop() {
			Debug.Log( "lockDragDrop" );
			m_picReady.Visible = false;
			Text = m_msgDropProcess;
			m_pictureBox1.Visible = true;
			m_picReady.Visible = false;
			if( m_setting.se_start != null ) {
				PlaySound( m_setting.se_start, IntPtr.Zero, PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_ASYNC );
			}
		}

		void unlockDragDrop() {
			Debug.Log( "unlockDragDrop" );
			m_picReady.Visible = true;
			m_pictureBox1.Visible = false;
			Text = m_msgDropEntry;

			if( m_setting.se_finish != null ) {
				PlaySound( m_setting.se_finish, IntPtr.Zero, PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_ASYNC );
			}
		}


		void runProcess( CommandInfo cmdInfo, string filepath ) {
			using( var p = new System.Diagnostics.Process() ) {
				p.StartInfo.FileName = cmdInfo.cmdExe;
				var args = cmdInfo.cmdArg;
				args = args.replace( "%1", filepath.quote() );
				p.StartInfo.Arguments = args;

				p.StartInfo.UseShellExecute = false;
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.RedirectStandardOutput = true;

				Debug.Log( "{0} {1}".format( p.StartInfo.FileName, p.StartInfo.Arguments ) );
				p.Start();

				string[] stdout = p.StandardOutput.ReadToEnd().Replace( "\r", "" ).Split( '\n' );
				foreach( var line in stdout ) Debug.Log( line );
				p.WaitForExit();
			}
		}

		/// <summary>
		/// メニュー・終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMenuExit( object sender, EventArgs e ) {
			this.Close();
		}

		/// <summary>
		/// マウスホイール操作時の処理を行います
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_MouseWheel( object sender, MouseEventArgs e ) {
			if( m_commandList.Count <= 0 ) return;

			if( 0 < m_commandList.Count ) {
				if( 0 < e.Delta ) {
					m_listindex++;
					if( m_commandList.Count <= m_listindex ) {
						m_listindex = 0;
					}
				}
				else if( e.Delta < 0 ) {
					m_listindex--;
					if( m_listindex < 0 ) {
						m_listindex = m_commandList.Count - 1;
					}
				}
			}
			ShowScriptLabel();
			if( m_setting.se_select != null ) {
				PlaySound( m_setting.se_select, IntPtr.Zero, PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_ASYNC );
			}
		}

		void ShowScriptLabel() {
			if( m_commandList.Count == 0 ) return;
			label2.Text = m_commandList[ m_listindex ].cmdName;
		}

		

		private async void Form1_DragDrop( object sender, DragEventArgs e ) {
			if( m_currentCommand.dropAction == false ) {
				MessageBox.Show(
				"ドロップアクションできないコマンドです",
				"確認",
				MessageBoxButtons.OK, MessageBoxIcon.Warning );
				return;
			}
			lockDragDrop();

			var files = (string[]) e.Data.GetData( DataFormats.FileDrop, false );

			await Task.Factory.StartNew( () => {
				foreach( var file in files ) {
					//executeScript( file );
					using( var p = new System.Diagnostics.Process() ) {
						p.StartInfo.FileName = m_currentCommand.cmdExe;
						var args = m_currentCommand.cmdArg;
						args = args.replace( "%1", file.quote() );
						args = args.replace( @"\$\(filename\)", file.getFileName() );
						args = args.replace( @"\$\(input\)", file );
						p.StartInfo.Arguments = args;

						p.StartInfo.UseShellExecute = false;
						p.StartInfo.CreateNoWindow = true;
						p.StartInfo.RedirectStandardOutput = true;

						Debug.Log( "{0} {1}".format( p.StartInfo.FileName, p.StartInfo.Arguments ) );
						p.Start();
						p.PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle;

						string[] stdout = p.StandardOutput.ReadToEnd().Replace( "\r", "" ).Split( '\n' );
						foreach( var line in stdout ) Debug.Log( line );
						p.WaitForExit();
					}
				}
			} );

			unlockDragDrop();
		}

		private void Form1_DragEnter( object sender, DragEventArgs e ) {
			if( e.Data.GetDataPresent( DataFormats.FileDrop ) ) {
				e.Effect = DragDropEffects.All;
			}
			else {
				e.Effect = DragDropEffects.None;
			}
		}



		/// <summary>
		/// マウスのボタンが押されたときのイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_MouseDown( object sender, MouseEventArgs e ) {
			if( ( e.Button & MouseButtons.Left ) == MouseButtons.Left ) {
				//位置を記憶する
				m_mousePoint = new Point( e.X, e.Y );
			}
		}


		/// <summary>
		/// マウスが動いたときのイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_MouseMove( object sender, MouseEventArgs e ) {
			if( ( e.Button & MouseButtons.Left ) == MouseButtons.Left ) {
				this.Left += e.X - m_mousePoint.X;
				this.Top += e.Y - m_mousePoint.Y;
				//または、つぎのようにする
				//this.Location = new Point(
				//    this.Location.X + e.X - mousePoint.X,
				//    this.Location.Y + e.Y - mousePoint.Y);
			}
		}

		private void label2_Click( object sender, EventArgs e ) {
			DialogResult result = MessageBox.Show(
				"次の動作を実行しようとしています。\n\n> {0}\n\nよろしいですか?".format( m_currentCommand.cmdName, "---------------------------------" ),
				"確認",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question );
			if( DialogResult.Yes == result ) {
				runProcess( m_currentCommand, "" );
			}
		}

		
	}
}
