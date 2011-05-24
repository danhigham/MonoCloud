/*
 *  {
    "id" : 4891667,
    "permalink" : "vtiburcio",
    "username" : "Vtiburcio",
    "uri" : "https://api.soundcloud.com/users/4891667",
    "permalink_url" : "http://soundcloud.com/vtiburcio",
    "avatar_url" : "http://i1.sndcdn.com/avatars-000003847371-zyf7fo-large.jpg?b17c165",
    "country" : "United States",
    "full_name" : "Veronica Tiburcio",
    "city" : "Lowell",
    "description" : null,
    "discogs_name" : null,
    "myspace_name" : null,
    "website" : null,
    "website_title" : null,
    "online" : true,
    "track_count" : 0,
    "playlist_count" : 0,
    "followers_count" : 0,
    "followings_count" : 0,
    "public_favorites_count" : 3
    } */

using System;
namespace MonoSoundCloud.Entities
{
	public class User : ISoundCloudEntity
	{
		public static string REST_PATH = "/users.json";
			
		public string RestPath { get { return REST_PATH; } }
	
		public int id { get; set; }
		public string username { get; set; }
		public string avatar_url { get; set; }
		public string full_name { get; set; }
		public string uri { get; set; }
		
		public User ()
		{
			
		}
	}
}

