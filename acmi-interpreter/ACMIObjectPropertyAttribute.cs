using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace acmi_interpreter;

/// <summary>
/// A property defined via ACMI telemetry or database file that may be named differently than its ACMI counterpart.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class ACMIObjectPropertyAttribute : Attribute
{
    /// <summary>
    /// An Alias
    /// </summary>
    public string PropertyAlias { get; set; }
    public Type TargetType { get; set; }
    public object? TargetIndex { get; set; }

    /// <summary>
    /// Initializes an instance of the ACMIObjectProperty Attribute with a property alias and conversion function.
    /// </summary>
    /// <param name="Alias"></param>
    /// <param name="conversionFunction"></param>
    public ACMIObjectPropertyAttribute(string Alias, Type TargetType)
    {
        PropertyAlias = Alias;
        this.TargetType = TargetType;
        TargetIndex = null;
    }

    public ACMIObjectPropertyAttribute(string Alias, Type TargetType, object TargetIndex)
    {
        PropertyAlias = Alias;
        this.TargetType = TargetType;
        this.TargetIndex = TargetIndex;
    }

    public object? GetValue (object instance)
    {
        var propRef = instance.GetType().GetProperty(PropertyAlias);

        if (propRef is null) throw new InvalidOperationException($"The property '{PropertyAlias}' does not exist on Type {instance.GetType().FullName}");
        
        if (TargetIndex is null) return propRef.GetValue(instance);
        else return propRef.GetValue(instance, new object?[] { TargetIndex });
    }

    public T? GetValue<T> (object instance)
    {
        return (T?) GetValue(instance);
    }

    public void SetValue<T> (object instance, T? value)
    {
        var propRef = instance.GetType().GetProperty(PropertyAlias);

        if (propRef is null) throw new InvalidOperationException($"The property '{PropertyAlias}' does not exist on Type {instance.GetType().FullName}");
        if (propRef.PropertyType != typeof(T)) throw new InvalidCastException($"Value '{value}' cannot be cast to Type {propRef.PropertyType.FullName}");

        if (TargetIndex is null) propRef.SetValue(instance, value);
        else propRef.SetValue(instance, value, new object?[] { TargetIndex });
    }
}
