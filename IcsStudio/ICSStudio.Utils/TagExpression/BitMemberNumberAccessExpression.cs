namespace ICSStudio.Utils.TagExpression
{
    public class BitMemberNumberAccessExpression : TagMemberAccessExpressionBase
    {
        public int Number { get; set; }

        public override string ToString()
        {
            return $"{Target}.{Number}";
        }
    }
}
