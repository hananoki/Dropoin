using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dropoin {
	class Setting {
		public int posx;
		public int posy;

		public int list_idx;

		public List<string> envList = new List<string>();

		public string se_start = "";
		public string se_finish = "";
		public string se_select = "";

		public List<CommandInfo> m_cmdList = new List<CommandInfo>();
	}
}
