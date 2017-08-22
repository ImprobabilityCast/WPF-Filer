using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BrendanGrant.Helpers.FileAssociation;


namespace WpfFiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach(DriveInfo d in drives)
                tv1.Items.Add(CreateTreeViewItem(d));
            config = new ConfigureWpfFiler();
            config.LoadConfiguration();
            Tabs.Items.Add(new CloseableTab());
        }

        private ConfigureWpfFiler config;
        
        /*!
         * \return This function will never return null.
         */
        private System.Drawing.Icon GetAssociatedIcon(string file, bool largeIcon)
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

        private TreeViewItem CreateTreeViewItem(object o)
        {
            TreeViewItem i = new TreeViewItem();
            i.Header = o.ToString();
            i.Tag = o;
            i.MouseLeftButtonUp += tv1_item_MouseLeftButtonUp;
            i.MouseRightButtonUp += tv1_item_MouseRightButtonUp;
            i.Items.Add("Loading...");
            return i;
        }

        private object saved_item = null;
        private void Click(object sender, bool open_new_tab = false)
        {
            TreeViewItem i = (sender as TreeViewItem);
            // This bit here is to stop all the parent tree view items from opening new tabs
            if (i.HasItems)
                foreach (object a in i.Items)
                    if (a == saved_item)
                    {
                        saved_item = i;
                        return;
                    }
            saved_item = i;

            CloseableTab tab = open_new_tab ? new CloseableTab() : Tabs.SelectedItem as CloseableTab;
            if (i.Tag as DirectoryInfo != null)
                Populate(tab, i.Tag as DirectoryInfo);
            else if((i.Tag as DriveInfo).IsReady)
                Populate(tab, (i.Tag as DriveInfo).RootDirectory);
            if(open_new_tab)
                Tabs.Items.Add(tab);
            tab.Focus();
        }

        private void Populate(CloseableTab tab, DirectoryInfo info)
        {
            //if(info.GetAccessControl())
            tab.Title = info.Name;
            tab.GetWrapPanel().Children.Clear();
            foreach(FileSystemInfo file in info.GetFileSystemInfos())
            {
                StackPanel stack = new StackPanel();
                Image img = new Image();
                System.Drawing.Icon ico;
                stack.Orientation = Orientation.Vertical;
                stack.Margin = new Thickness(4);
                stack.Width = config.IconSize;
                stack.Tag = file;
                stack.MouseEnter += stack_MouseEnter;
                stack.MouseLeave += stack_MouseLeave;

                if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    ico = config.DefaultFolderIcon;
                else
                {
                    //Win32API.SHGetFileInfo(file.FullName, 0, ref fInfo, (uint)Marshal.SizeOf(fInfo), 0);
                    ico = GetAssociatedIcon(file.Name, true);
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
                stack.Children.Add(img);

                TextBlock tb = new TextBlock();
                tb.Text = file.Name;
                tb.TextAlignment = TextAlignment.Center;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.VerticalAlignment = VerticalAlignment.Bottom;
                stack.Children.Add(tb);
                //Process.Start(file.Name);
                tab.GetWrapPanel().Children.Add(stack);
            }
        }

        private void tv1_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem i = e.Source as TreeViewItem;
            //i.IsSelected = true;
            if (i.Items.Count == 1 && i.Items[0] is string)
            {
                i.Items.Clear();
                DirectoryInfo theDir = null;
                if (i.Tag is DriveInfo)
                {
                    DriveInfo c = i.Tag as DriveInfo;
                    if (c.IsReady)
                        theDir = c.RootDirectory;
                }
                else if (i.Tag is DirectoryInfo)
                    theDir = i.Tag as DirectoryInfo;
                if (theDir != null)
                {
                    try
                    {
                        foreach (DirectoryInfo dir in theDir.GetDirectories())
                            i.Items.Add(CreateTreeViewItem(dir));
                    }
                    catch { }
                }
            }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (TabItem ti in Tabs.Items)
            {
                if (ti.IsSelected)
                {
                    this.Title = ti.Header.ToString();
                    break;
                }
            }
        }

        private void tv1_item_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Click(sender, true);
        }

        private void tv1_item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Click(sender);
        }

        private void stack_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as StackPanel).Background = config.HoverBackgroundBrush;
        }

        private void stack_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as StackPanel).Background = config.BackgroundBrush;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            config.SaveConfiguration();
        }
    }
}
