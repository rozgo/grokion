public var avatar : GameObject;
public var spirit : GameObject;
public var chunks : ParticleEmitter;
public var lightningFX : ParticleEmitter; 
public var chunksSFX : AudioClip;
public var lightningSFX : AudioClip;
public var checkpoint : Checkpoint;
public var guides : GameObject[];
public var music : AudioClip;

function Awake () {

	if (PlayerPrefs.HasKey(name)) {
		avatar.SetActiveRecursively(false);
		spirit.SetActiveRecursively(false);
		chunks.gameObject.active = false;
		gameObject.SetActiveRecursively(false);
		lightningFX.gameObject.active = false;
	}
	else {
		checkpoint.gameObject.active = false;
	}

}

function Start () {
	
	yield;
	
	var vignette : Vignette = GetComponent(Vignette);
	
	Game.director.transform.position = guides[0].transform.position;
	Game.director.transform.rotation = Quaternion.Euler(0, 180, 0);
	
	avatar.animation["CUT_INTRO01"].speed = 0;
	avatar.animation.Play("CUT_INTRO01");
	
	spirit.animation["RUN"].speed = 0;
	spirit.animation.Play("RUN");
	
	chunks.gameObject.active = false;
	lightningFX.gameObject.active = false;
	
	spirit.animation["RUN"].speed = 1;
	vignette.MoveTo(Game.director.gameObject, guides[1], 5);
	
	yield WaitForSeconds(4);
	
	Game.fx.SetLetterBox(true);
	
	yield WaitForSeconds(1);
	
	yield Game.hud.StartCoroutine(Game.hud.Dialog("Vignette|AO|1"));
	
	
	spirit.animation.Play("WAKE");
	spirit.animation["WAKE"].speed = 1;
	
	yield WaitForSeconds(spirit.animation["WAKE"].length /2);
	
	avatar.animation["CUT_INTRO01"].speed = 1;
	
	chunks.gameObject.active = true;
	Game.fx.PlaySound(chunksSFX);
	
	yield WaitForSeconds(0.5);
	
	lightningFX.gameObject.active = true;
	Game.fx.PlaySound(lightningSFX);
	
	yield WaitForSeconds(avatar.animation["CUT_INTRO01"].length);
	avatar.animation["IDLE"].speed = 1;
	avatar.animation["IDLE"].wrapMode = WrapMode.Loop;
	avatar.animation.CrossFade("IDLE");
	
	yield Game.hud.StartCoroutine(Game.hud.Dialog("Vignette|AO|2"));
	
	spirit.animation.Play("END");
	spirit.animation["END"].speed = 1;
	
	yield WaitForSeconds(spirit.animation["END"].length);
	
	
	
	spirit.SetActiveRecursively(false);

	Game.fx.SetLetterBox(false);

	yield WaitForSeconds(1);
	
	checkpoint.gameObject.active = true;
	checkpoint.Spawn();
	
	avatar.SetActiveRecursively(false);
	
	Game.fx.PlayMusic(music);
	

	
	PlayerPrefs.SetInt(name, 1);
}
