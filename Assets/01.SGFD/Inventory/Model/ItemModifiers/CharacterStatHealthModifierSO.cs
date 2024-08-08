using UnityEngine;
using Photon.Pun;

[CreateAssetMenu]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        PlayerStats health = character.GetComponent<PlayerStats>();
        if (health != null && health.photonView.IsMine)
        {
            // �ڽ��� Ŭ���̾�Ʈ������ RPC ȣ��
            health.photonView.RPC("IncreaseHealth", RpcTarget.All, val);
        }
    }
}
