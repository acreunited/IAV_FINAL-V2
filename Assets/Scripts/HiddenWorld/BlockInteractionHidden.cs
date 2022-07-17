using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockInteractionHidden : MonoBehaviour
{
    public Camera cam;

    enum InteractionType { DESTROY, BUILD };
    InteractionType interactionType;
    Block.BlockType[] type;
    int pointer = 0;
    public Text blockType;
    string[] blocks;
    private Dictionary<Block.BlockType, int> numBlocks = new Dictionary<Block.BlockType, int>();
    public Text blue;
    public Text purple;
    public Text orange;
    public GameObject selection;
      public int pickaxeUse = 0;
    AudioSource audioSource;
    public string deviceName;
    public bool useMicrophone;
    public AudioClip audioClip;
    private bool canChange = true;
    public AudioSource cube;
    public AudioClip createCube;
    void Start()
    {
        type = new Block.BlockType[] { Block.BlockType.BLUE, Block.BlockType.PURPLE, Block.BlockType.ORANGE };
       // blocks = new string[] { "DIRT", "ROCK", "DIAMOND" };//, "GOLD" };
        //blockType.text = blocks[pointer];
        numBlocks.Add(Block.BlockType.BLUE, 0);
        numBlocks.Add(Block.BlockType.PURPLE, 0);
        numBlocks.Add(Block.BlockType.ORANGE, 0);
        blue.text = "" + numBlocks[Block.BlockType.BLUE];
        purple.text = "" + numBlocks[Block.BlockType.PURPLE];
        orange.text = "" + numBlocks[Block.BlockType.ORANGE];
        selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(61, -60);
        audioSource = GetComponent<AudioSource>();
        if (useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {
                deviceName = Microphone.devices[0];
                audioSource.clip = Microphone.Start(deviceName, true, 10, AudioSettings.outputSampleRate);
                print(AudioSettings.outputSampleRate);
                while (Microphone.GetPosition(deviceName) < (AudioSettings.outputSampleRate / 1000) * 30) ;

            }
            else
                useMicrophone = false;
        }


        if (!useMicrophone)
        {
            audioSource.clip = audioClip;
        }

        audioSource.loop = true;
        audioSource.Play();

    }

    // Update is called once per frame
    void Update()
    {
        if (AudioAnalysis.Whistle(audioSource) && canChange)
        {
            pointer = (pointer + 1);
            pointer = pointer % type.Length;
            //blockType.text = blocks[pointer];
            SetSelection(type[pointer]);
            canChange = false;
            StartCoroutine(TimeOfChange());
        }
        bool interaction = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        if (interaction)
        {
            Debug.Log("ajaj");
            interactionType = Input.GetMouseButtonDown(0) ? InteractionType.DESTROY : InteractionType.BUILD;
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
            {
                string chunkName = hit.collider.name;
                float chunkx = hit.collider.gameObject.transform.position.x;
                float chunky = hit.collider.gameObject.transform.position.y;
                float chunkz = hit.collider.gameObject.transform.position.z;
                Vector3 hitBlock;
                if (interactionType == InteractionType.DESTROY)
                {
                    hitBlock = hit.point - hit.normal / 2f;
                }
                else
                {
                    hitBlock = hit.point + hit.normal / 2f;
                }

                int blockx = (int)(Mathf.Round(hitBlock.x) - chunkx);
                int blocky = (int)(Mathf.Round(hitBlock.y) - chunky);
                int blockz = (int)(Mathf.Round(hitBlock.z) - chunkz);
                Chunk c;
                if (World.chunkDict.TryGetValue(chunkName, out c))
                {
                    if (interactionType == InteractionType.DESTROY)
                    {
                        Block.BlockType typeaux = c.chunkdata[blockx, blocky, blockz].GetBlockType();
                        Debug.Log(typeaux.ToString());
             
                     
                        
                            int aux = numBlocks[typeaux];

                            aux++;
                            numBlocks[typeaux] = aux;
                            SetNumOfBlocksText(typeaux);

                            c.chunkdata[blockx, blocky, blockz].SetType(Block.BlockType.AIR);
                        
                    }
                    else
                    {
                        int num = numBlocks[type[pointer]];
                        if (num > 0)
                        {
                            c.chunkdata[blockx, blocky, blockz].SetType(type[pointer]);
                            cube.clip = createCube;
                            cube.Play();
                            num--;
                            numBlocks[type[pointer]] = num;
                            SetNumOfBlocksText(type[pointer]);
                        }
                    }


                    List<string> updates = new List<string>();
                    updates.Add(chunkName);
                    if (blockx == 0)
                        updates.Add(World.CreateChunkName(new Vector3(chunkx - World.chunkSize, chunky, chunkz)));
                    if (blockx == World.chunkSize - 1)
                        updates.Add(World.CreateChunkName(new Vector3(chunkx + World.chunkSize, chunky, chunkz)));
                    if (blocky == 0)
                        updates.Add(World.CreateChunkName(new Vector3(chunkx, chunky - World.chunkSize, chunkz)));
                    if (blocky == World.chunkSize - 1)
                        updates.Add(World.CreateChunkName(new Vector3(chunkx, chunky - World.chunkSize, chunkz)));
                    if (blockz == 0)
                        updates.Add(World.CreateChunkName(new Vector3(chunkx, chunky, chunkz - World.chunkSize)));
                    if (blockz == World.chunkSize - 1)
                        updates.Add(World.CreateChunkName(new Vector3(chunkx, chunky, chunkz + World.chunkSize)));
                    foreach (string cname in updates)
                    {
                        if (World.chunkDict.TryGetValue(cname, out c))
                        {
                            DestroyImmediate(c.goChunk.GetComponent<MeshFilter>());
                            DestroyImmediate(c.goChunk.GetComponent<MeshRenderer>());
                            DestroyImmediate(c.goChunk.GetComponent<MeshCollider>());
                            c.DrawChunk();
                        }
                    }
                }
            }
        }
    }

    public void SetNumOfBlocksText(Block.BlockType type)
    {
        switch (type)
        {
            case Block.BlockType.BLUE:
                blue.text = "" + numBlocks[Block.BlockType.BLUE];
                break;
            case Block.BlockType.PURPLE:
                purple.text = "" + numBlocks[Block.BlockType.PURPLE];
                break;
            case Block.BlockType.ORANGE:
                orange.text = "" + numBlocks[Block.BlockType.ORANGE];
                break;
        }
    }

    public void SetSelection(Block.BlockType type)
    {
        switch (type)
        {
            case Block.BlockType.BLUE:
                selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(61, -60);
                break;
            case Block.BlockType.PURPLE:
                selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(61, -125);
                break;
            case Block.BlockType.ORANGE:
                selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(61, -192);
                break;
        }
    }

    IEnumerator TimeOfChange()
    {
        yield return new WaitForSeconds(1);
        canChange = true;
    }
}
