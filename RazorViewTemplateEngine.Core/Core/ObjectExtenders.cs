using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RazorViewTemplateEngine.Core {
    public static class ObjectExtenders
    {
        public static bool IsAnonymous(this object obj)
        {
            Type type = obj.GetType();

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }
    }
}