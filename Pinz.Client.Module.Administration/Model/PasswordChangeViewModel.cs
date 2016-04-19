using Com.Pinz.Client.Commons.Prism;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class PasswordChangeViewModel : BindableValidationBase
    {
        private string _oldPasword;
        [Required]
        public string OldPasword
        {
            get { return _oldPasword; }
            set { SetProperty(ref this._oldPasword, value); }
        }

        private string _newPasword;
        [Required]
        [MinLength(6)]
        public string NewPasword
        {
            get { return _newPasword; }
            set { SetProperty(ref this._newPasword, value); }
        }

        private string _newPasword2;
        [Required]
        [MinLength(6)]
        public string NewPasword2
        {
            get { return _newPasword2; }
            set { SetProperty(ref this._newPasword2, value); }
        }

        public void Reset()
        {
            OldPasword = null;
            NewPasword = null;
            NewPasword2 = null;
        }
    }
}
