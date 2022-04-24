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
            InitializeComponent();
            DataContext = this;
            if(Properties.Settings.Default.Date_Inicial != DateTime.MinValue) { DatePicker_Inicial.Text = Properties.Settings.Default.Date_Inicial.ToString(); }
            if(Properties.Settings.Default.Date_Final != DateTime.MinValue) { DatePicker_Final.Text = Properties.Settings.Default.Date_Final.ToString(); }
            if(Properties.Settings.Default.Juros != 0) { TextBox_Juros.Text = Properties.Settings.Default.Juros.ToString(); }
            if(Properties.Settings.Default.Carencia != 0) { TextBox_Carencia.Text = Properties.Settings.Default.Carencia.ToString(); }
            Check_Calc_Auto.IsChecked = Properties.Settings.Default.Calculo_Automatico;
            Switch_Type_Juros.IsChecked = Properties.Settings.Default.Type_Juros;
            Swtich_Period_Juros.IsChecked = Properties.Settings.Default.Type_Date;
            if(Properties.Settings.Default.Value != 0) { TextBox_Valor.Text = Properties.Settings.Default.Value.ToString(); }
            if(Check_Calc_Auto.IsChecked == true) {
                Check_Calc_Auto_Click(null, null);
            }
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
            await Task.Delay(505);
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
        bool calculing = false;
        private void Calc_Auto() {
            if (!CalcAutoEnable) { return; }
            try {
                if (calculing) { return; }
                calculing = true;
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
                Formatacao_Texto();

                Animate_Text(Value_Nominal, double.Parse(Value_Nominal.Text.Substring(2)), juros.Valor_Nominal);
                Animate_Text(Value_Total, double.Parse(Value_Total.Text.Substring(2)), juros.Valor_Total);
                Animate_Text(Value_Juros, double.Parse(Value_Juros.Text.Substring(2)), juros.Valor_Juros);
                Animate_Text(Total_Days, double.Parse(Total_Days.Text.Replace(" dias", "")), juros.Dias_Totais, false);

                Value_Nominal.ToolTip = juros.Valor_Nominal;
                Value_Total.ToolTip = juros.Valor_Total;
                Value_Juros.ToolTip = juros.Valor_Juros;
                
                if (AvisoCalcAuto.Height == 46) {
                    DoubleAnimation switchAnimation = new DoubleAnimation(46, 30, TimeSpan.FromMilliseconds(350));
                    AvisoCalcAuto.BeginAnimation(HeightProperty, switchAnimation);
                }
                calculing = false;
            }
            catch(Exception e) {
                if(AvisoCalcAuto.Height == 30) {
                    DoubleAnimation switchAnimation = new DoubleAnimation(30, 46, TimeSpan.FromMilliseconds(350));
                    AvisoCalcAuto.BeginAnimation(HeightProperty, switchAnimation);
                }
                calculing = false;
            }
        }
        bool CalcAutoEnable = false;
        private async void Formatacao_Texto() {
            await Task.Delay(300);
            if(Value_Nominal.Text.Length >= 11 && Value_Nominal.FontSize == 18) {
                DoubleAnimation doubleAnimationTextLenght = new DoubleAnimation(18, 14, TimeSpan.FromMilliseconds(350));
                Value_Nominal.BeginAnimation(FontSizeProperty, doubleAnimationTextLenght);
            }
            else if(Value_Nominal.Text.Length <= 11 && Value_Nominal.FontSize == 14) {
                DoubleAnimation doubleAnimationTextLenght = new DoubleAnimation(14, 18, TimeSpan.FromMilliseconds(350));
                Value_Nominal.BeginAnimation(FontSizeProperty, doubleAnimationTextLenght);
            }
            if(Value_Juros.Text.Length >= 11 && Value_Juros.FontSize == 18) {
                DoubleAnimation doubleAnimationTextLenght = new DoubleAnimation(18, 14, TimeSpan.FromMilliseconds(350));
                Value_Juros.BeginAnimation(FontSizeProperty, doubleAnimationTextLenght);
            }
            else if(Value_Juros.Text.Length <= 11 && Value_Juros.FontSize == 14) {
                DoubleAnimation doubleAnimationTextLenght = new DoubleAnimation(14, 18, TimeSpan.FromMilliseconds(350));
                Value_Juros.BeginAnimation(FontSizeProperty, doubleAnimationTextLenght);
            }
            if(Value_Total.Text.Length >= 14 && Value_Total.FontSize == 22) {
                DoubleAnimation doubleAnimationTextLenght = new DoubleAnimation(22, 18, TimeSpan.FromMilliseconds(350));
                Value_Total.BeginAnimation(FontSizeProperty, doubleAnimationTextLenght);
            }
            else if(Value_Total.Text.Length <= 14 && Value_Total.FontSize == 18) {
                DoubleAnimation doubleAnimationTextLenght = new DoubleAnimation(18, 22, TimeSpan.FromMilliseconds(350));
                Value_Total.BeginAnimation(FontSizeProperty, doubleAnimationTextLenght);
            }
            else if(Value_Total.Text.Length >= 17 && Value_Total.FontSize == 18) {
                DoubleAnimation doubleAnimationTextLenght = new DoubleAnimation(18, 14, TimeSpan.FromMilliseconds(350));
                Value_Total.BeginAnimation(FontSizeProperty, doubleAnimationTextLenght);
            }
            else if(Value_Total.Text.Length <= 17 && Value_Total.FontSize == 14) {
                DoubleAnimation doubleAnimationTextLenght = new DoubleAnimation(14, 18, TimeSpan.FromMilliseconds(350));
                Value_Total.BeginAnimation(FontSizeProperty, doubleAnimationTextLenght);
            }
        }
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            try {
                DateTime temp = new DateTime();
                Properties.Settings.Default.Date_Inicial = DateTime.TryParse(DatePicker_Inicial.Text, out temp) ? temp : DateTime.MinValue;
                Properties.Settings.Default.Date_Final = DateTime.TryParse(DatePicker_Final.Text, out temp) ? temp : DateTime.MinValue;
                Properties.Settings.Default.Juros = TextBox_Juros.Text == String.Empty ? 0 : double.Parse(TextBox_Juros.Text);
                Properties.Settings.Default.Carencia = TextBox_Carencia.Text == String.Empty ? 0 : int.Parse(TextBox_Carencia.Text);
                Properties.Settings.Default.Value = TextBox_Valor.Text == String.Empty ? 0 : double.Parse(TextBox_Valor.Text);
                Properties.Settings.Default.Type_Date = (bool) Swtich_Period_Juros.IsChecked;
                Properties.Settings.Default.Type_Juros = (bool) Swtich_Period_Juros.IsChecked;
                Properties.Settings.Default.Calculo_Automatico = (bool) Check_Calc_Auto.IsChecked;
                Properties.Settings.Default.Save();
            }
            catch(Exception) {
                Console.WriteLine("Ocorreu um erro");
            }
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e) {
            Animate_Text(Value_Nominal, double.Parse(Value_Nominal.Text.Substring(2)), 0);
            Animate_Text(Value_Total, double.Parse(Value_Total.Text.Substring(2)), 0);
            Animate_Text(Value_Juros, double.Parse(Value_Juros.Text.Substring(2)), 0);
            Animate_Text(Total_Days, double.Parse(Total_Days.Text.Replace(" dias", "")), 0, false);
            DatePicker_Inicial.Text = "";
            DatePicker_Final.Text = "";
            TextBox_Juros.Text = "";
            TextBox_Carencia.Text = "";
            TextBox_Valor.Text = "";
        }
    }
}
