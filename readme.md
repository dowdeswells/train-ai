1. Initial direction

i want to create a very simple gravity simulation in c#. i want to model a flying object that has just one action which produces thrust to push the object up at an acceleration just enough to slightly surpass the effect of gravity in the opposite direction - the output of each simulation loop will simply be the altitude of the object 

I want to connect this simulation to input and output via UDP

Bash UDP Port Avalailability:

````
ss -ulpn
````

my game accepts input on udp port 5005 to receive thrust commands and sends telemetry of current game time (in seconds), altitude (in meters), velocity, acceleration and Thrust Status on udp port 5006. how do i use Stable Baselines3 to train an AI to keep the altitude between 500 and 600 meters for 20 seconds 

Later i ammended this to start the simulation at a height of 200m and attempt to get the AI to hover

2. The Python "Bridge" Code

You'll need gymnasium and stable-baselines3 installed (pip install gymnasium stable-baselines3).

This resulted in the following set of bash commands 
(stable-baselines3[extra] includes tensorboard)
(nvidia-smi showed a cuda version of 12.8 ==> cu128)

````
sudo apt install python3-pip python3-venv ffmpeg freeglut3-dev xvfb -y
python3 -m venv sb3_env
source sb3_env/bin/activate
nvidia-smi
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu128
pip install "stable-baselines3[extra]"
````

3. Some usefull stuff

VS Code, launched from the activated venv inherits the environment

You can see the training process using the dashboard. Launched from bash under the venv:

````
tensorboard --logdir logs
````
