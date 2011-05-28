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
		private string _currentUrl;
		
		public event EventHandler<TimecodeUpdateArgs> TimecodeUpdated;
		public event EventHandler<EventArgs> TrackEnded;
		
		public SoundCloudStreamer (double initalVolumeLevel)
		{
			_initialVolume = initalVolumeLevel;
			
			Gst.Application.Init();
		}
		
		public void Pause(){
			if (_playBin != null) _playBin.SetState(State.Paused);	
		}
		
		public void PlayAudioStream (string url)
		{
			if (_currentUrl == url && _playBin != null) {
				_playBin.SetState(State.Playing);
				return;
			}
			    
			_currentUrl = url;
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
		       			Console.WriteLine ("Stream finished.");
				
		        		_playBin.SetState (Gst.State.Null);
						if (TrackEnded != null)
							TrackEnded (this, new EventArgs());
					
		        		break;
		    	}
		
		    	return true;
			 }));
			
			// Play the stream
			if (_playBin.CurrentState != State.Paused)
				_playBin.SetState(State.Playing);
			
			// Start rendering the waveform
			StartTimecodeUpdate();	
		}
		
		public void StopAudio (){
			if (_playBin != null) _playBin.SetState(State.Null);
		}
		
		private void StartTimecodeUpdate()
		{
			if (_updateThread != null) {
				_updateThread.Abort();
			}
			_updateThread = new Thread(() => {
				
				Console.WriteLine("Starting timecode thread");
				do 
				{
					if (_playBin != null)
					{
						long elapsed;
						long duration;
						
						Gst.Format fmt = Gst.Format.Time;
						
						_playBin.QueryPosition(ref fmt, out elapsed);
						_playBin.QueryDuration(ref fmt, out duration);
						
						//long elapsedSecs = elapsed / 1000000000;
						//long totalSecs = duration / 1000000000;
						
						if (TimecodeUpdated != null) {
							Gdk.Threads.Enter(); //Run the event through the STA thread to update the front end...
							TimecodeUpdated(this, new TimecodeUpdateArgs() { ElapsedTime = elapsed, Duration = duration });
							Gdk.Threads.Leave();
						}
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
			StopAudio();
			
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

