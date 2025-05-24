
public interface IRegion
{
    /// <summary>
    /// Acknowlege BCP descriptions
    /// </summary>
    List<BorderControlPointDescription> BorderControlPointDescriptions { set; }
}


/// <summary>
/// Represents the region implementation that the user provides.
/// Gets implemented on user application side by the compiler.
/// </summary>
public interface IUserImplemention<TInterface>
{
    /// <summary>
    /// The creation of the patch inlay, which implements the interface TInterface.
    /// What the user patched on white inside the region.
    /// </summary>
    TInterface Create();
}


/// <summary>
/// Implemented by the region designer. TPatch can be a living slice or just an integer for a Fuse loop
/// </summary>
public interface IHasInputSplicers<TOuter, TInner, TPatch>
{
    /// gets set on user application side by target code before entering the region.
    /// can get read by the region designer.
    /// </summary>
    void AcknowledgeInputSplicerValue(BorderControlPointDescription splicer, object outerValue);

    /// <summary>
    /// Access a value for a patch id or instance
    /// </summary>
    object RetrieveInputSplicer(TPatch patchInstance, BorderControlPointDescription inputSplicer);
}


public static class MarkerTypes
{
    public class T { }
    public class T1 { }
    public class T2 { }

}

public static class MarkerTypeInstances
{
    public static readonly Type T = typeof(MarkerTypes.T);
    public static readonly Type T1 = typeof(MarkerTypes.T1);
    public static readonly Type T2 = typeof(MarkerTypes.T2);
}

public struct BorderControlPointDescription
{
    public string Name;
    public Type Outer;
    public Type Inner; 
    public int Index;
    public bool IsSplicer;

    public BorderControlPointDescription(string name, Type outer, Type inner, int index, bool isSplicer)
    {
        Name = name;
        Outer = outer;
        Inner = inner;
        Index = index;
        IsSplicer = isSplicer;
    }

    public override string ToString()
    {
        return $"{Index}: {Outer} {Name}";
    }
}

public struct IncomingLinkDescription
{
    public Type TypeInfo;
    public int Index;

    public IncomingLinkDescription(Type typeInfo, int index)
    {
        TypeInfo = typeInfo;
        Index = index;
    }

    public override string ToString()
    {
        return $"{Index}: {TypeInfo}";
    }
}