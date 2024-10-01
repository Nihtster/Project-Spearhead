using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

///<summary>A Scriptable Object for containing an audio clip, and its settings, to be created at runtim</summary>
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Create New Sound Library", order = 0)]
public class SoundLibrary : ScriptableObject {
  ///<summary>A list of all the sounds in the project</summary>
  [Tooltip("A list of all the sounds in the project")]
  public List<Sound> sounds;
}

[System.Serializable]
///<summary>The class structure containing most the information related to an audio sourc</summary>
public class Sound {
  ///<summary>The name of the sound file to be used by scripts when playing the audio clip</summary>
  [Tooltip("The name of the sound file to be used by scripts when playing the audio clip")]
  public string name;
  ///<summary>The audio clip, as well as any random alternates to be played</summary>
  [Tooltip("The audio clip, as well as any random alternates to be played")]
  public List<AudioClip> clips;

  [Range(0f, 1f)]
  public float volume = 1f;
  [Range(0.1f, 3f)]
	public float pitch = 1f;
  [Range(0f, 1f)]
  ///<summary>A percent value representing the maximum amount of random change in pitch in the positive or negative direction</summary>
  [Tooltip("A percent value representing the maximum amount of random change in pitch in the positive or negative direction")]
  public float pitchVariance = 0;
	[Range(0f, 1f)]
  ///<summary>How much of a 3D effect the audio source has</summary>
	[Tooltip("How much of a 3D effect the audio source has")]
  public float spacialBlend = 1f;
	public bool loop = false;
  ///<summary>How the audio should fade with distance. Should be linear for sounds heard from a decent distance away</summary>
  [Tooltip("How the audio should fade with distance. Should be linear for sounds heard from a decent distance away")]
	public AudioRolloffMode audioRolloffMode = AudioRolloffMode.Linear;
	public bool bypassEffects = false;
	public AudioMixerGroup audioMixerGroup;
  ///<summary>Whether the audio source immedietly plays when its created</summary>
  [Tooltip("Whether the audio source immedietly plays when its created")]
	public bool playOnAwake = true;
}