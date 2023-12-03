# Python CubeSatOS Driver

## Setup i2c
- Enable i2c in raspberry pi configuration
- Raise baudrate. `sudo nano /boot/config.txt`; look for `dtparam`; change the line to `dtparam=i2c_arm=on,i2c_arm_baudrate=400000`

## Install requisite packages
- `sudo apt-get install libopenblas-dev`

## Start the project
- `python3 -m venv .venv`
- `source .venv/bin/activate`
- `pip3 install -r requirements.txt`
- `python3 main.py`

