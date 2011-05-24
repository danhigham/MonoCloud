using System;
using System.Collections.Generic;
using MonoSoundCloud;
using MonoSoundCloud.Entities;

namespace MonoSoundCloud.Tests
{
	using NUnit.Framework;
	
	[TestFixture]
	public class APITests
	{
		
		public APITests ()
		{

		}
		
		[Test]
		public void SearchForTrackCollection()
		{
			SoundCloudRestClient _rClient = new SoundCloudRestClient();
			List<Track> tracks = _rClient.SearchCollection<Track>("Goldie", 10);
			Assert.AreEqual(10, tracks.Count);
		}
		
		[Test]
		public void SearchForUserCollection()
		{
			SoundCloudRestClient _rClient = new SoundCloudRestClient();
			List<User> users = _rClient.SearchCollection<User>("NOISIA", 5);
			Assert.AreEqual(5, users.Count);
		}
	}
}

