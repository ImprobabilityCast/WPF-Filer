using System;
using System.Collections.Generic;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfFiler
{
    public abstract class Attribute<T>
    {
        public const string Key = null;
        public virtual T Value { get; set; }
        public abstract bool SetValue(string value);
        public abstract void Default();
        public abstract string GetValueString();
    }

    public class Background : Attribute<SolidColorBrush>
    {
        public new const string Key = "Background";
        public override SolidColorBrush Value { get; set; }
        public Background()
        {
            Default();
        }
        public override void Default()
        {
            Value = Brushes.White;
        }
        /*!
         * \return true if successful, false otherwise
         */
        public override bool SetValue(string value)
        {
            Color color = (Color)ColorConverter.ConvertFromString(value);
            if (color != null)
                Value = new SolidColorBrush(color);
            else
                if(Value != null)
                    Default();
            return (color != null);
        }
        public override string GetValueString()
        {
            return Value.ToString();
        }
    }

    public class HoverBackground : Background
    {
        public new const string Key = "HoverBackground";
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

        protected void SetIcon()
        {
            Value = Win32API.ExtractIcon(path, index, IconSize, true);
        }
        /*!
         * Setting this property will result in an attempt to load a new icon.
         */
        public uint IconSize
        {
            get { return IconSize; }
            set
            {
                IconSize = value;
                SetIcon();
            }
        }

        public new const string Key = "DefaultIcon";
        public override System.Drawing.Icon Value { get; set; }
        public DefaultIcon()
        {
            Default();
        }
        public override void Default()
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
        public override bool SetValue(string value)
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
        public override string GetValueString()
        {
            return path + " " + index + " " + IconSize;
        }
    }

    public class FolderIcon : DefaultIcon
    {
        public new const string Key = "FolderIcon";
        public FolderIcon()
        {
            path = "shell32.dll";
            index = 4;
            IconSize = 64;
            Default();
        }
    }

    public class Attributes
    {
        private Background background;
        private DefaultIcon default_icon;
        private FolderIcon folder_icon;
        private HoverBackground hover_background;
        
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
                Background.Key + ": " + background.GetValueString() + "\r\n" +
                "; Icons are specified with the following syntax:\r\n" +
                "; Attribute: path-to-icon-file index-of-icon\r\n" +
                DefaultIcon.Key + ": " + default_icon.GetValueString() + "\r\n" +
                FolderIcon.Key + ": " + folder_icon.GetValueString() + "\r\n" +
                HoverBackground.Key + ": " + hover_background.GetValueString() + "\r\n" +
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
                case Background.Key:
                    return background.GetValueString();
                case DefaultIcon.Key:
                    return default_icon.GetValueString();
                case FolderIcon.Key:
                    return folder_icon.GetValueString();
                case HoverBackground.Key:
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
                case Background.Key:
                    background.SetValue(value);
                    break;
                case DefaultIcon.Key:
                    default_icon.SetValue(value);
                    break;
                case FolderIcon.Key:
                    folder_icon.SetValue(value);
                    break;
                case HoverBackground.Key:
                    hover_background.SetValue(value);
                    break;
                default:
                    return false;
            }
            return true;
        }
        public bool SetAttribute(string key, uint value)
        {
            if (key == "IconSize")
            {
                IconSize = value;
                return true;
            }
            else return false;
        }
    }
}
