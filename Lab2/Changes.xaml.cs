using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for Changes.xaml
    /// </summary>
    public partial class Changes : Window
    {
        public Changes(List<DataUnit> before, List<DataUnit> after)
        {
            InitializeComponent();
            beforeGrid.ItemsSource = before;
            afterGrid.ItemsSource = after;
        }

        private void Grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGridTextColumn column = e.Column as DataGridTextColumn;
            Style style = new Style(typeof(TextBlock));
            switch (column.Header.ToString())
            {
                case "Id":
                    column.Width = 30;
                    break;
                case "Name":
                    style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                    column.MinWidth = 100;
                    column.MaxWidth = 180;
                    column.ElementStyle = style;
                    break;
                case "Description":
                    style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                    column.MinWidth = 300;
                    column.ElementStyle = style;
                    break;
                case "Source":
                    style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                    column.MinWidth = 100;
                    column.MaxWidth = 180;
                    column.ElementStyle = style;
                    break;
                case "Object":
                    style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                    column.MinWidth = 100;
                    column.MaxWidth = 180;
                    column.ElementStyle = style;
                    break;
                case "Confidentiality":
                    style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
                    column.ElementStyle = style;
                    column.Width = 45;
                    break;
                case "Integrity":
                    style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
                    column.ElementStyle = style;
                    column.Width = 45;
                    break;
                case "Availability":
                    style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
                    column.ElementStyle = style;
                    column.Width = 45;
                    break;
                default:
                    column.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
