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

namespace WPFGANA.UserControls.Recargas.Paquetes
{
    /// <summary>
    /// Lógica de interacción para PaquetesUC.xaml
    /// </summary>
    public partial class PaquetesUC : UserControl
    {

        private ObservableCollection<OperatorsViewModel> LstPackets;

        TransactionBetPlay Transaction;

        CollectionViewSource view = new CollectionViewSource();

        OperatorsViewModel SelectedOperator = new OperatorsViewModel();

      //  ResponseGetPackets Operators = new ResponseGetPackets();

        public PaquetesUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = new TransactionBetPlay();
            this.DataContext = Transaction;
            Transaction = transaction;
            LstPackets = new ObservableCollection<OperatorsViewModel>();
            LoadPacketsAsync();
        }

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_SelectPacket(object sender, TouchEventArgs e)
        {
            try
            {
                //Probar

                var data = sender as ListViewItem;
                SelectedOperator = (OperatorsViewModel)data.Content;

                Transaction.SelectOperator.consumeFacturacion = SelectedOperator.ConsumeFacturacion;
                Transaction.SelectOperator.cantidad = SelectedOperator.cantidad;
                Transaction.SelectOperator.desOperador = SelectedOperator.DesOperator;
                Transaction.SelectOperator.desPaquete = SelectedOperator.desPaquete;
                Transaction.SelectOperator.idOperador = SelectedOperator.IdOperator;
                Transaction.SelectOperator.idPaqueteRecarga = SelectedOperator.idPaqueteRecarga;
                Transaction.SelectOperator.idPaqueteOperador = SelectedOperator.idPaqueteOperador;
                Transaction.SelectOperator.nomPaquete = SelectedOperator.nomPaquete;
                Transaction.SelectOperator.porcentajeComision = SelectedOperator.PorcentajeComision;
                Transaction.SelectOperator.tipoRecarga = SelectedOperator.tipoRecarga;
                Transaction.SelectOperator.valorComercial = Convert.ToDouble(SelectedOperator.valorComercial.Replace("$",""));
                Transaction.SelectOperator.vigencia = SelectedOperator.Vigencia;

                Utilities.navigator.Navigate(UserControlView.PackNum, Transaction);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task LoadPacketsAsync()
        {
            try
            {


                foreach (var Operator in Transaction.Operators.model.list)
                {

                    if (Transaction.Operador == Operator.desOperador)
                    {
                        LstPackets.Add(new OperatorsViewModel
                        {
                            ImageData =
                         Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                        Assembly.GetEntryAssembly().Location),
                        "Operadores", Operator.desOperador.ToString() + ".png"))),
                            Tag = Operator.idOperador.ToString(),
                            abreviatura = Operator.desOperador.ToString(),
                            IsSelect = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                        Assembly.GetEntryAssembly().Location),
                        "Operadores", Operator.desOperador.ToString() + ".png"))),
                            tipoRecarga = Operator.tipoRecarga,
                            cantidad = Operator.cantidad,
                            ConsumeFacturacion = Operator.consumeFacturacion,
                            DesOperator = Operator.desOperador,
                            IdOperator = Operator.idOperador,
                            idPaqueteOperador = Operator.idPaqueteOperador,
                            idPaqueteRecarga = Operator.idPaqueteRecarga,
                            nomPaquete = Operator.nomPaquete,
                            PorcentajeComision = Operator.porcentajeComision,
                            valorComercial = String.Format("{0:C0}", Operator.valorComercial),
                            Vigencia = Operator.vigencia,
                            desPaquete = Operator.desPaquete

                        });
                    }
                 
                };

                //LstSelectOperators.Add(new OperatorsViewModel
                //{
                //    ImageData =
                //             Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //            Assembly.GetEntryAssembly().Location),
                //            "Loterias", Transaction.LotteryList.model.list[0].desLoteria.ToString() + ".png"))),
                //    Tag = Transaction.LotteryList.model.list[0].sorteo.ToString(),
                //    IdLoteria = Transaction.LotteryList.model.list[0].idLoteria.ToString(),
                //    DesLoteria = Transaction.LotteryList.model.list[0].desLoteria.ToString(),
                //    abreviatura = Transaction.LotteryList.model.list[0].abreviatura.ToString(),
                //    ImageDataS = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //            Assembly.GetEntryAssembly().Location),
                //            "LoteriasS", Transaction.LotteryList.model.list[0].desLoteria.ToString() + ".png"))),
                //    IsSelect = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //            Assembly.GetEntryAssembly().Location),
                //            "Loterias", Transaction.LotteryList.model.list[0].desLoteria.ToString() + ".png"))),
                //});

                view.Source = LstPackets;
                this.DataContext = view;
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveErrorControl(JsonConvert.SerializeObject(ex), "Load lotteries", EError.Aplication, ELevelError.Medium);
            }
        }
    }
}
