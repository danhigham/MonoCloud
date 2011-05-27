/*
 * {
    "id" : 15872486,
    "created_at" : "2011/05/24 21:57:22 +0000",
    "user_id" : 2605073,
    "duration" : 216637,
    "commentable" : true,
    "state" : "finished",
    "sharing" : "public",
    "tag_list" : "",
    "permalink" : "jesus-saves",
    "description" : "",
    "streamable" : true,
    "downloadable" : false,
    "genre" : "",
    "release" : "",
    "purchase_url" : null,
    "label_id" : null,
    "label_name" : "",
    "isrc" : "",
    "video_url" : null,
    "track_type" : "",
    "key_signature" : "",
    "bpm" : null,
    "title" : "Flea Market Creep - Jesus Saves",
    "release_year" : null,
    "release_month" : null,
    "release_day" : null,
    "original_format" : "mp3",
    "license" : "all-rights-reserved",
    "uri" : "https://api.soundcloud.com/tracks/15872486",
    "permalink_url" : "http://soundcloud.com/fleamarketcreep/jesus-saves",
    "artwork_url" : null,
    "waveform_url" : "http://w1.sndcdn.com/UODjq0JLZmXe_m.png",
    "user": {
     "id" : 2605073,
     "permalink" : "fleamarketcreep",
     "username" : "fleamarketcreep",
     "uri" : "https://api.soundcloud.com/users/2605073",
     "permalink_url" : "http://soundcloud.com/fleamarketcreep",
     "avatar_url" : "http://a1.sndcdn.com/images/default_avatar_large.png?b17c165"
     },
    "stream_url" : "https://api.soundcloud.com/tracks/15872486/stream",
    "playback_count" : 0,
    "download_count" : 0,
    "favoritings_count" : 0,
    "comment_count" : 0,
    "attachments_uri" : "https://api.soundcloud.com/tracks/15872486/attachments"
    } */

using System;
namespace MonoSoundCloud.Entities
{
	public class Track : ISoundCloudEntity
	{
		public static string REST_PATH = "/tracks.json";
			
		public string RestPath { get { return REST_PATH; } }

		/* instance properties */
		public int id { get; set; }
		public string title { get; set; }
		public User user { get; set; }
		public string uri { get; set; }
		public string permalink_url { get; set; }
		public string artwork_url { get; set; }
		public string waveform_url { get; set; }
		public string stream_url { get; set; }
		public int duration { get; set; }
		public DateTime created_at { get; set; }
		public string genre { get; set; }
		
		public Track ()
		{
			
		}
	}
}

