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
            // 채팅 입력을 처리하는 부분은 NetworkManager에서 관리합니다.
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
        yield return new WaitForSeconds(10f); // 5초 후 이미지 비활성화
        chatImage.gameObject.SetActive(false);
        chatText.text = ""; // 텍스트 초기화
    }
}
