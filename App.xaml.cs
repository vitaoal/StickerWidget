using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace WpfApp1
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            Debug.WriteLine("=== APP INICIANDO ===");
            
            // Cria e mostra a janela principal
            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            
            // Força a criação do handle da janela sem mostrá-la
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.ShowActivated = false;
            mainWindow.Show();
            mainWindow.Hide();
            
            Debug.WriteLine("=== JANELA CRIADA E OCULTA ===");
        }
    }
}
