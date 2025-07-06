#!/usr/bin/env python3
"""
Test WebSocket connection to PrintBridge server
Usage: python test_websocket.py
"""

import websocket
import json
import base64
import time
from PIL import Image, ImageDraw, ImageFont
import io

def create_test_image():
    """Create a simple test PNG image for 56x31mm at 108 DPI"""
    width_pixels = int(56 / 25.4 * 108)  # 56mm at 108 DPI
    height_pixels = int(31 / 25.4 * 108)  # 31mm at 108 DPI

    img = Image.new('RGB', (width_pixels, height_pixels), color='white')
    draw = ImageDraw.Draw(img)

    # Add some text
    try:
        font = ImageFont.load_default()
    except:
        font = None

    text_x = width_pixels // 4 if width_pixels >= 4 else 0
    text_y = height_pixels // 3 if height_pixels >= 3 else 0

    draw.text((text_x, text_y), "TEST", fill='black', font=font)
    draw.rectangle([2, 2, max(width_pixels-2, 2), max(height_pixels-2, 2)], outline='black', width=1)

    buffer = io.BytesIO()
    img.save(buffer, format='PNG')
    img_base64 = base64.b64encode(buffer.getvalue()).decode('utf-8')

    return img_base64

def on_message(ws, message):
    """Handle incoming WebSocket messages"""
    try:
        data = json.loads(message)
        print(f"✓ Received: {data}")
        
        if data.get('type') == 'connection':
            print(f"✓ Connected! Available printers: {data.get('printers', [])}")
            print(f"✓ Default printer: {data.get('defaultPrinter', 'Unknown')}")
            
            # Send a test print after connection
            time.sleep(1)
            test_image = create_test_image()
            print(f"✓ Sending test print (image size: {len(test_image)} chars)")
            ws.send(test_image)
            
    except json.JSONDecodeError:
        print(f"✓ Received raw message: {message[:100]}...")

def on_error(ws, error):
    """Handle WebSocket errors"""
    print(f"✗ WebSocket error: {error}")

def on_close(ws, close_status_code, close_msg):
    """Handle WebSocket connection close"""
    print(f"WebSocket connection closed: {close_status_code} - {close_msg}")

def on_open(ws):
    """Handle WebSocket connection open"""
    print("✓ WebSocket connected to PrintBridge server")

def main():
    """Test WebSocket connection"""
    print("PrintBridge WebSocket Test")
    print("=" * 40)
    
    # Create WebSocket connection
    ws = websocket.WebSocketApp(
        "ws://localhost:8080/ws",
        on_open=on_open,
        on_message=on_message,
        on_error=on_error,
        on_close=on_close
    )
    
    print("Connecting to ws://localhost:8080/ws...")
    
    # Run for 10 seconds to test the connection
    import threading
    timer = threading.Timer(10.0, ws.close)
    timer.start()
    
    try:
        ws.run_forever()
    except KeyboardInterrupt:
        print("\nTest interrupted by user")
    finally:
        timer.cancel()
    
    print("=" * 40)
    print("WebSocket test completed!")

if __name__ == "__main__":
    main() 