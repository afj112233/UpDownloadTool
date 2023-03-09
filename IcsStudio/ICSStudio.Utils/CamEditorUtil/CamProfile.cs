namespace ICSStudio.Utils.CamEditorUtil
{
        public enum SegmentType : uint
        {
            Linear = 0,
            Cubic = 1
        }

        public class CamProfile
        {
            // Master
            public double X { get; set; }
            // Slave
            public double Y { get; set; }

            public SegmentType Type { get; set; }
            public uint Status { get; set; }

            public double C0 { get; set; }
            public double C1 { get; set; }
            public double C2 { get; set; }
            public double C3 { get; set; }

            public override string ToString()
            {
                return $"{X} {Y} {Type} {Status} {C0} {C1} {C2} {C3}";
            }
        }
    }