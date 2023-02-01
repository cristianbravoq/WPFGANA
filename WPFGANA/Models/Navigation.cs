using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFGANA.Classes;
using WPFGANA.Models;
using WPFGANA.Services.Object;
using WPFGANA.UserControls.BetPlay;
using WPFGANA.UserControls;
using WPFGANA.UserControls.Administrator;
using WPFGANA.UserControls.SuperChance;
using WPFGANA.UserControls.Recargas;
using WPFGANA.UserControls.Recargas.Paquetes;
using WPFGANA.UserControls.Recargas.Recargas;
using WPFGANA.Services.ObjectIntegration;

namespace WPFGANA.Models
{
    public class Navigation : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private UserControl _view;

        public UserControl View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(View)));
            }
        }

        public void Navigate(UserControlView newWindow, object data = null, object complement = null) => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                switch (newWindow)
                {
                    //BetPlay

                    case UserControlView.Config:
                        View = new ConfigurateUC();
                        break;
                    case UserControlView.ReturnMoney:
                        View = new ReturnMoneyUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Menu:
                        View = new Main();
                        break;
                    case UserControlView.Login:
                        View = new Login();
                        break;
                    case UserControlView.Recharge:
                        View = new RechargeUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Validate:
                        View = new ValidateUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Payment:
                        View = new UserControls.BetPlay.PaymentUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Finish:
                        View = new FinishBetPlayUC((TransactionBetPlay)data);
                        break;

                    //SuperChance

                    case UserControlView.info:
                        View = new InfoUC();
                        break;
                    case UserControlView.Form:
                        View = new FormUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Dia:
                        View = new DateTxUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Loterias:
                        View = new LotteryUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Apuesta:
                        View = new SelectNumUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Verificar:
                        View = new ConfirmLotteryUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Eliminar:
                        View = new CancelLotteryUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.PagosChance:
                        View = new UserControls.SuperChance.PaymentUC((TransactionBetPlay)data);
                        break;
                    case UserControlView.Politicas:
                        View = new Politicas((TransactionBetPlay)data);
                        break;
                    case UserControlView.Adicional:
                        View = new ExtraSelectNum((TransactionBetPlay)data);
                        break;

                    case UserControlView.Imprimir:
                        View = new PrintUC((TransactionBetPlay)data);
                        break;

                    case UserControlView.FinishChance:
                        View = new FinishChance((TransactionBetPlay)data);
                        break;

                    //Recargas

                    case UserControlView.SelectOperator:
                        View = new SelectOperador((TransactionBetPlay)data, (ResponseGetPackets)complement);
                        break;

                    case UserControlView.SelectOption:
                        View = new SelectOptionUC();
                        break;

                    case UserControlView.PackNum:
                        View = new NumeroPaquetesUC((TransactionBetPlay)data);
                        break;

                    case UserControlView.TypePack:
                        View = new PaquetesUC((TransactionBetPlay)data);
                        break;

                    case UserControlView.ValidatePack:
                        View = new ValidatePackUC((TransactionBetPlay)data);
                        break;

                    case UserControlView.RechargeNum:
                        View = new NumeroRecargaUC();
                        break;

                    case UserControlView.RechargeCel:
                        View = new RecargasUC((TransactionBetPlay)data);
                        break;

                    case UserControlView.ValidateRecharge:
                        View = new ValidateInfoUC((TransactionBetPlay)data);
                        break;

                    case UserControlView.PaymentRecharge:
                        View = new UserControls.Recargas.PaymentUC((TransactionBetPlay)data);
                        break;

                    case UserControlView.SuccesRechatge:
                        View = new SuccesTransactionUC((TransactionBetPlay)data);
                        break;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Navigate", ex, ex.ToString());
            }
            GC.Collect();
        });
    }
}