namespace GravitySimulation.Lib;

public class FlyingThing
{
    private readonly IFlightController _controller;
    private readonly double _thrustPower = 30.0;
    public bool IsThrusting;
    public double Altitude;
    public double Velocity;
    public double Acceleration;

    public FlyingThing(IFlightController controller)
    {
        _controller = controller;
        Altitude = WorldModel.StartHeight;
    }

    public void ApplyWorld(WorldModel world, double deltaTime)
    {
        Acceleration = WorldModel.Gravity;
        IsThrusting = _controller.IsThrusting;
        if (IsThrusting)
        {
            Acceleration += _thrustPower;
        }    
        
        Velocity += Acceleration * deltaTime;
        Altitude += Velocity * deltaTime;

        // 4. Ground Collision
        if (Altitude <= 0)
        {
            Altitude = 0;
            Velocity = 0.0; // Stop falling at ground
            Acceleration = 0.0;
        }
    }

    public void Reset()
    {
        Altitude = WorldModel.StartHeight;
        Velocity = 0.0; // Stop falling at ground
        Acceleration = 0.0;
    }
}

public interface IFlightController
{
    bool IsThrusting { get; }
}