using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Libraries.MVVMCore;
using MLeaderTextSetup.Actions;
using MLeaderTextSetup.Models;

namespace MLeaderTextSetup.ViewModels
{
    public class TextSetupViewModel : ViewModelBase
    {
        private readonly Action _close;
        private readonly PreviewDataModel _previewData = new PreviewDataModel();

        public List<string> TextStyles { get; }
        public List<ColorItemModel> ColorItems { get; }

        private MLeaderTextSettingModel _settings;
        public MLeaderTextSettingModel Settings
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

        private ColorItemModel _selectedColorItem;
        public ColorItemModel SelectedColorItem
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
        public RelayCommand DrawVertexCommand { get; }
        public RelayCommand CloseCommand { get; }

        public TextSetupViewModel(Action close)
        {
            _close = close;

            TextStyles = TextStyleAction.GetTextStyleNames().OrderBy(x => x).ToList();
            ColorItems = GetColorItems();

            Settings = SettingsAction.LoadFromDrawing() ?? new MLeaderTextSettingModel();

            _selectedColorItem = ColorItems.FirstOrDefault(c => c.AciIndex == Settings.ColorIndex) 
                                 ?? ColorItems.FirstOrDefault(c => c.AciIndex == 256);

            DefaultCommand = new RelayCommand(p =>
            {
                Settings = new MLeaderTextSettingModel();
                OnPropertyChanged(nameof(TextStyleName));
                OnPropertyChanged(nameof(TextHeight));
                OnPropertyChanged(nameof(FormatTemplate));
                
                SelectedColorItem = ColorItems.FirstOrDefault(c => c.AciIndex == 256);
                
                UpdatePreview();
            });

            DrawCommand = new RelayCommand(p =>
            {
                try
                {
                    SettingsAction.SaveToDrawing(Settings);
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

            DrawVertexCommand = new RelayCommand(p =>
            {
                try
                {
                    SettingsAction.SaveToDrawing(Settings);
                    _close();

                    var doc = Application.DocumentManager.MdiActiveDocument;
                    doc.SendStringToExecute("MLEADER_DRAW_VERTEX ", true, false, false);
                }
                catch (Exception ex)
                {
                    var doc = Application.DocumentManager.MdiActiveDocument;
                    if (doc != null)
                        doc.Editor.WriteMessage($"\nLỗi: {ex.Message}");
                }
            });


            CloseCommand = new RelayCommand(p => _close());

            UpdatePreview();
        }

        private void UpdatePreview()
        {
            PreviewText = PreviewAction.BuildText(Settings, _previewData);
        }

        private List<ColorItemModel> GetColorItems()
        {
            return new List<ColorItemModel>
            {
                new ColorItemModel { Name = "ByLayer", AciIndex = 256 },
                new ColorItemModel { Name = "Red", AciIndex = 1 },
                new ColorItemModel { Name = "Yellow", AciIndex = 2 },
                new ColorItemModel { Name = "Green", AciIndex = 3 },
                new ColorItemModel { Name = "Cyan", AciIndex = 4 },
                new ColorItemModel { Name = "Blue", AciIndex = 5 },
                new ColorItemModel { Name = "Magenta", AciIndex = 6 },
                new ColorItemModel { Name = "White", AciIndex = 7 },
                new ColorItemModel { Name = "Gray", AciIndex = 8 },
                new ColorItemModel { Name = "Light Gray", AciIndex = 9 }
            };
        }
    }
}
