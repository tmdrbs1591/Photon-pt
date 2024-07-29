using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] Image fadeIn;
    [SerializeField] Image fadeOut;
    // Start is called before the first frame update
    void Start()
    {
        fadeOut.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(string sceneName)
    {
       StartCoroutine( FadeScene(sceneName));
    }
    IEnumerator FadeScene(string sceneName)
    {
        fadeIn.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(sceneName);
    }
}
