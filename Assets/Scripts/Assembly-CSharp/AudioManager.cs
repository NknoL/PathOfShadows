using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public Sound[] sounds;

	private void Awake()
	{
		Sound[] array = sounds;
		foreach (Sound sound in array)
		{
			sound.source = base.gameObject.AddComponent<AudioSource>();
			sound.source.clip = sound.clip;
			sound.source.volume = sound.volume;
			sound.source.pitch = sound.pitch;
			sound.source.loop = sound.loop;
		}
	}

	public void Play(string name)
	{
		Array.Find(sounds, (Sound sounds) => sounds.name == name).source.Play();
	}

	public void Pause(string name)
	{
		Array.Find(sounds, (Sound sounds) => sounds.name == name).source.Pause();
	}
}
