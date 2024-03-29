﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WPFGANA.Classes.DB;
using WPFGANA.Classes.Printer;
using WPFGANA.Classes.UseFull;
using WPFGANA.DataModel;
using WPFGANA.Models;
using WPFGANA.Resources;
using WPFGANA.Services;
using WPFGANA.Services.Object;

namespace WPFGANA.Classes
{
    public class AdminPayPlus : INotifyPropertyChanged
    {
        #region "Referencias"

        private static Api api;

        private static ApiIntegration _apiIntegration;

        public static ApiIntegration ApiIntegration
        {
            get { return _apiIntegration; }
        }

        public Action<bool> callbackResult;//Calback de mensaje

        private static CONFIGURATION_PAYDAD _dataConfiguration;

        public static CONFIGURATION_PAYDAD DataConfiguration
        {
            get { return _dataConfiguration; }
        }

        private static DataPayPlus _dataPayPlus;

        public static DataPayPlus DataPayPlus
        {
            get { return _dataPayPlus; }
        }

        private static PrintService _printService;

        public static PrintService PrintService
        {
            get { return _printService; }
        }

        private static ReaderBarCode _readerBarCode;

        public static ReaderBarCode ReaderBarCode
        {
            get { return _readerBarCode; }
        }

        private static ControlPeripherals _controlPeripherals;

        public static ControlPeripherals ControlPeripherals
        {
            get { return _controlPeripherals; }
        }

        private string _descriptionStatusPayPlus;

        public string DescriptionStatusPayPlus
        {
            get { return _descriptionStatusPayPlus; }
            set
            {
                _descriptionStatusPayPlus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DescriptionStatusPayPlus)));
            }
        }

        public class ParametersLog
        {
            public string Id { get; set; }

            public IdEvents Transaccion { get; set; }

        }

        public class IdEvents
        {
            public string IdTransaccion { get; set; }

            public string Fecha { get; set; }

            public string Clase { get; set; }

            public string Metodo { get; set; }

            public string Estado { get; set; }

            public string Cliente { get; set; }

            public string Description { get; set; }

        }


        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #endregion

        #region "Constructor"
        public AdminPayPlus()
        {
            if (api == null)
            {
                api = new Api();
            }

            if (_printService == null)
            {
                _printService = new PrintService();
            }

            if (_dataPayPlus == null)
            {
                _dataPayPlus = new DataPayPlus();
            }

            if (_readerBarCode == null)
            {
                _readerBarCode = new ReaderBarCode();
            }
        }
        #endregion


        public async void Start()
        {
            DescriptionStatusPayPlus = MessageResource.ComunicationServer;

            if (await LoginPaypad())
            {

                if (_apiIntegration == null)
                {
                    _apiIntegration = new ApiIntegration(_dataConfiguration.ID_PAYPAD.ToString());
                    //var Cod = await AdminPayPlus.ApiIntegration.GetToken();
                }

                DescriptionStatusPayPlus = MessageResource.StatePayPlus;

                if (await ValidatePaypad())
                {


                    DescriptionStatusPayPlus = MessageResource.ValidatePeripherals;

                    if (_dataPayPlus.PayPadConfiguration.enablE_VALIDATE_PERIPHERALS)
                    {
                        ValidatePeripherals();
                    }
                    else
                    {
                        callbackResult?.Invoke(true);
                    }

                }
                else
                {
                    DescriptionStatusPayPlus = MessageResource.StatePayPlusFail;
                    callbackResult?.Invoke(false);
                }
            }
            else
            {
                DescriptionStatusPayPlus = MessageResource.ComunicationServerFail;
                callbackResult?.Invoke(false);
            }
        }

        private async Task<bool> LoginPaypad()

        {
            try
            {
                var config = LoadInformation();

                if (config != null)
                {
                    var result = await api.GetSecurityToken(config);

                    if (result != null)
                    {
                        config.ID_PAYPAD = Convert.ToInt32(result.User);
                        config.ID_SESSION = Convert.ToInt32(result.Session);
                        config.TOKEN_API = result.Token;

                        if (SqliteDataAccess.UpdateConfiguration(config))
                        {
                            _dataConfiguration = config;
                            return true;
                        }
                    }
                    else
                    {
                        SaveErrorControl(MessageResource.ErrorServiceLogin, MessageResource.NoGoInitial, EError.Api, ELevelError.Strong);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
            return false;
        }

        public static async Task<bool> ValidatePaypad()
        {
            try
            {
                var response = await api.CallApi("InitPaypad");
                if (response != null)
                {
                    _dataPayPlus = JsonConvert.DeserializeObject<DataPayPlus>(response.ToString());

                    //Utilities.ImagesSlider = JsonConvert.DeserializeObject<List<string>>(data.ListImages.ToString());
                    if (_dataPayPlus.StateBalanece || _dataPayPlus.StateUpload)
                    {
                        SaveLog(new RequestLog
                        {
                            Reference = response.ToString(),
                            Description = MessageResource.PaypadGoAdmin,
                            State = 4,
                            Date = DateTime.Now
                        }, ELogType.General);
                        return true;
                    }
                    if (_dataPayPlus.State && _dataPayPlus.StateAceptance && _dataPayPlus.StateDispenser)
                    {
                        return true;
                    }
                    else
                    {
                        SaveLog(new RequestLog
                        {
                            Reference = response.ToString(),
                            Description = MessageResource.NoGoInitial + _dataPayPlus.Message,
                            State = 6,
                            Date = DateTime.Now
                        }, ELogType.General);

                        SaveErrorControl(MessageResource.NoGoInitial, _dataPayPlus.Message, EError.Aplication, ELevelError.Strong);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
            return false;
        }

        private void ValidatePeripherals()
        {
            try
            {
                if (_controlPeripherals == null)
                {
                    _controlPeripherals = new ControlPeripherals(Utilities.GetConfiguration("Port"),
                        Utilities.GetConfiguration("ValuesDispenser"));
                }

                _controlPeripherals.callbackError = error =>
                {
                    SaveLog(new RequestLogDevice
                    {
                        Code = "",
                        Date = DateTime.Now,
                        Description = error.Item2,
                        Level = ELevelError.Strong
                    }, ELogType.Device);

                    DescriptionStatusPayPlus = MessageResource.ValidatePeripheralsFail;

                    Finish(false);
                };

                _controlPeripherals.callbackToken = isSucces =>
                {
                    _controlPeripherals.callbackError = null;

                    Finish(isSucces);
                };
                _controlPeripherals.Start();

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
                callbackResult?.Invoke(false);
            }
        }

        private void Finish(bool isSucces)
        {
            _controlPeripherals.callbackToken = null;
            _controlPeripherals.callbackError = null;

            if (isSucces)
            {
                SaveLog(new RequestLog
                {
                    Reference = "",
                    Description = MessageResource.PaypadStarSusses,
                    State = 1,
                    Date = DateTime.Now
                }, ELogType.General);
            }

            callbackResult?.Invoke(isSucces);
        }

        private CONFIGURATION_PAYDAD LoadInformation()
        {
            try
            {
                string[] keys = Utilities.ReadFile(@"" + ConstantsResource.PathKeys);

                if (keys.Length > 0)
                {
                    string[] server = keys[0].Split(';');
                    string[] payplus = keys[1].Split(';');

                    return new CONFIGURATION_PAYDAD
                    {
                        USER_API = server[0].Split(':')[1],
                        PASSWORD_API = server[1].Split(':')[1],
                        USER = payplus[0].Split(':')[1],
                        PASSWORD = payplus[1].Split(':')[1],
                        TYPE = Convert.ToInt32(payplus[2].Split(':')[1])
                    };
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
            return null;
        }

        public async static void SaveLog(object log, ELogType type)
        {
            try
            {
                Task.Run(async () =>
                {
                    var saveResult = SqliteDataAccess.SaveLog(log, type);
                    object result = "false";

                    if (log != null && saveResult != null)
                    {
                        if (type == ELogType.General)
                        {
                            result = await api.CallApi("SaveLog", (RequestLog)log);
                        }
                        else if (type == ELogType.Error)
                        {
                            var error = (ERROR_LOG)log;
                            result = await api.CallApi("SaveLogError", error);
                            SaveErrorControl(error.DESCRIPTION, $"Clase: {error.NAME_CLASS} Metodo: {error.NAME_FUNCTION}", EError.Device, ELevelError.Medium);
                        }
                        else
                        {
                            var error = (RequestLogDevice)log;
                            result = await api.CallApi("SaveLogDevice", error);
                            SaveErrorControl(error.Description, "", EError.Device, error.Level);

                            if (error.Level == ELevelError.Strong)
                            {
                                SaveLog(new RequestLog
                                {
                                    Reference = "",
                                    Description = error.Description,
                                    State = 2,
                                    Date = DateTime.Now
                                }, ELogType.General);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
        }

        public static void SaveLog(string mensaje)
        {
            try
            {
                var json = JsonConvert.SerializeObject(mensaje);
                var pathFile = Utilities.GetConfiguration("PathLog");
                if (!Directory.Exists(pathFile))
                {
                    Directory.CreateDirectory(pathFile);
                }
                var file = "Log" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
                var nameFile = Path.Combine(pathFile, file);
                if (!File.Exists(nameFile))
                {
                    var archivo = File.CreateText(nameFile);
                    archivo.Close();
                }
                using (StreamWriter sw = File.AppendText(nameFile))
                {
                    sw.WriteLine(string.Concat(DateTime.Now.ToString(), " ", json));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat("Error: ", " ", ex.Message, " ", ex.StackTrace));
            }
        }

        public static void SaveLog(string clase, string metodo, string estado, string Descripcion, TransactionBetPlay Ts)
        {
            try
            {

                if(Ts == null)
                {
                    IdEvents Date = new IdEvents
                    {

                        IdTransaccion = "0",
                        Fecha = DateTime.Now.ToString(),
                        Clase = clase,
                        Metodo = metodo,
                        Description = Descripcion,
                        Estado = estado,
                        Cliente = "GANA",
                    };

                    ParametersLog Log = new ParametersLog
                    {

                        Id = "0",

                        Transaccion = Date,
                    };

                    var json = JsonConvert.SerializeObject(Log);

                    var pathFile = Utilities.GetConfiguration("PathLog");
                    if (!Directory.Exists(pathFile))
                    {
                        Directory.CreateDirectory(pathFile);
                    }
                    var file = "Log" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
                    var nameFile = Path.Combine(pathFile, file);
                    if (!File.Exists(nameFile))
                    {
                        var archivo = File.CreateText(nameFile);
                        archivo.Close();
                    }
                    using (StreamWriter sw = File.AppendText(nameFile))
                    {
                        sw.WriteLine(json);
                    }


                }
                else
                {
                    IdEvents Date = new IdEvents
                    {

                        IdTransaccion = Ts.IdTransactionAPi.ToString(),
                        Fecha = DateTime.Now.ToString(),
                        Clase = clase,
                        Metodo = metodo,
                        Description = Descripcion,
                        Estado = estado,
                        Cliente = "GANA",
                    };

                    ParametersLog Log = new ParametersLog
                    {

                        Id = Ts.IdTransactionAPi.ToString(),

                        Transaccion = Date,
                    };

                    var json = JsonConvert.SerializeObject(Log);

                    var pathFile = Utilities.GetConfiguration("PathLog");
                    if (!Directory.Exists(pathFile))
                    {
                        Directory.CreateDirectory(pathFile);
                    }
                    var file = "Log" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
                    var nameFile = Path.Combine(pathFile, file);
                    if (!File.Exists(nameFile))
                    {
                        var archivo = File.CreateText(nameFile);
                        archivo.Close();
                    }
                    using (StreamWriter sw = File.AppendText(nameFile))
                    {
                        sw.WriteLine(json);
                    }

                }

              
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat("Error: ", " ", ex.Message, " ", ex.StackTrace));
            }
        }

        public static void SaveErrorControl(string desciption, string observation, EError error, ELevelError level, int device = 0, int idTrensaction = 0)
        {
            try
            {
                Task.Run(async () =>
                {
                    if (_dataConfiguration != null)
                    {
                        var idPaypad = _dataConfiguration.ID_PAYPAD;
                        if (idPaypad == null)
                        {
                            idPaypad = int.Parse(Utilities.GetConfiguration("idPaypad"));
                        }

                        if (desciption.Contains("FATAL"))
                        {
                            level = ELevelError.Strong;
                        }

                        PAYPAD_CONSOLE_ERROR consoleError = new PAYPAD_CONSOLE_ERROR
                        {
                            PAYPAD_ID = (int)idPaypad,
                            DATE = DateTime.Now,
                            STATE = 0,
                            DESCRIPTION = desciption,
                            OBSERVATION = observation,
                            ERROR_ID = (int)error,
                            ERROR_LEVEL_ID = (int)level,
                            REFERENCE = idTrensaction
                        };

                        SqliteDataAccess.InsetConsoleError(consoleError);

                        List<PAYPAD_ERROR_CONSOLE> consoleErro = new List<PAYPAD_ERROR_CONSOLE>()
                        {
                            new PAYPAD_ERROR_CONSOLE
                            {
                                PAYPAD_ID = (int)idPaypad,
                                DATE = DateTime.Now,
                                STATE = 1,
                                DESCRIPTION = desciption,
                                OBSERVATION = observation,
                                ERROR_ID = (int)error,
                                ERROR_LEVEL_ID = (int)level
                            }
                        };

                        await api.CallApi("SaveErrorConsole", consoleErro);
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
        }

        public static async Task<int> SavePayer(PAYER payer)
        {
            try
            {
                payer.STATE = true;

                var resultPayer = await api.CallApi("SavePayer", payer);

                if (resultPayer != null)
                {
                    return JsonConvert.DeserializeObject<int>(resultPayer.ToString());
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
            return 0;
        }

        public static async Task SaveTransaction(TransactionBetPlay transaction)
        {
            try
            {
                if (transaction != null)
                {
                    //bool response = await CreateConsecutivoDashboard(transaction);

                    //if (!response)
                    //{
                    //    return;
                    //}

                    //transaction.IsReturn = await ValidateMoney(transaction);

                    if (transaction.payer == null)
                    {
                        transaction.payer = new PAYER
                        {
                            IDENTIFICATION = "0",
                            NAME = Utilities.GetConfiguration("NAME_PAYPAD"),
                            LAST_NAME = Utilities.GetConfiguration("LAST_NAME_PAYPAD")
                        };
                    }

                    transaction.payer.PAYER_ID = await SavePayer(transaction.payer);

                    if (transaction.payer.PAYER_ID > 0)
                    {
                        var data = new TRANSACTION
                        {
                            TYPE_TRANSACTION_ID = Convert.ToInt32(transaction.Tipo),
                            PAYER_ID = transaction.payer.PAYER_ID,
                            STATE_TRANSACTION_ID = Convert.ToInt32(transaction.State),
                            TOTAL_AMOUNT = Convert.ToDecimal(transaction.Amount),
                            DATE_END = DateTime.Now,
                            TRANSACTION_ID = 0,
                            RETURN_AMOUNT = 0,
                            INCOME_AMOUNT = 0,
                            PAYPAD_ID = 0,
                            DATE_BEGIN = DateTime.Now,
                            STATE_NOTIFICATION = 0,
                            STATE = 0,
                            DESCRIPTION = "Transaccion iniciada",
                            TRANSACTION_REFERENCE = ""
                        };

                        data.TRANSACTION_DESCRIPTION.Add(new TRANSACTION_DESCRIPTION
                        {
                            AMOUNT = Convert.ToDecimal(transaction.Amount),
                            TRANSACTION_ID = data.ID,
                            TRANSACTION_PRODUCT_ID = Convert.ToInt32(transaction.Type),
                            DESCRIPTION = Utilities.GetDescriptionEnum(typeof(ETypeTramites), transaction.Type.ToString()) + transaction.valor,
                            EXTRA_DATA = "hola",
                            TRANSACTION_DESCRIPTION_ID = 0,
                            STATE = true
                        });

                        if (data != null)
                        {
                            var responseTransaction = await api.CallApi("SaveTransaction", data);
                            if (responseTransaction != null)
                            {
                                transaction.IdTransactionAPi = JsonConvert.DeserializeObject<int>(responseTransaction.ToString());

                                if (transaction.IdTransactionAPi > 0)
                                {
                                    data.TRANSACTION_ID = transaction.IdTransactionAPi;
                                    transaction.TransactionId = SqliteDataAccess.SaveTransaction(data);
                                }
                            }
                            else
                            {
                                SaveLog(new RequestLog
                                {
                                    Reference = transaction.reference,
                                    Description = string.Concat(MessageResource.NoInsertTransaction, " en su primer intento "),
                                    State = 1,
                                    Date = DateTime.Now
                                }, ELogType.General);

                                responseTransaction = await api.CallApi("SaveTransaction", data);
                                if (responseTransaction != null)
                                {
                                    transaction.IdTransactionAPi = JsonConvert.DeserializeObject<int>(responseTransaction.ToString());

                                    if (transaction.IdTransactionAPi > 0)
                                    {
                                        data.TRANSACTION_ID = transaction.IdTransactionAPi;
                                        transaction.TransactionId = SqliteDataAccess.SaveTransaction(data);
                                    }
                                }
                                else
                                {
                                    SaveLog(new RequestLog
                                    {
                                        Reference = transaction.reference,
                                        Description = string.Concat(MessageResource.NoInsertTransaction, " en su segundo intento "),
                                        State = 1,
                                        Date = DateTime.Now
                                    }, ELogType.General);
                                }
                            }
                        }
                    }
                    else
                    {
                        SaveLog(new RequestLog
                        {
                            Reference = transaction.reference,
                            Description = MessageResource.NoInsertPayment + transaction.payer.IDENTIFICATION,
                            State = 1,
                            Date = DateTime.Now
                        }, ELogType.General);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
        }

        public async static Task<bool> CreateConsecutivoDashboard(TransactionBetPlay ts)
        {
            try
            {
                var response = await api.CallApi("GetInvoiceData", true);

                if (response != null)
                {
                    //if (response.CodeError == 200)
                    //{
                    var responseApi = JsonConvert.DeserializeObject<SP_GET_INVOICE_DATA_Result>(response.ToString());

                    if (responseApi.IS_AVAILABLE == true)
                    {
                        ts.ConsecutivoId = Convert.ToInt32(responseApi.RANGO_ACTUAL);
                        return true;
                    }
                    //}
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async static void SaveDetailsTransaction(int idTransactionAPi, decimal enterValue, int opt, int quantity, string code, string description)
        {
            try
            {
                var details = new RequestTransactionDetails
                {
                    Code = code,
                    Denomination = Convert.ToInt32(enterValue),
                    Operation = opt,
                    Quantity = quantity,
                    TransactionId = idTransactionAPi,
                    Description = description
                };

                var response = await api.CallApi("SaveTransactionDetail", details);

                if (response != null)
                {
                    SqliteDataAccess.SaveTransactionDetail(details, 1);
                }
                else
                {
                    SqliteDataAccess.SaveTransactionDetail(details, 0);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
        }

        public async static void UpdateTransaction(TransactionBetPlay transaction)
        {
            try
            {
                if (transaction != null)
                {
                    TRANSACTION tRANSACTION = SqliteDataAccess.UpdateTransaction(transaction);

                    if (tRANSACTION != null)
                    {
                        tRANSACTION.TRANSACTION_REFERENCE = transaction.ConsecutivoId.ToString();

                        var responseTransaction = await api.CallApi("UpdateTransaction", tRANSACTION);
                        if (responseTransaction != null)
                        {
                            tRANSACTION.STATE = 1;
                            SqliteDataAccess.UpdateTransactionState(tRANSACTION);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
        }

        public static async Task<bool> UpdateBalance(PaypadOperationControl paypadData)
        {
            try
            {
                string action = "";

                if (_dataPayPlus.StateBalanece)
                {
                    action = "UpdateBalance";
                }
                else
                {
                    action = "UpdateUpload";
                }

                var response = await api.CallApi(action, paypadData);

                if (response != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
                return false;
            }
        }

        public static async Task<bool> ValidateUser(string name, string pass)
        {
            try
            {
                var response = await api.CallApi("ValidateUserPayPad", new RequestAuth
                {
                    UserName = name,
                    Password = pass
                });

                if (response != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
                return false;
            }
        }

        public static async Task<PaypadOperationControl> DataListPaypad(ETypeAdministrator typeAdministrator)
        {
            try
            {
                string action = "";

                if (typeAdministrator == ETypeAdministrator.Balancing)
                {
                    action = "GetBalance";
                }
                else
                {
                    action = "GetUpload";
                }

                var response = await api.CallApi(action);

                if (response != null)
                {
                    var operationControl = JsonConvert.DeserializeObject<PaypadOperationControl>(response.ToString());

                    return operationControl;
                }

                return null;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
                return null;
            }
        }

        public static void NotificateInformation()
        {
            try
            {
                Task.Run(async () =>
                {
                    var transactions = SqliteDataAccess.GetTransactionNotific();
                    if (transactions.Count > 0)
                    {
                        foreach (var transaction in transactions)
                        {
                            var responseTransaction = await api.CallApi("UpdateTransaction", transaction);
                            if (responseTransaction != null)
                            {
                                transaction.STATE = 1;
                                SqliteDataAccess.UpdateTransactionState(transaction);
                            }
                        }
                    }

                    var detailTeansactions2 = SqliteDataAccess.GetDetailsTransaction();
                    foreach (var detail in detailTeansactions2)
                    {
                        var response = await api.CallApi("SaveTransactionDetail", new RequestTransactionDetails
                        {
                            Code = detail.CODE,
                            Denomination = Convert.ToInt32(detail.DENOMINATION),
                            Operation = (int)detail.OPERATION,
                            Quantity = (int)detail.QUANTITY,
                            TransactionId = (int)detail.TRANSACTION_ID,
                            Description = detail.DESCRIPTION
                        });

                        if (response != null)
                        {
                            detail.STATE = 1;
                            SqliteDataAccess.UpdateTransactionDetailState(detail);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
        }

        private static async Task<bool> ValidateMoney(TransactionBetPlay transaction)
        {
            try
            {
                if (Convert.ToDecimal(transaction.Amount) > 0)
                {
                    var isValidateMoney = await api.CallApi("ValidateDispenserAmount", transaction.Amount);
                    if (isValidateMoney != null)
                    {
                        return (bool)isValidateMoney;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
            return false;
        }

        public static async Task<int> GetConsecutive(bool isIncrement = true)
        {
            try
            {
                var response = await api.CallApi("GetConsecutiveTransaction", isIncrement);

                if (response != null)
                {
                    var consecutive = JsonConvert.DeserializeObject<ResponseConsecutive>(response.ToString());

                    if (consecutive.IS_AVAILABLE == true)
                    {
                        return int.Parse(consecutive.RANGO_ACTUAL.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, ex.ToString());
            }
            return 0;
        }



    }
}
