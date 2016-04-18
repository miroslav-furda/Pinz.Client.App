using System;
using Com.Pinz.Client.DomainModel;
using Prism.Commands;
using Prism.Mvvm;
using Com.Pinz.Client.Model.Service;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class UserSelfAdministrationModel : BindableBase
    {
        public User CurrentUser { get; private set; }
        private User BackupUser { get; set; }

        public PasswordChangeViewModel PasswordChangeModel { get; private set; }

        private bool _isUserInEditMode;
        public bool IsUserInEditMode
        {
            get
            {
                return _isUserInEditMode;
            }
            private set
            {
                SetProperty(ref this._isUserInEditMode, value);
            }
        }
        public DelegateCommand StartUserChangesCommand { get; private set; }
        public DelegateCommand SaveUserChangesCommand { get; private set; }
        public DelegateCommand CancelUserChangesCommand { get; private set; }

        private bool _isPasswordInEditMode;
        public bool IsPasswordInEditMode
        {
            get
            {
                return _isPasswordInEditMode;
            }
            private set
            {
                SetProperty(ref this._isPasswordInEditMode, value);
            }
        }
        public DelegateCommand StartPasswordChangeCommand { get; private set; }
        public DelegateCommand ChangeUserPasswordCommand { get; private set; }
        public DelegateCommand CancelPasswordChangeCommand { get; private set; }

        private IAdminClientService adminService;

        public UserSelfAdministrationModel(IAdminClientService adminService)
        {
            this.adminService = adminService;

            CurrentUser = adminService.CurrentUser;

            IsUserInEditMode = false;
            IsPasswordInEditMode = false;
            PasswordChangeModel = new PasswordChangeViewModel();

            StartUserChangesCommand = new DelegateCommand(StartUserChanges);
            SaveUserChangesCommand = new DelegateCommand(SaveUserChanges);
            CancelUserChangesCommand = new DelegateCommand(CancelUserChanges);

            StartPasswordChangeCommand = new DelegateCommand(StartPasswordChange);
            ChangeUserPasswordCommand = new DelegateCommand(ChangeUserPassword);
            CancelPasswordChangeCommand = new DelegateCommand(CancelPasswordChange);

        }

        private void CancelPasswordChange()
        {
            IsPasswordInEditMode = false;
        }

        private void ChangeUserPassword()
        {
            if (!PasswordChangeModel.HasErrors)
            {
                adminService.ChangePasswordForUser(CurrentUser, PasswordChangeModel.OldPasword, PasswordChangeModel.NewPasword, PasswordChangeModel.NewPasword2);
                IsPasswordInEditMode = false;
            }
        }

        private void StartPasswordChange()
        {
            PasswordChangeModel.Reset();
            IsPasswordInEditMode = true;
        }

        private void CancelUserChanges()
        {
            IsUserInEditMode = false;
        }

        private void SaveUserChanges()
        {
            IsUserInEditMode = false;
            throw new NotImplementedException();
        }

        private void StartUserChanges()
        {
            IsUserInEditMode = true;
        }
    }
}
