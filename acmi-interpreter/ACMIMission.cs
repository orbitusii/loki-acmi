using loki_geo;

namespace acmi_interpreter;

/// <summary>
/// A container representing an ACMI file, based on the file format specification outlined at <a href="https://www.tacview.net/documentation/acmi/"/>
/// </summary>
public class ACMIMission
{
    /// <summary>
    /// The type of data format to use. Generally "text/acmi/tacview".
    /// <br/>Sent via the message or file header.
    /// </summary>
    public string FileType { get; set; } = string.Empty;
    /// <summary>
    /// The version of <see cref="FileType"/>, defining specific characters and implementations.<br/>
    /// For this plugin, only ACMI 2.2 is really supported. Others may work, but it's not expected.
    /// <br/>Sent via the message or file header.
    /// </summary>
    public Version FileVersion { get; set; } = new("0.0");

    /// <summary>
    /// Source simulator, control station or file format.
    /// </summary>
    public string DataSource { get; set; } = string.Empty;
    /// <summary>
    /// Software or hardware used to record the data.
    /// </summary>
    public string DataRecorder { get; set; } = string.Empty;
    /// <summary>
    /// Author or operator who has created this recording.
    /// </summary>
    public string Author {  get; set; } = string.Empty;
    /// <summary>
    /// Mission/flight title or designation.
    /// </summary>
    public string Title {  get; set; } = string.Empty;
    /// <summary>
    /// Category of the flight/mission.
    /// </summary>
    public string Category { get; set; } = string.Empty;
    /// <summary>
    /// Free text containing the briefing of the flight/mission.
    /// </summary>
    public string Briefing { get; set; } = string.Empty;
    /// <summary>
    /// Free text containing the debriefing.
    /// </summary>
    public string Debriefing { get; set; } = string.Empty;
    /// <summary>
    /// Free comments about the flight. May contain escaped end-of-line characters.
    /// </summary>
    public string Comments { get; set; } = string.Empty;

    /// <summary>
    /// Recording (file) creation (UTC) time.
    /// </summary>
    public DateTime RecordingTime;
    /// <summary>
    /// Base time (UTC) for the current mission. This time is combined with each frame offset (in seconds) to get the final absolute UTC time for each data sample.
    /// </summary>
    public DateTime ReferenceTime;

    /// <summary>
    /// Used to reduce the file size by centering coordinates around a median point. Will be added to each object Longitude and Latitude to get the final coordinates.
    /// </summary>
    public LatLonCoord ReferencePosition { get; set; } = new LatLonCoord() { Lat_Degrees = 0, Lon_Degrees = 0, Alt = 0 };

    /// <summary>
    /// Invoked when ANY event is logged.
    /// </summary>
    public event EventHandler<ACMIEventArgs>? OnAnyEvent;
    /// <summary>
    /// Invoked when a Message (or Generic) event is logged.
    /// </summary>
    public event EventHandler<ACMIEventArgs>? OnGenericEvent;
    /// <summary>
    /// Invoked when a Bookmark event is logged.
    /// </summary>
    public event EventHandler<ACMIEventArgs>? OnBookmarkEvent;
    /// <summary>
    /// Invoked when a Debug event is logged. Requires Tacview to be in Debug mode.
    /// </summary>
    public event EventHandler<ACMIEventArgs>? OnDebugEvent;
    /// <summary>
    /// Invoked when an object leaves the area without being destroyed.
    /// </summary>
    public event EventHandler<ACMIEventArgs>? OnObjectLeftArea;
    /// <summary>
    /// Invoked when an object is destroyed.
    /// </summary>
    public event EventHandler<ACMIEventArgs>? OnObjectDestroyed;
    /// <summary>
    /// Invoked when an object takes off and begins flight.
    /// </summary>
    public event EventHandler<ACMIEventArgs>? OnObjectTakeoff;
    /// <summary>
    /// Invoked when an object lands.
    /// </summary>
    public event EventHandler<ACMIEventArgs>? OnObjectLand;
    /// <summary>
    /// Invoked when a weapon times out, e.g. it reaches or misses the target.
    /// </summary>
    public event EventHandler<ACMIEventArgs>? OnWeaponTimeout;

    /// <summary>
    /// Invokes any event handlers subscribed to the associated Event Types.
    /// </summary>
    /// <param name="args"></param>
    protected virtual void RaiseEvent(ACMIEventArgs args)
    {
        var raiseAny = OnAnyEvent;
        if (raiseAny is not null) raiseAny(this, args);

        var raiseSpecific = args.EventType switch
        {
            ACMIEventType.Bookmark => OnBookmarkEvent,
            ACMIEventType.Debug => OnDebugEvent,
            ACMIEventType.LeftArea => OnObjectLeftArea,
            ACMIEventType.Destroyed => OnObjectDestroyed,
            ACMIEventType.TakenOff => OnObjectTakeoff,
            ACMIEventType.Landed => OnObjectLand,
            ACMIEventType.Timeout => OnWeaponTimeout,
            _ => OnGenericEvent,
        };
        if (raiseSpecific is not null) raiseSpecific(this, args);
    }

    /// <summary>
    /// The current Mission time, in seconds.
    /// </summary>
    public float CurrentFrame { get; private set; } = 0f;
    public DateTime CurrentTime_UTC => ReferenceTime + TimeSpan.FromSeconds(CurrentFrame);

    /// <summary>
    /// A collection containing all of the objects in this mission.
    /// </summary>
    public Dictionary<ulong, ACMIObject> Objects { get; private set; } = new Dictionary<ulong, ACMIObject>();

    public void UpdateWithData(string[] Lines)
    {
        if (Lines[0].StartsWith('#'))
        {
            float newTime = float.Parse(Lines[0].Remove(0, 1));
            CurrentFrame = newTime;
        }
        foreach(var line in Lines)
        {
            if (line.StartsWith("//") || line.StartsWith("#")) continue;
            else if(line.StartsWith("FileType=") || line.StartsWith("FileVersion="))
            {
                Console.WriteLine(line);
                continue;
            }

            ACMIMessage message = new(line);
            if(message.IsGlobal)
            {
                // Update global stuff
            }
            else if (Objects.ContainsKey(message.ObjectID))
            {
                var obj = Objects[message.ObjectID];

                obj.UpdateFrom(message);
                if (message.IsDestroyed) obj.Destroyed = true;
            }
            else
            {
                var newObj = new ACMIObject(message.ObjectID);
                newObj.UpdateFrom(message);

                Objects[message.ObjectID] = newObj;
            }

            if(message.IsEvent)
            {
                RaiseEvent(new() { EventType = ACMIEventType.Message, EventText = message.BareText });
            }
        }
    }
}
