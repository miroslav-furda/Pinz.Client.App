using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.DomainModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.DomainModel
{
    public class Company : BindableValidationBase, ICompany
    {
        public Guid CompanyId { get; set; }

        private string _name;
        [Required]//(ErrorMessageResourceName = "Company_Name_Required", ErrorMessageResourceType = typeof(Resources))]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }

        public override string ToString()
        {
            return string.Format("Company[CompanyId:{0}, Name:{1}", CompanyId, Name);
        }
    }
}
