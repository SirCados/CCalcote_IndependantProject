using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    PlayerInput _playerInput;
    Rigidbody _playerRigidBody;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerRigidBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MovePlayer()
    {
        
    }
}
