using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun;

public class PlayerChat : MonoBehaviourPun
{
    public Image chatImage;
    public TMP_Text chatText;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // ä�� �Է��� ó���ϴ� �κ��� NetworkManager���� �����մϴ�.
        }

        if (Input.GetKeyDown(KeyCode.O) && photonView.IsMine)
        {
            PhotonNetwork.Instantiate("Grenade", transform.position, Quaternion.identity);
        }
    }

    [PunRPC]
    public void ActivateChatImage(string message)
    {
        chatImage.gameObject.SetActive(false);
        chatImage.gameObject.SetActive(true);
        chatText.text = message;
        //StartCoroutine(DeactivateChatImageAfterDelay());
    }

    private IEnumerator DeactivateChatImageAfterDelay()
    {
        yield return new WaitForSeconds(10f); // 5�� �� �̹��� ��Ȱ��ȭ
        chatImage.gameObject.SetActive(false);
        chatText.text = ""; // �ؽ�Ʈ �ʱ�ȭ
    }
}
