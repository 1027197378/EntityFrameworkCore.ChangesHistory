namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// 配置历史记录
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class HistoryAttribute : Attribute
    {
        /// <summary>
        /// 启用记录
        /// </summary>
        /// <param name="source">记录来源（默认为表名）</param>
        /// <param name="recordStatus">需要记录的操作类型(Modified|Added|Deleted)</param>
        public HistoryAttribute(string source = "", EntityState recordStatus = EntityState.Modified)
        {
            Source = source;
            RecordStatus = recordStatus;
        }

        public string Source { get; set; }

        public EntityState RecordStatus { get; set; }

    }
}
