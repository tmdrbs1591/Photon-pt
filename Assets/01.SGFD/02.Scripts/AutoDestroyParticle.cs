using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.stopAction = ParticleSystemStopAction.Destroy; // ��ƼŬ �ý����� ����Ǹ� ���� ������Ʈ�� �����մϴ�.
    }
}
