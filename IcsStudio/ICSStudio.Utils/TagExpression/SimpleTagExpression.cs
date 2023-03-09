namespace ICSStudio.Utils.TagExpression
{
    public class SimpleTagExpression : TagExpressionBase
    {
        public string Scope { get; set; }
        public string TagName { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Scope))
                return TagName;

            return $"\\{Scope}.{TagName}";
        }
    }

}
