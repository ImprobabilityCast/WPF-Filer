using System;
using System.Windows.Media;

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
    
    public class IconHelper : IAttribute<System.Drawing.Icon>
    {
        protected string path;
        protected int index;
        
        public System.Drawing.Icon Value { get; set; }

        // private default constructor to prevent unintended initalization
        private IconHelper() { }

        public IconHelper(string icon_path, int idx)
        {
            path = icon_path;
            index = idx;
            Value = Win32API.ExtractIcon(path, index, true);
        }

        /*
         * Sets the icon for the class. Invailid argumants will have no effect other than
         * causing the method to return false.
         * @param value Expected value syntax: "path index". 
         * \return Returns false when invaild arguments are passed, true otherwise.
         */
        public bool SetFromString(string value)
        {
            bool result = false;
            string[] array = value.Split(' ');
            if((array.Length > 1) && int.TryParse(array[1], out index))
            {
                path = array[0];
                System.Drawing.Icon ico = Win32API.ExtractIcon(path, index, true);
                if (ico != null)
                {
                    Value = ico;
                    result = true;
                }
              
            }
            return result;
        }

        public override string ToString()
        {
            return path + " " + index;
        }
    }

    public class Attributes
    {
        public Background background;
        public IconHelper default_icon;
        public IconHelper folder_icon;
        public Background hover_background;
        public UInt16 icon_size;
        
        
        /*!
         * Everything is initialized to the default values.
         */
        public Attributes()
        {
            background = new Background();
            hover_background = new Background(Brushes.AliceBlue);
            default_icon = new IconHelper("shell32.dll", 0);
            folder_icon = new IconHelper("shell32.dll", 4);
            icon_size = 64;
        }

        public string[] GetAllAttributes()
        {
            return new string[] {
                "Background: " + background,
                "DefaultIcon: " + default_icon,
                "FolderIcon: " + folder_icon,
                "HoverBackground: " + hover_background,
                "IconSize: " + icon_size
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
                    result = icon_size.ToString();
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
                    result = UInt16.TryParse(value, out UInt16 siz);
                    if (result)
                        icon_size = siz;
                    break;
            }
            return result;
        }
    }
}
