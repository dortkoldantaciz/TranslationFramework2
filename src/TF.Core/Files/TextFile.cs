using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using TF.Core.Entities;
using TF.Core.Helpers;
using TF.Core.POCO;
using TF.Core.TranslationEntities;
using TF.Core.Views;
using TF.IO;
using WeifenLuo.WinFormsUI.Docking;
using Yarhl.IO;
using Yarhl.Media.Text;
using OfficeOpenXml;

namespace TF.Core.Files
{
    public class TextFile : TranslationFile
    {
        protected PlainText _text;
        protected TextView _view;

        public override int SubtitleCount
        {
            get
            {
                var subtitles = GetText();
                return subtitles.Text.Length;
            }
        }

        public TextFile(string gameName, string path, string changesFolder, System.Text.Encoding encoding) : base(gameName, path, changesFolder, encoding)
        {
            Type = FileType.TextFile;
        }

        public override void Open(DockPanel panel)
        {
            _view = new TextView();

            _text = GetText();
            _view.LoadData(_text);
            _view.Show(panel, DockState.Document);
        }

        protected virtual PlainText GetText()
        {
            var result = new PlainText();

            var lines = File.ReadAllText(Path, FileEncoding);
            result.Text = lines;
            result.Translation = lines;
            result.Loaded = lines;
            result.PropertyChanged += SubtitlePropertyChanged;
            
            LoadChanges(result);
            
            return result;
        }
        
        protected virtual void SubtitlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NeedSaving = _text.HasChanges;
            OnFileChanged();
        }

        public override bool Search(string searchString, string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Path;
            }
            var bytes = File.ReadAllBytes(path);

            var pattern = FileEncoding.GetBytes(searchString);

            var searchHelper = new SearchHelper(pattern);
            var index1 = searchHelper.Search(bytes);

            var index2 = -1;
            if (HasChanges)
            {
                bytes = File.ReadAllBytes(ChangesFile);
                pattern = Encoding.Unicode.GetBytes(searchString);
                searchHelper = new SearchHelper(pattern);
                index2 = searchHelper.Search(bytes);
            }

            return index1 != -1 || index2 != -1;
        }

        public override void SaveChanges()
        {
            using (var fs = new FileStream(ChangesFile, FileMode.Create))
            using (var output = new ExtendedBinaryWriter(fs, Encoding.Unicode))
            {
                output.Write(ChangesFileVersion);
                output.WriteString(_text.Translation);
                _text.Loaded = _text.Translation;
            }

            NeedSaving = false;
            OnFileChanged();
        }

        protected virtual void LoadChanges(PlainText text)
        {
            if (HasChanges)
            {
                using (var fs = new FileStream(ChangesFile, FileMode.Open))
                using (var input = new ExtendedBinaryReader(fs, Encoding.Unicode))
                {
                    var version = input.ReadInt32();

                    if (version != ChangesFileVersion)
                    {
                        //File.Delete(ChangesFile);
                        return;
                    }

                    var t = input.ReadString();
                    text.PropertyChanged -= SubtitlePropertyChanged;
                    text.Translation = t;
                    text.Loaded = t;
                    text.PropertyChanged += SubtitlePropertyChanged;
                }
            }
        }

        public override void Rebuild(string outputFolder)
        {
            var outputPath = System.IO.Path.Combine(outputFolder, RelativePath);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outputPath));

            var text = GetText();

            using (var fs = new FileStream(outputPath, FileMode.Create))
            using (var output = new StreamWriter(fs, FileEncoding))
            {
                output.Write(text.Translation);
            }
        }

        public override bool SearchText(string searchString, int direction)
        {
            if (_text == null || string.IsNullOrEmpty(searchString))
            {
                return false;
            }

            return _view.SearchText(searchString, direction);
        }

        public override void ExportPo(string path)
        {
            var directory = System.IO.Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            var po = new Po()
            {
                Header = new PoHeader(GameName, "dummy@dummy.com", "es-ES")
            };

            var entry = new PoEntry();
            var text = GetText();
            var tmp = text.Text.Replace(LineEnding.RealLineEnding, LineEnding.PoLineEnding);
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = "<!empty>";
            }
            entry.Original = tmp;

            if (text.Text != text.Translation)
            {
                tmp = text.Translation.Replace(LineEnding.RealLineEnding, LineEnding.PoLineEnding);
                if (string.IsNullOrEmpty(tmp))
                {
                    tmp = "<!empty>";
                }
                entry.Translated = tmp;
            }

            po.Add(entry);
            
            var po2binary = new Yarhl.Media.Text.Po2Binary();
            var binary = po2binary.Convert(po);
            
            binary.Stream.WriteTo(path);
        }

        public override void ImportPo(string inputFile, bool save = true, bool parallel = true)
        {
            var dataStream = DataStreamFactory.FromFile(inputFile, FileOpenMode.Read);
            var binary = new BinaryFormat(dataStream);
            var binary2Po = new Yarhl.Media.Text.Binary2Po();
            var po = binary2Po.Convert(binary);

            _text = GetText();
            var tmp = _text.Text.Replace(LineEnding.RealLineEnding, LineEnding.PoLineEnding);
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = "<!empty>";
            }
            var entry = po.FindEntry(tmp);

            if (!string.IsNullOrEmpty(entry.Translated))
            {
                _text.Translation = entry.Translated.Replace(LineEnding.PoLineEnding, LineEnding.RealLineEnding);
            }

            if (save)
            {
                SaveChanges();
            }
        }

        public override void ExportExcel(string path)
        {
            var directory = System.IO.Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            using (var excel = new ExcelPackage())
            {
                var sheet = excel.Workbook.Worksheets.Add("Sheet 1");
                sheet.Cells["A1"].LoadFromArrays(new[] { new[] { "ORIGINAL", "TRANSLATION" } });

                var text = GetText();
                var original = text.Text;
                var translation = text.Translation;

                sheet.Cells[2, 1].Value = original;
                sheet.Cells[2, 2].Value = translation;

                var excelFile = new FileInfo(path);
                excel.SaveAs(excelFile);
            }
        }

        public override void ImportExcel(string inputFile)
        {
            using (var excel = new ExcelPackage(new FileInfo(inputFile)))
            {
                var sheet = excel.Workbook.Worksheets[1];

                if (sheet.Dimension.Rows >= 2)
                {
                    // var original = sheet.Cells[2, 1].Text;
                    var translation = sheet.Cells[2, 2].Text;

                    _text = GetText();
                    _text.Translation = translation;
                    SaveChanges();
                }
            }
        }
    }
}
