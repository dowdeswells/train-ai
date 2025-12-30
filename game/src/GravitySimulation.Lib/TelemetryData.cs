using System.Runtime.InteropServices;

namespace GravitySimulation.Lib;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TelemetryData
{
    public bool ThrustStatus;
    public double Timestamp;
    public long Altitude;
    public long Velocity;
    public long Acceleration;
}