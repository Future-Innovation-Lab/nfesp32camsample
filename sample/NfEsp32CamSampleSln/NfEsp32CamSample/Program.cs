using System;
using System.IO;
using System.Diagnostics;
using nanoFramework.Hardware.Esp32.Camera;
using nanoFramework.System.IO.FileSystem;

namespace CameraTest
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Starting ESP32-CAM test...");

            // Try to mount SD card with retry logic (handles busy state from previous session)
            var sdCard = new SDCard(new SDCardMmcParameters { dataWidth = SDCard.SDDataWidth._1_bit });

            int retries = 3;
            bool mounted = false;

            for (int i = 0; i < retries && !mounted; i++)
            {
                try
                {
                    if (i > 0)
                    {
                        Debug.WriteLine($"SD mount retry {i}/{retries - 1}...");
                        System.Threading.Thread.Sleep(2000);  // Wait for card to finish previous operations
                    }

                    sdCard.Mount();
                    mounted = true;
                    Debug.WriteLine("SD card mounted at D:\\");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Mount attempt {i + 1} failed: {ex.Message}");
                }
            }

            if (!mounted)
            {
                Debug.WriteLine("Failed to mount SD card after retries");
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

            // Generate unique filename prefix for this session
            DateTime bootTime = DateTime.UtcNow;
            int imageCounter = 0;

            try
            {
                // Capture images
                for (int i = 0; i < 5; i++)
                {
                    Debug.WriteLine($"\nCapturing image {i + 1}/5...");

                    byte[] imageData = camera.CaptureImage();

                    if (imageData != null && imageData.Length > 0)
                    {
                        Debug.WriteLine($"Image captured: {imageData.Length} bytes");

                        // Verify JPEG header
                        if (imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF)
                        {
                            Debug.WriteLine("Valid JPEG image");
                        }

                        // Create unique filename: YYYYMMDD_HHMMSS_NNN.jpg
                        string filename = $"D:\\{bootTime:yyyyMMdd_HHmmss}_{imageCounter++:D3}.jpg";

                        try
                        {
                            File.WriteAllBytes(filename, imageData);
                            Debug.WriteLine($"Saved to {filename}");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to save image: {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Failed to capture image");
                    }

                    System.Threading.Thread.Sleep(1000);
                }

                // List all images on SD card
                Debug.WriteLine("\nAll images on SD card:");
                try
                {
                    string[] files = Directory.GetFiles("D:\\");
                    foreach (string file in files)
                    {
                        FileInfo info = new FileInfo(file);
                        Debug.WriteLine($"  {file} - {info.Length} bytes");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to list files: {ex.Message}");
                }
            }
            finally
            {
                // Always clean up properly
                camera.Dispose();
                Debug.WriteLine("Camera disposed");

                if (sdCard.IsMounted)
                {
                    try
                    {
                        sdCard.Unmount();
                        Debug.WriteLine("SD card unmounted");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error unmounting SD card: {ex.Message}");
                    }
                }
            }

            Debug.WriteLine("\nTest complete!");
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}