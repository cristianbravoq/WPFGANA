using System;
using System.Collections.Generic;
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
using WPFGANA.Services.ObjectIntegration;
using WPFGANA.ViewModel;

namespace WPFGANA.UserControls.BetPlay
{
    /// <summary>
    /// Lógica de interacción para RechargeUC.xaml
    /// </summary>
    public partial class RechargeUC : UserControl
    {

        private TransactionBetPlay Transaction;
        public ValueModel value;


        public RechargeUC(TransactionBetPlay Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            this.DataContext = Transaction;
            AdminPayPlus.SaveLog("RechargeUC", "entrando a la ejecucion", "OK", "", Transaction);

            value = new ValueModel
            {
                Val = 0
            };

            this.DataContext = value;


            AdminPayPlus.SaveLog("RechargeUC", "Saliendo de la ejecucion", "OK", "", Transaction);
        }

        private void Btn_DeleteAllTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                TxtMonto.Text = string.Empty;
                
            }
            catch (Exception ex)
            {
               
            }
        }

        private void Keyboard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Image image = (Image)sender;
                string Tag = image.Tag.ToString();
                TxtMonto.Text += Tag;
            }
            catch (Exception ex)
            {

            }
        }

        private void Btn_DeleteTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string val = TxtMonto.Text;

                if (val.Length > 0)
                {
                    TxtMonto.Text = val.Remove(val.Length - 1);
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        private bool Validate()
        {

            string value = TxtMonto.Text.Replace("$", "");
            value = value.Replace(",", "");

            if (TxtMonto.Text != string.Empty)
            {
                Transaction.Amount = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Btn_ContinuarTouchDown(object sender, TouchEventArgs e)
        {

            if (Validate())
            {
           
                SendData();
            }
            else
            {
                Utilities.navigator.Navigate(UserControlView.Recharge);
            }

            
        }

        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private async void SendData()
        {
            try
            {

                AdminPayPlus.SaveLog("RechargeUC", "entrando a la ejecucion SendData", "OK", "", Transaction);

                Task.Run(async () =>
                {
                    Transaction.payer = new DataModel.PAYER
                    {
                        IDENTIFICATION = Transaction.Document,
                        NAME = Transaction.Name,

                        STATE = Transaction.statePaySuccess,

                    };
                   Transaction.State = ETransactionState.Initial;
                    Transaction.Type = ETypeTramites.BetPlay;
                   Transaction.Tipo = ETransactionType.Payment;
                    Transaction.eTypeTramites = ETypeTramites.BetPlay;
                    Transaction.valor = Transaction.Amount;

                    await AdminPayPlus.SaveTransaction(Transaction);

                    AdminPayPlus.SaveLog("RechargeUC", "SendData", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "inicial", "/n", "Monto:", Transaction.Amount.ToString()), Transaction);


                    Utilities.CloseModal();

                    if (this.Transaction.IdTransactionAPi == 0)
                    {
                        Utilities.ShowModal("No se puede guardar la transacción, intentelo más tarde.", EModalType.Error);

                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                    else
                    {
                        Utilities.navigator.Navigate(UserControlView.Validate, Transaction);
                    }
                });
            //    Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);

              
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("RechargeUC", "Error Catch la ejecucion SendData", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void txtVal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TxtMonto.Text.Length > 15)
            {
                TxtMonto.Text = TxtMonto.Text.Remove(15, 1);
                return;
            }
        }
    }
}
