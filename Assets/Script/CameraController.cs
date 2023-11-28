using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float panSpeed = 10f;
    //valeur min et max
    [SerializeField] float panBorderThickness = 10f;
    //use map size to set limit
    public Vector2 panLimit;

    //require to be more flexible in negative if angle < 90
    [SerializeField] float scrollSpeed = 20f;
    [SerializeField] float minY = 3f;
    [SerializeField] float maxY = 15f;

    //add rotation 
    //add a follow/unfollow object function
    //ajuster l'angle entre 60 et 90 degré quand on dézoom

    void Update()
    {
        Vector3 pos = transform.position;

        if(Input.GetKey(KeyCode.Z) || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q) || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("MouseScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y - 5, panLimit.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;

        panSpeed = Mathf.Lerp(8, 20, (pos.y - minY) / maxY);
    }
}
