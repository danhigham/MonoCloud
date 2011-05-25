using System;
using System.Net;
using Gdk;
using System.IO;
using Mono.Web;
using Mono.Http;
	
namespace MonoSoundCloud
{
	public static class NetHelper
	{
		public static Pixbuf LoadImage (string url)
		{
    		Stream str = null;
    		HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(url);
    		HttpWebResponse wRes = (HttpWebResponse)(wReq).GetResponse();
    		str = wRes.GetResponseStream();

			return new Pixbuf(str);
		}
		
		public static Pixbuf LoadWaveform (string url)
		{
			Console.WriteLine(url);
			Stream str = null;
    		HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(url);
    		HttpWebResponse wRes = (HttpWebResponse)(wReq).GetResponse();
    		str = wRes.GetResponseStream();

			Pixbuf p = new Pixbuf(str);
			
			float ratio = (float)p.Height / (float)p.Width;
			int width = 550;
			int newHeight = (int)(width * ratio);
			
			Pixbuf scaled = p.ScaleSimple(width, newHeight, InterpType.Bilinear);
			
			Pixbuf bg = new Pixbuf(Colorspace.Rgb, true, 8, width, newHeight);
			bg.Fill(0xf46c13FF);
			
			scaled.Composite(bg, 0, 0, width, newHeight, 0, 0, 1, 1, InterpType.Bilinear, 255);
			
			return bg;
		}
		
	}
}

