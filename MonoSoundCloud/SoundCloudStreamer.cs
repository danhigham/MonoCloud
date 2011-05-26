using System;
using Gst.BasePlugins;
using Gst.Interfaces;
using Gst;
using System.Threading;

namespace MonoSoundCloud
{
	public class TimecodeUpdateArgs : EventArgs
	{
		public long ElapsedTime = 0;
		public long Duration = 0;
	}
	
	public class SoundCloudStreamer : System.IDisposable
	{
		private PlayBin2 _playBin;
		private Thread _updateThread;
		private double _initialVolume;
	
		public event EventHandler<TimecodeUpdateArgs> TimecodeUpdated;
		
		public SoundCloudStreamer (double initalVolumeLevel)
		{
			_initialVolume = initalVolumeLevel;
			
			Gst.Application.Init();
		}
		
		public void PlayAudioStream (string url)
		{
			if (_playBin != null) {
				_playBin.SetState(Gst.State.Null);
				_playBin.SetVolume(StreamVolumeFormat.Linear, _initialVolume);
			}
			
			url = String.Format("{0}?client_id={1}", url, SoundCloudRestClient.API_CLIENT_ID);
			Console.WriteLine(url);
			
			_playBin = ElementFactory.Make ("playbin2", "play") as PlayBin2;
			_playBin.Uri = url;
			
			_playBin.Bus.AddWatch ( new BusFunc( (bus, message) => {
		
				switch (message.Type) {
		      		case Gst.MessageType.Error:
		        		Enum err;
		        		string msg;
		        		message.ParseError (out err, out msg);
		        		Console.WriteLine ("Gstreamer error: {0}", msg);
		        		//loop.Quit ();
		        		break;
		      		case Gst.MessageType.Eos:
		       			Console.WriteLine ("Thank you, come again");
				
						//TODO : Move on to next track in the playlist
		        		_playBin.SetState (Gst.State.Null);
		        		break;
		    	}
		
		    	return true;
			 }));
			
			// Play the stream
			_playBin.SetState(Gst.State.Playing);
			
			// Start rendering the waveform
			StartTimecodeUpdate();	
		}
		
		private void StartTimecodeUpdate()
		{
			_updateThread = new Thread(() => {
				do 
				{
					if (_playBin != null)
					{
						long elapsed;
						long duration;
						
						Gst.Format fmt = Gst.Format.Time;
						
						_playBin.QueryPosition(ref fmt, out elapsed);
						_playBin.QueryDuration(ref fmt, out duration);
						
						long elapsedSecs = elapsed / 1000000000;
						long totalSecs = duration / 1000000000;
						
						if (TimecodeUpdated != null)
							TimecodeUpdated(this, new TimecodeUpdateArgs() { ElapsedTime = elapsed, Duration = duration });
						
						Thread.Sleep(500);
					}	
				} while(1==1);	
			});
			
			_updateThread.Start();
		}
		
		private void StopTimecodeUpdate()
		{
			if (_updateThread != null)
				_updateThread.Abort();
		}
		
		public void ChangeVolume(double level)
		{
			if (_playBin != null)
				_playBin.SetVolume (StreamVolumeFormat.Linear, level);
		}
		
		public void Dispose()
		{
			Console.WriteLine("In Dispose()");
			// do any cleaning up here
			
			StopTimecodeUpdate();
			
			// stop the garbage collector from cleaning up twice
			GC.SuppressFinalize(this);
		}
		
		// override the Finalize() method
		~SoundCloudStreamer()
		{
			Console.WriteLine("In Finalize()");
		
			// call the Dispose() method
			Dispose();
		}
		
	}
}

