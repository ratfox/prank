using UnityEngine;

public class VictimControl : MonoBehaviour {
    const int SCARED_SECONDS = 2;
    const float DEFAULT_SPEED = 1.5f;
    const float SCARED_SPEED = 3f;
    const float MIN_SCARY_DISTANCE = 2.5f;
    const int MIN_WALK_DISTANCE = 3;
    const int VISION_DISTANCE = 5;

    public float speed = 1f;
    public Vector2 direction;
    public GameObject player = null;
    public float x = 0;
    public AudioClip scaredClip;
    public AudioClip[] notScary;
    private AudioSource audioSource;
    public float y = 0;
    public float distance_view = 0;
    public bool debug = false;
    public bool scared = false;
    public float scared_since;
    public bool stunned = false;
    public float stunned_since;
    public GameObject explosionPrefab;
    SpriteRenderer sprite;
    Animator animator;
    Collider2D my_collider;

    // Start is called before the first frame update
    void Start() {
        sprite = GetComponent<SpriteRenderer>();   
        animator = GetComponent<Animator>();   
        my_collider = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player"); 
        audioSource = gameObject.AddComponent<AudioSource>();
        direction = GetRandomDirection();
    }
    
    Vector2 GetRandomDirection() {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 randomDirection = Vector2.zero;
        // Shuffle the array to randomize the order of directions
        ShuffleArray(directions);
        foreach (Vector2 dir in directions)
        {
            // Cast a ray in the current direction
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, MIN_WALK_DISTANCE);
            // Check if there are no collidable objects within the specified distance
            if (hit.collider == null)
            {
                randomDirection = dir;
                break;
            }
        }
        return randomDirection;
    }

    // Helper method to shuffle the array
    private void ShuffleArray<T>(T[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + Random.Range(0, n - i);
            T temp = array[r];
            array[r] = array[i];
            array[i] = temp;
        }
    }

    void updatePic() {
        if (direction == Vector2.up) {
            animator.SetBool("is_back", true);
            animator.SetBool("is_front", false);
            return;
        }
        if (direction == Vector2.down) {
            animator.SetBool("is_back", false);
            animator.SetBool("is_front", true);
            return;
        }
        animator.SetBool("is_front", false);
        animator.SetBool("is_back", false);
        if (direction == Vector2.left) {
            sprite.flipX = true;
        }
        if (direction == Vector2.right) {
            sprite.flipX = false;
        }
    }

    // Update is called once per frame
    void Update() {
        updatePic();
        speed = stunned ? 0 : scared ? SCARED_SPEED : DEFAULT_SPEED;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!my_collider.enabled) {
            return;
        }
        direction = GetRandomDirection();
        if (collision.gameObject.CompareTag("Victim")) {
            if (collision.gameObject.GetComponent<VictimControl>().scared) {
                getStunned();
            }
        }
        if (collision.gameObject.CompareTag("Player")) {
            markPlayerAsNotScary(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (scared && collider.gameObject.CompareTag("Trap")) {
            getStunned();
            Destroy(collider.gameObject);
            player.GetComponent<PlayerControl>().item_exists = false;
            player.GetComponent<PlayerControl>().new_item_since = Time.time;
        }
    }

    void FixedUpdate() {
        if (direction == Vector2.zero) {
            direction = GetRandomDirection();
        }
        if (scared) {
            if (Time.time - scared_since > SCARED_SECONDS) {
                scared = false;
                animator.SetBool("is_scared", false);
            }
            return;
        }
        if (stunned) {
            if (Time.time - stunned_since > 4) {
                stunned = false;
                animator.SetBool("is_stunned", false);
                my_collider.enabled = true;

            }
            return;
        }
        // Calculate the distance from the player
        float distance = Vector2.Distance(player.transform.position, transform.position);
        var dir_player =  player.transform.position - transform.position;
        if (distance < MIN_SCARY_DISTANCE &&
                player.GetComponent<PlayerControl>().scary &&
                player.GetComponent<PlayerControl>().booSource.isPlaying) {
            getScared(dir_player);
        }
        if (Vector2.Angle(dir_player, direction) < 15) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir_player, VISION_DISTANCE);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player")) {
                if (distance < MIN_SCARY_DISTANCE && player.GetComponent<PlayerControl>().scary) {
                    getScared(dir_player);
                } else {
                    markPlayerAsNotScary(true);
                }
            }
        }
    }

    private void getStunned() {
        stunned = true;
        animator.SetBool("is_stunned", true);
        Destroy(Instantiate(explosionPrefab, transform.position, transform.rotation), 0.9f);
        stunned_since = Time.time;
        my_collider.enabled = false;
    }

    private void getScared(Vector2 dir_player) {
        audioSource.clip = scaredClip;
        audioSource.volume = 0.01f;
        audioSource.Play();
        scared = true;
        animator.SetBool("is_scared", true);
        scared_since = Time.time;
        markPlayerAsNotScary(false);
    }

    private void markPlayerAsNotScary(bool with_sound) {
        if (!player.GetComponent<PlayerControl>().scary) {
            return;
        }
        player.GetComponent<PlayerControl>().MarkAsNotScary();
        if (!with_sound) {
            return;
        }
        // Get a random index within the array length
        int randomIndex = Random.Range(0, notScary.Length);
        // Assign the randomly selected audio clip to the AudioSource
        audioSource.clip = notScary[randomIndex];
        audioSource.volume = 1f;
        audioSource.Play();
    }
}
