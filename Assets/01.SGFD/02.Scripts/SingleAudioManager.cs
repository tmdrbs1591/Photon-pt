using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleAudioManager : MonoBehaviour
{
    // 오디오 클립 배열
    public AudioClip[] clips;
    public AudioClip[] songs;
    public GameObject audioObject;
    public static SingleAudioManager instance;
    private AudioSource aud;
    private float initVolume;
    private int curSong;

    void Awake()
    {
        // 현재 오디오 매니저 오브젝트가 파괴되지 않도록 설정
        // DontDestroyOnLoad(gameObject);

        // 이미 존재하는 오디오 리스너를 찾음
        AudioListener existingListener = FindObjectOfType<AudioListener>();

        // 이미 오디오 리스너가 있으면 이를 비활성화
        if (existingListener != null)
        {
            existingListener.enabled = false;
        }
        else
        {
            // 오디오 리스너가 없으면 생성
            gameObject.AddComponent<AudioListener>();
        }

        // 싱글턴 인스턴스 설정
        instance = this;

        // 오디오 소스 컴포넌트를 가져옴
        aud = gameObject.GetComponent<AudioSource>();

        // 오디오 소스 컴포넌트가 없으면 추가
        if (aud == null)
        {
            aud = gameObject.AddComponent<AudioSource>();
        }

        // 초기 볼륨 설정
        initVolume = aud.volume;
    }

    // 지정된 인덱스의 노래로 전환
    public IEnumerator SwitchSong(int index)
    {
        // 현재 재생 중인 노래와 동일하면 실행 중지
        if (curSong == index) yield break;

        // 현재 노래의 볼륨을 점진적으로 줄임
        for (float i = 0; i < 1; i += Time.unscaledDeltaTime)
        {
            aud.volume = Mathf.Lerp(initVolume, 0, i);
            yield return null;
        }

        // 노래 정지 및 새로운 노래 설정 후 재생
        aud.Stop();
        aud.clip = songs[index];
        aud.Play();

        // 새로운 노래의 볼륨을 점진적으로 증가
        for (float i = 0; i < 1; i += Time.unscaledDeltaTime)
        {
            aud.volume = Mathf.Lerp(0, initVolume, i);
            yield return null;
        }

        // 현재 노래 인덱스 업데이트
        curSong = index;
        yield break;
    }

    // 지정된 위치에서 사운드 재생
    public void PlaySound(Vector3 position, int index, float pitch = 1, float volume = 1, Transform follower = null)
    {
        // 오디오 오브젝트 생성
        SingleAudioObject aud = GameObject.Instantiate(audioObject, new Vector3(position.x, position.y, -5), Quaternion.identity).GetComponent<SingleAudioObject>();

        // 오디오 오브젝트를 따라다닐 대상 설정
        aud.follow = follower;

        // 오디오 클립 설정
        aud.clip = clips[index];

        // 피치 설정
        aud.pitch = pitch;

        // 볼륨 설정
        volume = aud.volume;
    }
}
