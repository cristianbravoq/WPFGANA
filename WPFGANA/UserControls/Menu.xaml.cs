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
using WPFGANA.Classes.Printer;
using WPFGANA.Classes.UseFull;
using WPFGANA.Models;
using WPFGANA.Resources;
using WPFGANA.Services.ObjectIntegration;
using WPFGANA.ViewModel;

namespace WPFGANA.UserControls
{
    /// <summary>
    /// Lógica de interacción para Main.xaml
    /// </summary>
    
    public partial class Main : UserControl
    {

        private static PrintService _printService;

        public static PrintService PrintService
        {
            get { return _printService; }
        }

        public Main()
        {
            try
            {
                InitializeComponent();

                if (_printService == null)
                {
                    _printService = new PrintService();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }        
        }



        private void Btn_BetPlayTouchDown(object sender, TouchEventArgs e)
        {
            if (ValidarServiciosBetPlay())
            {
                Utilities.navigator.Navigate(UserControlView.Login);
            }
            else
            {
                Utilities.navigator.Navigate(UserControlView.Main);
            }
        }

        private bool ValidarServiciosBetPlay()
        {

            try
            {

                AdminPayPlus.SaveLog("MenuUC", "entrando a la ejecucion ValidateService", "OK", "", null);


                var Respuesta = AdminPayPlus.ApiIntegration.ValidateServicesBetPlay();

                if (Respuesta.ResponseCode.ToString() == "OK")
                {
                    var Respuesta2 = AdminPayPlus.ApiIntegration.ValidateTokenBetPlay();

                    if (Respuesta2.ResponseCode.ToString() == "OK")
                    {
                        AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion if true ValidateService", "OK", "", null);
                        return true;
                    }

                    Utilities.ShowModal("En estos Momentos los servicios de BetPlay no estan Disponibles", EModalType.Error);
                    AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion ValidateService if not true", "OK", "", null);
                    return false;
                }
                else
                {
                    Utilities.ShowModal("En estos Momentos los servicios de BetPlay no estan Disponibles", EModalType.Error);
                    AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion ValidateService if not true", "OK", "", null);
                    return false;
                }

            }
            catch(Exception ex)
            {
                Utilities.ShowModal("En estos Momentos los servicios de BetPlay no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("MenuUC", "Error Catch la ejecucion ValidateService", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                return false;
            }

        }

        private bool ValidarServiciosSuperChance()
        {

            try
            {

                AdminPayPlus.SaveLog("MenuUC", "entrando a la ejecucion ValidateService", "OK", "", null);


                var Respuesta = AdminPayPlus.ApiIntegration.ValidateServicesSuperChance();

                if (Respuesta.ResponseCode.ToString() == "OK")
                {
                    var Respuesta2 = AdminPayPlus.ApiIntegration.ValidateTokenSuperChance();

                    if (Respuesta2.ResponseCode.ToString() == "OK")
                    {
                        AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion if true ValidateService", "OK", "", null);
                        return true;
                    }

                    Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                    AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion ValidateService if not true", "OK", "", null);
                    return false;
                }
                else
                {
                    Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                    AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion ValidateService if not true", "OK", "", null);
                    return false;
                }

            }
            catch (Exception ex)
            {
                Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("MenuUC", "Error Catch la ejecucion ValidateService", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                return false;
            }

        }

        private void Btn_Jugar(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.info);
        }

        private void Btn_RechargeTouchDown(object sender, TouchEventArgs e)
        {
            if (LoginMachine())
            {
                Utilities.navigator.Navigate(UserControlView.SelectOption);
                //  GetOperators();
            }
        }

        private bool LoginMachine()
        {
            try
            {
               
                    var Respuesta = AdminPayPlus.ApiIntegration.LoginMachine();

                    if (Respuesta.ResponseCode.ToString() == "OK")
                    {
                        AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion if true LoginMachine", "OK", "", null);
                        return true;
                    }

                    Utilities.ShowModal("En estos Momentos los servicios de Recargas no estan Disponibles", EModalType.Error);
                    AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion LoginMachine if not true", "OK", "", null);
                    return false;
                
            }
            catch (Exception ex)
            {
                Utilities.ShowModal("En estos Momentos los servicios de Recargas no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("MenuUC", "Error Catch la ejecucion LoginMachine", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                return false;
            }
        }

       

    }
}
