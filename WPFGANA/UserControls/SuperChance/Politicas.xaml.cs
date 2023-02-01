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
    /// Interaction logic for Politicas.xaml
    /// </summary>
    public partial class Politicas : UserControl
    {

        TransactionBetPlay Transaction;
        public Politicas(TransactionBetPlay transaction)
        {

            try
            {
                InitializeComponent();
                Transaction = transaction;
                this.DataContext = Transaction;
                //GridBrowser.Navigate(new Uri("file:///C:/CertificadosElectronicos/1.pdf"));
            }
            catch (Exception ex)
            {

            }

        }

        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Form, Transaction);
        }
    }
}
