namespace GravitySimulation.Lib;

public class WorldModel
{
    public const double Gravity = -9.81;
    
    public ICollection<FlyingThing> _things = new List<FlyingThing>();

    public void AddThing(FlyingThing thing)
    {
        _things.Add(thing);
    }

    public void Advance(double deltaTime)
    {
        foreach (var thing in _things)
        {
            thing.ApplyWorld(this, deltaTime);
        }
    }

    public void Reset()
    {
        foreach (var thing in _things)
        {
            thing.Reset();
        }
    }
}