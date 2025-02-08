using DearFuture.Views;
namespace DearFuture
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Register navigation routes
            Routing.RegisterRoute(nameof(CreateCapsulePage), typeof(CreateCapsulePage));
        }
    }

}
