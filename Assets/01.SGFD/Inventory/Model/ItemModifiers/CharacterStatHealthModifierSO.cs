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
            // 자신의 클라이언트에서만 RPC 호출
            health.photonView.RPC("IncreaseHealth", RpcTarget.All, val);
        }
    }
}
