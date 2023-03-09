namespace ICSStudio.Utils.TagExpression
{
    public class MemberAccessExpression : TagMemberAccessExpressionBase
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Target}.{Name}";
        }
    }
}
