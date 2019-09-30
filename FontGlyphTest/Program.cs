using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;


namespace FontGlyphTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> charLines = new List<string>();
            DirectoryInfo d = new DirectoryInfo(@"D:\MachineLearning\Document Layout Analysis\Fonts");
            List<FileInfo> Files = d.GetFiles("*.ttc").ToList(); // ttf, fon, ttc // 'fon' doesn't work
            Files.AddRange(d.GetFiles("*.ttf"));

            foreach (var fontFile in Files)
            {
                Uri uri = new Uri(fontFile.FullName);
                GlyphTypeface glyphTypeface = new GlyphTypeface(uri);

                foreach (var unicodePair in unicodeChars)
                {
                    //ScatterplotView view = new ScatterplotView();
                    //view.Dock = System.Windows.Forms.DockStyle.Fill;
                    //view.LinesVisible = true;

                    int indexUnicode = unicodePair.Key;
                    char unicode = unicodePair.Value;

                    if (!glyphTypeface.CharacterToGlyphMap.ContainsKey(indexUnicode)) continue;

                    var geometry = glyphTypeface.GetGlyphOutline(
                                    glyphTypeface.CharacterToGlyphMap[indexUnicode],
                                    100, 1);
                    var boundingBox = geometry.Bounds;

                    var stepX = boundingBox.Width / 8;
                    var stepY = boundingBox.Height / 8;
                    var blockArea = stepX * stepY;

                    List<int> data = new List<int>();

                    var path = geometry.GetFlattenedPathGeometry();

                    for (int j = 0; j < 8; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            RectangleGeometry block = new RectangleGeometry(
                                new System.Windows.Rect(
                                    new System.Windows.Point(boundingBox.X + i * stepX, boundingBox.Y + j * stepY),
                                    new System.Windows.Point(boundingBox.X + (i + 1) * stepX, boundingBox.Y + (j + 1) * stepY)));

                            PathGeometry intersectionGeometry = PathGeometry.Combine(block, path, GeometryCombineMode.Intersect, null);
                            var area = Math.Round(intersectionGeometry.GetArea() / blockArea * 16, 0);
                            data.Add((int)area);
                        }
                    }
                    var dataStr = string.Join(",", data);
                    dataStr += "," + indexUnicode.ToString();

                    charLines.Add(dataStr);

                    /*var figures = path.Figures.ToList();

                    var segments = figures.Select(f => f.Segments.ToList()).ToList();

                    foreach (var group in segments)
                    {
                        foreach (var segment in group)
                        {
                            if (segment is PolyLineSegment polyLineSegment)
                            {
                                var points = polyLineSegment.Points.ToList();
                                var x = points.Select(p => (p.X / (boundingBox.X + boundingBox.Width)) * 8.0).ToArray();
                                var y = points.Select(p => -(p.Y / (boundingBox.Height)) * 8.0).ToArray();

                                view.Graph.GraphPane.AddCurve(unicode.ToString(),
                                    x, y,
                                    System.Drawing.Color.Black);

                            }
                            else if (segment is ArcSegment arcSegment)
                            {
                                throw new NotImplementedException();
                            }
                            else if (segment is BezierSegment bezierSegment)
                            {
                                throw new NotImplementedException();
                            }
                            else if (segment is LineSegment lineSegment)
                            {
                                throw new NotImplementedException();
                            }
                            else if (segment is PolyBezierSegment polyBezierSegment)
                            {
                                throw new NotImplementedException();
                            }
                            else if (segment is PolyQuadraticBezierSegment polyQuadraticBezierSegment)
                            {
                                throw new NotImplementedException();
                            }
                            else if (segment is QuadraticBezierSegment quadraticBezierSegment)
                            {
                                throw new NotImplementedException();
                            }
                            else
                            {
                                throw new ArgumentException(segment.GetType().ToString());
                            }
                        }
                    }*/

                    //view.Graph.GraphPane.AxisChange();
                    //var f1 = new System.Windows.Forms.Form();
                    //f1.Width = 1000;
                    //f1.Height = 1000;
                    //f1.Controls.Add(view);
                    //f1.ShowDialog();


                }
            }

            File.WriteAllLines(@"D:\MachineLearning\Document Layout Analysis\Fonts\chars-train.csv", charLines);
        }

        static Dictionary<int, char> unicodeChars = new Dictionary<int, char>()
        {
            { 33, '!' },
            { 34, '"' },
            { 35, '#' },
            { 36, '$' },
            { 37, '%' },
            { 38, '&' },
            { 39, '\'' },
            { 40, '(' },
            { 41, ')' },
            { 42, '*' },
            { 43, '+' },
            { 44, ',' },
            { 45, '-' },
            { 46, '.' },
            { 47, '/' },
            { 48, '0' },
            { 49, '1' },
            { 50, '2' },
            { 51, '3' },
            { 52, '4' },
            { 53, '5' },
            { 54, '6' },
            { 55, '7' },
            { 56, '8' },
            { 57, '9' },
            { 58, ':' },
            { 59, ';' },
            { 60, '<' },
            { 61, '=' },
            { 62, '>' },
            { 63, '?' },
            { 64, '@' },
            { 65, 'A' },
            { 66, 'B' },
            { 67, 'C' },
            { 68, 'D' },
            { 69, 'E' },
            { 70, 'F' },
            { 71, 'G' },
            { 72, 'H' },
            { 73, 'I' },
            { 74, 'J' },
            { 75, 'K' },
            { 76, 'L' },
            { 77, 'M' },
            { 78, 'N' },
            { 79, 'O' },
            { 80, 'P' },
            { 81, 'Q' },
            { 82, 'R' },
            { 83, 'S' },
            { 84, 'T' },
            { 85, 'U' },
            { 86, 'V' },
            { 87, 'W' },
            { 88, 'X' },
            { 89, 'Y' },
            { 90, 'Z' },
            { 91, '[' },
            { 92, '\\' },
            { 93, ']' },
            { 94, '^' },
            { 95, '_' },
            { 96, '`' },
            { 97, 'a' },
            { 98, 'b' },
            { 99, 'c' },
            { 100, 'd' },
            { 101, 'e' },
            { 102, 'f' },
            { 103, 'g' },
            { 104, 'h' },
            { 105, 'i' },
            { 106, 'j' },
            { 107, 'k' },
            { 108, 'l' },
            { 109, 'm' },
            { 110, 'n' },
            { 111, 'o' },
            { 112, 'p' },
            { 113, 'q' },
            { 114, 'r' },
            { 115, 's' },
            { 116, 't' },
            { 117, 'u' },
            { 118, 'v' },
            { 119, 'w' },
            { 120, 'x' },
            { 121, 'y' },
            { 122, 'z' },
            { 123, '{' },
            { 124, '|' },
            { 125, '}' },
            { 126, '~' },
        };
    }
}
