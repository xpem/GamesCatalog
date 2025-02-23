using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesCatalog.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        bool isBusy;

        public bool IsBusy
        {
            get => isBusy; set
            {
                if (isBusy != value)
                {
                    SetProperty(ref (isBusy), value);
                }
            }
        }

        protected static bool IsOn => Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
