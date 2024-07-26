using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;  // �����̴��� ����ϱ� ���� ���ӽ����̽�

public class HpbarListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;        // �÷��̾� �̸��� ǥ���� �ؽ�Ʈ
    [SerializeField] Slider hpSlider;      // HP�� ǥ���� �����̴�
    [SerializeField] TMP_Text levelText;   // ������ ǥ���� �ؽ�Ʈ
    Player player;

    public void Setup(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
        UpdateHPBar(); // �ʱ� HP �� ���� ������Ʈ
    }

    // HP �ٿ� ������ ������Ʈ�ϴ� �޼���
    void UpdateHPBar()
    {
        PlayerStats playerStats = GetPlayerStatsByNickName(text.text);
        if (playerStats != null)
        {
            // HP �����̴��� �� ���� (������ ǥ��)
            hpSlider.value = playerStats.curHp / playerStats.maxHp;

            // ���� �ؽ�Ʈ ����
            levelText.text = "LV." + playerStats.playerLevel.ToString();
        }
    }

    // �г������� PlayerStats�� ã�� �޼���
    PlayerStats GetPlayerStatsByNickName(string nickName)
    {
        foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView photonView = playerObject.GetComponent<PhotonView>();
            if (photonView != null && photonView.Owner.NickName == nickName)
            {
                return playerObject.GetComponent<PlayerStats>();
            }
        }
        return null; // �÷��̾ ã�� ���� ���
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        UpdateHPBar();  // �� ������ HP�� ������ ������Ʈ (�ֽ� ���� �ݿ�)
    }
}
