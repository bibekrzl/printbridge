"""
Test script for PrintBridge API
Usage: python test_api.py
"""

import requests
import json
import base64
import time
from PIL import Image, ImageDraw, ImageFont
import io

# Configuration
BASE_URL = "http://localhost:8080"

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

    draw.text((text_x, text_y), "56x31mm", fill='black', font=font)
    draw.rectangle([2, 2, max(width_pixels-2, 2), max(height_pixels-2, 2)], outline='black', width=1)

    # --- Save the image to disk for preview ---
    img.save("test_label.png", format="PNG")
    print(f"Saved test_label.png for preview. Size: {width_pixels}x{height_pixels} pixels")

    buffer = io.BytesIO()
    img.save(buffer, format='PNG')
    img_base64 = base64.b64encode(buffer.getvalue()).decode('utf-8')

    return img_base64

def test_get_printers():
    """Test GET /printers endpoint"""
    print("Testing GET /printers...")
    try:
        response = requests.get(f"{BASE_URL}/printers")
        if response.status_code == 200:
            printers = response.json()
            print(f"✓ Found {len(printers)} printers:")
            for printer in printers:
                print(f"  - {printer}")
        else:
            print(f"✗ Error: {response.status_code}")
    except Exception as e:
        print(f"✗ Connection error: {e}")

def test_print_endpoint():
    """Test POST /print endpoint"""
    print("\nTesting POST /print...")
    
    # Create test image
    test_image = create_test_image()
    
    # Test data
    data = {
        "base64Image": test_image,
        "printerName": None  # Use default printer
    }
    
    try:
        response = requests.post(
            f"{BASE_URL}/print",
            json=data,
            headers={'Content-Type': 'application/json'}
        )
        
        if response.status_code == 200:
            result = response.json()
            print(f"✓ Print job submitted successfully")
            print(f"  Printer: {result.get('printerName', 'Unknown')}")
            print(f"  Success: {result.get('success', False)}")
            if result.get('errorMessage'):
                print(f"  Error: {result['errorMessage']}")
        else:
            print(f"✗ Error: {response.status_code}")
            print(f"  Response: {response.text}")
    except Exception as e:
        print(f"✗ Connection error: {e}")

def test_jobs_endpoint():
    """Test GET /jobs endpoint"""
    print("\nTesting GET /jobs...")
    try:
        response = requests.get(f"{BASE_URL}/jobs")
        if response.status_code == 200:
            jobs = response.json()
            print(f"✓ Found {len(jobs)} print jobs:")
            for job in jobs[-3:]:  # Show last 3 jobs
                timestamp = job.get('timestamp', 'Unknown')
                printer = job.get('printerName', 'Unknown')
                success = job.get('success', False)
                status = "✓ Success" if success else "✗ Error"
                print(f"  {timestamp} - {printer} - {status}")
        else:
            print(f"✗ Error: {response.status_code}")
    except Exception as e:
        print(f"✗ Connection error: {e}")

def test_websocket():
    """Test WebSocket endpoint (basic connection test)"""
    print("\nTesting WebSocket connection...")
    try:
        import websocket
        
        def on_message(ws, message):
            print(f"✓ WebSocket received: {message}")
            ws.close()
        
        def on_error(ws, error):
            print(f"✗ WebSocket error: {error}")
        
        def on_close(ws, close_status_code, close_msg):
            print("WebSocket connection closed")
        
        def on_open(ws):
            print("✓ WebSocket connected successfully")
            # Send test image
            test_image = create_test_image()
            ws.send(test_image)
        
        # Connect to WebSocket
        ws = websocket.WebSocketApp(
            "ws://localhost:8080/ws",
            on_open=on_open,
            on_message=on_message,
            on_error=on_error,
            on_close=on_close
        )
        
        # Run for 5 seconds
        import threading
        timer = threading.Timer(5.0, ws.close)
        timer.start()
        ws.run_forever()
        timer.cancel()
        
    except ImportError:
        print("✗ websocket-client library not installed. Install with: pip install websocket-client")
    except Exception as e:
        print(f"✗ WebSocket error: {e}")

def main():
    """Run all tests"""
    print("PrintBridge API Test Script")
    print("=" * 40)
    
    # Check if server is running
    try:
        response = requests.get(f"{BASE_URL}/", timeout=5)
        if response.status_code == 200:
            print("✓ Server is running")
        else:
            print(f"✗ Server responded with status {response.status_code}")
            return
    except Exception as e:
        print(f"✗ Cannot connect to server: {e}")
        print("Make sure PrintBridge is running on http://localhost:8080")
        return
    
    # Run tests
    test_get_printers()
    test_print_endpoint()
    test_jobs_endpoint()
    test_websocket()
    
    print("\n" + "=" * 40)
    print("Test completed!")
    print("Visit http://localhost:8080 to see the dashboard")

if __name__ == "__main__":
    main() 