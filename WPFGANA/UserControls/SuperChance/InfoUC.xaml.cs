using Newtonsoft.Json;
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

namespace WPFGANA.UserControls.SuperChance
{
    /// <summary>
    /// Lógica de interacción para InfoUC.xaml
    /// </summary>
    public partial class InfoUC : UserControl
    {

        TransactionBetPlay Transaction;

        public InfoUC()
        {
            InitializeComponent();
            Transaction = new TransactionBetPlay();
        }

        private void Btn_Jugar_TouchDown(object sender, TouchEventArgs e)
        {
            GuardarParametros();
        }

        public void GuardarParametros()
        {

            try
            {

                //string data = Utilities.GetConfiguration("Validate");



                AdminPayPlus.SaveLog("DateTxUC", "entrando a la ejecucion GetLotteries", "OK", "", null);

                Task.Run(() =>
                {

                    try
                    {
                        RequestParameters Data = new RequestParameters
                        {
                            idUsuarioVendedor = Convert.ToInt32(Utilities.GetConfiguration("IdMaquina")),
                        };


                        var Respuesta = AdminPayPlus.ApiIntegration.CargarParametros(Data);



                        var ResponseData = JsonConvert.DeserializeObject<ResponseParameters>(Respuesta.ResponseData.ToString());

                        AdminPayPlus.SaveLog("DateTxUC", "Respuesta del servicio ResponseParameters", "OK", String.Concat(ResponseData), null);


                        if (Respuesta != null)
                        {
                            if (ResponseData.ok == true)
                            {
                                Transaction.Parametros = ResponseData;

                                Utilities.CloseModal();

                                Utilities.navigator.Navigate(UserControlView.Form, Transaction);
                            }
                            else
                            {
                                Utilities.CloseModal();
                                Utilities.ShowModal("No se pudo obtener los parámetros", EModalType.Error);
                            }
                        }
                        else
                        {
                            Utilities.CloseModal();
                            Utilities.ShowModal("En estos momentos los servicios de Super Chance no están disponibles", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.Menu);
                        }

                    }
                    catch (Exception ex)
                    {
                        Utilities.CloseModal();
                        Utilities.ShowModal("En estos momentos los servicios de Super Chance no están disponibles", EModalType.Error);
                        AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }


                });

                Utilities.ShowModal("Estamos consultando la información,un momento por favor", EModalType.Preload);

            }
            catch (Exception ex)
            {
                Utilities.CloseModal();
                Utilities.ShowModal("En estos momentos los servicios de Super Chance no están disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }

        }



    }
}
