using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockInteraction : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera cam;

    enum InteractionType { DESTROY,BUILD};
    InteractionType interactionType;
    Block.BlockType []type;
    int pointer = 0;
    public Text blockType;
    string[] blocks;
    private Dictionary<Block.BlockType, int> numBlocks = new Dictionary<Block.BlockType, int>();
    public Text dirt;
    public Text rock;
    public Text diamond;
    public GameObject selection;
    public GameObject pickaxe;
    public int pickaxeUse = 0;
    void Start()
    {
        type = new Block.BlockType[] { Block.BlockType.DIRT, Block.BlockType.STONE,Block.BlockType.DIAMOND };
        blocks = new string[] { "DIRT", "ROCK","DIAMOND" };//, "GOLD" };
        //blockType.text = blocks[pointer];
        numBlocks.Add(Block.BlockType.DIRT, 0);
        numBlocks.Add(Block.BlockType.STONE, 0);
        numBlocks.Add(Block.BlockType.DIAMOND, 0);
        dirt.text = ""+numBlocks[Block.BlockType.DIRT];
        rock.text = ""+numBlocks[Block.BlockType.STONE];
        diamond.text = ""+numBlocks[Block.BlockType.DIAMOND];
        selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(61, -60);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pointer = (pointer + 1);
            pointer = pointer % type.Length;
            //blockType.text = blocks[pointer];
            SetSelection(type[pointer]);
        }
        if (Input.GetKeyDown(KeyCode.M) && numBlocks[Block.BlockType.STONE]>=4)
        {
            pickaxe.SetActive(true);
            int aux = numBlocks[Block.BlockType.STONE];
            aux = aux - 4;
            numBlocks[Block.BlockType.STONE] = aux;
            SetNumOfBlocksText(Block.BlockType.STONE);
        }
        if (pickaxeUse == 6)
        {
            pickaxe.SetActive(false);
            pickaxeUse = 0;
        }
        bool interaction = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        if (interaction)
        {
            Debug.Log("ajaj");
            interactionType = Input.GetMouseButtonDown(0) ? InteractionType.DESTROY : InteractionType.BUILD;
            RaycastHit hit;
            if(Physics.Raycast(cam.transform.position,cam.transform.forward,out hit, 10))
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
                if(World.chunkDict.TryGetValue(chunkName,out c))
                {
                    if (interactionType == InteractionType.DESTROY)
                    {
                        Block.BlockType typeaux = c.chunkdata[blockx, blocky, blockz].GetBlockType();
                        Debug.Log(typeaux.ToString());
                        if (typeaux == Block.BlockType.GRASS)
                        {
                            typeaux = Block.BlockType.DIRT;
                        }
                        if (typeaux == Block.BlockType.DIAMOND )
                        {
                            if (pickaxe.activeSelf)
                            {
                                int num = numBlocks[Block.BlockType.DIAMOND];
                                num++;
                                numBlocks[Block.BlockType.DIAMOND] = num;
                                SetNumOfBlocksText(Block.BlockType.DIAMOND);
                                c.chunkdata[blockx, blocky, blockz].SetType(Block.BlockType.AIR);
                                pickaxeUse++;
                            }
                        }
                        else
                        {
                            int aux = numBlocks[typeaux];

                            aux++;
                            numBlocks[typeaux] = aux;
                            SetNumOfBlocksText(typeaux);

                            c.chunkdata[blockx, blocky, blockz].SetType(Block.BlockType.AIR);
                        }
                    }
                    else
                    {
                        int num = numBlocks[type[pointer]];
                        if (num > 0)
                        {
                            c.chunkdata[blockx, blocky, blockz].SetType(type[pointer]);
                            num--;
                            numBlocks[type[pointer]] = num;
                            SetNumOfBlocksText(type[pointer]);
                        }
                    }
                    DestroyImmediate(c.goChunk.GetComponent<MeshFilter>());
                    DestroyImmediate(c.goChunk.GetComponent<MeshRenderer>());
                    DestroyImmediate(c.goChunk.GetComponent<MeshCollider>());
                    c.DrawChunk();
                }
            }
        }
    }

    public void SetNumOfBlocksText(Block.BlockType type)
    {
        switch (type)
        {
            case Block.BlockType.DIRT:
                dirt.text = "" + numBlocks[Block.BlockType.DIRT];
                break;
            case Block.BlockType.STONE:
                rock.text = "" + numBlocks[Block.BlockType.STONE];
                break;
            case Block.BlockType.DIAMOND:
                diamond.text = "" + numBlocks[Block.BlockType.DIAMOND];
                break;
        }
    }

    public void SetSelection(Block.BlockType type)
    {
        switch (type)
        {
            case Block.BlockType.DIRT:
                selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(61, -60);
                break;
            case Block.BlockType.STONE:
                selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(61, -125);
                break;
            case Block.BlockType.DIAMOND:
                selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(61, -192);
                break;
        }
    }
}
