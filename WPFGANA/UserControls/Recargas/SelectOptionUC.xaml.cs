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

namespace WPFGANA.UserControls.Recargas
{
    /// <summary>
    /// Lógica de interacción para SelectOptionUC.xaml
    /// </summary>
    public partial class SelectOptionUC : UserControl
    {

        TransactionBetPlay Transaction;

        public SelectOptionUC()
        {
            InitializeComponent();
            Transaction = new TransactionBetPlay();

        }

        private void BtnPaquetes_touchDown(object sender, TouchEventArgs e)
        {
            Transaction.eTypeTramites = ETypeTramites.PaquetesCel;
            GetOperators();
           
        }

        private void BtnRecargas_touchDown(object sender, TouchEventArgs e)
        {
            Transaction.eTypeTramites = ETypeTramites.RecargasCel;
            GetOperators();
         //   Utilities.navigator.Navigate(UserControlView.RechargeNum);
        }

        private void BtnCancelar_touchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        public void GetOperators()
        {
            try
            {

                Task.Run(() =>
                {

                    var Respuesta = AdminPayPlus.ApiIntegration.GetPackets();

                    if (Respuesta.ResponseCode.ToString() == "OK")
                    {

                        Utilities.CloseModal();
                        AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion if true GetOperators", "OK", "", null);
                        var ResponseData = JsonConvert.DeserializeObject<ResponseGetPackets>(Respuesta.ResponseData.ToString());
                        Utilities.navigator.Navigate(UserControlView.SelectOperator, Transaction,ResponseData);

                    }
                    else
                    {
                        Utilities.ShowModal("No se pudo obtener los operadores por favor intenta nuevamente", EModalType.Error);
                        AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion GetOperators if not true", "OK", Respuesta.ResponseMessage, null);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }

                  
                });

                Utilities.ShowModal("Estamos Consultado la informacion un momento porfavor",EModalType.Preload);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
