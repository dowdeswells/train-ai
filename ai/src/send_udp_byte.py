#!/usr/bin/env python3
import socket
import time
import argparse

def send_loop(host: str, port: int, interval: float, data: bytes) -> None:
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    try:
        while True:
            sock.sendto(data, (host, port))
            print(f"Sent byte {data} to {host}:{port}")
            time.sleep(interval)
    except KeyboardInterrupt:
        pass
    finally:
        sock.close()

def main():
    p = argparse.ArgumentParser(description="Send one byte over UDP repeatedly.")
    p.add_argument("--host", default="127.0.0.1", help="Destination host")
    p.add_argument("--port", type=int, default=5005, help="Destination UDP port")
    p.add_argument("--byte", type=int, default=1, help="Byte value to send (0-255)")
    p.add_argument("--interval", type=float, default=0.3, help="Seconds between sends")
    args = p.parse_args()

    data = bytes([args.byte & 0xFF])
    send_loop(args.host, args.port, args.interval, data)


if __name__ == "__main__":
    main()
