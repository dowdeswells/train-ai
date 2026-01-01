import errno
import struct
import gymnasium as gym
from gymnasium import spaces
import socket
import numpy as np






class GravityEnv(gym.Env):
    def __init__(self):
        super(GravityEnv, self).__init__()

        # 1. Define Spaces
        self.action_space = spaces.Discrete(3) # 0: Do Nothing, 1: Turn Thrust On, 2: Turn Thrust Off
        # Obs: [Time, Altitude, Velocity, Acceleration]
        self.observation_space = spaces.Box(low=-np.inf, high=np.inf, shape=(4,), dtype=np.float32)

        server_address = '/tmp/gravity_sim.sock'
        self.client = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
        self.client.connect(server_address)

    def _get_telemetry(self):
        latest_packet = self.client.recv(1024)
        if len(latest_packet) > 0:
          format_str = "<?dqqq"
              
          unpacked = struct.unpack(format_str, latest_packet)
          
          data = {
              "ThrustStatus": unpacked[0],
              "Timestamp":    unpacked[1],
              "Altitude":     unpacked[2],
              "Velocity":     unpacked[3],
              "Acceleration": unpacked[4]
          }
          return np.array([data["Timestamp"], data["Altitude"]/100, data["Velocity"]/100,data["Acceleration"]/100], dtype=np.float32)
        else:
            return np.zeros(4, dtype=np.float32)


    def step(self, action):
        # Send action to C# (Port 5005)
        self.client.send(bytes([action]))
        
        # Get new state from C# (Port 5006)
        obs = self._get_telemetry()
        altitude = obs[1]
        velocity = obs[2]
        time_elapsed = obs[0]

        reward = 0

        # reward

        if 20 <= altitude <= 40:
            reward = 20
        
        if (10 <= altitude < 20) or (40 < altitude <= 50):
            reward = -5
        
        if (2 <= altitude < 10) or (50 < altitude <= 58):
            reward = -10


        print(f"sent {action}, received: time: {time_elapsed}, Altitude {altitude}, Velocity {velocity}, Rewarded: {reward}")
        # 4. Termination Logic
        terminated = time_elapsed > 0 and (altitude <=0 or altitude >= 60) # Crash or Fly away
        truncated = False 
        
        return obs, reward, terminated, truncated, {}

    def reset(self, seed=None, options=None):
        super().reset(seed=seed)
        self.client.send(bytes([3]))
        has_restarted = False
        while not has_restarted:
          obs = self._get_telemetry()
          has_restarted = obs[0] < 0.4
          if not has_restarted:
              self.client.send(bytes([0]))
        return obs, {}
    
