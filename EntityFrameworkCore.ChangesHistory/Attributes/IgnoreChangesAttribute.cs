namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// 忽略字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreChangesAttribute : Attribute
    {

    }
}