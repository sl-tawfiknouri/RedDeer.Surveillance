namespace TestHarness.State
{
    using TestHarness.State.Interfaces;

    public class ProgramState : IProgramState
    {
        public bool Executing { get; set; }
    }
}