using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WPFGANA.Classes;
using WPFGANA.Models;

namespace WPFGANA.Services.Object
{
    public class Response
    {
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
