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
    /// Lógica de interacción para FormUC.xaml
    /// </summary>
    public partial class FormUC : UserControl
    {

       TransactionBetPlay Transaction;
       public string Validar = "Debes Completar los Campos Requeridos: \n";

        public FormUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            this.DataContext = Transaction;
        }

        private void Image_TouchDown(object sender, TouchEventArgs e)
        {

        }

        private void Btn_CheckTouchdown(object sender, TouchEventArgs e)
        {

            int Tag = Convert.ToInt32((sender as Image).Tag);

            switch (Tag)
            {
                case 1:                   
                    BtnAceptar.Visibility = Visibility.Hidden;
                    Yes.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    Yes.Visibility = Visibility.Visible;
                    BtnAceptar.Visibility = Visibility.Visible;
                    Not.Visibility = Visibility.Hidden;
                    break;

            }

     

        }

        private void Btn_CedulaTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, (TextBox)sender, this, 300, 780);
        }

        private void Btn_DocumentoTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, (TextBox)sender, this, 650, 1050);

        }

        private void Btn_CelTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, (TextBox)sender, this, 300, 1150);
            
        }

        private void Btn_EmailTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, (TextBox)sender, this, 300, 1250);

        }

        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }
        private void Btn_ContinuarTouchDown(object sender, TouchEventArgs e)
        {
            ValidateData();
        }

        public void ValidateData()
        {

            if(TxtNombre.Text == "" || ((ComboBoxItem)TypeDocument.SelectedItem).Content.ToString() == null || ((ComboBoxItem)dia.SelectedItem).Content.ToString() == null || ((ComboBoxItem)Mes.SelectedItem).Content.ToString() == null || ((ComboBoxItem)Año.SelectedItem).Content.ToString() == null || TxtCedula.Text == "" || TxtCelular.Text == "" )
            {
                if (TxtNombre.Text == "")
                {
                    Validar += string.Concat("Nombre \n");
                }
                if (((ComboBoxItem)TypeDocument.SelectedItem).Content.ToString() == null)
                {
                    Validar += string.Concat("Tipo de Documento \n");
                };
                if (((ComboBoxItem)dia.SelectedItem).Content.ToString() == null)
                {
                    Validar += string.Concat("Dia de Nacimiento \n");
                };
                if (((ComboBoxItem)Mes.SelectedItem).Content.ToString() == null)
                {
                    Validar += string.Concat("Mes de Nacimiento \n");
                };
                if (((ComboBoxItem)Año.SelectedItem).Content.ToString() == null)
                {
                    Validar += string.Concat("Año de Nacimiento \n");
                };
                if (TxtCedula.Text == "")
                {
                    Validar += string.Concat("Cedula \n");
                }
                if (TxtCelular.Text == "")
                {
                    Validar += string.Concat("Celular \n");
                }

                Utilities.ShowModal(Validar,EModalType.Error);
            }
            else
            {
                Transaction.payer = new DataModel.PAYER();

                Transaction.Name = TxtNombre.Text;
                Transaction.Document = TxtCedula.Text;
                Transaction.payer.EMAIL = TxtCorreo.Text;
                Transaction.payer.PHONE = Convert.ToDecimal(TxtCelular.Text);

                if (((ComboBoxItem)TypeDocument.SelectedItem).Content.ToString() != null)
                {
                    if (((ComboBoxItem)TypeDocument.SelectedItem).Content.ToString() == "Cedula de ciudadania")
                    {
                        Transaction.TypeDocument = "CC";
                    }
                }

                Utilities.navigator.Navigate(UserControlView.Dia, Transaction);
            }
                
        }

        private void Btn_TerminosCondiciones(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Politicas, Transaction);
        }

        private void Btn_ClearName(object sender, TouchEventArgs e)
        {
            TxtNombre.Text = "";
        }

        private void Btn_ClearEmail(object sender, TouchEventArgs e)
        {
            TxtCorreo.Text = "";
        }

        private void Btn_ClearCel(object sender, TouchEventArgs e)
        {
            TxtCelular.Text = "";
        }

        private void Btn_ClearDocument(object sender, TouchEventArgs e)
        {
            TxtCedula.Text = "";
        }
    }
}
