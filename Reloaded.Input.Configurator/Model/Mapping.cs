using System.Diagnostics;
using Reloaded.Input.Structs;
using Reloaded.WPF.MVVM;

namespace Reloaded.Input.Configurator.Model;

public class Mapping : ObservableObject
{
    /// <summary>
    /// Time during which mapping is allowed.
    /// </summary>
    const int TimeToMap = 5000;

    public VirtualController Source     { get; private set; }
    public string Name                  { get; private set; }
    public int MappingIndex             { get; private set; }
    public MappingType Type             { get; private set; }
    public string MapMeText             { get; private set; }
    public string FriendlyName          { get; private set; }
    public bool IsNotBinding            { get; private set; } = true;
    public string CurrentValue          { get; private set; } = "";
        
    public Mapping(VirtualController source, string name, int mappingIndex, MappingType type)
    {
        Source = source;
        Name = name;
        MappingIndex = mappingIndex;
        MapMeText = "Map Me!";
        FriendlyName = source.GetFriendlyMappingName(mappingIndex);
        Type = type;
    }

    public void UpdateValue()
    {
        if (IsNotBinding)
            Source.PollAll();
        
        CurrentValue = Type == MappingType.Axis ? $"{Source.GetAxis(MappingIndex)}" 
            : $"{Source.GetButton(MappingIndex)}";
    }

    public async Task Map(int mappingNo = 0)
    {
        if (!IsNotBinding)
            return;

        IsNotBinding = false;
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var cts = new CancellationTokenSource();
        await Source.Map(MappingIndex, Type, mappingNo, cts.Token, Callback);
            
        stopWatch.Reset();
        IsNotBinding = true;
        MapMeText    = "Map Me!";
        FriendlyName = Source.GetFriendlyMappingName(MappingIndex);

        void Callback()
        {
            if (stopWatch.ElapsedMilliseconds > TimeToMap)
                cts.Cancel();

            MapMeText = $"Time Left: {TimeToMap - stopWatch.ElapsedMilliseconds}";
        }
    }

    public void UnMap(int mappingNo = 0)
    {
        Source.UnMap(MappingIndex, mappingNo);
        FriendlyName = Source.GetFriendlyMappingName(MappingIndex);
    }
}