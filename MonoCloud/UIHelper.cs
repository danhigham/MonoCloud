using System;
using System.Text;
using Gdk;
using Gtk;

namespace MonoCloud
{
	public static class UIHelper
	{
		public static string SmallText(string caption)
		{
			return String.Format("<span size=\"small\">{0}</span>", caption);		
		}
		
	}
	
	public class PlayPauseButton : EventBox 
	{
		private Pixbuf _playImage;
		private Pixbuf _pauseImage;
		private Gtk.Image _image;
		private bool _paused;
		
		public PlayPauseButton(string playXPM, string pauseXPM)
		{
			_paused = false;
			_playImage = new Pixbuf(playXPM);
			_pauseImage = new Pixbuf(pauseXPM);
            
            _image = new Gtk.Image();
			_image.Pixbuf = _playImage;
            
			Add(_image);
            _image.Show();
			
			this.ButtonPressEvent+=	 delegate(object o, ButtonPressEventArgs args) {
				if (!_paused)
					_image.Pixbuf = _pauseImage;
				else 
					_image.Pixbuf = _playImage;
				
				_paused = !_paused;
			};	
				
			this.WidthRequest = _image.Pixbuf.Width;
		}
	}
	
	public class XPMButton : EventBox 
	{
		private Pixbuf _defaultImage;
		private Pixbuf _pressedImage;

		private Gtk.Image _image;
		
		public XPMButton(string defaultXPM, string pressedXPM)
		{
			_defaultImage = new Pixbuf(defaultXPM);
			_pressedImage = new Pixbuf(pressedXPM);
            
            _image = new Gtk.Image();
			_image.Pixbuf = _defaultImage;
            
			Add(_image);
            _image.Show();
			
			this.ButtonPressEvent+=	 delegate(object o, ButtonPressEventArgs args) {
				_image.Pixbuf = _pressedImage;
			};	
			
			this.ButtonReleaseEvent+= delegate(object o, ButtonReleaseEventArgs args) {
				_image.Pixbuf = _defaultImage;
			};
			
			this.WidthRequest = _image.Pixbuf.Width;
		}
	}
}

