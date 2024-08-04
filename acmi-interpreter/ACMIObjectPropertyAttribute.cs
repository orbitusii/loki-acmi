namespace acmi_interpreter;

/// <summary>
/// A property defined via ACMI telemetry or database file that may be named differently than its ACMI counterpart.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class ACMIObjectPropertyAttribute : Attribute
{
    /// <summary>
    /// The ACMI-side name of this property
    /// </summary>
    public string Alias { get; set; }
    public Type TargetType { get; set; }
    public object? TargetIndex { get; set; }

    /// <summary>
    /// Initializes an instance of the ACMIObjectProperty Attribute with a property alias and conversion function.
    /// </summary>
    /// <param name="Alias"></param>
    /// <param name="conversionFunction"></param>
    public ACMIObjectPropertyAttribute(string Alias, Type TargetType)
    {
        this.Alias = Alias;
        this.TargetType = TargetType;
        TargetIndex = null;
    }

    public ACMIObjectPropertyAttribute(string Alias, Type TargetType, object TargetIndex)
    {
        this.Alias = Alias;
        this.TargetType = TargetType;
        this.TargetIndex = TargetIndex;
    }

    protected object? GetValue (object instance)
    {
        var propRef = instance.GetType().GetProperty(Alias);

        if (propRef is null) throw new InvalidOperationException($"The property '{Alias}' does not exist on Type {instance.GetType().FullName}");
        
        if (TargetIndex is null) return propRef.GetValue(instance);
        else return propRef.GetValue(instance, new object?[] { TargetIndex });
    }

    protected T? GetValue<T> (object instance)
    {
        return (T?) GetValue(instance);
    }

    protected void SetValue<T> (object instance, T? value)
    {
        var propRef = instance.GetType().GetProperty(Alias);

        if (propRef is null) throw new InvalidOperationException($"The property '{Alias}' does not exist on Type {instance.GetType().FullName}");
        if (propRef.PropertyType != typeof(T)) throw new InvalidCastException($"Value '{value}' cannot be cast to Type {propRef.PropertyType.FullName}");

        if (TargetIndex is null) propRef.SetValue(instance, value);
        else propRef.SetValue(instance, value, new object?[] { TargetIndex });
    }
}
