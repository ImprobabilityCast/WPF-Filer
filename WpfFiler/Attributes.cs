using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfFiler
{
    interface IAttribute<T>
    {
        T Value { get; set; }
        bool SetFromString(string value);
    }

    public class Background : IAttribute<SolidColorBrush>
    {
        public SolidColorBrush Value { get; set; }
        public Background()
        {
            Value = Brushes.White;
        }
        public Background(SolidColorBrush brush)
        {
            Value = brush;
        }

        /*!
         * \return true if successful, false otherwise
         */
        public bool SetFromString(string value)
        {
            Color color = (Color) ColorConverter.ConvertFromString(value);
            if (color != null)
                Value = new SolidColorBrush(color);
            return (color != null);
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    
    public class DefaultIcon : IAttribute<System.Drawing.Icon>
    {
        protected string path;
        protected int index;
        protected uint icon_size;

        protected bool SetIcon()
        {
            System.Drawing.Icon ico = Win32API.ExtractIcon(path, index, true);
            bool result = (ico != null);
            if (result)
                Value = ico;
            return result;
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
        
        public System.Drawing.Icon Value { get; set; }
        public DefaultIcon()
        {
            Default();
        }
        public virtual void Default()
        {
            path = "shell32.dll";
            index = 0;
            IconSize = 64;
            SetIcon();
        }

        /*!
         * Sets the icon for the class. The icon will have the size set by 'SetSize'
         * or it will default to 64px. Invailid argumants will result in the default icon being set.
         * @param value Expected value syntax: "path index". 
         * \return Returns false when invaild arguments are passed, true otherwise.
         */
        public bool SetFromString(string value)
        {
            string[] array = value.Split(' ');
            if((array.Length == 2) && int.TryParse(array[1], out index))
            {
                path = array[0];
                if (!SetIcon())
                    Default();
                else return true;
            }
            else
                Default();
            return false;
        }

        public override string ToString()
        {
            return path + " " + index;
        }
    }

    public class FolderIcon : DefaultIcon
    {
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
        public Background background;
        public DefaultIcon default_icon;
        public FolderIcon folder_icon;
        public Background hover_background;
        
        
        /*!
         * Everything is initialized to the default values.
         */
        public Attributes()
        {
            background = new Background();
            hover_background = new Background(Brushes.AliceBlue);
            default_icon = new DefaultIcon();
            folder_icon = new FolderIcon();
        }

        public string[] GetAllAttributes()
        {
            return new string[] {
                "Background: " + background,
                "DefaultIcon: " + default_icon,
                "FolderIcon: " + folder_icon,
                "HoverBackground: " + hover_background,
                "IconSize: " + default_icon.IconSize
            };
        }

        /*!
         * \return The value of the specified key, or an empty string if the key
         * is not associated with any attribute.
         */
        public string GetValueString(string key)
        {
            string result;
            switch (key)
            {
                case "Background":
                    result = background.ToString();
                    break;
                case "DefaultIcon":
                    result = default_icon.ToString();
                    break;
                case "FolderIcon":
                    result = folder_icon.ToString();
                    break;
                case "HoverBackground":
                    result = hover_background.ToString();
                    break;
                case "IconSize":
                    result = default_icon.IconSize.ToString();
                    break;
                default:
                    result = "";
                    break;
            }
            return result;
        }

        /*!
         * Sets an attribute, obviously.
         * @param key Attribute to change. 
         * @param value The new value.
         */
        public bool SetAttribute(string key, string value)
        {
            bool result = false;
            switch (key)
            {
                case "Background":
                    result = background.SetFromString(value);
                    break;
                case "DefaultIcon":
                    result = default_icon.SetFromString(value);
                    break;
                case "FolderIcon":
                    result = folder_icon.SetFromString(value);
                    break;
                case "HoverBackground":
                    result = hover_background.SetFromString(value);
                    break;
                case "IconSize":
                    if (uint.TryParse(value, out uint siz))
                    {
                        default_icon.IconSize = siz;
                        folder_icon.IconSize = siz;
                        result = true;
                    }
                    break;
            }
            return result;
        }
    }
}
