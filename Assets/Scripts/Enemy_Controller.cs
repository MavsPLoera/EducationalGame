using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    [Header("Enemy Information")]
    public float moveSpeed;
    public float minMoveSpeed = 2.0f;
    public float maxMoveSpeed = 3.0f;
    public float minScale = 1f;
    public float maxScale = 1f;
    public float turnSpeed;
    public bool changeMovement;
    public float startGravity = 0.0f;
    public float endGravity = 5.0f;
    public float currentTime = 0.0f;
    public float desiredTime = 2.0f;

    public Sprite[] spriteImages;
    public AudioClip[] destroyedAudioClips; 
    private SpriteRenderer spriteRenderer;
    public GameObject player;
    public Rigidbody2D rb;
    public Vector3 inversedSpawnTrasform;
    private AudioSource audioSource;
    public ParticleSystem deathParticles;

    //Can probably change the sprite based on a random number.
    //have an array of sprite images

    void Start()
    {
        //Set up enemy "stats" before spawing
        turnSpeed = Random.Range(20f, 80f);
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        audioSource = GameObject.Find("Enemy_AudioSource").GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        Transform enemyTransform = GetComponent<Transform>();
        float scale = Random.Range(minScale, maxScale);
        enemyTransform.transform.localScale = new Vector3(scale, scale, 1);
        inversedSpawnTrasform = -(enemyTransform.transform.position);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteImages[Random.Range(0, spriteImages.Length)];
        changeMovement = false;
    }

    // Update is called once per frame
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
            transform.position = Vector2.MoveTowards(transform.position, inversedSpawnTrasform, moveSpeed * Time.deltaTime);
            transform.Rotate(0f, 0f, turnSpeed * Time.deltaTime);

            if (transform.position == inversedSpawnTrasform)
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

        //Destroy particles after they play.
        ParticleSystem particles = Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(particles.gameObject, particles.main.duration);
        Destroy(gameObject);
    }

    //Resets the enemy stats to what they were at the beggining
    //Was having weird behaviour when a played played a game the enemies would remember their increased difficulty stats.
    public void resetStats()
    {
        minMoveSpeed = 2.0f;
        maxMoveSpeed = 3.0f;
        minScale = 1f;
        maxScale = 1f;
    }
}
