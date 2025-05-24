

using System.Runtime.CompilerServices;

class Application
{
    private MyThreadSafeRegion layerRegion;
    private UserImplemention_ForMyRegion layerRegionInlay;
    private BorderControlPointDescription inputSplicer0 = new BorderControlPointDescription("splicer 0", typeof(List<float>), typeof(float), 0, true);
    private BorderControlPointDescription inputSplicer1 = new BorderControlPointDescription("splicer 1", typeof(List<int>), typeof(int), 1, true);

    public Application()
    {
        // Compiler-generated code:

        // initializes process node "MyRegion"
        layerRegionInlay = new UserImplemention_ForMyRegion();
        layerRegion = new MyThreadSafeRegion(layerRegionInlay);
        layerRegionInlay.region = layerRegion;

        layerRegion.BorderControlPointDescriptions = new()
        {
            inputSplicer0,
            inputSplicer1
        };
    }

    public void Update()
    {
        // user patch just updates one input splicer.
        // the other one gets written on another moment

        layerRegion.AcknowledgeInputSplicerValue(inputSplicer0, new List<float> { 7.0f, 12.0f }); 
        
        var layers = layerRegion.Update(); // calls the region

        // user patched something weird:
        foreach (var layer in layers)
        {
            var _ = Task.Run(() =>
            {
                while (true)
                {
                    layer.Notify(new Notification());
                }
            });
        }
    }
}


///additional Compiler-generated code in the user application:


class UserImplemention_ForMyRegion : IUserImplemention<ILayer>, IDisposable
{
    public MyThreadSafeRegion region; // gets set on initialization by the compiler

    /// <summary>
    /// Creates the patch inlay.
    /// What the user patched on white inside the region.
    /// </summary>
    public ILayer Create()
    {
        var patchInstance = new PatchInlay(region);

        // can have user code here

        return patchInstance;
    }

    private class PatchInlay : ILayer
    {
        private MyThreadSafeRegion region;

        BorderControlPointDescription inputSplicer0; 
        BorderControlPointDescription inputSplicer1;

        public PatchInlay(MyThreadSafeRegion region)
        {
            this.region = region;
            inputSplicer0 = region.BorderControlPointDescriptions[0];
            inputSplicer1 = region.BorderControlPointDescriptions[1];
        }

        public void Notify(Notification n)
        {
            var input0 = (float)region.RetrieveInputSplicer(this, inputSplicer0);
            var input1 = (int)region.RetrieveInputSplicer(this, inputSplicer1);
            // existing splicers should still work even if the order changed 
        }

        public void Render(CallerInfo c)
        {
        }
    }

    public void Dispose()
    {
    }
}
