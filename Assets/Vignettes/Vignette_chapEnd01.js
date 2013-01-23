
//These two are required for the sake of checking which armor is currently active.
public var playerPrefKey = "";
public var saveKey = false;

public var avatar : GameObject;
public var avatarBroken : GameObject;

// This one is for camera guides
public var guides : GameObject[];

//These are optional
public var seaCreature : GameObject; 
public var spirit : GameObject;
public var finalText : TextMesh;
public var finalText02 : TextMesh;

public var earth01 : AudioClip;
public var creature : AudioClip;
public var splash : ParticleEmitter[];
public var ripples : ParticleEmitter;

public var textLine01= "";
public var textLine02= "";
public var letterSound : AudioClip;


function Awake () {

	if (PlayerPrefs.HasKey(playerPrefKey)) { 
		gameObject.SetActiveRecursively(false);
	}
	else {
		
	}

}

function OnTriggerEnter(collider : Collider) {

// BEGINCODE -- STOP PLAYER AND REPLACE WITH CUTSCENE MODEL

 
	if (collider.gameObject.layer == Game.characterLayer) { // Checking to see if it has been triggered by the player
	
	if (Game.character.GetSuit() == Character.Suit.Armor){} // Check to see which armor is currently being used.
	else{
		avatar = avatarBroken; 
		}
	

yield;
	
	var vignette : Vignette = GetComponent(Vignette); // Fetches Vignette specific functions. Like move to and look at.
	
	while (Game.character.InJump() == true || Game.character.InFall() == true)
	{
		yield;
		}
	
	avatar.transform.position = Game.character.transform.position;
	spirit.transform.position = Game.character.transform.position;
	avatar.SetActiveRecursively(true);
	
	avatar.animation["SHOOT_RUN"].speed = 1;
	avatar.animation.Play("SHOOT_RUN");
	avatar.animation.CrossFade("IDLE", 0.3);
	
	
	
	
	Game.character.Pause();
	Game.character.Hide();



// ENDCODE -- STOP PLAYER AND REPLACE WITH CUTSCENE MODEL

	Game.fx.SetLetterBox(true);
	vignette.MoveTo(Game.director.gameObject, guides[0], 3);
	
	yield WaitForSeconds(1);
	avatar.animation.CrossFade("CUT_TURNZ", 0.1);
	yield WaitForSeconds(avatar.animation["CUT_TURNZ"].length /2);
	avatar.animation.CrossFade("CUT_TURNZIDLE", 0.4);
	avatar.animation.wrapMode = WrapMode.Loop;
	
	yield WaitForSeconds(1);
	spirit.SetActiveRecursively(true);
	spirit.animation["START"].speed = 1;
	spirit.animation.Play("START");
	
	yield WaitForSeconds(1);
	
	vignette.MoveTo(Game.director.gameObject, guides[1], 5);
	
	yield Game.hud.StartCoroutine(Game.hud.Dialog("Vignette|GM|1"));
	
	vignette.MoveTo(Game.director.gameObject, guides[2], 5);
	
	vignette.LookAt(Game.director.gameObject, guides[3], 5);
	ripples.gameObject.particleEmitter.emit = true;
	
	Game.fx.SetLetterBox(false);
	yield WaitForSeconds(3);
	ripples.gameObject.particleEmitter.minSize = 1.0;
	ripples.gameObject.particleEmitter.maxSize = 2.0;
	yield WaitForSeconds(1);
	Game.fx.PlaySound(earth01);
	
	Game.fx.QuickShake(2, 4);
	yield WaitForSeconds(2);
	Game.fx.QuickShake(6, 15);
	Game.fx.PlaySound(earth01);
	splash[0].gameObject.particleEmitter.emit = true;
	
	seaCreature.SetActiveRecursively(true);
	seaCreature.animation["START"].speed = 0.3;
	seaCreature.animation.Play("START");
	yield WaitForSeconds(2);
	splash[0].gameObject.particleEmitter.emit = false;
	splash[1].gameObject.particleEmitter.emit = true;
	Game.fx.PlaySound(creature);
	yield WaitForSeconds(2);
	splash[2].gameObject.particleEmitter.emit = true;
	splash[1].gameObject.particleEmitter.emit = false;
	
	
	PlayerPrefs.SetInt("chapterOneEnd", 1);
	
	
	yield WaitForSeconds(1);
	splash[2].gameObject.particleEmitter.emit = false;
	yield WaitForSeconds(3);
	ripples.gameObject.particleEmitter.emit = false;
	yield WaitForSeconds(2);
	Game.fx.dramaBox.renderer.enabled = true;
	
	finalText.transform.position = Game.fx.dramaBox.transform.position;
	finalText.transform.transform.rotation = Game.fx.dramaBox.transform.rotation;
	finalText.transform.Rotate(Vector3.right * 90,	Space.Self);
	finalText.text = "";
	finalText.renderer.enabled = true;
	
	finalText02.transform.position = Game.fx.dramaBox.transform.position;
	finalText02.transform.transform.rotation = Game.fx.dramaBox.transform.rotation;
	finalText02.transform.Rotate(Vector3.right * 90,	Space.Self);
	finalText02.transform.Translate(0.0,-0.15,0.0, Space.Self);
	finalText02.text = "";
	finalText02.renderer.enabled = true;
	
	 for (var letter in textLine01.ToCharArray()) {
        finalText.text += letter;
        Game.fx.PlaySound(letterSound);
        yield WaitForSeconds (Random.value / 4);
    }      
	
	for (var letter2 in textLine02.ToCharArray()) {
        finalText02.text += letter2;
        Game.fx.PlaySound(letterSound);
        yield WaitForSeconds (Random.value / 4);
    }    
	
	if (saveKey == true && playerPrefKey != ""){ //Check to see if SetKey is Activated
	PlayerPrefs.SetInt(playerPrefKey, 1); //Set Key
	}else{
		Debug.Log("Key was not set. Check to see if you gave a keyname");
	}
	
	yield WaitForSeconds(4);

	
Application.LoadLevel("StoryLoader"); 
	
	}
}








/* THE FIRST VIGNETTE EXAMPLE FROM ALEX. Its here for reference.



public var avatar : GameObject;
public var spirit : GameObject;
public var chunks : ParticleEmitter;
public var chunksSFX : AudioClip;
public var checkpoint : Checkpoint;
public var guides : GameObject[];
public var music : AudioClip;

function Awake () {

	if (PlayerPrefs.HasKey(name)) {
		avatar.SetActiveRecursively(false);
		spirit.SetActiveRecursively(false);
		chunks.gameObject.active = false;
		gameObject.SetActiveRecursively(false);
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
	
	yield WaitForSeconds(avatar.animation["CUT_INTRO01"].length);
	avatar.animation.Play("IDLE");
	avatar.animation["IDLE"].speed = 1;	
	
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

*/