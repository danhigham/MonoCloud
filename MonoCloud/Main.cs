using System;
using Gtk;


namespace MonoCloud
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			Gdk.Threads.Init();
			MainWindow win = new MainWindow ();
			win.ShowAll();
			
			Gdk.Threads.Enter();
			Application.Run ();
			Gdk.Threads.Leave();
		}
	}
}

