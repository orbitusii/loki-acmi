using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace acmi_interpreter;

/// <summary>
/// Events can be used to inject any kind of text, bookmark and debug information into the flight recording. They are a bit special: They are declared like properties, but unlike properties, you can declare several events in the same frame without overriding the previous one.
/// </summary>
public class ACMIEventArgs : EventArgs
{
    public ACMIEventType EventType { get; init; }
    /// <summary>
    /// The objects affected by this event. May be none.
    /// </summary>
    public ulong[] ObjectIDs { get; init; } = Array.Empty<ulong>();
    /// <summary>
    /// Free text that describes the event.
    /// </summary>
    public string EventText { get; init; } = string.Empty;

    public static ACMIEventArgs FromRawMessage(string data)
    {
        throw new NotImplementedException();
    }

    public static ACMIEventArgs Message(ulong[] objects, string eventText) =>
        new()
        {
            EventType = ACMIEventType.Message,
            ObjectIDs = objects,
            EventText = eventText
        };

    public static ACMIEventArgs Bookmark(ulong[] objects, string eventText) =>
        new()
        {
            EventType = ACMIEventType.Bookmark,
            ObjectIDs = objects,
            EventText = eventText
        };

    public static ACMIEventArgs Debug(ulong[] objects, string eventText) =>
        new()
        {
            EventType = ACMIEventType.Bookmark,
            ObjectIDs = objects,
            EventText = eventText
        };

    public static ACMIEventArgs LeftArea(ulong[] objects, string eventText) =>
        new()
        {
            EventType = ACMIEventType.LeftArea,
            ObjectIDs = objects,
            EventText = eventText
        };

    public static ACMIEventArgs Destroyed(ulong[] objects, string eventText) =>
        new()
        {
            EventType = ACMIEventType.Destroyed,
            ObjectIDs = objects,
            EventText = eventText
        };

    public static ACMIEventArgs TakenOff(ulong[] objects, string eventText) =>
        new()
        {
            EventType = ACMIEventType.TakenOff,
            ObjectIDs = objects,
            EventText = eventText
        };

    public static ACMIEventArgs Landed(ulong[] objects, string eventText) =>
        new()
        {
            EventType = ACMIEventType.Landed,
            ObjectIDs = objects,
            EventText = eventText
        };

    public static ACMIEventArgs Timeout(ulong[] objects, string eventText) =>
        new()
        {
            EventType = ACMIEventType.Timeout,
            ObjectIDs = objects,
            EventText = eventText
        };
}

public enum ACMIEventType
{
    /// <summary>
    /// Generic event.
    /// </summary>
    Message,
    /// <summary>
    /// Bookmarks are highlighted in the time line and in the event log. They are easy to spot and handy to highlight parts of the flight, like a bombing run, or when the trainee was in her final approach for landing.
    /// </summary>
    Bookmark,
    /// <summary>
    /// Debug events are highlighted and easy to spot in the timeline and event log. Because they must be used for development purposes, they are displayed only when launching Tacview with the command line argument /Debug:on
    /// </summary>
    Debug,
    /// <summary>
    /// This event is useful to specify when an aircraft (or any object) is cleanly removed from the battlefield (not destroyed). This prevents Tacview from generating a <see cref="Destroyed"/> event by error.
    /// </summary>
    LeftArea,
    /// <summary>
    /// When an object has been officially destroyed.
    /// </summary>
    Destroyed,
    /// <summary>
    /// Because Tacview may not always properly auto-detect take-off events, it can be useful to manually inject this event in the flight recording.
    /// </summary>
    TakenOff,
    /// <summary>
    /// Because Tacview may not always properly auto-detect landing events, it can be useful to manually inject this event in the flight recording.
    /// </summary>
    Landed,
    /// <summary>
    /// Mainly used for real-life training debriefing to specify when a weapon (typically a missile) reaches or misses its target. Tacview will report in the shot log as well as in the 3D view the result of the shot.
    /// </summary>
    Timeout,
}
