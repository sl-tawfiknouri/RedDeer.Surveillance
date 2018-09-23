using Surveillance.Rules.Marking_The_Close.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IMarkingTheCloseRuleFactory
    {
        IMarkingTheCloseRule Build();
    }
}