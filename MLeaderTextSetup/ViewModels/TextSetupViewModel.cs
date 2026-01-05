using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Autodesk.AutoCAD.ApplicationServices;
using MLeaderTextSetup.Actions;
using MLeaderTextSetup.Models;

namespace MLeaderTextSetup.ViewModels
{
    public class TextSetupViewModel : BaseViewModel
    {
        private readonly Action _close;
        private readonly PreviewData _previewData = new PreviewData();

        public List<string> TextStyles { get; }
        public List<ColorItem> ColorItems { get; }

        private MLeaderTextSettings _settings;
        public MLeaderTextSettings Settings
        {
            get => _settings;
            private set { _settings = value; OnPropertyChanged(); }
        }

        public string TextStyleName
        {
            get => Settings.TextStyleName;
            set { Settings.TextStyleName = value; OnPropertyChanged(); UpdatePreview(); }
        }

        public double TextHeight
        {
            get => Settings.TextHeight;
            set
            {
                if (value <= 0) value = 0.1;
                Settings.TextHeight = value;
                OnPropertyChanged();
                UpdatePreview();
            }
        }

        private ColorItem _selectedColorItem;
        public ColorItem SelectedColorItem
        {
            get => _selectedColorItem;
            set
            {
                _selectedColorItem = value;
                if (value != null)
                {
                    Settings.ColorIndex = value.AciIndex;
                    Settings.ColorByLayer = (value.AciIndex == 256);
                }
                OnPropertyChanged();
                UpdatePreview();
            }
        }

        public string FormatTemplate
        {
            get => Settings.FormatTemplate;
            set { Settings.FormatTemplate = value ?? ""; OnPropertyChanged(); UpdatePreview(); }
        }

        private string _previewText = "";
        public string PreviewText
        {
            get => _previewText;
            set { _previewText = value; OnPropertyChanged(); }
        }

        public RelayCommand DefaultCommand { get; }
        public RelayCommand DrawCommand { get; }
        public RelayCommand CloseCommand { get; }

        public TextSetupViewModel(Action close)
        {
            _close = close;

            TextStyles = TextStyleActions.GetTextStyleNames().OrderBy(x => x).ToList();
            ColorItems = GetColorItems();

            Settings = SettingsActions.LoadFromDrawing() ?? new MLeaderTextSettings();

\            _selectedColorItem = ColorItems.FirstOrDefault(c => c.AciIndex == Settings.ColorIndex) 
                                 ?? ColorItems.FirstOrDefault(c => c.AciIndex == 256);

            DefaultCommand = new RelayCommand(() =>
            {
                Settings = new MLeaderTextSettings();
                OnPropertyChanged(nameof(TextStyleName));
                OnPropertyChanged(nameof(TextHeight));
                OnPropertyChanged(nameof(FormatTemplate));
                
                SelectedColorItem = ColorItems.FirstOrDefault(c => c.AciIndex == 256);
                
                UpdatePreview();
            });

            DrawCommand = new RelayCommand(() =>
            {
                try
                {
                    SettingsActions.SaveToDrawing(Settings);
                    _close();

                    var doc = Application.DocumentManager.MdiActiveDocument;
                    doc.SendStringToExecute("MLEADER_DRAW ", true, false, false);
                }
                catch (Exception ex)
                {
                    var doc = Application.DocumentManager.MdiActiveDocument;
                    if (doc != null)
                        doc.Editor.WriteMessage($"\nLỗi: {ex.Message}");
                }
            });

            CloseCommand = new RelayCommand(() => _close());

            UpdatePreview();
        }

        private void UpdatePreview()
        {
            PreviewText = PreviewActions.BuildText(Settings, _previewData);
        }

        private List<ColorItem> GetColorItems()
        {
            return new List<ColorItem>
            {
                new ColorItem { Name = "ByLayer", AciIndex = 256, ColorBrush = Brushes.White },
                new ColorItem { Name = "Red", AciIndex = 1, ColorBrush = Brushes.Red },
                new ColorItem { Name = "Yellow", AciIndex = 2, ColorBrush = Brushes.Yellow },
                new ColorItem { Name = "Green", AciIndex = 3, ColorBrush = Brushes.Lime },
                new ColorItem { Name = "Cyan", AciIndex = 4, ColorBrush = Brushes.Cyan },
                new ColorItem { Name = "Blue", AciIndex = 5, ColorBrush = Brushes.Blue },
                new ColorItem { Name = "Magenta", AciIndex = 6, ColorBrush = Brushes.Magenta },
                new ColorItem { Name = "White", AciIndex = 7, ColorBrush = Brushes.White },
                new ColorItem { Name = "Gray", AciIndex = 8, ColorBrush = Brushes.Gray },
                new ColorItem { Name = "Light Gray", AciIndex = 9, ColorBrush = Brushes.LightGray }
            };
        }
    }
}
