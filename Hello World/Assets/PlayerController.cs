using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float turnSpeed = 180;
    public float tiltSpeed = 180;
    public float walkSpeed = 5;

    [SerializeField]
    private Transform fpcam;
    private Camera topcam;
    [SerializeField] private Transform prefab;

    [SerializeField]
    TextMesh playerNameDisplay;

    //NetworkVariable<float> forward = new NetworkVariable<float>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    //NetworkVariable<float> turn = new NetworkVariable<float>(0,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (IsOwner) playerName.Value = PlayerPrefs.GetString("PlayerName");

        playerNameDisplay.text = playerName.Value.ToString();
    }



     // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SpawnServerRpc();
            }
            if (Input.GetKeyDown(KeyCode.Mouse0)) HitScanServerRpc();
            float forward = Input.GetAxisRaw("Vertical");
            float turn =  Input.GetAxis("Mouse X");
            float right = Input.GetAxisRaw("Horizontal");
            transform.Translate(new Vector3(right, 0, forward).normalized * walkSpeed * Time.deltaTime);
            transform.Rotate(new Vector3(0, turn * turnSpeed * Time.deltaTime, 0));
            //RelayInputServerRpc(forward *walkSpeed * Time.deltaTime, turn * turnSpeed * Time.deltaTime); //this needs time delta time



        }
        //if(IsServer)
        //{
        //    transform.Translate(new Vector3(0, 0, forward.Value * walkSpeed * Time.deltaTime)); //this is for network variable isntead of rpc
        //    transform.Rotate(new Vector3(0, turn.Value * turnSpeed * Time.deltaTime, 0));
        //}


        float tilt = Input.GetAxis("Mouse Y");
        if (fpcam != null)
            fpcam.Rotate(new Vector3(-tilt * tiltSpeed * Time.deltaTime, 0));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //swap cameras
            topcam.enabled = !topcam.enabled;
            fpcam.GetComponent<Camera>().enabled = !fpcam.GetComponent<Camera>().enabled;
        }

        
    }

    [ServerRpc]
    void RelayInputServerRpc(float forward, float turn)
    {
        transform.Translate(new Vector3(0, 0, forward));
        transform.Rotate(new Vector3(0, turn, 0));
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
               //Debug.DrawRay(, fpcam.forward * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
            }
            
        }
        else
        {
            Debug.DrawRay(fpcam.position, fpcam.forward * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }

    [ServerRpc]
    void SpawnServerRpc()
    {
        var obj = Instantiate(prefab);
        obj.position = transform.position + 4 * transform.forward;
        obj.GetComponent<NetworkObject>().Spawn(true);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            transform.position = new Vector3(Random.Range(-20f, 20f), 1f, Random.Range(-20f, 20f));
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
