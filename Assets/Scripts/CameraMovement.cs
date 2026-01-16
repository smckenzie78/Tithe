using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject player;
    private Vector3 playerPosition;
    private Camera mainCamera;
    private float cameraSize;
    public float zoomOutSize;
    public float zoomInSize;
    public float zoomSpeed;
    void Start()
    {
        player = GameObject.Find("Player");
        mainCamera = Camera.main;
        cameraSize = zoomInSize;
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cameraSize = zoomOutSize;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cameraSize = zoomInSize;
        }
    }

    void LateUpdate()
    {
        transform.position = playerPosition;
        mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, cameraSize, zoomSpeed * Time.deltaTime);
    }
}
