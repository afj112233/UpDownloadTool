namespace ICSStudio.Utils.TagExpression
{
    public class BitMemberExpressionAccessExpression : TagMemberAccessExpressionBase
    {
        public int? Number { get; set; }

        public SimpleTagExpression ExpressionNumber { get; set; }

        public override string ToString()
        {
            if (Number.HasValue)
                return $"{Target}.[{Number}]";

            if (ExpressionNumber != null)
                return $"{Target}.[{ExpressionNumber}]";

            return string.Empty;
        }
    }
}
