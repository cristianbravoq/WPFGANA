using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Lógica de interacción para ConfirmLotteryUC.xaml
    /// </summary>
    public partial class ConfirmLotteryUC : UserControl
    {

        TransactionBetPlay Transaction;

        public ConfirmLotteryUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            this.DataContext = Transaction;
            LiquidarChance();
            NumerosLoteria();
            CargarLoterias();
        }



        private void Btn_Continuar(object sender, TouchEventArgs e)
        {
            SendData();
        }

        private void Btn_Cancelar(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_Nueva(object sender, TouchEventArgs e)
        {

        }

        private async void SendData()
        {
            try
            {

                AdminPayPlus.SaveLog("RechargeUC", "entrando a la ejecucion SendData", "OK", "", Transaction);

                Task.Run(async () =>
                {
                    Transaction.payer = new DataModel.PAYER
                    {
                        IDENTIFICATION = Transaction.Document,
                        NAME = Transaction.Name,

                        STATE = Transaction.statePaySuccess,

                    };
                    Transaction.State = ETransactionState.Initial;
                    Transaction.Type = ETypeTramites.SuperChance;
                    Transaction.Tipo = ETransactionType.Payment;
                    Transaction.eTypeTramites = ETypeTramites.SuperChance;
                    Transaction.valor = Transaction.Amount;

                    await AdminPayPlus.SaveTransaction(Transaction);

                    AdminPayPlus.SaveLog("RechargeUC", "SendData", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "inicial", "/n", "Monto:", Transaction.Amount.ToString()), Transaction);


                    Utilities.CloseModal();

                    if (this.Transaction.IdTransactionAPi == 0)
                    {
                        Utilities.ShowModal("No se puede guardar la transacción, intentelo más tarde.", EModalType.Error);

                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                    else
                    {
                        Utilities.navigator.Navigate(UserControlView.PagosChance, Transaction);
                    }
                });
                   Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);


            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("RechargeUC", "Error Catch la ejecucion SendData", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        public void LiquidarChance()
        {
            try
            {

                AdminPayPlus.SaveLog("SelecTNumUCS", "entrando a la ejecucion GetNumber", "OK", "", null);

           //     Task.Run(() =>
           //     {

                    RequestLiquidar Data = new RequestLiquidar();

                    Data.idProductoRed = "1";
                    Data.idUsuarioVendedor = "105421";

                    foreach (var item in Transaction.NumeroChance)
                    {
                        NumeroLiquidar Numeros = new NumeroLiquidar
                        {
                          derecho = item.derecho.ToString(),
                          Combinado = item.Combinado.ToString(),
                          cuña = item.cuña.ToString(),
                          cifra = item.cifra.ToString()

                        };

                        Data.numeros.Add(Numeros);
                    }

                    foreach (var item in Transaction.LoteriaLiquidar)
                    {
                        LoteriaLiquidar Loterias = new LoteriaLiquidar
                        {

                            idLoteria = item.idLoteria,
                            sorteo = item.sorteo,

                        };
                        Data.loterias.Add(Loterias);
                    }

                    var Respuesta = AdminPayPlus.ApiIntegration.LiquidarChance(Data);

                    var ResponseData = JsonConvert.DeserializeObject<ResponseLiquidar>(Respuesta.ResponseData.ToString());

                    AdminPayPlus.SaveLog("ComfirmLotteries", "Respuesta del servicio LiquidarChance", "OK",String.Concat(ResponseData), null);


                    if (Respuesta != null)
                    {
                        if (ResponseData.ok == true)
                        {
                            Utilities.CloseModal();

                            Iva.Content = String.Format("{0:C0}", Convert.ToDecimal(ResponseData.model.valorIva));
                            Valor.Content = String.Format("{0:C0}", Convert.ToDecimal(ResponseData.model.valor));
                            ValorT.Content = String.Format("{0:C0}", Convert.ToDecimal(ResponseData.model.valorTotal));
                            Podrias.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar));
                             Transaction.IVA = ResponseData.model.valorIva;
                            Transaction.Valor = ResponseData.model.valor;


                           Transaction.Amount = ResponseData.model.valorTotal.ToString();

                        }
                        else
                        {
                            Utilities.ShowModal("No se pudo liquidar el Chance", EModalType.Error);
                        }
                    }
                    else
                    {
                        Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }

     //           });

    //            Utilities.ShowModal("Estamos Validando la informacion, Un momento Porfavor", EModalType.Preload);

            }
            catch (Exception ex)
            {

                Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        public void NumerosLoteria()
        {

            try
            {

                if (Transaction.NumeroChance.Count == 1)
                {

                    if (Transaction.NumeroChance[0] != null)
                    {
                        NumeroL1.Content = Transaction.NumeroChance[0].numero;
                        Derecho1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho));
                        Combinado1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado));
                        Cuña1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña));
                        Cifra1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra));
                      //  Eliminar1.Visibility = Visibility.Visible;
                    }
                }
                if (Transaction.NumeroChance.Count == 2)
                {
                    if (Transaction.NumeroChance[0] != null)
                    {
                        NumeroL1.Content = Transaction.NumeroChance[0].numero;
                        Derecho1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho));
                        Combinado1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado));
                        Cuña1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña));
                        Cifra1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra));
                        //     Eliminar1.Visibility = Visibility.Visible;
                    }
                    if (Transaction.NumeroChance[1] != null)
                    {
                        NumeroL2.Content = Transaction.NumeroChance[1].numero;
                        Derecho2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho));
                        Combinado2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado));
                        Cuña2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña));
                        Cifra2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra));
                        //         Eliminar2.Visibility = Visibility.Visible;
                    }
                }
                if (Transaction.NumeroChance.Count == 3)
                {
                    if (Transaction.NumeroChance[0] != null)
                    {
                        NumeroL1.Content = Transaction.NumeroChance[0].numero;
                        Derecho1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho));
                        Combinado1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado));
                        Cuña1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña));
                        Cifra1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra));
                        //     Eliminar1.Visibility = Visibility.Visible;
                    }
                    if (Transaction.NumeroChance[1] != null)
                    {
                        NumeroL2.Content = Transaction.NumeroChance[1].numero;
                        Derecho2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho));
                        Combinado2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado));
                        Cuña2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña));
                        Cifra2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra));
                        //         Eliminar2.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[2] != null)
                    {
                        NumeroL3.Content = Transaction.NumeroChance[2].numero;
                        Derecho3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho));
                        Combinado3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado));
                        Cuña3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña));
                        Cifra3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra));
                        //          Eliminar3.Visibility = Visibility.Visible;
                    }
                }

                if (Transaction.NumeroChance.Count == 4)
                {
                    if (Transaction.NumeroChance[0] != null)
                    {
                        NumeroL1.Content = Transaction.NumeroChance[0].numero;
                        Derecho1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho));
                        Combinado1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado));
                        Cuña1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña));
                        Cifra1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra));
                        //     Eliminar1.Visibility = Visibility.Visible;
                    }
                    if (Transaction.NumeroChance[1] != null)
                    {
                        NumeroL2.Content = Transaction.NumeroChance[1].numero;
                        Derecho2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho));
                        Combinado2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado));
                        Cuña2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña));
                        Cifra2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra));
                        //         Eliminar2.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[2] != null)
                    {
                        NumeroL3.Content =Transaction.NumeroChance[2].numero;
                        Derecho3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho));
                        Combinado3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado));
                        Cuña3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña));
                        Cifra3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra));
                        //          Eliminar3.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[3] != null)
                    {
                        NumeroL4.Content = Transaction.NumeroChance[3].numero;
                        Derecho4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].derecho));
                        Combinado4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].Combinado));
                        Cuña4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cuña));
                        Cifra4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cifra));
                        //             Eliminar4.Visibility = Visibility.Visible;
                    }
                }

                if (Transaction.NumeroChance.Count == 5)
                {
                    if (Transaction.NumeroChance[0] != null)
                    {
                        NumeroL1.Content =Transaction.NumeroChance[0].numero;
                        Derecho1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho));
                        Combinado1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado));
                        Cuña1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña));
                        Cifra1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra));
                        //     Eliminar1.Visibility = Visibility.Visible;
                    }
                    if (Transaction.NumeroChance[1] != null)
                    {
                        NumeroL2.Content = Transaction.NumeroChance[1].numero;
                        Derecho2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho));
                        Combinado2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado));
                        Cuña2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña));
                        Cifra2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra));
                        //         Eliminar2.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[2] != null)
                    {
                        NumeroL3.Content =Transaction.NumeroChance[2].numero;
                        Derecho3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho));
                        Combinado3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado));
                        Cuña3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña));
                        Cifra3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra));
                        //          Eliminar3.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[3] != null)
                    {
                        NumeroL4.Content = Transaction.NumeroChance[3].numero;
                        Derecho4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].derecho));
                        Combinado4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].Combinado));
                        Cuña4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cuña));
                        Cifra4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cifra));
                        //             Eliminar4.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[4] != null)
                    {
                        NumeroL5.Content = Transaction.NumeroChance[4].numero;
                        Derecho5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].derecho));
                        Combinado5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].Combinado));
                        Cuña5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cuña));
                        Cifra5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cifra));
                        //             Eliminar4.Visibility = Visibility.Visible;
                    }
                }

                if (Transaction.NumeroChance.Count == 6)
                {
                    if (Transaction.NumeroChance[0] != null)
                    {
                        NumeroL1.Content = Transaction.NumeroChance[0].numero;
                        Derecho1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho));
                        Combinado1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado));
                        Cuña1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña));
                        Cifra1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra));
                        //     Eliminar1.Visibility = Visibility.Visible;
                    }
                    if (Transaction.NumeroChance[1] != null)
                    {
                        NumeroL2.Content = Transaction.NumeroChance[1].numero;
                        Derecho2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho));
                        Combinado2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado));
                        Cuña2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña));
                        Cifra2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra));
                        //         Eliminar2.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[2] != null)
                    {
                        NumeroL3.Content = Transaction.NumeroChance[2].numero;
                        Derecho3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho));
                        Combinado3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado));
                        Cuña3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña));
                        Cifra3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra));
                        //          Eliminar3.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[3] != null)
                    {
                        NumeroL4.Content = Transaction.NumeroChance[3].numero;
                        Derecho4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].derecho));
                        Combinado4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].Combinado));
                        Cuña4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cuña));
                        Cifra4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cifra));
                        //             Eliminar4.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[4] != null)
                    {
                        NumeroL5.Content =(Transaction.NumeroChance[4].numero);
                        Derecho5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].derecho));
                        Combinado5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].Combinado));
                        Cuña5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cuña));
                        Cifra5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cifra));
                        //             Eliminar4.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[5] != null)
                    {
                        NumeroL6.Content = (Transaction.NumeroChance[5].numero);
                        Derecho6.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].derecho));
                        Combinado6.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].Combinado));
                        Cuña6.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].cuña));
                        Cifra6.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].cifra));
                        //             Eliminar6.Visibility = Visibility.Visible;
                    }
                }

                if (Transaction.NumeroChance.Count == 7)
                {
                    if (Transaction.NumeroChance[0] != null)
                    {
                        NumeroL1.Content = Transaction.NumeroChance[0].numero;
                        Derecho1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho));
                        Combinado1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado));
                        Cuña1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña));
                        Cifra1.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra));
                        //     Eliminar1.Visibility = Visibility.Visible;
                    }
                    if (Transaction.NumeroChance[1] != null)
                    {
                        NumeroL2.Content = Transaction.NumeroChance[1].numero;
                        Derecho2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho));
                        Combinado2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado));
                        Cuña2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña));
                        Cifra2.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra));
                        //         Eliminar2.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[2] != null)
                    {
                        NumeroL3.Content = Transaction.NumeroChance[2].numero;
                        Derecho3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho));
                        Combinado3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado));
                        Cuña3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña));
                        Cifra3.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra));
                        //          Eliminar3.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[3] != null)
                    {
                        NumeroL4.Content = Transaction.NumeroChance[3].numero;
                        Derecho4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].derecho));
                        Combinado4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].Combinado));
                        Cuña4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cuña));
                        Cifra4.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cifra));
                        //             Eliminar4.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[4] != null)
                    {
                        NumeroL5.Content = Transaction.NumeroChance[4].numero;
                        Derecho5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].derecho));
                        Combinado5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].Combinado));
                        Cuña5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cuña));
                        Cifra5.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cifra));
                        //             Eliminar4.Visibility = Visibility.Visible;
                    }

                    if (Transaction.NumeroChance[5] != null)
                    {
                        NumeroL6.Content = Transaction.NumeroChance[5].numero;
                        Derecho6.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].derecho));
                        Combinado6.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].Combinado));
                        Cuña6.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].cuña));
                        Cifra6.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].cifra));
                        //             Eliminar6.Visibility = Visibility.Visible;
                    }


                    if (Transaction.NumeroChance[6] != null)
                    {
                        NumeroL7.Content =(Transaction.NumeroChance[6].numero);
                        Derecho7.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[6].derecho));
                        Combinado7.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[6].Combinado));
                        Cuña7.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[6].cuña));
                        Cifra7.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[6].cifra));
                        //             Eliminar7.Visibility = Visibility.Visible;
                    }
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void CargarLoterias()
        {

            try
            {
                var converter = new ImageSourceConverter();
                if (Transaction.LoteriaChance.Count == 1)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                }
                if (Transaction.LoteriaChance.Count == 2)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                }
                if (Transaction.LoteriaChance.Count == 3)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));

                }
                if (Transaction.LoteriaChance.Count == 4)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));
                    lot4.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[3].desLoteria + ".png"));

                }
                if (Transaction.LoteriaChance.Count == 5)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));
                    lot4.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[3].desLoteria + ".png"));
                    lot5.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[4].desLoteria + ".png"));

                }
                if (Transaction.LoteriaChance.Count == 6)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));
                    lot4.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[3].desLoteria + ".png"));
                    lot5.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[4].desLoteria + ".png"));
                    lot6.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[5].desLoteria + ".png"));


                }
                if (Transaction.LoteriaChance.Count == 7)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));
                    lot4.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[3].desLoteria + ".png"));
                    lot5.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[4].desLoteria + ".png"));
                    lot6.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[5].desLoteria + ".png"));
                    lot7.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[6].desLoteria + ".png"));


                }
                if (Transaction.LoteriaChance.Count == 8)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));
                    lot4.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[3].desLoteria + ".png"));
                    lot5.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[4].desLoteria + ".png"));
                    lot6.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[5].desLoteria + ".png"));
                    lot7.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[6].desLoteria + ".png"));
                    lot8.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[7].desLoteria + ".png"));


                }
                if (Transaction.LoteriaChance.Count == 9)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));
                    lot4.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[3].desLoteria + ".png"));
                    lot5.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[4].desLoteria + ".png"));
                    lot6.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[5].desLoteria + ".png"));
                    lot7.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[6].desLoteria + ".png"));
                    lot8.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[7].desLoteria + ".png"));
                    lot9.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[8].desLoteria + ".png"));


                }
                if (Transaction.LoteriaChance.Count == 10)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));
                    lot4.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[3].desLoteria + ".png"));
                    lot5.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[4].desLoteria + ".png"));
                    lot6.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[5].desLoteria + ".png"));
                    lot7.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[6].desLoteria + ".png"));
                    lot8.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[7].desLoteria + ".png"));
                    lot9.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[8].desLoteria + ".png"));
                    lot10.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[9].desLoteria + ".png"));


                }
                if (Transaction.LoteriaChance.Count == 11)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));
                    lot4.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[3].desLoteria + ".png"));
                    lot5.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[4].desLoteria + ".png"));
                    lot6.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[5].desLoteria + ".png"));
                    lot7.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[6].desLoteria + ".png"));
                    lot8.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[7].desLoteria + ".png"));
                    lot9.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[8].desLoteria + ".png"));
                    lot10.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[9].desLoteria + ".png"));
                    lot11.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[10].desLoteria + ".png"));

                }
                if (Transaction.LoteriaChance.Count == 12)
                {
                    lot1.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[0].desLoteria + ".png"));
                    lot2.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[1].desLoteria + ".png"));
                    lot3.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[2].desLoteria + ".png"));
                    lot4.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[3].desLoteria + ".png"));
                    lot5.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[4].desLoteria + ".png"));
                    lot6.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[5].desLoteria + ".png"));
                    lot7.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[6].desLoteria + ".png"));
                    lot8.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[7].desLoteria + ".png"));
                    lot9.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[8].desLoteria + ".png"));
                    lot10.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[9].desLoteria + ".png"));
                    lot11.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[10].desLoteria + ".png"));
                    lot12.Source = (ImageSource)converter.ConvertFromString(string.Concat(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Images\SuperChance\Logos\" + Transaction.LoteriaChance[11].desLoteria + ".png"));

                }
            }
            catch(Exception ex)
            {

            }

          
        }
    }
}
