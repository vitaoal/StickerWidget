using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Services;

namespace WpfApp1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly WindowManagerService _windowManager;
        private readonly ImageService _imageService;
        private ObservableCollection<StickerModel> _stickers;
        private bool _isLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<StickerModel> Stickers
        {
            get => _stickers;
            set 
            { 
                _stickers = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(HasStickers));
            }
        }

        public bool HasStickers => Stickers != null && Stickers.Count > 0;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectStickerCommand { get; }

        public MainViewModel()
        {
            _windowManager = new WindowManagerService();
            _imageService = new ImageService();
            Stickers = new ObservableCollection<StickerModel>();
            SelectStickerCommand = new RelayCommand(OnStickerSelected);
            
            // Load from user's Pictures folder by default
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            LoadStickers(defaultPath); 
        }

        public async void LoadStickers(string path)
        {
            IsLoading = true;
            try
            {
                Stickers = await _imageService.LoadImagesAsync(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar stickers: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void PrepareForShow()
        {
            // Capture focus BEFORE showing the window
            _windowManager.SaveCurrentForeignWindow();
        }

        private async void OnStickerSelected(object parameter)
        {
            if (parameter is StickerModel sticker)
            {
                try
                {
                    // 1. Copy to clipboard
                    _imageService.CopyImageToClipboard(sticker.FilePath);

                    // 2. Hide Window
                    Application.Current.MainWindow.Hide();

                    // 3. Restore Focus
                    _windowManager.RestoreLastForeignWindow();

                    // 4. Simulate Paste
                    await _windowManager.SimulatePasteAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao enviar sticker: {ex.Message}");
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Func<object, bool> _canExecute;

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
            
            public void Execute(object parameter) => _execute(parameter);
            
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }
    }
}
