using UnityEngine;
using System.Collections;

public class Character : StateMachine {
    
    public static float cameraDistance = 7;
    
    public AudioClip jumpSound;
    public AudioClip fallSound;
    public AudioClip landSound;
    public AudioClip stepSound;
    public Renderer[] skins;
    
    AudioClip shotSound;
    AudioClip missileSound;
    AudioClip flameSound;
    AudioClip flameLoopSound;
    AudioClip grappleClip;
    AudioClip grappleLoopClip;
    AudioClip grenadeClip;
    AudioClip swimClip;
    AudioClip hitClip;
    AudioClip chargeLoopClip;
    AudioClip chargeStartClip;
    AudioClip turboClip;
    Material swimsuitMaterial;
	Material gridSuitMaterial;
    Material suitMaterial;
    Material fallenMaterial;
    Material invulnerableMaterial;
    
    int hp;
    public int HP {get {return hp;}}
    public int energyTankCount;

    public Transform Head {get {return head;}}

    public GameObject model;
    public Transform root;
    public Transform leftArm;
    public Transform spine;
    public Transform head;
    public Transform rightArm;
    public Transform rightForearm;
    public Transform rightFoot;
    public GameObject muzzle;
    public ParticleEmitter flame;
    public GameObject flameTip;
    public Grapple grapple;
    public GameObject charge;

    bool canCharge = false;
    public bool CanCharge {
        get { return canCharge; }
        set { canCharge = value; }
    }

    Pivot pivot;
    Button buttonA;
    Button buttonB;
    public new CapsuleCollider collider;
    public new Transform transform;
    public new Animation animation;
    public new Rigidbody rigidbody;
    bool grappling = false;
    bool shooting = false;
    bool burning = false;
    float moveSpeed = 6;
    bool invulnerable = false;
    public int jumpCount = 0;
    int maxJumps = 1;
    bool marionette = true;
    bool dying = false;
    Water water;
    ArrayList fallenFX = null;
    float slope = 0.5f;
    float chargeTime = 0;
    bool charged = false;
    bool pushing = false;
    float recoil = 0;
    float restTime = 0;

    string animNameIdle = "IDLE";
    string animNameRun = "SHOOT_RUN";
    string animNameJump = "JUMP";
    string animNameFall = "FALL";
    string animNameHit = "HIT";
    string animNameSwim = "SWIMMING";    
    string animNameSwimUp = "SWIMMING_UP";
    string animNameShootDown = "SHOOT_DOWN";
    string animNameShootUp = "SHOOT_UP";
    string animNamePush = "PUSH";
    
    public bool Marionette {get {return marionette;}}
    
    public enum Weapon {
        None = 0,
        Projectile,
        Missile,
        FlameThrower,
        Grenade,
        Grapple,
        Max,
    }
    Weapon weapon = Weapon.None;
    public Weapon GetWeapon() {return weapon;}
    
    public enum Suit {
        None = 0,
        Armor,
        Swimsuit,
		GridSuit,
        Max,
    }
    Suit suit = Suit.None;
    public Suit GetSuit() {return suit;}
    
    public enum Boots {
        Normal = 0,
        Gravity,
        Max,
    }
    Boots boots = Boots.Normal;
    public Boots GetBoots() {return boots;}
    
    static bool animEventsAttached = false;
    
    private class CharacterState : State {
        
        public virtual void Shoot () {
        }
        
        public virtual void OnButtonDown (Button button) {
        }
        
        public virtual void OnButtonUp (Button button) {
        }
        
        public virtual void OnButtonCancel (Button button) {
        }
        
        public virtual void OnBurn () {
        }
        
        public virtual void OnCollisionEnter (Collision collision) {
        }
        
        public virtual void OnWaterEnter () {
        }
        
        public virtual void OnWaterExit () {
        }
        
        public virtual void OnFixedUpdate () {
        }
        
        public virtual void OnLateUpdate () {
        }

        public virtual void OnExplosion (GameObject source) {
        }
    }
    
    private class RestState : CharacterState {
        
        Character character;
        AnimationState idleAnim;
        Rigidbody rb;
        Vector3 worldContact;
        Vector3 localContact;
        Collider colliderContact;
        int frames;
        float startTime;
        
        public RestState (Character character) {
            this.character = character;
            idleAnim = character.animation[character.animNameIdle];
            idleAnim.wrapMode = WrapMode.Loop;
        }
        
        public override void OnEnter () {
            colliderContact = null;
            character.jumpCount = 0;
            character.rigidbody.velocity = Vector3.zero;
            idleAnim.speed = 1;
            character.animation.CrossFade(character.animNameIdle);
            startTime = Time.time;
        }
        
        public override void OnExit () {
        	character.restTime = Time.time - startTime;
        }
        
        public override void Shoot () {
            character.StartCoroutine(character.ShootCoroutine());
        }
        
        public override void OnButtonDown (Button button) {
            if (button == character.buttonA) {
                character.Jump();
            }
            else if (button == character.buttonB) {
                character.Shoot();
            }
        }

        public override void OnUpdate () {
            Vector3 velocity = Physics.gravity;
            Vector3 origin = character.transform.position + character.collider.center;
            RaycastHit hit;
            Vector3 snapPoint = origin - Vector3.up * character.collider.height * 0.5f;
            Vector3 snapVelocity = Vector3.zero;
            if (Physics.Raycast(origin, -Vector3.up, out hit, 3, Game.defaultMask) && hit.normal.y > character.slope) {
                snapVelocity = (hit.point - snapPoint) * 30;
                snapVelocity.y = Mathf.Max(snapVelocity.y, Physics.gravity.y) - 0.01f;
                velocity = snapVelocity;
                if (hit.collider.rigidbody != null) {
                    hit.collider.rigidbody.AddForceAtPosition(-hit.normal * 5000 * Time.deltaTime, hit.point);
                }
                if (colliderContact != hit.collider) {
                    colliderContact = hit.collider;
                    worldContact = character.transform.position;
                    localContact = hit.collider.transform.InverseTransformPoint(character.transform.position);
                }
                else {
                    Vector3 prevWorldContact = worldContact;
                    worldContact = hit.collider.transform.TransformPoint(localContact);
                    Vector3 worldDelta = worldContact - prevWorldContact;
                    if (Time.deltaTime > 0) {
                        velocity += worldDelta * 1.0f / Time.deltaTime;
                    }
                }
            }
            else {
                character.Fall();
            }
            character.rigidbody.velocity = velocity;
            if (!character.marionette && 
                Mathf.Abs(character.pivot.transform.localPosition.x) > 0.8f && character.InRest()) {
                character.Run();
            }
        }
        
        public override void OnCollisionEnter (Collision collision) {
            if (collision.collider.gameObject.layer == Game.projectilesLayer) {
                character.Hit(10);
            }
        }
        
        public override void OnBurn () {
            character.Burn();
        }
        
        public override void OnWaterEnter () {
            character.Swim();
        }
    }
    
	GameObject trail = null;
    float turboDuration = -1;
    IEnumerator DoTurbo () {
		bool inTurbo = false;
    	yield return 0;
    	float initialSpeed = moveSpeed;
    	while ((InRun() || InJump())&& turboDuration > 0) {
			if (!inTurbo) {
				inTurbo = true;
				if (trail == null) {
					trail = (GameObject)Instantiate(Resources.Load("TurboTrail", typeof(GameObject)));
					trail.transform.position = collider.bounds.center;
					trail.transform.parent = transform;
				}
			}
    		turboDuration -= Time.deltaTime;
    		moveSpeed += 100 * Time.deltaTime;
    		moveSpeed = Mathf.Min(moveSpeed, 12);
    		yield return 0;
    	}
    	while (moveSpeed > initialSpeed) {
    		moveSpeed -= 10 * Time.deltaTime;
    		yield return 0;
    	}
    	turboDuration = -1;
    	moveSpeed = initialSpeed;
    }
    
    public void TurboJump (Vector3 jumpDirection) {
		rigidbody.velocity = Vector3.zero;
       	Turbo();
       	Jump();
       	JumpState jumpState = state as JumpState;
       	if (jumpState != null) {
       		jumpState.acceleration = jumpDirection * 30;
       	}
    }
    
    public void Turbo () {
    	turboDuration = 2;
        if (turboClip == null) {
            turboClip = (AudioClip)Resources.Load("TurboBoost",typeof(AudioClip));
        }
        Game.fx.PlaySound(turboClip);
    	StartCoroutine(DoTurbo());
    }
    
    private class RunState : CharacterState {
        
        Character character;
        float speed;
        AnimationState runAnim;
        Vector3 oldPos;
        float stuckTime;
        float easeInTimer;
        
        public RunState (Character character) {
            this.character = character;
            runAnim = character.animation[character.animNameRun];
            runAnim.wrapMode = WrapMode.Loop;
            character.animation[character.animNamePush].wrapMode = WrapMode.Loop;
        }
        
        public override void OnEnter () {
            character.pushing = false;
            oldPos = Vector3.zero;
            stuckTime = 0;
            character.jumpCount = 0;
            character.animation.CrossFade(character.animNameRun);
            if (character.restTime > 0.5f) {
            	easeInTimer = 0.6f;
            }
            else {
            	easeInTimer = 1.0f;
            }
        }
        
        public override void OnUpdate () {
            if (character.marionette) {
                speed = -character.transform.right.x;
            }
            else {
            	if (easeInTimer < 1.0f) {
            		easeInTimer += Time.deltaTime;
            		if (easeInTimer > 1.0f) {
            			easeInTimer = 1.0f;
            		}
            	}
                speed = character.pivot.transform.localPosition.x * easeInTimer;
            }
            runAnim.speed = 1.2f * Mathf.Abs(speed);
            if (character.pushing) {
                runAnim.speed *= 0.2f;
                speed = Mathf.Clamp(speed, -1, 1);
            }
			float prevLookX = Mathf.Sign(character.transform.right.x);
            if (speed > 0.1f) {
                character.transform.rotation = Quaternion.AngleAxis(180,Vector3.up);
            }
            else if (speed < -0.1f) {
                character.transform.rotation = Quaternion.AngleAxis(0,Vector3.up);
            }
			float newLookX = Mathf.Sign(character.transform.right.x);
			if (prevLookX != newLookX && Mathf.Sign(character.pivot.crosshair.x) != newLookX) {
				character.pivot.crosshair.x *= -1;
			}
            Vector3 origin = character.transform.position + character.collider.center;
            RaycastHit hit;
            Vector3 snapPoint = origin - Vector3.up * character.collider.height * 0.5f;
            Vector3 snapVelocity = Vector3.zero;
            if (Physics.Raycast(origin, -Vector3.up, out hit, 3, Game.defaultMask) && 
                hit.normal.y > character.slope) {
                snapVelocity = (hit.point - snapPoint) * 30;
                snapVelocity.y = Mathf.Max(snapVelocity.y, Physics.gravity.y) - 0.01f;
                Vector3 velocity = new Vector3(-character.moveSpeed * speed,0,0) + snapVelocity;
                if (hit.collider.rigidbody != null) {
                    hit.collider.rigidbody.AddForceAtPosition(-hit.normal * 5000 * Time.deltaTime, 
                                                              hit.point);
                    if (hit.normal.y > character.slope) {
                        velocity += hit.collider.rigidbody.velocity;
                    }
                }
                character.rigidbody.velocity = velocity;
                if (Vector3.Distance(oldPos, character.transform.position)<2 && 
                    Mathf.Abs(character.pivot.transform.localPosition.y) < 0.2f &&
                    hit.normal.y > 0.9999f) {
                    stuckTime += Time.deltaTime;
                }
                else {
                    stuckTime = 0;
                    oldPos = character.transform.position;
                }
                if (stuckTime > 1) {
                    if (!character.pushing) {
                        character.pushing = true;
                        character.CancelCharge();
                        character.animation.CrossFade(character.animNamePush);
                    }
                }
            }
            else if (character.InRun()) {
                character.Fall();
            }
            if (!character.marionette && 
                Mathf.Abs(character.pivot.transform.localPosition.x) < 0.7f && character.InRun()) {
                character.Rest();
            }
        }
        
        public override void Shoot () {
            character.StartCoroutine(character.ShootCoroutine());
        }
        
        public override void OnButtonDown (Button button) {
            if (button == character.buttonA) {
                character.Jump();
            }
            else if (button == character.buttonB) {
                character.Shoot();
            }
        }
        
        public override void OnCollisionEnter (Collision collision) {
            if (collision.collider.gameObject.layer == Game.projectilesLayer) {
                character.Hit(10);
            }
        }
        
        public override void OnBurn () {
            character.Burn();
        }
        
        public override void OnWaterEnter () {
            character.Swim();
        }
    }
    
    private class JumpState : CharacterState {
        
        Character character;
        public Vector3 acceleration;
        float speed;
        AnimationState jumpAnim;
        Vector3 pull;
        
        public JumpState (Character character) {
            this.character = character;
            jumpAnim = character.animation[character.animNameJump];
        }
        
        public override void OnEnter () {
            ++character.jumpCount;
            if (character.boots == Boots.Gravity) {
                acceleration = new Vector3(0, 15, 0);
                pull = new Vector3(0, 20, 0);
            }
            else {
                acceleration = new Vector3(0, 15, 0);
                pull = new Vector3(0, 34, 0);
            }
            Game.fx.PlaySound(character.jumpSound);
            jumpAnim.wrapMode = WrapMode.Once;
            jumpAnim.speed = 2;
            character.animation.CrossFade(character.animNameJump, 0.1f);
        }

        public override void OnExit () {
        }

        public override void OnUpdate () {
            if ((!character.marionette && !character.buttonA.down)) {
               acceleration -= Vector3.up * Time.deltaTime * 30;
            }
            if (character.marionette) {
                speed = 0;
            }
            else {
                speed = character.pivot.transform.localPosition.x;
            }
            acceleration -= Time.deltaTime * pull;
            Vector3 velocity = acceleration;
            velocity += new Vector3(-character.moveSpeed * speed,0,0);
			velocity = velocity.normalized * Mathf.Min(velocity.magnitude, 20);
            character.rigidbody.velocity = velocity;
            if (acceleration.y<0 && character.InJump()) {
                character.Fall();
            }
        }
        
        public override void Shoot () {
            character.StartCoroutine(character.ShootCoroutine());
        }
        
        public override void OnButtonDown (Button button) {
            if (button == character.buttonB) {
                character.Shoot();
            }
        }
        
        IEnumerator DrawRayTimed (Vector3 origin, Vector3 direction, Color color, float duration) {
            do {
                duration -= Time.deltaTime;
                Debug.DrawRay (origin, direction, color);
                yield return 0;
            }
            while (duration > 0);
        }
        
        public override void OnCollisionEnter (Collision collision) {
            if (collision.collider.gameObject.layer == Game.projectilesLayer) {
                character.Hit(10);
            }
        }
        
        public override void OnBurn () {
            character.Burn();
        }
        
        public override void OnWaterEnter () {
            character.Swim();
        }
    }
    
    private class FallState : CharacterState {
        
        Character character;
        Vector3 acceleration;
        float speed;
        AnimationState fallAnim;
        float fallTime;
        float hookDistance = 0;
        Vector3 hookTarget;
        Vector3 oldPos;
        
        public FallState (Character character) {
            this.character = character;
            fallAnim = character.animation[character.animNameFall];
        }
        
        public override void OnEnter () {
            oldPos = character.transform.position + new Vector3(1, 1, 1);
            fallTime = Time.time;
            acceleration = Vector3.zero;
            //Game.fx.PlaySound(character.fallSound);
            fallAnim.wrapMode = WrapMode.Once;
            fallAnim.speed = 2;
            character.animation.CrossFade(character.animNameFall, 0.3f);
            if (character.grapple.Hooked) {
                hookDistance = Vector3.Distance(character.transform.position, character.grapple.hookPoint);
                hookTarget = character.grapple.hookPoint;
                hookTarget.y -= hookDistance;
                //swingStart = character.transform.position;
            }
        }
        
        void CancelFallVelocity () {
            if (!character.rigidbody.isKinematic) {
                Vector3 velocity = character.rigidbody.velocity;
                velocity.y = 0;
                character.rigidbody.velocity = velocity;
            }
        }

        public override void OnUpdate () {
            bool hardSound = false;
            Vector3 velocity = Vector3.zero;
            if (character.grapple.Hooked) {
                fallTime = Time.time;
                acceleration = new Vector3(0, -2, 0);
                Vector3 swingDirection = character.grapple.hookPoint - character.transform.position;
                acceleration += swingDirection;
                acceleration -= Vector3.right * character.pivot.transform.localPosition.x;
                acceleration += Vector3.up * character.pivot.transform.localPosition.y;
                velocity = acceleration * Time.deltaTime * 10;
                character.rigidbody.velocity += velocity;
                if (character.rigidbody.velocity.magnitude > 10) {
                    character.rigidbody.velocity = character.rigidbody.velocity.normalized * 10;
                }
            }
            else {
                if (character.marionette) {
                    speed = 0;
                }
                else {
                    speed = character.pivot.transform.localPosition.x;
                }
                acceleration.y -= Time.deltaTime * 50;
                velocity = acceleration;
                if (velocity.sqrMagnitude > Physics.gravity.sqrMagnitude) {
                    hardSound = true;
                    velocity = Physics.gravity;
                }
                velocity = Vector3.Max(velocity, Physics.gravity);
                velocity += new Vector3(-character.moveSpeed * speed,0,0);
                if (Vector3.Distance(oldPos, character.transform.position) < 0.1f) {
                    velocity.x = Vector3.right.x;
                    fallTime = Time.time;
                }
                if ((Time.time - fallTime) < 0.1f) {
                    velocity.x = Mathf.Lerp(character.rigidbody.velocity.x, velocity.x, Time.deltaTime);
                }
                character.rigidbody.velocity = velocity;
            }
            Vector3 origin = character.transform.position + character.collider.center;
            RaycastHit hit;
            if (character.InFall()) {
                if (Physics.Raycast(origin, -Vector3.up, out hit, 1, Game.defaultMask) &&
                    hit.normal.y > character.slope) {
                    if (hardSound) {
                        Game.fx.PlaySound(character.landSound);
                    }
                    CancelFallVelocity();
                    if (Mathf.Abs(character.pivot.transform.localPosition.x) > 0.3f) {
                        character.Run();
                    }
                    else {
                        character.Rest();
                    }
                }
            }
            oldPos = character.transform.position;
        }
        
        public override void Shoot () {
            character.StartCoroutine(character.ShootCoroutine());
        }
        
        public override void OnButtonDown (Button button) {
            if (button == character.buttonA) {
                if ((Time.time - fallTime) < 0.3f && character.jumpCount < character.maxJumps) {
                    character.Jump();
                }
            }
            else if (button == character.buttonB) {
                character.Shoot();
            }
        }
        
        public override void OnCollisionEnter (Collision collision) {
            if (collision.collider.gameObject.layer == Game.projectilesLayer) {
                character.Hit(10);
            }
            for (int i=0; i<collision.contacts.Length; ++i) {
                if (collision.contacts[i].normal.y > character.slope) {
                    if (character.InFall()) {
                        character.Rest();
                    }
                    break;
                }
            }
        }
        
        public override void OnBurn () {
            character.Burn();
        }
        
        public override void OnWaterEnter () {
            character.Swim();
        }
    }

    IEnumerator Invulnerable () {
        invulnerable = true;
        bool waitForHitEnd = InHit();
        Color color = Color.white;
        float timer = 1;
        while (timer > 0) {
            color.b = Mathf.PingPong(Time.time * 10, 1);
            color.r = color.b;
            color.g = color.b;
            for (int i=0; i<skins.Length; ++i) {
                skins[i].material.color = color;
            }
            if (waitForHitEnd && (InHit() || InFall())) {
            }
            else {
            	timer -= Time.deltaTime;
            }
            yield return 0;
        }
		if (!dying) {
        	SetSuit(suit);
		}
        invulnerable = false;
    }

    private class HitState : CharacterState {
        
        Character character;
        AnimationState hitAnim;
        float timer;
        Vector3 fallVelocity;
        public int damage;
        
        public HitState (Character character) {
            this.character = character;
            hitAnim = character.animation[character.animNameHit];
            damage = 0;
        }
        
        public override void OnEnter () {
            character.AddHP(-damage);
            Game.fx.Flash(0.05f);
            if (character.hitClip == null) {
                character.hitClip = (AudioClip)Resources.Load("Hit", typeof(AudioClip));
            }
            Game.fx.PlaySound(character.hitClip);
            timer = 1.0f;
            hitAnim.wrapMode = WrapMode.Once;
            hitAnim.speed = 1;
            if (character.water == null) {
                fallVelocity = new Vector3(character.transform.right.x * -7, 20, 0);
                character.animation.Play(character.animNameHit);
            }
            else {
                fallVelocity = new Vector3(character.transform.right.x * -1, 5, 0);
            }
            Game.fx.Hit(character.spine.position + new Vector3(0,1,0), 1);
            character.StartCoroutine(character.Invulnerable());
        }
        
        public override void OnUpdate () {
            if (character.water == null) {
                fallVelocity += Physics.gravity * Time.deltaTime * 10;
                fallVelocity.y = Mathf.Max(fallVelocity.y, -10);
            }
            else {
                fallVelocity += Physics.gravity * Time.deltaTime * 2;
                fallVelocity.y = Mathf.Max(fallVelocity.y, -2);
            }
            character.rigidbody.velocity = fallVelocity;
            Vector3 origin = character.transform.position + character.collider.center;
            RaycastHit hit;
            if (fallVelocity.y < 0 && Physics.Raycast(origin, -Vector3.up, out hit, 1, Game.defaultMask)) {
                Game.fx.PlaySound(character.landSound);
                if (character.water != null) {
                    character.Swim();
                }
                else {
                    character.Rest();
                }
                return;
            }
            timer -= Time.deltaTime;
            if (timer < 0) {
                if (character.water != null) {
                    character.Swim();
                }
                else {
                    character.Fall();
                }
            }
        }
        
        public override void OnWaterEnter () {
            character.Swim();
        }
    }
	
	private class LimboState : CharacterState {
	
        Character character;

        public LimboState (Character character) {
			character.Pause();
		}
	}
    
    private class SwimState : CharacterState {

        Character character;
        float speed;
        AnimationState swimAnim;
        Vector3 buoyancy;
        Vector3 velocity;
        float inertia;
        float hitTimer = 0;
        Vector3 center;
        int direction;

        public SwimState (Character character) {
            character.animation[character.animNameSwimUp].speed = 0.7f;
            this.character = character;
            character.animation[character.animNameHit].layer = 1;
            swimAnim = character.animation[character.animNameSwim];
            swimAnim.wrapMode = WrapMode.Loop;
        }
        
        void Damage () {
            if (character.water != null && character.water.lava) {
                character.AddHP(-200);
            }
            else {
                character.AddHP(-35);
            }
            Game.fx.Flash(0.05f);
            if (character.hitClip == null) {
                character.hitClip = (AudioClip)Resources.Load("Hit", typeof(AudioClip));
            }
            Game.fx.PlaySound(character.hitClip);
            //character.animation.Play(character.animNameHit);
            Game.fx.Hit(character.spine.position, 1);
        }
        
        public override void OnEnter () {
            inertia = 0;
            center = character.collider.center;
            direction = character.collider.direction;
            character.collider.center = Vector3.zero;
            character.collider.direction = 0;
            buoyancy = Physics.gravity * 0.1f;
            velocity = Vector3.zero;
            character.animation.CrossFade(character.animNameSwim);
            if (character.water.lava) {
                hitTimer = 0;
            }
            else {
                hitTimer = 2;
            }
        }
        
        public override void OnExit () {
            character.collider.center = center;
            character.collider.direction = direction;
        }
        
        public override void OnUpdate () {
            if (character.marionette) {
                speed = -character.transform.right.x;
            }
            else {
                speed = character.pivot.transform.localPosition.x;
            }
            if (Mathfx.Approx(speed, 0, 0.01f)) {
                swimAnim.speed = 0.8f;
            }
            else {
                swimAnim.speed = 0.9f * speed;
            }
            if (speed > 0.1f) {
                character.transform.rotation = Quaternion.AngleAxis(180,Vector3.up);
            }
            else if (speed < -0.1f) {
                character.transform.rotation = Quaternion.AngleAxis(0,Vector3.up);
            }
            velocity.x = -character.moveSpeed * 0.4f * speed;
            velocity += buoyancy * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, -2, 2);
            if (character.suit != Suit.Swimsuit || character.water.lava) {
                hitTimer -= Time.deltaTime;
                if (hitTimer < 0) {
                    hitTimer = 2;
                    Damage();
                    velocity = -velocity;
                }
            }
            if (speed == 0) {
                inertia -= inertia * Time.deltaTime;
                velocity.x += inertia;
            }
            else {
                inertia = velocity.x;
            }
            character.rigidbody.velocity = velocity;
            buoyancy += Physics.gravity * Time.deltaTime;
            if (buoyancy.magnitude > 2) {
                buoyancy = buoyancy.normalized * 2;
            }
        }
        
        public override void Shoot () {
            character.StartCoroutine(character.ShootCoroutine());
        }
        
        public override void OnButtonDown (Button button) {
            if (button == character.buttonA) {
                if (character.swimClip == null) {
                    character.swimClip = (AudioClip)Resources.Load("Swim",typeof(AudioClip));
                }
                Game.fx.PlaySound(character.swimClip);
                character.animation.Blend(character.animNameSwimUp);
                if (velocity.y < 1) {
                    buoyancy = Vector3.up * 80;
                }
                else {
                    buoyancy = Vector3.up * 40;
                }
            }
            else if (button == character.buttonB) {
                character.Shoot();
            }
        }
        
        public override void OnCollisionEnter (Collision collision) {
        }
        
        public override void OnWaterExit () {
            if (velocity.y > 0) {
                character.Jump();
            }
            else {
                character.Rest();
            }
        }
    }

    public bool InRest () {
        return state is RestState;
    }
    
    public void Rest () {
        RestState restState = (RestState)states[typeof(RestState)];
        if (restState == null) {
            restState = new RestState(this);
            states[typeof(RestState)] = restState;
        }
        Change(restState);
    }

    public bool InRun () {
        return state is RunState;
    }
    
    public void Run () {    
        RunState runState = (RunState)states[typeof(RunState)];
        if (runState == null) {
            runState = new RunState(this);
            states[typeof(RunState)] = runState;
        }
        Change(runState);
    }

    public bool InJump () {
        return state is JumpState;
    }
    
    public void Jump () {
        JumpState jumpState = (JumpState)states[typeof(JumpState)];
        if (jumpState == null) {
            jumpState = new JumpState(this);
            states[typeof(JumpState)] = jumpState;
        }
        Change(jumpState);
    }

    public bool InFall () {
        return state is FallState;
    }
    
    public void Fall () {
        FallState fallState = (FallState)states[typeof(FallState)];
        if (fallState == null) {
            fallState = new FallState(this);
            states[typeof(FallState)] = fallState;
        }
        Change(fallState);
    }
    
    public bool InSwim () {
        return state is SwimState;
    }
    
    public void Swim () {
        SwimState swimState = (SwimState)states[typeof(SwimState)];
        if (swimState == null) {
            swimState = new SwimState(this);
            states[typeof(SwimState)] = swimState;
        }
        Change(swimState);
    }
	
    public bool InLimbo () {
        return state is LimboState;
    }
    
    public void Limbo () {
        LimboState limboState = (LimboState)states[typeof(LimboState)];
        if (limboState == null) {
            limboState = new LimboState(this);
            states[typeof(LimboState)] = limboState;
        }
        Change(limboState);
    }
    
    public bool InHit () {
        return state is HitState;
    }
    
    public void Hit (int damage) {
        if (invulnerable) {
            return;
        }
        HitState hitState = (HitState)states[typeof(HitState)];
        if (hitState == null) {
            hitState = new HitState(this);
            states[typeof(HitState)] = hitState;
        }
        if (suit == Suit.None) {
            damage *= 3;
        }
        else if (suit == Suit.Swimsuit) {
            damage *= 2;
        }
        if (Game.casual) {
        	damage /= 2;
        }
        if (hp < damage && explosionDamage && (Game.casual || suit == Suit.Armor)) {
        	damage = hp;
        }
        hitState.damage = damage;
        Change(hitState);
    }
    
    public void HitSupressReact (int damage) {
        if (invulnerable) {
            return;
        }
        if (suit == Suit.None) {
            damage *= 3;
        }
        else if (suit == Suit.Swimsuit) {
            damage *= 2;
        }
        if (Game.casual) {
        	damage /= 2;
        }
        if (hp < damage && explosionDamage && (Game.casual || suit == Suit.Armor)) {
        	damage = hp;
        }
        AddHP(-damage);
        StartCoroutine(Invulnerable());
    }
    
    public void Shoot () {
        CharacterState characterState = state as CharacterState;
        if (!shooting && characterState != null) {
            characterState.Shoot();
        }
    }
    
    void FlameThrowerOn () {
        flame.emit = true;
        if (flameSound == null) {
            flameSound = (AudioClip)Resources.Load("FlameStart",typeof(AudioClip));
            flameLoopSound = (AudioClip)Resources.Load("FlameThrower",typeof(AudioClip));
        }
        Game.fx.PlaySound(flameSound);
        Game.fx.LoopSound(flameLoopSound);
    }
    
    void FlameThrowerOff () {
        flame.emit = false;
        Game.fx.StopSound(flameLoopSound);
    }
    
    void GrappleOn () {
        grappling = true;
        grapple.gameObject.active = true;
        if (grappleClip == null) {
            grappleClip = (AudioClip)Resources.Load("GrappleStart",typeof(AudioClip));
            grappleLoopClip = (AudioClip)Resources.Load("GrappleLoop",typeof(AudioClip));
        }
        Game.fx.PlaySound(grappleClip);
        Game.fx.LoopSound(grappleLoopClip);
        grapple.Begin();
    }
    
    public void GrappleOff () {
    	if (grapple.Hooked) {
			jumpCount = 0;
    	}
        grappling = false;
        Game.fx.StopSound(grappleLoopClip);
        grapple.End();
        grapple.gameObject.active = false;
    }
    
    IEnumerator ShootCoroutine () {
        shooting = true;
        recoil = 0.3f;
        if (pivot.transform.localPosition.y < -0.8f) {
            animation[animNameShootDown].speed = 6;
            animation.Blend(animNameShootDown);
        }
        else if (pivot.transform.localPosition.y > 0.8f) {
            animation[animNameShootUp].speed = 6;
            animation.Blend(animNameShootUp);
        }
        if (weapon == Weapon.Projectile) {
            Vector3 velocity = rigidbody.velocity * 0.2f;
            velocity += -rightArm.right * 15;
            Vector3 position = rightArm.position - rightArm.right * 0.5f;
            Projectile projectile = Projectile.Launch(position, velocity, 0.7f);
            if (projectile != null) {
                if (charge.active) {
                    GameObject chargeObject = (GameObject)Instantiate(charge);
                    chargeObject.transform.parent = projectile.transform;
                    chargeObject.transform.localPosition = Vector3.zero;
                    chargeObject.transform.rotation = Quaternion.Euler(90, 0, 0);
                    Destroy(chargeObject, 0.7f);
                    int damage = (int)Mathf.Clamp((Time.time - chargeTime - 1) * 100, 20, 200);
                    int mass = (int)Mathf.Clamp((Time.time - chargeTime - 1) * 30, 5, 30);
                    projectile.rigidbody.mass = mass;
                    projectile.Damage = damage;
                    Consume(mass);
                }
                else if (canCharge) {
                    chargeTime = Time.time;
                }
                muzzle.active = true;
                if (shotSound == null) {
                    shotSound = (AudioClip)Resources.Load("Shot", typeof(AudioClip));
                }
                Game.fx.PlaySound(shotSound);
                Physics.IgnoreCollision(projectile.collider, collider);
				if (Game.grid != null) {
            		Consume(1);
				}
            }
            yield return new WaitForSeconds(0.03f);
            muzzle.active = false;
            yield return new WaitForSeconds(0.08f);
        }
        else if (weapon == Weapon.Missile) {
            Missile missile = 
                Missile.Launch(rightArm.position - rightArm.transform.right, -rightArm.right * 8, 2.0f);
            if (missile != null) {
                muzzle.active = true;
                if (missileSound == null) {
                    missileSound = (AudioClip)Resources.Load("Missile", typeof(AudioClip));
                }
                Game.fx.PlaySound(missileSound);
                Physics.IgnoreCollision(missile.collider, collider);
                Consume(20);
            }
            yield return new WaitForSeconds(0.03f);
            muzzle.active = false;
            yield return new WaitForSeconds(1);
        }
        else if (weapon == Character.Weapon.FlameThrower) {
            if (water == null) {
                FlameThrowerOn();
				if (Game.grid != null) {
            		Consume(1);
				}
            }
        }
        else if (weapon == Weapon.Grenade) {
            Vector3 velocity = rigidbody.velocity * 0.2f;
            velocity += -rightArm.right * 8;
            Vector3 position = rightArm.position - rightArm.right * 0.5f;
            Grenade grenade = Grenade.Launch(position, velocity, 2.0f);
            if (grenade != null) {
                muzzle.active = true;
                if (grenadeClip == null) {
                    grenadeClip = (AudioClip)Resources.Load("Grenade", typeof(AudioClip));
                }
                Game.fx.PlaySound(grenadeClip);
                Physics.IgnoreCollision(grenade.collider, collider);
                Consume(10);
            }
            yield return new WaitForSeconds(0.03f);
            muzzle.active = false;
            yield return new WaitForSeconds(1);
        }
        else if (weapon == Character.Weapon.Grapple) {
            GrappleOn();
			if (Game.grid != null) {
        		Consume(1);
			}
        }
        shooting = false;
    }
    
    void OnButtonDown (Button button) {
        CharacterState characterState = state as CharacterState;
        if (characterState != null) {
            characterState.OnButtonDown(button);
        }
    }

    void CancelCharge () {
        chargeTime = 0;
        charged = false;
        charge.active = false;
        charge.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        if (chargeStartClip != null) {
            Game.fx.StopSound(chargeLoopClip);
        }
    }
    
    void OnButtonUp (Button button) {
        if (button == buttonB) {
            if (charge.active) {
                StartCoroutine(ShootCoroutine());
            }
            CancelCharge();
            if (weapon == Weapon.FlameThrower) {
                FlameThrowerOff();
            }
            else if (weapon == Weapon.Grapple) {
                GrappleOff();
            }
        }
        CharacterState characterState = state as CharacterState;
        if (characterState != null) {
            characterState.OnButtonUp(button);
        }
    }
    
    void OnButtonCancel (Button button) {
    	OnButtonUp(button);
    }
    
    void OnCollisionEnter (Collision collision) {
        CharacterState characterState = state as CharacterState;
        if (characterState != null) {
            characterState.OnCollisionEnter(collision);
        }
    }
    
    void OnBurn () {
        CharacterState characterState = state as CharacterState;
        if (characterState != null) {
            characterState.OnBurn();
        }
    }
    
    void OnWaterEnter (Water water) {
        this.water = water;
        CharacterState characterState = state as CharacterState;
        if (characterState != null) {
            characterState.OnWaterEnter();
        }
    }
    
    void OnWaterExit () {
        this.water = null;
        CharacterState characterState = state as CharacterState;
        if (characterState != null) {
            characterState.OnWaterExit();
        }
    }

	bool explosionDamage = false;
    void OnExplosion (GameObject source) {
    	explosionDamage = true;
        float blastRadius = 3;
        RaycastHit hit;
        Vector3 rayDir = source.transform.position - collider.bounds.center;
        rayDir.Normalize();
        if (Physics.Raycast(collider.bounds.center, rayDir, out hit, blastRadius, Game.defaultMask) &&
        hit.collider.gameObject == source) {
	        float dist = Vector3.Distance(source.transform.position, transform.position);
	        if (dist < blastRadius) {
		        CharacterState characterState = state as CharacterState;
		        if (characterState != null) {
		            characterState.OnExplosion(source);
		        }
	            int damage = (int)(100 * (1 - dist/blastRadius));
	            Hit(damage);
	        }
        }
        explosionDamage = false;
    }
    
    void OnStepEvent () {
        Game.fx.PlaySoundPitched(stepSound);
    }
    
    public override void Change (State state) {
        base.Change(state);
    }
    
    public void Pause () {
        rigidbody.isKinematic = true;
        enabled = false;
        animation.enabled = false;
        Game.hud.EnableControls(false);
    }
    
    public void Continue () {
        rigidbody.isKinematic = false;
        enabled = true;
        animation.enabled = true;
        Game.hud.EnableControls(true);
    }
    
    public void AddEnergyTank () {
        ++energyTankCount;
        AddHP(3000);
    }
    
    public void RemoveEnergyTank () {
        --energyTankCount;
        AddHP(3000);
    }
    
    public void UpdateEnergyHud () {
        Game.hud.SetHP(hp, energyTankCount);
        PlayerPrefs.SetInt("EnergyTankCount", energyTankCount);
        PlayerPrefs.SetInt("HP", hp);
    }

    public int MaxHP () {
        return 100 * energyTankCount + 99;
    }
    
    public bool IsHPFull() {
        if (hp == (100 * energyTankCount + 99)) {
            return true;
        }
        return false;
    }

    public void Consume (float consumption) {
		if (Game.grid == null) {
	        if (consumption > this.hp) {
	            consumption = this.hp;
	        }
	        AddHP(-(int)consumption);
		}
		else {
			Game.grid.Consume(weapon);
		}
    }
    
    public void AddHP (int hp) {
        if (!dying) {
            this.hp += hp;
            if (this.hp > (100 * energyTankCount + 99)) {
                this.hp = (100 * energyTankCount + 99);
            }
            if (this.hp < 0) {
                this.hp = 0;
                dying = true;
                Game.game.GameOver(true);
            }
        }
        UpdateEnergyHud();
    }

    void Awake () {
    }
    
    void Start () {
        Game.character = this;
        grapple.gameObject.active = false;
        if (!animEventsAttached) {
            animEventsAttached = true;
            AnimationEvent stepEvent;
            stepEvent = new AnimationEvent();
            stepEvent.functionName = "OnStepEvent";
            stepEvent.messageOptions = SendMessageOptions.DontRequireReceiver;
            stepEvent.time = 0.0f;
            animation[animNameRun].clip.AddEvent(stepEvent);
            stepEvent = new AnimationEvent();
            stepEvent.functionName = "OnStepEvent";
            stepEvent.messageOptions = SendMessageOptions.DontRequireReceiver;
            stepEvent.time = 0.3f;
            animation[animNameRun].clip.AddEvent(stepEvent);
        }
        energyTankCount = PlayerPrefs.GetInt("EnergyTankCount", 0);
        hp = PlayerPrefs.GetInt("HP", 0);
        if (PlayerPrefs.HasKey("Charge")) {
            canCharge = true;
        }
        pivot = Game.hud.pivot;
        buttonA = Game.hud.buttonA;
        buttonB = Game.hud.buttonB;
        buttonA.receiver = gameObject;
        buttonB.receiver = gameObject;
        muzzle.active = false;
        flameTip.active = false;
        flame.emit = false;
        grapple.gameObject.active = false;
        charge.active = false;
        Weapon lastWeapon = (Weapon)PlayerPrefs.GetInt("Weapon", 0);
        SetWeapon(lastWeapon);
		if (Game.grid == null) {
	        Suit lastSuit = (Suit)PlayerPrefs.GetInt("Suit", 0);
	        SetSuit(lastSuit);
		}
		else {
			Disintegrate disingtegrate = gameObject.AddComponent<Disintegrate>();
			disingtegrate.duration = 3;
			disingtegrate.deathDelay = 2;
			hp = 99;
			energyTankCount = 0;
			SetSuit(Suit.GridSuit);
		}

        Boots lastBoots = (Boots)PlayerPrefs.GetInt("Boots", 0);
        SetBoots(lastBoots);
        UpdateEnergyHud();
        PlayerPrefs.SetInt("Spirit", 1);
        if (PlayerPrefs.HasKey("Spirit")) {
            Instantiate(Resources.Load("Spirit"));
        }

		if (weapon == Weapon.None && (PlayerPrefs.HasKey("Projectile") || (Game.grid != null && Game.grid.projectileCount > 0))) {
			SetWeapon(Weapon.Projectile);
			Game.hud.UpdateWeapons();
		}
    }
    
    public void SetSuit (Suit suit) {
        if (fallenFX != null) {
            foreach (GameObject fxObject in fallenFX) {
                Destroy(fxObject);
            }
        }
        if (suit == Suit.None) {
            fallenFX = new ArrayList();
            this.suit = suit;
            PlayerPrefs.SetInt("Suit", (int)suit);
            if (fallenMaterial == null) {
                fallenMaterial = (Material)Resources.Load("AvatarBrokenSuit", typeof(Material));
            }
            for (int i=0; i<skins.Length; ++i) {
                skins[i].material = fallenMaterial;
            }
            GameObject body = (GameObject)Instantiate(Resources.Load("AvatarBrokenBody", typeof(GameObject)));
            Component[] bodyParts = (Component[])body.GetComponentsInChildren(typeof(MeshFilter));
            foreach (Renderer skin in skins) {
            	MeshFilter meshFilter = (MeshFilter)skin.gameObject.GetComponent(typeof(MeshFilter));
            	foreach (MeshFilter bodyPart in bodyParts) {
            		if (meshFilter.name == bodyPart.name) {
            			meshFilter.mesh = bodyPart.mesh;
            		}
            	}
            }
            Destroy(body);
            GameObject fxObject = null;
            fxObject = (GameObject)Instantiate(Resources.Load("FallenShort"));
            fallenFX.Add(fxObject);
            fxObject.transform.parent = spine;
            fxObject.transform.localPosition = Vector3.zero;
            fxObject = (GameObject)Instantiate(Resources.Load("FallenShort"));
            fallenFX.Add(fxObject);
            fxObject.transform.parent = rightFoot;
            fxObject.transform.localPosition = Vector3.zero;
        }
        else if (suit == Suit.Armor) {
            this.suit = suit;
            PlayerPrefs.SetInt("Suit", (int)suit);
            if (suitMaterial == null) {
                suitMaterial = (Material)Resources.Load("AvatarSuit", typeof(Material));
            }
            for (int i=0; i<skins.Length; ++i) {
                skins[i].material = suitMaterial;
            }
            GameObject body = (GameObject)Instantiate(Resources.Load("AvatarRegularBody", typeof(GameObject)));
            Component[] bodyParts = (Component[])body.GetComponentsInChildren(typeof(MeshFilter));
            foreach (Renderer skin in skins) {
            	MeshFilter meshFilter = (MeshFilter)skin.gameObject.GetComponent(typeof(MeshFilter));
            	foreach (MeshFilter bodyPart in bodyParts) {
            		if (meshFilter.name == bodyPart.name) {
            			meshFilter.mesh = bodyPart.mesh;
            		}
            	}
            }
            Destroy(body);
        }
        else if (suit == Suit.Swimsuit) {
            this.suit = suit;
            PlayerPrefs.SetInt("Suit", (int)suit);
            if (swimsuitMaterial == null) {
                swimsuitMaterial = (Material)Resources.Load("AvatarSwimsuit", typeof(Material));
            }
            for (int i=0; i<skins.Length; ++i) {
                skins[i].material = swimsuitMaterial;
            }
            GameObject body = (GameObject)Instantiate(Resources.Load("AvatarRegularBody", typeof(GameObject)));
            Component[] bodyParts = (Component[])body.GetComponentsInChildren(typeof(MeshFilter));
            foreach (Renderer skin in skins) {
            	MeshFilter meshFilter = (MeshFilter)skin.gameObject.GetComponent(typeof(MeshFilter));
            	foreach (MeshFilter bodyPart in bodyParts) {
            		if (meshFilter.name == bodyPart.name) {
            			meshFilter.mesh = bodyPart.mesh;
            		}
            	}
            }
            Destroy(body);
        }
        else if (suit == Suit.GridSuit) {
            this.suit = suit;
            PlayerPrefs.SetInt("Suit", (int)suit);
            if (gridSuitMaterial == null) {
                gridSuitMaterial = (Material)Resources.Load("AvatarGridSuit", typeof(Material));
            }
            for (int i=0; i<skins.Length; ++i) {
                skins[i].material = gridSuitMaterial;
            }
            GameObject body = (GameObject)Instantiate(Resources.Load("AvatarRegularBody", typeof(GameObject)));
            Component[] bodyParts = (Component[])body.GetComponentsInChildren(typeof(MeshFilter));
            foreach (Renderer skin in skins) {
            	MeshFilter meshFilter = (MeshFilter)skin.gameObject.GetComponent(typeof(MeshFilter));
            	foreach (MeshFilter bodyPart in bodyParts) {
            		if (meshFilter.name == bodyPart.name) {
            			meshFilter.mesh = bodyPart.mesh;
            		}
            	}
            }
            Destroy(body);
        }
    }
    
    public void SetBoots (Boots boots) {
        if (boots == Boots.Normal) {
            this.boots = boots;
            PlayerPrefs.SetInt("Boots", (int)boots);
        }
        else if (boots == Boots.Gravity) {
            this.boots = boots;
            PlayerPrefs.SetInt("Boots", (int)boots);
        }
    }
    
    public void SetWeapon (Weapon weapon) {
        if (weapon == Weapon.None) {
            flameTip.active = false;
            this.weapon = Weapon.None;
            PlayerPrefs.SetInt("Weapon", (int)weapon);
        }
        else if (weapon == Weapon.Projectile) {
            flameTip.active = false;
            this.weapon = Weapon.Projectile;
            PlayerPrefs.SetInt("Weapon", (int)weapon);
        }
        else if (weapon == Weapon.Missile) {
            flameTip.active = false;
            this.weapon = Weapon.Missile;
            PlayerPrefs.SetInt("Weapon", (int)weapon);
        }
        else if (weapon == Weapon.FlameThrower) {
            flameTip.active = true;
            this.weapon = Weapon.FlameThrower;
            PlayerPrefs.SetInt("Weapon", (int)weapon);
        }
        else if (weapon == Weapon.Grenade) {
            this.weapon = Weapon.Grenade;
            PlayerPrefs.SetInt("Weapon", (int)weapon);
        }
        else if (weapon == Weapon.Grapple) {
            this.weapon = Weapon.Grapple;
            PlayerPrefs.SetInt("Weapon", (int)weapon);
        }
    }
    
    public void SetMarionette (bool enabled) {
        if (enabled) {
            marionette = true;
            Game.hud.EnableControls(false);
        }
        else {
            Game.hud.EnableControls(true);
            marionette = false;
        }
    }
    
    void Burn () {
        if (!burning) {
            StartCoroutine(BurnCoroutine());
        }
    }
    
    IEnumerator BurnCoroutine() {
        burning = true;
        //Color color = new Color(1, 0.5f, 0);
        GameObject flamesObject = (GameObject)Instantiate(Resources.Load("Flames",typeof(GameObject)));
        flamesObject.transform.position = collider.bounds.center;
        ParticleEmitter flames = (ParticleEmitter)flamesObject.GetComponent(typeof(ParticleEmitter));
        flamesObject.transform.parent = transform;
        Game.fx.PlaySound(flameLoopSound);
        float timer = 3;
        float damage = 0;
        do {
            timer -= Time.deltaTime;
            damage += 10 * Time.deltaTime;
            if (damage > 1) {
                damage -= 1;
                //AddHP(-(int)damage);
            }
            yield return 0;
        } while (timer > 0);
        flames.emit = false;
        SetSuit(suit);
        burning = false;
    }
    
   	public void Hide () {
   		foreach (Renderer skin in skins) {
   			skin.enabled = false;
   		}
   	}
    
   	public void Show () {
   		foreach (Renderer skin in skins) {
   			skin.enabled = true;
   		}
   	}

    void FixedUpdate () {
		
        state.OnUpdate();
    }
    
    void Update () {
        if (Time.timeScale > 0) {
            if (chargeTime > 0) {
                if ((Time.time-chargeTime) > 1 && !charge.active) {
                    charge.active = true;
                    if (chargeStartClip == null) {
                        chargeStartClip = (AudioClip)Resources.Load("ChargeStart",typeof(AudioClip));
                    }
                    Game.fx.PlaySound(chargeStartClip);
                }
                else if ((Time.time - chargeTime) > 2 && !charged) {
                    charged = true;
                    if (chargeLoopClip == null) {
                        chargeLoopClip = (AudioClip)Resources.Load("ChargeLoop",typeof(AudioClip));
                    }
                    Game.fx.LoopSound(chargeLoopClip);
                }
                if (charge.active) {
                    float chargeSize = Mathf.Clamp((Time.time - chargeTime) / 4, 0.3f, 0.5f);
                    charge.transform.localScale = Vector3.one * chargeSize;
                }
            }
            Vector3 directorPos = transform.position;
            if (!marionette && !pushing) {
                Vector3 direction = pivot.transform.localPosition;
                direction.x *= -1;
                directorPos += direction * 3;
            }
            directorPos.y += 1;
            directorPos.z = cameraDistance + Director.zoom;
            float distance = Vector3.Distance(Game.director.transform.position,transform.position);
            if (Game.director.transform.forward.z > 0) {
                Game.director.transform.position = directorPos;
            }
            else {
                Game.director.transform.position = 
                    Vector3.Lerp(Game.director.transform.position,directorPos,Time.deltaTime * 
                    Mathf.Min(distance, 3));
            }
            Game.director.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (flame.emit) {
			if (Game.grid == null) {
            	Consume(100 * Time.deltaTime);
			}
            flame.worldVelocity = -rightForearm.right * Random.Range(2.0f, 3.0f) + rigidbody.velocity;
        }
    }
            
    void LateUpdate () {
        if (pushing && !grappling && !shooting) {
        }
        else if (grapple.Hooked) {
			if (Game.grid == null) {
            	Consume(100 * Time.deltaTime);
			}
            Vector3 localHook = (grapple.hookPoint - rightArm.position).normalized;
            float angle = Mathf.Atan2(-localHook.x, localHook.y) * Mathf.Rad2Deg;
            rightArm.rotation = Quaternion.AngleAxis(angle-90, Vector3.forward);
            rightForearm.localRotation = Quaternion.identity;
        }
        else {
			Vector3 dir = pivot.crosshair.normalized;
			dir.Normalize();
			float angle = Vector3.Angle(dir, Vector3.up);
			if (dir.x < 0) {
				rightArm.rotation = Quaternion.AngleAxis(angle - 90, Vector3.Cross(Vector3.up, dir));
			}
			else {
				rightArm.rotation = Quaternion.AngleAxis(angle + 90, Vector3.Cross(Vector3.up, dir));
			}
			// hack to make arm not go through armor (visual tweak)
			if (transform.right.x < 0) {
				//rightArm.position += Vector3.forward * 0.05f;
			}
            rightForearm.localRotation = Quaternion.identity;
        }
        if (recoil > 0) {
            if (state is RestState) {
                spine.Rotate(new Vector3(0, 0, -10 * (0.4f - recoil / 0.3f)));
                leftArm.Rotate(new Vector3(0, 0, -30 * (0.4f - recoil / 0.3f)));
            }
            recoil -= Time.deltaTime;
        }
    }
}
