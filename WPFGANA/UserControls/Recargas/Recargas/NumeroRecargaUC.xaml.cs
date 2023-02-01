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

namespace WPFGANA.UserControls.Recargas.Recargas
{
    /// <summary>
    /// Lógica de interacción para NumeroRecargaUC.xaml
    /// </summary>
    public partial class NumeroRecargaUC : UserControl
    {

        public bool txtcedula = false;
        public bool txtvalidar = false;
        TransactionBetPlay Transaction;

        public NumeroRecargaUC()
        {
            InitializeComponent();
            Transaction = new TransactionBetPlay();
        }

        private void focusTxtvalidar(object sender, RoutedEventArgs e)
        {

            txtcedula = false;
            txtvalidar = true;
        }

        private void focusTxtCedula(object sender, RoutedEventArgs e)
        {

            txtcedula = true;
            txtvalidar = false;
        }

        private void Btn_DeleteAllTouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (txtcedula == true)
                {
                    TxtNumCel.Text = string.Empty;
                }

                if (txtvalidar == true)
                {
                    TxtVal.Text = string.Empty;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Btn_DeleteTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string val = TxtNumCel.Text;
                string val2 = TxtVal.Text;

                if (txtcedula == true)
                {
                    if (val.Length > 0)
                    {
                        TxtNumCel.Text = val.Remove(val.Length - 1);
                        // TxtValidate.Text = val2.Remove(val.Length - 1);
                    }
                }

                if (txtvalidar == true)
                {
                    if (val2.Length > 0)
                    {
                        //    TxtCedula.Text = val.Remove(val.Length - 1);
                        TxtVal.Text = val2.Remove(val2.Length - 1);
                    }

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Keyboard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (txtcedula == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtNumCel.Text += Tag;
                }

                if (txtvalidar == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtVal.Text += Tag;

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BtnContinuar_TouchDown(object sender, TouchEventArgs e)
        {
            if (validateNum())
            {
                Transaction.NumOperator = Convert.ToInt64(TxtNumCel.Text);
                Utilities.navigator.Navigate(UserControlView.RechargeCel, Transaction);
            }
        }


        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        public bool validateNum()
        {

            if (TxtNumCel.Text.Length == 10 && TxtVal.Text.Length == 10)
            {
                if (Convert.ToInt64(TxtNumCel.Text) == Convert.ToInt64(TxtVal.Text))
                {
                    return true;
                }
                else
                {
                    Utilities.ShowModal("Por favor Verifica tu Número ingresado", EModalType.Error);
                    return false;
                }
            }
            else
            {
                Utilities.ShowModal("Por favor ingresa un número de celular valido", EModalType.Error);
                return false;
            }




        }
    }
}
