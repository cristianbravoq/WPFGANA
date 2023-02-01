using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using WPFGANA.Services.Object;
using WPFGANA.Services.ObjectIntegration;
using WPFGANA.ViewModel;

namespace WPFGANA.UserControls.Recargas.Recargas
{
    /// <summary>
    /// Lógica de interacción para ValidateInfoUC.xaml
    /// </summary>
    public partial class ValidateInfoUC : UserControl
    {

        TransactionBetPlay Transaction;

        public ValidateInfoUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            LblCelular.Content = transaction.NumOperator;
            Precio.Content = string.Concat("$",transaction.Amount);
        }

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void BtnContinue_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.PaymentRecharge, Transaction);
        }
    }
}
