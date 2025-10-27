# ESP32-CAM to Web API Solution

Complete solution for capturing images with ESP32-CAM and uploading them to a .NET 9 Web API.

## Solution Overview

This solution consists of two main components:

### 1. CamApiSample (.NET 9 Web API)
A RESTful API that receives JPEG images and stores them on the file system.

**Location**: `f:\work\experiments\espcamdeploy\sample\CamApiSample\`

**Key Features**:
- Image upload endpoint (`POST /api/Image/upload`)
- Image listing endpoint (`GET /api/Image/list`)
- Configurable storage path
- Automatic directory creation
- Detailed logging

### 2. NfEsp32CamApiSample (nanoFramework Application)
A client application for ESP32-CAM that captures and uploads images.

**Location**: `f:\work\experiments\espcamdeploy\sample\NfEsp32CamSampleSln\NfEsp32CamApiSample\`

**Key Features**:
- WiFi connectivity
- Camera image capture
- HTTP client for image upload
- Configurable capture interval
- Error handling and retry logic

## Quick Start Guide

### Step 1: Start the Web API

1. Open a terminal and navigate to the API project:
   ```powershell
   cd f:\work\experiments\espcamdeploy\sample\CamApiSample
   ```

2. Run the API:
   ```powershell
   dotnet run
   ```

3. Note the URL (typically `http://localhost:5000`)

4. Find your PC's IP address:
   ```powershell
   ipconfig
   ```
   Look for your IPv4 Address (e.g., 192.168.1.100)

### Step 2: Configure the ESP32-CAM Client

1. Open the solution in Visual Studio:
   - Open `NfEsp32CamSampleSln.sln`

2. Edit `NfEsp32CamApiSample\Program.cs`:
   ```csharp
   private const string WIFI_SSID = "YourWiFiName";
   private const string WIFI_PASSWORD = "YourWiFiPassword";
   private const string API_URL = "http://192.168.1.100:5000/api/Image/upload";
   ```

3. Restore NuGet packages (Build → Restore NuGet Packages)

### Step 3: Deploy to ESP32-CAM

1. Connect your ESP32-CAM via USB
2. Select your device in Visual Studio's Device Explorer
3. Press F5 to build and deploy
4. Monitor the Debug output window

### Step 4: Verify Images

Images will be stored in: `CamApiSample\Images\`

You can also check via the API:
```powershell
curl http://localhost:5000/api/Image/list
```

## Architecture

```
┌─────────────────┐
│   ESP32-CAM     │
│                 │
│  - WiFi         │
│  - Camera       │
│  - HTTP Client  │
└────────┬────────┘
         │
         │ HTTP POST
         │ (JPEG image)
         │
         ▼
┌─────────────────┐
│  .NET Web API   │
│                 │
│  - ImageController
│  - File Storage │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   File System   │
│   (Images/)     │
└─────────────────┘
```

## Data Flow

1. **ESP32-CAM** captures image using camera hardware
2. **WiFi** connection maintained to local network
3. **HTTP POST** sends JPEG data to Web API
4. **Web API** receives image and generates unique filename
5. **File System** stores image with timestamp
6. **Response** sent back to ESP32-CAM confirming upload

## Configuration Files

### Web API Configuration
`CamApiSample\appsettings.json`:
```json
{
  "ImageStoragePath": "Images"
}
```

### ESP32-CAM Configuration
`NfEsp32CamApiSample\Program.cs`:
```csharp
private const string WIFI_SSID = "YOUR_WIFI_SSID";
private const string WIFI_PASSWORD = "YOUR_WIFI_PASSWORD";
private const string API_URL = "http://YOUR_PC_IP:5000/api/Image/upload";
private const int CAPTURE_INTERVAL = 5000; // milliseconds
```

## Testing the API

### Upload a test image:
```powershell
curl -X POST http://localhost:5000/api/Image/upload `
  -H "Content-Type: image/jpeg" `
  --data-binary "@test.jpg"
```

### List all images:
```powershell
curl http://localhost:5000/api/Image/list
```

## Troubleshooting

### ESP32-CAM can't connect to WiFi
- Verify SSID and password
- Ensure 2.4GHz network (ESP32 doesn't support 5GHz)
- Check signal strength

### Upload fails with connection refused
- Verify Web API is running
- Check firewall settings
- Confirm IP address is correct
- Ensure both devices on same network

### Camera initialization fails
- Reset the ESP32-CAM
- Check hardware connections
- Verify nanoFramework firmware version

### Images not appearing in folder
- Check `ImageStoragePath` configuration
- Verify write permissions
- Check API logs for errors

## Performance Considerations

### ESP32-CAM Side
- Lower resolution = faster capture
- Adjust JPEG quality for size/quality trade-off
- Increase interval to reduce network load

### Web API Side
- Consider adding image compression
- Implement cleanup for old images
- Add database for metadata tracking
- Consider cloud storage for scalability

## Future Enhancements

Potential improvements:
- [ ] Add authentication/API keys
- [ ] Implement image thumbnails
- [ ] Add real-time streaming
- [ ] Database integration
- [ ] Cloud storage (Azure Blob, AWS S3)
- [ ] Image processing (face detection, etc.)
- [ ] WebSocket for real-time updates
- [ ] Image gallery web interface
- [ ] Motion detection on ESP32-CAM
- [ ] Time-lapse functionality

## Project Structure

```
sample/
├── CamApiSample/                    # .NET 9 Web API
│   ├── Controllers/
│   │   └── ImageController.cs       # Upload & list endpoints
│   ├── Images/                      # Stored images
│   ├── Program.cs
│   ├── appsettings.json
│   └── README.md
│
└── NfEsp32CamSampleSln/
    ├── NfEsp32CamSample/            # SD card example
    │   └── Program.cs
    └── NfEsp32CamApiSample/         # API client
        ├── Program.cs               # Camera + HTTP upload
        ├── packages.config
        └── README.md
```

## Dependencies

### .NET Web API
- .NET 9 SDK
- ASP.NET Core

### nanoFramework ESP32-CAM Client
- nanoFramework.CoreLibrary (1.17.11)
- nanoFramework.Hardware.Esp32.Camera (1.0.1)
- nanoFramework.System.Net.Http (1.5.150)
- nanoFramework.Runtime.Events (1.11.21)
- nanoFramework.System.Device.Wifi (1.5.91)

## License

Sample project for demonstration and educational purposes.

## Support

For issues or questions:
- Check the README files in each project
- Review the debug output
- Verify all configuration values
- Check network connectivity

## Credits

Built with:
- [nanoFramework](https://www.nanoframework.net/)
- [.NET 9](https://dotnet.microsoft.com/)
- [ESP32-CAM](https://github.com/espressif/esp32-camera)
