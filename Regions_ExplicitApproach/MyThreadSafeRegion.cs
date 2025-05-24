using System.Collections;
using System.Collections.Concurrent;


/// <summary>
/// This defines a region in C#. It is the region that the user creates via the node browser.
/// </summary>

[ProcessNode]
public class MyThreadSafeRegion : IRegion, IHasInputSplicers<List<MarkerTypes.T>, MarkerTypes.T, ILayer>
{
    public List<BorderControlPointDescription> BorderControlPointDescriptions { get; set; }

    Dictionary<BorderControlPointDescription, IList> InputSplicersOuter = new();

    public void AcknowledgeInputSplicerValue(BorderControlPointDescription splicer, object outerValue)
    {
        // our region only has Update, which should always be called from the same thread
        InputSplicersOuter[splicer] = (IList)outerValue;
    }

    private ConcurrentDictionary<(ILayer, BorderControlPointDescription), object> inputSplicersForPatchInstances = new();
    public object RetrieveInputSplicer(ILayer patchInstance, BorderControlPointDescription inputSplicer)
    {
        // the inside is prepared to access the splicer values per patch in a thread-safe manner.
        return inputSplicersForPatchInstances[(patchInstance, inputSplicer)];
    }

    /// <summary>
    /// Take implementation once on create. It once describes the program patched by the user inside the region. 
    /// If the region changes we need to swap the program somehow.
    /// Values captured by closures are taken care of explicitely by incoming links. Those can be written partially. 
    ///   On the outside some incoming links might be on one moment others on another outer patch moment. Only those of the current moment get updated.
    /// </summary>
    public MyThreadSafeRegion(IUserImplemention<ILayer> region)
    {
        this.region = region;
        p1 = region.Create();
        p2 = region.Create();
    }

    public List<ILayer> Update()
    {
        foreach (var inputSplicer in InputSplicersOuter)
        {
            inputSplicersForPatchInstances[(p1, inputSplicer.Key)] = inputSplicer.Value[0];
            inputSplicersForPatchInstances[(p2, inputSplicer.Key)] = inputSplicer.Value[1];
        }

        return new List<ILayer>() { p1, p2 }; // user may call Notify on different threads
    }

    private IUserImplemention<ILayer> region;
    private ILayer p1;
    private ILayer p2;

}


public sealed class CallerInfo;
public sealed class Notification;

public interface ILayer
{
    void Render(CallerInfo c);
    void Notify(Notification n);
}