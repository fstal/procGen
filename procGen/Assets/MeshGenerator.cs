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

// vertex count = (width + 1) * (lenght + 1) 

//https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html


[RequireComponent(typeof(MeshFilter))]     //Just so we dont try to add a mesh to nothing
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;
    
    //Vector2[] uvs;
    Vector3[] vertices;           //vertices
    int[] triangles;              //triangles
    public int xMax = 20;         //xSize  height
    public int zMax = 20;         //zSize  width

    public float lacunarity = 0;    //how much detail that is added or removed from each octave, adjusts our frequency
    public float scale = 20f;       // basically distance from which we view noisemap

    [Range(0f,3f)]
    public float redistribution = 1f; //raises elevation value to a power
    
    [Range(0f,1.0f)] 
    public float persistance = 0;   // adjusts amplitude

    float frequencyA = 5f;
    float frequencyB = 12f;
    float frequencyC = 20f;
    float amplitudeA = 20;
    float amplitudeB = 10f;
    float amplitudeC = 5f;


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //Tests
    

        // End tests

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
        // e.g [0, 1, 2, 1, 3, 2] would be the indexes of the vertices for the two first triangles, forming a square =>
        //[ (0,0,0), (0,0,1), (1,0,0), (0,0,1), (1,0,1), (1,0,0) ]

            //had weird ass behaviour where it connected last square in a row with first square on
            // the next row
        
        triangles = new int[xMax * zMax * 6];           // every square needs 6 stored vertice positions, even if theres only 4 actual vertices
        int vert = 0;                                   // increments 1 every iteration of inner loop
        int tri = 0;                                    // increments 6 every iteration of inner loop
        
        for (int z = 0; z < zMax; z++)
        {
            for (int x = 0; x < xMax; x++)
            {
                //First  trianglehalf of square   
                triangles[tri + 0] = vert + 0;
                triangles[tri + 1] = vert + xMax + 1;
                triangles[tri + 2] = vert + 1;

                //Second half of square
                triangles[tri + 3] = vert + 1;
                triangles[tri + 4] = vert + xMax + 1;
                triangles[tri + 5] = vert + xMax + 2;  

                vert++;
                tri += 6;   
            }   
            vert++;
        }

        //uvs = new Vector2[vertices.Length];
        
        for (int z = 0, idx = 0; z < zMax + 1; z++) // zMax + 1
        {
            for (int x = 0; x < xMax + 1; x++)
            {
                vertices[idx] = new Vector3(x, GenerateNoiseValue(x,z), z);
                idx++;
            }
        }



    }

    void UpdateMesh()
    {
        mesh.Clear();   //clears our mesh from prev data

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();      //using inbuilt, we could just as well calculate them ourselves, y tho
    }

    private float GenerateNoiseValue(int x, int z)
    //we need to make sure not to feed the give the PerlinNoise method a whole number, we need fractions 
    { 
        float fx = (float)x / xMax * scale;
        float fz = (float)z / zMax * scale;
        //Debug.Log(fx + "   " + fz);
        //Debug.Log(Mathf.PerlinNoise(frequencyA * fx, frequencyA * fz));
        //Debug.Log(x.GetType());
        float noiseValue = Mathf.PerlinNoise(frequencyA * fx, frequencyA * fz) * amplitudeA
                         + Mathf.PerlinNoise(frequencyB * fx, frequencyB * fz) * amplitudeB
                         + Mathf.PerlinNoise(frequencyC * fx, frequencyC * fz) * amplitudeC;
                        
        return Mathf.Pow(noiseValue, redistribution);
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
        if (zMax < 1) {zMax = 1;}
        if (xMax < 1) {xMax = 1;}
        if (lacunarity < 1) {lacunarity = 1;} 
    }
}