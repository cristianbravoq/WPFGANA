using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGANA.Services.ObjectIntegration
{
    public class RequestGeneric
    {
        public string data { get; set; }
        public DateTime callDate { get; set; }
    }



    // BetPlay

    public class RequestMachine
    {

        public string Token { get; set; }
        public long identificacion { get; set; }

    }


    public class RequestNotify
    {
        public string serieTerminal { get; set; }
        public int dane { get; set; }
        public int codigoVendedor { get; set; }
        public int CodigoPuntoVenta { get; set; }
        public long CedulaApostador { get; set; }
        public int MontoRecargar { get; set; }
    }

    //SuperChance

    //Loterias 

    public class RequestFecha
    {
        public string fecha { get; set; } 
        public int idProductoRed { get; set; } 

    }

    //Numero Aleatorio

    public class RequestNumeroRandom
    {
        //  public string Numero { get; set; } = "";
        public int cifras { get; set; } = 4;

    }

    //Validar Numero Existente

    public class RequestValidarNumero
    {
        public List<NumeroValidar> numeros { get; set; }
        public List<LoteriaValidar> loterias { get; set; }
        public string fecSorteo { get; set; }
    }

    public class NumeroValidar
    {
        public string numero { get; set; }
    }

    public class LoteriaValidar
    {
        public string idLoteria { get; set; }
    }

    // Liquidar Chance

    public class RequestLiquidar
    {
        public string idProductoRed { get; set; } = "1";
        public string idUsuarioVendedor { get; set; } = "105421";
        public List<NumeroLiquidar> numeros { get; set; } = new List<NumeroLiquidar>();
        public List<LoteriaLiquidar> loterias { get; set; } = new List<LoteriaLiquidar>();
    }

    public class NumeroLiquidar
    {
        public string derecho { get; set; }
        public string cifra { get; set; }
        public string cuña { get; set; }

        public string Combinado { get; set; }
    }

    public class LoteriaLiquidar
    {
        public string idLoteria { get; set; } = "33";
        public string sorteo { get; set; } = "103071";
    }

    //Crear Chance

    public class RequestCrearChance
    {
        public string idTransaccion { get; set; } = "800700600";
        public string idProductoRed { get; set; } = "1";
        public string idProducto { get; set; } = "1";
        public string idUsuarioVendedor { get; set; } = "104546";
        public string fecSorteo { get; set; } = "2022-06-08";
        public string documentType { get; set; } = "CC";
        public string clienteNumDocumento { get; set; } = "1094900424";
        public string latitud { get; set; } = "6.217";
        public string longitud { get; set; } = "-75.567";
        public List<NumeroChance> numeros { get; set; } = new List<NumeroChance>();
        public List<LoteriaChance> loterias { get; set; } = new List<LoteriaChance>();
       // public Empresaexterna empresaExterna { get; set; } = new Empresaexterna();
    }

 //   public class Empresaexterna
  //  {
    //    public string esPedidoWeb { get; set; } = "S";
     //   public string codigoEmpresa { get; set; } = "2";
  //  }

    public class NumeroChance
    {
        public string numero { get; set; } 
        public int derecho { get; set; } 
        public int cifra { get; set; } 
        public int cuña { get; set; }

        public int Combinado { get; set; }
    }

    public class LoteriaChance
    {
        public string idLoteria { get; set; } = "29";
        public string desLoteria { get; set; } = "PAISITA 1";
        public string sorteo { get; set; } = "44493707";

        public string Abrv { get; set; }


    }

    //Cargar Parametros

    public class RequestParameters
    {
        public int idUsuarioVendedor { get; set; }
    }

    //Recargas Celular

    //---------------------------------------------------------------------------------------------------

    //Request Login Vending Recargas

    public class RequestLoginRecargas
    {
        public int idAplicacion { get; set; } = 16;// superchance, billonario, recargas
                                                   //  public int idAplicacion { get; set; } = 20;// betplay
        public string username { get; set; } = "vending";
        public string password { get; set; } = "123456";
        public string ClientId { get; set; } = "vending";
    }

    //Request for servies Getpackets

    public class RequestGetPackets
    {
        public int idProductoRed { get; set; } = 101078; //30;
    }

    //Request Packet Create

    public class RequestCreatePacketMaquina
    {
        public int idRed { get; set; } = 1;
        public string latitud { get; set; } = "6.217";
        public string longitud { get; set; } = "-75.567";
        public int idProducto { get; set; } = 96;
        public int idProductoRed { get; set; } = 101078;
        public int idUsuarioVendedor { get; set; } = 103620;
        public long phone { get; set; } = 3002552820;
        public int idPaqueteRecarga { get; set; } = 101098;
        public string valorComercial { get; set; } = "3200";
        public int idTransaccion { get; set; } = 88888953;
        public string providerClass { get; set; } = "PacketConexredProvider";
        public string idPaqueteOperador { get; set; } = "255";

   //     public string Operador { get; set; }
    }

    //Request Product Create

    public class RequestRecharge
    {
        public int idTransaccion { get; set; } = 88888954;
        public int idProducto { get; set; } = 6;
        public int idProductoRed { get; set; } = 63;
        public int idUsuarioVendedor { get; set; } = 103620;
        public string latitud { get; set; } = "6.217";
        public string longitud { get; set; } = "-75.567";
        public string providerClass { get; set; } = "GenericRechargeConexredProvider";
        public long cuentaRecarga { get; set; } = 3002552820;
        public string valorTotal { get; set; } = "5000";
        public string Operador { get; set; }


    }
}
