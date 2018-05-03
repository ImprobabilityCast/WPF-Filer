using System.ComponentModel;
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
        private ConfigureWpfFiler config;

        public MainWindow()
        {
            InitializeComponent();
            config = new ConfigureWpfFiler();
            config.LoadConfiguration();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach(DriveInfo d in drives)
                Tree.Items.Add(CreateTreeViewItem(d));
            Tabs.Items.Add(new CloseableTab());
        }

        private TreeViewItem CreateTreeViewItem(object o)
        {
            Button btn = new Button();
            btn.Content = o.ToString();
            btn.BorderThickness = new Thickness(0);
            btn.Background = config.BackgroundBrush;
            btn.Click += Tree_Click;
            btn.ContextMenuOpening += Tree_Click;
            btn.Tag = o;

            TreeViewItem i = new TreeViewItem();
            i.Header = btn;
            i.Items.Add("Loading...");
            i.Expanded += Tree_Expanded;
            return i;
        }
        
        private void Tree_Click(object sender, RoutedEventArgs args)
        {
            Button i = (sender as Button);
            // This bit here is to stop all the parent tree view items from opening new tabs
            foreach (CloseableTab t in Tabs.Items)
                if (t.Title.Equals(i.Content))
                {
                    t.Focus();
                    return;
                }

            CloseableTab tab;
            if (args.RoutedEvent.Name.Equals("ContextMenuOpening")
                    || Tabs.SelectedItem == null)
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
                LocationBox.Text = info.FullName;
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
            LocationBox.Text = info.FullName;
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

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedItem is CloseableTab tab)
            {
                Title = tab.Title;
                LocationBox.Text = tab.Title;
            }
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

        private void Tree_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem i = e.Source as TreeViewItem;

            if (i.Items.Count == 1 && i.Items[0] is string)
            {
                i.Items.Clear();
                DirectoryInfo theDir = null;
                object tag = (i.Header as Button).Tag;
                if (tag is DriveInfo)
                {
                    DriveInfo c = tag as DriveInfo;
                    if (c.IsReady)
                        theDir = c.RootDirectory;
                }
                else if (tag is DirectoryInfo)
                    theDir = tag as DirectoryInfo;
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
    }
}
