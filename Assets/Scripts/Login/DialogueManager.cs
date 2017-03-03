using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public List<string> ConversationList;
    public Text Conversation;
    public int count = 0;
    public AudioSource light;

    void OnEnable()
    {
        StartCoroutine(PlaySound(light, 3f));
    }

    public void NextWord()
    {
        if (count < ConversationList.Count)
        {
            Conversation.text = ConversationList[count];
            count += 1;
        }
    }

	public void LoadScene (string sc)
    {
        if (count < ConversationList.Count) return;
        else Application.LoadLevel(sc);
	}

    IEnumerator PlaySound(AudioSource _audio, float _time)
    {
        yield return new WaitForSeconds(_time);
        _audio.Play();
    }
}
