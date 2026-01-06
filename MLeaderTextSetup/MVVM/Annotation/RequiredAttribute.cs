using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaweeApp.MVVM.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {
        //Display message if the field is empty
        public string Message;
        public RequiredAttribute(string message) { Message = message; }
    }
}
