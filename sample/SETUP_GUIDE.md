# Setup and Deployment Guide

## Complete Setup Instructions for ESP32-CAM Image Upload System

This guide walks you through setting up both the .NET Web API and the ESP32-CAM client.

---

## Part 1: Setting Up the Web API

### Prerequisites
- .NET 9 SDK installed
- Windows, Linux, or macOS
- Administrator/elevated permissions for firewall configuration

### Step-by-Step Setup

#### 1. Navigate to the API Project
```powershell
cd f:\work\experiments\espcamdeploy\sample\CamApiSample
```

#### 2. Verify the Build
```powershell
dotnet build
```

Expected output: "Build succeeded"

#### 3. Run the API
```powershell
dotnet run
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

#### 4. Get Your PC's IP Address

**On Windows:**
```powershell
ipconfig
```

Look for "IPv4 Address" under your active network adapter (e.g., 192.168.1.100)

**On Linux/macOS:**
```bash
ip addr show
# or
ifconfig
```

#### 5. Configure Firewall (Windows)

Open PowerShell as Administrator and run:
```powershell
New-NetFirewallRule -DisplayName "CamAPI Allow 5000" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

Or manually:
1. Open Windows Defender Firewall
2. Click "Advanced settings"
3. Select "Inbound Rules"
4. Click "New Rule"
5. Select "Port" → Next
6. Select "TCP" and enter "5000" → Next
7. Select "Allow the connection" → Next
8. Select all profiles → Next
9. Name it "CamAPI Port 5000" → Finish

#### 6. Test the API Locally

```powershell
# List images (should return empty initially)
curl http://localhost:5000/api/Image/list
```

Expected response:
```json
{"count":0,"images":[]}
```

#### 7. Test from Network (Optional)

From another device on the same network:
```bash
curl http://YOUR_PC_IP:5000/api/Image/list
```

If this fails, check firewall settings.

---

## Part 2: Setting Up the ESP32-CAM Client

### Prerequisites
- Visual Studio 2022 or later
- nanoFramework Visual Studio extension installed
- ESP32-CAM board
- USB cable for programming
- WiFi network (2.4GHz)

### Step-by-Step Setup

#### 1. Install nanoFramework Extension

If not already installed:
1. Open Visual Studio
2. Go to Extensions → Manage Extensions
3. Search for "nanoFramework"
4. Install "nanoFramework VS Extension"
5. Restart Visual Studio

#### 2. Flash nanoFramework Firmware to ESP32-CAM

1. Download nanoff tool:
   ```powershell
   dotnet tool install -g nanoff
   ```

2. Connect ESP32-CAM to USB (you may need an FTDI adapter)

3. Put ESP32-CAM in bootloader mode:
   - Hold the IO0 button
   - Press and release the RESET button
   - Release IO0 button

4. Flash the firmware:
   ```powershell
   nanoff --target ESP32_CAM --serialport COM3 --update
   ```
   (Replace COM3 with your actual port)

5. Press RESET button after flashing

#### 3. Open the Solution

1. Navigate to: `f:\work\experiments\espcamdeploy\sample\NfEsp32CamSampleSln`
2. Open `NfEsp32CamSampleSln.sln` in Visual Studio

#### 4. Configure WiFi and API Settings

Open `NfEsp32CamApiSample\Program.cs` and update:

```csharp
// Replace with your WiFi credentials
private const string WIFI_SSID = "YourWiFiNetworkName";
private const string WIFI_PASSWORD = "YourWiFiPassword";

// Replace with your PC's IP address from Part 1, Step 4
private const string API_URL = "http://192.168.1.100:5000/api/Image/upload";

// Optional: Adjust capture interval (milliseconds)
private const int CAPTURE_INTERVAL = 5000; // 5 seconds
```

**Important Notes:**
- Ensure WiFi network is 2.4GHz (ESP32 doesn't support 5GHz)
- Use your PC's IP address, not "localhost"
- ESP32-CAM and PC must be on the same network

#### 5. Restore NuGet Packages

Right-click on the `NfEsp32CamApiSample` project → "Restore NuGet Packages"

Or use Package Manager Console:
```powershell
Update-Package -reinstall
```

#### 6. Connect ESP32-CAM

1. Connect ESP32-CAM via USB
2. Open Device Explorer (View → Other Windows → Device Explorer)
3. Wait for device to appear
4. If device doesn't appear, press RESET button

#### 7. Deploy to ESP32-CAM

1. Set `NfEsp32CamApiSample` as the startup project (right-click → Set as Startup Project)
2. Press F5 or click "Start Debugging"
3. Wait for build and deployment to complete

#### 8. Monitor Debug Output

Open the Output window (View → Output) and select "nanoFramework" from the dropdown.

Expected output:
```
ESP32-CAM API Sample Starting...
Connecting to WiFi...
Connected to WiFi: YourNetwork
IP Address: 192.168.1.50
Camera initialized successfully!

--- Capturing image 1 ---
Image captured: 45678 bytes
Sending 45678 bytes to http://192.168.1.100:5000/api/Image/upload...
Response: {"message":"Image uploaded successfully",...}
Image uploaded successfully!

--- Capturing image 2 ---
...
```

---

## Part 3: Verification

### 1. Check Uploaded Images

Navigate to the Images folder:
```powershell
cd f:\work\experiments\espcamdeploy\sample\CamApiSample\Images
dir
```

You should see JPEG files with timestamps.

### 2. View Images via API

```powershell
curl http://localhost:5000/api/Image/list
```

Response will show all uploaded images with metadata.

### 3. Open an Image

Open one of the uploaded images to verify it's a valid JPEG from the camera.

---

## Troubleshooting Guide

### Web API Issues

#### "Port 5000 is already in use"
```powershell
# Find what's using the port
netstat -ano | findstr :5000

# Kill the process (use PID from above)
taskkill /PID <PID> /F

# Or change the port in launchSettings.json
```

#### "Unable to bind to http://0.0.0.0:5000"
Run Visual Studio as Administrator or use localhost instead:
- Edit `Properties\launchSettings.json`
- Change `http://0.0.0.0:5000` to `http://localhost:5000`
- Update ESP32-CAM to use your specific IP

#### Images folder permission denied
- Run Visual Studio as Administrator
- Or change `ImageStoragePath` in `appsettings.json` to a writable location

### ESP32-CAM Issues

#### "Failed to connect to WiFi"
- Verify SSID and password are correct (case-sensitive)
- Ensure network is 2.4GHz, not 5GHz
- Check WiFi signal strength at ESP32-CAM location
- Try connecting to a different network
- Check router settings (some routers block IoT devices)

#### "Failed to initialize camera"
- Press RESET button on ESP32-CAM
- Check camera ribbon cable connection
- Verify ESP32-CAM board is genuine
- Re-flash nanoFramework firmware

#### "HTTP Error: 404" or "Connection Refused"
- Verify Web API is running
- Check IP address is correct (use `ipconfig`)
- Ensure firewall allows port 5000
- Test connectivity: `ping YOUR_PC_IP` from another device
- Verify both devices on same network/subnet

#### "Device not detected in Visual Studio"
- Install CP2102 or CH340 USB drivers (depending on your board)
- Try a different USB cable (some are power-only)
- Press RESET button
- Check Device Manager for COM ports
- Try a different USB port

#### "Out of memory" errors
- Reduce image resolution in camera config
- Increase capture interval
- Reset the device

### Network Connectivity Issues

#### ESP32-CAM and PC can't communicate
```powershell
# On PC, find your IP
ipconfig

# Verify ESP32-CAM can reach PC
# Check Debug output for ESP32-CAM's IP address
# From another PC on the network:
ping ESP32_CAM_IP
```

#### Intermittent uploads
- Check WiFi signal strength
- Reduce capture frequency
- Check router bandwidth/load
- Implement retry logic (already included)

---

## Advanced Configuration

### Adjust Image Quality

In `NfEsp32CamApiSample\Program.cs`, modify camera config:

```csharp
var config = CameraConfig.CreateDefault();
config.FrameSize = FrameSize.SVGA;  // Resolution
config.JpegQuality = 10;             // Quality (0-63, lower = better)
config.PixelFormat = PixelFormat.JPEG;

camera.Initialize(config);
```

Available resolutions:
- `QQVGA` - 160x120
- `QVGA` - 320x240
- `VGA` - 640x480
- `SVGA` - 800x600 (default)
- `XGA` - 1024x768
- `UXGA` - 1600x1200

### Change Storage Location

Edit `CamApiSample\appsettings.json`:

```json
{
  "ImageStoragePath": "C:\\MyImages"
}
```

### Add Authentication

In `ImageController.cs`, add an API key check:

```csharp
[HttpPost("upload")]
public async Task<IActionResult> UploadImage([FromHeader] string apiKey)
{
    if (apiKey != "your-secret-key")
    {
        return Unauthorized();
    }
    // ... rest of code
}
```

Update ESP32-CAM client to send API key in header.

---

## Performance Optimization

### For ESP32-CAM
- Lower resolution for faster captures
- Increase interval to reduce network load
- Adjust JPEG quality for balance

### For Web API
- Add image compression
- Implement async file operations (already done)
- Add response caching for list endpoint
- Consider database for metadata

---

## Next Steps

1. ✅ Web API running and accessible
2. ✅ ESP32-CAM capturing and uploading images
3. ✅ Images visible in filesystem

Optional enhancements:
- [ ] Add web interface for viewing images
- [ ] Implement image gallery
- [ ] Add real-time notifications
- [ ] Set up cloud storage
- [ ] Add motion detection
- [ ] Create time-lapse functionality

---

## Getting Help

If you encounter issues:
1. Check this troubleshooting guide
2. Review debug output from both applications
3. Verify network connectivity
4. Check firewall settings
5. Ensure all configuration values are correct

## Success Checklist

- [ ] .NET 9 SDK installed
- [ ] Web API builds successfully
- [ ] Web API running on port 5000
- [ ] Firewall configured to allow port 5000
- [ ] PC IP address identified
- [ ] nanoFramework extension installed
- [ ] ESP32-CAM firmware flashed
- [ ] WiFi credentials configured
- [ ] API URL configured with PC IP
- [ ] NuGet packages restored
- [ ] ESP32-CAM connected and detected
- [ ] Application deployed to ESP32-CAM
- [ ] Debug output shows successful uploads
- [ ] Images appearing in filesystem
- [ ] API list endpoint returns uploaded images

---

Congratulations! Your ESP32-CAM image upload system is now operational! 📷✨
