using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mic : MonoBehaviour {
    AudioSource audioSource;
    public AudioClip audioClip;
    public bool useMicrophone;
    public string deviceName;
    public GameObject wolfs;
    public GameObject sheeps;
    public Material materialSheep;
    public Material materialWolf;
    public Camera cam;
    bool canCreate;
    enum Animals { SHEEP, WOLF };


    void Start() {
        canCreate = true;
        audioSource = GetComponent<AudioSource>();
        ///currentPlayer = player2;
        if (useMicrophone) {
            if (Microphone.devices.Length > 0) {
                deviceName = Microphone.devices[0];
                audioSource.clip = Microphone.Start(deviceName, true, 10, AudioSettings.outputSampleRate);
                print(AudioSettings.outputSampleRate);
                while (Microphone.GetPosition(deviceName) < (AudioSettings.outputSampleRate / 1000) * 30) ;

            }
            else
                useMicrophone = false;
        }


        if (!useMicrophone) {
            audioSource.clip = audioClip;
        }

        audioSource.loop = true;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update() {
       float energy = AudioAnalysis.MeanEnergy(audioSource);

        if (AudioAnalysis.ConvertToDB(energy) > 40 && canCreate) {
            canCreate = false;
            float peakFrequency = AudioAnalysis.ComputeSpectrumPeak(audioSource, true);
            float concentration = AudioAnalysis.ConcentrationAroundPeak(peakFrequency);

            //assombio
            if (concentration > 0.8f) {
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10)) {
                    string chunkName = hit.collider.name;
                    float chunkx = hit.collider.gameObject.transform.position.x;
                    float chunky = hit.collider.gameObject.transform.position.y;
                    float chunkz = hit.collider.gameObject.transform.position.z;
                    Vector3 hitBlock = hit.point - hit.normal / 2f;

                    int blockx = (int)(Mathf.Round(hitBlock.x) - chunkx);
                    int blocky = (int)(Mathf.Round(hitBlock.y) - chunky);
                    int blockz = (int)(Mathf.Round(hitBlock.z) - chunkz);
                    Chunk c;
                    if (World.chunkDict.TryGetValue(chunkName, out c)) {
                       
                        c.chunkdata[blockx, blocky, blockz].SetType(Block.BlockType.GRASS);
                       

                        DestroyImmediate(c.goChunk.GetComponent<MeshFilter>());
                        DestroyImmediate(c.goChunk.GetComponent<MeshRenderer>());
                        DestroyImmediate(c.goChunk.GetComponent<MeshCollider>());
                        c.DrawChunk();
                    }
                }


            }
 

        }

    }

   

    IEnumerator TimeToCreate() {
        yield return new WaitForSeconds(1);
        canCreate = true;

    }


}






