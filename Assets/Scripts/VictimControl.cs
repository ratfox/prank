using UnityEngine;

public class VictimControl : MonoBehaviour {
    public float speed = 1f;
    public int dir = 0;
    public bool on_path_point = false;
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
    const int SCARED_SECONDS = 2;
    const float DEFAULT_SPEED = 1.5f;
    const float SCARED_SPEED = 2.5f;
    const int MIN_SCARY_DISTANCE = 3;
    GameObject path_point = null;
    public bool leaving_path_point = false;
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
    }

    // Update is called once per frame
    void Update() {
        switch (dir) {
            case 90:
              animator.SetBool("is_front", false);
              animator.SetBool("is_back", true);
              break;
            case 270:
              animator.SetBool("is_front", true);
              animator.SetBool("is_back", false);
              break;
            default:
              animator.SetBool("is_front", false);
              animator.SetBool("is_back", false);
              break;
        }
        float x = dir == 0 ? 1 : dir == 180 ? -1 : 0;
        float y = dir == 90 ? 1 : dir == 270 ? -1 : 0;
        speed = scared ? SCARED_SPEED : stunned ? 0 : DEFAULT_SPEED;
        sprite.flipX = x < 0;
        var pos = transform.position;
        if (on_path_point && !leaving_path_point) {
            pos = path_point.transform.position;
            var directions_ = path_point.GetComponent<PathPoint>().directions;
            if (Vector2.Distance(pos, transform.position) < 0.05) {
                dir = directions_[Random.Range(0, directions_.Length)];
                leaving_path_point = true;
            }
            transform.position = Vector2.Lerp(transform.position, pos, Time.deltaTime * speed);
        } else {
            pos += new Vector3(x, y, 0);
            transform.position = Vector2.Lerp(transform.position, pos, Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("PathPoint")) {
            on_path_point = true;
            path_point = collider.gameObject;
            leaving_path_point = false;
        }
    }

    private void OnTriggerLeave2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("PathPoint")) {
            on_path_point = false;
            path_point = null;
            leaving_path_point = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Victim")) {
            if (collision.gameObject.GetComponent<VictimControl>().scared) {
                stunned = true;
                animator.SetBool("is_stunned", true);
                stunned_since = Time.time;
                my_collider.enabled = false;
            } else {
                if (!scared) {
                    dir = dir < 135 ? dir + 180 : dir - 180;
                    if (on_path_point) {
                        leaving_path_point = !leaving_path_point;
                    }
                }
            }
        }
        if (collision.gameObject.CompareTag("Player")) {
            markPlayerAsNotScary();
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
        if (distance < MIN_SCARY_DISTANCE &&
                player.GetComponent<PlayerControl>().scary &&
                player.GetComponent<PlayerControl>().booSource.isPlaying) {
            getScared();
        } 
        var dir_player =  player.transform.position - transform.position;
        x = dir_player.x;
        y = dir_player.y;
        // Cast a ray to player.
        if ((dir_player.x > 0 && Mathf.Abs(dir_player.y) * 5 < dir_player.x && dir == 0) ||
            (dir_player.x < 0 && Mathf.Abs(dir_player.y) * 5 < -dir_player.x && dir == 180) ||
            (dir_player.y > 0 && Mathf.Abs(dir_player.x) * 5 < dir_player.y && dir == 90)  ||
            (dir_player.y < 0 && Mathf.Abs(dir_player.x) * 5 < -dir_player.y && dir == 270)) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir_player);
            distance_view = Vector2.Distance(player.transform.position, transform.position);
            if (hit.collider != null) debug_object = hit.collider.gameObject;
            // If it hits the player
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player")) {
                debug = true;
                distance_view = distance;
                if (distance < MIN_SCARY_DISTANCE && player.GetComponent<PlayerControl>().scary) {
                    getScared();
                } else {
                    markPlayerAsNotScary();
                }
            }
        }
    }

    private void getScared() {
        scaredSource.Play();
        scared = true;
        animator.SetBool("is_scared", true);
        scared_since = Time.time;
        dir = dir < 135 ? dir + 180 : dir - 180;
        markPlayerAsNotScary();
        if (on_path_point) {
            leaving_path_point = !leaving_path_point;
        }
    }

    private void markPlayerAsNotScary() {
        player.GetComponent<PlayerControl>().MarkAsNotScary();
    }
}
