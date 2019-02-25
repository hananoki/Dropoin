using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsLib;

namespace Dropoin {
	public partial class MainWindow : Form {
	
		//マウスのクリック位置を記憶
		private Point m_mousePoint;

		string  m_msgDropEntry = "ファイルをドロップ";
		string  m_msgDropProcess = "処理を実行中";

		bool dontSetting=false;

		static Config m_settingInstance;

		Config m_config {
			get {
				if( m_settingInstance == null ) {
					Helper.ReadJson( ref m_settingInstance, Helper.m_configPath );
				}
				if( m_settingInstance == null ) {
					m_settingInstance = new Config();
					Debug.Log( "Setting: 新規作成します" );
				}
				return m_settingInstance;
			}
			set {
				m_settingInstance = value;
			}
		}


		int m_listindex {
			get { return m_config.list_idx; }
			set { m_config.list_idx = value; }
		}

		CommandInfo m_currentCommand {
			get { return m_config.m_cmdList[ m_listindex ]; }
		}
		List<CommandInfo> m_commandList {
			get { return m_config.m_cmdList; }
		}
		List<string> m_envList {
			get { return m_config.envList; }
		}


		public MainWindow() {
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
			this.MouseWheel += new MouseEventHandler( this.Form1_MouseWheel );
		}


		/// <summary>
		/// フォームのロード時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load( object sender, EventArgs e ) {
			Font = SystemFonts.IconTitleFont;

			m_pictureBox1.Visible = false;
			m_picReady.Visible = true;

			m_config.RollbackWindow( this );

			if( m_envList.Count == 0 ) {
				m_envList.Add( "" );
				m_envList.Add( "" );
			}

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

			m_config.BackupWindow( this );
			Helper.WriteJson( m_config , Helper.m_configPath);
		}


		void lockDragDrop() {
			Debug.Log( "lockDragDrop" );
			m_picReady.Visible = false;
			Text = m_msgDropProcess;
			m_pictureBox1.Visible = true;
			m_picReady.Visible = false;
			if( m_config.se_start != null ) {
				Win32.PlaySound( m_config.se_start, IntPtr.Zero, Win32.PlaySoundFlags.SND_FILENAME | Win32.PlaySoundFlags.SND_ASYNC );
			}
		}


		void unlockDragDrop() {
			Debug.Log( "unlockDragDrop" );
			m_picReady.Visible = true;
			m_pictureBox1.Visible = false;
			Text = m_msgDropEntry;

			if( m_config.se_finish != null ) {
				Win32.PlaySound( m_config.se_finish, IntPtr.Zero, Win32.PlaySoundFlags.SND_FILENAME | Win32.PlaySoundFlags.SND_ASYNC );
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
			if( m_config.se_select != null ) {
				Win32.PlaySound( m_config.se_select, IntPtr.Zero, Win32.PlaySoundFlags.SND_FILENAME | Win32.PlaySoundFlags.SND_ASYNC );
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

		private void ログToolStripMenuItem_Click( object sender, EventArgs e ) {
			LogWindow.Visible = true;
		}
	}
}
