# CAMERA TEST SCRIPT
# Prints the camera data to the screen.
# Adapted from github.com/adafruit/Adafruit_CircuitPython_MLX90640
# Matplotlib code adapted from https://github.com/tomshaffner/PiThermalCam/blob/master/sequential_versions/matplotlib_therm_cam.py

import time
import board
import busio
import adafruit_mlx90640
import matplotlib.pyplot as plt
import numpy as np

CAMERA_HEIGHT = 24
CAMERA_WIDTH = 32
CAMERA_DIM = (CAMERA_HEIGHT, CAMERA_WIDTH)

i2c = busio.I2C(board.SCL, board.SDA, frequency=800000)

mlx = adafruit_mlx90640.MLX90640(i2c)
print("MLX found on I2C bus: ", [hex(i) for i in mlx.serial_number])

mlx.refresh_rate = adafruit_mlx90640.RefreshRate.REFRESH_2_HZ

def start_plotting():
	plt.ion()
	fig,ax = plt.subplots(figsize=(12,7))
	plt_data = ax.imshow(np.zeros(CAMERA_DIM),vmin=0,vmax=60)
	color_bar = fig.colorbar(plt_data)
	color_bar.set_label("Temperature [$^{\circ}$C]", fontsize=14)

	frame = np.zeros((CAMERA_HEIGHT * CAMERA_WIDTH, ))

	while True:
		try:
			mlx.getFrame(frame)
		except ValueError:
			# These errors happen, just retry
			continue

		data_array = np.reshape(frame, CAMERA_DIM)
		plt_data.set_data(np.fliplr(data_array))
		# plt_data.set_clim(vmin=np.min(data_array), vmax=np.max(data_array))
		color_bar.mappable.set_clim(vmin=np.min(data_array), vmax=np.max(data_array))
		plt.pause(1)
		print("Plotting")

start_plotting()
