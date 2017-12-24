using Livet;
using MaterialDesignThemes.Wpf;
using Rialto.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Rialto.ViewModels
{
    public class ExportDialogViewModel : ViewModel
    {
        private string _SaveDirectory;
        public string SaveDirectory
        {
            get { return _SaveDirectory; }
            set
            {
                _SaveDirectory = value;
                RaisePropertyChanged(nameof(SaveDirectory));
            }
        }

        private bool _Exporting = false;
        public bool Exporting
        {
            get { return _Exporting; }
            set
            {
                _Exporting = value;
                RaisePropertyChanged(nameof(Exporting));
            }
        }

        private long tagId;
        private ExportService service = new ExportService();

        public ExportDialogViewModel(long tagId)
        {
            this.tagId = tagId;
        }
        
        public async void Export()
        {
            Exporting = true;
            await service.Export(SaveDirectory, tagId, Some(new ExportOptions { OrderRename = true }));
            Exporting = false;
        }
    }
}
