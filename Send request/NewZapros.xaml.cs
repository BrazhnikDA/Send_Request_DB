using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Send_request
{

    public partial class NewZapros : Window
    {
        private string zapros = "";

        public NewZapros(string newZapros_)
        {
            InitializeComponent();

            zapros = newZapros_;
            rtbEditor.AppendText(zapros);

            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        }

		private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
		{
			object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
			btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
			btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
			temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
			btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
			cmbFontFamily.SelectedItem = temp;
			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
			cmbFontSize.Text = temp.ToString();
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
			if (dlg.ShowDialog() == true)
			{
				FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
				TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
				range.Load(fileStream, DataFormats.Text);
			}
		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Save_Files();
		}

		private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbFontFamily.SelectedItem != null)
				rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
		}

		private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
		{
			rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
		}

		private void Image_MouseEnter(object sender, MouseEventArgs e)
		{
			btnRun.Background = Brushes.DarkGreen;
		}

		private void Image_MouseLeave(object sender, MouseEventArgs e)
		{
			btnRun.Background = Brushes.LightGreen;
		}

		private void Image_MouseDown(object sender, RoutedEventArgs e)
		{
			var textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
			string Str = textRange.Text;
			MainWindow main = new MainWindow(Str);
			main.Set_Zapros(Str);
			main.Show();
			main.Connect_DataBase();
		}

		private void HotKeyTriger(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.LeftCtrl && e.Key == Key.S)
			{
				Save_Files();
			}
		}

		private void Save_Files()
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Sql files (*.sql)|*.sql|All files (*.*)|*.*";
			if (dlg.ShowDialog() == true)
			{
				FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
				TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
				range.Save(fileStream, DataFormats.Text);
			}
		}
	}
}
