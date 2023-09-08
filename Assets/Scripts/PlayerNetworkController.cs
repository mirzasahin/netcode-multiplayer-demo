using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject spawnableObjectPrefab;
    private GameObject spawnableObject;

    private NetworkVariable<CustomData> randomDataType = new NetworkVariable<CustomData>(
        new CustomData
        {
            myInt = 14,
            myBool = true,
            myString = "Hello world!",
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct CustomData : INetworkSerializable
    {
        public int myInt;
        public bool myBool;
        public FixedString128Bytes myString;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref myInt);
            serializer.SerializeValue(ref myBool);
            serializer.SerializeValue(ref myString);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomDataType.OnValueChanged += RandomNumberOnValueChanged;
    }

    private void RandomNumberOnValueChanged(CustomData previousValue, CustomData newValue)
    {
        Debug.Log(OwnerClientId + " : " + newValue.myInt + " : " + newValue.myBool + " : " + newValue.myString);
    }

    // Update is called once per frame
    void Update()
    {
        HandleNetworking();
    }


    private void HandleNetworking()
    {
        if (!IsOwner) return;

        if (Input.GetKeyUp(KeyCode.S))
        {
            spawnableObject = Instantiate(spawnableObjectPrefab);
            spawnableObject.GetComponent<NetworkObject>().Spawn();
            //randomDataType.Value = new CustomData
            //{
            //    myInt = 10,
            //    myBool = false,
            //    myString = "How are you?"
            //};
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            Destroy(spawnableObject);
        }

        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDir.z += 1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z -= 1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x -= 1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x += 1f;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
