
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.VBox vbox2;

	private global::Gtk.HBox hbox5;

	private global::Gtk.Label label2;

	private global::Gtk.HScale hscale3;

	private global::Gtk.HBox hbox4;

	private global::Gtk.Entry SearchTextBox;

	private global::Gtk.Button SearchButton;

	private global::Gtk.HPaned hpaned3;

	private global::Gtk.ScrolledWindow GtkScrolledWindow1;

	private global::Gtk.TreeView SearchResults;

	private global::Gtk.Fixed fixed6;

	private global::Gtk.Image Waveform;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox2 = new global::Gtk.VBox ();
		this.vbox2.Name = "vbox2";
		this.vbox2.Spacing = 2;
		this.vbox2.BorderWidth = ((uint)(3));
		// Container child vbox2.Gtk.Box+BoxChild
		this.hbox5 = new global::Gtk.HBox ();
		this.hbox5.HeightRequest = 36;
		this.hbox5.Name = "hbox5";
		this.hbox5.Spacing = 6;
		this.hbox5.BorderWidth = ((uint)(3));
		// Container child hbox5.Gtk.Box+BoxChild
		this.label2 = new global::Gtk.Label ();
		this.label2.Name = "label2";
		this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Volume");
		this.hbox5.Add (this.label2);
		global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox5[this.label2]));
		w1.Position = 0;
		w1.Expand = false;
		w1.Fill = false;
		// Container child hbox5.Gtk.Box+BoxChild
		this.hscale3 = new global::Gtk.HScale (null);
		this.hscale3.WidthRequest = 80;
		this.hscale3.CanFocus = true;
		this.hscale3.Name = "hscale3";
		this.hscale3.Adjustment.Upper = 100;
		this.hscale3.Adjustment.PageIncrement = 10;
		this.hscale3.Adjustment.StepIncrement = 1;
		this.hscale3.DrawValue = false;
		this.hscale3.Digits = 0;
		this.hscale3.ValuePos = ((global::Gtk.PositionType)(2));
		this.hbox5.Add (this.hscale3);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox5[this.hscale3]));
		w2.Position = 1;
		// Container child hbox5.Gtk.Box+BoxChild
		this.hbox4 = new global::Gtk.HBox ();
		this.hbox4.HeightRequest = 40;
		this.hbox4.Name = "hbox4";
		this.hbox4.Spacing = 5;
		this.hbox4.BorderWidth = ((uint)(2));
		// Container child hbox4.Gtk.Box+BoxChild
		this.SearchTextBox = new global::Gtk.Entry ();
		this.SearchTextBox.CanFocus = true;
		this.SearchTextBox.Name = "SearchTextBox";
		this.SearchTextBox.IsEditable = true;
		this.SearchTextBox.InvisibleChar = '•';
		this.hbox4.Add (this.SearchTextBox);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox4[this.SearchTextBox]));
		w3.Position = 0;
		// Container child hbox4.Gtk.Box+BoxChild
		this.SearchButton = new global::Gtk.Button ();
		this.SearchButton.CanFocus = true;
		this.SearchButton.Events = ((global::Gdk.EventMask)(512));
		this.SearchButton.Name = "SearchButton";
		this.SearchButton.UseUnderline = true;
		// Container child SearchButton.Gtk.Container+ContainerChild
		global::Gtk.Alignment w4 = new global::Gtk.Alignment (0.5f, 0.5f, 0f, 0f);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w5 = new global::Gtk.HBox ();
		w5.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w6 = new global::Gtk.Image ();
		w6.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "search", global::Gtk.IconSize.Menu);
		w5.Add (w6);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w8 = new global::Gtk.Label ();
		w8.LabelProp = global::Mono.Unix.Catalog.GetString ("Search");
		w8.UseUnderline = true;
		w5.Add (w8);
		w4.Add (w5);
		this.SearchButton.Add (w4);
		this.hbox4.Add (this.SearchButton);
		global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox4[this.SearchButton]));
		w12.Position = 1;
		w12.Expand = false;
		w12.Fill = false;
		this.hbox5.Add (this.hbox4);
		global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox5[this.hbox4]));
		w13.Position = 2;
		this.vbox2.Add (this.hbox5);
		global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hbox5]));
		w14.Position = 0;
		w14.Expand = false;
		w14.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.hpaned3 = new global::Gtk.HPaned ();
		this.hpaned3.CanFocus = true;
		this.hpaned3.Name = "hpaned3";
		this.hpaned3.Position = 442;
		this.hpaned3.BorderWidth = ((uint)(3));
		// Container child hpaned3.Gtk.Paned+PanedChild
		this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
		this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
		this.SearchResults = new global::Gtk.TreeView ();
		this.SearchResults.CanFocus = true;
		this.SearchResults.Name = "SearchResults";
		this.SearchResults.SearchColumn = 2;
		this.GtkScrolledWindow1.Add (this.SearchResults);
		this.hpaned3.Add (this.GtkScrolledWindow1);
		global::Gtk.Paned.PanedChild w16 = ((global::Gtk.Paned.PanedChild)(this.hpaned3[this.GtkScrolledWindow1]));
		w16.Resize = false;
		// Container child hpaned3.Gtk.Paned+PanedChild
		this.fixed6 = new global::Gtk.Fixed ();
		this.fixed6.Name = "fixed6";
		this.fixed6.HasWindow = false;
		this.hpaned3.Add (this.fixed6);
		this.vbox2.Add (this.hpaned3);
		global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hpaned3]));
		w18.Position = 1;
		// Container child vbox2.Gtk.Box+BoxChild
		this.Waveform = new global::Gtk.Image ();
		this.Waveform.HeightRequest = 80;
		this.Waveform.Name = "Waveform";
		this.vbox2.Add (this.Waveform);
		global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.Waveform]));
		w19.Position = 2;
		w19.Expand = false;
		w19.Fill = false;
		w19.Padding = ((uint)(3));
		this.Add (this.vbox2);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 592;
		this.DefaultHeight = 460;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
	}
}
