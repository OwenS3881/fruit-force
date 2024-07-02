using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {

      if (instance == null)
      {
        instance = this;
      }
      else
      {
        Destroy(gameObject);
        return;
      }

      DontDestroyOnLoad(gameObject);

      foreach (Sound s in sounds)
      {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;
        s.source.playOnAwake = false;
      }
    }

    public void PlaySound(string name)
    {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      if (s == null)
      {
        Debug.LogWarning("Sound: " + name + " was not found.");
        return;
      }
      if (s.source.isPlaying)
        {
            //Debug.Log("Sound: " + name + " is already playing");
            return;
        }
      s.source.Play();
    }

    public void PlaySoundOneShot(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " was not found.");
            return;
        }
        s.source.PlayOneShot(s.clip);
    }

    public void StopSound(string name)
    {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      if (s == null)
      {
        Debug.LogWarning("Sound: " + name + " was not found.");
        return;
      }
        if (!s.source.isPlaying)
        {
            //Debug.Log("Sound: " + name + " is not playing");
            return;
        }
        s.source.Stop();
    }
}
