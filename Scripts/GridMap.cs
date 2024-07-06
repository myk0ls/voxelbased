using Godot;
using System;
using System.Collections;
using DotnetNoise;

public partial class GridMap : Godot.GridMap
{

	FastNoiseLite noise = new FastNoiseLite();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random random = new Random();
		
		noise.Seed = (int)random.NextInt64();
		GD.Print(noise.Seed);

		// GENERATE TERRAIN
		for (int x = 0;  x < 200; x++)
		{
            for (int y = 0; y < 20; y++)
			{
                for (int z = 0; z < 200; z++)
				{
					if (y < noise.GetNoise2Dv(new Vector2(x,z)) * 20 + 10)
					{
						SetCellItem(new Vector3I(x,y,z), 0, 0);
					}
				}

            }

        }

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void DestroyBlock(Vector3 WorldCoordinate)
	{
		Vector3I MapCoordiante = LocalToMap(WorldCoordinate);
		SetCellItem(MapCoordiante, -1);
	}

    public void PlaceBlock(Vector3 WorldCoordinate, int BlockIndex)
    {
        Vector3I MapCoordiante = LocalToMap(WorldCoordinate);
        SetCellItem(MapCoordiante, BlockIndex);
    }
}
