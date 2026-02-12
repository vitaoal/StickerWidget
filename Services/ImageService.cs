using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfApp1.Services
{
    public class ImageService
    {
        private const int ThumbnailSize = 200;

        public async Task<ObservableCollection<StickerModel>> LoadImagesAsync(string folderPath)
        {
            var images = new ObservableCollection<StickerModel>();
            if (!Directory.Exists(folderPath)) return images;

            var files = await Task.Run(() =>
            {
                return Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                                .Where(s => s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || 
                                           s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || 
                                           s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                           s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                                           s.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                                .ToList();
            });

            foreach (var file in files)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(file, UriKind.Absolute);
                        bitmap.DecodePixelWidth = ThumbnailSize; // Optimize RAM usage
                        bitmap.EndInit();
                        bitmap.Freeze(); // Make it cross-thread accessible

                        images.Add(new StickerModel
                        {
                            FilePath = file,
                            FileName = Path.GetFileNameWithoutExtension(file),
                            ImageSource = bitmap,
                            IsAnimated = file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao carregar imagem {file}: {ex.Message}");
                    }
                });
            }

            return images;
        }

        public void CopyImageToClipboard(string filePath)
        {
            try
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                
                // Se for GIF, copia o arquivo para manter animação
                if (extension == ".gif")
                {
                    CopyGifToClipboard(filePath);
                }
                else
                {
                    // Para outros formatos, copia como imagem
                    CopyStaticImageToClipboard(filePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao copiar imagem: {ex.Message}");
            }
        }

        private void CopyGifToClipboard(string filePath)
        {
            try
            {
                // Abordagem 1: Copia como arquivo (funciona em WhatsApp Desktop, Discord, etc)
                var dataObject = new DataObject();
                
                // Adiciona como FileDropList para apps que suportam arrastar arquivos
                var fileList = new System.Collections.Specialized.StringCollection();
                fileList.Add(filePath);
                dataObject.SetFileDropList(fileList);
                
                // Abordagem 2: Também carrega os bytes do GIF para apps que suportam stream
                var gifBytes = File.ReadAllBytes(filePath);
                using (var memoryStream = new MemoryStream(gifBytes))
                {
                    // System.Drawing.Image para preservar metadados do GIF
                    var gifImage = System.Drawing.Image.FromStream(memoryStream);
                    
                    // Adiciona em múltiplos formatos para compatibilidade
                    dataObject.SetData("GIF", gifImage);
                    dataObject.SetData(DataFormats.Bitmap, gifImage);
                }
                
                // Abordagem 3: Também adiciona o stream direto
                var gifStream = new MemoryStream(gifBytes);
                dataObject.SetData("image/gif", gifStream);
                
                Clipboard.Clear();
                Clipboard.SetDataObject(dataObject, true);
                
                System.Diagnostics.Debug.WriteLine($"GIF copiado (animado): {filePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao copiar GIF, tentando método alternativo: {ex.Message}");
                // Fallback: copia como imagem estática se falhar
                CopyStaticImageToClipboard(filePath);
            }
        }

        private void CopyStaticImageToClipboard(string filePath)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
            bitmap.EndInit();

            Clipboard.Clear();
            Clipboard.SetImage(bitmap);
            
            System.Diagnostics.Debug.WriteLine($"Imagem estática copiada: {filePath}");
        }
    }

    public class StickerModel
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public BitmapSource ImageSource { get; set; }
        public bool IsAnimated { get; set; }
    }
}
