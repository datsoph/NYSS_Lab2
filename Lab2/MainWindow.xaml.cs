using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<DataUnit> fullSheet;
        public List<List<DataUnit>> pageSheets;
        public int currentPage;
        public string xlsxPath = @"thrlist.xlsx";
        public string oldXlsxPath = @"thrlist_old.xlsx";

        public MainWindow()
        {
            if (!CheckSheetFile())
            {
                MessageBox.Show("Ошибка: отсутствует необходимая база данных.");
                Environment.Exit(0);
            }
            InitializeComponent();
            InitializeSheet();
        }
        
        public bool CheckSheetFile()
        {
            if (!File.Exists(xlsxPath))
            {
                if (MessageBox.Show("База не найдена. Скачать?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    try
                    {
                        DownloadFile();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка: не удалось установить соединение с сервером.");
                        return false;
                    }
            }
            return File.Exists(xlsxPath);
        }

        public void DownloadFile()
        {
            using (WebClient client = new WebClient())
                client.DownloadFile("https://bdu.fstec.ru/files/documents/thrlist.xlsx", xlsxPath);
        }

        public void InitializeSheet()
        {
            fullSheet = new List<DataUnit>();
            using (XLWorkbook wbook = new XLWorkbook(xlsxPath))
            {
                IXLWorksheet wsheet = wbook.Worksheet(1);
                int row = 3;
                while (wsheet.Cell("A" + row).GetString() != "")
                {
                    fullSheet.Add(new DataUnit(wsheet.Cell("A" + row).GetValue<int>(),
                                               wsheet.Cell("B" + row).GetValue<string>(),
                                               wsheet.Cell("C" + row).GetValue<string>(),
                                               wsheet.Cell("D" + row).GetValue<string>(),
                                               wsheet.Cell("E" + row).GetValue<string>(),
                                               wsheet.Cell("F" + row).GetValue<string>(),
                                               wsheet.Cell("G" + row).GetValue<string>(),
                                               wsheet.Cell("H" + row).GetValue<string>(),
                                               wsheet.Cell("J" + row).GetValue<DateTime>()));
                    row++;
                }
            }
            InitializePageSheets();
        }

        public void InitializePageSheets()
        {
            pageSheets = new List<List<DataUnit>>();
            for (int i = 0; i < fullSheet.Count; i++)
            {
                if (i % 15 == 0)
                    pageSheets.Add(new List<DataUnit>());
                pageSheets.Last().Add(fullSheet[i]);
            }
            currentPage = 0;
            grid.ItemsSource = pageSheets[currentPage];
            pageBlock.Text = $"Current Page: {currentPage + 1}";
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Проверить наличие изменений в базе?", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.No)
                return;
            File.Delete(oldXlsxPath);
            File.Copy(xlsxPath, oldXlsxPath);
            try
            {
                DownloadFile();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка: не удалось установить соединение с сервером.");
                File.Copy(oldXlsxPath, xlsxPath);
                File.Delete(oldXlsxPath);
                return;
            }
            if (HashCompare())
                MessageBox.Show("Обновление не требуется.");
            else
            {
                List<DataUnit> beforeChanges = new List<DataUnit>();
                List<DataUnit> afterChanges = new List<DataUnit>();
                using (XLWorkbook wbook = new XLWorkbook(xlsxPath))
                {
                    IXLWorksheet wsheet = wbook.Worksheet(1);
                    int row = 3;
                    while (wsheet.Cell("A" + row).GetString() != "")
                    {
                        if (wsheet.Cell("A" + row).GetValue<int>() > fullSheet.Count)
                        {
                            afterChanges.Add(new DataUnit(wsheet.Cell("A" + row).GetValue<int>(),
                                                       wsheet.Cell("B" + row).GetValue<string>(),
                                                       wsheet.Cell("C" + row).GetValue<string>(),
                                                       wsheet.Cell("D" + row).GetValue<string>(),
                                                       wsheet.Cell("E" + row).GetValue<string>(),
                                                       wsheet.Cell("F" + row).GetValue<string>(),
                                                       wsheet.Cell("G" + row).GetValue<string>(),
                                                       wsheet.Cell("H" + row).GetValue<string>(),
                                                       wsheet.Cell("J" + row).GetValue<DateTime>()));
                            fullSheet.Add(afterChanges.Last());
                        }
                        else if (wsheet.Cell("J" + row).GetValue<DateTime>() > fullSheet[row - 3].LastChanged)
                        {
                            beforeChanges.Add(fullSheet[row - 3]);
                            afterChanges.Add(new DataUnit(wsheet.Cell("A" + row).GetValue<int>(),
                                                       wsheet.Cell("B" + row).GetValue<string>(),
                                                       wsheet.Cell("C" + row).GetValue<string>(),
                                                       wsheet.Cell("D" + row).GetValue<string>(),
                                                       wsheet.Cell("E" + row).GetValue<string>(),
                                                       wsheet.Cell("F" + row).GetValue<string>(),
                                                       wsheet.Cell("G" + row).GetValue<string>(),
                                                       wsheet.Cell("H" + row).GetValue<string>(),
                                                       wsheet.Cell("J" + row).GetValue<DateTime>()));
                            fullSheet[row - 3] = afterChanges.Last();
                        }
                        row++;
                    }
                }
                MessageBox.Show($"База успешно обновлена. Записей обновлено: {beforeChanges.Count}; записей добавлено: {afterChanges.Count - beforeChanges.Count}.");
                new Changes(beforeChanges, afterChanges).Show();
                InitializePageSheets();
            }
            File.Delete(oldXlsxPath);
        }

        private bool HashCompare()
        {
            byte[] oldFile, newFile;
            using (var md5 = MD5.Create()) using (var stream = File.OpenRead(oldXlsxPath))
                oldFile = md5.ComputeHash(stream);
            using (var md5 = MD5.Create()) using (var stream = File.OpenRead(xlsxPath))
                newFile = md5.ComputeHash(stream);
            return oldFile.SequenceEqual(newFile);
        }

        private void Grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGridTextColumn column = e.Column as DataGridTextColumn;
            Style style = new Style(typeof(TextBlock));
            switch (column.Header.ToString())
            {
                case "ExtendedId":
                    column.Width = 60;
                    column.Header = "Id";
                    break;
                case "Name":
                    column.MinWidth = 300;
                    style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                    column.ElementStyle = style;
                    break;
                default:
                    column.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void ToFirstPage(object sender, RoutedEventArgs e)
        {
            currentPage = 0;
            grid.ItemsSource = pageSheets[currentPage];
            pageBlock.Text = $"Current Page: {currentPage + 1}";

        }

        private void ToPreviousPage(object sender, RoutedEventArgs e)
        {
            if (currentPage == 0)
                return;
            currentPage--;
            grid.ItemsSource = pageSheets[currentPage];
            pageBlock.Text = $"Current Page: {currentPage + 1}";
        }

        private void ToNextPage(object sender, RoutedEventArgs e)
        {
            if (currentPage + 1 == pageSheets.Count)
                return;
            currentPage++;
            grid.ItemsSource = pageSheets[currentPage];
            pageBlock.Text = $"Current Page: {currentPage + 1}";
        }

        private void ToLastPage(object sender, RoutedEventArgs e)
        {
            currentPage = pageSheets.Count - 1;
            grid.ItemsSource = pageSheets[currentPage];
            pageBlock.Text = $"Current Page: {currentPage + 1}";
        }

        private void ShowInfo(object sender, RoutedEventArgs e)
        {
            if (grid.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выделить элемент.");
                return;
            }
            new Info(grid.SelectedItem.ToString()).Show();
        }
    }
}
