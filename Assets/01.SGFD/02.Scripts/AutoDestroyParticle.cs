using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.stopAction = ParticleSystemStopAction.Destroy; // 파티클 시스템이 종료되면 게임 오브젝트를 삭제합니다.
    }
}
