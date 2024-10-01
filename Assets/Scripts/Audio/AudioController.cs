using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>The main class for automatically handling audio sources. To use in code, simply call AudioController.Play("sound_name_here"); See the full function for all parameters</summary>
public class AudioController : MonoBehaviour {

	///<summary>The singleton reference of the active AudioController</summary>
	public static AudioController singleton;

	private void Awake() {
		//Set up the singleton reference
		if(singleton != null) {
			Debug.LogWarning("There are two AudioControllers! Please ensure there's only one AudioController in the scene at a time.");
			Destroy(this);
			return;
		} else {
			DontDestroyOnLoad(this);
			singleton = this;
		}

		//Load the library ScriptableObject from the resources folder
		library = Resources.Load<SoundLibrary>("SoundLibrary");

		//Below are security checks to make sure everything is loading correctly
		if(library == null) {
			Debug.LogError("AudioController SoundLibrary is null! Please make sure there is a SoundLibrary.asset file located in the Unity Project's Resources folder.");
		}

		List<string> names = new List<string>();
		foreach(Sound sound in library.sounds) {
			//Check for conflicting names
			if(names.Contains(sound.name)) {
				Debug.LogError("Sound " + sound.name + " has two entries in the library! Please ensure every sound name is unique!");
			}
			names.Add(sound.name);

			//Check for missing audio clips
			foreach(AudioClip clip in sound.clips) {
				if(clip == null) {
					Debug.LogError("Sound " + sound.name + " has a missing audio clip! Please make sure every clip has a working reference!");
				}
			}
			//Check for empty clip array
			if(sound.clips.ToArray().Length == 0) {
				Debug.LogWarning("Sound " + sound.name + " doesn't have any clips, so will never play any audio.");
			}

			//Check for missing audio mixer groups
			if(sound.audioMixerGroup == null) {
				Debug.LogWarning("Sound " + sound.name + " is not a part of any AudioMixerGroup. Consider assigning it to an AudioMixerGroup.");
			}
		}
		Debug.Log("Audio Controller loaded succesfully!\nSounds loaded: " + library.sounds.ToArray().Length);
	}

	///<summary>The reference sound library used to look up information on the audio sources</summary>
	[HideInInspector]
	public static SoundLibrary library;

	///<summary>Finds and returns a sound with a given name</summary>
	///<param name="soundName">The name of the sound to search for</param>
	///<returns>The Sound with the specified soundName</returns>
	public static Sound GetSound(string soundName) {
		foreach(Sound sound in library.sounds) {
			if(sound.name == soundName) { return sound; }
		}
		Debug.LogError("GetSound(" + soundName + ") found no sound in the library!");
		return null;
	}

	///<summary>Create and play a sound based on its reference</summary>
	///<param name='soundName'>The name of the sound reference to be played</param>
	///<param name='parent'>The transform parent the sound should be attached to</param>
	///<param name='position'>The position offset for the sound to start with</param>
	///<param name='localSpace'>Whether the position coordinates are in local or global space</param>
	///<param name='deleteAfterPlay'>Whether the object should be deleted immediately after finishing playing a sound</param>
	///<param name='pitchVariance'>The percent pitch variance in either direction</param>
	public static AudioSource PlaySound(string soundName, Transform parent = null, Vector3 position = default(Vector3), bool localSpace = true, bool deleteAfterPlay = true) {
		return PlaySound(GetSound(soundName), parent, position, localSpace, deleteAfterPlay);
	}

	///<summary>Create and play a sound based on its reference</summary>
	///<param name='sound'>The sound reference to be played</param>
	///<param name='parent'>The transform parent the sound should be attached to</param>
	///<param name='position'>The position offset for the sound to start with</param>
	///<param name='localSpace'>Whether the position coordinates are in local or global space</param>
	///<param name='deleteAfterPlay'>Whether the object should be deleted immediately after finishing playing a sound</param>
	///<param name='pitchVariance'>The percent pitch variance in either direction</param>
	public static AudioSource PlaySound(Sound sound, Transform parent = null, Vector3 position = default(Vector3), bool localSpace = true, bool deleteAfterPlay = true) {
		//Create a new gameobject and parent it to the transform of the object (if provided)
		GameObject go = new GameObject(sound.name);
		go.SetActive(false);
		go.transform.parent = parent;
		//Set position
		if(localSpace) { go.transform.localPosition = position; }
		else { go.transform.position = position; }
		
		AudioSource source = go.AddComponent<AudioSource>();
		if(deleteAfterPlay) { DeleteAfterPlayingSounds(source); }
		//Choose a random clip from the list of clips of the sound
		AudioClip clip = sound.clips[Random.Range(0, sound.clips.ToArray().Length)];
		//Assign all the values from the reference to the component
		source.clip = clip;
		source.volume = sound.volume;
		source.pitch = sound.pitch;
		source.loop = sound.loop;
		source.spatialBlend = sound.spacialBlend;
		source.rolloffMode = sound.audioRolloffMode;
		source.bypassEffects = sound.bypassEffects;
		source.outputAudioMixerGroup = sound.audioMixerGroup;
		source.playOnAwake = sound.playOnAwake;
		go.SetActive(true);
		return source;
	}

	///<summary>Automatically delete an audio source and its respective empty gameobject when its finished playing</summary>
	///<param name="source">The audio source to be deleted once done playing</param>
  private static void DeleteAfterPlayingSounds(AudioSource source) {
		singleton.StartCoroutine(DeleteAfterPlayingSoundsCoroutine(source));
  }

	private static IEnumerator DeleteAfterPlayingSoundsCoroutine(AudioSource sound) {
		yield return new WaitForSeconds(1);
		yield return new WaitUntil(() => sound == null || !sound.isPlaying);
		if(sound) { Destroy(sound.gameObject); }
	}
}