using System;
using System.Diagnostics;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Parser;
using ICSStudio.Utils.TagExpression;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.EditorPackage.Reference
{
    public partial class CrossReferenceViewModel
    {
        private void SearchAndAddLogicItem(RLLRoutine rll, Tag tag, string searchName)
        {
            var parserService =
                Package.GetGlobalService(typeof(SParserService)) as IParserService;

            var parseInformation = parserService?.GetCachedParseInformation(rll);

            if (parseInformation == null)
                return;

            var searchExpression = GetSearchExpression(searchName);
            if (searchExpression == null)
                return;

            foreach (var parameter in parseInformation.Parameters)
            {
                if (parameter.Tag == tag && MatchExpression(searchExpression, parameter.TagExpression))
                {
                    //TODO(gjc): Destructive edit later
                    AddLogicItem(parameter.Row - 1, parameter.Col, parameter.Parent.Name,
                        rll.ParentCollection.ParentProgram.Name, rll.Name,
                        false, RoutineType.RLL,
                        OnlineEditType.Original,
                        parameter.Name, "");
                }
            }

        }

        private void SearchAndAddRoutineReference(RLLRoutine rll, string searchName)
        {
            var parserService =
                Package.GetGlobalService(typeof(SParserService)) as IParserService;

            var parseInformation = parserService?.GetCachedParseInformation(rll);

            if (parseInformation == null)
                return;

            foreach (var instruction in parseInformation.Instructions)
            {
                if (!instruction.Name.Equals("JSR", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (instruction.Parameters?[0]?.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase) != true)
                    return;

                AddLogicItem(SimpleLogicItems, instruction.Row - 1, instruction.Col, "JSR", rll.ParentCollection.ParentProgram.Name, rll.Name, false, RoutineType.RLL, OnlineEditType.Original, instruction.Name, "");
            }
        }

        private void SearchAndAddLabelReference(RLLRoutine rll, string searchName)
        {
            var parserService =
                Package.GetGlobalService(typeof(SParserService)) as IParserService;

            var parseInformation = parserService?.GetCachedParseInformation(rll);

            if (parseInformation == null)
                return;

            foreach (var instruction in parseInformation.Instructions)
            {
                if (!instruction.Name.Equals("LBL", StringComparison.OrdinalIgnoreCase) && !instruction.Name.Equals("JMP", StringComparison.OrdinalIgnoreCase))
                    continue;

                var label = instruction.Parameters?[0]?.Name;
                if(!Name.Equals(label, StringComparison.OrdinalIgnoreCase))
                    continue;

                AddLogicItem(SimpleLogicItems, instruction.Row - 1, instruction.Col, instruction.Name, rll.ParentCollection.ParentProgram.Name, rll.Name, false, RoutineType.RLL, OnlineEditType.Original, instruction.Name, "");
            }
        }

        private bool MatchExpression(TagExpressionBase searchExpression, TagExpressionBase tagExpression)
        {
            if (searchExpression == null)
                return false;

            if (tagExpression == null)
                return false;

            TagExpressionParser parser = new TagExpressionParser();

            TagExpressionBase searchNode = parser.GetSimpleTagExpression(searchExpression);
            TagExpressionBase node = parser.GetSimpleTagExpression(tagExpression);

            while (true)
            {
                searchNode = searchNode.Next;
                node = node.Next;

                if (searchNode == null)
                    return true;

                if (node == null)
                    return false;

                var searchMemberAccessExpression = searchNode as MemberAccessExpression;
                var nodeMemberAccessExpression = node as MemberAccessExpression;
                if (searchMemberAccessExpression != null && nodeMemberAccessExpression != null)
                {
                    if (string.Equals(searchMemberAccessExpression.Name, nodeMemberAccessExpression.Name,
                            StringComparison.OrdinalIgnoreCase))
                        continue;

                    return false;
                }

                var searchElementAccessExpression = searchNode as ElementAccessExpression;
                var nodeElementAccessExpression = node as ElementAccessExpression;
                if (searchElementAccessExpression != null && nodeElementAccessExpression != null)
                {
                    if (searchElementAccessExpression.Indexes != null &&
                        searchElementAccessExpression.Indexes.Count > 0)
                    {
                        if (nodeElementAccessExpression.Indexes != null &&
                            nodeElementAccessExpression.Indexes.Count > 0)
                        {
                            if (searchElementAccessExpression.Indexes.SequenceEqual(nodeElementAccessExpression
                                    .Indexes))
                                continue;

                            return false;
                        }

                        if (nodeElementAccessExpression.ExpressionIndexes != null &&
                            nodeElementAccessExpression.ExpressionIndexes.Count > 0)
                        {
                            continue;
                        }

                        return false;
                    }

                    Debug.Assert(false);
                    return false;

                }

                var searchBitMemberNumberAccessExpression = searchNode as BitMemberNumberAccessExpression;
                if (searchBitMemberNumberAccessExpression != null)
                {
                    var nodeBitMemberNumberAccessExpression = node as BitMemberNumberAccessExpression;
                    if (nodeBitMemberNumberAccessExpression != null)
                    {
                        if (searchBitMemberNumberAccessExpression.Number == nodeBitMemberNumberAccessExpression.Number)
                            return true;

                        return false;
                    }

                    var nodeBitMemberExpressionAccessExpression = node as BitMemberExpressionAccessExpression;
                    if (nodeBitMemberExpressionAccessExpression != null)
                        return true;
                }


                return false;
            }

        }

        private TagExpressionBase GetSearchExpression(string searchName)
        {
            try
            {
                TagExpressionParser parser = new TagExpressionParser();
                return parser.Parser(searchName);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }
    }
}
