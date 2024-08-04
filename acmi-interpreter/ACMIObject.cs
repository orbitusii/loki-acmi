using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace acmi_interpreter;

/// <summary>
/// A real-time object from an ACMI source.
/// </summary>
public class ACMIObject
{
    public ulong ObjectID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    [ACMIObjectProperty("Parent", typeof(ulong))]
    public ulong ParentID { get; set; }
    public ulong Next {  get; set; }
    /// <summary>
    /// This abbreviated name will be displayed in the 3D view and in any other cases with small space to display the object name. Typically defined in Tacview database. Should not be defined in telemetry data. 
    /// </summary>
    public string ShortName { get; set; } = string.Empty;
    /// <summary>
    /// More detailed object name, used in small windows. Should not be defined in telemetry data.
    /// </summary>
    public string LongName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public string CallSign { get; set; } = string.Empty;
    public string Registration { get; set; } = string.Empty;

    public int Squawk { get; set; }
    public string ICAO24 { get; set; } = string.Empty;

    public string Pilot { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    /// <summary>
    /// ISO 3166-1 alpha-2 country code. See <a href="https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2" />
    /// </summary>
    public string Country { get; set; } = string.Empty;

    public string Coalition { get; set; } = string.Empty;
    public ACMIColor Color { get; set; }
    /// <summary>
    /// Filename of the 3D model which will be used to represent the object in the 3D view. 3D models must be in Wavefront .obj file format and stored in either %ProgramData%\Tacview\Data\Meshes\ or %APPDATA%\Tacview\Data\Meshes\
    /// </summary>
    public string Shape { get; set; } = string.Empty;
    public string Debug { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public ulong FocusedTarget { get; set; }

    [ACMIObjectProperty("LockedTarget", typeof(ulong), 0)]
    [ACMIObjectProperty("LockedTarget1", typeof(ulong), 1)]
    [ACMIObjectProperty("LockedTarget2", typeof(ulong), 2)]
    [ACMIObjectProperty("LockedTarget3", typeof(ulong), 3)]
    [ACMIObjectProperty("LockedTarget4", typeof(ulong), 4)]
    [ACMIObjectProperty("LockedTarget5", typeof(ulong), 5)]
    [ACMIObjectProperty("LockedTarget6", typeof(ulong), 6)]
    [ACMIObjectProperty("LockedTarget7", typeof(ulong), 7)]
    [ACMIObjectProperty("LockedTarget8", typeof(ulong), 8)]
    [ACMIObjectProperty("LockedTarget9", typeof(ulong), 9)]
    public ulong[] LockedTarget { get; set; } = new ulong[10];


}
