using System;
using System.Collections.Generic;
using System.ComponentModel;
using WPFGANA.Classes;
using WPFGANA.DataModel;
using WPFGANA.Services.ObjectIntegration;
using WPFGANA.ViewModel;

namespace WPFGANA.Models
{

    public class TransactionBetPlay : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public string tramite { get; set; }

        public ETypeTramites eTypeTramites { get; set; }
        public string Document { get; set; }

        public string TypeDocument { get; set; }

        public string Fecha { get; set; }

        public string Name { get; set; }

        public string Amount { get; set; }

        public int ValorGanar { get; set; }

        public float IVA { get; set; }

        public float Valor { get; set; }

        public int ExtraMum { get; set; }

        public ResponseFecha LotteryList { get; set; }

        public bool StateReturnMoney { get; set; }

        public int consecutivo { get; set; } = 800700625;

        public string valor { get; set; }

        public int StateNotification { get; set; }

        public bool statePaySuccess { get; set; }

        public string Observation { get; set; }

        public DateTime DateTransaction { get; set; }

        public string reference { get; set; }
        public ETransactionType Tipo { get; set; }

        public ETypeTramites Type { get; set; }

        public string StatePay { get; set; }

        public string NumeroLoteria { get; set; }

        public string NumeroDerecho { get; set; }

        public string NumeroCombinado { get; set; }

        public string nit { get; set; }

        public string calificacion { get; set; }

        public string SerieTerminal { get; set; }

        public int CodigoVendedor { get; set; }

        public int codigoPuntoVenta { get; set; }

        public PaymentViewModel Payment { get; set; }

        public List<NumeroLiquidar> NumeroLiquidar { get; set; } = new List<NumeroLiquidar>();
        public List<LoteriaLiquidar> LoteriaLiquidar { get; set; } = new List<LoteriaLiquidar>();

        public List<NumeroChance> NumeroChance { get; set; } = new List<NumeroChance>();
        public List<LoteriaChance> LoteriaChance { get; set; } = new List<LoteriaChance>();

        public ResponseValidarNumero NumberValidate { get; set; }

        public ResponseParameters Parametros { get; set; }

        //Recargas

        public ResponseGetPackets Operators { get; set; }

        public OperatorSelected SelectOperator { get; set; }

        public long NumOperator { get; set; }

        public string Operador { get; set; }

        public string Token { get; set; }

        public PAYER payer { get; set; }

        public ETransactionState State { get; set; }

        private int _transactionId { get; set; }

        public ResponseOKCreate create { get; set; }
        public ResponseNotify Notify { get; set; }

        public int TransactionId
        {
            get
            {
                return _transactionId;
            }
            set
            {
                _transactionId = value;
                OnPropertyRaised("TransactionId");
            }
        }

        private int _consecutivoId { get; set; }

        public int ConsecutivoId
        {
            get
            {
                return _consecutivoId;
            }
            set
            {
                _consecutivoId = value;
                OnPropertyRaised("ConsecutivoId");
            }
        }

        private int _idTransactionAPi { get; set; }

        public int IdTransactionAPi
        {
            get
            {
                return _idTransactionAPi;
            }
            set
            {
                _idTransactionAPi = value;
                OnPropertyRaised("IdTransactionAPi");
            }
        }


    }

    //Recargas

    public class OperatorSelected
    {
        public float porcentajeComision { get; set; }
        public int idOperador { get; set; }
        public string desOperador { get; set; }
        public string tipoRecarga { get; set; }
        public int idPaqueteRecarga { get; set; }
        public string idPaqueteOperador { get; set; }
        public string nomPaquete { get; set; }
        public string desPaquete { get; set; }
        public double valorComercial { get; set; }
        public int vigencia { get; set; }
        public int cantidad { get; set; }
        public bool consumeFacturacion { get; set; }
    }

    //Otros

    public class ResponseNotifyT
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

    public class ResponseFechaT
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

    //public class NumeroLiquidarT
    //{
    //    public string derecho { get; set; }
    //}

    //public class LoteriaLiquidarT
    //{
    //    public string idLoteria { get; set; }
    //    public string sorteo { get; set; } 
    //}


    //SuperChance

    public class TransactionChance : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

    }
}
