using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFGANA.Classes;

namespace WPFGANA.Services.ObjectIntegration
{
    public class ResponseGeneric
    {
        public string ResponseMessage { get; set; }
        public object ResponseData { get; set; }
        public EResponseCode ResponseCode { get; set; }
    }

    public class ResponseToken
    {
        public string Data { get; set; }
        public EResponseCode ResponseCode { get; set; }

    }



    public class ResponseUser
    {
        public bool ok { get; set; }
        public string msgError { get; set; }
        public int errorCode { get; set; }
        public string token { get; set; }
        public Model model { get; set; }
    }

    public class Model
    {
        public string nit { get; set; }
        public long identificacion { get; set; }
        public string serieTerminal { get; set; }
        public string mensaje { get; set; }
        public int codigoVendedor { get; set; }
        public int codigoPuntoVenta { get; set; }
    }

    //BetPlay

    public class ResponseNotify
    {
        public bool ok { get; set; }
        public string msgError { get; set; }
        public int errorCode { get; set; }
        public string token { get; set; }
        public ModelNotify model { get; set; }
    }

    public class ModelNotify
    {
        public long cedulaApostador { get; set; }
        public long montoRecargado { get; set; }
        public long saldoCuenta { get; set; }
        public string numeroSeguridad { get; set; }

    }

    // SuperChance

    //Respuesta Servicio Fecha

    public class ResponseFecha
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public ModelFecha model { get; set; }
    }

    public class ModelFecha
    {
        public ListFecha[] list { get; set; }
    }

    public class ListFecha
    {
        public int idLoteria { get; set; }
        public int sorteo { get; set; }
        public string abreviatura { get; set; }
        public string desLoteria { get; set; }
        public string horaSorteo { get; set; }
        public int numCifras { get; set; }
        public string esExcluyente { get; set; }
        public DateTime fecSorteo { get; set; }
        public string nacional { get; set; }
    }

    //Respuesta Numero Random

    public class ResponseRandom
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public ModelRandom model { get; set; }
    }

    public class ModelRandom
    {
        public string numero { get; set; }
    }

    //Respuesta Validar Numero

    public class ResponseValidarNumero
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public ModelValidarNumero model { get; set; }
    }

    public class ModelValidarNumero
    {
        public DuplaValidarNumero[] duplas { get; set; }
        public string message { get; set; }
    }

    public class DuplaValidarNumero
    {
        public string numero { get; set; }
        public string loteria { get; set; }
        public string disponible { get; set; }
    }

    // Respuesta Liquidar Chance

    public class ResponseLiquidar
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public ModelLiquidar model { get; set; }
    }

    public class ModelLiquidar
    {
        public float valorIva { get; set; }
        public float valorTotal { get; set; }
        public float valor { get; set; }
    }

    //Respuesta Crear Chance

    public class ResponseCreate
    {
        public bool ok { get; set; }
        public string msgError { get; set; }
        public int errorCode { get; set; }
        public ErrorinformationCreate errorInformation { get; set; }
        public string token { get; set; }
    }

    public class ErrorinformationCreate
    {
        public string msg { get; set; }
    }

    public class ResponseOKCreate
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public ModelCreate model { get; set; }
    }

    public class ModelCreate
    {
        public DateTime fecVenta { get; set; }
        public int ideEmpresa { get; set; }
        public int idEquipo { get; set; }
        public int ideOficina { get; set; }
        public int ideBarrio { get; set; }
        public int consecutivoBeneficencia { get; set; }
        public int ideCiudad { get; set; }
        public int idePais { get; set; }
        public int colilla { get; set; }
        public int ideCanal { get; set; }
        public string claVenta { get; set; }
        public int ideDepartamento { get; set; }
        public int ideRegion { get; set; }
        public string serie { get; set; }
        public int ideSitioVenta { get; set; }
        public int porcentajeComision { get; set; }
        public int idSitioVenta { get; set; }
        public int ideZona { get; set; }
    }

    //Respuesta Cargar Parametros

    public class ResponseParameters
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public ModelParameters model { get; set; }
    }

    public class ModelParameters
    {
        public int vlrMinimoColilla { get; set; }
        public int vlrMaximoColilla { get; set; }
        public int numMinimoLineas { get; set; }
        public int numMaximoLineas { get; set; }
        public int numMinimoLoterias { get; set; }
        public int numMaximoLoterias { get; set; }
        public int iva { get; set; }
        public bool calculaValores { get; set; }
        public string sigla { get; set; }
        public int cifras { get; set; }
        public DetallesmodalidadParameters[] detallesModalidad { get; set; }
        public PlanespremioParameters[] planesPremios { get; set; }
        public ValoresapuestaParameters[] valoresApuesta { get; set; }
    }

    public class DetallesmodalidadParameters
    {
        public string posicionesJuego { get; set; }
        public string posicionesApuesta { get; set; }
        public int valorMaximoDirecto { get; set; }
        public int valorMinimoDirecto { get; set; }
        public int valorMaximoCuna { get; set; }
        public int valorMinimoCuna { get; set; }
        public int valorMaximoCombinado { get; set; }
        public int valorMinimoCombinado { get; set; }
        public int valorMaximoCifra { get; set; }
        public int valorMinimoCifra { get; set; }
        public float porcentajeDirecto { get; set; }
        public float porcentajeCuna { get; set; }
        public float porcentajeCombinado { get; set; }
        public float porcentajeCifra { get; set; }
    }

    public class PlanespremioParameters
    {
        public int cifras { get; set; }
        public int pagaDirecto { get; set; }
        public int pagaCuna { get; set; }
        public int pagaCombinado { get; set; }
        public int pagaCifra { get; set; }
    }

    public class ValoresapuestaParameters
    {
        public int valor { get; set; }
    }

    //Recargas

    //Response for services Login

    public class ResponseLogin
    {
        public bool ok { get; set; }
        public ModelLogin model { get; set; }
    }

    public class ModelLogin
    {
        public int tipoUsuario { get; set; }
        public int idUsuario { get; set; }
        public string username { get; set; }
        public int estado { get; set; }
        public RoleLogin[] roles { get; set; }
        public string token { get; set; }
        public int idAplicacion { get; set; }
        public string lastAccessIp { get; set; }
        public DateTime lastAccessDate { get; set; }
        public int wrongPasswordCount { get; set; }
        public int idSistema { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string clientId { get; set; }
        public string publicKeyPemClient { get; set; }
        public string privateKeyPemServer { get; set; }
        public string publicKeyPemServer { get; set; }
    }

    public class RoleLogin
    {
        public int idRol { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
    }

    //Response for services Gepackets

    public class ResponseGetPackets
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public string msgError { get; set; }
        public ModelPackets model { get; set; }
    }

    public class ModelPackets
    {
        public ListPackets[] list { get; set; }
    }

    public class ListPackets
    {
        public float porcentajeComision { get; set; }
        public int idOperador { get; set; }
        public string desOperador { get; set; }
        public string tipoRecarga { get; set; }
        public int idPaqueteRecarga { get; set; }
        public string idPaqueteOperador { get; set; }
        public string nomPaquete { get; set; }
        public string desPaquete { get; set; }
        public float valorComercial { get; set; }
        public int vigencia { get; set; }
        public int cantidad { get; set; }
        public bool consumeFacturacion { get; set; }
    }

    //Response for services ProductCreate

    public class ResponseProductCreate
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public string msgError { get; set; }

        public ModelProducct model { get; set; }
    }

    public class ModelProducct
    {
        public DateTime fecVenta { get; set; }
        public string secFacActual { get; set; }
        public string claVenta { get; set; }
        public int idEquipo { get; set; }
        public string resolucion { get; set; }
        public string serie { get; set; }
        public string prefijo { get; set; }
        public int idSitioVenta { get; set; }
        public int colilla { get; set; }
    }

    //Response for services CreatePacket

    public class ResponseCreatePacket
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public ModelCreatePacket model { get; set; }
    }

    public class ModelCreatePacket
    {
        public DateTime fecVenta { get; set; }
        public string secFacActual { get; set; }
        public string claVenta { get; set; }
        public int idEquipo { get; set; }
        public string resolucion { get; set; }
        public string serie { get; set; }
        public string prefijo { get; set; }
        public int idSitioVenta { get; set; }
        public int colilla { get; set; }
    }
}
