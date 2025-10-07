using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    Vector3 offset;
    Vector3 newPos;
    public GameObject gamePlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = gamePlayer.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        newPos = transform.position;
        newPos.x = gamePlayer.transform.position.x - offset.x;
        newPos.z = gamePlayer.transform.position.z - offset.z;
        transform.position = newPos;
    }

}
