import os
import struct
import gymnasium as gym
from gymnasium import spaces
import socket
import numpy as np
from stable_baselines3 import PPO, A2C, SAC, a2c
from gravity_env import GravityEnv

def main() -> None:

  log_dir = "logs"
  model_dir = "models"

  os.makedirs(log_dir, exist_ok=True)
  os.makedirs(model_dir, exist_ok=True)

  # Create the environment
  env = GravityEnv()

  # Instantiate the AI (MlpPolicy is a standard neural network)
  model = A2C("MlpPolicy", env, verbose=1, device="cpu", tensorboard_log=log_dir)
  TIMESTEPS = 2500
  iters = 0
  while True:
    iters += 1
    model.learn(total_timesteps=TIMESTEPS, reset_num_timesteps=False)
    model.save(f"{model_dir}/a2c_gravity_pilot_{TIMESTEPS * iters}")    


if __name__ == "__main__":
	main()  