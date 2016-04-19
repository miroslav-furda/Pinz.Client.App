using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Com.Pinz.Client.Commons.Prism
{
    public class BindableValidationBase : BindableBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);
            if (result && !string.IsNullOrEmpty(propertyName))
            {
                ValidateModelProperty(value, propertyName);
            }
            return result;
        }


        protected void ValidateModelProperty(object value, string propertyName)
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(this, null, null) { MemberName = propertyName };
            if (!Validator.TryValidateProperty(value, validationContext, validationResults))
            {
                _validationErrors.Add(propertyName, new List<string>());
                foreach (ValidationResult validationResult in validationResults)
                {
                    _validationErrors[propertyName].Add(validationResult.ErrorMessage);
                }
            }
            RaiseErrorsChanged(propertyName);
        }


        public bool ValidateModel()
        {
            foreach(string propertyName in _validationErrors.Keys)
            {
                RaiseErrorsChanged(propertyName);
            }
            _validationErrors.Clear();

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(this, null, null);
            if (!Validator.TryValidateObject(this, validationContext, validationResults, true))
            {
                foreach (ValidationResult validationResult in validationResults)
                {
                    IEnumerator<string> enumerator = validationResult.MemberNames.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        string propertyName = enumerator.Current;
                        if (_validationErrors.ContainsKey(propertyName))
                        {
                            _validationErrors[propertyName].Add(validationResult.ErrorMessage);
                        }
                        else
                        {
                            _validationErrors.Add(propertyName, new List<string> { validationResult.ErrorMessage });
                        }
                        RaiseErrorsChanged(propertyName);
                    }
                }
            }
            return _validationErrors.Count > 0;
        }


        #region INotifyDataErrorInfo members
        public bool HasErrors
        {
            get
            {
                return _validationErrors.Count > 0;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_validationErrors.ContainsKey(propertyName))
            {
                return null;
            }
            else
            {
                return _validationErrors[propertyName];
            }
        }
        #endregion
    }
}
