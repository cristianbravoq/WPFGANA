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
    /// Lógica de interacción para SelectOperador.xaml
    /// </summary>
    public partial class SelectOperador : UserControl
    {

        private ObservableCollection<OperatorsViewModel> LstSelectOperators;

        TransactionBetPlay Transaction;

        CollectionViewSource view = new CollectionViewSource();

        OperatorsViewModel SelectedOperator = new OperatorsViewModel();

        ResponseGetPackets Operators = new ResponseGetPackets();

        public SelectOperador(TransactionBetPlay transaction,ResponseGetPackets Data)
        {
            InitializeComponent();
            Transaction = transaction;
            Transaction.Operators = Data;
            LstSelectOperators = new ObservableCollection<OperatorsViewModel>();
            LoadOperatorsAsync();
        }

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_SelectOperator(object sender, TouchEventArgs e)
        {
            try
            {
                //Probar
                var data = sender as ListViewItem; 
                SelectedOperator = (OperatorsViewModel)data.Content;

                Transaction.SelectOperator = new OperatorSelected();

                Transaction.Operador = SelectedOperator.DesOperator;

                if(Transaction.eTypeTramites == ETypeTramites.PaquetesCel)
                {
                    Utilities.navigator.Navigate(UserControlView.TypePack, Transaction);
                }
                else
                {
                    Utilities.navigator.Navigate(UserControlView.RechargeNum, Transaction);
                }

            
                

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task LoadOperatorsAsync()
        {
            try
            {



                //foreach (var Operator in Transaction.Operators.model.list)
                //{

                    

                //    LstSelectOperators.Add(new OperatorsViewModel
                //    {
                //        ImageData =
                //         Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //        Assembly.GetEntryAssembly().Location),
                //        "Operadores", Operator.desOperador.ToString() + ".png"))),
                //        Tag = Operator.idOperador.ToString(),
                //        abreviatura = Operator.desOperador.ToString(),                       
                //        IsSelect = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //        Assembly.GetEntryAssembly().Location),
                //        "Operadores", Operator.desOperador.ToString() + ".png"))),
                //        tipoRecarga = Operator.tipoRecarga,
                //        cantidad = Operator.cantidad,
                //        ConsumeFacturacion = Operator.consumeFacturacion,
                //        DesOperator = Operator.desOperador,
                //        IdOperator = Operator.idOperador,
                //        idPaqueteOperador = Operator.idPaqueteOperador,
                //        idPaqueteRecarga = Operator.idPaqueteRecarga,
                //        nomPaquete = Operator.nomPaquete,
                //        PorcentajeComision = Operator.porcentajeComision,
                //      //  valorComercial = Operator.valorComercial,
                //        Vigencia = Operator.vigencia,
                //        desPaquete = Operator.desPaquete                       

                //    });

                //};

                LstSelectOperators.Add(new OperatorsViewModel
                {
                    ImageData =
                    Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                   Assembly.GetEntryAssembly().Location),
                   "Operadores", Transaction.Operators.model.list[0].desOperador.ToString() + ".png"))),
                    Tag = Transaction.Operators.model.list[0].idOperador.ToString(),
                    abreviatura = Transaction.Operators.model.list[0].desOperador.ToString(),
                    IsSelect = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                   Assembly.GetEntryAssembly().Location),
                   "Operadores", Transaction.Operators.model.list[0].desOperador.ToString() + ".png"))),
                    tipoRecarga = Transaction.Operators.model.list[0].tipoRecarga,
                    cantidad = Transaction.Operators.model.list[0].cantidad,
                    ConsumeFacturacion = Transaction.Operators.model.list[0].consumeFacturacion,
                    DesOperator = Transaction.Operators.model.list[0].desOperador,
                    IdOperator = Transaction.Operators.model.list[0].idOperador,
                    idPaqueteOperador = Transaction.Operators.model.list[0].idPaqueteOperador,
                    idPaqueteRecarga = Transaction.Operators.model.list[0].idPaqueteRecarga,
                    nomPaquete = Transaction.Operators.model.list[0].nomPaquete,
                    PorcentajeComision = Transaction.Operators.model.list[0].porcentajeComision,
                    //  valorComercial = Operator.valorComercial,
                    Vigencia = Transaction.Operators.model.list[0].vigencia,
                    desPaquete = Transaction.Operators.model.list[0].desPaquete

                });

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

                view.Source = LstSelectOperators;
                this.DataContext = view;
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveErrorControl(JsonConvert.SerializeObject(ex), "Load lotteries", EError.Aplication, ELevelError.Medium);
            }
        }
    }
}
