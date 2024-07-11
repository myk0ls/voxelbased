using Godot;
using System;

[GlobalClass]
public partial class BlockStats : Resource
{
    [Export]
    public bool Stackable { get; set; }

    [Export]
    public bool Breakable { get; set; }

    [Export]
    public bool Droppable { get; set; }

    [Export]
    public float BreakPace{ get; set; }

    public BlockStats(bool Stackable, bool Breakable, bool Droppable, float BreakPace)
    {
        this.Stackable = Stackable;
        this.Breakable = Breakable;
        this.BreakPace = BreakPace;
    }

    public BlockStats() : this(true, true, true, 1) { }


}
