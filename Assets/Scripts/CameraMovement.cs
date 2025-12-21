using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject player;
    private Vector3 playerPosition;
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
    }

    void LateUpdate()
    {
        transform.position = playerPosition;
    }
}
