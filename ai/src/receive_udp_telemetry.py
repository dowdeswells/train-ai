
#!/usr/bin/env python3
"""Simple UDP receiver that prints received byte arrays.

Binds to port 5006 by default and prints the sender address,
length and hex dump of each received datagram.
"""

import argparse
import socket
import struct

def unpack_telemetry(byte_data):
    # Format String Explanation:
    # < : Little Endian (Standard for C# / Intel / AMD)
    # ? : bool (1 byte)
    # d : double (8 bytes)
    # q : long long / int64 (8 bytes)
    # q : long long / int64 (8 bytes)
    # q : long long / int64 (8 bytes)
    format_str = "<?dqqq"
    
    unpacked = struct.unpack(format_str, byte_data)
    
    data = {
        "ThrustStatus": unpacked[0],
        "Timestamp":    unpacked[1],
        "Altitude":     unpacked[2],
        "Velocity":     unpacked[3],
        "Acceleration": unpacked[4]
    }
    return data

def receive_loop(host: str, port: int, bufsize: int = 4096) -> None:
	sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sock.bind((host, port))
	print(f"Listening on {host}:{port}")
	try:
		while True:
			data, addr = sock.recvfrom(bufsize)
			# hex_str = data.hex()
			# print(f"Received {len(data)} bytes from {addr}: {hex_str}")
			telemetry = unpack_telemetry (data)
			print(f"Seconds: {telemetry["Timestamp"]} Altitude {telemetry["Altitude"]}, Velocity {telemetry["Velocity"]} ")
	except KeyboardInterrupt:
		print("Receiver interrupted, exiting")
	finally:
		sock.close()


def main() -> None:
	p = argparse.ArgumentParser(description="Receive arrays of bytes over UDP (default port 5006)")
	p.add_argument("--host", default="0.0.0.0", help="Host/interface to bind to")
	p.add_argument("--port", type=int, default=5006, help="UDP port to bind to")
	p.add_argument("--bufsize", type=int, default=4096, help="Receive buffer size")
	args = p.parse_args()

	receive_loop(args.host, args.port, args.bufsize)


if __name__ == "__main__":
	main()

