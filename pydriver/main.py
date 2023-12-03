import socketio
import eventlet
import threading

PORT = 1234

socket = socketio.Server()
app = socketio.WSGIApp(socket)

@socket.event
def connect(sid, environ):
    print("Client connected: ", sid)

@socket.event
def disconnect(sid):
    print("Client disconnected: ", sid)

@socket.event
def set_motor_powers(sid, data):
    l_power = data["l"]
    r_power = data["r"]
    cam_power = data["c"]
    print("Setting motor powers: ", l_power, r_power, cam_power)

def camera_task():
    while True:
        print("Emitting camera data")
        socket.emit("camera_data", [1, 2, 3])
        socket.sleep(1)

camera_thread = socket.start_background_task(camera_task)
eventlet.wsgi.server(eventlet.listen(('', PORT)), app)


