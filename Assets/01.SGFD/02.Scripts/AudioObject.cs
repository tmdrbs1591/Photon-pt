using UnityEngine;

public class AudioObject : MonoBehaviour
{
    private AudioSource aud;
    private bool following;
    private Transform follow;

    public void Initialize(AudioClip clip, float pitch, float volume, Transform follower)
    {
        aud = gameObject.GetComponent<AudioSource>();
        if (aud == null)
        {
            aud = gameObject.AddComponent<AudioSource>();
        }

        aud.clip = clip;
        aud.pitch = pitch;
        aud.volume = volume;
        aud.Play();

        follow = follower;
        following = follow != null;
    }

    void Update()
    {
        if (follow != null)
        {
            transform.position = new Vector3(follow.position.x, follow.position.y, -5);
        }

        if (following && follow == null)
        {
            Destroy(gameObject);
        }

        if (!aud.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
