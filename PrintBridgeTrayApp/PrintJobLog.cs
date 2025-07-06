using System.Text.Json;

namespace PrintBridgeTrayApp;

public class PrintJobLog
{
    private readonly List<PrintJob> jobs = new();
    private readonly object lockObject = new();

    public void AddJob(PrintJob job)
    {
        lock (lockObject)
        {
            jobs.Add(job);
            
            // Keep only the last 100 jobs to prevent memory issues
            if (jobs.Count > 100)
            {
                jobs.RemoveAt(0);
            }
            
            Console.WriteLine($"Job logged: {job.Timestamp:yyyy-MM-dd HH:mm:ss} - {job.PrinterName} - {(job.Success ? "Success" : "Error")}");
        }
    }

    public List<PrintJob> GetJobs()
    {
        lock (lockObject)
        {
            return jobs.ToList();
        }
    }

    public void ClearJobs()
    {
        lock (lockObject)
        {
            jobs.Clear();
            Console.WriteLine("Print job log cleared");
        }
    }
}

public class PrintJob
{
    public DateTime Timestamp { get; set; }
    public string PrinterName { get; set; } = "";
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
} 