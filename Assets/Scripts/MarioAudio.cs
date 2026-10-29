using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioAudio : MonoBehaviour
{
    private static MarioAudio instance;
    private readonly Dictionary<string,AudioClip> clips=new Dictionary<string,AudioClip>();
    private AudioSource music, effects;
    private void Awake() {
        instance=this; music=gameObject.AddComponent<AudioSource>(); effects=gameObject.AddComponent<AudioSource>();
        music.loop=true; music.volume=.18f; effects.volume=.45f;
        clips["jump"]=Tone(350,950,.14f); clips["stomp"]=Tone(180,50,.13f);
        clips["coin"]=Tone(1000,1700,.12f); clips["powerup"]=Tone(300,1400,.5f);
        clips["damage"]=Tone(500,90,.3f); clips["death"]=Tone(550,60,.9f);
        clips["win"]=Tone(400,1300,.65f);
        SceneManager.sceneLoaded+=Loaded;
    }
    private void OnDestroy() { SceneManager.sceneLoaded-=Loaded; if(instance==this)instance=null; }
    private void Start() { if(music.clip==null)Loaded(SceneManager.GetActiveScene(),LoadSceneMode.Single); }
    private void Loaded(Scene scene,LoadSceneMode mode) {
        music.Stop();
        if(scene.name=="GameOver") { Play("death"); return; }
        if(scene.name=="Victory") { Play("win"); return; }
        int theme=scene.name=="1-2"?1:scene.name=="1-3"?2:scene.name=="1-4"?3:0;
        music.clip=Melody(theme); music.Play();
    }
    public static void Play(string cue) { if(instance!=null && instance.clips.TryGetValue(cue,out var clip)) instance.effects.PlayOneShot(clip); }
    private static AudioClip Tone(float from,float to,float duration) {
        int n=(int)(22050*duration); var samples=new float[n]; float phase=0;
        for(int i=0;i<n;i++){float t=i/(float)n; phase+=Mathf.Lerp(from,to,t)/22050; samples[i]=(Mathf.Repeat(phase,1)<.5f?1:-1)*.22f*(1-t)*Mathf.Min(1,t*50);}
        var clip=AudioClip.Create("SFX",n,1,22050,false);clip.SetData(samples,0);return clip;
    }
    private static AudioClip Melody(int theme) {
        int[][] notes={new[]{64,67,72,67,69,65,62,67,60,64,67,64,62,59,60,0},new[]{48,60,51,63,50,62,46,58,48,55,60,55,46,53,58,0},new[]{60,64,67,72,71,67,65,62,60,64,69,72,74,71,67,0},new[]{45,52,57,58,57,52,48,46,45,52,57,60,58,55,52,0}};
        float beat=theme==2?.27f:.19f;int per=(int)(22050*beat);float[] samples=new float[per*32];
        for(int i=0;i<samples.Length;i++){int step=i/per;int note=notes[theme][step%16];float t=(i%per)/(float)per; if(note==0)continue;float hz=440*Mathf.Pow(2,(note-69)/12f);float p=i/22050f; samples[i]=(.16f*Mathf.Sin(2*Mathf.PI*hz*p)+.08f*Mathf.Sin(2*Mathf.PI*hz*.5f*p))*(1-t);}
        var clip=AudioClip.Create("Ambiente "+theme,samples.Length,1,22050,false);clip.SetData(samples,0);return clip;
    }
}
