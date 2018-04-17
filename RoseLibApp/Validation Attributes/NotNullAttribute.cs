using System;
using System.Collections.Generic;
using System.Text;

namespace RoseLibApp.RoseLib.Validation_Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    class NotNullAttribute : ArgumentValidationAttribute
    {
        public override void Validate(object value, string argumentName)
        {
            if(value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
