using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionScript : MonoBehaviour
{

    //public int numIterations = 5;
    public int dropletLifeSpan = 1;
    public float inertia = .05f;
    public float initialSpeed = 1;
    //public float initialWaterVolume = 1;
    public float depositSpeed = .3f;
    public float evaporateSpeed = .01f;
    public float sedimentCapacityFactor = 4; // Multiplier for how much sediment a droplet can carry
    public float minSedimentCapacity = .01f; // Used to prevent carry capacity getting too close to zero on flatter terrain
    public float erodeSpeed = .3f;
    public float gravity = 4;
    bool reset = true;
    System.Random prng;

    public void InitPrng(int mapX, int mapZ)
    {   
        //Needed so that we don't reset prng with each animate iteration
        if(reset)
        {
            prng = new System.Random (0);
            //InitializeBrushIndices (mapX, mapZ, erosionRadius);
            reset = false;
        }
    }
    
    public void Erosion(Vector3[] mapVertices, int mapX, int mapZ, float noiseDiff, bool animate, int numIterations, float initialWaterVolume){
        
        //Initialize the random number seed. (We need this if we have enabled animation)
        InitPrng(mapX, mapZ);

        //If we enable animate we only want to do a certain amount of iterations per frame
        if(animate){
            numIterations = 2000;
        }

        //Loop for amount of iterations.
        for (int iteration = 0; iteration < numIterations; iteration++){
            //Create drops at random positions
            float posX = prng.Next (0, mapX);
            float posZ = prng.Next(0, mapZ);
            //Set variables vars
            float dirX = 0;
            float dirZ = 0;
            float speed = initialSpeed;
            float water = initialWaterVolume;
            float sediment = 0;




            //Droplet doing droplet things during it's lifetime.
            //We do everything for each droplet after we create it.
            for (int dropletLife = 0; dropletLife < dropletLifeSpan; dropletLife++)
            {
                
                //Calculate the offset in the cell.
                //First time we will be at an edge point but in future iterations we may end up in middle since we move the droplet
                int nodeX = (int) posX;
                int nodeZ = (int) posZ;

                //Calc droplet index
                int dropletIndex = nodeZ * mapX + nodeX;
                
                float cellOffsetX = posX - nodeX;
                float cellOffsetZ = posZ - nodeZ;

                //Use bilinear interpolation to calc droplet height and direction
                HeightAndGrad heightAndGrad = CalcHeightAndGrad (mapVertices, mapX, mapZ, posX, posZ, noiseDiff);

                //Now we need to move
                //Because of following code we may end up inside a cell
                //Inertia (tröghet) is used to give the droplet possibility to not stop in the first possible pit
                dirX = (dirX * inertia - heightAndGrad.gradientX * (1 - inertia));
                dirZ = (dirZ * inertia - heightAndGrad.gradientZ * (1 - inertia));

                // Normalize direction
                float lengthOfDir = Mathf.Sqrt (dirX * dirX + dirZ * dirZ);
                if (lengthOfDir != 0) {
                    dirX = dirX / lengthOfDir;
                    dirZ = dirZ / lengthOfDir;
                }
                
                //Move position
                posX = posX + dirX;
                posZ = posZ + dirZ;


                //Protection so that we don't run off map
                if ((dirX == 0 && dirZ == 0) || posX <= 0 || posX >= mapX - 1 || posZ <= 0 || posZ >= mapZ - 1) {
                    break;
                }


                // Now that we have moved we need to find a height for the new posisiton
                // Find the droplet's new height and calculate the deltaHeight
                float newHeight = CalcHeightAndGrad (mapVertices, mapX, mapZ, posX, posZ, noiseDiff).height;
                float deltaHeight = newHeight - heightAndGrad.height;

                //Calculating sediment capacity
                float sedimentCapacity = Mathf.Max (-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                // If carrying more sediment than capacity, or if flowing uphill we want to deposit sediment
                if (sediment > sedimentCapacity || deltaHeight > 0) {
                    //Add sediment to the landscape through bilinear interpolation if the droplet is carrying too much
                    float amountToDeposit = (deltaHeight > 0) ? Mathf.Min (deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                    sediment -= amountToDeposit;

                    mapVertices[dropletIndex].y += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetZ);
                    mapVertices[dropletIndex + 1].y += amountToDeposit * cellOffsetX * (1 - cellOffsetZ);
                    mapVertices[dropletIndex + mapX].y += amountToDeposit * (1 - cellOffsetX) * cellOffsetZ;
                    mapVertices[dropletIndex + mapX + 1].y += amountToDeposit * cellOffsetX * cellOffsetZ;
                }
                //Otherwise we want to erode from the ground
                else{
                    //Give the droplet more sediment
                    float amountToErode = Mathf.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);                    
                    float currDroplet = mapVertices[dropletIndex].y/noiseDiff;
                    float weighedErodeAmount = amountToErode;
                    float deltaSediment = (currDroplet < amountToErode) ? currDroplet : amountToErode;

                    //Since map isn't at 0 we don't go below 20
                    if(mapVertices[dropletIndex].y > 20){
                        mapVertices[dropletIndex].y -= deltaSediment;
                        sediment += deltaSediment;
                    }   
                        
                }
                // Update droplet's speed and water content
                speed = Mathf.Sqrt (speed * speed + deltaHeight * gravity);
                water *= (1 - evaporateSpeed);
            }
        }
    }

    HeightAndGrad CalcHeightAndGrad (Vector3[] mapVertices, int mapX, int mapZ, float posX, float posZ, float noiseDiff){
        
        //Droplet pos
        int coordX = (int) posX;
        int coordZ = (int) posZ;

        //Finding the offset
        float x = posX - coordX;
        float z = posZ - coordZ;

        //Node index for finding correct NW vertex
        int nodeIndex = coordZ * mapX + coordX;

        //Find the 4 corners NW, NE, SW, SE (North West and so on...)
        float heightNW = mapVertices[nodeIndex].y;
        float heightNE = mapVertices[nodeIndex + 1].y;
        float heightSW = mapVertices[nodeIndex + mapX].y;
        float heightSE = mapVertices[nodeIndex + mapX + 1].y;

        //Normalize values
        heightNW = heightNW/noiseDiff;
        heightNE = heightNE/noiseDiff;
        heightSW = heightSW/noiseDiff;
        heightSE = heightSE/noiseDiff;
        
        //Find direction through bilinear interpolation
        float gradientX = (heightNE - heightNW) * (1 - z) + (heightSE - heightSW) * z;
        float gradientZ = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

        //Calculate a height based on the different heights surrounding the droplet
        float newHeight = heightNW * (1 - x) * (1 - z) + heightNE * x * (1 - z) + heightSW * (1 - x) * z + heightSE * x * z;

        //Return statement
        return new HeightAndGrad () { height = newHeight, gradientX = gradientX, gradientZ = gradientZ };
        
    }

    struct HeightAndGrad {
        public float height;
        public float gradientX;
        public float gradientZ;
    }

}
