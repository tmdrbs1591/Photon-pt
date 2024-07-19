using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Character
{
    Knight, Archer , Dragoon
}

public class CharManager : MonoBehaviour
{

    public static CharManager instance;  // �̱��� ������ ���� ���� �ν��Ͻ� ����
                                         // Start is called before the first frame update
    void Start()
    {
        if (instance == null) instance = this; // �ν��Ͻ��� null�� ���, �ڱ� �ڽ��� instance�� �Ҵ�
        else if (instance != null) return; // �̹� �ٸ� �ν��Ͻ��� �����ϸ�, �� �ν��Ͻ��� �ı��ϰ� �����մϴ�.
        DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ��ü�� �ı����� �ʵ��� �����մϴ�.
    }

    public Character currentCharacter; // ���� ���õ� ĳ���͸� ������ ����

    public void CharChangeKnight()
    {
        currentCharacter = Character.Knight;
    }
    public void CharChangeArcher()
    {
        currentCharacter = Character.Archer;
    }
    public void CharChangeDragoon()
    {
        currentCharacter = Character.Dragoon;
    }
}
