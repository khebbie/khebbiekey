using System;
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Forms;

/// <summary>
/// Create an instance of this class in the constructor of a form. 
/// Will save and restore window state, size, and position.
/// Uses DesktopBounds (instead of just Form.Location/Size) 
/// to place window correctly on a multi screen desktop.
/// http://www.codeproject.com/useritems/FormState.asp
/// </summary>
class FormState
{
    private Form _parent;
    private string _registry_key;

    /// <summary>
    /// Initializes an instance of the FormState class.
    /// </summary>
    /// <param name="parent">
    /// The form to store settings for.
    /// </param>
    /// <param name="sub_key">
    /// Registry path from HKEY_CURRENT_USER to place for storing settings.
    /// Will create a subkey named "FormState".
    /// </param>
    public FormState(Form parent, string subkey)
    {
        this._parent = parent;
        this._registry_key = subkey + "\\FormState";
        this._parent.Load += new EventHandler(On_Load);
        this._parent.FormClosed += new FormClosedEventHandler(On_FormClosed);
    }

    public void SaveValue(string name, object value)
    {
        this.RegKey.SetValue(name, value);
    }

    public object GetValue(string name, object default_value)
    {
        return this.RegKey.GetValue(name, default_value);
    }

    /// <summary>
    /// If for some reason the value stored in reg cannot be parsed to int 
    /// the default_value is returned.
    /// </summary>
    public int GetIntValue(string name, int default_value)
    {
        int val = default_value;
        if (!int.TryParse(this.RegKey.GetValue(name, default_value).ToString(), out val))
            val = default_value;
        return val;
    }

    private RegistryKey RegKey
    {
        get
        {
            return Registry.CurrentUser.CreateSubKey(
              this._registry_key + "\\" + this._parent.Name);
        }
    }

    private void On_Load(object sender, EventArgs e)
    {
        int X, Y, width, height, window_state;

        // place to get settings from
        RegistryKey key = this.RegKey;

        if (!int.TryParse(key.GetValue("DesktopBounds.Width",
          this._parent.DesktopBounds.Width).ToString(),
          out width))
            width = this._parent.DesktopBounds.Width;
        if (!int.TryParse(key.GetValue("DesktopBounds.Height",
          this._parent.DesktopBounds.Height).ToString(),
          out height))
            height = this._parent.DesktopBounds.Height;
        if (!int.TryParse(key.GetValue("DesktopBounds.X",
          this._parent.DesktopBounds.X).ToString(),
          out X))
            X = this._parent.DesktopBounds.X;
        if (!int.TryParse(key.GetValue("DesktopBounds.Y",
          this._parent.DesktopBounds.Y).ToString(),
          out Y))
            Y = this._parent.DesktopBounds.Y;

        // In case of multi screen desktops, check if we got the
        // screen the form was when closed.
        // If not there we put it in upper left corner of nearest 
        // screen.
        // We don't bother checking size (as long as the user see
        // the form ...).
        Rectangle screen_bounds = Screen.GetBounds(new Point(X, Y));
        if (X > screen_bounds.X + screen_bounds.Width)
        {
            X = screen_bounds.X;
            Y = screen_bounds.Y;
        }

        this._parent.DesktopBounds = new Rectangle(X, Y, width, height);

        if (!int.TryParse(key.GetValue("WindowState",
          (int)this._parent.WindowState).ToString(),
          out window_state))
            window_state = (int)this._parent.WindowState;

        this._parent.WindowState = (FormWindowState)window_state;
    }

    private void On_FormClosed(object sender, FormClosedEventArgs e)
    {
        // There may be cases where the event is raised twice.
        // To avoid handling it twice we remove the handler.
        this._parent.FormClosed -= new FormClosedEventHandler(On_FormClosed);
        // TODO: find out why it is raised twice ...

        // place to store settings
        RegistryKey key = this.RegKey;

        // save window state
        key.SetValue("WindowState", (int)this._parent.WindowState);

        // save pos & size in normal window state
        if (this._parent.WindowState != FormWindowState.Normal)
            this._parent.WindowState = FormWindowState.Normal;
        key.SetValue("DesktopBounds.Y", this._parent.DesktopBounds.Y);
        key.SetValue("DesktopBounds.X", this._parent.DesktopBounds.X);
        key.SetValue("DesktopBounds.Width", this._parent.DesktopBounds.Width);
        key.SetValue("DesktopBounds.Height", this._parent.DesktopBounds.Height);
    }
}