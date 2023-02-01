using Newtonsoft.Json;
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
    /// Lógica de interacción para PaymentUC.xaml
    /// </summary>
    public partial class PaymentUC : UserControl
    {

        private TransactionBetPlay Transaction;
        private ETypeTramites typeTramites;
       public PaymentViewModel paymentViewModel;
        private int Intentos = 1;

        public PaymentUC(TransactionBetPlay Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            this.DataContext = Transaction;
            OrganizeValues();

        }

        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            CancellPay();
        }

        private void OrganizeValues()
        {
            try
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion OrganizeValues", "OK", "", Transaction);

                Transaction.Amount = Utilities.RoundValue(Convert.ToDecimal(Transaction.Amount)).ToString();

                this.paymentViewModel = new PaymentViewModel
                {
                    PayValue = Convert.ToDecimal(Transaction.Amount),
                    ValorFaltante = Convert.ToDecimal(Transaction.Amount),
                    ImgContinue = Visibility.Hidden,
                    ImgCancel = Visibility.Visible,
                    ImgCambio = Visibility.Hidden,
                    ValorSobrante = 0,
                    ValorIngresado = 0,
                    viewList = new CollectionViewSource(),
                    Denominations = new List<DenominationMoney>(),
                    ValorDispensado = 0
                };

                this.DataContext = this.paymentViewModel;

                ActivateWallet();
                //SavePay();
                //SavePayConceptos();

                AdminPayPlus.SaveLog("PaymentUserControl", "saliendo de la ejecucion OrganizeValues", "OK", "", Transaction);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion OrganizeValues", " Error", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

    
    private void ActivateWallet()
        {
            try
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion ActivaWallet", "OK", "", Transaction);



                AdminPayPlus.ControlPeripherals.callbackValueIn = enterValue =>
                {
                    AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion CallbackValueIn", "OK", enterValue.ToString(), Transaction);

               //     cancelar.Visibility = Visibility.Hidden;

                    if (enterValue.Item1 > 0)
                    {
                        if (!this.paymentViewModel.StatePay)
                        {
                            paymentViewModel.ValorIngresado += enterValue.Item1;

                            paymentViewModel.RefreshListDenomination(int.Parse(enterValue.Item1.ToString()), 1, enterValue.Item2);

                            AdminPayPlus.SaveDetailsTransaction(Transaction.IdTransactionAPi, enterValue.Item1, 2, 1, enterValue.Item2, string.Empty);
                           // LoadView();
                        }
                    }

                    AdminPayPlus.SaveLog("PaymentUserControl", "saliendo de la ejecucion CallbackValueIn", "OK", enterValue.ToString(), Transaction);


                };

                AdminPayPlus.ControlPeripherals.callbackTotalIn = enterTotal =>
                {
                    AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion CallBackTotalin", "OK", enterTotal.ToString(), Transaction);

                    if (!this.paymentViewModel.StatePay)
                    {
                        this.paymentViewModel.ImgCancel = Visibility.Hidden;
                  //      cancelar.Visibility = Visibility.Hidden;

                        AdminPayPlus.ControlPeripherals.StopAceptance();

                        if (enterTotal > 0 && paymentViewModel.ValorSobrante > 0)
                        {
                            this.paymentViewModel.ImgCambio = Visibility.Visible;
                            SavePay();
                     //       ReturnMoney(paymentViewModel.ValorSobrante,true);
                        }
                        else
                        {
                            if(Transaction.eTypeTramites == ETypeTramites.BetPlay)
                            {
                                Transaction.tramite = "Recargas BetPlay";
                                SavePay();
                            }
                        }
                    }

                    AdminPayPlus.SaveLog("PaymentUserControl", "saliendo la ejecucion CallBackTotalin", "OK", enterTotal.ToString(), Transaction);

                };

                AdminPayPlus.ControlPeripherals.callbackError = error =>
                {

                    AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion CallbackError ActivateWallet Aceptador", "OK", error.ToString(), Transaction);


                    AdminPayPlus.SaveLog(new RequestLogDevice
                    {
                        Code = error.Item1,
                        Date = DateTime.Now,
                        Description = error.Item2,
                        Level = ELevelError.Medium,
                        TransactionId = Transaction.IdTransactionAPi
                    }, ELogType.Device);

                    AdminPayPlus.SaveLog("PaymentUserControl", "Saliendo de la ejecucion CallbackError ActivateWallet Aceptador", "OK", error.ToString(), Transaction);

                };

                AdminPayPlus.ControlPeripherals.StartAceptance(paymentViewModel.PayValue);
                //  });

              
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion ActivateWallet", "ERROR", string.Concat(ex.Message, ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void ReturnMoney(decimal returnValue, bool state)
        {
            try
            {

                AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion ReturnMoney", "OK", "", Transaction);


                AdminPayPlus.ControlPeripherals.callbackTotalOut = totalOut =>
                {

                    AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion callbackTotalOut", "OK", totalOut.ToString(), Transaction);


                    if (state)
                    {

                        AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion callbackTotalOut", "OK", totalOut.ToString(), Transaction);

                        paymentViewModel.ValorDispensado = totalOut;
                        Transaction.StateReturnMoney = false;

                        if (paymentViewModel.ValorDispensado == paymentViewModel.ValorSobrante)
                        {
                            Transaction.StateReturnMoney = true;
                            if (Transaction.eTypeTramites == ETypeTramites.BetPlay)
                            {
                                AdminPayPlus.SaveLog("PaymentUserControl", "Saliendo de la ejecucion callbackTotalOut", "OK", totalOut.ToString(), Transaction);
                                SavePay();
                            }                          

                        }
                        else
                        {
                            Transaction.Observation += MessageResource.IncompleteMony + " " + "Devolvio: " + totalOut.ToString();
                            Utilities.ShowModal("No se pudo devolver el dinero completamente. devolvió " + string.Format("{0:C0}", totalOut) + " Por favor comuniquese con un administrador.", EModalType.Error);
                            AdminPayPlus.SaveLog("PaymentUserControl", "Saliendo de la ejecucion callbackTotalOut", "OK", totalOut.ToString(), Transaction);
                            SavePay(ETransactionState.Error);
                        }
                    }
                };

                AdminPayPlus.ControlPeripherals.callbackLog = log =>
                {

                    AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion callbacklog", "OK", log, Transaction);


                    paymentViewModel.SplitDenomination(log);
                    AdminPayPlus.SaveDetailsTransaction(Transaction.IdTransactionAPi, 0, 0, 0, string.Empty, log);

                    AdminPayPlus.SaveLog("PaymentUserControl", "Saliendo a la ejecucion callbacklog", "OK", log, Transaction);

                };

                AdminPayPlus.ControlPeripherals.callbackOut = valueOut =>
                {

                    AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion callbackOut", "OK", valueOut.ToString(), Transaction);


                    AdminPayPlus.ControlPeripherals.callbackOut = null;
                    if (state)
                    {
                        paymentViewModel.ValorDispensado = valueOut;
                        Transaction.StateReturnMoney = false;

                        if (paymentViewModel.ValorDispensado == paymentViewModel.ValorSobrante)
                        {
                            Transaction.StateReturnMoney = true;
                            if (Transaction.eTypeTramites == ETypeTramites.BetPlay)
                            {
                                SavePay();
                            }
                       
                        }
                        else
                        {
                            Transaction.Observation += MessageResource.IncompleteMony + " " + "Devolvio: " + valueOut.ToString();
                            Utilities.ShowModal("No se pudo devolver el dinero completamente. devolvió " + string.Format("{0:C0}", valueOut) + " Por favor comuniquese con un administrador.", EModalType.Error);
                            SavePay(ETransactionState.Error);
                        }

                        AdminPayPlus.SaveLog("PaymentUserControl", "saliendo a la ejecucion callbackOut", "OK", valueOut.ToString(), Transaction);

                    }
                };
                AdminPayPlus.SaveLog("PaymentUserControl", "saliendo de la ejecucion ReturnMoney", "OK", string.Concat("Valor a Retornar:" + returnValue), Transaction);


                AdminPayPlus.ControlPeripherals.StartDispenser(returnValue);
             
            }
            catch (Exception ex)
            {
                Utilities.navigator.Navigate(UserControlView.Menu);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }



        private void CancellPay()
        {
            try
            {

                AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion CancellPay", "OK", "", Transaction);


                this.paymentViewModel.ImgContinue = Visibility.Hidden;

                this.paymentViewModel.ImgCancel = Visibility.Hidden;

                if (Utilities.ShowModal(MessageResource.CancelTransaction, EModalType.Information))
                {
                    AdminPayPlus.ControlPeripherals.StopAceptance();
                    AdminPayPlus.ControlPeripherals.callbackLog = null;
                    if (!this.paymentViewModel.StatePay)
                    {
                        if (paymentViewModel.ValorIngresado > 0)
                        {
                            Transaction.Payment = paymentViewModel;
                           
                        }
                        else
                        {
                            Transaction.Payment = paymentViewModel;
                            FinishCancelNotPay();
                        }
                    }
                }
                else
                {
                    if (paymentViewModel.ValorIngresado > 0)
                    {
                        this.paymentViewModel.ImgContinue = Visibility.Visible;
                    }

                    this.paymentViewModel.ImgCancel = Visibility.Visible;
                }

                AdminPayPlus.SaveLog("PaymentUserControl", "saliendo de la ejecucion CancellPay", "OK", "", Transaction);


            }

            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion CancellPay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

    private async void SavePay(ETransactionState statePay = ETransactionState.Initial)
        {
            try
            {

                

                AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion SavePay", "OK", "", Transaction);


                if (!this.paymentViewModel.StatePay)
                {
                    this.paymentViewModel.StatePay = true;
                    Transaction.Payment = paymentViewModel;
                    Transaction.State = statePay;

                    AdminPayPlus.ControlPeripherals.ClearValues();

                    Notify();                    



                    GC.Collect();
                }



            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion SavePay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
             //   CancelTransaction();
            }
        
        }

        private void FinishCancelNotPay()
        {
            try

            {

                AdminPayPlus.SaveLog("ReturnMonyUserControl", "entrando a la ejecucion FinishCancelNotPay", "OK", "", Transaction);


                AdminPayPlus.ControlPeripherals.ClearValues();

                if (!string.IsNullOrEmpty(Transaction.Observation))
                {
                    AdminPayPlus.SaveErrorControl(Transaction.Observation, "", EError.Device, ELevelError.Medium);
                }

                Transaction.State = ETransactionState.Cancel;

                Transaction.StatePay = "Cancelada";

                AdminPayPlus.SaveLog("ReturnMonyUserControl", "FinishCancelNotPay", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", Transaction.StatePay.ToString(), "/n", "Monto:", Transaction.Amount.ToString(), "/n", "Valor Dispensado:", Transaction.Payment.ValorDispensado.ToString(), "/n", "Valor Ingresado:", Transaction.Payment.ValorIngresado.ToString()), Transaction);


                AdminPayPlus.UpdateTransaction(Transaction);

                AdminPayPlus.SaveLog("ReturnMonyUserControl", "Saliendo de la ejecucion FinishCancelPay", "OK", "", Transaction);


                   Utilities.navigator.Navigate(UserControlView.Menu);
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion FinishCancelPay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);

                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        public void Notify()
        {

            try
            {

                Task.Run(() =>             
               {

           //        Utilities.ShowModal("Estamos validando información un momento por favor", EModalType.Preload);
                  
                   AdminPayPlus.SaveLog("PaymentUC", "entrando a la ejecucion ValidateUser", "OK", "", null);

                    RequestNotify Data = new RequestNotify
                    {
                    CedulaApostador =  Convert.ToInt64(Transaction.Document),
                    codigoVendedor = Transaction.CodigoVendedor,
                    CodigoPuntoVenta = Transaction.codigoPuntoVenta,
                    serieTerminal = Transaction.SerieTerminal,
                    dane = Convert.ToInt32(Utilities.GetConfiguration("dane")),
                    MontoRecargar = Convert.ToInt32(Transaction.Amount)

                    };



                   Utilities.CloseModal();
                   var Respuesta = AdminPayPlus.ApiIntegration.NotifyPayment(Data);

                   if(Respuesta.ResponseCode.ToString() != "Error")
                   {
                       var ResponseData = JsonConvert.DeserializeObject<ResponseNotify>(Respuesta.ResponseData.ToString());

                       AdminPayPlus.SaveLog("PaymentUC", "Respuesta del servicio ValidateUser", "OK", ResponseData.ToString(), null);


                    


                       if (Respuesta != null)
                       {
                           if (Respuesta.ResponseCode.ToString() == "OK")
                           {

                               Transaction.Notify = ResponseData;

                               this.Transaction.statePaySuccess = true;
                               Transaction.State = ETransactionState.Success;
                               AdminPayPlus.SaveLog("PaymentUserControl", "saliendo de la ejecucion SavePay", "OK", "", Transaction);
                               ReturnMoney(paymentViewModel.ValorSobrante, true);
                               Utilities.CloseModal();
                               Utilities.navigator.Navigate(UserControlView.Finish, Transaction);

                              // Utilities.CloseModal();
                               //            return true;
                           }
                           else
                           {
                               Utilities.CloseModal();
                               Utilities.ShowModal("No se pudo notificar el pago", EModalType.Error);     
                               Utilities.navigator.Navigate(UserControlView.ReturnMoney, Transaction);
                               //              return false;
                           }
                       }
                       else
                       {
                           Utilities.CloseModal();
                           Utilities.ShowModal("En estos Momentos los servicios de BetPlay no estan Disponibles", EModalType.Error);
                           Utilities.navigator.Navigate(UserControlView.Menu);
                           //            return false;
                       }
                   }
                   else
                   {

                       Utilities.CloseModal();
                       Utilities.ShowModal("No se encontro información para el usuario consultado", EModalType.Error);
                       Utilities.CloseModal();
                       Utilities.navigator.Navigate(UserControlView.ReturnMoney, Transaction);



                   }
                  


                  

                });

                Utilities.ShowModal("Estamos validando información un momento por favor", EModalType.Preload);


            }
            catch (Exception ex)
            {

                Utilities.ShowModal("No se encontro información para el usuario consultado", EModalType.Error); 
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
          //      return false;
            }

        //    return false;
        }

       

    

}

    
}
