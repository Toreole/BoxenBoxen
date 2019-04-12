using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    protected Transform canvas;
    [SerializeField]
    protected GameObject diaBoxPrefab;
    [SerializeField]
    protected string playerCharacterName = "_CharacterA";

    [SerializeField]
    protected DialoguePart[] preFightParts;
    [SerializeField]
    protected DialoguePart[] postFightParts;

    [SerializeField]
    protected bool advanceToNextScene = true;

    protected bool fightEnded = false;
    protected int partIndex = 0;

    protected GameObject diaBox;
    protected TextMeshProUGUI nameText;
    protected TextMeshProUGUI contentText;

    protected BoxController[] characters;

    [SerializeField]
    protected AudioSource audioSource;

    private void Start()
    {
        diaBox = Instantiate(diaBoxPrefab, canvas);
        nameText = diaBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        contentText = diaBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        characters = FindObjectsOfType<BoxController>();

        StartCoroutine(DoStartDialogue());
    }

    private void Update()
    {
        foreach(var @char in characters)
        {
            if (@char.IsDead)
            {
                if (@char.name.Equals(playerCharacterName))
                    BadEnd();
                else
                    GoodEnd();
            }
        }
    }

    void BadEnd()
    {
        SceneManager.LoadScene(0);
    }

    void GoodEnd()
    {
        StartCoroutine(DoEndDialogue());
    }

    protected IEnumerator DoStartDialogue()
    {
        diaBox.SetActive(true);
        for (partIndex = 0; partIndex < preFightParts.Length; partIndex++)
        {
            var part = preFightParts[partIndex];
            nameText.text = part.name;
            contentText.text = "";
            if(part.soundClip)
            {
                audioSource.clip = part.soundClip;
                audioSource.Play();
            }
            for(int i = 0; i < part.content.Length; i++)
            {
                contentText.text += part.content[i];
                yield return null;
            }
            yield return new WaitForSeconds(part.waitTime);
        }
        yield return null;
        foreach (var @char in characters)
            @char.Activate();
        diaBox.SetActive(false);
    }

    protected IEnumerator DoEndDialogue()
    {
        diaBox.SetActive(true);
        for (partIndex = 0; partIndex < postFightParts.Length; partIndex++)
        {
            var part = postFightParts[partIndex];
            nameText.text = part.name;
            contentText.text = "";
            if (part.soundClip)
            {
                audioSource.clip = part.soundClip;
                audioSource.Play();
            }
            for (int i = 0; i < part.content.Length; i++)
            {
                contentText.text += part.content[i];
            }
            yield return new WaitForSeconds(part.waitTime);
        }
        yield return null;
        
        if(advanceToNextScene)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}

[System.Serializable]
public struct DialoguePart
{
    public string name;
    public string content;
    public float waitTime;
    public AudioClip soundClip;
}
