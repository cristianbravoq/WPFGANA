using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WPFGANA.UserControls.SuperChance
{
    /// <summary>
    /// Lógica de interacción para DateTxUC.xaml
    /// </summary>
    public partial class DateTxUC : UserControl
    {

        TransactionBetPlay Transaction;

        public string año = "";
        public DateTxUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
           
            Transaction = transaction;
            this.DataContext = Transaction;
            GetDays();

        }


        private void GetDays()
        {


            try
            {

                DateTime Fecha = new DateTime();

                Fecha = DateTime.Now;

                Day1.Content = "HOY";
                
                Mes.Content = Fecha.ToString("MMM", new CultureInfo("es-CO")).ToUpper();

              


                Day2.Content = Fecha.AddDays(1).ToString("dd");
                Mes2.Content = Fecha.AddDays(1).ToString("MMM",new CultureInfo("es-CO")).ToUpper();

                Day3.Content = Fecha.AddDays(2).ToString("dd");
                Mes3.Content = Fecha.AddDays(2).ToString("MMM", new CultureInfo("es-CO")).ToUpper();

                Day4.Content = Fecha.AddDays(3).ToString("dd");
                Mes4.Content = Fecha.AddDays(3).ToString("MMM", new CultureInfo("es-CO")).ToUpper();

                Day5.Content = Fecha.AddDays(4).ToString("dd");
                Mes5.Content = Fecha.AddDays(4).ToString("MMM", new CultureInfo("es-CO")).ToUpper();

                Day6.Content = Fecha.AddDays(5).ToString("dd");
                Mes6.Content = Fecha.AddDays(5).ToString("MMM", new CultureInfo("es-CO")).ToUpper();



                // año = AÑO.Year.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_ContinuarTouchDown(object sender, TouchEventArgs e)
        {
            GetLotteries();
        }

        public void GetLotteries()
        {

            try
            {

                //string data = Utilities.GetConfiguration("Validate");



                AdminPayPlus.SaveLog("DateTxUC", "entrando a la ejecucion GetLotteries", "OK", "", null);

                Task.Run(() =>
                {

                    RequestFecha Data = new RequestFecha
                    {
                        fecha = Transaction.Fecha,
                        idProductoRed = 1
                    };


                    var Respuesta = AdminPayPlus.ApiIntegration.GetLotteries(Data);





                    if (Respuesta.ResponseCode.ToString() != "Error")
                    {
                        var ResponseData = JsonConvert.DeserializeObject<ResponseFecha>(Respuesta.ResponseData.ToString());

                        AdminPayPlus.SaveLog("DateTxUC", "Respuesta del servicio GetLotteries", "OK", String.Concat(ResponseData), null);


                        if (ResponseData.model.list != null)
                        {
                            Transaction.LotteryList = ResponseData;

                            Utilities.CloseModal();

                            Utilities.navigator.Navigate(UserControlView.Loterias, Transaction);
                        }
                        else
                        {
                            Utilities.ShowModal("No hay loterias disponibles para esta fecha", EModalType.Error);
                        }
                    }
                    else
                    {
                        Utilities.ShowModal("En estos momentos los servicios de Super Chance no están disponibles", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }

                });

                Utilities.ShowModal("Estamos consultando la información,un momento por favor", EModalType.Preload);

            }
            catch (Exception ex)
            {

                Utilities.ShowModal("En estos momentos los servicios de Super Chance no están disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }

        }

        private void Btn_SelectDay(object sender, TouchEventArgs e)
        {
            //int Tag = Convert.ToInt32((sender as Image).Tag);

            //switch (Tag)
            //{
            //    case 1:
            //        Transaction.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
            //        Dia1S.Visibility = Visibility.Visible;
            //        Dia2S.Visibility = Visibility.Hidden;
            //        Dia3S.Visibility = Visibility.Hidden;
            //        Dia4S.Visibility = Visibility.Hidden;
            //        Dia5S.Visibility = Visibility.Hidden;
            //        Dia6S.Visibility = Visibility.Hidden;

            //        break;
            //    case 2:
            //        Transaction.Fecha = string.Concat(año+"-"+Mes2.Content+"-"+Day2.Content);
            //        Dia1S.Visibility = Visibility.Hidden;
            //        Dia2S.Visibility = Visibility.Visible;
            //        Dia3S.Visibility = Visibility.Hidden;
            //        Dia4S.Visibility = Visibility.Hidden;
            //        Dia5S.Visibility = Visibility.Hidden;
            //        Dia6S.Visibility = Visibility.Hidden;
            //        break;
            //    case 3:
            //        Transaction.Fecha = string.Concat(año + "-" + Mes3.Content + "-" + Day3.Content);
            //        Dia1S.Visibility = Visibility.Hidden;
            //        Dia2S.Visibility = Visibility.Hidden;
            //        Dia3S.Visibility = Visibility.Visible;
            //        Dia4S.Visibility = Visibility.Hidden;
            //        Dia5S.Visibility = Visibility.Hidden;
            //        Dia6S.Visibility = Visibility.Hidden;
            //        break;
            //    case 4:
            //        Transaction.Fecha = string.Concat(año + "-" + Mes4.Content + "-" + Day4.Content);
            //        Dia1S.Visibility = Visibility.Hidden;
            //        Dia2S.Visibility = Visibility.Hidden;
            //        Dia3S.Visibility = Visibility.Hidden;
            //        Dia4S.Visibility = Visibility.Visible;
            //        Dia5S.Visibility = Visibility.Hidden;
            //        Dia6S.Visibility = Visibility.Hidden;
            //        break;
            //    case 5:
            //        Transaction.Fecha = string.Concat(año + "-" + Mes5.Content + "-" + Day5.Content);
            //        Dia1S.Visibility = Visibility.Hidden;
            //        Dia2S.Visibility = Visibility.Hidden;
            //        Dia3S.Visibility = Visibility.Hidden;
            //        Dia4S.Visibility = Visibility.Hidden;
            //        Dia5S.Visibility = Visibility.Visible;
            //        Dia6S.Visibility = Visibility.Hidden;
            //        break;
            //    case 6:
            //        Transaction.Fecha = string.Concat(año + "-" + Mes6.Content + "-" + Day6.Content);
            //        Dia1S.Visibility = Visibility.Hidden;
            //        Dia2S.Visibility = Visibility.Hidden;
            //        Dia3S.Visibility = Visibility.Hidden;
            //        Dia4S.Visibility = Visibility.Hidden;
            //        Dia5S.Visibility = Visibility.Hidden;
            //        Dia6S.Visibility = Visibility.Visible;
            //        break;

            //}
        }

        private void Btn_SelectDayH(object sender, TouchEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Label).Tag);

            switch (Tag)
            {

                case 11:
                    Transaction.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
                    Dia1S.Visibility = Visibility.Visible;
                    Dia2S.Visibility = Visibility.Hidden;
                    Dia3S.Visibility = Visibility.Hidden;
                    Dia4S.Visibility = Visibility.Hidden;
                    Dia5S.Visibility = Visibility.Hidden;
                    Dia6S.Visibility = Visibility.Hidden;

                    break;
                case 12:
                    Transaction.Fecha = string.Concat(año + "-" + Mes2.Content + "-" + Day2.Content);
                    Dia1S.Visibility = Visibility.Hidden;
                    Dia2S.Visibility = Visibility.Visible;
                    Dia3S.Visibility = Visibility.Hidden;
                    Dia4S.Visibility = Visibility.Hidden;
                    Dia5S.Visibility = Visibility.Hidden;
                    Dia6S.Visibility = Visibility.Hidden;
                    break;
                case 13:
                    Transaction.Fecha = string.Concat(año + "-" + Mes3.Content + "-" + Day3.Content);
                    Dia1S.Visibility = Visibility.Hidden;
                    Dia2S.Visibility = Visibility.Hidden;
                    Dia3S.Visibility = Visibility.Visible;
                    Dia4S.Visibility = Visibility.Hidden;
                    Dia5S.Visibility = Visibility.Hidden;
                    Dia6S.Visibility = Visibility.Hidden;
                    break;
                case 14:
                    Transaction.Fecha = string.Concat(año + "-" + Mes4.Content + "-" + Day4.Content);
                    Dia1S.Visibility = Visibility.Hidden;
                    Dia2S.Visibility = Visibility.Hidden;
                    Dia3S.Visibility = Visibility.Hidden;
                    Dia4S.Visibility = Visibility.Visible;
                    Dia5S.Visibility = Visibility.Hidden;
                    Dia6S.Visibility = Visibility.Hidden;
                    break;
                case 15:
                    Transaction.Fecha = string.Concat(año + "-" + Mes5.Content + "-" + Day5.Content);
                    Dia1S.Visibility = Visibility.Hidden;
                    Dia2S.Visibility = Visibility.Hidden;
                    Dia3S.Visibility = Visibility.Hidden;
                    Dia4S.Visibility = Visibility.Hidden;
                    Dia5S.Visibility = Visibility.Visible;
                    Dia6S.Visibility = Visibility.Hidden;
                    break;
                case 16:
                    Transaction.Fecha = string.Concat(año + "-" + Mes6.Content + "-" + Day6.Content);
                    Dia1S.Visibility = Visibility.Hidden;
                    Dia2S.Visibility = Visibility.Hidden;
                    Dia3S.Visibility = Visibility.Hidden;
                    Dia4S.Visibility = Visibility.Hidden;
                    Dia5S.Visibility = Visibility.Hidden;
                    Dia6S.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
