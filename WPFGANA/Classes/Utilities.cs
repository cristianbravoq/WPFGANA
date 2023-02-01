using Newtonsoft.Json;
using System;
using BarcodeLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Speech.Synthesis;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFGANA.Classes.Printer;
using WPFGANA.Models;
using WPFGANA.Resources;
using WPFGANA.Windows.Alerts;
using BarcodeLib.Barcode;

namespace WPFGANA.Classes
{
    public class Utilities
    {
        #region "Referencias"
        public static Navigation navigator { get; set; }

        private static SpeechSynthesizer speechSynthesizer;

        public static UserControl UCSupport;

        private static ModalW modal { get; set; }

        //   private static ModalW modal { get; set; }
        #endregion
        private static PrintService _printService;

        public static PrintService PrintService
        {
            get { return _printService; }
        }

        public static string GetConfigData(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetConfiguration(string key, bool decodeString = false)
        {
            try
            {
                string value = "";
                AppSettingsReader reader = new AppSettingsReader();
                value = reader.GetValue(key, typeof(String)).ToString();
                if (decodeString)
                {
                    value = Encryptor.Decrypt(value);
                }
                return value;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return string.Empty;
            }
        }

        public static string CodeBar(string Factura)
        {
            byte[] base64SingleBytes;
            string str2;
            try
            {
                string str = "";

                int num3 = 0;
                int num4 = 0x3b9ac9ff;
                for (int i = 0; i < 50; i++)
                {
                    Linear barcode = new Linear();
                    barcode.Type = BarcodeType.CODABAR;
                    barcode.Data = string.Concat(Factura);
                    // barcode.Data = string.Concat("(415)" + Code + "(8020)" + string.Format(Factura).PadLeft(10, '0') + "(3900)" + string.Format("{0:0}", ValorFactura).PadLeft(10, '0') + "(96)" + fecha);
                    barcode.UOM = UnitOfMeasure.PIXEL;
                    barcode.BarWidth = 2;
                    barcode.BarHeight = 30;
                    barcode.TextFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
                    base64SingleBytes = barcode.drawBarcodeAsBytes();
                    int length = base64SingleBytes.GetLength(0);
                    if (length > num3) { num3 = length; }
                    if (length < num4) { num4 = length; }
                    if ((num3 != length) && (num4 < num3))
                    {
                        byte[] inArray = base64SingleBytes;
                        str = string.Format("data:image/png;base64," + Convert.ToBase64String(inArray), new object[0]);

                        File.WriteAllBytes(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Barcode\code.png", inArray);

                        break;
                    }
                }
                str2 = str;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return str2;
        }

        public static bool ShowModal(string message, EModalType type, bool timer = false)
        {
            bool response = false;
            try
            {
                ModalModel model = new ModalModel
                {
                    Tittle = "Estimado Cliente: ",
                    Messaje = message,
                    Timer = timer,
                    TypeModal = type,
                    ImageModal = @"Images/Backgrounds/bg-modal-info.png",
                };

                if (type == EModalType.Error)
                {
                    model.ImageModal = @"Images/Backgrounds/bg-modal-danger.png";
                }
                else if (type == EModalType.Information)
                {
                    model.ImageModal = @"Images/Backgrounds/bg-modal-info.png";
                }
                else if (type == EModalType.NoPaper)
                {
                    model.ImageModal = @"Images/Backgrounds/bg-modal-danger.png";
                }
                else if (type == EModalType.Preload)
                {
                    model.ImageModal = @"Images/Backgrounds/bg-modal-info.png";
                }

                Application.Current.Dispatcher.Invoke(delegate
                {
                    modal = new ModalW(model);
                    modal.ShowDialog();

                    if (modal.DialogResult.HasValue && modal.DialogResult.Value)
                    {
                        response = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            GC.Collect();
            return response;
        }



        public static BitmapImage LoadImageFromFile(Uri path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = path;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.DecodePixelWidth = 900;
            bitmap.EndInit();
            bitmap.Freeze(); //This is the magic line that releases/unlocks the file.
            return bitmap;
        }

        public static void CloseModal() => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                if (modal != null)
                {
                    modal.Close();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
            }
        });

        public static void RestartApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = Path.Combine(Directory.GetCurrentDirectory(), GetConfiguration("NAME_APLICATION"));
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static void UpdateApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = GetConfiguration("APLICATION_UPDATE");
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }


        //public static void PrintVoucherBetplay(TransactionBetPlay ts)
        //{
        //    try
        //    {
        //        CLSPrint objPrint = new CLSPrint();

        //        objPrint.Cedula = ts.Document;
        //        objPrint.Monto = ts.Amount;

        //        objPrint.ImprimirComprobante();
        //    }
        //    catch (Exception ex)
        //    {
        //        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
        //        PrintVoucherBetPlay(ts);
        //    }
        //}

        public static void PrintVoucherBetPlay(TransactionBetPlay Transaction)
        {
            try
            {

                if (_printService == null)
                {
                    _printService = new PrintService();
                }

                if (Transaction != null)
                {


                    String CodigoSitio = Utilities.GetConfigData("CodSitio");
                    String Usuario = Utilities.GetConfigData("UsuarioID");

                    SolidBrush color = new SolidBrush(Color.Black);
                    Font fontKey = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
                    Font fontValue = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
                    int y = 0;
                    int sum = 20;
                    //int sum = 10;
                    int x = 200;
                    //int x = 40;
                    int xKey = 15;
                    int xMax = 100;
                    //int xMax = 50;
                    int ymax = 38;

                    string formaPago = "Efectivo";


                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    // Rectangle rect = new Rectangle(0, y += sum - 15, 280, 20);

                    Rectangle rect = new Rectangle(0, y += sum - 15, 100, 20);

                    //int multiplier = (xMax / 45);


                    rect = new Rectangle(0, y += ymax, 270, 15);
                    //rect = new Rectangle(0, y = y, 270, 15);


                    var data = new List<DataPrinter>()
                    {
                        //new DataPrinter{ image = GetConfiguration("ImageBoucher"), x = 40, y = y += 10, direction = sf  },
                        //ok new DataPrinter { image = GetConfiguration("ImageBoucher"), x = 30, y = 0 , direction = sf},

                        new DataPrinter { brush = color, font = fontKey, value = "BetPlay-Apuestas Deportivas", x = 85, y = y },
                         new DataPrinter { brush = color, font = fontKey, value = "Autoriza COLJUEGOS Contrato C1444", x = 85, y = y+10 },
                         new DataPrinter { brush = color, font = fontKey, value = "", x = 85, y = y+10 },

                        new DataPrinter { brush = color, font = fontValue, value = String.Concat("Fecha:     ",DateTime.Now.Date.ToString("yyyy-MM-dd-HH:mm:ss")) ?? string.Empty, x = 75, y = y+=20},
       //             new DataPrinter { brush = color, font = fontValue, value = "-------------------------------------------------------------------", x = 2, y = y += 20},

                    new DataPrinter { brush = color, font = fontKey, value = "DATOS RECARGA - BETPLAY", x = 85, y = y += 20},       
                    
                    //Numero de Cedula

                        new DataPrinter { brush = color, font = fontKey, value = "Número de Cedula", x = 40, y = y += 24 },
                        new DataPrinter { brush = color, font = fontValue, value = Transaction.Document ?? string.Empty, x = 155, y = y},

                        //new DataPrinter { brush = color, font = fontKey, value = "Número de Cedula", x = xKey, y = y += sum },
                        //new DataPrinter { brush = color, font = fontValue, value = Transaction.Document ?? string.Empty, x = (xMax - Transaction.Document.Length * multiplier), y = y },

                    //Valor A Pagar
                        new DataPrinter { brush = color, font = fontKey, value = "Valor recarga", x = 40, y = y += 24 },
                        new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}",  String.Concat("$" + Transaction.Amount.ToString())), x = 155, y = y },

                       new DataPrinter { brush = color, font = fontKey, value = "REALIZA TU APUESTA EN: BETPLAY.COM.CO", x = 85, y = y+10 },

                      new DataPrinter { brush = color, font = fontValue, value = String.Concat("OF: ",Transaction.create.model.ideOficina.ToString(),"AS: ",Utilities.GetConfiguration("Terminal"),"EQ:  ","000000052441"), x = 20, y = y+20 },

                        };
                    ymax = 0;

                    Utilities.PrintService.Start(data);
                    //AdminPayPlus.PrintService.Start(data);




                }
            }
            catch (Exception ex)
            {
                //   Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }

        }


        public static void PrintVoucherSuperChance(TransactionBetPlay Transaction)
        {
            try
            {


                if (_printService == null)
                {
                    _printService = new PrintService();
                }


                SolidBrush color = new SolidBrush(Color.Black);
                Font fontKey = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
                Font fontValue = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
                Font fonTitle = new Font("Arial", 7, System.Drawing.FontStyle.Bold);
                Font fonTitleV = new Font("Arial", 6, System.Drawing.FontStyle.Regular);
                int y = 0;
                int sum = 20;
                //int sum = 10;
                int x = 200;
                //int x = 40;
                int xKey = 15;
                int xMax = 100;
                //int xMax = 50;
                int ymax = 10;
                string Loterias = "";
                string Numero1 = "";
                string Numero2 = "";
                string Numero3 = "";
                string Numero4 = "";
                string Numero5 = "";
                string Numero6 = "";
                string Numero7 = "";

                string formaPago = "Efectivo";


                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                foreach (var item in Transaction.LoteriaChance)
                {
                    Loterias += string.Concat(item.Abrv + "     ");
                }

                if (Transaction.NumeroChance.Count == 1)
                {
                    Numero1 = string.Concat(Transaction.NumeroChance[0].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra)));
                    Numero2 += String.Concat("XXXX");
                    Numero3 += String.Concat("XXXX");
                    Numero4 += String.Concat("XXXX");
                    Numero5 += String.Concat("XXXX");
                    Numero6 += String.Concat("XXXX");
                    Numero7 += String.Concat("XXXX");
                }

                if (Transaction.NumeroChance.Count == 2)
                {
                    Numero1 = string.Concat(Transaction.NumeroChance[0].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra)));
                    Numero2 = string.Concat(Transaction.NumeroChance[1].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra)));
                    Numero3 += String.Concat("XXXX");
                    Numero4 += String.Concat("XXXX");
                    Numero5 += String.Concat("XXXX");
                    Numero6 += String.Concat("XXXX");
                    Numero7 += String.Concat("XXXX");
                }

                if (Transaction.NumeroChance.Count == 3)
                {
                    Numero1 = string.Concat(Transaction.NumeroChance[0].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra)));
                    Numero2 = string.Concat(Transaction.NumeroChance[1].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra)));
                    Numero3 = string.Concat(Transaction.NumeroChance[2].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra)));
                    Numero4 += String.Concat("XXXX");
                    Numero5 += String.Concat("XXXX");
                    Numero6 += String.Concat("XXXX");
                    Numero7 += String.Concat("XXXX");
                }

                if (Transaction.NumeroChance.Count == 4)
                {
                    Numero1 = string.Concat(Transaction.NumeroChance[0].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra)));
                    Numero2 = string.Concat(Transaction.NumeroChance[1].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra)));
                    Numero3 = string.Concat(Transaction.NumeroChance[2].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra)));
                    Numero4 = string.Concat(Transaction.NumeroChance[3].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cifra)));
                    Numero5 += String.Concat("XXXX");
                    Numero6 += String.Concat("XXXX");
                    Numero7 += String.Concat("XXXX");
                }


                if (Transaction.NumeroChance.Count == 5)
                {
                    Numero1 = string.Concat(Transaction.NumeroChance[0].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra)));
                    Numero2 = string.Concat(Transaction.NumeroChance[1].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra)));
                    Numero3 = string.Concat(Transaction.NumeroChance[2].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra)));
                    Numero4 = string.Concat(Transaction.NumeroChance[3].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cifra)));
                    Numero5 = string.Concat(Transaction.NumeroChance[4].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cifra)));
                    Numero6 += String.Concat("XXXX");
                    Numero7 += String.Concat("XXXX");
                }


                if (Transaction.NumeroChance.Count == 6)
                {
                    Numero1 = string.Concat(Transaction.NumeroChance[0].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra)));
                    Numero2 = string.Concat(Transaction.NumeroChance[1].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra)));
                    Numero3 = string.Concat(Transaction.NumeroChance[2].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra)));
                    Numero4 = string.Concat(Transaction.NumeroChance[3].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cifra)));
                    Numero5 = string.Concat(Transaction.NumeroChance[4].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cifra)));
                    Numero6 = string.Concat(Transaction.NumeroChance[5].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].cifra)));
                    Numero7 += String.Concat("XXXX");
                }

                if (Transaction.NumeroChance.Count == 7)
                {
                    Numero1 = string.Concat(Transaction.NumeroChance[0].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[0].cifra)));
                    Numero2 = string.Concat(Transaction.NumeroChance[1].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[1].cifra)));
                    Numero3 = string.Concat(Transaction.NumeroChance[2].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[2].cifra)));
                    Numero4 = string.Concat(Transaction.NumeroChance[3].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[3].cifra)));
                    Numero5 = string.Concat(Transaction.NumeroChance[4].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[4].cifra)));
                    Numero6 = string.Concat(Transaction.NumeroChance[5].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[5].cifra)));
                    Numero7 = string.Concat(Transaction.NumeroChance[6].numero + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[6].derecho)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[6].cuña)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[6].Combinado)) + "     " + String.Format("{0:C0}", Convert.ToDecimal(Transaction.NumeroChance[6].cifra)));

                }
                // Rectangle rect = new Rectangle(0, y += sum - 15, 280, 20);

                Rectangle rect = new Rectangle(0, y += sum - 15, 100, 20);

                //int multiplier = (xMax / 45);
                String CodigoSitio = Utilities.GetConfigData("CodSitio");
                String Usuario = Utilities.GetConfigData("UsuarioID");


                rect = new Rectangle(0, y += ymax, 270, 15);
                //rect = new Rectangle(0, y = y, 270, 15);


                var data = new List<DataPrinter>()
                {
                        new DataPrinter { brush = color, font = fontKey, value ="SUPERCHANCE",x = 90, y = y},

                        new DataPrinter { brush = color, font = fonTitleV, value ="EL CHANCE MANUAL ES ILEGAL",x = 60, y = y += 20},

                        new DataPrinter { brush = color, font = fontKey, value = Loterias,x = 50, y = y += 30},


                        new DataPrinter { brush = color, font = fonTitle, value = "NÚMERO.  DERE.  CUNA.  COMB.  CIF.",x = 50, y = y += 30},

                         new DataPrinter { brush = color, font = fonTitleV, value = Numero1,x = 50, y = y += 20},

                         new DataPrinter { brush = color, font = fonTitleV, value = Numero2,x = 50, y = y += 10},

                         new DataPrinter { brush = color, font = fonTitleV, value = Numero3,x = 50, y = y += 10},

                         new DataPrinter { brush = color, font = fonTitleV, value = Numero4,x = 50, y = y += 10},

                         new DataPrinter { brush = color, font = fonTitleV, value = Numero5,x = 50, y = y += 10},

                         new DataPrinter { brush = color, font = fonTitleV, value = Numero6,x = 50, y = y += 10},

                         new DataPrinter { brush = color, font = fonTitleV, value = Numero7,x = 50, y = y += 10},



                        new DataPrinter { brush = color, font = fonTitleV, value = String.Concat(Utilities.GetConfiguration("") + "COL - "+ Utilities.GetConfigData("Terminal") + "     CLI - "+Transaction.Document) , x = 50, y = y+=20},

                        new DataPrinter { brush = color, font = fonTitleV, value = String.Concat("FV: "+ DateTime.Now.ToString("dd/MM/yyyy") +" "+ DateTime.Now.ToString("HH:mm:ss")+" "+"FS:" + Convert.ToDateTime(Transaction.Fecha).ToString("dd/MM/yyyy")) , x = 50, y = y+=20},

                        new DataPrinter { brush = color, font = fonTitleV, value = String.Concat("Vlr: "+ string.Format("{0:C2}", String.Concat("$" + Transaction.valor)) + "   "+"IVA:"+ string.Format("{0:C2}", String.Concat("$" + Transaction.IVA)) + "    " + "TOTAL:"+ string.Format("{0:C2}", String.Concat("$" + Transaction.Amount))) , x = 50, y = y+=20},

                        new DataPrinter { brush = color, font = fonTitleV, value = "OF:", x = 50, y = y += 20},
                        new DataPrinter { brush = color, font = fonTitleV, value =  Transaction.create.model.ideOficina.ToString(), x = 70, y = y },

                        new DataPrinter { brush = color, font = fonTitleV, value = String.Concat(Transaction.create.model.serie,"-",Transaction.create.model.colilla.ToString()), x = 50, y = y+=25 },

                   new DataPrinter { brush = color, font = fonTitleV, value = "SV:", x = 90, y = y },
                        new DataPrinter { brush = color, font = fonTitleV, value = Utilities.GetConfigData("Sitio"), x = 100, y = y },

                           new DataPrinter { brush = color, font = fonTitleV, value = "E:", x = 110, y = y },
                        new DataPrinter { brush = color, font = fonTitleV, value = Transaction.create.model.consecutivoBeneficencia.ToString(), x = 130, y = y },

                    new DataPrinter { brush = color, font = fontKey, value ="MEDELLIN",x = 100, y = y += 25},

                    new DataPrinter { brush = color, font = fonTitleV, value = Transaction.create.model.claVenta.ToString(), x = 105, y = y+= 25},

                    new DataPrinter{ image = $@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Barcode\code.png" , x = 80, y = y += 25 , direction = sf  },

                   new DataPrinter { brush = color, font = fonTitleV, value ="Contrato Vigente No. 32 de 2021",x = 100, y = y += 35},
                };

                ymax = 0;

                Utilities.PrintService.Start(data);
                //AdminPayPlus.PrintService.Start(data);




            }
            catch (Exception ex)
            {
                //   Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }

        }

        public static void PrintVoucherRecargas(TransactionBetPlay Transaction)
        {
            try
            {

                if (_printService == null)
                {
                    _printService = new PrintService();
                }

                if (Transaction != null)
                {


                    String CodigoSitio = Utilities.GetConfigData("CodSitio");
                    String Usuario = Utilities.GetConfigData("UsuarioID");

                    SolidBrush color = new SolidBrush(Color.Black);
                    Font fontKey = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
                    Font fontValue = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
                    int y = 0;
                    int sum = 20;
                    //int sum = 10;
                    int x = 200;
                    //int x = 40;
                    int xKey = 15;
                    int xMax = 100;
                    //int xMax = 50;
                    int ymax = 38;

                    string formaPago = "Efectivo";


                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    // Rectangle rect = new Rectangle(0, y += sum - 15, 280, 20);

                    Rectangle rect = new Rectangle(0, y += sum - 15, 100, 20);

                    //int multiplier = (xMax / 45);


                    rect = new Rectangle(0, y += ymax, 270, 15);
                    //rect = new Rectangle(0, y = y, 270, 15);

                    string info = "";

                    if(Transaction.eTypeTramites == ETypeTramites.PaquetesCel)
                    {
                         info = "Paquetes Celular";
                    }
                    else
                    {
                         info = "Recargas Celular";
                    }

                    string TypePack = "";

                    if (Transaction.eTypeTramites == ETypeTramites.PaquetesCel)
                    {
                        TypePack = Transaction.SelectOperator.desPaquete;
                    }
                    else
                    {
                        TypePack = "Recarga Celular";
                    }

                    var data = new List<DataPrinter>()
                    {
                        //new DataPrinter{ image = GetConfiguration("ImageBoucher"), x = 40, y = y += 10, direction = sf  },
                        //ok new DataPrinter { image = GetConfiguration("ImageBoucher"), x = 30, y = 0 , direction = sf},

                        new DataPrinter { brush = color, font = fontKey, value = "Fecha", x = 85, y = y },
                        new DataPrinter { brush = color, font = fontKey, value = "Hora", x = 145, y = y },

                          //  data.Add(new DataPrinter { brush = color, font = fontValue, value = AdminPayPlus.DataConfiguration.ID_PAYPAD.ToString() ?? string.Empty, x = 23, y = y += 15 });
                       //     data.Add(new DataPrinter { brush = color, font = fontValue, value = Transaction.IdTransactionAPi.ToString() ?? string.Empty, x = 83, y = y });
                        new DataPrinter { brush = color, font = fontValue, value = DateTime.Now.Date.ToString("yyyy-MM-dd") ?? string.Empty, x = 75, y = y+=20},
                        new DataPrinter { brush = color, font = fontValue, value = DateTime.Now.ToString("HH:mm:ss") ?? string.Empty, x = 145, y = y},

                        new DataPrinter { brush = color, font = fontKey, value = "Cód.Sitio", x = 55, y = y += 20},
                        new DataPrinter { brush = color, font = fontValue, value = CodigoSitio, x = 155, y = y },

                        new DataPrinter { brush = color, font = fontKey, value = info , x = 55, y = y += 20},
               //         new DataPrinter { brush = color, font = fontValue, value = Usuario, x = 155, y = y },

                    new DataPrinter { brush = color, font = fontValue, value = "-------------------------------------------------------------------", x = 2, y = y += 20},
                    
                    //Numero de Cedula

                        new DataPrinter { brush = color, font = fontKey, value = "Número de Celular", x = 40, y = y += 24 },
                        new DataPrinter { brush = color, font = fontValue, value = Transaction.NumOperator.ToString() ?? string.Empty, x = 155, y = y},

                        new DataPrinter { brush = color, font = fontKey, value = "Descripcion", x = 40, y = y += 24 },
                        new DataPrinter { brush = color, font = fontValue, value = TypePack ?? string.Empty, x = 155, y = y},

                        //new DataPrinter { brush = color, font = fontKey, value = "Número de Cedula", x = xKey, y = y += sum },
                        //new DataPrinter { brush = color, font = fontValue, value = Transaction.Document ?? string.Empty, x = (xMax - Transaction.Document.Length * multiplier), y = y },

                    //Valor A Pagar
                        new DataPrinter { brush = color, font = fontKey, value = "Valor recarga", x = 40, y = y += 24 },
                        new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}",  String.Concat("$" + Transaction.Amount.ToString())), x = 155, y = y },

                      ////Estado de la trasaccion
                        new DataPrinter { brush = color, font = fontKey, value = "Estado de trasacción", x = 40, y = y += 24},

                        new DataPrinter { brush = color, font = fontValue, value = Transaction.StatePay.ToString() ?? string.Empty,  x = 167, y = y  },

                   
                    //Total Imgresado
                        new DataPrinter { brush = color, font = fontKey, value = "Valor ingresado", x = 40, y = y += 24},

                        new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}", String.Concat("$" + Transaction.Payment.ValorIngresado.ToString())),  x = 155, y = y  },

                    new DataPrinter { brush = color, font = fontKey, value = "Total Devuelto:", x = 40, y = y += 24},
                    new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}", String.Concat("$" + Transaction.Payment._valorDispensado.ToString())), x = 155, y = y },

                    new DataPrinter { brush = color, font = fontValue, value = "-------------------------------------------------------------------", x = 2, y = y+=20 },

                        new DataPrinter { brush = color, font = fontValue, value = "Toda Transaccion esta sujeta", x = 55, y = y += 15},
                        new DataPrinter { brush = color, font = fontValue, value = "a verificacion y aprobacion", x = 55, y = y +=15 },




                        };
                    ymax = 0;

                    Utilities.PrintService.Start(data);
                    //AdminPayPlus.PrintService.Start(data);

                }
            }
            catch (Exception ex)
            {
                //   Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }

        }


        public static decimal RoundValue(decimal Total)
        {
            try
            {

                decimal roundTotal = 0;
                roundTotal = Math.Ceiling(Total / 100) * 100;
                return roundTotal;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return Total;
            }
        }

        public static bool ValidateModule(decimal module, decimal amount)
        {
            try
            {
                var result = (amount % module);
                return result == 0 ? true : false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return false;
            }
        }

        public static T ConverJson<T>(string path)
        {
            T response = default(T);
            try
            {
                using (StreamReader file = new StreamReader(path, Encoding.UTF8))
                {
                    try
                    {
                        var json = file.ReadToEnd().ToString();
                        if (!string.IsNullOrEmpty(json))
                        {
                            response = JsonConvert.DeserializeObject<T>(json);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return response;
        }

        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,8}$");
                return regex.IsMatch(email);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return false;
            }
        }

        public static void Speak(string text)
        {
            try
            {
                if (GetConfiguration("ActivateSpeak").ToUpper() == "SI")
                {
                    if (speechSynthesizer == null)
                    {
                        speechSynthesizer = new SpeechSynthesizer();
                    }

                    speechSynthesizer.SpeakAsyncCancelAll();
                    speechSynthesizer.SelectVoice("Microsoft Sabina Desktop");
                    speechSynthesizer.SpeakAsync(text);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static string[] ReadFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllLines(path);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return null;
        }

        public static string GetIpPublish()
        {
            try
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(GetConfiguration("UrlGetIp"));
                }
            }
            catch (Exception ex)
            {
                // Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return GetConfiguration("IpDefoult");
        }


        public static void OpenKeyboard(bool keyBoard_Numeric, object textBox, object thisView, int x = 0, int y = 0)
        {
            try
            {



                WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
                {
                    control = textBox,
                    userControl = thisView is UserControl ? thisView as UserControl : null,
                    eType = (keyBoard_Numeric == true) ? WPKeyboard.Keyboard.EType.Numeric : WPKeyboard.Keyboard.EType.Standar,
                    window = thisView is Window ? thisView as Window : null,
                    X = x,
                    Y = y
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static string[] ErrorDevice()
        {
            try
            {
                string[] keys = Utilities.ReadFile(@"" + ConstantsResource.PathDevice);

                return keys;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return null;
            }
        }

        public static bool IsMultiple(decimal value)
        {
            try
            {
                if (value % 100 != 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return true;
        }

        public static string GetDescriptionEnum(Type enume, string name)
        {
            try
            {
                MemberInfo info = enume.GetMember(name).First();
                return info.GetCustomAttribute<DescriptionAttribute>().Description.ToString();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return string.Empty;
            }
        }
    }
}
