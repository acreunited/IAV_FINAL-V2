using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils 
{
    static float[] smoothArr = new float[] { 0.0002f,0.0005f,0.001f,0.1f };
    static float smooth=0.002f;
    static float smooth3D = 10 * smooth;
    static int maxHeight = 150;
    static int octaves = 7;
    static int[] octavesArr = new int[] {5,6,7};
    static float persistence = 0.7f;
    static float offset = 32000f;
    // Start is called before the first frame update

    public static int GenerateHeight(float x,float z)
    { 
        return (int)Map(0, maxHeight, 0, 1, fBM(x*smooth, z*smooth, octaves, persistence));
    }

    public static int GenerateStoneHeight(float x, float z)
    {
        return (int)Map(0, maxHeight-20, 0, 1, fBM(x * 3*smooth, z *3*smooth, octaves-1, 1.2f*persistence));
    }

    static float Map(float newmin,float newmax,float orimin,float orimax,float val)
    {
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(orimin, orimax, val));
    }

    public static float fBM3D(float x,float y,float z, int octaves, float persistence)
    {
   

        float xy = fBM(x*smooth3D, y*smooth3D, octaves,persistence);
        float yx = fBM(y * smooth3D, x * smooth3D, octaves, persistence);
        float xz = fBM(x * smooth3D, z * smooth3D, octaves, persistence);
        float zx = fBM(z * smooth3D, x * smooth3D, octaves, persistence);
        float yz = fBM(y * smooth3D, z * smooth3D, octaves, persistence);
        float zy = fBM(z * smooth3D, y * smooth3D, octaves, persistence);
        return (xy + yx + xz + zx + yz + zy)/6;

    }
    
    static float fBM(float x,float z, int octaves, float persistence)
    {
        float total = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;
        for(int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise((x+offset)*frequency,(z+offset)*frequency)*amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
           
            
        }
        return total / maxValue;
    }

    public static int GenerateHeightRaandom(float x, float z)
    {
        int random1 = (int)Random.Range(0, 2);
        float smoothaux = smoothArr[random1];
        int random2 = (int)Random.Range(0, 1);
        int octavesaux = octavesArr[random2];
        return (int)Map(0, maxHeight, 0, 1, fBM(x * smoothaux, z * smoothaux, octavesaux, persistence));
    }

    public static int GenerateStoneHeightRandom(float x, float z)
    {
        int random = (int)Random.Range(0, 2);
        float smoothaux = smoothArr[random];
        int random2 = (int)Random.Range(0, 1);
        int octavesaux = octavesArr[random2];
        return (int)Map(0, maxHeight - 10, 0, 1, fBM(x * 3 * smoothaux, z * 3 * smoothaux, octavesaux - 1, 1.2f * persistence));
    }

    public static float fBM3DRandom(float x, float y, float z, int octaves, float persistence)
    {
        int random = (int)Random.Range(0, 2);
        float smoothaux = smoothArr[random];
        float smooth3D = smoothaux * 10;

        float xy = fBM(x * smooth3D, y * smooth3D, octaves, persistence);
        float yx = fBM(y * smooth3D, x * smooth3D, octaves, persistence);
        float xz = fBM(x * smooth3D, z * smooth3D, octaves, persistence);
        float zx = fBM(z * smooth3D, x * smooth3D, octaves, persistence);
        float yz = fBM(y * smooth3D, z * smooth3D, octaves, persistence);
        float zy = fBM(z * smooth3D, y * smooth3D, octaves, persistence);
        return (xy + yx + xz + zx + yz + zy) / 6;

    }

    // Update is called once per frame

}
