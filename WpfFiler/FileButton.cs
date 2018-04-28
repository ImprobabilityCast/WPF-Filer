using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using BrendanGrant.Helpers.FileAssociation;

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

        // private constructor to prevent unintended initalization
        private FileButton() { }

        // Aliases file
        // does not modify config
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

        /*!
        * \return This function will never return null.
        */
        protected static System.Drawing.Icon GetAssociatedIcon(string file,
                ConfigureWpfFiler config, bool largeIcon)
        {
            int dot = file.LastIndexOf('.');
            if (dot == -1)
                return config.DefaultFileIcon;
            try
            {
                FileAssociationInfo fai = new FileAssociationInfo(file.Substring(dot));
                ProgramAssociationInfo pai = new ProgramAssociationInfo(fai.ProgID);
                ProgramIcon icon = pai.DefaultIcon;
                //ExtractIconEx(file, number, out large, out small, 1);
                System.Drawing.Icon ico = Win32API.ExtractIcon(icon.Path, icon.Index, largeIcon);
                if (ico == null)
                    return config.DefaultFileIcon;
                else
                    return ico;
            }
            catch
            {
                return config.DefaultFileIcon;
            }
        }

        protected static Image GetImage(FileSystemInfo file, ConfigureWpfFiler config)
        {
            System.Drawing.Icon ico;
            Image img = new Image();

            if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                ico = config.DefaultFolderIcon;
            else
            {
                //Win32API.SHGetFileInfo(file.FullName, 0, ref fInfo, (uint)Marshal.SizeOf(fInfo), 0);
                ico = GetAssociatedIcon(file.Name, config, true);
            }
            //    ico = System.Drawing.Icon.ExtractAssociatedIcon(file.FullName);

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
