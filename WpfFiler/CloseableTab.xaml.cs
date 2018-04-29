using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfFiler
{
    /// <summary>
    /// Interaction logic for CloseableTab.xaml
    /// </summary>
    
    public partial class CloseableTabHeader : UserControl
    {
        public CloseableTabHeader()
        {
            InitializeComponent();
        }
    }

    public class CloseableTab : TabItem
    {
        public WrapPanel Panel
        {
            get;
            protected set;
        }

        public string Title
        {
            get { return (Header as CloseableTabHeader).TitleLabel.Content.ToString(); }
            set { (Header as CloseableTabHeader).TitleLabel.Content = value; }
        }


        public CloseableTab()
        {
            Constructor();
        }

        public CloseableTab(Color background)
        {
            Constructor();
            Panel.Background = new SolidColorBrush(background);
        }


        private void Constructor()
        {
            Panel = new WrapPanel();
            CloseableTabHeader header = new CloseableTabHeader();
            ScrollViewer sv = new ScrollViewer();
            header.CloseButton.MouseEnter += CloseButtonMouseEnter;
            header.CloseButton.MouseLeave += CloseButtonMouseLeave;
            header.CloseButton.Click += CloseButtonClick;
            header.TitleLabel.SizeChanged += TitleLabelSizeChanged;
            Header = header;
            sv.Content = Panel;
            Content = sv;
        }

        private void CloseButtonMouseEnter(object sender, MouseEventArgs e)
        {
            (Header as CloseableTabHeader).CloseButton.Foreground = Brushes.Red;
        }
        private void CloseButtonMouseLeave(object sender, MouseEventArgs e)
        {
            (Header as CloseableTabHeader).CloseButton.Foreground = Brushes.Black;
        }
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            (Parent as TabControl).Items.Remove(this);
        }
        private void TitleLabelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            (Header as CloseableTabHeader).CloseButton.Margin =
                new Thickness((Header as CloseableTabHeader).TitleLabel.ActualWidth + 5, 3, 4, 0);
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            (Header as CloseableTabHeader).CloseButton.Visibility = Visibility.Visible;
        }
        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            (Header as CloseableTabHeader).CloseButton.Visibility = Visibility.Hidden;
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            (Header as CloseableTabHeader).CloseButton.Visibility = Visibility.Visible;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!IsSelected)
                (Header as CloseableTabHeader).CloseButton.Visibility = Visibility.Hidden;
        }

    }
}

