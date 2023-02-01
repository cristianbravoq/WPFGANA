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
using WPFGANA.Resources;
using WPFGANA.Services;
using WPFGANA.Services.Object;

namespace WPFGANA.UserControls.Administrator
{
    /// <summary>
    /// Lógica de interacción para ConfigurateUC.xaml
    /// </summary>
    public partial class ConfigurateUC : UserControl
    {

       
        private AdminPayPlus init;
        private ApiIntegration cootregua;

        public ConfigurateUC()
        {
            try
            {
                InitializeComponent();

                if (init == null)
                {
                    init = new AdminPayPlus();
                }

                txtMs.DataContext = init;

                Initial();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private async void Initial()
        {
            try
            {
                init.callbackResult = result =>
                {
                    ProccesResult(result);
                };

                init.Start();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private async void ProccesResult(bool result)
        {
            try
            {
                if (AdminPayPlus.DataPayPlus.StateUpdate)
                {
                    Utilities.ShowModal(MessageResource.UpdateAplication, EModalType.Error, true);
                    Utilities.UpdateApp();
                }
                else if (AdminPayPlus.DataPayPlus.StateBalanece)
                {
                    Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, null, MessageResource.ModoAdministrativo);
                    //Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Balancing);
                }
                else if (AdminPayPlus.DataPayPlus.StateUpload)
                {
                    Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, null, MessageResource.ModoAdministrativo);
                    //Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Upload);
                }
                else
                {
                    Finish(result);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                Utilities.ShowModal(MessageResource.NoService, EModalType.Error, false);
                Initial();
            }
        }

        private void Finish(bool state)
        {
            try
            {
                // Task.Run(() =>
                //{
                if (state)
                {
                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = MessageResource.YesGoInitial,
                        State = 1,
                        Date = DateTime.Now
                    }, ELogType.General);

                    Utilities.navigator.Navigate(UserControlView.Menu);
                }
                else
                {
                    if (!string.IsNullOrEmpty(AdminPayPlus.DataPayPlus.Message))
                    {
                        AdminPayPlus.SaveLog(new RequestLog
                        {
                            Reference = "",
                            Description = MessageResource.NoGoInitial,
                            State = 6,
                            Date = DateTime.Now
                        }, ELogType.General);

                        Error.SaveLogError("Finish", this.GetType().Name, null, AdminPayPlus.DataPayPlus.Message);
                        Utilities.ShowModal(MessageResource.NoService + " " + MessageResource.NoMoneyKiosco, EModalType.Error, false);
                        Initial();
                    }
                    else
                    {

                        AdminPayPlus.SaveLog(new RequestLog
                        {
                            Reference = "",
                            Description = MessageResource.NoGoInitial,
                            State = 2,
                            Date = DateTime.Now
                        }, ELogType.General);

                        Error.SaveLogError("Finish", this.GetType().Name, null, init.DescriptionStatusPayPlus);
                        Utilities.ShowModal(MessageResource.NoService + " " + init.DescriptionStatusPayPlus, EModalType.Error, false);
                        Initial();
                    }
                }
                //  });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                Utilities.ShowModal(MessageResource.NoService, EModalType.Error, false);
                Initial();
            }
        }
    }
}
