using System;
using System.Text;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

using MonoSoundCloud.Entities;

namespace MonoSoundCloud
{
	
	public class SoundCloudRestClient
	{
		public const string REST_END_POINT = "http://api.soundcloud.com";
		public const string API_CLIENT_ID = "428f80dcd293274bd1e0b3374d3fb73a";
		
		public SoundCloudRestClient()
		{
			
		}
		
		public List<T> SearchCollection<T>(string query, int limit)
		{	
			return SearchCollection<T>(query, limit, 1);
		}
	
		public List<T> SearchCollection<T>(string query, int limit, int page)
		{	
			//New collection for results
			var genericObj = Activator.CreateInstance<T>();
			
			if (!(genericObj is ISoundCloudEntity)) return null;
			
			ISoundCloudEntity entity = genericObj as ISoundCloudEntity;	
			string _queryEndPoint = String.Format("{0}{1}", REST_END_POINT, entity.RestPath);

			NameValueCollection data = new NameValueCollection();
			data.Add("q", query);
			data.Add("limit", limit.ToString());
			data.Add("page", page.ToString());
			
			WebResponse response = DoGetRequest(_queryEndPoint, "GET", data);
			Stream s = response.GetResponseStream();
			byte[] content = ReadStream(s);
			
			s.Read(content, 0, content.Length);
			string stringContent = Encoding.UTF8.GetString(content);
			
			return new JavaScriptSerializer().Deserialize<List<T>>(stringContent);
		}
		
		private WebResponse DoGetRequest(string url, string method, NameValueCollection querystring)
		{
			querystring.Add("client_id", API_CLIENT_ID);
			
			String strPost = String.Empty;
    		StreamWriter myWriter = null;
 
			foreach(string key in querystring.Keys) {
				if (strPost == String.Empty) 
					strPost = String.Format("{0}={1}", key, querystring[key]);
				else
					strPost += String.Format("&{0}={1}", key, querystring[key]);
			}	
			
			
			if (method == "GET") url = String.Format("{0}?{1}", url, strPost);
    		Uri processUrl = new Uri(url);
			
    		HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(processUrl);
    		objRequest.Method = method;
    		
			if (method == "POST"){
				objRequest.ContentLength = strPost.Length;
    			objRequest.ContentType = "application/x-www-form-urlencoded";
				
			    try
			    {
			        myWriter = new StreamWriter(objRequest.GetRequestStream());
			        myWriter.Write(strPost);
			    }
			    catch (Exception e)
			    {
			        Debug.WriteLine(e.Message);
			    }
			    finally
			    {
			        myWriter.Close();
			    }
			}
			
			return objRequest.GetResponse();
		}
		
		private byte[] ReadStream(Stream input)
		{
		    byte[] buffer = new byte[16*1024];
		    using (MemoryStream ms = new MemoryStream())
		    {
		        int read;
		        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
		        {
		            ms.Write(buffer, 0, read);
		        }
		        return ms.ToArray();
		    }
		}
	}
}

