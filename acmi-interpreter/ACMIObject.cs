using loki_geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace acmi_interpreter;

/// <summary>
/// A real-time object from an ACMI source.
/// </summary>
public class ACMIObject
{
    public ACMIObject (ulong ObjectID)
    {
        this.ObjectID = ObjectID;
    }

    // KEY VALUES
    public ulong ObjectID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Destroyed { get; set; }

    [ACMIObjectProperty("T", typeof(loki_geo.LatLonCoord))]
    public loki_geo.LatLonCoord Position { get; set; }
    public float Heading { get; set; }


    // OBJECT TYPE DATA
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

    public string Squawk { get; set; } = string.Empty;
    public string ICAO24 { get; set; } = string.Empty;

    public string Pilot { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    /// <summary>
    /// ISO 3166-1 alpha-2 country code. See <a href="https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2" />
    /// </summary>
    public string Country { get; set; } = string.Empty;
    public string Coalition { get; set; } = string.Empty;


    // DISPLAY
    public ACMIColor Color { get; set; }
    /// <summary>
    /// Filename of the 3D model which will be used to represent the object in the 3D view. 3D models must be in Wavefront .obj file format and stored in either %ProgramData%\Tacview\Data\Meshes\ or %APPDATA%\Tacview\Data\Meshes\
    /// </summary>
    public string Shape { get; set; } = string.Empty;
    public string Debug { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;


    // RELATIONS
    [ACMIObjectProperty("Parent", typeof(ulong))]
    public ulong ParentID { get; set; }
    public ulong Next { get; set; }

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


    // NUMERICAL PROPERTIES
    public float Importance { get; set; } = 0;
    public int Slot { get; set; } = 0;
    /// <summary>
    /// Specifies that an object is disabled (typically out-of-combat) without being destroyed yet. This is particularly useful for combat training and shotlogs.
    /// </summary>
    public bool Disabled { get; set; } = false;
    public float Visible { get; set; } = 1;
    public float Health { get; set; } = 1;

    public float Length { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Radius { get; set; }

    [ACMIObjectProperty("IAS", typeof(float))]
    public float Airspeed_Indicated { get; set; }
    [ACMIObjectProperty("CAS", typeof(float))]
    public float Airspeed_Calibrated { get; set; }
    [ACMIObjectProperty("TAS", typeof(float))]
    public float Airspeed { get; set; }
    [ACMIObjectProperty("Mach", typeof(float))]
    public float MachNumber { get; set; }

    [ACMIObjectProperty("AOA", typeof(float))]
    public float AngleOfAttack { get; set; }
    [ACMIObjectProperty("AOS", typeof(float))]
    public float AngleOfSlip { get; set; }

    [ACMIObjectProperty("AGL", typeof(float))]
    public float Altitude_Ground { get; set; }

    [ACMIObjectProperty("HDG", typeof(float))]
    public float Heading_True { get; set; }
    [ACMIObjectProperty("HDM", typeof(float))]
    public float Heading_Magnetic { get; set; }

    [ACMIObjectProperty("FuelWeight", typeof(float), 0)]
    [ACMIObjectProperty("FuelWeight1", typeof(float), 1)]
    [ACMIObjectProperty("FuelWeight2", typeof(float), 2)]
    [ACMIObjectProperty("FuelWeight3", typeof(float), 3)]
    [ACMIObjectProperty("FuelWeight4", typeof(float), 4)]
    [ACMIObjectProperty("FuelWeight5", typeof(float), 5)]
    [ACMIObjectProperty("FuelWeight6", typeof(float), 6)]
    [ACMIObjectProperty("FuelWeight7", typeof(float), 7)]
    [ACMIObjectProperty("FuelWeight8", typeof(float), 8)]
    [ACMIObjectProperty("FuelWeight9", typeof(float), 9)]
    public float[] FuelWeight { get; set; } = new float[10];

    public int RadarMode { get; set; } = 0;
    public float RadarHorizontalBeamwidth { get; set; }
    public float RadarVerticalBeamwidth { get; set; }

    public int LockedTargetMode { get; set; } = 0;
    public float LockedTargetAzimuth { get; set; }
    public float LockedTargetElevation { get; set; }
    public float LockedTargetRange { get; set; }

    public void UpdateFrom(ACMIMessage message)
    {
        var props = GetType().GetProperties();

        foreach (var segment in message.Segments)
        {
            string[] split = segment.Split('=');
            PropertyInfo prop;
            object? index = null;

            if (props.FirstOrDefault(p => p.Name.Equals(split[0])) is PropertyInfo pr) prop = pr;
            else
            {
                IEnumerable<PropertyInfo> pras = props.Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(ACMIObjectPropertyAttribute)));
                var matched = pras.FirstOrDefault(p => p.GetCustomAttributes<ACMIObjectPropertyAttribute>().FirstOrDefault(a => a.Alias.Equals(split[0])) is not null);
                
                if (matched is null) continue;

                prop = matched;
                var attribute = matched.GetCustomAttributes<ACMIObjectPropertyAttribute>().FirstOrDefault(a => a.Alias.Equals(split[0]));
                index = prop.GetCustomAttribute<ACMIObjectPropertyAttribute>()?.TargetIndex ?? null;
            }

            UpdateProperty(prop, split[1], index);
        }
    }

    protected bool UpdateProperty(PropertyInfo property, string ValueText, object? index)
    {
        Type ptype = property.PropertyType;
        object? value = null;
        object? currentValue = property.GetValue(this, new object?[] { index });

        if (property.Name == nameof(Position))
        {
            LatLonCoord currentLatLon = (LatLonCoord?)currentValue ?? new LatLonCoord();
            string[] splits = ValueText.Split('|');
            int length = splits.Length;

            double lon = double.TryParse(splits[0], out var rlo) ? rlo : currentLatLon.Lon_Degrees;
            double lat = double.TryParse(splits[1], out var rla) ? rla : currentLatLon.Lat_Degrees;
            double alt = double.TryParse(splits[2], out var ra) ? ra : currentLatLon.Alt;

            loki_geo.LatLonCoord coord = new()
            {
                Alt = alt,
                Lon_Degrees = lon,
                Lat_Degrees = lat
            };

            if (length == 3 || length == 5) value = coord;
            else if (length == 6 || length == 9)
            {
                Heading = float.TryParse(splits[5], out var hdg) ? hdg : Heading;
                value = coord;
            }
            else return false;
        }
        else if (ptype == typeof(ulong))
        {
            bool parsed = ulong.TryParse(ValueText, System.Globalization.NumberStyles.AllowHexSpecifier, null, out ulong result);
            if (!parsed) return false;
            else value = result;
        }
        else if (ptype == typeof(float))
        {
            bool parsed = float.TryParse(ValueText, out float result);
            if (!parsed) return false;
            else value = result;
        }
        else if (ptype == typeof(int))
        {
            bool parsed = int.TryParse(ValueText, out int result);
            if (!parsed) return false;
            else value = result;
        }
        else if(ptype == typeof(bool))
        {
            bool result = ValueText == "1" ? true : false;
            value = result;
        }
        else if (ptype == typeof(string)) value = ValueText;

        if (index is null) property.SetValue(this, value);
        else property.SetValue(this, value, new object?[] { index });
        return true;
    }
}
