using System;
using Com.Pinz.Client.DomainModel;
using Prism.Commands;
using Prism.Mvvm;
using Com.Pinz.Client.Model.Service;
using AutoMapper;
using Ninject;
using Prism.Interactivity.InteractionRequest;

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
        private IMapper mapper;

        public InteractionRequest<INotification> ChangeNotification { get; private set; }

        [Inject]
        public UserSelfAdministrationModel(IAdminClientService adminService, [Named("WpfClientMapper") ]  IMapper mapper)
        {
            this.adminService = adminService;
            this.mapper = mapper;

            CurrentUser = adminService.CurrentUser;
            BackupUser = new User();

            IsUserInEditMode = false;
            IsPasswordInEditMode = false;
            PasswordChangeModel = new PasswordChangeViewModel();

            StartUserChangesCommand = new DelegateCommand(StartUserChanges);
            SaveUserChangesCommand = new DelegateCommand(SaveUserChanges);
            CancelUserChangesCommand = new DelegateCommand(CancelUserChanges);

            StartPasswordChangeCommand = new DelegateCommand(StartPasswordChange);
            ChangeUserPasswordCommand = new DelegateCommand(ChangeUserPassword);
            CancelPasswordChangeCommand = new DelegateCommand(CancelPasswordChange);

            ChangeNotification = new InteractionRequest<INotification>();

        }

        private void CancelPasswordChange()
        {
            IsPasswordInEditMode = false;
        }

        private void ChangeUserPassword()
        {
            if (!PasswordChangeModel.ValidateModel())
            {
                if( adminService.ChangePasswordForUser(CurrentUser, PasswordChangeModel.OldPassword, PasswordChangeModel.NewPassword, PasswordChangeModel.NewPassword2))
                {
                    ChangeNotification.Raise(new Notification()
                    {
                        Title = Properties.Resources.PasswordChange_Title,
                        Content = Properties.Resources.PasswordChange_Success
                    });
                    IsPasswordInEditMode = false;
                }
                else
                {
                    ChangeNotification.Raise(new Notification()
                    {
                        Title = Properties.Resources.PasswordChange_Title,
                        Content = Properties.Resources.PasswordChange_Failed
                    });
                }
            }
        }

        private void StartPasswordChange()
        {
            if (IsUserInEditMode)
                CancelUserChanges();
            PasswordChangeModel.Reset();
            IsPasswordInEditMode = true;
        }

        private void CancelUserChanges()
        {
            mapper.Map(BackupUser, CurrentUser);
            IsUserInEditMode = false;
        }

        private void SaveUserChanges()
        {
            IsUserInEditMode = false;
            adminService.UpdateUser(CurrentUser);
        }

        private void StartUserChanges()
        {
            if (IsPasswordInEditMode)
                CancelPasswordChange();
            mapper.Map(CurrentUser,BackupUser);
            IsUserInEditMode = true;
        }
    }
}
