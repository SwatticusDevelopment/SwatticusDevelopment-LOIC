
using System;
using System.Windows.Forms;

namespace LOIC
{
	static class Program
	{
		[STAThread]
		static void Main(string[] cmdLine)
		{
			bool hive = false, hide = false;
			string ircserver = "", ircport = "", ircchannel = "";

			int count = 0;
			foreach(string s in cmdLine)
			{
				if(s.ToLowerInvariant() == "/hidden") {
					hide = true;
				}

				if(s.ToLowerInvariant() == "/hivemind") {
					hive = true;
					ircserver = cmdLine[count + 1]; 
					try {ircport = cmdLine[count + 2];}
					catch(Exception) {ircport = "6667";} 
					try {ircchannel = cmdLine[count + 3];}
					catch(Exception) {ircchannel = "#loic";} /
				}

				count++;
			}

			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain(hive, hide, ircserver, ircport, ircchannel));
		}
	}
}