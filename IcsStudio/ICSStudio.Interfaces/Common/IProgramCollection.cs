namespace ICSStudio.Interfaces.Common
{
    public interface IProgramCollection : IBaseComponentCollection<IProgram>
    {
        void Remove(int programUid);

        void Remove(string programName);

        IProgram Add(string programName, ProgramType programType);

        IProgram Add(string programName, ProgramType programType, ProgramCreationData programCreationData);
    }
}
