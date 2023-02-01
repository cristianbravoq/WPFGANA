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
using WPFGANA.Services.Object;
using WPFGANA.Services.ObjectIntegration;
using WPFGANA.ViewModel;

namespace WPFGANA.UserControls.Recargas
{
    /// <summary>
    /// Lógica de interacción para SuccesTransactionUC.xaml
    /// </summary>
    public partial class SuccesTransactionUC : UserControl
    {
        private TransactionBetPlay Transaction;

        public SuccesTransactionUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            this.DataContext = Transaction;

            AdminPayPlus.SaveLog("FinishBetPlay", "entrando a la ejecucion", "OK", "", Transaction);

            FinishTransaction();

            AdminPayPlus.SaveLog("FinishBetPlay", "Saliendo de la ejecucion", "OK", "", Transaction);

        }

        public void FinishTransaction()
        {
            try
            {

                AdminPayPlus.SaveLog("FinishBetPlay", "entrando a la ejecucion FinishTransaction", "OK", "", Transaction);

                if (Transaction.State == ETransactionState.Success)
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        //  Description = Transaction.Notify.ToString(),
                        Reference = Transaction.IdTransactionAPi.ToString()
                    }, ELogType.General);
                }
                else
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Description = MessageResource.NoveltyTransation,
                        Reference = Transaction.IdTransactionAPi.ToString()

                    }, ELogType.General);
                }

                Transaction.State = ETransactionState.Success;

                // Task.Run(() =>
                //{
                AdminPayPlus.UpdateTransaction(Transaction);

                AdminPayPlus.SaveLog("SuccesUserControl", "FinishTransaction", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "Aprobada", "/n", "Monto:", Transaction.Amount.ToString(), "/n", "Valor Dispensado:", Transaction.Payment.ValorDispensado.ToString(), "/n", "Valor Ingresado:", Transaction.Payment.ValorIngresado.ToString()), Transaction);

                Transaction.StatePay = "Aprobado";

                Utilities.PrintVoucherRecargas(this.Transaction);

                //  Thread.Sleep(8000);
                //      Utilities.PrintVoucherBetPlay(this.Transaction);

                //    Thread.Sleep(6000);

                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (Transaction.State == ETransactionState.Error)
                    {
                        Utilities.RestartApp();
                    }
                    else
                    {
                        Utilities.navigator.Navigate(UserControlView.Main);
                    }

                });
                GC.Collect();
                // });

                AdminPayPlus.SaveLog("SussesUserControl", "Saliendo de la ejecucion FinishTransaction", "OK", "", Transaction);

                Utilities.navigator.Navigate(UserControlView.Menu);


            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("SuccesUserControl", "Error Catch la ejecucion FinishTransaction", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }
    }
}
