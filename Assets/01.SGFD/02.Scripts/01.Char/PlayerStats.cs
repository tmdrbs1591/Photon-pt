using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerStats : MonoBehaviourPun, IPunObservable
{
    public float speed; // 이동 속도
    public float attackCoolTime = 0.5f;
    public float attackPower = 1f; // 변경된 attackPower 값을 직접 사용하지 않기 위해 private 필드로 변경

    public float maxHp;
    public float curHp;

    public float skillCoolTime = 5f; // 스킬 쿨타임 설정

    public LevelUp uiLevelUp;

    [Header("레벨")]
    public int playerLevel = 1;
    public float currentXp; // 현재 경험치
    public float xp = 100; // 총경험치

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

            // Photon RPC 호출
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
        Update(); // UI 업데이트
    }

    // IPunObservable 인터페이스 구현
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터 전송
            stream.SendNext(playerLevel);
            stream.SendNext(currentXp);
            stream.SendNext(xp);
        }
        else
        {
            // 데이터 수신
            playerLevel = (int)stream.ReceiveNext();
            currentXp = (float)stream.ReceiveNext();
            xp = (float)stream.ReceiveNext();
        }
    }
}
