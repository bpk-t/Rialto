using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.ViewModels
{
    public class ExportDialogViewModel : ViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public ExportDialogViewModel(string title, string message)
        {
            this.Title = title;
            this.Message = message;
        }
    }
}
