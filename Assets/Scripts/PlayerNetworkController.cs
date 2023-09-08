using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private NetworkVariable<float> randomFloatNumber = new NetworkVariable<float>(5.5f,
                                                            NetworkVariableReadPermission.Everyone,
                                                            NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        randomFloatNumber.OnValueChanged += RandomNumberOnValueChanged;
    }

    private void RandomNumberOnValueChanged(float previousValue, float newValue)
    {
        Debug.Log(OwnerClientId + " : " + randomFloatNumber.Value);
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
            randomFloatNumber.Value = Random.Range(1, 100);
        }

        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDir.z += 1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z -= 1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x -= 1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x += 1f;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
