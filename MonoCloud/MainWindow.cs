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
		
		/* Track Title */
		TreeViewColumn trackTitleColumn = new TreeViewColumn();
		trackTitleColumn.Title = "Track Title";
		
		CellRendererText trackCell = new CellRendererText();
		trackTitleColumn.PackStart(trackCell, true);
		trackTitleColumn.AddAttribute(trackCell, "text", 0);
		
		/* Time */
		TreeViewColumn timeColumn = new TreeViewColumn();
		timeColumn.Title = "Duration";
		
		CellRendererText timeCell = new CellRendererText();
		timeColumn.PackStart(timeCell, true);
		timeColumn.AddAttribute(timeCell, "text", 1);
		
		/* User */
		TreeViewColumn userColumn = new TreeViewColumn();
		userColumn.Title = "User";
		
		CellRendererText userCell = new CellRendererText();
		userColumn.PackStart(userCell, true);
		userColumn.AddAttribute(userCell, "text", 2);
		
		/* Created */
		TreeViewColumn createdColumn = new TreeViewColumn();
		createdColumn.Title = "Created";
		
		CellRendererText createdCell = new CellRendererText();
		createdColumn.PackStart(createdCell, true);
		createdColumn.AddAttribute(createdCell, "text", 3);
		
		/* Genre */
		TreeViewColumn genreColumn = new TreeViewColumn();
		genreColumn.Title = "Genre";
		
		CellRendererText genreCell = new CellRendererText();
		genreColumn.PackStart(genreCell, true);
		genreColumn.AddAttribute(genreCell, "text", 4);
		
		
		SearchResults.AppendColumn(trackTitleColumn);
		SearchResults.AppendColumn(userColumn);
		SearchResults.AppendColumn(timeColumn);
		SearchResults.AppendColumn(createdColumn);
		SearchResults.AppendColumn(genreColumn);
			
		_searchResultStore = new ListStore(typeof (string), typeof(string), typeof (string), typeof(string), typeof(string));
		SearchResults.Model = _searchResultStore;
		
		SearchButton.Clicked += delegate(object sender, EventArgs e) {
			_searchResultStore.Clear();
			
			SoundCloudRestClient rc = new SoundCloudRestClient();
			string textToSearch = SearchTextBox.Buffer.Text;
			List<Track> tracks = rc.SearchCollection<Track>(textToSearch, 10);
			
			tracks.ForEach(t => {
				
				TimeSpan duration = TimeSpan.FromSeconds(t.duration / 1000);
				string sDuration = String.Format("{0}:{1}", duration.Minutes.ToString("00"), duration.Seconds.ToString("00"));
				
				_searchResultStore.AppendValues (
				    							t.title,
				    							sDuration,                      
												t.user.username,
				    							PrettyDate(t.created_at),
				                                t.genre
												);
				
			});
			
			SearchResults.ShowAll();
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

