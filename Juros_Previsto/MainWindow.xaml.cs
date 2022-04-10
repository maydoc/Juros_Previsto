using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Juros_Previsto {
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window {
        
        public MainWindow() {
            DataContext = this;
        }
        private void Switch_Mode_Theme(object sender, RoutedEventArgs e) {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            if (theme.GetBaseTheme() == BaseTheme.Dark) {
                theme.SetBaseTheme(Theme.Light);
                paletteHelper.SetTheme(theme);
            }
            else {
                theme.SetBaseTheme(Theme.Dark);
                paletteHelper.SetTheme(theme);
            }
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private async void Button_Refresh_Click(object sender, RoutedEventArgs e) {
            Animate_Text(Value_Nominal, double.Parse(Value_Nominal.Text.Substring(2)), 0, milliseconds:200);
            Animate_Text(Value_Total, double.Parse(Value_Total.Text.Substring(2)), 0, milliseconds:200);
            Animate_Text(Value_Juros, double.Parse(Value_Juros.Text.Substring(2)), 0, milliseconds:200);
            Animate_Text(Total_Days, double.Parse(Total_Days.Text.Replace(" dias", "")), 0, false, milliseconds:200);
            await Task.Delay(201);
            Button_Calculate_Click(null, null);
        }
        private async void Animate_Text(TextBlock textBlock, double to, double from, bool doubleformat = true, int milliseconds = 500) { 
            DoubleAnimation animaiton = new DoubleAnimation(to, from, TimeSpan.FromMilliseconds(milliseconds));
            Rectangle rectangle = new Rectangle() { Width = to };
            rectangle.SizeChanged += (sender, args) => {
                textBlock.Text = doubleformat ? rectangle.Width.ToString("C2") : rectangle.Width.ToString("N0") + " dias";
            };
            MainGrid.Children.Add(rectangle);
            
            rectangle.BeginAnimation(WidthProperty, animaiton);
            await Task.Delay(500);
            MainGrid.Children.Remove(rectangle);
        }

        private void Switch_View_Date_Change(object sender, RoutedEventArgs e) {
            switch (Transitioner_Days.SelectedIndex)
            {
                case 0:
                    Transitioner_Days.SelectedIndex = 1;
                    break;
                case 1:
                    Transitioner_Days.SelectedIndex = 0;
                    break;
                default:
                    break;
            }
        }
        private void Button_Calculate_Click(object sender, RoutedEventArgs e) {

            if (!Scan_Values()) { return; }

            BaseCalculoJuros juros = new BaseCalculoJuros();
            int carencia = 0;
            if(TextBox_Carencia.Text != String.Empty) {
                carencia = int.Parse(TextBox_Carencia.Text);
            }
            juros.OpenCalc(DateTime.Parse(DatePicker_Inicial.Text),
                DateTime.Parse(DatePicker_Final.Text),
                double.Parse(TextBox_Valor.Text),
                double.Parse(TextBox_Juros.Text),
                (bool) Switch_Type_Juros.IsChecked,
                (bool) Swtich_Period_Juros.IsChecked,
                carencia);

            juros.ExecuteCalc();

            Animate_Text(Value_Nominal, double.Parse(Value_Nominal.Text.Substring(2)), juros.Valor_Nominal);
            Animate_Text(Value_Total, double.Parse(Value_Total.Text.Substring(2)), juros.Valor_Total);
            Animate_Text(Value_Juros, double.Parse(Value_Juros.Text.Substring(2)), juros.Valor_Juros);
            Animate_Text(Total_Days, double.Parse(Total_Days.Text.Replace(" dias", "")), juros.Dias_Totais, false);
        }
        private bool Scan_Values() {
            try {
                DateTime.Parse(DatePicker_Inicial.Text);
            }
            catch(Exception) {
                TextDialog.Text = "Digite uma data inicial válida!";
                DialogHostMain.IsOpen = true;
                return false;
            }
            try {
                DateTime.Parse(DatePicker_Final.Text);
            }
            catch(Exception) {
                TextDialog.Text = "Digite uma data final válida!";
                DialogHostMain.IsOpen = true;
                return false;
            }
            if(TextBox_Juros.Text == String.Empty) {
                TextDialog.Text = "Informe o juros!";
                DialogHostMain.IsOpen = true;
                return false;
            }
            if(TextBox_Valor.Text == String.Empty) {
                TextDialog.Text = "Digite o valor!";
                DialogHostMain.IsOpen = true;
                return false;
            }
            return true;
        }
        private void Calc_Auto() {
            if (!CalcAutoEnable) { return; }
            try {
                BaseCalculoJuros juros = new BaseCalculoJuros();
                int carencia = 0;
                if(TextBox_Carencia.Text != String.Empty) {
                    carencia = int.Parse(TextBox_Carencia.Text);
                }
                juros.OpenCalc(DateTime.Parse(DatePicker_Inicial.Text),
                    DateTime.Parse(DatePicker_Final.Text),
                    double.Parse(TextBox_Valor.Text),
                    double.Parse(TextBox_Juros.Text),
                    (bool) Switch_Type_Juros.IsChecked,
                    (bool) Swtich_Period_Juros.IsChecked,
                    carencia);

                juros.ExecuteCalc();

                Animate_Text(Value_Nominal, double.Parse(Value_Nominal.Text.Substring(2)), juros.Valor_Nominal);
                Animate_Text(Value_Total, double.Parse(Value_Total.Text.Substring(2)), juros.Valor_Total);
                Animate_Text(Value_Juros, double.Parse(Value_Juros.Text.Substring(2)), juros.Valor_Juros);
                Animate_Text(Total_Days, double.Parse(Total_Days.Text.Replace(" dias", "")), juros.Dias_Totais, false);
                
                if (AvisoCalcAuto.Height == 46) {
                    DoubleAnimation switchAnimation = new DoubleAnimation(46, 30, TimeSpan.FromMilliseconds(350));
                    AvisoCalcAuto.BeginAnimation(HeightProperty, switchAnimation);
                }
            }
            catch(Exception) {

                if(AvisoCalcAuto.Height == 30) {
                    DoubleAnimation switchAnimation = new DoubleAnimation(30, 46, TimeSpan.FromMilliseconds(350));
                    AvisoCalcAuto.BeginAnimation(HeightProperty, switchAnimation);
                }
            }
        }
        bool CalcAutoEnable = false;
        private void Check_Calc_Auto_Click(object sender, RoutedEventArgs e) {
            switch(Check_Calc_Auto.IsChecked) {
                case true:
                    CalcAutoEnable = true;
                    Calc_Auto();
                    break;
                case false:
                    CalcAutoEnable = false;  
                    if(AvisoCalcAuto.Height == 46) {
                        DoubleAnimation switchAnimation = new DoubleAnimation(46, 30, TimeSpan.FromMilliseconds(350));
                        AvisoCalcAuto.BeginAnimation(HeightProperty, switchAnimation);
                    }
                    break;
                default:
                    break;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {
            Calc_Auto();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            Calc_Auto();
        }

        private void Switch_Type_Click(object sender, RoutedEventArgs e) {
            Calc_Auto();
        }
    }
}
