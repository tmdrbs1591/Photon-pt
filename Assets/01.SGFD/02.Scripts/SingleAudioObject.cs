using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAudioObject : MonoBehaviour
{
    // 오디오 소스 컴포넌트
    private AudioSource aud;

    // 오디오 클립
    public AudioClip clip;

    // 오디오 피치
    public float pitch = 1;

    // 오디오 볼륨
    public float volume = 1;

    // 따라다닐 트랜스폼
    public Transform follow;

    // 오브젝트가 따라다니는지 여부
    private bool following;

    void Start()
    {
        // 오디오 소스 컴포넌트를 가져옴
        aud = gameObject.GetComponent<AudioSource>();

        // 오디오 소스 설정
        aud.clip = clip;
        aud.pitch = pitch;
        aud.volume = volume;

        // 오디오 재생
        aud.Play();

        // 따라다닐 트랜스폼이 설정되어 있는지 여부 확인
        following = follow != null;
    }

    // 매 프레임마다 호출되는 업데이트 메서드
    void Update()
    {
        // 따라다닐 트랜스폼이 있으면 오브젝트 위치를 업데이트
        if (follow != null)
            transform.position = new Vector3(follow.position.x, follow.position.y, -5);

        // 따라다니는 중에 트랜스폼이 없어지면 오브젝트를 파괴
        if (following && follow == null)
            Destroy(gameObject);

        // 오디오 재생이 끝나면 오브젝트를 파괴
        if (!aud.isPlaying)
            Destroy(gameObject); // ObjectPool.ReturnToPool("AudioObject", gameObject); // 풀링 시스템을 사용하는 경우
    }
}
