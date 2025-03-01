using UnityEngine;

public class Bullet_Controller : MonoBehaviour
{
    private Vector3 bulletDirection;
    public float bulletSpeed = 100f;

    //Set bullet direction and direction of the bullet to where mouse is.
    public void setUp(Vector3 bulletDirection, float rotation)
    {
        this.bulletDirection = bulletDirection;
        transform.eulerAngles = new Vector3(0f, 0f,  rotation);
        Destroy(gameObject, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += bulletDirection * bulletSpeed * Time.deltaTime;
    }

    //If bullet collides with fake or real enemy tell enemy to die and add to player score
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Enemy_Controller>(out Enemy_Controller enemy))
        {
            Destroy(gameObject);
            enemy.death();
        }
        else if(collision.TryGetComponent<FakeEnemy_Controller>(out FakeEnemy_Controller fakeEnemy))
        {
            Destroy(gameObject);
            fakeEnemy.death();
        }
    }
}
