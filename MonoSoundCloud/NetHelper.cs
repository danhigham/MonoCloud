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
		
		
		
	}
}

