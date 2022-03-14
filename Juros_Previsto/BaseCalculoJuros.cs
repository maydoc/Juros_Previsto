using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juros_Previsto {
    class BaseCalculoJuros {

        DateTime datainicio = new DateTime();
        DateTime datafinal = new DateTime();
        double valor = 0.00;
        double juros = 0.00;
        bool jurosvalor = false;
        int carencia = 0;
        bool aodia = false;

        public double Valor_Total = 0.0;
        public double Valor_Nominal = 0.0;
        public double Valor_Juros = 0.0;
        public int Dias_Totais = 0;
        public string Dias_Formalizado = "";

        /// <summary>
        /// Inicia a classe e define os valores das variáveis para a base de cálculo do juros
        /// </summary>
        /// <param name="dtinicio">A data de ínicio</param>
        /// <param name="dtfinal">A data final</param>
        /// <param name="valor_parc">O valor da parcela</param>
        /// <param name="juros_rs">Valor do Juros (Porcentagem ou Valor)</param>
        /// <param name="jurostype">Condição verificadora do tipo de juros a se considerar</param>
        /// <param name="carencia_dias">Quantidade de dias de carência</param>
        /// <param name="periodojuros">Modo a se cobrar o juros (ao dia ou ao mês)</param>
        public void OpenCalc(DateTime dtinicio, DateTime dtfinal, double valor_parc, double juros_rs, bool jurostype, bool periodojuros, int carencia_dias = 0) {
            datainicio = dtinicio;
            datafinal = dtfinal;
            valor = valor_parc;
            juros = juros_rs;
            jurosvalor = jurostype;
            carencia = carencia_dias;
            aodia = periodojuros;
            Valor_Nominal = valor_parc;
        }
        public bool ExecuteCalc() {
            TimeSpan temp_date = datafinal.Subtract(datainicio);
            Dias_Totais = int.Parse(temp_date.TotalDays.ToString("N0"));
            if (Dias_Totais < carencia) {
                Valor_Juros = 0.00;
                Valor_Total = valor;
                return true;
            }
            double porcentagemjuros = 0.00;
            double valorjuros = juros;
            if (!aodia) {
                porcentagemjuros = juros / 30;
                valorjuros = valorjuros / 30;
            }
            if (!jurosvalor) {
                porcentagemjuros = porcentagemjuros * Dias_Totais;
                double temp_const = porcentagemjuros / 100;
                double jurostemp = temp_const * valor;
                Valor_Juros = jurostemp;
                Valor_Total = valor + jurostemp;
            }
            else {
                double temp_const = valorjuros * Dias_Totais;
                Valor_Juros = temp_const;
                Valor_Total = valor + temp_const;
            }
            return true;
        }
    }
}
