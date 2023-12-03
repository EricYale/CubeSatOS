import board
import busio
import adafruit_mlx90640
import socketio
import eventlet
import RPi.GPIO as GPIO

PORT = 1234
CAMERA_HEIGHT = 24
CAMERA_WIDTH = 32
CAMERA_POLL_RATE = 0.3

L_MOTOR_HI_PIN = 5
L_MOTOR_LO_PIN = 0

R_MOTOR_HI_PIN = 13
R_MOTOR_LO_PIN = 11

CAM_MOTOR_HI_PIN = 26
CAM_MOTOR_LO_PIN = 19


i2c = busio.I2C(board.SCL, board.SDA, frequency=800000)

mlx = adafruit_mlx90640.MLX90640(i2c)
print("MLX found on I2C bus: ", [hex(i) for i in mlx.serial_number])
mlx.refresh_rate = adafruit_mlx90640.RefreshRate.REFRESH_2_HZ

socket = socketio.Server()
app = socketio.WSGIApp(socket)

l_power = 0
r_power = 0
cam_power = 0

GPIO.setmode(GPIO.BCM)
GPIO.setup(L_MOTOR_HI_PIN, GPIO.OUT)
GPIO.setup(L_MOTOR_LO_PIN, GPIO.OUT)
GPIO.setup(R_MOTOR_HI_PIN, GPIO.OUT)
GPIO.setup(R_MOTOR_LO_PIN, GPIO.OUT)
GPIO.setup(CAM_MOTOR_HI_PIN, GPIO.OUT)
GPIO.setup(CAM_MOTOR_LO_PIN, GPIO.OUT)

l_hi = GPIO.PWM(L_MOTOR_HI_PIN, 100)
l_lo = GPIO.PWM(L_MOTOR_LO_PIN, 100)
r_hi = GPIO.PWM(R_MOTOR_HI_PIN, 100)
r_lo = GPIO.PWM(R_MOTOR_LO_PIN, 100)
cam_hi = GPIO.PWM(CAM_MOTOR_HI_PIN, 100)
cam_lo = GPIO.PWM(CAM_MOTOR_LO_PIN, 100)

l_hi.start(0)
l_lo.start(0)
r_hi.start(0)
r_lo.start(0)
cam_hi.start(0)
cam_lo.start(0)

def write_gpio(l_power, r_power, cam_power):
	l_hi.ChangeDutyCycle(0 if l_power < 0 else l_power * 100)
	l_lo.ChangeDutyCycle(0 if l_power > 0 else -l_power * 100)
	r_hi.ChangeDutyCycle(0 if r_power < 0 else r_power * 100)
	r_lo.ChangeDutyCycle(0 if r_power > 0 else -r_power * 100)
	cam_hi.ChangeDutyCycle(0 if cam_power < 0 else cam_power * 100)
	cam_lo.ChangeDutyCycle(0 if cam_power > 0 else -cam_power * 100)

@socket.event
def connect(sid, environ):
	print("Client connected: ", sid)

@socket.event
def disconnect(sid):
	print("Client disconnected: ", sid)
	write_gpio(0, 0, 0)

@socket.event
def set_motor_powers(sid, data):
	l_power = data["l"]
	r_power = data["r"]
	cam_power = data["c"]
	write_gpio(l_power, r_power, cam_power)
	print("Setting motor powers: ", l_power, r_power, cam_power)

def camera_task():
	frame = [0] * CAMERA_HEIGHT * CAMERA_WIDTH
	while True:
		print("Emitting camera data")

		try:
			mlx.getFrame(frame)
		except ValueError:
			# These errors happen, just retry
			continue
		
		socket.emit("camera_data", frame)
		socket.sleep(CAMERA_POLL_RATE)

camera_thread = socket.start_background_task(camera_task)
eventlet.wsgi.server(eventlet.listen(('', PORT)), app)
