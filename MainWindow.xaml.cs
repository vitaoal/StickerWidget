using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private HotKeyService _hotKeyService;
        private MainViewModel _viewModel;

        public MainWindow()
        {
            Debug.WriteLine("MainWindow: Constructor iniciado");
            InitializeComponent();
            _viewModel = (MainViewModel)DataContext;
            
            ShowInTaskbar = false;
            
            SourceInitialized += MainWindow_SourceInitialized;
            Loaded += MainWindow_Loaded;
            Debug.WriteLine("MainWindow: Constructor concluído");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainWindow: Loaded event disparado");
            
            // Recarrega stickers ao abrir
            RefreshStickers();
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            Debug.WriteLine("MainWindow: SourceInitialized event disparado");
            
            _hotKeyService = new HotKeyService(this, ToggleWidget);
            
            Debug.WriteLine("MainWindow: Tentando registrar hotkey...");
            if (!_hotKeyService.RegisterHotKey())
            {
                Debug.WriteLine("MainWindow: ERRO ao registrar hotkey");
                MessageBox.Show(
                    "Não foi possível registrar o atalho Ctrl+Alt+S.\n\nTalvez outro aplicativo já o esteja usando.", 
                    "Sticker Widget - Erro de Atalho", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
            }
            else
            {
                Debug.WriteLine("MainWindow: Hotkey registrado com sucesso!");
                ShowNotification();
            }
        }

        private void ShowNotification()
        {
            Debug.WriteLine("MainWindow: Mostrando notificação");
            
            var notification = new Window
            {
                Width = 320,
                Height = 110,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Transparent,
                Topmost = true,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.Manual
            };

            notification.Left = SystemParameters.WorkArea.Right - 340;
            notification.Top = SystemParameters.WorkArea.Bottom - 130;

            var border = new System.Windows.Controls.Border
            {
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 30, 30)),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(63, 63, 70)),
                BorderThickness = new Thickness(1),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = System.Windows.Media.Colors.Black,
                    BlurRadius = 20,
                    ShadowDepth = 0,
                    Opacity = 0.8
                },
                Child = new System.Windows.Controls.StackPanel
                {
                    Children =
                    {
                        new System.Windows.Controls.TextBlock
                        {
                            Text = "✨ Sticker Widget Ativo",
                            Foreground = System.Windows.Media.Brushes.White,
                            FontSize = 15,
                            FontWeight = FontWeights.SemiBold
                        },
                        new System.Windows.Controls.TextBlock
                        {
                            Text = "Pressione Ctrl+Alt+S para abrir",
                            Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 128, 128)),
                            FontSize = 12,
                            Margin = new Thickness(0, 6, 0, 0)
                        }
                    }
                }
            };

            notification.Content = border;
            notification.Show();

            Debug.WriteLine("MainWindow: Notificação exibida");

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3.5)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                notification.Close();
                Debug.WriteLine("MainWindow: Notificação fechada");
            };
            timer.Start();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainWindow: Botão refresh clicado");
            RefreshStickers();
        }

        private void RefreshStickers()
        {
            Debug.WriteLine("MainWindow: Recarregando stickers...");
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            _viewModel.LoadStickers(defaultPath);
        }

        private void ToggleWidget()
        {
            Debug.WriteLine($"MainWindow: ToggleWidget chamado. IsVisible={this.IsVisible}");
            
            if (this.IsVisible)
            {
                Debug.WriteLine("MainWindow: Ocultando widget");
                this.Hide();
            }
            else
            {
                Debug.WriteLine("MainWindow: Mostrando widget");
                
                // Recarrega stickers toda vez que abre
                RefreshStickers();
                
                _viewModel.PrepareForShow();
                this.Show();
                this.Activate();
                this.Focus();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainWindow: Botão fechar clicado");
            this.Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            Debug.WriteLine("MainWindow: OnClosed chamado");
            _hotKeyService?.Dispose();
            base.OnClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            // ESC para fechar
            if (e.Key == Key.Escape)
            {
                this.Hide();
            }
        }
    }
}