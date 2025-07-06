using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;

namespace PrintBridgeTrayApp;

public class PrintService
{
    private const int DPI = 203; // DPI for real-world size calculation

    public async Task<PrintResult> PrintImageAsync(string base64Image, string? printerName = null)
    {
        try
        {
            // Decode base64 image
            var imageBytes = Convert.FromBase64String(base64Image);
            using var image = Image.FromStream(new MemoryStream(imageBytes));
            
            // Save temporarily
            var tempPath = Path.GetTempFileName() + ".png";
            image.Save(tempPath, ImageFormat.Png);
            
            Console.WriteLine($"Decoded image: {image.Width}x{image.Height} pixels");
            
            // Calculate real-world size at 203 DPI
            var widthInches = image.Width / (float)DPI;
            var heightInches = image.Height / (float)DPI;
            
            Console.WriteLine($"Real-world size: {widthInches:F2}\" x {heightInches:F2}\"");
            
            // Print the image
            var success = await PrintImageToPrinterAsync(tempPath, printerName);
            
            // Clean up temp file
            try
            {
                File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not delete temp file: {ex.Message}");
            }
            
            return new PrintResult
            {
                Success = success,
                PrinterName = printerName ?? GetDefaultPrinter(),
                ErrorMessage = success ? null : "Print operation failed"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Print error: {ex.Message}");
            return new PrintResult
            {
                Success = false,
                PrinterName = printerName ?? GetDefaultPrinter(),
                ErrorMessage = ex.Message
            };
        }
    }

    private async Task<bool> PrintImageToPrinterAsync(string imagePath, string? printerName)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var printDocument = new PrintDocument();
                
                // Set printer
                if (!string.IsNullOrEmpty(printerName))
                {
                    printDocument.PrinterSettings.PrinterName = printerName;
                }
                
                // Verify printer exists
                if (!printDocument.PrinterSettings.IsValid)
                {
                    Console.WriteLine($"Invalid printer: {printerName ?? "default"}");
                    return false;
                }
                
                Console.WriteLine($"Printing to: {printDocument.PrinterSettings.PrinterName}");
                
                // Set paper size to custom label (1.25"x2.25" or 31mm x 56mm)
                foreach (PaperSize ps in printDocument.PrinterSettings.PaperSizes)
                {
                    if ((ps.PaperName.Contains("1.25") && ps.PaperName.Contains("2.25")) ||
                        (ps.PaperName.Contains("56") && ps.PaperName.Contains("31")))
                    {
                        printDocument.DefaultPageSettings.PaperSize = ps;
                        printDocument.PrinterSettings.DefaultPageSettings.PaperSize = ps;
                        Console.WriteLine($"Force set paper size to: {ps.PaperName} ({ps.Width}x{ps.Height})");
                        break;
                    }
                }
                // Set margins to zero
                printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                
                // Set up print event handlers
                printDocument.PrintPage += (sender, e) =>
                {
                    using var image = Image.FromFile(imagePath);

                    // For thermal printers, use exact pixel dimensions (no scaling)
                    var imageWidth = image.Width;
                    var imageHeight = image.Height;

                    // Draw the image at original size, top-left aligned
                    e.Graphics.DrawImage(image, 0, 0, imageWidth, imageHeight);

                    Console.WriteLine($"Drew image at (0, 0) with size {imageWidth}x{imageHeight}");
                };
                
                // Print silently
                printDocument.Print();
                
                Console.WriteLine("Print job completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Print operation failed: {ex.Message}");
                return false;
            }
        });
    }

    public List<string> GetAvailablePrinters()
    {
        var printers = new List<string>();
        
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            printers.Add(printer);
        }
        
        Console.WriteLine($"Found {printers.Count} printers");
        return printers;
    }

    public string GetDefaultPrinter()
    {
        try
        {
            using var printDocument = new PrintDocument();
            var defaultPrinter = printDocument.PrinterSettings.PrinterName;
            Console.WriteLine($"Default printer: {defaultPrinter}");
            return defaultPrinter ?? "Unknown";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting default printer: {ex.Message}");
            return "Unknown";
        }
    }
}

public class PrintResult
{
    public bool Success { get; set; }
    public string PrinterName { get; set; } = "";
    public string? ErrorMessage { get; set; }
} 