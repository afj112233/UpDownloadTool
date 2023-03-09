using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSStudio.AvalonEdit;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Rendering;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Pen = System.Windows.Media.Pen;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.StxEditor.ViewModel.Highlighting
{
    public class TextMarkerService : IBackgroundRenderer, IVisualLineTransformer, INotifyCollectionChanged
    {
        // private readonly TextEditor _textEditor;
        private readonly TextSegmentCollection<TextMarker> _markers;
        public object LockObject = new object();

        public sealed class TextMarker : TextSegment
        {
            public TextMarker(int startOffset, int length)
            {
                StartOffset = startOffset;
                if (length > -1)
                    Length = length;
            }

            public Color? BackgroundColor { get; set; }
            public Color MarkerColor { get; set; }
            public string ToolTip { get; set; }
        }

        public TextMarkerService(TextDocument document)
        {
            TextDocument = document;
            _markers = new TextSegmentCollection<TextMarker>(document);
        }

        public TextMarkerService(TextDocument document, STRoutine routine)
        {
            TextDocument = document;
            _markers = new TextSegmentCollection<TextMarker>(document);
            _stRoutine = routine;
        }

        private STRoutine _stRoutine;

        public TextDocument TextDocument { get; }

        public bool OnlyAddError { set; get; } = false;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_markers == null || !textView.VisualLinesValid)
            {
                return;
            }

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
            {
                return;
            }

            int viewStart = visualLines.First().FirstDocumentLine.Offset;
            int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
            foreach (TextMarker marker in _markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
            {
                if (marker.BackgroundColor != null)
                {
                    var geoBuilder = new BackgroundGeometryBuilder {AlignToWholePixels = true, CornerRadius = 3};
                    geoBuilder.AddSegment(textView, marker);
                    Geometry geometry = geoBuilder.CreateGeometry();
                    if (geometry != null)
                    {
                        Color color = marker.BackgroundColor.Value;
                        var brush = new SolidColorBrush(color);
                        brush.Freeze();
                        drawingContext.DrawGeometry(brush, null, geometry);
                    }
                }

                foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
                {
                    Point startPoint = r.BottomLeft;
                    Point endPoint = r.BottomRight;

                    var usedPen = new Pen(new SolidColorBrush(marker.MarkerColor), 1);
                    usedPen.Freeze();
                    const double offset = 2.5;

                    int count = Math.Max((int) ((endPoint.X - startPoint.X) / offset) + 1, 4);

                    var geometry = new StreamGeometry();

                    using (StreamGeometryContext ctx = geometry.Open())
                    {
                        ctx.BeginFigure(startPoint, false, false);
                        ctx.PolyLineTo(CreatePoints(startPoint, endPoint, offset, count).ToArray(), true, false);
                    }

                    geometry.Freeze();

                    drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
                    break;
                }
            }
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Caret; }
        }

        public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
        {
        }

        public void Clear()
        {
            _markers.Clear();
            ErrorCount = 0;
            var delMarkers = _markers.ToArray();
            _markers.Clear();
            if (delMarkers.Length > 0)
                CollectionChanged?.Invoke(_markers,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, delMarkers));
        }

        public void Create(int offset, int length, string codeSnippet, Color color, List<Tuple<int, int>> commentList)
        {
            if (length > 0)
            {
                var offsets = SplitTextMarketByLine(offset, length, codeSnippet);
                for (int i = 0; i < offsets.Count; i++)
                {
                    if (offsets.Count == 1)
                    {
                        var m = new TextMarker(offsets[0], 1);
                        _markers.Add(m);
                        m.MarkerColor = color;
                        //m.ToolTip = message;

                        CollectionChanged?.Invoke(_markers,
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                                new List<TextMarker> {m}));
                        break;
                    }
                    else
                    {
                        if (i == offsets.Count - 1) break;
                        int currentOffset = offsets[i];
                        int sLength = offsets[i + 1] - offsets[i] + 1;
                        if (offset >= codeSnippet.Length - 1 && sLength == 0) continue;
                        var m = new TextMarker(currentOffset, sLength);
                        _markers.Add(m);
                        m.MarkerColor = color;
                        //m.ToolTip = message;

                        CollectionChanged?.Invoke(_markers,
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                                new List<TextMarker> {m}));
                    }

                }
            }
        }

        public int ErrorCount = 0;
        public bool IsAddTextMarker { set; get; } = true;
        public void CreateSkinCommentAndBlankLine(int offset, int length, string codeSnippet, Color color,
            List<Tuple<int, int>> commentList, bool canAddError, string toolTip = "")
        {
            if (offset < 0 || length < 0) return;
            if (offset + length > codeSnippet.Length)
            {
                length = codeSnippet.Length - offset;
            }

            if(IsAddTextMarker)
            {
                var astStmtMod = _stRoutine.GetCurrentMod(_stRoutine.CurrentOnlineEditType);
                if (astStmtMod != null)
                {
                    astStmtMod.TextMarkers.Add(new Tuple<string, int, int, Color>(toolTip, offset, length, color));
                }
            }
            Debug.Assert(!string.IsNullOrEmpty(toolTip));
            if (canAddError)
            {
                var location = TextDocument.GetLocationNotVerifyAccess(offset);
                var errorWindow = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
                Debug.Assert(errorWindow != null);
                ErrorCount++;
                Console.WriteLine($@"Error:Line {location.Line},{(string.IsNullOrEmpty(toolTip) ? "need more error info" : toolTip)}");
                errorWindow?.AddErrors(
                    $"Error:Line {location.Line},{(string.IsNullOrEmpty(toolTip) ? "need more error info" : toolTip)}",
                    OrderType.Order, _stRoutine.CurrentOnlineEditType, location.Line - 1,
                    location.Column - 1,
                    _stRoutine);
            }

            if (OnlyAddError) return;
            var range = Skin(offset, length, commentList, codeSnippet);
            foreach (var tuple in range)
            {
                if (tuple.Item2 - tuple.Item1 <= 0)
                    continue;
                if (IsRedundant(tuple.Item1, tuple.Item2))
                    continue;
                var m = new TextMarker(tuple.Item1, tuple.Item2 - tuple.Item1);
                //m.ToolTip = toolTip;
                _markers.Add(m);
                m.MarkerColor = color;
                CollectionChanged?.Invoke(_markers,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<TextMarker> {m}));
            }
        }

        private bool IsRedundant(int offset, int end)
        {
            if (_markers.FirstOrDefault(m => m.StartOffset == offset && m.EndOffset == end) != null) return true;
            return false;
        }

        public void Redraw(TextEditor editor, ISegment segment)
        {
            editor.TextArea.TextView.Redraw(segment);
        }

        public IEnumerable<TextMarker> GetMarkersAtOffset(int offset)
        {
            return _markers == null ? Enumerable.Empty<TextMarker>() : _markers.FindSegmentsContaining(offset);
        }
        
        public void RemoveLineMarker(DocumentLine line)
        {
            var markers = GetMarkersAtLine(line);
            foreach (var m in markers)
            {
                Remove(m);
            }
        }

        public void RemoveMarketInComment(string code)
        {
            int p = code.IndexOf("(*");
            if (p != -1)
            {
                while (p != -1)
                {
                    int p2 = code.IndexOf("*)", p) + 1;
                    if (p2 > p)
                    {
                        p2 = code.Length - 1;
                        RemoveRangeMarket(p, p2);
                    }
                    else
                    {
                        RemoveRangeMarket(p, code.Length - 1);
                        break;
                    }

                    p = code.IndexOf("(*", p2 + 1);
                }

            }

            p = code.IndexOf("//");
            if (p != -1)
            {
                while (p != -1)
                {
                    int p2 = code.IndexOf("\n", p);
                    if (p2 > p)
                    {
                        RemoveRangeMarket(p, p2);
                    }
                    else
                    {
                        break;
                    }

                    p = code.IndexOf("//", p2 + 1);
                }

            }

        }

        public void RemoveRangeMarket(int offset, int endOffset)
        {
            offset = offset + 1;
            if (offset >= endOffset) return;
            var markets = _markers.FindOverlappingSegments(offset, endOffset - offset - 1);
            foreach (var textMarker in markets)
            {
                Remove(textMarker);
            }
        }

        public int GetMarkerCount()
        {
            return _markers.Count;
        }

        public IEnumerable<TextMarker> GetMarkersAtRange(int offset, int end)
        {
            if (offset == -1 || end < 1) return null;
            return _markers.FindOverlappingSegments(offset, end - offset + 1);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Remove(TextMarker marker)
        {
            if (_markers.Remove(marker))
            {
                CollectionChanged?.Invoke(_markers,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                        new List<TextMarker> {marker}));
            }
        }
        
        private IEnumerable<Point> CreatePoints(Point start, Point end, double offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new Point(start.X + (i * offset), start.Y - ((i + 1) % 2 == 0 ? offset : 0));
            }
        }

        private IEnumerable<TextMarker> GetMarkersAtLine(DocumentLine line)
        {
            return _markers == null
                ? Enumerable.Empty<TextMarker>()
                : _markers.FindOverlappingSegments(line.Offset, line.EndOffset - line.Offset);
        }

        private List<int> SplitTextMarketByLine(int offset, int length, string codeSnippet)
        {
            List<int> offsetList = new List<int>();
            offsetList.Add(offset);
            int f = codeSnippet.IndexOf("\n", offset);
            while (f > -1 && f < offset + length - 1)
            {
                offsetList.Add(f + 1);
                f = codeSnippet.IndexOf("\n", f + 1);
            }

            if (!offsetList.Contains(offset + length - 1))
                offsetList.Add(offset + length - 1);
            return offsetList;
        }

        private List<Tuple<int, int>> Skin(int offset, int length, List<Tuple<int, int>> commentList,
            string codeSnippet)
        {
            List<Tuple<int, int>> offsetList = new List<Tuple<int, int>>();
            var rangComment = commentList.Where(c => c.Item1 >= offset && c.Item2 <= offset + length).ToList();
            Regex blankLineRegex = new Regex(@"((\n|\r\n))[\t ]*|^[\t ]+\n|[\t ]+$");
            var snippet = codeSnippet.Substring(offset, length);
            var matches = blankLineRegex.Matches(snippet);
            if (rangComment.Count > 0)
            {
                int start = offset;
                foreach (var tuple in rangComment)
                {
                    if (tuple.Item1 > 0)
                        offsetList.Add(new Tuple<int, int>(start, tuple.Item1 - 1));
                    start = tuple.Item2 + 1;
                }

                offsetList.Add(new Tuple<int, int>(start, offset + length));
            }
            else
            {
                offsetList.Add(new Tuple<int, int>(offset, offset + length));
            }

            if (matches.Count > 0)
            {
                int i = 0;
                var final = new List<Tuple<int, int>>();
                for (int j = 0; j < offsetList.Count; j++)
                {
                    var tuple = offsetList[j];
                    if (i < matches.Count)
                    {

                        if (matches[i].Index + matches[i].Length + offset <= tuple.Item1)
                        {
                            while (i < matches.Count && matches[i].Index + matches[i].Length + offset <= tuple.Item1)
                            {
                                i++;
                            }

                            if (i >= matches.Count)
                            {
                                final.Add(tuple);
                                continue;
                            }
                        }

                        if (tuple.Item1 >= matches[i].Index + matches[i].Length + offset)
                        {
                            i++;
                            final.Add(tuple);
                            continue;
                        }

                        if (tuple.Item1 <= matches[i].Index + offset &&
                            tuple.Item2 >= matches[i].Index + matches[i].Length + offset)
                        {
                            int d = 0;
                            if (matches[i].Value == "\r\n" || matches[i].Value == "\n") d = -1;
                            final.Add(new Tuple<int, int>(tuple.Item1, matches[i].Index + offset + d));
                            //final.Add(new Tuple<int, int>(matches[i].Index + offset + matches[i].Length, tuple.Item2));
                            offsetList[j] = new Tuple<int, int>(matches[i].Index + offset + matches[i].Length,
                                tuple.Item2);
                            j--;
                        }
                        else
                        {
                            if (tuple.Item1 > matches[i].Index + offset &&
                                tuple.Item2 >= matches[i].Index + matches[i].Length + offset)
                            {
                                //final.Add(new Tuple<int, int>(matches[i].Index + matches[i].Length, tuple.Item2));
                                offsetList[j] = new Tuple<int, int>(matches[i].Index + matches[i].Length,
                                    tuple.Item2);
                                j--;
                            }
                            else if (tuple.Item1 > matches[i].Index + offset &&
                                     tuple.Item2 < matches[i].Index + matches[i].Length + offset)
                            {
                                final.Add(tuple);
                                continue;
                            }
                            else if (tuple.Item1 <= matches[i].Index + offset &&
                                     tuple.Item2 < matches[i].Index + matches[i].Length + offset)
                            {
                                if (tuple.Item2 <= matches[i].Index + offset)
                                {
                                    final.Add(tuple);
                                }
                                else
                                {
                                    int d = 0;
                                    if (matches[i].Value == "\r\n" || matches[i].Value == "\n") d = -1;
                                    final.Add(new Tuple<int, int>(tuple.Item1, matches[i].Index + offset + d));
                                }
                            }
                            else
                            {
                                Debug.Assert(false);
                            }
                        }

                    }
                    else
                    {
                        final.Add(tuple);
                    }
                }

                Debug.Assert(i >= matches.Count - 1);
                return final;
            }
            else
            {
                return offsetList;
            }
        }
    }
}
