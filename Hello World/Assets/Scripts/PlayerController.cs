using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    //Movement Variables
    public float turnSpeed = 180;
    public float tiltSpeed = 180;
    public float walkSpeed = 5;
    public float sprintSpeed = 10;

    // Default gravity value
    public float gravity = 9.81f;

    // jumpSpeed = upwards force / jumpBoostMultiplier = movement direction force
    public float jumpSpeed = 5;
    public float jumpBoostMultiplier = 1.2f;

    // Player movement direction holder
    Vector3 moveDirection = Vector3.zero;

    // Player attribute variables
    public int maxHealth = 10;
    public int weapon1Damage = 2;
    public float weapon1ShotDelay = .5f;

    // Unity Character controller
    CharacterController characterController;

    // Player number for spawnpoint allocation
    public int playerNumber;

    // Animator
    private bool hasAnimator;
    private Animator animator;

    // Railgun Animator
    private bool hasRailgunAnimator;
    private Animator railgunAnimator;

    // Animation IDs
    private int animIDMelee;
    private int animIDRailgunShoot;
    private int animIDJump;
    private int animIDHit;
    private int animIDDeath;
    private int animIDLaser;
    private int animIDWallSummon;
    private int animIDMissilesCast;
    private int animIDRunning;
    private int animIDGrounded;
    private int animIDSpeed;
    private int animIDMotionX;
    private int animIDMotionZ;


    [SerializeField]
    private Transform fpcam;
    private Camera topcam;
    [SerializeField] 
    private Transform prefab;

    [SerializeField]
    TextMesh playerNameDisplay;

    //Network Variables
    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> helmetSelection = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> playerHealth = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> playerIsDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> team = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Helmets
    public GameObject Helmet1;
    public GameObject Helmet2;
    public GameObject Helmet3;
    public GameObject Helmet4;

    //Railgun
    public GameObject Railgun;
 

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        if (IsOwner) playerName.Value = PlayerPrefs.GetString("PlayerName");

        //Helmet Assignment
        if (IsOwner) helmetSelection.Value = PlayerPrefs.GetInt("Helmet");
        
        // find character controller
        characterController = GetComponent<CharacterController>();

        // find and set animator IDs
        hasAnimator = TryGetComponent(out animator);
        hasRailgunAnimator = Railgun.TryGetComponent(out animator);
        AssignAnimationIDs();


        // SetNameServerRpc(playerName.Value.ToString());
    }

    private void AssignAnimationIDs()
    {
        animIDMelee = Animator.StringToHash("Melee");
        animIDRailgunShoot = Animator.StringToHash("Shoot");
        animIDJump = Animator.StringToHash("Jump");
        animIDHit = Animator.StringToHash("Hit");
        animIDDeath = Animator.StringToHash("Death");
        animIDLaser = Animator.StringToHash("Laser");
        animIDWallSummon = Animator.StringToHash("WallSummon");
        animIDMissilesCast = Animator.StringToHash("MissilesCast");
        animIDRunning = Animator.StringToHash("Running");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDSpeed = Animator.StringToHash("Speed");
        animIDMotionX = Animator.StringToHash("MotionX");
        animIDMotionZ = Animator.StringToHash("MotionZ");
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
            if (Input.GetKeyDown(KeyCode.E)) WallSpawnServerRpc();
            if (Input.GetKeyDown(KeyCode.Mouse0)) Shoot();
           
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




        if(playerIsDead.Value == true)
        {
            //play death animation
            animator.SetTrigger(animIDDeath);
            Invoke("Spawn", 3);
        }



        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    //swap cameras
        //    topcam.enabled = !topcam.enabled;
        //    fpcam.GetComponent<Camera>().enabled = !fpcam.GetComponent<Camera>().enabled;
        //}
        //  https://addam-davis1989.medium.com/jumping-with-physics-based-character-controller-in-unity-45462a04e62


       if(IsOwner) DebugInputs();

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

    public void Spawn()
    {
        playerIsDead.Value = false;
        playerHealth.Value = maxHealth;
        if (team.Value == 1) //Spawn on Team1 spawn point
        {
            GameObject[] spawnPos = GameObject.FindGameObjectsWithTag("Team1Spawn");
            transform.position = spawnPos[playerNumber].transform.position;
        }
        else if (team.Value == 2) //Spawn on Team2 spawn point
        {
            GameObject[] spawnPos = GameObject.FindGameObjectsWithTag("Team2Spawn");
            transform.position = spawnPos[playerNumber].transform.position;
        }
        else //Spawn Randomly on map
        {
            transform.position = new Vector3(Random.Range(-20f, 20f), 1f, Random.Range(-20f, 20f));
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
        railgunAnimator.SetTrigger(animIDRailgunShoot);
        Debug.Log("Weapon Charging");
        Invoke("HitScanServerRpc", weapon1ShotDelay);
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
       
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(fpcam.position, fpcam.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Player") & hit.collider.GetComponent<PlayerController>().team.Value != team.Value )
            {
                FindObjectOfType<HelloWorldManager>().DamagePlayerClientRpc(hit.collider.gameObject.GetComponent<PlayerController>(), weapon1Damage);
                Debug.DrawRay(fpcam.position, fpcam.forward * hit.distance, Color.yellow);
                Debug.Log(playerName.Value + "Landed a Shot");
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
    void WallSpawnServerRpc()
    {
        var obj = Instantiate(prefab);
        obj.position = transform.position + 4 * transform.forward + -0.1f * transform.up;
        obj.rotation = transform.rotation;
        obj.Rotate(-90, 0, 0);
        obj.GetComponent<NetworkObject>().Spawn(true);
    }

    public override void OnNetworkSpawn()
    {

        if (IsOwner)
        {
            Spawn();
        }

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
