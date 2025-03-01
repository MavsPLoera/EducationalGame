using UnityEngine;

public class FakeEnemy_Controller : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float turnSpeed;
    public GameObject player;
    public AudioClip[] destroyedAudioClips;
    public Sprite[] spriteImages;
    public GameObject[] moveTowardsLocations;
    public bool changeMovement;
    public float startGravity = 0.0f;
    public float endGravity = 5.0f;
    public float currentTime = 0.0f;
    public float desiredTime = 2.0f;
    public Rigidbody2D rb;
    public ParticleSystem deathParticles;

    private Transform moveTwoardsLocationSelected;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private ParticleSystem enemeyParticleSystem;

    void Start()
    {
        //Set up enemy "stats" before spawing
        turnSpeed = Random.Range(20f, 80f);
        moveSpeed = Random.Range(2f, 3f);
        audioSource = GameObject.Find("Enemy_AudioSource").GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        Transform enemyTransform = GetComponent<Transform>();
        float scale = Random.Range(1f, 2f);
        enemyTransform.transform.localScale = new Vector3(scale, scale, 1);
        enemeyParticleSystem = GetComponent<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveTwoardsLocationSelected = moveTowardsLocations[Random.Range(0, moveTowardsLocations.Length)].GetComponent<Transform>();
        spriteRenderer.sprite = spriteImages[Random.Range(0, spriteImages.Length)];
        changeMovement = false;
    }

    void Update()
    {
        /*
         * Normal movement for enemy is to travel from one side of the spawn radius
         * to its inverse
         * will destroy itself once the asteroid equals the inverse of the starting position.
         * 
         * Only case this would change is when the player is warping.
         * We will halt the movement and turn on the enemies gravity.
         * Change the destroy object if the y-pos is less than -20
         */

        if (changeMovement == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveTwoardsLocationSelected.position, moveSpeed * Time.deltaTime);
            transform.Rotate(0f, 0f, turnSpeed * Time.deltaTime);

            if (transform.position == moveTwoardsLocationSelected.position)
                Destroy(gameObject);
        }
        else
        {
            currentTime += Time.deltaTime;
            rb.gravityScale = Mathf.Lerp(startGravity, endGravity, currentTime / desiredTime);

            if (transform.position.y < -20)
                Destroy(gameObject);
        }
    }

    public void death()
    {
        //Select random audio clip and let player know that they killed a enemy.
        audioSource.PlayOneShot(destroyedAudioClips[Random.Range(0, destroyedAudioClips.Length)]);
        PlayerBarrel_Controller.instance.enemyDestroyed();
        
        //Destory particles after they play
        ParticleSystem particles = Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(particles.gameObject, particles.main.duration);
        Destroy(gameObject);
    }
}
