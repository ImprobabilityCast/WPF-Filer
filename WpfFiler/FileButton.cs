using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;


namespace WpfFiler
{
    public class FileButton : Button
    {
        /*
         * protected properties
         */

        protected StackPanel panel;

        /*
         * public properties
         */

        public TextBlock textBlock;
        public FileSystemInfo info;

        /*
         * constructors
         */

        // private default constructor to prevent unintended initalization
        private FileButton() { }

        // Aliases file
        // Does not modify config
        public FileButton(FileSystemInfo file, ConfigureWpfFiler config)
        {
            Margin = new Thickness(4);
            BorderThickness = new Thickness(0);
            Background = config.BackgroundBrush;
            info = file;

            textBlock = new TextBlock
            {
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Bottom,
                Text = info.Name
            };

            panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Width = config.IconSize
            };

            Image img = GetImage(info, config);
            panel.Children.Add(img);
            panel.Children.Add(textBlock);

            AddChild(panel);
        }

        /*
         * protected helper methods 
         */

        protected static Image GetImage(FileSystemInfo file, ConfigureWpfFiler config)
        {
            System.Drawing.Icon ico;
            Image img = new Image();

            if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                ico = config.DefaultFolderIcon;
            else
                ico = config.DefaultFileIcon;

            BitmapSource bmp = Imaging.CreateBitmapSourceFromHIcon(
                                        ico.Handle, Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions()
                                        );
            ico.Dispose();
            img.Source = bmp;
            img.Width = config.IconSize;
            img.Height = config.IconSize;
            img.HorizontalAlignment = HorizontalAlignment.Center;
            img.VerticalAlignment = VerticalAlignment.Center;
            return img;
        }
    }
}
