using System;
using System.ComponentModel;

namespace AggieGlobal.WebApi.Common
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class TenantApplicationAttribute : Attribute
    {
        public string ThemeFolderName { get; set; }
        public string TenantApplicationName { get; set; }
        public string AppCode { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class TenantAppAccountPackageAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class FieldDataTypeAttribute : DescriptionAttribute
    {
        public FieldDataTypeAttribute(string description) : base(description) { }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class FilePropertyAttribute : DescriptionAttribute
    {
        public FilePropertyAttribute(string description) : base(description) { }
        public string DisplayName { get; set; }
        public bool Hide { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class SearchConditionAttribute : Attribute
    {
        public string DisplayText { get; set; }
    }
}
