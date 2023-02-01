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
    /// Lógica de interacción para RecargasUC.xaml
    /// </summary>
    public partial class RecargasUC : UserControl
    {

        TransactionBetPlay Transaction;
        public ValueModel value;

        public RecargasUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = transaction;

            value = new ValueModel
            {
                Val = 0
            };

            this.DataContext = value;
        }

        private void BtnContinuar_TouchDown(object sender, TouchEventArgs e)
        {
            if (Validate())
            {
           //     Transaction.Amount = TxtValor.Text;
                Transaction.eTypeTramites = ETypeTramites.RecargasCel;
                Utilities.navigator.Navigate(UserControlView.ValidateRecharge,Transaction);

            }
        }

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_DeleteAllTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                TxtValor.Text = string.Empty;
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
                string val = TxtValor.Text;

                if (val.Length > 0)
                {
                    TxtValor.Text = val.Remove(val.Length - 1);

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
                Image image = (Image)sender;
                string Tag = image.Tag.ToString();
                TxtValor.Text += Tag;

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

                string value = TxtValor.Text.Replace("$", "");
                value = value.Replace(",", "");

                if (value == "")
                {
                    Utilities.ShowModal("Por favor,Digite un valor valido", EModalType.Error);
                    return false;
                }
                if(Convert.ToInt32(value) % 100 != 0)
                {
                    Utilities.ShowModal("Por favor,Digite un valor valido", EModalType.Error);
                    return false;
                }
                if (Convert.ToInt32(value) < 1000)
                {
                    Utilities.ShowModal("Por favor,Digite un valor valido", EModalType.Error);
                    return false;
                }

                Transaction.Amount = value;

                return true;

            }
            catch(Exception ex)
            {
                return false;
            }
  
        }

        private void txtVal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtValor.Text.Length > 15)
            {
                TxtValor.Text = TxtValor.Text.Remove(15, 1);
                return;
            }
        }
    }
}
