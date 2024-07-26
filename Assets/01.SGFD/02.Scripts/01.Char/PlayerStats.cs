using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerStats : MonoBehaviourPun, IPunObservable
{
    public float speed; // �̵� �ӵ�
    public float attackCoolTime = 0.5f;
    public float attackPower = 1f; // ����� attackPower ���� ���� ������� �ʱ� ���� private �ʵ�� ����

    public float maxHp;
    public float curHp;

    public float skillCoolTime = 5f; // ��ų ��Ÿ�� ����

    public LevelUp uiLevelUp;

    [Header("����")]
    public int playerLevel = 1;
    public float currentXp; // ���� ����ġ
    public float xp = 100; // �Ѱ���ġ

    [SerializeField] Slider xpSlider;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text xpText;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        Player_XP();
    }

    // Update is called once per frame
    void Update()
    {
        xpSlider.value = Mathf.Lerp(xpSlider.value, currentXp / xp, Time.deltaTime * 40f);
        levelText.text = "LV." + playerLevel.ToString();
        xpText.text = currentXp + "/" + xp;
    }

    public void Player_XP()
    {
        xp = playerLevel * 100;
    }

    public void LV_UP()
    {
        if (currentXp >= xp)
        {
            AudioManager.instance.PlaySound(transform.position, 7, Random.Range(1f, 1f), 0.4f);
            currentXp -= xp;
            playerLevel++;
            Player_XP();

            // Photon RPC ȣ��
            photonView.RPC("UpdatePlayerStats", RpcTarget.AllBuffered, playerLevel, currentXp, xp);
            uiLevelUp.Show();
        }
    }

    [PunRPC]
    void UpdatePlayerStats(int level, float currentXp, float xp)
    {
        this.playerLevel = level;
        this.currentXp = currentXp;
        this.xp = xp;
        Update(); // UI ������Ʈ
    }

    // IPunObservable �������̽� ����
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ������ ����
            stream.SendNext(playerLevel);
            stream.SendNext(currentXp);
            stream.SendNext(xp);
        }
        else
        {
            // ������ ����
            playerLevel = (int)stream.ReceiveNext();
            currentXp = (float)stream.ReceiveNext();
            xp = (float)stream.ReceiveNext();
        }
    }
}
