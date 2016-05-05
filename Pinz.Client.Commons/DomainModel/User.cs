using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.DomainModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.DomainModel
{
    public class User : BindableValidationBase, IUser
    {
        public Guid UserId { get; set; }

        private string _eMail;
        [Required]//(ErrorMessageResourceName = "User_EMail_Required", ErrorMessageResourceType = typeof(Resources))]
        public string EMail
        {
            get { return _eMail; }
            set { SetProperty(ref this._eMail, value); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref this._firstName, value); }
        }

        private string _familyName;
        public string FamilyName
        {
            get { return _familyName; }
            set { SetProperty(ref this._familyName, value); }
        }

        private bool _isCompanyAdmin;
        public bool IsCompanyAdmin
        {
            get { return _isCompanyAdmin; }
            set { SetProperty(ref this._isCompanyAdmin, value); }
        }

        private Guid _companyId;
        [Required]//(ErrorMessageResourceName = "User_Company_Required", ErrorMessageResourceType = typeof(Resources))]
        public Guid CompanyId
        {
            get { return _companyId; }
            set { SetProperty(ref this._companyId, value); }
        }

        public override string ToString()
        {
            return string.Format("User[UserId:{0}, FirstName:{1}, FamilyName:{2}, EMail:{3}, CompanyId:{4}",
                UserId, FirstName, FamilyName, EMail, CompanyId);
        }
    }
}
