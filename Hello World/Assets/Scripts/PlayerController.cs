﻿using System.Collections;
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

    public float gravity = 9.81f;

    public float jumpSpeed = 5;
    public float jumpBoostMultiplier = 1.2f;

    Vector3 moveDirection = Vector3.zero;

    public int maxHealth = 10;
    public int weapon1Damage = 2;

    CharacterController characterController;

    //Team Assignment
    public enum team
    {
        Team1,
        Team2,
        NoTeam
    }
    public team myTeam = team.Team1;
    public int playerNumber;

    // Animator
    private bool hasAnimator;
    private Animator animator;

    // Animation IDs
    private int animIDMelee;
    private int animIDShoot;
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
    [SerializeField] private Transform prefab;

    [SerializeField]
    TextMesh playerNameDisplay;

    //Network Variables
    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> playerHealth = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> playerIsDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //Helmets
    public GameObject Helmet1;
    public GameObject Helmet2;
    public GameObject Helmet3;
    public GameObject Helmet4;
 

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
        int helmetSelection = PlayerPrefs.GetInt("Helmet");
        if (helmetSelection == 1) Helmet1.SetActive(true);
        if (helmetSelection == 2) Helmet2.SetActive(true);
        if (helmetSelection == 3) Helmet3.SetActive(true);
        if (helmetSelection == 4) Helmet4.SetActive(true);


        characterController = GetComponent<CharacterController>();

        // find and set animator IDs
        hasAnimator = TryGetComponent(out animator);
        AssignAnimationIDs();


        // SetNameServerRpc(playerName.Value.ToString());


    }

    private void AssignAnimationIDs()
    {
        animIDMelee = Animator.StringToHash("Melee");
        animIDShoot = Animator.StringToHash("Shooting");
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

        playerNameDisplay.text = playerName.Value.ToString() + myTeam;
        if (IsOwner & playerIsDead.Value == false)
        {
            if (Input.GetKeyDown(KeyCode.E)) WallSpawnServerRpc();
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                HitScanServerRpc();
                //fire animation
            }
            //Movement Input
            float forward = Input.GetAxisRaw("Vertical");
            float right = Input.GetAxisRaw("Horizontal");
            float turn =  Input.GetAxis("Mouse X");
            float tilt = Input.GetAxis("Mouse Y");

            animator.SetFloat(animIDMotionZ, forward, 1f, Time.deltaTime * 10f);
            animator.SetFloat(animIDMotionX, right, 1f, Time.deltaTime * 10f);


            //camera rotation
            transform.Rotate(new Vector3(0, turn * turnSpeed * Time.deltaTime, 0));
            if (fpcam != null) fpcam.Rotate(new Vector3(-tilt * tiltSpeed * Time.deltaTime, 0));
            //camera rotation lock doesnt work
            //fpcam.transform.eulerAngles = new Vector3( Mathf.Clamp(fpcam.transform.rotation.x,0,180), fpcam.transform.eulerAngles.y);



            if (characterController.isGrounded)
            { 
                moveDirection = ((transform.TransformDirection(Vector3.forward)* forward) + (transform.TransformDirection(Vector3.right)*right)).normalized;

                animator.SetFloat(animIDSpeed, 0);
                animator.SetBool(animIDGrounded, true);
                animator.SetBool(animIDRunning, false);

                if (Input.GetKey(KeyCode.LeftShift) & forward > 0)
                {
                    moveDirection *= sprintSpeed;

                    animator.SetBool(animIDRunning, true);
                    animator.SetFloat(animIDSpeed, sprintSpeed);
                }
                else if(new Vector2(right,forward).magnitude > 0)
                {
                    moveDirection *= walkSpeed;

                    animator.SetBool(animIDRunning, false);
                    animator.SetFloat(animIDSpeed, walkSpeed);

                    
                }

                if (Input.GetButton("Jump"))
                {
                    moveDirection *= jumpBoostMultiplier;
                    moveDirection.y = jumpSpeed;

                    // Trigger Jump Param in Animation Controller
                    animator.SetTrigger(animIDJump);
                    animator.SetBool(animIDGrounded, false);
                }
            }
            else
            {

            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
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




    }

    public void Spawn()
    {
        playerIsDead.Value = false;
        playerHealth.Value = maxHealth;
        if (myTeam == team.Team1) //Spawn on Team1 spawn point
        {
            GameObject[] spawnPos = GameObject.FindGameObjectsWithTag("Team1Spawn");
            transform.position = spawnPos[playerNumber].transform.position;
        }
        if (myTeam == team.Team2) //Spawn on Team2 spawn point
        {
            GameObject[] spawnPos = GameObject.FindGameObjectsWithTag("Team2Spawn");
            transform.position = spawnPos[playerNumber].transform.position;
        }
        if (myTeam == team.NoTeam) //Spawn Randomly on map
        {
            transform.position = new Vector3(Random.Range(-20f, 20f), 1f, Random.Range(-20f, 20f));
        }
    }

    
    public void Hit()
    {
 

        animator.SetTrigger(animIDHit);

      
    }


    [ServerRpc]
    void SetNameServerRpc(string name)
    {
        playerNameDisplay.text = name;
    }

    [ServerRpc]
    void HitScanServerRpc()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(fpcam.position, fpcam.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Player"))
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

    [ServerRpc]
    void WallSpawnServerRpc()
    {
        var obj = Instantiate(prefab);
        obj.position = transform.position + 4 * transform.forward + 1 * transform.up;
        obj.rotation = transform.rotation;
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
