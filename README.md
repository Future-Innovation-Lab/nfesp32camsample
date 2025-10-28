# ESP32-CAM nanoFramework Firmware

Custom nanoFramework firmware for ESP32-CAM (AI-Thinker) with camera and SD card support.

> **⚠️ EDUCATIONAL USE ONLY**  
> This firmware and documentation are provided for educational and learning purposes only. Use at your own risk.

## Features

- ✅ ESP32 camera support (OV2640 sensor)
- ✅ PSRAM support (2MB allocated for framebuffers)
- ✅ WiFi and networking

## Hardware Requirements

- ESP32-CAM AI-Thinker board
- USB-to-Serial adapter (FTDI, CP2102, etc.) for flashing
- USB cable

## Prerequisites

### Install .NET nanoFramework Flash Tool

```bash
dotnet tool install -g nanoff
```

If already installed, update to latest version:
```bash
dotnet tool update -g nanoff
```

### Install Visual Studio 2022 with .NET nanoFramework Extension

1. Install Visual Studio 2022 (Community Edition or higher)
2. In Visual Studio, go to Extensions → Manage Extensions
3. Search for ".NET nanoFramework"
4. Install the extension and restart Visual Studio

## Flashing Firmware to ESP32-CAM

### Windows

1. **Connect ESP32-CAM to PC**
   - Connect ESP32-CAM to USB-to-Serial adapter:
     - ESP32-CAM GND → Adapter GND
     - ESP32-CAM 5V → Adapter 5V (or 3.3V if no 5V available)
     - ESP32-CAM U0R (GPIO3) → Adapter TX
     - ESP32-CAM U0T (GPIO1) → Adapter RX
     - ESP32-CAM IO0 → GND (for programming mode)
   - Connect adapter to PC USB port

2. **Find COM Port**
   - Open Device Manager
   - Look under "Ports (COM & LPT)"
   - Note the COM port number (e.g., COM5)

3. **Flash Firmware**
   
   Run the flash script with your COM port:
   ```cmd
   cd firmware
   flash.bat COM5
   ```
   Replace `COM5` with your actual COM port.

4. **Disconnect IO0 from GND and press RESET button**

## Verifying Installation

1. **In Visual Studio:**
   - Go to View → Other Windows → Device Explorer
   - You should see "ESP32_CAM_PSRAM" device listed
   - Click to connect

2. **Check Device Information:**
   - In Device Explorer, right-click device → Device Capabilities
   - Should show firmware version and available APIs

## Running the Sample Application

### Prerequisites

1. **Have nanoFramework.Hardware.Esp32.Camera library available**

### Setup

See the [samples documentation](sample/README.md) for detailed setup and usage instructions.

## Camera Configuration

The default configuration uses these pins (ESP32-CAM AI-Thinker standard):

```csharp
CameraConfig.CreateDefault() returns:
- PWDN: GPIO32
- RESET: GPIO-1 (not used)
- XCLK: GPIO0
- SIOD (SDA): GPIO26
- SIOC (SCL): GPIO27
- Data pins: D7=GPIO35, D6=GPIO34, D5=GPIO39, D4=GPIO36, 
             D3=GPIO21, D2=GPIO19, D1=GPIO18, D0=GPIO5
- VSYNC: GPIO25
- HREF: GPIO23
- PCLK: GPIO22
- XCLK Frequency: 20 MHz
- Pixel Format: JPEG
- Frame Size: SVGA (800x600)
- JPEG Quality: 12 (lower = better quality, range 0-63)
- Frame Buffers: 1
```

### Customizing Camera Settings

```csharp
var config = CameraConfig.CreateDefault();
config.frameSize = (int)FrameSize.VGA;     // 640x480
config.jpegQuality = 10;                    // Better quality
config.fbCount = 2;                         // Double buffering

camera.Initialize(config);
```

Available frame sizes: QVGA, VGA, SVGA, XGA, SXGA, UXGA

## Troubleshooting

### Firmware Won't Flash
- **Solution:** Ensure IO0 is connected to GND during power-on
- Try lower baud rate: `--baud 115200` instead of 921600
- Check USB cable and connections
- Try different USB port

### "Device not found" in Visual Studio
- **Solution:** 
  - Disconnect and reconnect ESP32-CAM
  - Press RESET button on ESP32-CAM
  - Check COM port in Device Manager
  - Ensure firmware was flashed successfully

### Camera Initialization Fails
- **Solution:**
  - Check camera ribbon cable is properly connected
  - Verify PSRAM is working (some ESP32-CAM boards have bad PSRAM)
  - Power supply must provide stable 5V with sufficient current (500mA+)

### SD Card Mount Fails
- **Solution:**
  - Ensure SD card is formatted as FAT32
  - Check card is not write-protected
  - Use smaller SD cards (≤32GB) for better compatibility
  - Try reformatting card
  - Some SD cards are incompatible - try a different card

### Deployment Hangs on "Connecting to debugger"
- **Solution:** 
  - Press RESET button on ESP32-CAM
  - Disconnect and reconnect device
  - Power supply must provide stable 5V with sufficient current (500mA+)

## Support

For issues with:
- **nanoFramework:** https://github.com/nanoframework/Home/discussions
- **ESP32-CAM Hardware:** ESP32-CAM community forums
- **This firmware:** Check camera library repository

## License

This firmware is based on nanoFramework which is licensed under MIT License.
Camera library components may have separate licensing.

## Credits

Built with:
- nanoFramework v1.0
- ESP-IDF v5.4.2
- esp32-camera component
- FatFS for SD card support
