using nanoFramework.Hardware.Esp32.Camera;
using nanoFramework.System.IO.FileSystem;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CameraTest
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Starting ESP32-CAM with SD card...");

            // ESP32-CAM uses SDIO/MMC interface with 1-bit mode
            // The pins are fixed by hardware (GPIO14=CLK, GPIO15=CMD, GPIO2=D0)
            var sdCard = new SDCard(new SDCardMmcParameters
            {
                dataWidth = SDCard.SDDataWidth._1_bit
            });

            try
            {
                sdCard.Mount();
                Debug.WriteLine("SD card mounted at D:\\");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to mount SD card: {ex.Message}");
                Debug.WriteLine("Make sure:");
                Debug.WriteLine("- SD card is inserted");
                Debug.WriteLine("- SD card is formatted as FAT32");
                Debug.WriteLine("- Card is not locked/write-protected");
                return;
            }

            // Initialize camera
            var camera = new Camera();
            var config = CameraConfig.CreateDefault();

            if (!camera.Initialize(config))
            {
                Debug.WriteLine("Failed to initialize camera!");
                return;
            }

            Debug.WriteLine("Camera initialized!");

            try
            {
                // Capture and save images
                for (int i = 1; i <= 5; i++)
                {
                    Debug.WriteLine($"\nCapturing image {i}...");
                    byte[] imageData = camera.CaptureImage();

                    if (imageData != null && imageData.Length > 0)
                    {
                        Debug.WriteLine($"Image captured: {imageData.Length} bytes");

                        // Save to SD card
                        string filename = $"D:\\image{i:D3}.jpg";
                        File.WriteAllBytes(filename, imageData);

                        Debug.WriteLine($"Saved to {filename}");
                    }

                    System.Threading.Thread.Sleep(1000);
                }

                // List files
                Debug.WriteLine("\nFiles on SD card:");
                string[] files = Directory.GetFiles("D:\\");
                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    Debug.WriteLine($"  {file} - {info.Length} bytes");
                }
            }
            finally
            {
                camera.Dispose();
                sdCard.Unmount();
                Debug.WriteLine("Done!");
            }

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}