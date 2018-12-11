using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dropoin
{
	public class CommandInfo {
		public string  cmdName;
		public string  cmdExe;
		public string  cmdArg;
		public string  outputExt;
		public bool    dropAction;

		public CommandInfo( string cmdName, string cmdExe, string cmdArg ) {
			this.cmdName = cmdName;
			this.cmdExe = cmdExe;
			this.cmdArg = cmdArg;
		}

		public CommandInfo()
			: this( "", "", "" ) {
		}
	}

	

	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			using( var mutex = new System.Threading.Mutex( false, typeof( MainFrame ).ToString() ) ) {
				if( mutex.WaitOne( 0, false ) == false ) {
					//すでに起動していると判断して終了
					MessageBox.Show( "多重起動はできません。", typeof( MainFrame ).ToString() );
					return;
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );
				Application.Run( new MainFrame() );
			}
		}
	}
}
