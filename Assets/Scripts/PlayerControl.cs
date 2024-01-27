using UnityEngine;

public class PlayerControl : MonoBehaviour {
    public float speed = 1f;
    public bool scary = false;
    public float not_scary_since;
    const int NOT_SCARY_FOR_SECONDS = 5;
    public AudioClip booClip;
    public AudioSource booSource;
    public bool mask_created = false;
    public GameObject pumpkinPrefab;
    public GameObject mask = null;
    SpriteRenderer sprite;
    Animator animator;

    const int TIME_UNTIL_PUMPKIN = 3;

    // Start is called before the first frame update
    void Start() {
        sprite = GetComponent<SpriteRenderer>();   
        animator = GetComponent<Animator>();
        booSource = gameObject.AddComponent<AudioSource>();
        booSource.clip = booClip;
    }

    // Update is called once per frame
    void Update() {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space)) {
            booSource.Play();
        }
        if (x != 0 || y != 0) {
            animator.SetBool("is_walking", true);
            Move(x, y);
        } else {
            animator.SetBool("is_walking", false);
        }
        if (mask != null) {
            mask.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
        }
    }

    private void Move(float x, float y) {
        if (x != 0) {
            sprite.flipX = x < 0;
        }
        var pos = transform.position;
        pos += new Vector3(x, y, 0);
        transform.position = Vector2.Lerp(transform.position, pos, Time.deltaTime * speed);
    }

    void FixedUpdate() {
        if (!scary && !mask_created && Time.time - not_scary_since > TIME_UNTIL_PUMPKIN) {
            var points_ = GameObject.FindGameObjectsWithTag("PathPoint");
            var point_ = points_[Random.Range(0, points_.Length)];
            var mask_ = Instantiate(pumpkinPrefab);
            mask_.transform.position = point_.transform.position;
            mask_created = true;
        }
    }

    public void MarkAsNotScary() {
        scary = false;
        not_scary_since = Time.time;
        Destroy(mask);
        mask = null;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Mask")) {
            mask = collider.gameObject;
            mask.transform.parent = transform;
            mask.transform.localPosition = new(0, 0.4f);
            scary = true;
            mask_created = false;
        }
    }
    
}
