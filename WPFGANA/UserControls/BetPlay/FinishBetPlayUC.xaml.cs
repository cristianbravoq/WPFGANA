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
using System.Threading;
using System.Reflection;
using WPFGANA.Services.Object;

namespace WPFGANA.UserControls.BetPlay
{
    /// <summary>
    /// Lógica de interacción para FinishBetPlayUC.xaml
    /// </summary>
    public partial class FinishBetPlayUC : UserControl
    {

         private TransactionBetPlay Transaction;

        public FinishBetPlayUC(TransactionBetPlay Ts)
        {



            InitializeComponent();           
            Transaction = Ts;
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

                    Utilities.PrintVoucherBetPlay(this.Transaction);

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

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("SuccesUserControl", "Error Catch la ejecucion FinishTransaction", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        private void Btn_salirTouchDown(object sender, TouchEventArgs e)
        {
            AdminPayPlus.SaveLog("SussesUserControl", "Calificacion:", Transaction.calificacion, "OK", Transaction);
            Utilities.navigator.Navigate(UserControlView.Menu);

        }

        private void Btn_Calificacion(object sender, TouchEventArgs e)
        {

            try
            {
                Transaction.calificacion = "";

                int Tag = Convert.ToInt32((sender as Image).Tag);

                switch (Tag)
                {
                    case 1:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Hidden;
                        StarS3.Visibility = Visibility.Hidden;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "1";

                        break;

                    case 2:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Hidden;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "2";
                        break;
                    case 3:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "3";
                        break;
                    case 4:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Visible;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "4";

                        break;

                    case 5:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Visible;
                        StarS5.Visibility = Visibility.Visible;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "5";
                        break;
                }
            }
            catch (Exception Ex)
            {

            }
        }
    }
}
