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
        this.Header = header;
        sv.Content = wpanel;
        Content = sv;
    }
    private WrapPanel wpanel;
    public WrapPanel GetWrapPanel() { return wpanel; }

    public string Title
    {
        get { return (this.Header as CloseableTabHeader).label_title.Content.ToString(); }
        set { (this.Header as CloseableTabHeader).label_title.Content = value; }
    }

    void button_close_mouse_enter(object sender, MouseEventArgs e)
    {
        (this.Header as CloseableTabHeader).button_close.Foreground = Brushes.Red;
    }
    void button_close_mouse_leave(object sender, MouseEventArgs e)
    {
        (this.Header as CloseableTabHeader).button_close.Foreground = Brushes.Black;
    }
    void button_close_click(object sender, RoutedEventArgs e)
    {
        (this.Parent as TabControl).Items.Remove(this);
    }
    void label_title_size_changed(object sender, SizeChangedEventArgs e)
    {
        (this.Header as CloseableTabHeader).button_close.Margin = new Thickness(
        (this.Header as CloseableTabHeader).label_title.ActualWidth + 5, 3, 4, 0);
    }

    protected override void OnSelected(RoutedEventArgs e)
    {
        base.OnSelected(e);
        (this.Header as CloseableTabHeader).button_close.Visibility = Visibility.Visible;
    }
    protected override void OnUnselected(RoutedEventArgs e)
    {
        base.OnUnselected(e);
        (this.Header as CloseableTabHeader).button_close.Visibility = Visibility.Hidden;
    }
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        (this.Header as CloseableTabHeader).button_close.Visibility = Visibility.Visible;
    }
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if(!this.IsSelected)
            (this.Header as CloseableTabHeader).button_close.Visibility = Visibility.Hidden;
    }





}

