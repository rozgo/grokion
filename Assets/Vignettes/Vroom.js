
//These two are required for the sake of checking which armor is currently active.
public var textLine01= "";
public var textLine02= "";
public var letterSound : AudioClip;
public var finalText : TextMesh;
public var finalText02 : TextMesh;


public var startColor : Color;
public var endColor : Color;
public var sceneMaterial : Material;
private var blendColor : Color; 
public var colorBlending = 0.5;
public var characterTurboSpeed = 6;
public var boostActive = false;
var boostTime = 10000;
public var backgroundMat : Material;

public var backStartColor : Color;
public var backEndColor : Color;
private var backBlendColor : Color;

//DecreaseOver time
public var timeLimit = 3600;
public var curCountDown = timeLimit;

public var floorParticles: ParticleEmitter;


function Start() {
	yield;
	var vignette : Vignette = GetComponent(Vignette);

	sceneMaterial.color = startColor;
	blendColor = Color.Lerp (startColor, endColor, colorBlending);
	
	
	var floorParticlesObject:GameObject = Instantiate(Resources.Load("FloorParticles"));
	floorParticles = floorParticlesObject.GetComponent(ParticleEmitter);
}


function Update() {

	if (Game.character == null) {
		return;
	}
	
	if (!Game.character.InRun()){
		floorParticles.emit = false;
	}
	else {
		if (!floorParticles.emit) {
			floorParticles.emit = true;
		}
	}
	
	if (Game.character.InJump() == true || Game.character.InFall() == true){
		if (colorBlending < 1.0) {
			colorBlending += Time.deltaTime;
		}
	}
	else {
		if (colorBlending > 0) {
			colorBlending -= Time.deltaTime * 2;
		}
	}
		
	blendColor = Color.Lerp (startColor, endColor, colorBlending);
	sceneMaterial.color = blendColor;
	
	floorParticles.transform.position = Game.character.transform.position;
	
	//BackgroudnAnim
	

    var offset = Time.time * 0.02;
    backgroundMat.mainTextureOffset = Vector2 (offset,offset);
    
     var scaleX = 6;
    var scaleY = 6;
    backgroundMat.mainTextureScale = Vector2 (scaleX,scaleY);
    
    backBlendColor = Color.Lerp (backStartColor, backEndColor, colorBlending);
		backgroundMat.color = backBlendColor;
    
}
		
		
	// Animate background.
		
	
	
	// Fetches Vignette specific functions. Like move to and look at.

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