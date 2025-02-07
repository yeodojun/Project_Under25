using UnityEngine;

public class BackGround : MonoBehaviour
{
    private float movespeed = 3f;
    void Update() {
        transform.position += Vector3.down * movespeed * Time.deltaTime;
        if (transform.position.y < -10) {
            transform.position += new Vector3(0, 20f, 0);
        }
    }
    
}
