using UnityEngine;

public class VictimControl : MonoBehaviour {
    const int SCARED_SECONDS = 2;
    const float DEFAULT_SPEED = 1.5f;
    const float SCARED_SPEED = 2.5f;
    const float MIN_SCARY_DISTANCE = 1.5f;
    const int MIN_WALK_DISTANCE = 3;
    const int VISION_DISTANCE = 5;

    public float speed = 1f;
    public Vector2 direction;
    public GameObject player = null;
    public float x = 0;
    public AudioClip scaredClip;
    private AudioSource scaredSource;
    public float y = 0;
    public float distance_view = 0;
    public bool debug = false;
    public bool scared = false;
    public float scared_since;
    public bool stunned = false;
    public float stunned_since;
    public GameObject debug_object = null;
    SpriteRenderer sprite;
    Animator animator;
    Collider2D my_collider;

    // Start is called before the first frame update
    void Start() {
        sprite = GetComponent<SpriteRenderer>();   
        animator = GetComponent<Animator>();   
        my_collider = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player"); 
        scaredSource = gameObject.AddComponent<AudioSource>();
        scaredSource.clip = scaredClip;
        scaredSource.volume = 0.2f;
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
        speed = scared ? SCARED_SPEED : stunned ? 0 : DEFAULT_SPEED;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!my_collider.enabled) {
            return;
        }
        Debug.Log("intersection");
        direction = GetRandomDirection();
        if (collision.gameObject.CompareTag("Victim")) {
            if (collision.gameObject.GetComponent<VictimControl>().scared) {
                stunned = true;
                animator.SetBool("is_stunned", true);
                stunned_since = Time.time;
                my_collider.enabled = false;
            }
        }
        if (collision.gameObject.CompareTag("Player")) {
            markPlayerAsNotScary();
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (direction == Vector2.zero) {
            direction = GetRandomDirection();
        }
    }


    void FixedUpdate() {
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, VISION_DISTANCE);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player")) {
            if (distance < MIN_SCARY_DISTANCE) {
                getScared(dir_player);
            } else {
                markPlayerAsNotScary();
            }
        }
    }

    private void getScared(Vector2 dir_player) {
        scaredSource.Play();
        scared = true;
        animator.SetBool("is_scared", true);
        scared_since = Time.time;
        markPlayerAsNotScary();
    }

    private void markPlayerAsNotScary() {
        player.GetComponent<PlayerControl>().MarkAsNotScary();
    }
}
