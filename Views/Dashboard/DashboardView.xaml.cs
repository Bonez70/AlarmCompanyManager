// Views/Dashboard/DashboardView.xaml.cs
using System.Windows.Controls;
using AlarmCompanyManager.ViewModels;

namespace AlarmCompanyManager.Views.Dashboard
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        public DashboardView(DashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}