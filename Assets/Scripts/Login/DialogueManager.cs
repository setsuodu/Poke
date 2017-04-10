using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public List<string> ConversationList;
    public Text Conversation;
    public int count = 0;
    public AudioSource light;

    void OnEnable()
    {
        StartCoroutine(PlaySound(light, 3f));
        NextWord();
    }

    public void NextWord()
    {
        if (count < ConversationList.Count)
        {
            string str = ConversationList[count].ToString();
            if (str.Contains("\\"))
            {
                str = str.Split('\\')[0] + "\n" + str.Split('\\')[1];
            }
            Conversation.text = str; //"<color=red> 操作步骤 </color>\n关火 -> 停气 -> 抬盖"
            count += 1;
        }
    }

	public void LoadScene (string sc)
    {
        if (count < ConversationList.Count) return;
        else SceneManager.LoadScene(sc);
	}

    IEnumerator PlaySound(AudioSource _audio, float _time)
    {
        yield return new WaitForSeconds(_time);
        _audio.Play();
    }
}
