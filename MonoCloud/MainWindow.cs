using System;
using Gtk;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MonoSoundCloud;
using MonoSoundCloud.Entities;

using Pixbuf = Gdk.Pixbuf;
using GtkApplication = Gtk.Application;

public partial class MainWindow : Gtk.Window
{
	private ListStore _searchResultStore;
	private Gdk.Pixbuf _currentWaveform;
	private Track _currentTrack;
	private SoundCloudStreamer _streamer;
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
				
		SetupResultList();
		
		/* Event handler for changing selection in the list */
		
		SearchResults.Selection.Changed += delegate(object sender, EventArgs e) {
			
			TreeSelection selection = sender as TreeSelection;
			TreeIter ti;
			selection.GetSelected(out ti);
			Track t = _searchResultStore.GetValue(ti, 0) as Track;
			_currentTrack = t;
			_currentWaveform = NetHelper.LoadImage(t.waveform_url);
			
			Waveform.Pixbuf = RenderWaveform(_currentWaveform, 0, 0);
			
			if (_streamer == null) {
				_streamer = new SoundCloudStreamer(VolumeControl.Value);
				
				_streamer.TimecodeUpdated+= delegate(object sender2, TimecodeUpdateArgs e2) {
					/* Update the waveform graphic */
					
					Waveform.Pixbuf = RenderWaveform(_currentWaveform, e2.ElapsedTime, e2.Duration);
					
					TimecodeLabel.Text = String.Format("{0} / {1}", TimeString(e2.ElapsedTime), TimeString(e2.Duration));
				};
			}
			
			_streamer.PlayAudioStream(t.stream_url);
		};
		
		
		
		/* Volume change */
		
		VolumeControl.ChangeValue += delegate(object o, ChangeValueArgs args) {
			if (_streamer != null) _streamer.ChangeVolume(VolumeControl.Value);
		};
		
		/* Search event */
		
		SearchButton.Clicked += delegate(object sender, EventArgs e) {
			_searchResultStore.Clear();
			
			SoundCloudRestClient rc = new SoundCloudRestClient();
			List<Track> tracks = rc.SearchCollection<Track>(SearchTextBox.Text, 25);
			
			tracks.ForEach(t => {
				
				TimeSpan duration = TimeSpan.FromSeconds(t.duration / 1000);
				string sDuration = String.Format("{0}:{1}", duration.Minutes.ToString("00"), duration.Seconds.ToString("00"));
				
				_searchResultStore.AppendValues (				                               
				                                t,
				                              	new Gdk.Pixbuf("Images/TreeViewRupertIcon.png"),
				    							t.title,
				    							sDuration,                      
												t.user.username,
				    							PrettyDate(t.created_at),
				                                t.genre
												);
				
			});
		};
		
	}

	private Pixbuf RenderWaveform (Pixbuf original, long elapsed, long total)	
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
		
		if (progressWidth > 0) {
		
			Pixbuf highlight = new Pixbuf(Gdk.Colorspace.Rgb, true, 8, progressWidth, newHeight);
			highlight.Fill(highlightColor);
		
			highlight.Composite(bg, 0, 0, progressWidth, newHeight, 0, 0, 1, 1, Gdk.InterpType.Bilinear, 255);
		
		}
		
		scaled.Composite(bg, 0, 0, width, newHeight, 0, 0, 1, 1, Gdk.InterpType.Bilinear, 255);

		return bg;
	}
		
	private void SetupResultList() 
	{
		/* Play Button */
		TreeViewColumn playButtonColumn = new TreeViewColumn();
		
		CellRendererPixbuf playCell = new CellRendererPixbuf();
		playButtonColumn.PackStart(playCell, true);
		playButtonColumn.AddAttribute(playCell, "pixbuf", 1);
		
		/* Track Title */
		TreeViewColumn trackTitleColumn = new TreeViewColumn();
		trackTitleColumn.Title = "Track Title";
		
		CellRendererText trackCell = new CellRendererText();
		trackTitleColumn.PackStart(trackCell, true);
		trackTitleColumn.AddAttribute(trackCell, "text", 2);
		
		/* Time */
		TreeViewColumn timeColumn = new TreeViewColumn();
		timeColumn.Title = "Duration";
		
		CellRendererText timeCell = new CellRendererText();
		timeColumn.PackStart(timeCell, true);
		timeColumn.AddAttribute(timeCell, "text", 3);
		
		/* User */
		TreeViewColumn userColumn = new TreeViewColumn();
		userColumn.Title = "User";
		
		CellRendererText userCell = new CellRendererText();
		userColumn.PackStart(userCell, true);
		userColumn.AddAttribute(userCell, "text", 4);
		
		/* Created */
		TreeViewColumn createdColumn = new TreeViewColumn();
		createdColumn.Title = "Created";
		
		CellRendererText createdCell = new CellRendererText();
		createdColumn.PackStart(createdCell, true);
		createdColumn.AddAttribute(createdCell, "text", 5);
		
		/* Genre */
		TreeViewColumn genreColumn = new TreeViewColumn();
		genreColumn.Title = "Genre";
		
		CellRendererText genreCell = new CellRendererText();
		genreColumn.PackStart(genreCell, true);
		genreColumn.AddAttribute(genreCell, "text", 6);
		
		SearchResults.AppendColumn(playButtonColumn);
		SearchResults.AppendColumn(trackTitleColumn);
		SearchResults.AppendColumn(userColumn);
		SearchResults.AppendColumn(timeColumn);
		SearchResults.AppendColumn(createdColumn);
		SearchResults.AppendColumn(genreColumn);
			
		_searchResultStore = new ListStore(
		                                   typeof(Track), //the track itself
		                                   typeof(Gdk.Pixbuf), //button
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

