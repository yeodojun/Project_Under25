using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private GameObject[] weapons;
    private int weaponIndex = 0;

    [SerializeField]
    private Transform shootTransform;

    [SerializeField]
    private float shootInterval = 0.05f;
    private float lastShootTime;
    void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float toX = Mathf.Clamp(mousePos.x, -1.75f, 1.75f);
        float toY = Mathf.Clamp(mousePos.y, -4.5f, 4.5f);
        transform.position = new Vector3(toX, toY, transform.position.z);
        Shoot();
    }
    
    void Shoot(){
        if (Time.time - lastShootTime > shootInterval) {
            Instantiate(weapons[weaponIndex], shootTransform.position, Quaternion.identity);
            lastShootTime = Time.time;
        }
    }
}
