using System;
using System.Collections.Generic;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfFiler
{
    interface Attribute<T>
    {
        T Value { get; set; }
        bool SetValue(string value);
        void Default();
        string GetValueString();
    }

    public class Background : Attribute<SolidColorBrush>
    {
        public const string KEY = "Background";
        public SolidColorBrush Value { get; set; }
        public Background()
        {
            Default();
        }
        public virtual void Default()
        {
            Value = Brushes.White;
        }
        /*!
         * \return true if successful, false otherwise
         */
        public bool SetValue(string value)
        {
            Color color = (Color)ColorConverter.ConvertFromString(value);
            if (color != null)
                Value = new SolidColorBrush(color);
            else
                if(Value != null)
                    Default();
            return (color != null);
        }
        public  string GetValueString()
        {
            return Value.ToString();
        }
    }

    public class HoverBackground : Background
    {
        public new const string KEY = "HoverBackground";
        public HoverBackground()
        {
            Default();
        }
        public override void Default()
        {
            Value = Brushes.AliceBlue;
        }
    }
    
    public class DefaultIcon : Attribute<System.Drawing.Icon>
    {
        protected string path;
        protected int index;
        protected uint icon_size;

        protected void SetIcon()
        {
            Value = Win32API.ExtractIcon(path, index, true);
        }
        /*!
         * Setting this property will result in an attempt to load a new icon.
         */
        public uint IconSize
        {
            get { return icon_size; }
            set
            {
                icon_size = value;
                SetIcon();
            }
        }

        public const string KEY = "DefaultIcon";
        public  System.Drawing.Icon Value { get; set; }
        public DefaultIcon()
        {
            Default();
        }
        public virtual void Default()
        {
            path = "shell32.dll";
            index = 1;
            IconSize = 64;
            SetIcon();
        }
        /*!
         * Sets the icon for the class. The icon will have the size set by 'SetSize'
         * or it will default to 64px. Invailid argumants will result in the default icon being set.
         * @param value Expected value syntax: "path index". 
         * \return Returns false when invaild arguments are passed, true otherwise.
         */
        public bool SetValue(string value)
        {
            string[] array = value.Split(' ');
            if((array.Length == 2) &&
                (int.TryParse(array[1], out index)) )
            {
                path = array[0];
                SetIcon();
                if (Value == null)
                {
                    Default();
                    return false;
                }
                else return true;
            }
            else
            {
                Default();
                return false;
            }
        }
        public  string GetValueString()
        {
            return path + " " + index + " " + IconSize;
        }
    }

    public class FolderIcon : DefaultIcon
    {
        public new const string KEY = "FolderIcon";
        public FolderIcon()
        {
            Default();
        }
        public override void Default()
        {
            path = "shell32.dll";
            index = 4;
            IconSize = 64;
            SetIcon();
        }
    }

    public class Attributes
    {
        protected Background background;
        protected DefaultIcon default_icon;
        protected FolderIcon folder_icon;
        protected HoverBackground hover_background;
        
        // Properties to make access easier
        public SolidColorBrush BackgroundBrush
        {
            get { return background.Value; }
        }
        public System.Drawing.Icon DefaultFileIcon
        {
            get { return default_icon.Value; }
        }
        public System.Drawing.Icon DefaultFolderIcon
        {
            get { return folder_icon.Value; }
        }
        public SolidColorBrush HoverBackgroundBrush
        {
            get { return hover_background.Value; }
        }
        public uint IconSize
        {
            get { return default_icon.IconSize; }
            set
            {
                default_icon.IconSize = value;
                folder_icon.IconSize = value;
            }
        }

        /*!
         * Everything is initialized to the default values.
         */
        public Attributes()
        {
            background = new Background();
            default_icon = new DefaultIcon();
            folder_icon = new FolderIcon();
            hover_background = new HoverBackground();
        }
        public string GetAllAttributes()
        {
            return
                Background.KEY + ": " + background.GetValueString() + "\r\n" +
                "; Icons are specified with the following syntax:\r\n" +
                "; Attribute: path-to-icon-file index-of-icon\r\n" +
                DefaultIcon.KEY + ": " + default_icon.GetValueString() + "\r\n" +
                FolderIcon.KEY + ": " + folder_icon.GetValueString() + "\r\n" +
                HoverBackground.KEY + ": " + hover_background.GetValueString() + "\r\n" +
                "IconSize: " + IconSize + "\r\n";
        }
        /*!
         * \return The value of the specified key, or null if the key is not associated with 
         * any attribute.
         */
        public string GetValueString(string key)
        {
            switch (key)
            {
                case Background.KEY:
                    return background.GetValueString();
                case DefaultIcon.KEY:
                    return default_icon.GetValueString();
                case FolderIcon.KEY:
                    return folder_icon.GetValueString();
                case HoverBackground.KEY:
                    return hover_background.GetValueString();
                default:
                    return null;
            }
        }
        /*!
         * Sets an attribute, obviously.
         * @param key Attribute to change. 
         * @param value The new value.
         */
        public bool SetAttribute(string key, string value)
        {
            switch (key)
            {
                case Background.KEY:
                    background.SetValue(value);
                    break;
                case DefaultIcon.KEY:
                    default_icon.SetValue(value);
                    break;
                case FolderIcon.KEY:
                    folder_icon.SetValue(value);
                    break;
                case HoverBackground.KEY:
                    hover_background.SetValue(value);
                    break;
                case "IconSize":
                    uint siz = 0;
                    if (!uint.TryParse(value, out siz))
                        return false;
                    IconSize = siz;
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
