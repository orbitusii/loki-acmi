using loki_geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    
}
