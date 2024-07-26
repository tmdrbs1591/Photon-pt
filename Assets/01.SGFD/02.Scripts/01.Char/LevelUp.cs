using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    public Augment[] augments;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        augments = GetComponentsInChildren<Augment>(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;

    }
    
    void Next()
    {
        foreach (Augment aug in augments)
        {
            aug.gameObject.SetActive(false);
        }

        int[] ran = new int[3];
        while(true)
        {
            ran[0] = Random.Range(0,augments.Length);
            ran[1] = Random.Range(0,augments.Length);
            ran[2] = Random.Range(0,augments.Length);

            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                break;
        }
        for (int index = 0; index < ran.Length; index++)
        {
            Augment ranAug = augments[ran[index]];
            //만렙이면 소비하는거로 바꾸기
            if (ranAug.level == ranAug.data.damages.Length)
            {
                augments[2].gameObject.SetActive(true);
            }
            else
            {
                ranAug.gameObject.SetActive(true);
            }
        }
        
    }
}
