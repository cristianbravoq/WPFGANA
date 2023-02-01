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

namespace WPFGANA.UserControls.BetPlay
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {

        private TransactionBetPlay Transaction;
        public bool txtcedula = false;
        public bool txtvalidar = false;

        public Login()
        {
             Transaction = new TransactionBetPlay();
             InitializeComponent();
            this.DataContext = Transaction;

            AdminPayPlus.SaveLog("LoginUC", "entrando a la ejecucion", "OK", "", null);

            ValidateToken();

            AdminPayPlus.SaveLog("loginUC", "Saliendo de la ejecucion", "OK", "", null);

        }


        public void ValidateToken()
        {

            try
            {
                AdminPayPlus.SaveLog("LoginUC", "entrando a la ejecucion ValidateToken", "OK", "", null);

                var Respuesta = AdminPayPlus.ApiIntegration.GetToken();

                if (Respuesta != null)
                {
                    if (Respuesta.ResponseCode.ToString() == "OK")
                    {
                        Transaction.Token = Respuesta.ResponseData.ToString();
                        AdminPayPlus.SaveLog("LoginUC", "Se genero el Token Correctamente", "OK", Transaction.Token, null);
                    }
                }
                else
                {
                    Utilities.ShowModal("En estos Momentos los servicios de BetPlay no estan Disponibles", EModalType.Error);
                    AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "OK", "", null);
                    Utilities.navigator.Navigate(UserControlView.Menu);
                }
            }
            catch(Exception ex)
            {
                Utilities.ShowModal("En estos Momentos los servicios de BetPlay no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "Error Catch la ejecucion ValidateToken", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        private void Keyboard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if(txtcedula == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtCedula.Text += Tag;
                }
                
                if(txtvalidar == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtValidate.Text += Tag;

                }

             
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Validate()
        {

            try
            {
                if (TxtValidate.Text != null && TxtValidate.Text != "" && TxtCedula.Text != null && TxtCedula.Text != "")
                {

                    if (TxtValidate.Text == TxtCedula.Text)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;              
            }          
        }

        private void Btn_DeleteTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string val = TxtCedula.Text;
                string val2 = TxtValidate.Text;

                if (txtcedula == true)
                {
                    if (val.Length > 0)
                    {
                        TxtCedula.Text = val.Remove(val.Length - 1);
                       // TxtValidate.Text = val2.Remove(val.Length - 1);
                    }
                }

                if (txtvalidar == true)
                {
                    if (val2.Length > 0)
                    {
                    //    TxtCedula.Text = val.Remove(val.Length - 1);
                        TxtValidate.Text = val2.Remove(val2.Length - 1);
                    }

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Btn_DeleteAllTouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (txtcedula == true)
                {
                    TxtCedula.Text = string.Empty;
                }

                if (txtvalidar == true)
                {
                    TxtValidate.Text = string.Empty;
                }
                 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private void Btn_ContinuarTouchDown(object sender, TouchEventArgs e)
        {

            if (Validate())
            {
                Transaction.Document = TxtCedula.Text;

                ValidateUser();

            }
            else
            {
                Utilities.ShowModal("El documento ingresado no coincide, por favor verifique la información", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.Login);
            }
            
        }

        public void ValidateUser()
        {
            
            try
            {

                string data = Utilities.GetConfiguration("Validate");

                AdminPayPlus.SaveLog("LoginUC", "entrando a la ejecucion ValidateUser", "OK", "", null);

                RequestMachine Data = new RequestMachine
                {
                    identificacion = Convert.ToInt64(Utilities.GetConfigData("Terminal")),
                 //   identificacion = Convert.ToInt64(Transaction.Document),
                    Token = Transaction.Token,
                };


                var Respuesta = AdminPayPlus.ApiIntegration.validateUser(Data);

                var ResponseData = JsonConvert.DeserializeObject<ResponseUser>(Respuesta.ResponseData.ToString());

                AdminPayPlus.SaveLog("LoginUC", "Respuesta del servicio ValidateUser", "OK", ResponseData.ToString(), null);


                if (Respuesta != null)
                {
                    if (Respuesta.ResponseCode.ToString() == "OK")
                    {
                        Transaction.nit = ResponseData.model.nit;
                        Transaction.SerieTerminal = ResponseData.model.serieTerminal;
                        Transaction.codigoPuntoVenta = ResponseData.model.codigoPuntoVenta;
                        Transaction.CodigoVendedor = ResponseData.model.codigoVendedor;
                        Utilities.navigator.Navigate(UserControlView.Recharge, Transaction);
                    }
                    else
                    {
                        Utilities.ShowModal("En estos momentos los servicios de Betplay no estan disponibles", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                }
                else
                {
                    Utilities.ShowModal("En estos Momentos los servicios de BetPlay no estan Disponibles", EModalType.Error);
                    Utilities.navigator.Navigate(UserControlView.Menu);
                }


            }
            catch (Exception ex)
            {

                Utilities.ShowModal("No se encontro información para el usuario consultado", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void focusTxtCedula(object sender, RoutedEventArgs e)
        {
            txtcedula = true;
            txtvalidar = false;
        }

        private void focusTxtvalidar(object sender, RoutedEventArgs e)
        {
            txtcedula = false;
            txtvalidar = true;
        }
    }
}
