# Quick Reference Card

## 🚀 Quick Commands

### Start Web API
```powershell
cd f:\work\experiments\espcamdeploy\sample\CamApiSample
dotnet run
```

### Get Your PC IP
```powershell
ipconfig | findstr IPv4
```

### Test API
```powershell
# List images
curl http://localhost:5000/api/Image/list

# Upload test image
curl -X POST http://localhost:5000/api/Image/upload -H "Content-Type: image/jpeg" --data-binary "@test.jpg"
```

### View Uploaded Images
```powershell
cd f:\work\experiments\espcamdeploy\sample\CamApiSample\Images
dir
```

---

## 📝 Configuration Checklist

### ESP32-CAM Program.cs
```csharp
WIFI_SSID = "YourWiFi"           // ❌ Must change
WIFI_PASSWORD = "YourPassword"    // ❌ Must change  
API_URL = "http://192.168.1.100:5000/api/Image/upload"  // ❌ Must change
CAPTURE_INTERVAL = 5000           // ✅ Optional
```

### Web API appsettings.json
```json
{
  "ImageStoragePath": "Images"    // ✅ Optional
}
```

---

## 🔧 Common Operations

### Firewall Rule (Windows - Run as Admin)
```powershell
New-NetFirewallRule -DisplayName "CamAPI Allow 5000" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

### Flash ESP32-CAM Firmware
```powershell
dotnet tool install -g nanoff
nanoff --target ESP32_CAM --serialport COM3 --update
```

### Restore NuGet Packages
```powershell
# In Visual Studio
Right-click project → Restore NuGet Packages
```

---

## 📊 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Image/upload` | Upload JPEG image |
| GET | `/api/Image/list` | List all uploaded images |

### Upload Image
```http
POST http://localhost:5000/api/Image/upload
Content-Type: image/jpeg

[Binary JPEG data]
```

Response:
```json
{
  "message": "Image uploaded successfully",
  "filename": "img_20251027_123045_abc123.jpg",
  "size": 45678,
  "path": "F:\\...\\Images\\img_20251027_123045_abc123.jpg"
}
```

### List Images
```http
GET http://localhost:5000/api/Image/list
```

Response:
```json
{
  "count": 5,
  "images": [
    {
      "filename": "img_20251027_123045_abc123.jpg",
      "size": 45678,
      "created": "2025-10-27T12:30:45Z"
    }
  ]
}
```

---

## 🐛 Quick Troubleshooting

| Problem | Solution |
|---------|----------|
| WiFi won't connect | Check SSID/password, use 2.4GHz network |
| Port 5000 in use | Change port in launchSettings.json |
| Camera init fails | Press RESET button, check firmware |
| Upload fails | Check firewall, verify IP address |
| Device not detected | Install USB drivers, try different cable |
| Out of memory | Reduce resolution, increase interval |

---

## 📱 Camera Resolutions

| Size | Resolution | File Size | Speed |
|------|-----------|-----------|-------|
| QQVGA | 160x120 | ~3KB | Fastest |
| QVGA | 320x240 | ~10KB | Fast |
| VGA | 640x480 | ~30KB | Medium |
| SVGA | 800x600 | ~50KB | Medium (Default) |
| XGA | 1024x768 | ~80KB | Slow |
| UXGA | 1600x1200 | ~150KB | Slowest |

---

## 🎯 Default Settings

### Web API
- **Port**: 5000
- **Protocol**: HTTP
- **Storage**: `./Images/`
- **URL**: `http://0.0.0.0:5000`

### ESP32-CAM
- **Capture Interval**: 5000ms (5 seconds)
- **Resolution**: SVGA (800x600)
- **Format**: JPEG
- **Quality**: Default (10)

---

## 📦 Required Packages

### Web API (.NET 9)
- Built-in ASP.NET Core libraries

### ESP32-CAM (nanoFramework)
- nanoFramework.CoreLibrary (1.17.11)
- nanoFramework.Hardware.Esp32.Camera (1.0.1)
- nanoFramework.System.Net.Http (1.5.150)
- nanoFramework.Runtime.Events (1.11.21)
- nanoFramework.System.Device.Wifi (1.5.91)

---

## 🔗 Important File Locations

```
sample/
├── CamApiSample/
│   ├── Controllers/ImageController.cs   ← API logic
│   ├── Properties/launchSettings.json   ← Port config
│   ├── appsettings.json                 ← Storage path
│   └── Images/                          ← Uploaded images
│
└── NfEsp32CamSampleSln/
    └── NfEsp32CamApiSample/
        └── Program.cs                    ← WiFi & API config
```

---

## 💡 Pro Tips

1. **Always check IP address** before deploying to ESP32-CAM
2. **Test API locally first** with curl or Postman
3. **Monitor debug output** for troubleshooting
4. **Lower resolution** for faster performance
5. **Increase interval** to reduce network load
6. **Check firewall** if uploads fail
7. **Use 2.4GHz WiFi** (ESP32 requirement)
8. **Press RESET** after flashing firmware

---

## 📞 Quick Verification Steps

1. ✅ API running: `curl http://localhost:5000/api/Image/list`
2. ✅ Firewall open: Test from another device
3. ✅ WiFi configured: Check SSID/password in code
4. ✅ IP correct: Use `ipconfig`, update API_URL
5. ✅ Device detected: Check Device Explorer in VS
6. ✅ Packages restored: Build succeeds
7. ✅ Firmware flashed: Device connects and runs
8. ✅ Images uploading: Check debug output
9. ✅ Files saved: Check Images folder
10. ✅ API returns data: List endpoint shows images

---

## 🎉 Success Indicators

**Web API Console:**
```
Now listening on: http://0.0.0.0:5000
Application started.
```

**ESP32-CAM Debug Output:**
```
Connected to WiFi: YourNetwork
Camera initialized successfully!
Image captured: 45678 bytes
Image uploaded successfully!
```

**Images Folder:**
```
img_20251027_120000_abc123.jpg
img_20251027_120005_def456.jpg
img_20251027_120010_ghi789.jpg
```

---

Keep this card handy for quick reference! 🚀
