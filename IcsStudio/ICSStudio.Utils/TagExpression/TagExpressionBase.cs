namespace ICSStudio.Utils.TagExpression
{
    public abstract class TagExpressionBase
    {
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }

        public TagExpressionBase Next { get; set; }

    }
}
