using HaweeApp.MVVM.Annotation;
using System.Collections.Generic;

namespace HaweeApp.MVVM
{
    public class ModelValidator
    {
        public static List<string> ValidateProperty(object instance, string propertyName)
        {
            var errors = new List<string>();
            var prop = instance.GetType().GetProperty(propertyName);
            if (prop is null) return errors;

            var value = prop.GetValue(instance);
            foreach (var attr in prop.GetCustomAttributes(true))
            {
                if (attr is RequiredAttribute notEmpty)
                {
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                        errors.Add(notEmpty.Message);
                }
            }

            return errors;
        }

        public static Dictionary<string, List<string>> ValidateObject(object instance)
        {
            var result = new Dictionary<string, List<string>>();
            var props = instance.GetType().GetProperties();

            foreach (var prop in props)
            {
                var errors = ValidateProperty(instance, prop.Name);
                if (errors.Count > 0)
                    result[prop.Name] = errors;
            }

            return result;
        }
    }
}
