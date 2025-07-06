# PrintBridge Tray App

A .NET 8 WinForms tray application that provides a background ASP.NET Core Web API server for printing base64 PNG images.

## Features

- **Tray Icon**: Runs in system tray with "PrintBridge Running" tooltip
- **Web API**: POST endpoint at `/print` for printing base64 PNG images
- **WebSocket**: Real-time printing via WebSocket at `/ws`
- **Dashboard UI**: Web interface at http://localhost:8080 showing:
  - Available printers
  - Last received image preview
  - Print job log
- **Silent Printing**: Uses System.Drawing.Printing for silent printing
- **Real-world Sizing**: Calculates size at 203 DPI
- **Windows Startup**: Optionally adds itself to Windows startup

## Prerequisites

- .NET 8 SDK
- Windows OS (for printing functionality)
- Visual Studio Code or Visual Studio

## Installation

1. **Clone or download the project**
2. **Install dependencies**:
   ```bash
   dotnet restore
   ```
3. **Build the project**:
   ```bash
   dotnet build
   ```
4. **Run the application**:
   ```bash
   dotnet run
   ```

## Usage

### Starting the Application

1. Run the application - it will start in the system tray
2. The web server starts automatically on http://localhost:8080
3. Right-click the tray icon to exit

### API Endpoints

#### POST /print
Print a base64 PNG image.

**Request Body:**
```json
{
  "base64Image": "iVBORw0KGgoAAAANSUhEUgAA...",
  "printerName": "HP LaserJet Pro" // optional
}
```

**Response:**
```json
{
  "success": true,
  "printerName": "HP LaserJet Pro",
  "errorMessage": null
}
```

#### WebSocket /ws
Send base64 PNG strings for real-time printing.

**Message Format:**
```
iVBORw0KGgoAAAANSUhEUgAA...
```

#### GET /printers
Get list of available printers.

#### GET /jobs
Get print job history.

### Dashboard

Visit http://localhost:8080 to access the web dashboard:

- **Available Printers**: Shows all installed printers
- **Last Received Image**: Preview of the most recent image
- **Print Job Log**: History of all print jobs with timestamps and status

### WebSocket Connection

The dashboard automatically connects to the WebSocket endpoint for real-time updates.

## Configuration

### DPI Setting
The application uses 203 DPI for real-world size calculations. To change this, modify the `DPI` constant in `PrintService.cs`.

### Port Configuration
The web server runs on port 8080 by default. To change this, modify the port in `WebServer.cs`.

## Troubleshooting

### Common Issues

1. **"dotnet command not found"**
   - Install .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0

2. **Printing fails**
   - Check that the printer is installed and accessible
   - Verify printer permissions
   - Check console output for error messages

3. **Web server won't start**
   - Check if port 8080 is already in use
   - Run as administrator if needed

4. **Base64 decoding errors**
   - Ensure the image is a valid PNG in base64 format
   - Remove any data URL prefixes (e.g., "data:image/png;base64,")

### Debug Information

The application logs detailed information to the console:
- Web server startup
- Print job details
- Error messages
- Printer discovery

## Development

### Project Structure

```
PrintBridgeTrayApp/
├── Program.cs              # WinForms entry point, tray icon
├── WebServer.cs            # ASP.NET Core server, endpoints
├── PrintService.cs         # Printing logic, DPI calculations
├── PrintJobLog.cs          # Job tracking and history
├── PrintBridgeTrayApp.csproj # Project file
└── README.md               # This file
```

### Building for Release

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

### Adding Custom Icons

Replace the default system icon by:
1. Adding an `.ico` file to the project
2. Updating the `Icon` property in `Program.cs`

## License

This project is provided as-is for educational and development purposes.

## Contributing

Feel free to submit issues and enhancement requests! 