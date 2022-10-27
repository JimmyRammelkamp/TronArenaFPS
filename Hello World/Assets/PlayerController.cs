using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float turnSpeed = 180;
    public float tiltSpeed = 180;
    public float walkSpeed = 1;

    [SerializeField]
    private Transform fpcam;
    private Camera topcam;
    [SerializeField] private Transform prefab;

    [SerializeField]
    TextMesh nick;

    //NetworkVariable<float> forward = new NetworkVariable<float>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    //NetworkVariable<float> turn = new NetworkVariable<float>(0,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //NetworkVariable<FixedString128Bytes> nickname = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    // Start is called before the first frame update
    void Start()
    {
        // Cursor.lockState = CursorLockMode.Confined;
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
            float forward = Input.GetAxis("Vertical");
            float turn = Input.GetAxis("Horizontal") + Input.GetAxis("Mouse X");
            transform.Translate(new Vector3(0, 0, forward * walkSpeed * Time.deltaTime));
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

        //nick.text = nickname.Value.ToString();
    }

    [ServerRpc]
    void RelayInputServerRpc(float forward, float turn)
    {
        transform.Translate(new Vector3(0, 0, forward));
        transform.Rotate(new Vector3(0, turn, 0));
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

    //public void SetNickname(string name)
    //{
    //    nickname.Value = name;
    //}
}
