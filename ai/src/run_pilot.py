import gymnasium as gym
from stable_baselines3 import PPO, A2C
import time
from gravity_env import GravityEnv


def main() -> None:
  # 1. Load the environment (the same class you defined earlier)
  env = GravityEnv()

  # 2. Load the trained brain
  model = A2C.load(f"./models/a2c_gravity_pilot_70000")

  print("Pilot engaged. Press Ctrl+C to stop.")

  hasCrashed = False
  obs, info = env.reset()
  try:
      while not hasCrashed:
          # 3. Predict the action
          # deterministic=True ensures the AI uses its best guess, not random exploration
          action, _states = model.predict(obs, deterministic=True)

          # 4. Apply the action to the game via UDP
          obs, reward, terminated, truncated, info = env.step(action)

          # Optional: Add a tiny sleep to match your game's physics frequency
          time.sleep(0.02) 

          if terminated or truncated:
              print(f"Crash occurred... {obs}")
              hasCrashed = True

  except KeyboardInterrupt:
      print("Pilot disengaged.")

if __name__ == "__main__":
	main()        