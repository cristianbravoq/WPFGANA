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
    /// Lógica de interacción para SelectNumUC.xaml
    /// </summary>
    public partial class SelectNumUC : UserControl
    {

        TransactionBetPlay Transaction;
        public string Pos1 = "";
        public string Pos2 = "";
        public string Pos3 = "";
        public string Pos4 = "";
        public int ValorGanarNumeroDerecho = 0;
        public int ValorGanarNumerocuña = 0;
        public int ValorGanarNumeroCombinado = 0;
        public int ValorGanarNumeroCifra = 0;
        public string Numero = "";
        public int cifra = 4;
        public bool txtNum1 = false;
        public bool txtNum2 = false;
        public bool txtNum3 = false;
        public bool txtNum4 = false;
        public bool txtDerecho = false;
        public bool txtCombinado = false;
        public bool txtCifra = false;
        public bool txtCuna = false;

        public SelectNumUC(TransactionBetPlay transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            this.DataContext = Transaction;
            validarPick3();
      
        }

        private void Btn_CelTouchDown(object sender, TouchEventArgs e)
        {
       //     Utilities.OpenKeyboard(true, (TextBox)sender, this, 250, 900);
        }


        private void Btn_Continuar(object sender, TouchEventArgs e)
        {      
                validarNumero();
                                     
        }

        public void validarPick3()
        {
            foreach (var item in Transaction.LoteriaChance)
            {

                if (item.desLoteria == "PICK3")
                {
                    Num4.Visibility = Visibility.Hidden;
                    cifra = 3;
                    Utilities.ShowModal("Recuerda que seleccionaste PICK3 y solo te permitira jugar con 3 cifras",EModalType.Error);
                }

            }

          
        }

        public void LiquidarChance()
        {
            try
            {

                //   AdminPayPlus.SaveLog("SelecTNumUCS", "entrando a la ejecucion GetNumber", "OK", "", null);

                //                     Task.Run(() =>
                //                   {

                RequestLiquidar Data = new RequestLiquidar();

                NumeroLiquidar Numeros = new NumeroLiquidar();

                foreach(var item in Transaction.NumeroChance)
                {
                    Numeros.cifra = item.cifra.ToString();
                    Numeros.Combinado = item.Combinado.ToString();
                    Numeros.cuña = item.cuña.ToString();
                    Numeros.derecho = item.derecho.ToString();

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

//                AdminPayPlus.SaveLog("DateTxUC", "Respuesta del servicio LiquidarChance", "OK", String.Concat(Data), null);

                var Respuesta = AdminPayPlus.ApiIntegration.LiquidarChance(Data);

                var ResponseData = JsonConvert.DeserializeObject<ResponseLiquidar>(Respuesta.ResponseData.ToString());

               AdminPayPlus.SaveLog("SelectNum", "Respuesta del servicio LiquidarChance", "OK", String.Concat(ResponseData), null);


                if (Respuesta != null)
                {
                    if (ResponseData.ok == true)
                    {
                        Utilities.CloseModal();
                        Utilities.ShowModal(string.Concat("Asi va tu apuesta:" + "\n" + "valor apostado: " + String.Format("{0:C0}", Convert.ToDecimal(ResponseData.model.valor)) + "\n" + "IVA: " + String.Format("{0:C0}", Convert.ToDecimal(ResponseData.model.valorIva)) + "\n" + "valor total a pagar: " + String.Format("{0:C0}", Convert.ToDecimal(ResponseData.model.valorTotal)) + "\n" + "Podrias ganar hasta: " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar))), EModalType.Error);

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

                //                     });

                //            Utilities.ShowModal("Estamos Validando la informacion, Un momento Porfavor", EModalType.Preload);

            }
            catch (Exception ex)
            {
                Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                //       AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        public void validarNumero()
        {
            try
            {

           //     if (Convert.ToInt32(TxtNumCombinado.Text) > 840 || Convert.ToInt32(TxtNumCifra.Text) > 840 || Convert.ToInt32(TxtNumDerecho.Text) > 840 || Convert.ToInt32(TxtNumCuña.Text) > 840 || Convert.ToInt32(TxtNumCombinado.Text) < 840000 || Convert.ToInt32(TxtNumCifra.Text) < 840000 || Convert.ToInt32(TxtNumDerecho.Text) < 840000 || Convert.ToInt32(TxtNumCuña.Text) < 840000)
        //        {
                    if (Num1.Text == "" && Num2.Text == "")
                    {
                        Utilities.ShowModal("Debes ingresar un numero valido", EModalType.Error);
                    }
                    else
                    {

                        //       Task.Run(() =>
                        //        {


                        AdminPayPlus.SaveLog("SelecTNumUCS", "entrando a la ejecucion GetNumber", "OK", "", null);

                        Numero = String.Concat(Num1.Text + Num2.Text + Num3.Text + Num4.Text);

                        Transaction.NumeroLoteria = Numero;

                        RequestValidarNumero Data = new RequestValidarNumero();

                        Data.numeros = new List<NumeroValidar>();

                        Data.loterias = new List<LoteriaValidar>();

                        Data.fecSorteo = Transaction.Fecha;

                        NumeroLiquidar NumerosD = new NumeroLiquidar
                        {

                            derecho = Transaction.NumeroLoteria,


                        };


                        NumeroChance NumerosC = new NumeroChance();

                        NumerosC.numero = Transaction.NumeroLoteria;

                        if (TxtNumDerecho.Text != null && TxtNumDerecho.Text != "")
                        {
                            NumerosC.derecho = Convert.ToInt32(TxtNumDerecho.Text);
                        }

                        if (TxtNumCombinado.Text != null && TxtNumCombinado.Text != "")
                        {
                            NumerosC.Combinado = Convert.ToInt32(TxtNumCombinado.Text);
                        }

                        if (TxtNumCuña.Text != null && TxtNumCuña.Text != "")
                        {
                            NumerosC.cuña = Convert.ToInt32(TxtNumCuña.Text);
                        }

                        if (TxtNumCifra.Text != null && TxtNumCifra.Text != "")
                        {
                            NumerosC.cifra = Convert.ToInt32(TxtNumCifra.Text);
                        }

                        Transaction.NumeroLiquidar.Add(NumerosD);

                        Transaction.NumeroChance.Add(NumerosC);

                        foreach (var item in Transaction.NumeroLiquidar)
                        {
                            NumeroValidar Numeros = new NumeroValidar
                            {
                                numero = item.derecho,

                            };

                            Data.numeros.Add(Numeros);
                        }

                        foreach (var item in Transaction.LoteriaLiquidar)
                        {
                            LoteriaValidar Loterias = new LoteriaValidar
                            {

                                idLoteria = item.idLoteria,

                            };
                            Data.loterias.Add(Loterias);
                        }

                        var Respuesta = AdminPayPlus.ApiIntegration.ValidateNumber(Data);

                        var ResponseData = JsonConvert.DeserializeObject<ResponseValidarNumero>(Respuesta.ResponseData.ToString());

                  AdminPayPlus.SaveLog("DateTxUC", "Respuesta del servicio Validar Numero", "OK",String.Concat(ResponseData), null);

                        if (Respuesta != null)
                        {
                            if (ResponseData.ok == true)
                            {
                                Transaction.NumberValidate = ResponseData;
                                //     Transaction.NumeroCombinado = TxtNumCombinado.Text;
                                //    Transaction.NumeroDerecho = TxtNumDerecho.Text;

                                Utilities.CloseModal();
                                LiquidarChance();
                                Utilities.navigator.Navigate(UserControlView.Verificar, Transaction);

                            }
                            else
                            {
                                Utilities.ShowModal("el numero seleccionado ha llegado a su tope", EModalType.Error);
                            }
                        }
                        else
                        {
                            Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.Menu);
                        }

                        //          });

                        //        Utilities.ShowModal("Estamos Validando la informacion, Un momento Porfavor", EModalType.Preload);
        //            }

                }
                //else
                //{
                //    Utilities.ShowModal("el monto minimo es de $840 pesos y el maximo son $840.000", EModalType.Error);
                //}
            }
            catch (Exception ex)
            {

                Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);

            }

        
                
    }

        private void Btn_RandomNumberTouchdown(object sender, TouchEventArgs e)
        {
            GetNumber();

        }

        public void GetNumber()
        {
            try
            {

                AdminPayPlus.SaveLog("SelecTNumUCS", "entrando a la ejecucion GetNumber", "OK", "", null);



                Num1.Text = "0";
                Num2.Text = "0";
                Num3.Text = "0";
                Num4.Text = "0";

                //   Task.Run(() =>
                //    {



                    RequestNumeroRandom Data = new RequestNumeroRandom
                    {

                        cifras = cifra

                    };


                    var Respuesta = AdminPayPlus.ApiIntegration.GetNumberRandom(Data);

                    var ResponseData = JsonConvert.DeserializeObject<ResponseRandom>(Respuesta.ResponseData.ToString());

                    AdminPayPlus.SaveLog("DateTxUC", "Respuesta del servicio GetLotteries", "OK", ResponseData.ToString(), null);


                    if (Respuesta != null)
                    {
                        if (ResponseData.ok == true)
                        {
                            Numero = ResponseData.model.numero;

                            Utilities.CloseModal();                          

                            char[] characters = Numero.ToCharArray();

                           if(cifra == 3)
                           {
                            Pos1 = Numero.ElementAt(0).ToString();
                            Pos2 = Numero.ElementAt(1).ToString();
                            Pos3 = Numero.ElementAt(2).ToString();

                            Num1.Text = Pos1;
                            Num2.Text = Pos2;
                            Num3.Text = Pos3;
                           }
                           else
                           {
                            Pos1 = Numero.ElementAt(0).ToString();
                            Pos2 = Numero.ElementAt(1).ToString();
                            Pos3 = Numero.ElementAt(2).ToString();
                            Pos4 = Numero.ElementAt(3).ToString();

                            Num1.Text = Pos1;
                            Num2.Text = Pos2;
                            Num3.Text = Pos3;
                            Num4.Text = Pos4;
                           }
                        
                        }
                        else
                        {
                            Utilities.ShowModal("No se pudo Obtener el numero Aleatorio", EModalType.Error);
                        }
                    }
                    else
                    {
                        Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }

          //      });

               

           //     Utilities.ShowModal("Estamos Consultando la informacion, Un momento Porfavor", EModalType.Preload);

            }
            catch (Exception ex)
            {

                Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        private void Btn_Num1(object sender, TouchEventArgs e)
        {
      //      Utilities.OpenKeyboard(true, (TextBox)sender, this, 540, 750);
        }

        private void Btn_Num2(object sender, TouchEventArgs e)
        {
    //        Utilities.OpenKeyboard(true, (TextBox)sender, this, 540, 750);
        }

        private void Btn_Num3(object sender, TouchEventArgs e)
        {
    //        Utilities.OpenKeyboard(true, (TextBox)sender, this, 540, 750);
        }

        private void Btn_Num4(object sender, TouchEventArgs e)
        {
 //           Utilities.OpenKeyboard(true, (TextBox)sender, this, 540, 750);
        }

        private void Btn_CombinadoTouchDown(object sender, TouchEventArgs e)
        {
 //           Utilities.OpenKeyboard(true, (TextBox)sender, this, 600, 900);
        }

        private void Btn_Cuña(object sender, TouchEventArgs e)
        {
   //         Utilities.OpenKeyboard(true, (TextBox)sender, this, 250, 1050);
        
        }

        private void Btn_NumCifra(object sender, TouchEventArgs e)
        {
  //          Utilities.OpenKeyboard(true, (TextBox)sender, this, 600, 1050);
        }

        private void ValorGanar(object sender, TextChangedEventArgs e)
        {


            try
            {
                Numero = String.Concat(Num1.Text + Num2.Text + Num3.Text + Num4.Text);

                if (TxtNumDerecho.Text.Length == 0)
                    {
                        ValorGanarNumeroDerecho = 0;
                        Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                        TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                }
                    else
                    {
                        //      TxtNumDerecho.Text = "";
                        int PagoDirecto = Transaction.Parametros.model.planesPremios[1].pagaDirecto;
                        int PagoDirecto3c = Transaction.Parametros.model.planesPremios[0].pagaDirecto;
                        int ValorApuesta = Convert.ToInt32(TxtNumDerecho.Text);
                        int ValorGanar = PagoDirecto * ValorApuesta;
                        int ValorGanar3c = PagoDirecto3c * ValorApuesta;




                        if (Numero.Length == 4)
                        {

                            ValorGanarNumeroDerecho = ValorGanar;
                            Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                            TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                    }
                        if (Numero.Length == 3)
                        {

                            ValorGanarNumeroDerecho = ValorGanar3c;
                            Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                            TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                    }
                    }
               
             

          

            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        private void ValorGanarCombinado(object sender, TextChangedEventArgs e)
        {

           
            try
            {
                Numero = String.Concat(Num1.Text + Num2.Text + Num3.Text + Num4.Text);

                if (TxtNumCombinado.Text.Length == 0)
                {
                    ValorGanarNumeroCombinado = 0;              
                    Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                    TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                }
                else
                {
                    int PagoDirecto = Transaction.Parametros.model.planesPremios[1].pagaCombinado;
                    int PagoDirecto3c = Transaction.Parametros.model.planesPremios[0].pagaCombinado;
                    int ValorApuesta = Convert.ToInt32(TxtNumCombinado.Text);
                    int ValorGanar = PagoDirecto * ValorApuesta;
                    int ValorGanar3c = PagoDirecto3c * ValorApuesta;

                    if (Numero.Length == 4)
                    {
                        ValorGanarNumeroCombinado = ValorGanar;
                        Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;             
                        TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                    }
                    if (Numero.Length == 3)
                    {
                        ValorGanarNumeroCombinado = ValorGanar3c;
                        Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                        TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                    }
                }
         

            }
            catch (Exception ex)
            {

            }
        }

        private void ValorGanarCuna(object sender, TextChangedEventArgs e)
        {
            try
            {

                Numero = String.Concat(Num1.Text + Num2.Text + Num3.Text + Num4.Text);

                if (TxtNumCuña.Text.Length == 0)
                {
                    ValorGanarNumerocuña = 0;
                    Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                    TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                }
                else
                {

                    int PagoDirecto = Transaction.Parametros.model.planesPremios[0].pagaCuna;
                    int ValorApuesta = Convert.ToInt32(TxtNumCuña.Text);
                    int ValorGanar = PagoDirecto * ValorApuesta;

                    if (Numero.Length == 3)
                    {
                        ValorGanarNumerocuña = ValorGanar;
                        Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                        TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                    }

                }

            }
            catch (Exception ex)
            {

            }
        }

        private void ValorGanarCifra(object sender, TextChangedEventArgs e)
        {

            try
            {

                Numero = String.Concat(Num1.Text + Num2.Text + Num3.Text + Num4.Text);

                if (TxtNumCifra.Text.Length == 0)
                {
                    ValorGanarNumeroCifra = 0;
                    Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                    TotalGanar.Content = Transaction.ValorGanar.ToString();
                }
                else
                {
                    int PagoDirecto = Transaction.Parametros.model.planesPremios[1].pagaCifra;
                    int PagoDirecto3c = Transaction.Parametros.model.planesPremios[0].pagaCifra;
                    int ValorApuesta = Convert.ToInt32(TxtNumCifra.Text);
                    int ValorGanar = PagoDirecto * ValorApuesta;
                    int ValorGanar3c = PagoDirecto3c * ValorApuesta;

                    if (Numero.Length == 4)
                    {
                        ValorGanarNumeroCifra = ValorGanar;
                        Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                      
                        TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                    }
                    if (Numero.Length == 3)
                    {
                        ValorGanarNumeroCifra = ValorGanar3c;
                        Transaction.ValorGanar = ValorGanarNumerocuña + ValorGanarNumeroCifra + ValorGanarNumeroCombinado + ValorGanarNumeroDerecho;
                        TotalGanar.Content = String.Format("{0:C0}", Convert.ToDecimal(Transaction.ValorGanar.ToString()));
                    }

                }

            }
            catch (Exception ex)
            {

            }
        }



        public void GuardarNumero()
        {
            try
            {

                AdminPayPlus.SaveLog("SelecTNumUCS", "entrando a la ejecucion GetNumber", "OK", "", null);

                //   Task.Run(() =>
                //   {
            //    if (Convert.ToInt32(TxtNumCombinado.Text) > 840 || Convert.ToInt32(TxtNumCifra.Text) > 840 || Convert.ToInt32(TxtNumDerecho.Text) > 840 || Convert.ToInt32(TxtNumCuña.Text) > 840 || Convert.ToInt32(TxtNumCombinado.Text) < 840000 || Convert.ToInt32(TxtNumCifra.Text) < 840000 || Convert.ToInt32(TxtNumDerecho.Text) < 840000 || Convert.ToInt32(TxtNumCuña.Text) < 840000)
        //        {
                    try
                    {

                        if (Num1.Text == "" && Num2.Text == "" && Num3.Text == "" && Num4.Text == "")
                        {
                            Utilities.ShowModal("Debes ingresar un numero valido", EModalType.Error);
                        }
                        else
                        {

                            Numero = String.Concat(Num1.Text + Num2.Text + Num3.Text + Num4.Text);

                            Transaction.NumeroLoteria = Numero;

                            if (Transaction.ExtraMum < 7)
                            {
                                RequestValidarNumero Data = new RequestValidarNumero();

                                Data.numeros = new List<NumeroValidar>();

                                Data.loterias = new List<LoteriaValidar>();

                                Data.fecSorteo = Transaction.Fecha;

                                NumeroLiquidar NumerosD = new NumeroLiquidar
                                {

                                    derecho = Transaction.NumeroLoteria,

                                };

                                NumeroChance NumerosC = new NumeroChance();

                                NumerosC.numero = Transaction.NumeroLoteria;

                                if (TxtNumDerecho.Text != null && TxtNumDerecho.Text != "")
                                {
                                    NumerosC.derecho = Convert.ToInt32(TxtNumDerecho.Text);
                                }

                                if (TxtNumCombinado.Text != null && TxtNumCombinado.Text != "")
                                {
                                    NumerosC.Combinado = Convert.ToInt32(TxtNumCombinado.Text);
                                }

                                if (TxtNumCuña.Text != null && TxtNumCuña.Text != "")
                                {
                                    NumerosC.cuña = Convert.ToInt32(TxtNumCuña.Text);
                                }

                                if (TxtNumCifra.Text != null && TxtNumCifra.Text != "")
                                {
                                    NumerosC.cifra = Convert.ToInt32(TxtNumCifra.Text);
                                }


                                Transaction.NumeroLiquidar.Add(NumerosD);
                                Transaction.NumeroChance.Add(NumerosC);


                                foreach (var item in Transaction.NumeroLiquidar)
                                {
                                    NumeroValidar Numeros = new NumeroValidar
                                    {
                                        numero = item.derecho,

                                    };

                                    Data.numeros.Add(Numeros);
                                }

                                foreach (var item in Transaction.LoteriaLiquidar)
                                {
                                    LoteriaValidar Loterias = new LoteriaValidar
                                    {

                                        idLoteria = item.idLoteria,

                                    };
                                    Data.loterias.Add(Loterias);
                                }

                                var Respuesta = AdminPayPlus.ApiIntegration.ValidateNumber(Data);

                                var ResponseData = JsonConvert.DeserializeObject<ResponseValidarNumero>(Respuesta.ResponseData.ToString());

                                AdminPayPlus.SaveLog("DateTxUC", "Respuesta del servicio GetLotteries", "OK", ResponseData.ToString(), null);

                                if (Respuesta != null)
                                {
                                    if (ResponseData.ok == true)
                                    {
                                        Transaction.NumberValidate = ResponseData;
                                        Transaction.ExtraMum += 1;
                                        if (Transaction.ExtraMum <= 7)
                                        {

                                            Utilities.CloseModal();
                                            LiquidarChance();
                                            Utilities.navigator.Navigate(UserControlView.Adicional, Transaction);
                                        }
                                        else
                                        {
                                            Utilities.CloseModal();
                                            Utilities.ShowModal("Ya superaste la cantidad maxima de numeros a escoger", EModalType.Error);

                                        }

                                    }
                                    else
                                    {
                                        Utilities.ShowModal("No se pudo Validar el Numero Aleatorio", EModalType.Error);
                                    }
                                }
                                else
                                {
                                    Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                                    Utilities.navigator.Navigate(UserControlView.Menu);
                                }
                            }
                            else
                            {
                                Utilities.ShowModal("Ya superaste la cantidad maxima de numeros a escoger", EModalType.Error);
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
               // }
                //else
                //{
                //    Utilities.ShowModal("el monto minimo es de $840 pesos y el maximo son $840.000", EModalType.Error);
                //}

                //     });

           //     Utilities.ShowModal("Estamos Validando la informacion, Un momento Porfavor", EModalType.Preload);

            }
            catch (Exception ex)
            {

                Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }


        private void Btn_NuevaApuesta(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_NumAdicional(object sender, TouchEventArgs e)
        {
            GuardarNumero();
        }

        private void Changed_num4(object sender, TextChangedEventArgs e)
        {
            //if(Num4.Text != "" && Num4.Text.Length == 1)
            //{
            //    Num1.Focus();
            //}
        }
     
        private void Focus_Num4(object sender, RoutedEventArgs e)
        {
           txtNum1 = false;
           txtNum2 = false;
           txtNum3 = false;
            txtNum4 = true;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Focus_Num3(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;        
        txtNum2 = false;
            txtNum3 = true;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Changed_num3(object sender, TextChangedEventArgs e)
        {
            //if (Num3.Text != "" && Num3.Text.Length == 1)
            //{
            //    Num4.Focus();
            //}
        }

        private void Changed_num2(object sender, TextChangedEventArgs e)
        {
            //if (Num2.Text != "" && Num2.Text.Length == 1)
            //{
            //    Num3.Focus();
            //}
        }

        private void Focus_Num2(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = true;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Focus_Num1(object sender, RoutedEventArgs e)
        {
            txtNum1 = true;
            txtNum2 = false;
           txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Changed_num1(object sender, TextChangedEventArgs e)
        {
            //if (Num1.Text != "" && Num1.Text.Length == 1)
            //{
            //    Num2.Focus();
            //}
        }

        private void Btn_DeleteTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string val = Num1.Text;
                string val2 = Num2.Text;
                string val3 = Num3.Text;
                string val4 = Num4.Text;
                string val5 = TxtNumCombinado.Text;
                string val6 = TxtNumDerecho.Text;
                string val7 = TxtNumCuña.Text;
                string val8 = TxtNumCifra.Text;
                

                if (txtNum1 == true)
                {
                    if (val.Length > 0)
                    {
                        Num1.Text = val.Remove(val.Length - 1);
                    }
                 //   txtNum1 = false;
                }

                if (txtNum2 == true)
                {
                    if (val.Length > 0)
                    {
                        Num2.Text = val2.Remove(val2.Length - 1);
                    }
                //    txtNum2 = false;
                }

                if (txtNum3 == true)
                {
                    if (val.Length > 0)
                    {
                        Num3.Text = val3.Remove(val3.Length - 1);
                   }
               //     txtNum3 = false;
                }

                if (txtNum4 == true)
                {
                    if (val.Length > 0)
                    {
                        Num4.Text = val4.Remove(val4.Length - 1);
                    }
            //        txtNum4 = false;
                }

                if (txtDerecho == true)
                {
                    if (val.Length > 0)
                    {
                        TxtNumDerecho.Text = val5.Remove(val5.Length - 1);
                    }
                }

                if (txtCombinado == true)
                {
                    if (val.Length > 0)
                    {
                        TxtNumCombinado.Text = val6.Remove(val6.Length - 1);                   
                    }
                }

                if (txtCuna == true)
                {
                    if (val.Length > 0)
                    {
                        TxtNumCuña.Text = val7.Remove(val7.Length - 1);
                    }
                }

                if (txtCifra == true)
                {
                    if (val.Length > 0)
                    {
                        TxtNumCifra.Text = val8.Remove(val8.Length - 1);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Btn_DeleteAllTouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (txtNum1 == true)
                {
                    Num1.Text = string.Empty;
                    txtNum1 = false;
                }

                if (txtNum2 == true)
                {
                    Num2.Text = string.Empty;
                    txtNum2 = false;
                }

                if (txtNum3 == true)
                {
                    Num3.Text = string.Empty;
                    txtNum3 = false;
                }

                if (txtNum4 == true)
                {
                    Num4.Text = string.Empty;
                    txtNum4 = false;
                }

                if (txtDerecho == true)
                {
                    TxtNumDerecho.Text = string.Empty;
                }

                if (txtCombinado == true)
                {
                    TxtNumCombinado.Text = string.Empty;
                }

                if (txtCuna == true)
                {
                    TxtNumCuña.Text = string.Empty;

                }

                if (txtCifra == true)
                {
                    TxtNumCifra.Text = string.Empty;
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
                if (txtNum1 == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    Num1.Text += Tag;
                  //  txtNum1 = false;
                }

                

                if (txtNum2 == true)
                {
                    
                    
                        Image image = (Image)sender;
                        string Tag = image.Tag.ToString();
                        Num2.Text += Tag;
                                      
                }

             //   txtNum2 = false;

                if (txtNum3 == true )
                {
                    
                        Image image = (Image)sender;
                        string Tag = image.Tag.ToString();
                        Num3.Text += Tag;
                    
                }

            //    txtNum3 = false;

                if (txtNum4 == true)
                {
                    
                        Image image = (Image)sender;
                        string Tag = image.Tag.ToString();
                        Num4.Text += Tag;
                    
                }

                if (txtDerecho == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtNumDerecho.Text += Tag;
                }

                if (txtCombinado == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtNumCombinado.Text += Tag;
                }

                if (txtCuna == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtNumCuña.Text += Tag;

                }

                if (txtCifra == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtNumCifra.Text += Tag;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Focus_Derecho(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = true;
        }

        private void Focus_Combinado(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = true;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Focus_cuna(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = true;
            txtDerecho = false;
        }

        private void Focus_Cifra(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = true;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }
    }
}
