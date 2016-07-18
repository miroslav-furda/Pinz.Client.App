using Com.Pinz.Client.DomainModel;
using Prism.Commands;
using Prism.Mvvm;
using AutoMapper;
using Ninject;
using Prism.Interactivity.InteractionRequest;
using Com.Pinz.Client.Commons.Wpf.Extensions;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.Model;
using Com.Pinz.Client.Commons.Prism;
using Prism.Events;
using System;
using Com.Pinz.Client.Commons.Event;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class UserSelfAdministrationModel : BindableBase
    {
        public TabModel TabModel { get; private set; }

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
        public AwaitableDelegateCommand SaveUserChangesCommand { get; private set; }
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
        public AwaitableDelegateCommand ChangeUserPasswordCommand { get; private set; }
        public DelegateCommand CancelPasswordChangeCommand { get; private set; }

        private readonly IAdministrationRemoteService _adminService;
        private readonly IMapper _mapper;
        private readonly UserNameClientCredentials _userCredentials;

        public InteractionRequest<INotification> ChangeNotification { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        [Inject]
        public UserSelfAdministrationModel(IAdministrationRemoteService adminService, ApplicationGlobalModel globalModel,
            UserNameClientCredentials userCredentials, [Named("WpfClientMapper")]  IMapper mapper, IEventAggregator eventAggregator)
        {
            this._adminService = adminService;
            this._mapper = mapper;
            this._userCredentials = userCredentials;
            this._eventAggregator = eventAggregator;

            TabModel = new TabModel()
            {
                Title = Properties.Resources.AdministrationTab_Title_User,
                CanClose = false,
                IsModified = false
            };

            CurrentUser = globalModel.CurrentUser;
            BackupUser = new User();

            IsUserInEditMode = false;
            IsPasswordInEditMode = false;
            PasswordChangeModel = new PasswordChangeViewModel();

            StartUserChangesCommand = new DelegateCommand(StartUserChanges);
            SaveUserChangesCommand = new AwaitableDelegateCommand(SaveUserChanges);
            CancelUserChangesCommand = new DelegateCommand(CancelUserChanges);

            StartPasswordChangeCommand = new DelegateCommand(StartPasswordChange);
            ChangeUserPasswordCommand = new AwaitableDelegateCommand(ChangeUserPassword);
            CancelPasswordChangeCommand = new DelegateCommand(CancelPasswordChange);

            ChangeNotification = new InteractionRequest<INotification>();

        }

        private void CancelPasswordChange()
        {
            IsPasswordInEditMode = false;
        }

        private async System.Threading.Tasks.Task ChangeUserPassword()
        {
            if (PasswordChangeModel.ValidateModel())
            {
                try
                {
                    bool success = await _adminService.ChangeUserPasswordAsync(CurrentUser, PasswordChangeModel.OldPassword, PasswordChangeModel.NewPassword, PasswordChangeModel.NewPassword2);
                    if (success)
                    {
                        ChangeNotification.Raise(new Notification()
                        {
                            Title = Properties.Resources.PasswordChange_Title,
                            Content = Properties.Resources.PasswordChange_Success
                        });
                        IsPasswordInEditMode = false;
                        _userCredentials.Password = PasswordChangeModel.NewPassword;
                        _userCredentials.UpdateCredentialsForAllFactories();
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
                catch (TimeoutException timeoutEx)
                {
                    _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
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
            _mapper.Map(BackupUser, CurrentUser);
            IsUserInEditMode = false;
        }

        private async System.Threading.Tasks.Task SaveUserChanges()
        {
            try
            {
                await _adminService.UpdateUserAsync(CurrentUser);
                IsUserInEditMode = false;
            }
            catch (TimeoutException timeoutEx)
            {
                _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
            }
        }

        private void StartUserChanges()
        {
            if (IsPasswordInEditMode)
                CancelPasswordChange();
            _mapper.Map(CurrentUser, BackupUser);
            IsUserInEditMode = true;
        }
    }
}
