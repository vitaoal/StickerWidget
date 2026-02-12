# Sticker Widget - Gerenciador de Stickers para Windows

## ?? Descrição

Widget para Windows desenvolvido em WPF que funciona como um gerenciador de stickers (imagens) para envio rápido em aplicativos de chat. O projeto utiliza arquitetura MVVM e APIs nativas do Windows para máxima performance.

## ? Funcionalidades

- **Atalho Global**: Pressione `Alt + Shift + S` para abrir/fechar o widget de qualquer lugar
- **Envio Rápido**: Clique em um sticker e ele será automaticamente colado no aplicativo ativo
- **Performance Otimizada**: Carregamento assíncrono com thumbnails otimizados (DecodePixelWidth)
- **Interface Moderna**: Window sem bordas, transparente e com design limpo
- **Gestão Inteligente de Foco**: Restaura automaticamente o foco para a janela anterior

## ??? Arquitetura

### Estrutura de Pastas

```
WpfApp1/
??? ViewModels/
?   ??? MainViewModel.cs          # ViewModel principal (MVVM)
??? Services/
?   ??? HotKeyService.cs           # Gerenciamento de atalhos globais
?   ??? WindowManagerService.cs    # Gestão de foco e simulação de paste
?   ??? ImageService.cs            # Carregamento otimizado de imagens
?   ??? NativeMethods.cs           # Interop com Win32 API
??? Converters/
?   ??? InverseBooleanToVisibilityConverter.cs
??? MainWindow.xaml                # Interface principal
??? MainWindow.xaml.cs             # Code-behind
??? App.xaml                       # Configuração da aplicação
```

## ?? Tecnologias Utilizadas

- **.NET 10**
- **WPF (Windows Presentation Foundation)**
- **MVVM Pattern**
- **Win32 API** (user32.dll)
  - `RegisterHotKey` - Atalho global
  - `GetForegroundWindow` - Captura janela ativa
  - `SetForegroundWindow` - Restaura foco
  - `keybd_event` - Simula Ctrl+V

## ?? Como Usar

1. **Configuração Inicial**:
   - Por padrão, o widget carrega imagens da pasta "Imagens" do Windows
   - Adicione seus stickers (PNG, JPG, JPEG, GIF, WEBP) nesta pasta

2. **Abrindo o Widget**:
   - Pressione `Alt + Shift + S` em qualquer lugar do Windows

3. **Enviando um Sticker**:
   - Clique no sticker desejado
   - O widget automaticamente:
     - Copia a imagem para a área de transferência
     - Se oculta
     - Restaura o foco para o aplicativo anterior
     - Simula Ctrl+V para colar a imagem

## ?? Fluxo de Execução

```
1. Usuário pressiona Alt+Shift+S
   ?
2. HotKeyService detecta e chama ToggleWidget()
   ?
3. MainViewModel.PrepareForShow() salva janela ativa atual
   ?
4. Widget é exibido
   ?
5. Usuário clica em um sticker
   ?
6. ImageService copia imagem para clipboard
   ?
7. Widget se oculta
   ?
8. WindowManagerService restaura foco da janela salva
   ?
9. WindowManagerService simula Ctrl+V
   ?
10. Imagem é colada no aplicativo de destino
```

## ?? Características Técnicas

### Performance
- **Carregamento Assíncrono**: Não bloqueia a UI
- **DecodePixelWidth**: Reduz uso de memória RAM em até 80%
- **Bitmap.Freeze()**: Permite acesso cross-thread seguro
- **CacheOption.OnLoad**: Otimiza carregamento de imagens

### Gestão de Foco
- Salva o handle da janela ativa antes de mostrar o widget
- Valida para não salvar o próprio widget como janela anterior
- Restaura foco de forma confiável usando SetForegroundWindow

### Simulação de Paste
- Delays otimizados entre eventos de teclado
- Sequência confiável de Ctrl+V
- Tratamento de erros em todas as etapas

## ?? Requisitos de Sistema

- Windows 10/11
- .NET 10 Runtime

## ?? Solução de Problemas

### O atalho não funciona
- Verifique se outro aplicativo não está usando Alt+Shift+S
- Execute como Administrador se necessário

### Imagens não aparecem
- Verifique se há imagens PNG/JPG na pasta "Imagens"
- Formatos suportados: PNG, JPG, JPEG, GIF, WEBP

### Paste não funciona
- Alguns aplicativos podem ter proteção contra automação
- Aguarde alguns segundos após abrir o aplicativo de destino

## ?? Licença

Este projeto é apenas para fins educacionais e demonstrativos.

## ????? Desenvolvimento

Desenvolvido seguindo as melhores práticas de:
- Clean Code
- SOLID Principles
- MVVM Pattern
- Async/Await Pattern
- Windows API Interop
