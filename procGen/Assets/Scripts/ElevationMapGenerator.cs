using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevationMapGenerator : MonoBehaviour   //aslong as we dont want several instances of the script - static
{

    public static float[,] GenerateElevationMap(int width, int height, float scale) 
    {
        float[,] noiseMap = new float [width, height];

        if (scale <= 0) {scale = 0.001f;}        // Avoid division by 0

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float sampleX = x / scale;    //need floats from x and y
                float sampleY = y /scale;

                float perlinValue = Mathf.PerlinNoise(sampleX,sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }
        return noiseMap;
    }
}
