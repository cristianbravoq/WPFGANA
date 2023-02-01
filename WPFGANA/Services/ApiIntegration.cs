 using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPFGANA.Classes;
using WPFGANA.Services.ObjectIntegration;

namespace WPFGANA.Services
{
    public class ApiIntegration
    {
        private HttpClient client;
        private string basseAddress;

        private static string Aplicacion;
        private static string Dispositivo;
        private static string Token;
        private static string KeyIntegration;

        public ApiIntegration(string dispositivo)
        {

            Aplicacion = Assembly.GetCallingAssembly().GetName().Name;
            Dispositivo = dispositivo;
        }

        public async Task<ResponseGeneric> GetData(object requestData, string controller, string BaseAddress)
        {
            try
            {

                HttpResponseMessage response = new HttpResponseMessage();

                client = new HttpClient();
                var request = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                client.BaseAddress = new Uri(basseAddress);

                response = client.PostAsync(controller, content).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("Error en el servicio del cliente", response.RequestMessage.ToString(), EError.Customer, ELevelError.Medium);
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();


                AdminPayPlus.SaveErrorControl("Error en el servicio del cliente", "", EError.Customer, ELevelError.Medium);

                var ResponseData = JsonConvert.DeserializeObject<ResponseGeneric>(result);

                //if (result != null)
                //{
                //    return result;
                //}

                return ResponseData;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public async Task<ResponseGeneric> GetData(string controller, string BaseAddress)
        {
            try
            {

                HttpResponseMessage response = new HttpResponseMessage();

                client = new HttpClient();
                //var request = JsonConvert.SerializeObject(requestData);
                var content = new StringContent("dato", Encoding.UTF8, "Application/json");
                client.BaseAddress = new Uri(basseAddress);

                if (controller == "BetPlay/ValidateToken" || controller == "BetPlay/ValidateServices" || controller == "SuperChance/ValidateServices" || controller == "SuperChance/ValidateToken")
                {
                    response = client.GetAsync(controller).GetAwaiter().GetResult();

                }
                else
                {
                    response = client.PostAsync(controller, content).GetAwaiter().GetResult();
                }


                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("Error en el servicio del cliente", response.RequestMessage.ToString(), EError.Customer, ELevelError.Medium);
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();

                var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(result);

                //if (result != null)
                //{
                //    return result;
                //}

                return requestData;

                AdminPayPlus.SaveErrorControl("Error en el servicio del cliente", "", EError.Customer, ELevelError.Medium);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }



        public ResponseGeneric ValidateTokenBetPlay()
        {
            try
            {
                string controller = Utilities.GetConfiguration("ValidateToken");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response TypeDocuments: ", response.ToString()), "Documents", EError.Api, ELevelError.Mild);


                    return response;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public ResponseGeneric ValidateServicesBetPlay()
        {
            try
            {
                string controller = Utilities.GetConfiguration("ValidateServices");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response TypeDocuments: ", response.ToString()), "Documents", EError.Api, ELevelError.Mild);


                    return response;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public ResponseGeneric GetToken()
        {
            try
            {
                string controller = Utilities.GetConfiguration("GetTokenBetPlay");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response TypeDocuments: ", response.ToString()), "Documents", EError.Api, ELevelError.Mild);
                    

                    return response;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public ResponseGeneric validateUser(RequestMachine Machine)
        {
            try
            {

                string controller = Utilities.GetConfiguration("ValidateUser");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                //        var Data = JsonConvert.SerializeObject(ValidateClient);

                var response = GetData(Machine, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response TypeDocuments: ", response.ToString()), "Documents", EError.Api, ELevelError.Mild);


                    return response;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public ResponseGeneric NotifyPayment(RequestNotify Machine)
        {
            try
            {

                string controller = Utilities.GetConfiguration("Notify");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                //        var Data = JsonConvert.SerializeObject(ValidateClient);

                var response = GetData(Machine, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response TypeDocuments: ", response.ToString()), "Documents", EError.Api, ELevelError.Mild);


                    return response;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }


        //SuperChance

        public ResponseGeneric ValidateTokenSuperChance()
        {
            try
            {
                string controller = Utilities.GetConfiguration("ValidateTokenSuperChance");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationSuperChance");

                var response = GetData(controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response Validar: ", response.ToString()), "Validar", EError.Api, ELevelError.Mild);


                    return response;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public ResponseGeneric ValidateServicesSuperChance()
        {
            try
            {
                string controller = Utilities.GetConfiguration("ValidateServicesSuperChance");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationSuperChance");

                var response = GetData(controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response Validar: ", response.ToString()), "Validar", EError.Api, ELevelError.Mild);


                    return response;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return null;
        }

        public ResponseGeneric GetLotteries(RequestFecha Loterias)
        {
            try
            {


                string controller = Utilities.GetConfiguration("GetLotteries");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationSuperChance");

                var response = GetData(Loterias, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response Validar: ", response.ToString()), "Validar", EError.Api, ELevelError.Mild);


                    return response;

                }

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;

        }

        public ResponseGeneric GetNumberRandom(RequestNumeroRandom request)
        {
            try
            {


                string controller = Utilities.GetConfiguration("GetNumber");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationSuperChance");

                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response Validar: ", response.ToString()), "Validar", EError.Api, ELevelError.Mild);


                    return response;

                }

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;

        }

        public ResponseGeneric ValidateNumber(RequestValidarNumero request)
        {
            try
            {

                string controller = Utilities.GetConfiguration("ValidateNumber");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationSuperChance");

                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
       //              var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response Validar: ", response.ToString()), "Validar", EError.Api, ELevelError.Mild);


                    return response;

                }

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;

        }

        public ResponseGeneric LiquidarChance(RequestLiquidar request)
        {
            try
            {

                string controller = Utilities.GetConfiguration("LiquidarChance");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationSuperChance");

                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response Validar: ", response.ToString()), "Validar", EError.Api, ELevelError.Mild);


                    return response;

                }

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;

        }

        public ResponseGeneric CrearChance(RequestCrearChance request)
        {
            try
            {

                string controller = Utilities.GetConfiguration("CrearChance");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationSuperChance");

                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response Validar: ", response.ToString()), "Validar", EError.Api, ELevelError.Mild);


                    return response;

                }

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;

        }

        public ResponseGeneric CargarParametros(RequestParameters request)
        {
            try
            {

                string controller = Utilities.GetConfiguration("Parametros");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationSuperChance");

                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    // var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(response.ToString());

                    AdminPayPlus.SaveErrorControl(string.Concat("Response Validar: ", response.ToString()), "Validar", EError.Api, ELevelError.Mild);


                    return response;

                }

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;
        }

        // Recharge Cel with GANA Services

        public  ResponseGeneric LoginMachine()
        {

            try
            {
                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationRecargas");
                string controller = Utilities.GetConfiguration("LoginMachine");
                var response = GetData(controller, basseAddress).GetAwaiter().GetResult();

                if(response != null)
                {
                    return response;
                }

            }
            catch(Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;

        }

        public ResponseGeneric GetPackets()
        {
            try
            {
                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationRecargas");

                RequestGetPackets Request = new RequestGetPackets
                {
                    idProductoRed = Convert.ToInt32(Utilities.GetConfiguration("idProductoRedPack")),
                };

                string controller = Utilities.GetConfiguration("GetPackets");
                var response = GetData(Request, controller, basseAddress).GetAwaiter().GetResult();

                if(response != null)
                {
                    return response;
                }
            }
            catch(Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;

        }

        public ResponseGeneric CreatePacket(RequestCreatePacketMaquina request)
        {
            try
            {
                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationRecargas");

                string controller = Utilities.GetConfiguration("CreatePack");
                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    return response;
                }
            }
            catch(Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;
        }

        public ResponseGeneric CreateProduct(RequestRecharge request)
        {
            try
            {
                basseAddress = Utilities.GetConfiguration("basseAddressIntegrationRecargas");

                string controller = Utilities.GetConfiguration("CreateProducts");
                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    return response;
                }
            }
            catch(Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }

            return null;
        }

        //Otros
        private string GetDataDescrypt(string dataEncrypt)
        {
            try
            {
                return Encryptor.Decrypt(dataEncrypt, KeyIntegration);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                return string.Empty;
            }
        }

    }
}
