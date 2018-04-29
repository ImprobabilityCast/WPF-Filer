﻿using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


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

            CloseableTab tab;
            if (open_new_tab || Tabs.SelectedItem == null)
            {
                tab = new CloseableTab();
                Tabs.Items.Add(tab);
            }
            else
                tab = Tabs.SelectedItem as CloseableTab;

            DirectoryInfo info = null;
            if (i.Tag as DirectoryInfo != null)
                info = i.Tag as DirectoryInfo;
            else if ((i.Tag as DriveInfo).IsReady)
                info = (i.Tag as DriveInfo).RootDirectory;

            if (info != null)
            {
                Populate(tab, info);
                Title = info.Name;
                tab.Focus();
            }
        }

        /*
         * @param tab Closeable tab to populate
         * @param info DirectoryInfo object to get all the file and directory info from.
         */
        private void Populate(CloseableTab tab, DirectoryInfo info)
        {
            // TO DO: 
            //          Account for forbidden folders/files
            //          Add caching in some form.
            //


            tab.Title = info.Name;
            tab.Panel.Children.Clear();
            foreach (FileSystemInfo file in info.GetFileSystemInfos())
            {
                FileButton btn = new FileButton(file, config);
                btn.MouseEnter += FileBtn_MouseEnter;
                btn.MouseLeave += FileBtn_MouseLeave;
                btn.Click += FileBtn_Click;
                tab.Panel.Children.Add(btn);
            }
        }

        private void tv1_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem i = e.Source as TreeViewItem;

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
            if (Tabs.SelectedItem is CloseableTab tab)
                Title = tab.Title;
        }

        private void tv1_item_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Click(sender, true);
        }

        private void tv1_item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Click(sender);
        }

        private void FileBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as FileButton).Background = config.HoverBackgroundBrush;
        }

        private void FileBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as FileButton).Background = config.BackgroundBrush;
        }

        private void FileBtn_Click(object sender, RoutedEventArgs e)
        {
            FileButton btn = sender as FileButton;
            if ((btn.info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo dir = new DirectoryInfo(btn.info.FullName);
                WrapPanel wrap = btn.Parent as WrapPanel;
                wrap.Children.Clear();
                Populate(Tabs.SelectedItem as CloseableTab, dir);
                Title = btn.info.Name;
            }
            else
            {
                Process.Start(btn.info.FullName);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            config.SaveConfiguration();
        }
    }
}
