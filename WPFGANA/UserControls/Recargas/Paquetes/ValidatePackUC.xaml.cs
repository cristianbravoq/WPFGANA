using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFGANA.Classes;
using WPFGANA.Classes.UseFull;
using WPFGANA.Models;
using WPFGANA.Resources;
using WPFGANA.Services.ObjectIntegration;
using WPFGANA.ViewModel;

namespace WPFGANA.UserControls.Recargas.Paquetes
{
    /// <summary>
    /// Lógica de interacción para ValidatePackUC.xaml
    /// </summary>
    public partial class ValidatePackUC : UserControl
    {
        TransactionBetPlay Transaction;

        public ValidatePackUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            INFO.Text = Transaction.SelectOperator.nomPaquete;
            LblCelular.Content = Transaction.NumOperator;
            Precio.Content = string.Concat("$", transaction.SelectOperator.valorComercial);
        }

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void BtnContinue_TouchDown(object sender, TouchEventArgs e)
        {
            Transaction.Amount = Transaction.SelectOperator.valorComercial.ToString();
            Utilities.navigator.Navigate(UserControlView.PaymentRecharge, Transaction);
        }
    }
}
