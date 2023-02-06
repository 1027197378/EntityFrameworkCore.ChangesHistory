namespace EntityFrameworkCore.ChangesHistory.Entity
{
    public class Difference
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 自定义名
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// BeforeValue
        /// </summary>
        public object BeforeValue { get; set; }

        /// <summary>
        /// AfterValue
        /// </summary>
        public object AfterValue { get; set; }
    }
}
