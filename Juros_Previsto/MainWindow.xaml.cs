using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Juros_Previsto {
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window {

        
        public MainWindow() {
            DataContext = this;
            
        }
        private void Switch_Mode_Theme(bool initialize = false) {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            if (theme.GetBaseTheme() == BaseTheme.Dark && initialize) {
                Button_Close.Foreground = Brushes.White;
                Button_DarkLight_Mode.Foreground = Brushes.White;
                Button_DarkLight_Mode.Background = (Brush) new BrushConverter().ConvertFromString("#424242");

            }
            else if (initialize) {
                theme.SetBaseTheme(Theme.Dark);
                paletteHelper.SetTheme(theme);
                Button_Close.Foreground = Brushes.Black;
                Button_DarkLight_Mode.Foreground = Brushes.Black;
                Button_DarkLight_Mode.Background = Brushes.White;
            }
            else {
                if (theme.GetBaseTheme() == BaseTheme.Dark) {
                    theme.SetBaseTheme(Theme.Light);
                    paletteHelper.SetTheme(theme);
                    Button_Close.Foreground = Brushes.Black;
                    Button_DarkLight_Mode.Foreground = Brushes.Black;
                    Button_DarkLight_Mode.Background = Brushes.White;
                }
                else {
                    theme.SetBaseTheme(Theme.Dark);
                    paletteHelper.SetTheme(theme);
                    Button_Close.Foreground = Brushes.White;
                    Button_DarkLight_Mode.Foreground = Brushes.White;
                    Button_DarkLight_Mode.Background = (Brush) new BrushConverter().ConvertFromString("#424242");
                }
            }
        }
        private void Button_DarkLight_Mode_Checked(object sender, RoutedEventArgs e) {
            Switch_Mode_Theme();
        }

        private void Button_DarkLight_Mode_Unchecked(object sender, RoutedEventArgs e) {
            Switch_Mode_Theme();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Switch_Mode_Theme(true);
        }

        private void Button_Refresh_Click(object sender, RoutedEventArgs e) {

        }

        private void Switch_View_Date_Checked(object sender, RoutedEventArgs e) {
            Transitioner_Days.SelectedIndex = 1;
        }

        private void Switch_View_Date_Unchecked(object sender, RoutedEventArgs e) {
            Transitioner_Days.SelectedIndex = 0;
        }

        private void Check_Calc_Auto_Checked(object sender, RoutedEventArgs e) {
            Button_Calculate.Visibility = Visibility.Hidden;
        }

        private void Check_Calc_Auto_Unchecked(object sender, RoutedEventArgs e) {
            Button_Calculate.Visibility = Visibility.Visible;
        }

        private void Button_Calculate_Click(object sender, RoutedEventArgs e) {
            BaseCalculoJuros juros = new BaseCalculoJuros();
            juros.OpenCalc(DateTime.Parse(DatePicker_Inicial.Text), DateTime.Parse(DatePicker_Final.Text), double.Parse(TextBox_Valor.Text), double.Parse(TextBox_Juros.Text), (bool) Switch_Type_Juros.IsChecked, (bool) Swtich_Period_Juros.IsChecked, int.Parse(TextBox_Carencia.Text));
            juros.ExecuteCalc();
            Value_Nominal.Text = juros.Valor_Nominal.ToString("C2");
            Value_Total.Text = juros.Valor_Total.ToString("C2");
            Value_Juros.Text = juros.Valor_Juros.ToString("C2");
            Total_Days.Text = juros.Dias_Totais.ToString() + " dias";
        }
    }
}
