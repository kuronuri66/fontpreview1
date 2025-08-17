using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Markup;
using System.Linq; // ToList() のために追加
using System.Diagnostics;
using System.Windows.Input;

namespace FontPreviewApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine("デバッグ情報: ウィンドウが初期化されました。");
            Debug.WriteLine("アプリケーションが正常に動作しています。");

            fontdisplay();
        }

        public void fontdisplay()
        {
            fontGrid.Children.Clear();

            // Fonts.SystemFontFamilies を List<FontFamily> に変換
            var fontList = Fonts.SystemFontFamilies.ToList();

            // フォントを名前（.Source）でアルファベット順にソート
            fontList.Sort((font1, font2) => string.Compare(font1.Source, font2.Source));

            var currentCulture = CultureInfo.CurrentUICulture;
            var language = XmlLanguage.GetLanguage(currentCulture.IetfLanguageTag);

            for (int i = 0; i < fontList.Count; i++) // ループ条件を簡潔に修正
            {
                var fontName1 = "";
                string enteredText = SearchTextBox.Text;
                string previewText = PreviewTextBox.Text;
                fontList[i].FamilyNames.TryGetValue(language, out string fontName);
                if (!string.IsNullOrEmpty(fontName))
                {
                    fontName1 = fontName;
                }
                else
                {
                    fontName1 = fontList[i].Source;
                }
                if ((fontList[i].Source.Contains(enteredText, StringComparison.OrdinalIgnoreCase) ||
                fontName1.Contains(enteredText, StringComparison.OrdinalIgnoreCase)) &&
                (JPCheckBox.IsChecked == false || IsJapaneseFont(fontList[i]))
                )

                {
                    Button newButton = new Button();
                    StackPanel buttonContentPanel = new StackPanel();
                    TextBlock newTextBlockname = new TextBlock();
                    Grid newGrid = new Grid();
                    TextBlock newTextBlock = new TextBlock();
                    TextBlock previewBlock = new TextBlock();


                    newButton.Tag = new { Font = fontList[i].Source, FontName = fontName1 };
                    newButton.Background = new SolidColorBrush(Color.FromRgb(31, 31, 31));
                    newButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                    newButton.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    newButton.BorderThickness = new Thickness(0);
                    //newButton.MouseRightButtonDown += copy_RightClick;

                    ContextMenu? contextMenu = this?.FindResource("myButtonContextMenu") as ContextMenu;

                    if (contextMenu != null)
                    {
                        newButton.ContextMenu = contextMenu;
                    }

                    {
                        buttonContentPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                        {

                            FontFamily gothicFont = new FontFamily("Noto-sans-JP");

                            newTextBlockname.Text = fontList[i].Source;
                            newTextBlockname.FontFamily = gothicFont;
                            newTextBlockname.FontSize = 12;
                            newTextBlockname.Foreground = new SolidColorBrush(Colors.White);
                            newTextBlockname.Background = new SolidColorBrush(Colors.Green);
                            newTextBlockname.HorizontalAlignment = HorizontalAlignment.Stretch;

                            ColumnDefinition col1 = new ColumnDefinition();
                            col1.Width = GridLength.Auto;
                            newGrid.ColumnDefinitions.Add(col1);

                            newGrid.ColumnDefinitions.Add(new ColumnDefinition());

                            newGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newGrid.VerticalAlignment = VerticalAlignment.Stretch;
                            {

                                // フォントの完全名を取得、存在しない場合は .Source を使用
                                if (!string.IsNullOrEmpty(fontName))
                                {
                                    newTextBlock.Text = fontName;
                                }
                                else
                                {
                                    newTextBlock.Text = fontList[i].Source;
                                }

                                newTextBlock.FontFamily = fontList[i];
                                newTextBlock.FontSize = 25;
                                newTextBlock.Foreground = new SolidColorBrush(Colors.White);
                                newTextBlock.HorizontalAlignment = HorizontalAlignment.Left;


                                previewBlock.FontFamily = fontList[i];
                                previewBlock.FontSize = 25;
                                previewBlock.Foreground = new SolidColorBrush(Colors.White);
                                previewBlock.HorizontalAlignment = HorizontalAlignment.Right;
                                previewBlock.Text = previewText;
                            }
                        }
                        Grid.SetColumn(newTextBlock, 0);
                        Grid.SetColumn(previewBlock, 1);
                        newGrid.Children.Add(newTextBlock);
                        newGrid.Children.Add(previewBlock);
                        buttonContentPanel.Children.Add(newTextBlockname);
                        buttonContentPanel.Children.Add(newGrid);
                    }
                    newButton.Content = buttonContentPanel;
                    fontGrid.Children.Add(newButton);
                }

            }
        }
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            fontdisplay();
        }
        private void PreviewChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                fontdisplay();

                // （任意）Enterが押された後にTextBoxからフォーカスを外す
                Keyboard.ClearFocus();
            }
        }

        private void JPClick(object sender, RoutedEventArgs e)
        {
            fontdisplay();
        }

        public bool IsJapaneseFont(FontFamily fontFamily)
        {
            // 日本語カルチャに対応する XmlLanguage を取得
            XmlLanguage japaneseLanguage = XmlLanguage.GetLanguage("ja-jp");

            // フォントファミリーの各タイプフェイスをループ
            foreach (FamilyTypeface typeface in fontFamily.FamilyTypefaces)
            {
                // タイプフェイスの FaceNames に日本語カルチャが含まれているかチェック
                if (fontFamily.FamilyNames.ContainsKey(japaneseLanguage))
                {
                    return true;
                }
            }

            // 見つからなければ false を返す
            return false;
        }

        private void Copyfontname(object sender, RoutedEventArgs e)
        {
            // イベントを発生させたのは MenuItem
            MenuItem? menuItem = sender as MenuItem;

            // MenuItem の親要素である ContextMenu を取得
            ContextMenu? contextMenu = menuItem?.Parent as ContextMenu;

            // ContextMenu が配置されているターゲット（この場合は Button）を取得
            Button? clickedButton = contextMenu?.PlacementTarget as Button;

            if (clickedButton != null)
            {
                // Button の Tag を取得
                dynamic tagData = clickedButton.Tag;

                try
                {
                    // クリップボードに文字列をコピー
                    Clipboard.SetText(tagData.FontName);

                    MessageBox.Show("テキストがクリップボードにコピーされました。");
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    // クリップボードへのアクセスが失敗した場合の処理
                    MessageBox.Show($"クリップボードへのアクセスに失敗しました: {ex.Message}");
                }
            }
        }
        
        private void Copyfont(object sender, RoutedEventArgs e)
        {
            // イベントを発生させたのは MenuItem
            MenuItem? menuItem = sender as MenuItem;

            // MenuItem の親要素である ContextMenu を取得
            ContextMenu? contextMenu = menuItem?.Parent as ContextMenu;

            // ContextMenu が配置されているターゲット（この場合は Button）を取得
            Button? clickedButton = contextMenu?.PlacementTarget as Button;

            if (clickedButton != null)
            {
                // Button の Tag を取得
                dynamic tagData = clickedButton.Tag;

                try
                {
                    // クリップボードに文字列をコピー
                    Clipboard.SetText(tagData.Font);

                    MessageBox.Show("テキストがクリップボードにコピーされました。");
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    // クリップボードへのアクセスが失敗した場合の処理
                    MessageBox.Show($"クリップボードへのアクセスに失敗しました: {ex.Message}");
                }
            }
        }
    }
}