using UnityEngine;
using System.Collections;

public class MagmaBoss : MonoBehaviour {

    public Health master;
    public GameObject guide1;
    public GameObject guide2;
    public GameObject gate;
    public AudioClip battleMusic;
    public bool saveState = false;
    public Vector3 presentOffset = Vector3.zero;
    
    AudioClip roomMusic;

    int hp;

    void Awake () {
    }

    void Start () {
        if (PlayerPrefs.HasKey(name)) {
            gameObject.SetActiveRecursively(false);
            enabled = false;
            master.gameObject.SetActiveRecursively(false);
        }
        roomMusic = Game.game.musicClip;
    }
    
    void OnHotspotEnter () {
		hp = master.HP;
		StartCoroutine(Sequence());
    }

    IEnumerator Sequence () {
        yield return StartCoroutine(Present());
        yield return StartCoroutine(Fight());
        yield return StartCoroutine(Victory());
    }

    IEnumerator Meter () {
        AudioClip meterClip = (AudioClip)Resources.Load("Meter", typeof(AudioClip));
        for (int i=0; i<20; ++i) {
            Game.fx.PlaySound(meterClip);
            Game.hud.SetMeter(i/20.0f);
            yield return StartCoroutine(WaitForRealSeconds(0.1f));
        }
    }

    void OnHotspotEnter (Hotspot hotspot) {
        
    }

    IEnumerator WaitForRealSeconds(float time) {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < time)
            yield return 0;
    }

    IEnumerator Present () {
        yield return new WaitForSeconds(1);
        string[] message = TextTable.GetLines(name);
        if (Game.character != null) {
            Game.character.Pause();
        }
        //Time.timeScale = 0;
        Game.fx.SetLetterBox(true);
        float directorTime = 0;
        Vector3 directorStart = Game.director.transform.position;
        Vector3 directorEnd = 
            master.transform.position + new Vector3(0, 0, Character.cameraDistance) + presentOffset;
        while (directorTime < 1) {
            Game.director.transform.position = Vector3.Lerp(directorStart, directorEnd, directorTime);
            directorTime += Game.realDeltaTime;
            yield return 0;
        }
        if (message.Length == 1) {
            Game.hud.Message(message[0], 2);
        }
        yield return StartCoroutine(Meter());
        if (Game.character != null) {
            Game.character.Continue();
        }
        //Time.timeScale = 1;
        Game.fx.SetLetterBox(false);
        Game.fx.PlayMusic(battleMusic);
    }

    IEnumerator Fight () {
        while (master != null && master.Alive()) {
            Game.hud.SetMeter(master.HP/(float)hp);
            yield return 1;
        }
        Game.hud.HideMeter();
        yield return new WaitForSeconds(2);
    }

    IEnumerator Victory () {
    	
    	Game.fx.StopMusic();
        
        if (Game.character != null) {
            Game.character.Pause();
        }

        Game.fx.SetLetterBox(true);
        
        float directorTime = 0;
        Vector3 directorStart = Game.director.transform.position;
        Vector3 directorEnd = 
            guide1.transform.position + new Vector3(0, 0, Character.cameraDistance);
        while (directorTime < 1) {
            Game.director.transform.position = Vector3.Lerp(directorStart, directorEnd, directorTime);
            directorTime += Game.realDeltaTime;
            yield return 0;
        }
        yield return StartCoroutine(WaitForRealSeconds(8));
        
        directorTime = 0;
        directorStart = Game.director.transform.position;
        directorEnd = 
            guide2.transform.position + new Vector3(0, 0, Character.cameraDistance);
        while (directorTime < 1) {
            Game.director.transform.position = Vector3.Lerp(directorStart, directorEnd, directorTime);
            directorTime += Game.realDeltaTime;
            yield return 0;
        }
        yield return StartCoroutine(WaitForRealSeconds(1));
        
        gate.SendMessage("OnSwitch", null, SendMessageOptions.DontRequireReceiver);
        yield return StartCoroutine(WaitForRealSeconds(3));
        
        Game.fx.PlayVictoryMusic();
        
        PlayerPrefs.SetInt(name, 1);
        yield return StartCoroutine(WaitForRealSeconds(1));
        
        if (Game.character != null) {
            Game.character.Continue();
        }

        Game.fx.SetLetterBox(false);
        
        yield return new WaitForSeconds(7);
        Game.fx.PlayMusic(roomMusic);
    }

}
