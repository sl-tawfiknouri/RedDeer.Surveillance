using TestHarness.State.Interfaces;

namespace TestHarness.State
{
    public class ProgramState : IProgramState
    {
        public bool Executing { get; set; }
    }
}
