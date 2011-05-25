using System;
using Gtk;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MonoSoundCloud;
using MonoSoundCloud.Entities;

public partial class MainWindow : Gtk.Window
{
	private ListStore _searchResultStore;
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		
		/* Create object to bind treeview */
		
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
		
		SearchResults.Selection.Changed += delegate(object sender, EventArgs e) {
			TreeSelection selection = sender as TreeSelection;
			TreeIter ti;
			selection.GetSelected(out ti);
			Track t = _searchResultStore.GetValue(ti, 0) as Track;
			
			Waveform.Pixbuf = NetHelper.LoadWaveform(t.waveform_url);
		};
		
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

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
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

