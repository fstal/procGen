using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Remember to handle backface culling, clock-wise  
//the values in the triangle array represents idexes in the vertice array and are given in sets of three, clock-wise

// https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
// https://docs.unity3d.com/Manual/Example-CreatingaBillboardPlane.html
// https://docs.unity3d.com/Manual/AnatomyofaMesh.html
// https://docs.unity3d.com/Manual/UsingtheMeshClass.html
// https://docs.unity3d.com/ScriptReference/Mesh.html
// https://docs.unity3d.com/ScriptReference/Mesh-uv.html
// mesh.uv => is an array of Vector2s that can have values between (0,0) and (1,1). The values represent fractional offsets into a texture. For example, (0,0) is the lower left corner and (0.5, 0.5) is the middle of the texture.
// vertex count = (width + 1) * (height + 1) 

[RequireComponent(typeof(MeshFilter))]     //We wouldn't want to try adding a mesh object to nothing
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;              //vertices
    Color[] colors;                  //colors
    int[] triangles;                 //triangles
    public bool autoUpdate;
    public int xMax = 255;           //xSize  height
    public int zMax = 255;           //zSize  width

    public float lacunarity = 1f;           //how much detail that is added or removed from each octave, adjusts our frequency
    public float scale = 10.03f;            // basically distance from which we view noisemap
    public float redistribution = 1.09f;    //raises elevation value to a power

    [Range(0f,1f)] 
    public float persistance = 0;   // adjusts amplitude
    public Gradient gradient;

    float frequencyA = .5f;
    float frequencyB = 2f;
    float frequencyC = 8f;
    float amplitudeA = 30f;
    float amplitudeB = 7.5f;
    float amplitudeC = 2f;
    
    // Variables for normalising height values
    float maxNoiseValue = 0;
    float minNoiseValue = 1000;


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    // void Update()
    // {
    //     UpdateMesh();
    // }
    public void Render() 
    {
        maxNoiseValue = 0f;
        minNoiseValue = 10000f;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xMax + 1) * (zMax + 1)];  // actual no. of vertices for xMax*zMax amount of squares

        //Now we want to loop through the vertices and assign their positions in the grid
        for (int z = 0, idx = 0; z < zMax + 1; z++) // zMax + 1
        {
            for (int x = 0; x < xMax + 1; x++)
            {
                vertices[idx] = new Vector3(x, GenerateNoiseValue(x,z), z);
                idx++;
            }
        }
        // here it gets a bit tricky, since we want to handle the issues of backface culling
        // we need to have the vertices of each triangle (every set of three elements) given
        // in a clockwise order.
        // e.g [0, 1, 2, 1, 3, 2] would be the two first triangles, forming a square =>
        //[ (0,0,0), (0,0,1), (1,0,0), (0,0,1), (1,0,1), (1,0,0) ]
            
            //had weird ass behaviour where it connected last square in a row with first square on
            // the next row, write in blog
        
        //int vert = 0;                                   // increments 1 every iteration of inner loop and 1 once more after every row
        //int tri = 0;                                    // increments 6 every iteration of inner loop
        triangles = new int[xMax * zMax * 6];             // every square needs 6 stored vertice positions, even if there's only 4 actual vertices
        for (int z = 0, verticeIdx = 0, triangleIdx = 0; z < zMax; z++)
        {
            for (int x = 0; x < xMax; x++)
            {
                //First triangle half of square   
                triangles[triangleIdx + 0] = verticeIdx + 0;
                triangles[triangleIdx + 1] = verticeIdx + xMax + 1;
                triangles[triangleIdx + 2] = verticeIdx + 1;

                //Second triangle half of square
                triangles[triangleIdx + 3] = verticeIdx + 1;
                triangles[triangleIdx + 4] = verticeIdx + xMax + 1;
                triangles[triangleIdx + 5] = verticeIdx + xMax + 2;  
                
                verticeIdx++;
                triangleIdx += 6;   
            }   
            verticeIdx++;
        }
        ColorTerrain();
    }

    private float GenerateNoiseValue(int x, int z)
    { 
        float fx = (float)x / xMax * scale;
        float fz = (float)z / zMax * scale;

        float noiseValue = Mathf.PerlinNoise(frequencyA * fx, frequencyA * fz) * amplitudeA
                         + Mathf.PerlinNoise(frequencyB * fx, frequencyB * fz) * amplitudeB 
                         + Mathf.PerlinNoise(frequencyC * fx, frequencyC * fz) * amplitudeC;
        noiseValue = Mathf.Pow(noiseValue, redistribution);

        if (noiseValue > maxNoiseValue) maxNoiseValue = noiseValue;
        if (noiseValue < minNoiseValue) minNoiseValue = noiseValue;

        return noiseValue;
    }

    private void ColorTerrain()
    {
    colors = new Color[vertices.Length];  
    float noiseDiff = maxNoiseValue - minNoiseValue;

    for (int z = 0, idx = 0; z < zMax + 1; z++) 
        {
            for (int x = 0; x < xMax + 1; x++)
            {
                float normNoiseValue = ((vertices[idx].y - minNoiseValue) / noiseDiff);
                colors[idx] = gradient.Evaluate(normNoiseValue);  
                if (normNoiseValue < 0.33f) vertices[idx].y = minNoiseValue*3f;
                idx++;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();   //clears our mesh from prev data
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();      //using inbuilt, we could just as well calculate them ourselves, y tho
    }

    void OnValidate() 
    {
        if (zMax < 1) {zMax = 1;} else if (zMax > 255) {zMax = 255;} //unity has a 65535 vertice limit of a mesh
        if (xMax < 1) {xMax = 1;} else if (xMax > 255) {xMax = 255;}
        if (lacunarity < 1) lacunarity = 1;
    }
}