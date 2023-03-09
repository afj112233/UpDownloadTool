using System.Collections.Generic;

namespace ICSStudio.Utils.TagExpression
{
    public class ElementAccessExpression : TagExpressionBase
    {
        public TagExpressionBase Target { get; set; }

        public List<int> Indexes { get; set; }

        public List<SimpleTagExpression> ExpressionIndexes { get; set; }

        public override string ToString()
        {
            if (Indexes != null && Indexes.Count > 0)
                return $"{Target}[{string.Join(",", Indexes)}]";

            if (ExpressionIndexes != null && ExpressionIndexes.Count > 0)
                return $"{Target}[{string.Join(",", ExpressionIndexes)}]";

            return string.Empty;
        }
    }
}
