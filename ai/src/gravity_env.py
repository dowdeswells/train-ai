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
        self.action_space = spaces.Discrete(2) # 0: Off, 1: On
        # Obs: [Time, Altitude, Velocity, Acceleration]
        self.observation_space = spaces.Box(low=-np.inf, high=np.inf, shape=(4,), dtype=np.float32)

        # 2. Setup UDP
        self.tx_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

        self.rx_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.rx_socket.bind(('127.0.0.1', 5006))
        self.rx_socket.setblocking(False)

    def _get_telemetry(self):
      
        latest_packet = None
        while True:
            try:
                received_bytes, _ = self.rx_socket.recvfrom(1024)
                latest_packet = received_bytes
            except socket.error as e:
                # EAGAIN or EWOULDBLOCK means the buffer is now empty
                if e.errno in (errno.EAGAIN, errno.EWOULDBLOCK):
                    break
                raise
        if latest_packet:
            # Assuming C# sends: "time,alt,vel,acc,thrust"
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
        self.tx_socket.sendto(bytes([action]), ('127.0.0.1', 5005))
        
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
        # Signal C# to reset (optional: you'd need to add reset logic in C#)
        self.tx_socket.sendto(bytes([2]), ('127.0.0.1', 5005))
        obs = self._get_telemetry()
        # while obs[0] > 0.2:
        #     print("Reset")
        #     self.tx_socket.sendto(bytes([2]), ('127.0.0.1', 5005))
        #     obs = self._get_telemetry()
        return obs, {}
    
