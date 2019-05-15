using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Here we're trying to build our meshes from scratch

//Remember to handle backface culling by making sure the vertices in 
//the triangle array are given clock-wise

// https://docs.unity3d.com/Manual/Example-CreatingaBillboardPlane.html
// https://docs.unity3d.com/Manual/AnatomyofaMesh.html
// https://docs.unity3d.com/Manual/UsingtheMeshClass.html
// https://docs.unity3d.com/ScriptReference/Mesh.html

// https://docs.unity3d.com/ScriptReference/Mesh-uv.html
//mesh.uv => is an array of Vector2s that can have values between (0,0) and (1,1). The values represent fractional offsets into a texture. For example, (0,0) is the lower left corner and (0.5, 0.5) is the middle of the texture.

// vertex count = (width + 1) * (height + 1) 

//https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html


[RequireComponent(typeof(MeshFilter))]     //Just so we dont try to add a mesh to nothing
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;
    Vector3[] vertices;           //vertices
    Color[] colors;           //colors
    int[] triangles;              //triangles
    public bool autoUpdate;
    public int xMax = 255;         //xSize  height
    public int zMax = 255;         //zSize  width

    public float lacunarity = 1f;    //how much detail that is added or removed from each octave, adjusts our frequency
    public float scale = 10.03f;       // basically distance from which we view noisemap
    public float redistribution = 1.09f; //raises elevation value to a power


    [Range(0f,1.0f)] 
    public float persistance = 0;   // adjusts amplitude
    public Gradient gradient;

    float frequencyA = .5f;
    float frequencyB = 2f;
    float frequencyC = 8f;
    float amplitudeA = 30f;
    float amplitudeB = 7.5f;
    float amplitudeC = 2f;
    
    float maxNoiseValue;
    float minNoiseValue;
    //float noiseDiff;



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
        maxNoiseValue = 0;
        minNoiseValue = 0;

        CreateShape();
        UpdateMesh();
    }


    void CreateShape()
    {
        vertices = new Vector3[(xMax + 1) * (zMax + 1)];  // actual no. of vertices for xMax*zMax amount of squares
        //Debug.Log((xMax + 1) * (zMax + 1));

        //Now we want to loop through the vertices and assign their positions in the grid
        // Basically creating the noise map
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
            // the next row
        
        triangles = new int[xMax * zMax * 6];           // every square needs 6 stored vertice positions, even if theres only 4 actual vertices
        int vert = 0;                                   // increments 1 every iteration of inner loop and 1 once more after every row
        int tri = 0;                                    // increments 6 every iteration of inner loop
        
        for (int z = 0; z < zMax; z++)
        {
            for (int x = 0; x < xMax; x++)
            {
                //First triangle half of square   
                triangles[tri + 0] = vert + 0;
                triangles[tri + 1] = vert + xMax + 1;
                triangles[tri + 2] = vert + 1;

                //Second triangle half of square
                triangles[tri + 3] = vert + 1;
                triangles[tri + 4] = vert + xMax + 1;
                triangles[tri + 5] = vert + xMax + 2;  
                
                vert++;
                tri += 6;   
            }   
            vert++;
        }
        ColorTerrain();
    }

    void UpdateMesh()
    {
        mesh.Clear();   //clears our mesh from prev data

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();      //using inbuilt, we could just as well calculate them ourselves, y tho
    }

    private float GenerateNoiseValue(int x, int z)
    { 
        float fx = (float)x / xMax * scale;
        float fz = (float)z / zMax * scale;
        //Debug.Log(Mathf.PerlinNoise(frequencyA * fx, frequencyA * fz));
        //Debug.Log(x.GetType());
        //Debug.Log(fx + "   " + fz);
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
    colors = new Color[vertices.Length];      //could just as well use the length from vertices, should be same
    float noiseDiff = maxNoiseValue - minNoiseValue;

    for (int z = 0, idx = 0; z < zMax + 1; z++) 
        {
            for (int x = 0; x < xMax + 1; x++)
            {
                float normNoiseValue = ((vertices[idx].y - minNoiseValue) / noiseDiff); //inverselerp??
                colors[idx] = gradient.Evaluate(normNoiseValue);        
                idx++;
            }
        }
    }

    // private void OnDrawGizmos()
    // {
    //     if (vertices == null) return;

    //     for (int i = 0; i < vertices.Length; i++)
    //     {
    //         Gizmos.DrawSphere(vertices[i], .1f);
    //     }
        
    // }

    void OnValidate() 
    {
        if (zMax < 1) {zMax = 1;} else if (zMax > 255) {zMax = 255;} //unity has a 65535 vertice limit of a mesh
        if (xMax < 1) {xMax = 1;} else if (xMax > 255) {xMax = 255;}
        if (lacunarity < 1) lacunarity = 1;

    }
}