using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

class CloseableTab : TabItem
{
    public CloseableTab()
    {
        wpanel = new WrapPanel();
        Constructor();
    }

    public CloseableTab(Color background)
    {
        wpanel = new WrapPanel();
        wpanel.Background = new SolidColorBrush(background);
        Constructor();
    }

    private void Constructor()
    {
        CloseableTabHeader header = new CloseableTabHeader();
        ScrollViewer sv = new ScrollViewer();
        header.button_close.MouseEnter += new MouseEventHandler(button_close_mouse_enter);
        header.button_close.MouseLeave += new MouseEventHandler(button_close_mouse_leave);
        header.button_close.Click += new RoutedEventHandler(button_close_click);
        header.label_title.SizeChanged += new SizeChangedEventHandler(label_title_size_changed);
        Header = header;
        sv.Content = wpanel;
        Content = sv;
    }
    private WrapPanel wpanel;
    public WrapPanel GetWrapPanel() { return wpanel; }

    public string Title
    {
        get { return (Header as CloseableTabHeader).label_title.Content.ToString(); }
        set { (Header as CloseableTabHeader).label_title.Content = value; }
    }

    void button_close_mouse_enter(object sender, MouseEventArgs e)
    {
        (Header as CloseableTabHeader).button_close.Foreground = Brushes.Red;
    }
    void button_close_mouse_leave(object sender, MouseEventArgs e)
    {
        (Header as CloseableTabHeader).button_close.Foreground = Brushes.Black;
    }
    void button_close_click(object sender, RoutedEventArgs e)
    {
        (Parent as TabControl).Items.Remove(this);
    }
    void label_title_size_changed(object sender, SizeChangedEventArgs e)
    {
        (Header as CloseableTabHeader).button_close.Margin = new Thickness(
        (Header as CloseableTabHeader).label_title.ActualWidth + 5, 3, 4, 0);
    }

    protected override void OnSelected(RoutedEventArgs e)
    {
        base.OnSelected(e);
        (Header as CloseableTabHeader).button_close.Visibility = Visibility.Visible;
    }
    protected override void OnUnselected(RoutedEventArgs e)
    {
        base.OnUnselected(e);
        (Header as CloseableTabHeader).button_close.Visibility = Visibility.Hidden;
    }
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        (Header as CloseableTabHeader).button_close.Visibility = Visibility.Visible;
    }
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if(!IsSelected)
            (Header as CloseableTabHeader).button_close.Visibility = Visibility.Hidden;
    }

}

