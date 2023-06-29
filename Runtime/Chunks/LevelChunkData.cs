using Godot;

namespace Leipzig3DGodot.Scripts;

public class LevelChunkData
{
    public Area3D Chunk { get; set; }
    public int[] ValidNeighbours { get; set; }
    public Vector3 VirtualPosition { get; set; }
}