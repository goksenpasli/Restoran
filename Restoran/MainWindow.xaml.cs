using System.Windows.Data;
using Restoran.ViewModel;

namespace Restoran
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        public static CollectionViewSource cvs;

        public static CollectionViewSource cvsürün;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            cvs = TryFindResource("Siparişler") as CollectionViewSource;
            cvsürün = TryFindResource("Ürünler") as CollectionViewSource;
        }
    }
}