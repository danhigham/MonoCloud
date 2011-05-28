using System;
using Gtk;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MonoCloud;
using MonoSoundCloud;
using MonoSoundCloud.Entities;

using Pixbuf = Gdk.Pixbuf;
using GtkApplication = Gtk.Application;

public partial class MainWindow : Gtk.Window
{
	private ListStore _searchResultStore;
	private Gdk.Pixbuf _currentWaveform;
	private Track _currentTrack;
	private TreeIter _currentTreeIter;
	private SoundCloudStreamer _streamer;
	
	private Dictionary<string, Gdk.Pixmap> _buttons;
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();	
		
		LoadButtons();	
		InitButtons();
		
		SetupResultList();
		
		/* Event handler for changing selection in the list */
		
		SearchResults.Selection.Changed += delegate(object sender, EventArgs e) {
			
			TreeSelection selection = sender as TreeSelection;
			TreeIter ti;
			
			selection.GetSelected(out ti);
			_currentTreeIter = ti;
			Track track = _searchResultStore.GetValue(ti, 0) as Track;
			
			if (track != null) DisplayTrackInfo(track);
		};
		
		/* Volume change */
		
		VolumeControl.ChangeValue += delegate(object o, ChangeValueArgs args) {
			if (_streamer != null) _streamer.ChangeVolume(VolumeControl.Value);
		};
		
		/* Search event */
		
		SearchButton.Clicked += delegate(object sender, EventArgs e) {
			_searchResultStore.Clear();
			
			RunSearch(SearchTextBox.Text);
		};
		
		//Run default search
		RunSearch("");
		
	}
	
	private void LoadButtons () 
	{
		_buttons = new Dictionary<string, Gdk.Pixmap>();
		_buttons.Add("play", Gdk.Pixmap.CreateFromXpm(this.GdkWindow, "Images/gtk.xpm"));
		_buttons.Add("play_hover", Gdk.Pixmap.CreateFromXpm(this.GdkWindow, "Images/play_button_hover.xpm"));
		            
	}
	
	private void InitButtons ()
	{
		PlayPauseButton button = new PlayPauseButton(
		                                             "Images/play_button.xpm", //play
		                                             "Images/pause_button.xpm"); //pause
		
		button.Clicked += delegate(object sender, PlayPauseButtonEventArgs e) {
			Console.WriteLine(_currentTreeIter.Stamp);
			
			if (_currentTreeIter.Stamp != 0) {
				if (!e.IsPaused)
					PlayTrack(_currentTreeIter);
				else
				{
					if (_streamer != null)
						_streamer.Pause();
				}
			} else button.SwitchState();
				
		};
		
		XPMButton backButton = new XPMButton("Images/back_button.xpm", 
		                                     "Images/back_button_pushed.xpm");                                       
		                                         
		XPMButton forwardButton = new XPMButton("Images/forward_button.xpm", 
		                                     "Images/forward_button_pushed.xpm");
		
		forwardButton.Clicked+= delegate(object sender, ButtonReleaseEventArgs e) {
			NextTrack();	
		};

		bottomBar.PackStart(backButton, false, false, 0);
		bottomBar.PackStart(button, false, false, 0);
		bottomBar.PackStart(forwardButton, false, false, 0);
	}
	
	private void RunSearch (string query) 
	{
		_currentTrack = null;
		_currentTreeIter.Stamp = 0;
		
		SoundCloudRestClient rc = new SoundCloudRestClient();
		List<Track> tracks = rc.SearchCollection<Track>(query, 25);
		
		tracks.ForEach(t => {
			
			TimeSpan duration = TimeSpan.FromSeconds(t.duration / 1000);
			string sDuration = 
				duration.Hours > 0 ?
								String.Format("{0}:{1}:{2}", duration.Hours.ToString("00"), duration.Minutes.ToString("00"), duration.Seconds.ToString("00")) :
								String.Format("{0}:{1}", duration.Minutes.ToString("00"), duration.Seconds.ToString("00"));
			
			_searchResultStore.AppendValues (				                               
			                                t,
			    							t.title,
			    							sDuration,                      
											t.user.username,
			    							PrettyDate(t.created_at),
			                                t.genre
											);
			
		});
		
		TreeIter iter;
		_searchResultStore.GetIterFirst(out iter);
		
		if (iter.Stamp > 0)
		{
			SearchResults.Selection.SelectIter(iter);
			Track track = _searchResultStore.GetValue(iter, 0) as Track;
			DisplayTrackInfo (track);
		}
	}
	
	private void DisplayTrackInfo (Track track) 
	{
		new Thread(() => {
			TrackInfo.Text = track.title;
			
			if (track.artwork_url != null) {
				string url = track.artwork_url.Replace("-large", "-crop");
				
				Pixbuf artwork = NetHelper.LoadImage(url);
				Artwork.Pixbuf = artwork.ScaleSimple(200, 200, Gdk.InterpType.Bilinear);
			} else {
				Artwork.Pixbuf = null;
			}
			
			if (track.user.avatar_url != null) {
				string url = track.user.avatar_url.Replace("-large", "-badge");
				Avatar.Pixbuf = NetHelper.LoadImage(url);
			} else {
				Avatar.Pixbuf = null;
			}
			
			UploadedBy.Markup = UIHelper.SmallText(String.Format("Uploaded by {0}\n{1}", track.user.username, track.created_at.ToShortDateString()));
			
			
			//Waveform.Pixbuf = RenderWaveform(_currentWaveform, 0, 0, track == _currentTrack);
		}).Start();
	}
	
	private void NextTrack()
	{
		if (_currentTreeIter.Stamp != 0)
			if (_searchResultStore.IterNext(ref _currentTreeIter))
				PlayTrack (_currentTreeIter);
	}
	
	private void PreviousTrack()
	{
		if (_currentTreeIter.Stamp != 0) {
			//if (_searchResultStore.	
		}
			
	}
	
	private void PlayTrack(TreeIter treeIter)
	{
		Track track = _searchResultStore.GetValue(treeIter, 0) as Track;
		_currentTrack = track;
		_currentWaveform = NetHelper.LoadImage(track.waveform_url);
		
		Status.Text = String.Format("Now playing :- {0}", track.title);
		
		/* Change selection on search result list */
		SearchResults.Selection.SelectIter(treeIter);
		
		if (_streamer == null) {
			Console.WriteLine("Creating streamer");
			try
			{
				_streamer = new SoundCloudStreamer(VolumeControl.Value);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return;
			}
			
			_streamer.TimecodeUpdated+= delegate(object sender2, TimecodeUpdateArgs e2) {
				/* Update the waveform graphic */
				
				Waveform.Pixbuf = RenderWaveform(_currentWaveform, e2.ElapsedTime, e2.Duration, true);
				
				TimecodeLabel.Text = String.Format("{0} / {1}", TimeString(e2.ElapsedTime), TimeString(e2.Duration));
			};
			
			_streamer.TrackEnded+= delegate(object sender2, EventArgs e2) {
				
				//Move on to the next track
				if (_searchResultStore.IterNext(ref _currentTreeIter))
					PlayTrack (_currentTreeIter);
			};
		}
		
		_streamer.PlayAudioStream(_currentTrack.stream_url);
				
	}
	
	private Pixbuf RenderWaveform (Pixbuf original, long elapsed, long total, bool showProgress)	
	{
		uint highlightColor = 0xf46c13FF;
		uint lowlightColor = 0xc9c9c9FF;
		
		Pixbuf p = original;
		
		float ratio = (float)p.Height / (float)p.Width;
		int width = 500;
		
		float progressRatio =  (float)elapsed / (float)total;
		
		int newHeight = (int)(width * ratio);
		
		int progressWidth = (int)(width * progressRatio);
		
		Pixbuf scaled = p.ScaleSimple(width, newHeight, Gdk.InterpType.Bilinear);

		Pixbuf bg = new Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, newHeight);
		bg.Fill(lowlightColor);
		
		if (progressWidth > 0 && showProgress) {
		
			Pixbuf highlight = new Pixbuf(Gdk.Colorspace.Rgb, true, 8, progressWidth, newHeight);
			highlight.Fill(highlightColor);
		
			highlight.Composite(bg, 0, 0, progressWidth, newHeight, 0, 0, 1, 1, Gdk.InterpType.Bilinear, 255);
		
		}
		
		scaled.Composite(bg, 0, 0, width, newHeight, 0, 0, 1, 1, Gdk.InterpType.Bilinear, 255);

		return bg;
	}
		
	private void SetupResultList() 
	{
		/* Track Title */
		TreeViewColumn trackTitleColumn = new TreeViewColumn();
		trackTitleColumn.Title = "Track Title";
		
		CellRendererText trackCell = new CellRendererText();
		trackTitleColumn.PackStart(trackCell, true);
		trackTitleColumn.AddAttribute(trackCell, "text", 1);
		
		/* Time */
		TreeViewColumn timeColumn = new TreeViewColumn();
		timeColumn.Title = "Duration";
		
		CellRendererText timeCell = new CellRendererText();
		timeColumn.PackStart(timeCell, true);
		timeColumn.AddAttribute(timeCell, "text", 2);
		
		/* User */
		TreeViewColumn userColumn = new TreeViewColumn();
		userColumn.Title = "User";
		
		CellRendererText userCell = new CellRendererText();
		userColumn.PackStart(userCell, true);
		userColumn.AddAttribute(userCell, "text", 3);
		
		/* Created */
		TreeViewColumn createdColumn = new TreeViewColumn();
		createdColumn.Title = "Created";
		
		CellRendererText createdCell = new CellRendererText();
		createdColumn.PackStart(createdCell, true);
		createdColumn.AddAttribute(createdCell, "text", 4);
		
		/* Genre */
		TreeViewColumn genreColumn = new TreeViewColumn();
		genreColumn.Title = "Genre";
		
		CellRendererText genreCell = new CellRendererText();
		genreColumn.PackStart(genreCell, true);
		genreColumn.AddAttribute(genreCell, "text", 5);
		
		SearchResults.AppendColumn(trackTitleColumn);
		SearchResults.AppendColumn(userColumn);
		SearchResults.AppendColumn(timeColumn);
		SearchResults.AppendColumn(createdColumn);
		SearchResults.AppendColumn(genreColumn);
			
		_searchResultStore = new ListStore(
		                                   typeof(Track), //the track itself
		                                   typeof(string), //title
		                                   typeof(string), //duration
		                                   typeof(string), //username
		                                   typeof(string), //date_created
		                                   typeof(string)); // genre
		
		SearchResults.Model = _searchResultStore;
	}
	
	private string TimeString (long t) {
	    long secs = t / 1000000000;
	    int mins = (int) (secs / 60);
	    secs = secs - (mins * 60);
	
	    if (mins >= 60) {
	      int hours = (int) (mins / 60);
	      mins = mins - (hours * 60);
	
	      return string.Format ("{0}:{1:d2}:{2:d2}", hours, mins, secs);
	    }
	
	    return string.Format ("{0}:{1:d2}", mins, secs);
	  }
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		if (_streamer != null)
			_streamer.Dispose();
		
		GtkApplication.Quit ();
		a.RetVal = true;
	}
	
	private string PrettyDate(DateTime date)
	{
	    DateTime Now = DateTime.Now;
	    TimeSpan Diff = Now - date;
	 	string prettyDate;
		
	    if (Diff.Seconds <= 0)
	    {
	        prettyDate = "just now";
	    } 
	    else if (Diff.Days > 30)
	    {
	        prettyDate = Diff.Days / 30 + " month" + (Diff.Days / 30 >= 2 ? "s " : " ") + "ago";
	    }
	    else if (Diff.Days > 7)
	    {
	        prettyDate = Diff.Days / 7 + " week" + (Diff.Days / 7 >= 2 ? "s " : " ") + "ago";
	    }
	    else if (Diff.Days >= 1)
	    {
	        prettyDate = Diff.Days + " day" + (Diff.Days >= 2 ? "s " : " ") + "ago";
	    }
	    else if (Diff.Hours >= 1)
	    {
	        prettyDate = Diff.Hours + " hour" + (Diff.Hours >= 2 ? "s " : " ") + "ago";
	    }
	    else if (Diff.Minutes >= 1)
	    {
	        prettyDate = Diff.Minutes + " minute" + (Diff.Minutes >= 2 ? "s " : " ") + "ago";
	    }
	    else
	    {
	        prettyDate = Diff.Seconds + " second" + (Diff.Seconds >= 2 ? "s " : " ") + "ago";
	    }
	    return prettyDate;
	}

}

