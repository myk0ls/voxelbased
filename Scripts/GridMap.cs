using Godot;
using System;
using System.Collections;
using DotnetNoise;
using System.Collections.Generic;

public partial class GridMap : Godot.GridMap
{

	FastNoiseLite noise = new FastNoiseLite();
    Random random = new Random();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		GenerateWorld();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public int GetBlock(Vector3 WorldCoordinate)
    {
        Vector3I MapCoordiante = LocalToMap(WorldCoordinate);
        int index = GetCellItem(MapCoordiante);
        return index;
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

	public void GenerateWorld()
	{
        int level = 3;

        noise.FractalOctaves = level;
        noise.FractalGain = 3;
        

        noise.Seed = (int)random.NextInt64();
        GD.Print(noise.Seed);

        int numberOfBlocks = 3;

        // GENERATE TERRAIN
        for (int x = 0; x < 200; x++)
        {
            for (int y = 0; y < 50; y++)
            {
                for (int z = 0; z < 200; z++)
                {

                    // Get the noise value
                    float noiseValue = noise.GetNoise2Dv(new Vector2(x, z));

                    // Scale the noise value to the range of 0 to 1
                    float scaledNoiseValue = (noiseValue + 1) / 2.0f;

                    // Determine the block index based on the scaled noise value
                    int blockIndex = (int)Math.Floor((1 - scaledNoiseValue) * numberOfBlocks);

                    // Ensure the block index is within the valid range
                    blockIndex = Math.Min(blockIndex, numberOfBlocks - 1);

                    // Determine the height threshold for the current position
                    float heightThreshold = scaledNoiseValue * 18 + 10;

                    // Place the block if below the height threshold
                    if (y < heightThreshold)
                    {
                        SetCellItem(new Vector3I(x, y, z), blockIndex, 0);
                    }
                }

            }

        }

        // After terrain generation, add trees
        for (int x = 0; x < 200; x++)
        {
            for (int z = 0; z < 200; z++)
            {
                // Simple condition to place a tree at certain locations
                if (random.NextDouble() < 0.01) // 5% chance to place a tree
                {
                    // Find the highest terrain point at this (x, z) location
                    int maxHeight = -1;
                    for (int y = 49; y >= 0; y--)
                    {
                        int cellItemBlockIndex = GetCellItem(new Vector3I(x, y, z));
                        if (cellItemBlockIndex != -1 && // assume -1 is not air
                            cellItemBlockIndex != 2 &&
                            cellItemBlockIndex != 3) // Assuming 2 is not stone
                        {
                            maxHeight = y;
                            break;
                        }
                    }

                    if (maxHeight != -1)
                    {
                    // Place tree trunk and leaves
                    PlaceTree(new Vector3I(x, maxHeight + 1, z));

                    }
                }
            }
        }

        void PlaceTree(Vector3I basePosition)
        {
            int trunkHeight = (int)random.NextInt64(4, 8); // Random trunk height between 4 and 7
            int trunkBlockIndex = 4;
            int leafBlockIndex = 3;
            // Place trunk
            for (int i = 0; i < trunkHeight; i++)
            {
                SetCellItem(basePosition + new Vector3I(0, i, 0), trunkBlockIndex, 0); // trunkBlockIndex should be the index of the trunk block
            }

            // Place leaves
            int leafStart = basePosition.Y + trunkHeight - 2; // Leaves start 2 blocks below the top of the trunk
            int leafEnd = basePosition.Y + trunkHeight;

            for (int y = leafStart; y <= leafEnd; y++)
            {
                for (int x = -2; x <= 2; x++)
                {
                    for (int z = -2; z <= 2; z++)
                    {
                        // Make a simple sphere-like shape for leaves
                        if (Math.Abs(x) + Math.Abs(y - leafStart) + Math.Abs(z) <= 2)
                        {
                            SetCellItem(new Vector3I(basePosition.X + x, y, basePosition.Z + z), leafBlockIndex, 0); // leafBlockIndex should be the index of the leaf block
                        }
                    }
                }
            }
        }

    }
}
