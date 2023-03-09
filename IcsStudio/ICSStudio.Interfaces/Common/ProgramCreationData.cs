namespace ICSStudio.Interfaces.Common
{
    public struct ProgramCreationData
    {
        public ushort RevisionMajor { get; set; }

        public ushort RevisionMinor { get; set; }

        public string RevisionExtension { get; set; }

        public string RevisionDescription { get; set; }

        public string Description { get; set; }

        public bool Inhibited { get; set; }

        public bool SynchronizeData { get; set; }

        public override int GetHashCode()
        {
            return (int) RevisionMajor << 16 & (int) RevisionMinor;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ProgramCreationData))
                return false;
            return this.Equals((ProgramCreationData) obj);
        }

        public bool Equals(ProgramCreationData other)
        {
            return (int) this.RevisionMajor == (int) other.RevisionMajor &&
                   (int) this.RevisionMinor == (int) other.RevisionMinor &&
                   (!(this.RevisionExtension != other.RevisionExtension) &&
                    !(this.RevisionDescription != other.RevisionDescription)) &&
                   (!(this.Description != other.Description) && this.Inhibited == other.Inhibited &&
                    this.SynchronizeData == other.SynchronizeData);
        }

        public static bool operator ==(ProgramCreationData programCreationData1,
            ProgramCreationData programCreationData2)
        {
            return programCreationData1.Equals(programCreationData2);
        }

        public static bool operator !=(ProgramCreationData programCreationData1,
            ProgramCreationData programCreationData2)
        {
            return !programCreationData1.Equals(programCreationData2);
        }
    }
}
