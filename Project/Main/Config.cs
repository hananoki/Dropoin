using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using CsLib;

namespace Dropoin {
	class Config {
		public int posx;
		public int posy;

		public int list_idx;

		public List<string> envList = new List<string>();

		public string se_start = "";
		public string se_finish = "";
		public string se_select = "";

		public List<CommandInfo> m_cmdList = new List<CommandInfo>();

		public void RollbackWindow( Control window ) {
			//ディスプレイの高さ
			int disph = Screen.PrimaryScreen.Bounds.Height;
			//ディスプレイの幅
			int dispw = Screen.PrimaryScreen.Bounds.Width;
			while( dispw <= posx ) {
				posx -= dispw;
			}
			if( posx <= 0 ) {
				posx = 120;
			}
			while( disph <= posy ) {
				posy -= disph;
			}
			if( posy <= 0 ) {
				posy = 120;
			}

			window.Location = new Point( posx, posy );
		}

		public void BackupWindow( Control window ) {
			posx = window.Location.X;
			posy = window.Location.Y;
		}
	}
}
