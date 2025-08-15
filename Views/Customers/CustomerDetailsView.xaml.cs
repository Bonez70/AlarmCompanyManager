// Views/Customers/CustomerDetailsView.xaml.cs
using System.Windows.Controls;
using AlarmCompanyManager.ViewModels;

namespace AlarmCompanyManager.Views.Customers
{
    public partial class CustomerDetailsView : UserControl
    {
        public CustomerDetailsView()
        {
            InitializeComponent();
        }

        public CustomerDetailsView(CustomerViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}