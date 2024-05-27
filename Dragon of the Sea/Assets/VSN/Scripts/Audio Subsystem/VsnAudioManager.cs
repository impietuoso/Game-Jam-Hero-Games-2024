using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioCooldown {
  public string clipName;
  public float duration;
}


public class VsnAudioManager : MonoBehaviour {

	public static VsnAudioManager instance;

  public AudioMixer mixer;
  public AudioSource audioSource;
  public AudioSource dialogSfxSource;
  public PartialLoopPlayer musicPlayer;
  public AudioSource[] ambienceSources;
  public AudioClip defaultDialogSfx = null;
  public AudioClip currentDialogSfx = null;
  public AudioClip dialogAdvanceSfx = null;
  public float dialogSfxTime = 0.08f;


  public string songPlaying;
  public AudioClip lastSongPlayingIntro;
  public AudioClip lastSongPlayingLoop;
  public float lastSongTimePlayed;
  public string[] pathsToBuffer;
  public List<AudioClip> bufferedAudioClips;

  public List<AudioCooldown> clipCooldowns;

  public const string masterVolumeParameter = "MasterVolume";
  public const string musicVolumeParameter = "MusicVolume";
  public const string sfxVolumeParameter = "SfxVolume";



  void Awake(){
    if(instance == null) {
      instance = this;
      DontDestroyOnLoad(gameObject);
    } else if(instance != this){
      Destroy(gameObject);
      return;
    }
    bufferedAudioClips = new List<AudioClip>();
    BufferAudioPaths();
  }
   

  void Start() {
    LoadVolumes();
  }


  //[Button]
  public void LoadVolumes() {
    Debug.LogWarning("Initializing volumes");
    SetGroupVolume(masterVolumeParameter, PlayerPrefs.GetInt(masterVolumeParameter, 10));
    SetGroupVolume(musicVolumeParameter, PlayerPrefs.GetInt(musicVolumeParameter, 10));
    SetGroupVolume(sfxVolumeParameter, PlayerPrefs.GetInt(sfxVolumeParameter, 7));
  }

  public void SetGroupVolume(string volumeParameter, int value) {
    float converted = ConvertToLogValue(value);
    Debug.LogWarning("Setting group " + volumeParameter + " to: " + value + ". converted to: " + converted);

    mixer.SetFloat(volumeParameter, converted);
  }

  public float ConvertToLogValue(float sliderValue) {
    sliderValue = Mathf.Max(sliderValue, 0.0001f);
    return Mathf.Log10(sliderValue / 10f) * 20f;
  }



  void BufferAudioPaths(){
    foreach(string currentPath in pathsToBuffer){
//      Debug.LogWarning("Buffering path Resources/" + currentPath);
      bufferedAudioClips.AddRange( Resources.LoadAll<AudioClip>(currentPath) );
    }
  }

  public void BufferAudioClip(string audioPath){
    bufferedAudioClips.Add(Resources.Load<AudioClip>(audioPath));
  }

  public void UnbufferAudioClip(string clipName) {
    for (int i = 0; i < bufferedAudioClips.Count; i++) {
      if (bufferedAudioClips[i].name == clipName) {
        bufferedAudioClips.RemoveAt(i);
        return;
      }
    }
  }

  AudioClip GetBufferedClip(string clipName){
    foreach(AudioClip clip in bufferedAudioClips){
      if(clip.name == clipName){
        return clip;
      }
    }
    return null;
  }

  public AudioClip GetAudioClip(string clipName){
    if(clipName == null){
      return null;
    }
  
    AudioClip clip = null;
    clip = GetBufferedClip(clipName);
    if(clip == null) {
      clip = Resources.Load<AudioClip>(clipName);
      //if(clip == null) {
      //  Debug.LogWarning("Error loading audio clip: " + clipName + ". Please check the provided path.");
      //}
    }
    return clip;
  }


  public void PrepareMusic(string introMusic, string loopMusic, bool storeLastPlayed = true){
    if(introMusic == null &&
       loopMusic == null){
      Debug.LogWarning("Prepare: Setting music to null.");
      return;
    }

    AudioClip introClip = GetAudioClip(introMusic);
    AudioClip loopClip = GetAudioClip(loopMusic);
    if(loopClip != null) {
      songPlaying = loopClip.name;
    }

    FadeMusic(1f, 0f);
    musicPlayer.SetMusic(introClip, loopClip);

    if(storeLastPlayed){
      StoreLastSongPlayed();
    }
  }


  public void PlayMusic(string introMusic, string loopMusic, bool storeLastPlayed = true){
    //    Debug.LogWarning("Trying to play music: " + introMusic + " and " + loopMusic);
    if(GetPlayingMusicName() == GetNameFromPath(loopMusic) &&
       musicPlayer.introSource.volume!=0f && musicPlayer.loopSource.volume != 0f ) {
      Debug.LogWarning("wont play the same music");
      return;
    }

    if(introMusic == null && loopMusic == null){
      Debug.LogWarning("Setting music to null.");
      return;
    }

    AudioClip introClip = GetAudioClip(introMusic);
    AudioClip loopClip = GetAudioClip(loopMusic);
    if(loopClip != null) {
      songPlaying = loopClip.name;
    }

    FadeMusic(1f, 0f);
    musicPlayer.SetMusic(introClip, loopClip);
    musicPlayer.StartPlaying();

    if(storeLastPlayed){
      StoreLastSongPlayed();
    }
  }

  public string GetNameFromPath(string songPath){
    if(songPath != null){
      char delimiter = '/';
      string[] parts = songPath.Split(delimiter);
      return parts[parts.Length-1];
    }
    return null;
  }

  public void StopMusic(){
    musicPlayer.StopPlaying();
  }

  public string GetPlayingMusicName(){
    if(musicPlayer.loopSource.clip != null &&
       (musicPlayer.loopSource.isPlaying || musicPlayer.introSource.isPlaying)
      ){
      return musicPlayer.loopSource.clip.name;
    }
    return "";
  }



  public void PlaySfx(string clipName){
    PlaySfx(clipName, 0f);
  }

  public void FadeMusic(float fadeValue, float fadeTime) {
    musicPlayer.FadeMusic(fadeValue, fadeTime);
  }

  public void PlaySfx(string clipName, float panValue){
    AudioClip audioClip = GetAudioClip(clipName);
    if(audioClip == null){
      return;
    }
    PlaySfx(audioClip, panValue);
  }

  public void PlaySfx(AudioClip audioClip){
    PlaySfx(audioClip, 0f);
  }

  public void PlaySfx(AudioClip audioClip, float panValue){
    foreach(AudioCooldown ac in clipCooldowns) {
      if(ac.clipName == audioClip.name) {
        return;
      }
    }
    clipCooldowns.Add(new AudioCooldown {
      clipName = audioClip.name,
      duration = 0.1f
    });
    audioSource.PlayOneShot(audioClip);
	}

  public void SetDialogSfxPitch(float pitch) {
    dialogSfxSource.pitch = pitch;
  }

  public void SetDialogSfx(AudioClip clip) {
    if(clip != null) {
      Debug.LogWarning("Clip name: " + clip.name);
      dialogSfxSource.Stop();
      currentDialogSfx = clip;
    } else {
      currentDialogSfx = defaultDialogSfx;
    }    
  }

  public void PlayDialogSfx(){
    //PlaySfx("SFX/UI/dialogue_letter");
    return;
  }

  public void PlayDialogAdvanceSfx() {
    //PlaySfx("SFX/UI/dialogue_next");
    return;
  }

  public void PlayAmbience(string clipName){
    AudioClip audioClip = GetAudioClip(clipName);
    if(audioClip == null){
      Debug.LogWarning("Error loading " + clipName + " ambience sfx. Please check its path");
      return;
    }
    AudioSource s = GetAvailableAmbienceSource();
    s.clip = audioClip;
    s.Play();
  }

  public void StopAmbience(string clipName){
    foreach(AudioSource s in ambienceSources){
      if(s.clip != null && s.clip.name == clipName){
        s.Stop();
        s.clip = null;
      }
    }
  }


  AudioSource GetAvailableAmbienceSource(){
    for(int i=0; i<ambienceSources.Length; i++){
      if( !ambienceSources[i].isPlaying )
        return ambienceSources[i];
    }
    return null;
  }

  public void SetVolume(float volume){
    audioSource.volume = volume;
    musicPlayer.introSource.volume = volume;
    musicPlayer.loopSource.volume = volume;
    foreach(AudioSource s in ambienceSources){
      s.volume = volume;
    }
  }

  public void ResumeLastSong() {
    musicPlayer.SetMusic(lastSongPlayingIntro, lastSongPlayingLoop);
    musicPlayer.PlayFromTime(lastSongTimePlayed);
  }

  public void StoreLastSongPlayed() {
    lastSongPlayingIntro = musicPlayer.introSource.clip;
    lastSongPlayingLoop = musicPlayer.loopSource.clip;   
  }

  public void StoreLastSongPlayedTime() {
    lastSongTimePlayed = musicPlayer.GetPlayedTime(); 
  }

  public void Update() {
    foreach(AudioCooldown cool in clipCooldowns) {
      cool.duration -= Time.unscaledDeltaTime;
    }
    for(int i=0; i<clipCooldowns.Count; i++) {
      if(clipCooldowns[i].duration < 0f) {
        clipCooldowns.Remove(clipCooldowns[i]);
      }
    }
  }
}
