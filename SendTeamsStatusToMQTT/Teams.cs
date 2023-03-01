// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class TeamsStatus
{
    private static string logsPath = Environment.ExpandEnvironmentVariables(@"%appdata%\Microsoft\Teams\logs.txt");

    public DateTime Time { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public Status CurrentStatus { get; set; }

    public TeamsStatus(string logLine)
    {
        ParseLogLine(logLine);
    }

    private void ParseLogLine(string logLine)
    {
        // Extract datetime
        string dt = logLine.Substring(0, logLine.IndexOf("GMT") - 1);
        Time = DateTime.Parse(dt);

        // Extract status
        var start = logLine.IndexOf("Added ") + 6;
        var end = logLine.IndexOf(" (current state");
        string status = logLine.Substring(start, end - start);

        CurrentStatus = (Status)Enum.Parse(typeof(Status), status);
    }

    public static List<TeamsStatus> GetTeamsStatusHistory()
    {
        var statusUpdates = new List<TeamsStatus>();

        using (var logStream = new FileStream(logsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (StreamReader logsReader = new(logStream))
        {
            while (logsReader.Peek() >= 0) // reading the old data
            {
                string currentLine = logsReader.ReadLine() ?? "";

                // Cut out garbage
                if (currentLine.Contains("Removing")) continue;
                if (currentLine.Contains("ConnectionError")) continue;
                if (currentLine.Contains("Added NewActivity")) continue;
                if (currentLine.Contains("NoNetwork")) continue;

                // Add the good stuff
                if (currentLine.Contains("(current state: ")) statusUpdates.Add(new TeamsStatus(currentLine));
            }
        }

        return statusUpdates;
    }
    public static TeamsStatus GetCurrentStatus(IEnumerable<TeamsStatus> statusList)
    {
        return statusList.Last();
    }
    public static TeamsStatus GetCurrentStatus()
    {
        var history = GetTeamsStatusHistory();
        return GetCurrentStatus(history);
    }

    public override string ToString()
    {
        return $"{Time}: {CurrentStatus}";
    }
}

public enum Status
{
    Offline,
    Available,
    Away,
    Busy,
    OnThePhone,
    Presenting,
    DoNotDisturb,
    InAMeeting,
    BeRightBack,
    Unknown
}