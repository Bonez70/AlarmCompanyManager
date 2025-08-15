// Views/Customers/CustomerListView.xaml.cs
using System.Windows.Controls;
using System.Windows.Input;
using AlarmCompanyManager.Models;
using AlarmCompanyManager.ViewModels;

namespace AlarmCompanyManager.Views.Customers
{
    public partial class CustomerListView : UserControl
    {
        public CustomerListView()
        {
            InitializeComponent();
        }

        public CustomerListView(CustomerViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Customer customer)
            {
                if (DataContext is CustomerViewModel viewModel)
                {
                    viewModel.ViewCustomerDetailsCommand.Execute(customer);
                }
            }
        }
    }
}