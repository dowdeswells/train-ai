namespace GravitySimulation.Lib;

public interface ISimulationController
{
    bool IsReset(ulong currentLoop);
    bool EndSimulation { get; }
}

public interface IInputReader
{
    void Read();
}