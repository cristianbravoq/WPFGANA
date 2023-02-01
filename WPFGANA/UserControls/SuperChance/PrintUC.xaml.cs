using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Lógica de interacción para PrintUC.xaml
    /// </summary>
    public partial class PrintUC : UserControl
    {

        TransactionBetPlay Transaction;

        public PrintUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            this.DataContext = Transaction;
            Imprimir();
        }

        private void Imprimir()
        {
            //  Thread.Sleep(5000); 
            Utilities.navigator.Navigate(UserControlView.Finish, Transaction);
        }
    }
}
