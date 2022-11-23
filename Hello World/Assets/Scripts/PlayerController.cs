using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Variables")]
    public float turnSpeed = 180;
    public float tiltSpeed = 180;
    public float walkSpeed = 5;
    public float sprintSpeed = 10;

    [Header("Gravity")]
    public float gravity = 9.81f;

    // jumpSpeed = upwards force / jumpBoostMultiplier = movement direction force
    [Header("Jump Attributes")]
    public float jumpSpeed = 5;
    public float jumpBoostMultiplier = 1.2f;

    // Player movement direction holder
    Vector3 moveDirection = Vector3.zero;

    [Header("Player Attributes")]
    public int maxHealth = 10;
    public int weapon1Damage = 2;
    [Tooltip("Delay before shot / ServerRPC")]
    public float weapon1ShotDelay = .5f;
    public float weapon1ShotMaxCooldown = 1f;
    public float wallPlaceMaxCooldown = 10f;

    private float weapon1ShotCooldown = 1f;
    private float wallPlaceCooldown = 1f;

    private bool wallPreview = false;

    [Header("UI Objects")]
    public WallCooldownUI wallCooldown;

    // Unity Character controller
    CharacterController characterController;

    [Header("Player Identifiers")]
    [Tooltip("Player number for spawnpoint allocation")]
    public int playerNumber;

    [SerializeField]
    TMP_Text playerNameDisplay;

    // Animator
    private bool hasAnimator;
    private bool hasRailgunAnimator;
    private Animator animator;
    private Animator railgunAnimator;

    // Player Animation IDs
    private int animIDMelee;
    private int animIDJump;
    private int animIDHit;
    private int animIDLaser;
    private int animIDWallSummon;
    private int animIDMissilesCast;
    private int animIDRunning;
    private int animIDGrounded;
    private int animIDSpeed;
    private int animIDMotionX;
    private int animIDMotionZ;
    // Railgun Animation IDs
    private int animIDShoot;

    [Header("Camera Objects")]
    [SerializeField]
    private Transform fpcam;
    private Camera topcam;

    [Header("Network Object Prefabs")]
    // Network Object prefabs

    // Rising wall prefabs
    [SerializeField] 
    private Transform risingWall;
    [SerializeField]
    private GameObject wallPreviewObj;

    // Laser line renderer prefabs
    [SerializeField]
    private Transform laserTeam1;
    [SerializeField]
    private Transform laserTeam2;

    // Particle system prefabs
    [SerializeField]
    private Transform blastBlue;
    [SerializeField]
    private Transform blastRed;

    // Ragdoll Prefab
    [SerializeField]
    private Transform ragdollPrefab;

    [Header("Customisable Objects")]
    //Helmets
    public GameObject Helmet1;
    public GameObject Helmet2;
    public GameObject Helmet3;
    public GameObject Helmet4;

    [Header("Player Weapon")]
    //Railgun
    public GameObject Railgun;

    [Header("Player Character")]
    public GameObject playerChar;

    [Header("Network Variables")]
    //Network Variables
    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<int> helmetSelection = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<int> playerHealth = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> playerIsDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> team = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


 

    private void Awake()
    {
       // playerIsDead.OnValueChanged += OnDeathStateChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        

        if (IsOwner)
        {
            //playerName.Value = PlayerPrefs.GetString("PlayerName");

            //Helmet Assignment
            //helmetSelection.Value = PlayerPrefs.GetInt("Helmet");

            playerNumber = (int)OwnerClientId;
            Debug.Log(playerName.Value + " is player number " + playerNumber);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        
        // find character controller
        characterController = GetComponent<CharacterController>();

        // find and set animator IDs
        hasAnimator = TryGetComponent(out animator);
        hasRailgunAnimator = Railgun.TryGetComponent(out railgunAnimator);

        AssignAnimationIDs();

        //SpawnServerRpc();

        // SetNameServerRpc(playerName.Value.ToString());
    }

    private void AssignAnimationIDs()
    {
        //PlayerAnimationIDs
        animIDMelee = Animator.StringToHash("Melee");
        animIDJump = Animator.StringToHash("Jump");
        animIDHit = Animator.StringToHash("Hit");
        animIDLaser = Animator.StringToHash("Laser");
        animIDWallSummon = Animator.StringToHash("WallSummon");
        animIDMissilesCast = Animator.StringToHash("MissilesCast");
        animIDRunning = Animator.StringToHash("Running");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDSpeed = Animator.StringToHash("Speed");
        animIDMotionX = Animator.StringToHash("MotionX");
        animIDMotionZ = Animator.StringToHash("MotionZ");

        //RailGunAnimationIDs
        animIDShoot = Animator.StringToHash("Shoot");
    }




    // Update is called once per frame
    void Update()
    {

        playerNameDisplay.text = playerName.Value.ToString() + ": Team " + team.Value;
        if (helmetSelection.Value == 1) Helmet1.SetActive(true); else Helmet1.SetActive(false);
        if (helmetSelection.Value == 2) Helmet2.SetActive(true); else Helmet2.SetActive(false);
        if (helmetSelection.Value == 3) Helmet3.SetActive(true); else Helmet3.SetActive(false);
        if (helmetSelection.Value == 4) Helmet4.SetActive(true); else Helmet4.SetActive(false);

        if (IsOwner & playerIsDead.Value == false)
        {
            // Wall Functions
            // Enable wall preview mode when player presses E
             if (Input.GetKeyDown(KeyCode.E) && wallPlaceCooldown <= 0)
            {
                wallPreview = true;
            }
             // Create Wall when player releases E
            if (Input.GetKeyUp(KeyCode.E) && wallPlaceCooldown <= 0 && FloorFaceCheck())
            {
                WallSpawnServerRpc(wallPreviewObj.transform.position);
                wallPreview = false;
                wallPreviewObj.SetActive(false);
                wallPlaceCooldown = wallPlaceMaxCooldown;
            }
            // Exit Preview mode if player releases E without aiming at floor
            else if(Input.GetKeyUp(KeyCode.E) && wallPlaceCooldown <= 0)
            {
                wallPreview = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && weapon1ShotCooldown <= 0)
            {
                Shoot();
                weapon1ShotCooldown = weapon1ShotMaxCooldown;
            }

            

            // If Wall Preview Mode is active, Check if looking at floor, if looking at floor raycast again to set hit point and assign position of preview mesh
            if (wallPreview)
            {
                if (FloorFaceCheck())
                {
                    wallPreviewObj.SetActive(true);
                    RaycastHit hit;
                    if (Physics.Raycast(fpcam.position, fpcam.forward, out hit, Mathf.Infinity))
                    {
                        if (hit.collider.CompareTag("Floor"))
                        {
                            wallPreviewObj.transform.position = hit.point;
                        } 
                    }
                }
                // Disable Preview mesh if no longer looking at floor
                else
                {
                    wallPreviewObj.SetActive(false);
                }
            }
            // Disable Preview mesh if out of preview mode
            else
            {
                wallPreviewObj.SetActive(false);
            }
            

            reduceCooldowns();
            wallCooldown.UpdateCooldownUI((wallPlaceMaxCooldown - wallPlaceCooldown) / wallPlaceMaxCooldown);

            //Movement Input
            float forward = Input.GetAxisRaw("Vertical");
            float right = Input.GetAxisRaw("Horizontal");
            float turn =  Input.GetAxis("Mouse X");
            float tilt = Input.GetAxis("Mouse Y");

            // Pass Movement directions for strafe blendtree
            animator.SetFloat(animIDMotionZ, forward, 1f, Time.deltaTime * 10f);
            animator.SetFloat(animIDMotionX, right, 1f, Time.deltaTime * 10f);

            //camera rotation
            transform.Rotate(new Vector3(0, turn * turnSpeed * Time.deltaTime, 0));
            if (fpcam != null) fpcam.Rotate(new Vector3(-tilt * tiltSpeed * Time.deltaTime, 0));
            //camera rotation lock doesnt work
            //fpcam.transform.eulerAngles = new Vector3( Mathf.Clamp(fpcam.transform.rotation.x,0,180), fpcam.transform.eulerAngles.y);


            // Move player when grounded
            if (characterController.isGrounded)
            { 
                moveDirection = ((transform.TransformDirection(Vector3.forward)* forward) + (transform.TransformDirection(Vector3.right)*right)).normalized;

                // Initialise player animator parameters
                animator.SetFloat(animIDSpeed, 0);
                animator.SetBool(animIDGrounded, true);
                animator.SetBool(animIDRunning, false);

                if (Input.GetKey(KeyCode.LeftShift) & forward > 0)
                {
                    moveDirection *= sprintSpeed;

                    // Animate sprint
                    animator.SetBool(animIDRunning, true);
                    animator.SetFloat(animIDSpeed, sprintSpeed);
                }
                else if(new Vector2(right,forward).magnitude > 0)
                {
                    moveDirection *= walkSpeed;

                    // Animate Strafe
                    animator.SetBool(animIDRunning, false);
                    animator.SetFloat(animIDSpeed, walkSpeed);
                }

                if (Input.GetButton("Jump"))
                {
                    moveDirection *= jumpBoostMultiplier;
                    moveDirection.y = jumpSpeed;

                    // Trigger Jump parameter in Animation Controller
                    animator.SetTrigger(animIDJump);
                    animator.SetBool(animIDGrounded, false);
                }
            }
            else
            {

            }
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Apply player movement
        characterController.Move(moveDirection * Time.deltaTime);




        //if (playerIsDead.Value == true)
        //{
        //    //play death animation
        //    animator.SetTrigger(animIDDeath);
        //    Invoke("Spawn", 3);
        //    FindObjectOfType<DeathmatchManager>().PlayerKilledServerRpc(team.Value);
        //}


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    //swap cameras
        //    topcam.enabled = !topcam.enabled;
        //    fpcam.GetComponent<Camera>().enabled = !fpcam.GetComponent<Camera>().enabled;
        //}
        //  https://addam-davis1989.medium.com/jumping-with-physics-based-character-controller-in-unity-45462a04e62


        if (IsOwner) DebugInputs();

    }

   
    public void PlayerDead()
    {
        //Railgun.SetActive(false);
       //playerChar.SetActive(false);
        ragdollSpawnServerRpc(team.Value, helmetSelection.Value);
        
        FindObjectOfType<DeathmatchManager>().PlayerKilledServerRpc(team.Value);
        DestroyServerRpc();
    }
    public void OnDeathStateChanged(bool previous, bool current)
    {
        if (playerIsDead.Value == true)
        {
            Invoke("Spawn", 3);
            FindObjectOfType<DeathmatchManager>().PlayerKilledServerRpc(team.Value);

        }
    }

    private bool FloorFaceCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpcam.position, fpcam.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Floor"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // Debug keys for testing - !REMEMBER TO REMOVE!
    public void DebugInputs()
    {
        // Team assign
        if(Input.GetKeyDown(KeyCode.T))
        {
            if(team.Value != 1)
            {
                team.Value = 1;

            }
            else if(team.Value != 2)
            {
                team.Value = 2;
            }
        }

        // Helmet assign
        if (Input.GetKeyDown(KeyCode.Alpha1)) helmetSelection.Value = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) helmetSelection.Value = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) helmetSelection.Value = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) helmetSelection.Value = 4;

    }

    private void reduceCooldowns()
    {
        if(weapon1ShotCooldown > 0)
        {
            weapon1ShotCooldown -= 1f * Time.deltaTime;
        }
        if(wallPlaceCooldown > 0)
        {
            wallPlaceCooldown -= 1f * Time.deltaTime;
        }
    }
    
    public void Hit()
    {
        // Trigger take damage animation
        animator.SetTrigger(animIDHit);
    }

    public void Shoot()
    {
        // Execute charging animation
        railgunAnimator.SetTrigger(animIDShoot);
        
        Debug.Log("Weapon Charging");
        Invoke("HitScanServerRpc", weapon1ShotDelay);
    }


    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        foreach(PlayerEntity player in FindObjectsOfType<PlayerEntity>())
        {
            if(this.OwnerClientId == player.OwnerClientId)
            {
                player.activePlayer.Value = false;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Destroy(this.gameObject);
    }
    [ServerRpc]
    void SetNameServerRpc(string name)
    {
        // Set Player name
        playerNameDisplay.text = name;
    }

    [ServerRpc]
    void HitScanServerRpc()
    {
        // Instantiate Laser Beam
        if(team.Value == 2)
        {
            var laserObj = Instantiate(laserTeam2);
            laserObj.position = Railgun.transform.position - 0.5f * Railgun.transform.up;
            laserObj.rotation = Railgun.transform.rotation;
            laserObj.Rotate(90, 0, 90);
            laserObj.GetComponent<NetworkObject>().Spawn(true);
        }
        else
        {
            var laserObj = Instantiate(laserTeam1);
            laserObj.position = Railgun.transform.position - 0.5f * Railgun.transform.up;
            laserObj.rotation = Railgun.transform.rotation;
            laserObj.Rotate(90, 0, 90);
            laserObj.GetComponent<NetworkObject>().Spawn(true);
        }
        
        

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(fpcam.position, fpcam.forward, out hit, Mathf.Infinity))
        {
            // Instantiate Blast Particle
            if(team.Value == 2)
            {
                var blastObj = Instantiate(blastRed);
                blastObj.position = hit.point + Railgun.transform.up;
                blastObj.GetComponent<NetworkObject>().Spawn(true);
            }
            else
            {
                var blastObj = Instantiate(blastBlue);
                blastObj.position = hit.point + Railgun.transform.up;
                blastObj.GetComponent<NetworkObject>().Spawn(true);
            }


            if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<PlayerController>().team.Value != team.Value )
            {
                FindObjectOfType<HelloWorldManager>().DamagePlayerClientRpc(hit.collider.gameObject.GetComponent<PlayerController>(), weapon1Damage);
                Debug.DrawRay(fpcam.position, fpcam.forward * hit.distance, Color.yellow);
                Debug.Log(playerName.Value + "Landed a Shot");
            }
            if (hit.collider.CompareTag("WallPrefab"))
            {
                FindObjectOfType<HelloWorldManager>().DamageWallClientRpc(hit.collider.gameObject.GetComponent<RisingWall>(), weapon1Damage);
                Debug.Log(playerName.Value + "Shot a WallPrefab");
            }

        }
        else
        {
            Debug.DrawRay(fpcam.position, fpcam.forward * 1000, Color.white);
            Debug.Log(playerName.Value + "Hit Nothing");
        }
    }

    // Wall Spawning RPC, creates terrain wall object
    [ServerRpc]
    void WallSpawnServerRpc(Vector3 position)
    {
        var obj = Instantiate(risingWall);
        obj.position = position;
        obj.rotation = transform.rotation;
        obj.Rotate(-90, 0, 0);
        obj.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    void ragdollSpawnServerRpc(int team, int helmet)
    {
        Debug.Log("Spawning Ragdoll");
        var ragObj = Instantiate(ragdollPrefab);
        ragObj.position = transform.position;
        ragObj.rotation = transform.rotation;
        ragObj.GetComponent<RagdollBehaviour>().SetHelmet(helmet);
        ragObj.GetComponent<RagdollBehaviour>().SetTeam(team);
        ragObj.GetComponent<NetworkObject>().Spawn(true);
    }

    public override void OnNetworkSpawn()
    {

        
        //Spawn();
       

        if (IsOwner && fpcam != null)
        {
            topcam = Camera.main;
            topcam.enabled = false;
            fpcam.GetComponent<Camera>().enabled = true;

        }
        else
        {
            fpcam.GetComponent<Camera>().enabled = false;

        }
    }

    public override void OnDestroy()
    {
        if (IsOwner && fpcam != null)
        {
            fpcam.GetComponent<Camera>().enabled = false;
            topcam.enabled = true;
        }
    }
}
