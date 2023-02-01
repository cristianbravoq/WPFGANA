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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFGANA.Classes;
using WPFGANA.Classes.UseFull;
using WPFGANA.Models;
using WPFGANA.Resources;
using WPFGANA.Services.ObjectIntegration;
using WPFGANA.ViewModel;

namespace WPFGANA.UserControls.BetPlay
{
    /// <summary>
    /// Lógica de interacción para ValidateUC.xaml
    /// </summary>
    public partial class ValidateUC : UserControl
    {

        private TransactionBetPlay Transaction;

        public ValidateUC(TransactionBetPlay Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            this.DataContext = Transaction;

            AdminPayPlus.SaveLog("ValidateUC", "entrando a la ejecucion", "OK", "", Transaction);


            TxtCedula.Text = string.Concat(Transaction.Document.ToString());
            TxtMonto.Text = string.Concat(String.Format("{0:C0}",Convert.ToDecimal(Transaction.Amount)));

            AdminPayPlus.SaveLog("ValidateUC", "Saliendo de la ejecucion", "OK", "", Transaction);
        }



        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_ContinuarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Payment,Transaction);
        }
    }
}
