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

namespace WPFGANA.UserControls.SuperChance
{
    /// <summary>
    /// Lógica de interacción para LotteryUC.xaml
    /// </summary>


    public partial class LotteryUC : UserControl
    {

        private ObservableCollection<LotteriesViewModel> LstLotteriesModel;

        TransactionBetPlay Transaction;

        LoteriaLiquidar loterias = new LoteriaLiquidar();

        LoteriaChance loteria = new LoteriaChance();

        CollectionViewSource view = new CollectionViewSource();

        LotteriesViewModel Selectedlotteries = new LotteriesViewModel();



        public LotteryUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            this.DataContext = Transaction;
            LstLotteriesModel = new ObservableCollection<LotteriesViewModel>();
            LoadLotteriesAsync();
        }


        private void Btn_ContinuarTouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (Transaction.LoteriaLiquidar.Count > 0 && Transaction.LoteriaChance.Count > 0)
                {
                    Utilities.navigator.Navigate(UserControlView.Apuesta, Transaction);
                }
                else
                {
                    Utilities.ShowModal("Debes seleccionar minimo una loteria", EModalType.Error);
                }

                //foreach (var item in lvLotteries.SelectedItems)
                //{
                //    Selectedlotteries = (LotteriesViewModel)item;


                //    LoteriaLiquidar loterias = new LoteriaLiquidar
                //    {
                //        idLoteria = Selectedlotteries.IdLoteria,

                //        sorteo = Selectedlotteries.Tag

                //    };

                //    LoteriaChance loteria = new LoteriaChance
                //    {
                //        idLoteria = Selectedlotteries.IdLoteria,
                //        sorteo = Selectedlotteries.Tag,
                //        desLoteria = Selectedlotteries.DesLoteria,
                //        Abrv = Selectedlotteries.abreviatura,

                //    };

                //    Transaction.LoteriaLiquidar.Add(loterias);
                //    Transaction.LoteriaChance.Add(loteria);



                //}



            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_TxtBuscarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, (TextBox)sender, this, 540, 750);
        }

        public async Task LoadLotteriesAsync()
        {
            try
            {



                //foreach (var Loteria in Transaction.LotteryList.model.list)
                //{

                //    LstLotteriesModel.Add(new LotteriesViewModel
                //    {
                //        ImageData =
                //         Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //        Assembly.GetEntryAssembly().Location),
                //        "Loterias", Loteria.desLoteria.ToString() + ".png"))),
                //        Tag = Loteria.sorteo.ToString(),
                //        IdLoteria = Loteria.idLoteria.ToString(),
                //        DesLoteria = Loteria.desLoteria.ToString(),
                //        abreviatura = Loteria.abreviatura.ToString(),
                //        ImageDataS = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //        Assembly.GetEntryAssembly().Location),
                //        "LoteriasS", Loteria.desLoteria.ToString() + ".png"))),
                //        IsSelect = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //        Assembly.GetEntryAssembly().Location),
                //        "Loterias", Loteria.desLoteria.ToString() + ".png"))),


                //    });

                //};

                LstLotteriesModel.Add(new LotteriesViewModel
                {
                    ImageData =
                             Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                            Assembly.GetEntryAssembly().Location),
                            "Loterias", Transaction.LotteryList.model.list[0].desLoteria.ToString() + ".png"))),
                    Tag = Transaction.LotteryList.model.list[0].sorteo.ToString(),
                    IdLoteria = Transaction.LotteryList.model.list[0].idLoteria.ToString(),
                    DesLoteria = Transaction.LotteryList.model.list[0].desLoteria.ToString(),
                    abreviatura = Transaction.LotteryList.model.list[0].abreviatura.ToString(),
                    ImageDataS = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                            Assembly.GetEntryAssembly().Location),
                            "LoteriasS", Transaction.LotteryList.model.list[0].desLoteria.ToString() + ".png"))),
                    IsSelect = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                            Assembly.GetEntryAssembly().Location),
                            "Loterias", Transaction.LotteryList.model.list[0].desLoteria.ToString() + ".png"))),
                });

                view.Source = LstLotteriesModel;
                this.DataContext = view;
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveErrorControl(JsonConvert.SerializeObject(ex), "Load lotteries", EError.Aplication, ELevelError.Medium);
            }
        }

        private void Btn_SelectLotterie(object sender, TouchEventArgs e)
        {

            try
            {

                var data = sender as ListViewItem;

                //  data.se
                //               loterias = new LoteriaLiquidar();
                //             loteria = new LoteriaChance();
                LotteriesViewModel Selectedlotteries = new LotteriesViewModel();

                Selectedlotteries = (LotteriesViewModel)data.Content;

                loterias.idLoteria = Selectedlotteries.IdLoteria;

                loterias.sorteo = Selectedlotteries.Tag;

                loteria.idLoteria = Selectedlotteries.IdLoteria;
                loteria.sorteo = Selectedlotteries.Tag;
                loteria.desLoteria = Selectedlotteries.DesLoteria;
                loteria.Abrv = Selectedlotteries.abreviatura;

                //    LoteriaLiquidar loteriasSelect = new LoteriaLiquidar
                //    {
                //        idLoteria = Selectedlotteries.IdLoteria,

                //        sorteo = Selectedlotteries.Tag

                //    };

                //    LoteriaChance loteriaSelect = new LoteriaChance
                //    {
                //        idLoteria = Selectedlotteries.IdLoteria,
                //        sorteo = Selectedlotteries.Tag,
                //        desLoteria = Selectedlotteries.DesLoteria,
                //        Abrv = Selectedlotteries.abreviatura
                //};



                if (Selectedlotteries.ImageData != Selectedlotteries.ImageDataS)
                {
                    Selectedlotteries.ImageData = Selectedlotteries.ImageDataS;

                    Transaction.LoteriaLiquidar.Add(loterias);
                    Transaction.LoteriaChance.Add(loteria);
                }
                else
                {
                    Selectedlotteries.ImageData = Selectedlotteries.IsSelect;

                    Transaction.LoteriaLiquidar.Remove(loterias);
                    Transaction.LoteriaChance.Remove(loteria);

                }

                lvLotteries.Items.Refresh();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
