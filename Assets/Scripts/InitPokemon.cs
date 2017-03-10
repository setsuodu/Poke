using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitPokemon : MonoBehaviour
{
    string _name;
    public Text notice, pokemonName, pokemonCP;

    void Start()
    {
        _name = PlayerPrefs.GetString("name");
        if (!string.IsNullOrEmpty(_name))
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(true);
            }
            GameObject go = Instantiate(Resources.Load<GameObject>(_name));
            go.transform.SetParent(this.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = new Vector3(5,5,5);

            go.GetComponent<CapsuleCollider>().enabled = false; //父物体已经有collider，避免冲突
            go.GetComponent<AudioSource>().Play();
            pokemonName.text = _name;
            pokemonCP.text = Random.Range(0, 150).ToString();
            notice.text = "A wild " + _name + " appeared!";
            StartCoroutine(noticeClock());
        }
    }

    void OnDisable()
    {
        PlayerPrefs.DeleteKey("name");
    }

    IEnumerator noticeClock()
    {
        yield return new WaitForSeconds(1f);
        notice.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        notice.gameObject.SetActive(false);
    }
}
