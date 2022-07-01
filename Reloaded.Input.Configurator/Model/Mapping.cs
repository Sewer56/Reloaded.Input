using System.Collections.ObjectModel;
using System.Diagnostics;
using Reloaded.Input.Structs;
using Reloaded.WPF.MVVM;
using Vortice;

namespace Reloaded.Input.Configurator.Model;

public class Mapping : ObservableObject
{
    /// <summary>
    /// Time during which mapping is allowed.
    /// </summary>
    const int TimeToMap = 5000;

    public VirtualController Source     { get; private set; }
    public ObservableCollection<MappingSlot> Slots { get; private set; }
    public string Name                  { get; private set; }
    public int MappingId                { get; private set; }
    public MappingType Type             { get; private set; }
    public bool IsNotBinding            { get; private set; } = true;
    public string CurrentValue          { get; private set; } = "";
    public string Description           { get; private set; }
        
    public Mapping(VirtualController source, string name, int mappingId, MappingType type, string description)
    {
        Source = source;
        Name = name;
        MappingId = mappingId;
        Type = type;
        Description = description;

        Slots = new ObservableCollection<MappingSlot>();
        foreach (var mappingsMapping in source.Mappings.GetOrCreateMapping(MappingId, type).Mappings)
        {
            Slots.Add(new MappingSlot(this, mappingsMapping.Key, mappingsMapping.Value.Inverted));
        }

        // If there are no items, make a dummy.
        CreateDefaultOrDummyIfNecessary();
    }

    public void UpdateValue()
    {
        if (!IsNotBinding) 
            return;

        Source.PollAll();
        CurrentValue = Type == MappingType.Axis ? $"{Source.GetAxis(MappingId)}"
            : $"{Source.GetButton(MappingId)}";
    }

    public async Task Map(int mappingNo = 0)
    {
        if (!IsNotBinding)
            return;

        IsNotBinding = false;
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var cts = new CancellationTokenSource();
        await Source.Map(MappingId, Type, mappingNo, cts.Token, Callback);
        stopWatch.Reset();

        // Update Mapping Text
        Slots.FirstOrDefault(x => x.MappingNo == mappingNo)?.UpdateMappingText();
        CreateDefaultOrDummyIfNecessary();

        // We're not binding anymore!
        IsNotBinding = true;

        void Callback()
        {
            if (stopWatch.ElapsedMilliseconds > TimeToMap)
                cts.Cancel();

            CurrentValue = $"Timeout: {TimeToMap - stopWatch.ElapsedMilliseconds}";
        }
    }

    public void UnMap(int mappingNo = 0)
    {
        Source.UnMap(MappingId, mappingNo);

        // Mapping No might not correspond to array index depending on remove order.
        var slot = Slots.FirstOrDefault(x => x.MappingNo == mappingNo);
        Slots.Remove(slot!);

        // Just so we're not left with 0.
        CreateDefaultOrDummyIfNecessary();
    }

    private void CreateDefaultOrDummyIfNecessary()
    {
        if (Slots.Count > 0)
        {
            // Create Default.
            var last = Slots[^1];
            if (last.IsValidMapping())
                Slots.Add(new MappingSlot(this));

            return;
        }

        // Create Dummy
        Slots.Add(new MappingSlot(this));
    }
}


public class MappingSlot : ObservableObject
{
    public Mapping Parent { get; set; }
    public int MappingNo { get; set; }
    public string MappingText { get; set; } = null!;
    public bool IsInverted { get; set; }

    /// <summary>
    /// Constructor for a dummy mapping slot that isn't backed by an actual mapping.
    /// </summary>
    public MappingSlot(Mapping parent)
    {
        Parent = parent;
        MappingNo = Parent.Source.GetMapping(Parent.MappingId)!.GetNextUnmappedIndex().GetValueOrDefault(0);
        UpdateMappingText();
    }

    /// <summary>
    /// Constructor for a non-dummy mapping slot with a known mapping no.
    /// </summary>
    public MappingSlot(Mapping parent, int mappingNo, bool valueInverted)
    {
        Parent = parent;
        MappingNo = mappingNo;
        IsInverted = valueInverted;
        UpdateMappingText();
    }

    public void UpdateMappingText()
    {
        var text = Parent.Source.GetFriendlyMappingName(Parent.MappingId, MappingNo);
        MappingText = !String.IsNullOrEmpty(text) ? text : "New";
    }

    public bool IsValidMapping()
    {
        var source = Parent.Source;
        return source.GetMapping(Parent.MappingId, MappingNo) != null;
    }

    public void ToggleInverted()
    {
        IsInverted = !IsInverted;
        var map = Parent.Source.GetMapping(Parent.MappingId, MappingNo);
        if (map != null)
            map.Inverted = IsInverted;

        UpdateMappingText();
    }
}