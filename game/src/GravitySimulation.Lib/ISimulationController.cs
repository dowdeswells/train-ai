namespace GravitySimulation.Lib;

public interface ISimulationController
{
    bool Reset { get; }
    bool EndSimulation { get; }
}

public interface IInputReader
{
    void Read();
}