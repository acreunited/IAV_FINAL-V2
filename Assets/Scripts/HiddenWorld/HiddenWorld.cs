using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HiddenWorld : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    public BlockInteraction blockInteraction;
    public GameObject text;
   
    // Start is called before the first frame update
    void Start()
    {
        actions.Add("New", Hidden);
        actions.Add("Menu", Menu);
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
        text.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        actions[speech.text].Invoke();
    }

    private void Hidden()
    {
        if(blockInteraction.NumOfblocks(Block.BlockType.DIRT)>=20 && blockInteraction.NumOfblocks(Block.BlockType.STONE) >= 10 && blockInteraction.NumOfblocks(Block.BlockType.DIAMOND) >= 6)
            SceneManager.LoadScene("HiddenWorld");
        else
        {
            text.SetActive(true);
            this.StartCoroutine(TimeOfText());
        }
    }

    private void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator TimeOfText()
    {
        yield return new WaitForSeconds(3);
        text.SetActive(false);
    }
}
