using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

namespace WPFGANA.UserControls.BetPlay
{
    /// <summary>
    /// Lógica de interacción para ReturnMoneyUC.xaml
    /// </summary>
    public partial class ReturnMoneyUC : UserControl
    {
        private TransactionBetPlay transaction;
        private decimal ValueReturn;

        public ReturnMoneyUC(TransactionBetPlay transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            ValueReturn = 0;

            if (this.transaction.Payment.ValorIngresado > 0)
            {
                ReturnMoney();
            }
            else
            {
              //  FinishCancelNotPay();
            //    Utilities.navigator.Navigate(UserControlView.Menu);

            }
        }

        private void ReturnMoney()
        {
            int i = 0;
            int j = 0;
            int x = 0;
            try
            {

                ValueReturn = transaction.Payment.ValorIngresado - transaction.Payment.ValorDispensado;
                AdminPayPlus.SaveLog("ReturnMonyUserControl", "entrando a la ejecucion ReturnMoney", "OK", "", transaction);

                txtValueReturn.Text = string.Format("{0:C0}", ValueReturn);
                //Task.Run(() =>
                //{
                AdminPayPlus.ControlPeripherals.callbackTotalOut = totalOut =>
                {
                    i++;
                    AdminPayPlus.SaveLog("ReturnMonyUserControl", "entrando a la ejecucion callbackTotalOut", "OK", totalOut.ToString(), transaction);

                    transaction.StateReturnMoney = true;

                    transaction.Payment.ValorDispensado = totalOut;
                    transaction.Payment.ValorSobrante = transaction.Payment.ValorIngresado;
                    transaction.State = ETransactionState.Cancel;
                    AdminPayPlus.SaveLog("ReturnMonyUserControl", "Saliendo de la ejecucion callbackTotalOut", "OK", totalOut.ToString(), transaction);

                    FinishCancelPay();

                };

                AdminPayPlus.ControlPeripherals.callbackError = error =>
                {
                    j++;
                    AdminPayPlus.SaveLog("ReturnMonyUserControl", "entrando a la ejecucion callbackError", "OK", error.ToString(), transaction);

                    AdminPayPlus.SaveLog(new RequestLogDevice
                    {
                        Code = error.Item1,
                        Date = DateTime.Now,
                        Description = error.Item2,
                        Level = ELevelError.Medium,
                        TransactionId = transaction.IdTransactionAPi
                    }, ELogType.Device);

                    AdminPayPlus.SaveLog("ReturnMonyUserControl", "Saliendo de la ejecucion callbackError", "OK", error.ToString(), transaction);

                };

                AdminPayPlus.ControlPeripherals.callbackOut = valueOut =>
                {
                    x++;
                                         AdminPayPlus.SaveLog("ReturnMonyUserControl", "entrando a la ejecucion callbackOut", "OK", valueOut.ToString(), transaction);

                    AdminPayPlus.ControlPeripherals.callbackOut = null;
                    transaction.Payment.ValorDispensado = valueOut;

                    transaction.StateReturnMoney = false;

                    if (!transaction.statePaySuccess && transaction.Payment.ValorDispensado != transaction.Payment.ValorIngresado)
                    {
                        transaction.State = ETransactionState.CancelError;
                        transaction.Observation += MessageResource.IncompleteMony + " " + "Devolvio: " + valueOut.ToString();
                    }
                    else
                    {
                        transaction.StateReturnMoney = true;
                    }
                    FinishCancelPay();
                    AdminPayPlus.SaveLog("ReturnMonyUserControl", "Saliendo de la ejecucion callbackOut", "OK", valueOut.ToString(), transaction);



                };

                AdminPayPlus.ControlPeripherals.callbackLog = log =>
                {
                    AdminPayPlus.SaveLog("ReturnMonyUserControl", "entrando a la ejecucion callbackLog", "OK", log, transaction);

                    AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, 0, 0, 0, string.Empty, log);
                    AdminPayPlus.SaveLog("ReturnMonyUserControl", "Saliendo de la ejecucion callbackLog", "OK", log, transaction);


                };

                AdminPayPlus.ControlPeripherals.StartDispenser(ValueReturn);

                AdminPayPlus.SaveLog("ReturnMonyUserControl", "saliendo de la ejecucion ReturnMoney", "OK", "", transaction);

                //});
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion ReturnMoney", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), transaction);

                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void FinishCancelPay()
        {
            try

            {
                AdminPayPlus.SaveLog("ReturnMonyUserControl", "entrando a la ejecucion FinishCancelPay", "OK", "", transaction);


                AdminPayPlus.ControlPeripherals.ClearValues();

                if (!string.IsNullOrEmpty(transaction.Observation))
                {
                    AdminPayPlus.SaveErrorControl(transaction.Observation, "", EError.Device, ELevelError.Medium);
                }

                transaction.State = ETransactionState.Cancel;

                transaction.StatePay = "Cancelada";

                AdminPayPlus.SaveLog("ReturnMonyUserControl", "FinishCancelPay", "OK", string.Concat("ID Transaccion:", transaction.IdTransactionAPi, "/n", "Estado Transaccion:", transaction.StatePay.ToString(), "/n", "Monto:", transaction.Amount.ToString(), "/n", "Valor Dispensado:", transaction.Payment.ValorDispensado.ToString(), "/n", "Valor Ingresado:", transaction.Payment.ValorIngresado.ToString()), transaction);


                AdminPayPlus.UpdateTransaction(transaction);

                Utilities.PrintVoucherSuperChance(transaction);

                AdminPayPlus.SaveLog("ReturnMonyUserControl", "Saliendo de la ejecucion FinishCancelPay", "OK", "", transaction);


                Utilities.navigator.Navigate(UserControlView.Menu);


            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void FinishCancelNotPay()
        {
            try

            {

                AdminPayPlus.SaveLog("ReturnMonyUserControl", "entrando a la ejecucion FinishCancelNotPay", "OK", "", transaction);


                AdminPayPlus.ControlPeripherals.ClearValues();

                if (!string.IsNullOrEmpty(transaction.Observation))
                {
                    AdminPayPlus.SaveErrorControl(transaction.Observation, "", EError.Device, ELevelError.Medium);
                }

                transaction.State = ETransactionState.Cancel;

                transaction.StatePay = "Cancelada";

                AdminPayPlus.SaveLog("ReturnMonyUserControl", "FinishCancelNotPay", "OK", string.Concat("ID Transaccion:", transaction.IdTransactionAPi, "/n", "Estado Transaccion:", transaction.StatePay.ToString(), "/n", "Monto:", transaction.Amount.ToString(), "/n", "Valor Dispensado:", transaction.Payment.ValorDispensado.ToString(), "/n", "Valor Ingresado:", transaction.Payment.ValorIngresado.ToString()), transaction);


                AdminPayPlus.UpdateTransaction(transaction);

                AdminPayPlus.SaveLog("ReturnMonyUserControl", "Saliendo de la ejecucion FinishCancelPay", "OK", "", transaction);


             //   Utilities.navigator.Navigate(UserControlView.Menu);
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion FinishCancelPay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), transaction);

                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
    }
}
