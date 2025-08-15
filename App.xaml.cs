// App.xaml.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using AlarmCompanyManager.Data;
using AlarmCompanyManager.Services;
using AlarmCompanyManager.ViewModels;
using AlarmCompanyManager.Utilities;

namespace AlarmCompanyManager
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Initialize logging
            Logger.Initialize();
            Logger.LogInfo("Application starting up");

            try
            {
                // Create service collection and configure services
                var services = new ServiceCollection();
                ConfigureServices(services);

                // Build service provider
                _serviceProvider = services.BuildServiceProvider();

                // Ensure database is created and seeded
                await InitializeDatabaseAsync();

                // Create and show main window
                var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
                var mainWindow = new MainWindow(mainViewModel);
                mainWindow.Show();

                Logger.LogInfo("Application startup completed successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during application startup");
                MessageBox.Show($"Application failed to start: {ex.Message}", "Startup Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                Logger.LogInfo("Application shutting down");

                _serviceProvider?.Dispose();
                Logger.Shutdown();
            }
            catch (Exception ex)
            {
                // Log to file or event log since our logger might be disposed
                System.Diagnostics.Debug.WriteLine($"Error during shutdown: {ex.Message}");
            }

            base.OnExit(e);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Database Context
            services.AddDbContext<AlarmCompanyContext>(options =>
            {
                var connectionString = GetConnectionString();
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging(false); // Set to true only for debugging
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            // Services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IWorkOrderService, WorkOrderService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddSingleton<IDialogService, DialogService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<CustomerViewModel>();
            services.AddTransient<WorkOrderViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<SettingsViewModel>();

            // Note: MainWindow is not registered here since we create it directly
        }

        private static string GetConnectionString()
        {
            // Connection string for SQL Server Express
            var defaultConnectionString =
                "Server=.\\SQLEXPRESS;Database=AlarmCompanyDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;";

            // Alternative connection strings you can try if the above doesn't work:
            // "Server=(local)\\SQLEXPRESS;Database=AlarmCompanyDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;"
            // "Server=localhost\\SQLEXPRESS;Database=AlarmCompanyDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;"

            return defaultConnectionString;
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                Logger.LogInfo("Initializing database");

                using var scope = _serviceProvider!.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AlarmCompanyContext>();

                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Run any pending migrations
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    Logger.LogInfo($"Applying {pendingMigrations.Count()} pending migrations");
                    await context.Database.MigrateAsync();
                }

                Logger.LogInfo("Database initialization completed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error initializing database");
                throw;
            }
        }

        public static T GetService<T>() where T : class
        {
            if (Current is App app && app._serviceProvider != null)
            {
                return app._serviceProvider.GetRequiredService<T>();
            }
            throw new InvalidOperationException("Service provider not available");
        }

        public static T? GetOptionalService<T>() where T : class
        {
            if (Current is App app && app._serviceProvider != null)
            {
                return app._serviceProvider.GetService<T>();
            }
            return null;
        }
    }
}