using Com.Pinz.Client.Commons.Prism;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class PasswordChangeViewModel : BindableValidationBase
    {
        private string _oldPassword;
        [Required]
        public string OldPassword
        {
            get { return _oldPassword; }
            set { SetProperty(ref this._oldPassword, value); }
        }

        private string _newPassword;
        [Required]
        [MinLength(6)]
        public string NewPassword
        {
            get { return _newPassword; }
            set { SetProperty(ref this._newPassword, value); }
        }

        private string _newPassword2;
        [Required]
        [MinLength(6)]
        [Compare("NewPassword")]
        public string NewPassword2
        {
            get { return _newPassword2; }
            set { SetProperty(ref this._newPassword2, value); }
        }

        public void Reset()
        {
            OldPassword = null;
            NewPassword = null;
            NewPassword2 = null;
        }
    }
}
