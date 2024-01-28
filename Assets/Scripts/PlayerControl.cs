using UnityEngine;

public class PlayerControl : MonoBehaviour {
    public float speed = 1f;
    public bool scary = false;
    public float not_scary_since;
    public float new_item_since;
    const int NOT_SCARY_FOR_SECONDS = 5;
    public AudioClip booClip;
    public AudioSource booSource;
    public bool mask_exists = false;
    public bool item_exists = false;
    public GameObject pumpkinPrefab;
    public GameObject bananaPrefab;
    public GameObject bananaPeelPrefab;
    public GameObject vasePrefab;
    public GameObject vaseBrokenPrefab;
    public GameObject mask = null;
    public GameObject item = null;
    SpriteRenderer sprite;
    Animator animator;

    const int TIME_UNTIL_PUMPKIN = 3;
    const int TIME_UNTIL_ITEM = 5;

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
            if (item != null) {
                if (item.name.Contains("banana")) {
                    Destroy(item);
                    item = null;
                    var peel = Instantiate(bananaPeelPrefab);
                    peel.transform.position = transform.position;
                } else if (item.name.Contains("vase")) {
                    Destroy(item);
                    item = null;
                    var trash = Instantiate(vaseBrokenPrefab);
                    trash.transform.position = transform.position;
                }

            }
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
        if (item != null) {
            item.transform.localPosition = new(sprite.flipX ? -0.2f: 0.2f, 0);
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
        if (!mask_exists) {
            if (!scary && Time.time - not_scary_since > TIME_UNTIL_PUMPKIN) {
                mask_exists = true;
                var points_ = GameObject.FindGameObjectsWithTag("PathPoint");
                var point_ = points_[Random.Range(0, points_.Length)];
                var mask_ = Instantiate(pumpkinPrefab);
                mask_.transform.position = point_.transform.position;
                new_item_since = Time.time;
            }
        } else {
            if (!item_exists && Time.time - new_item_since > TIME_UNTIL_ITEM) {
                var points_ = GameObject.FindGameObjectsWithTag("PathPoint");
                var point_ = points_[Random.Range(0, points_.Length)];
                var item_ = Random.Range(0,2) > 0 ? Instantiate(bananaPrefab) : Instantiate(vasePrefab);
                item_.transform.position = point_.transform.position;
                item_exists = true;
                new_item_since = Time.time;
            }
        }
    }

    public void MarkAsNotScary() {
        scary = false;
        not_scary_since = Time.time;
        if (mask != null) {
            Destroy(mask);
            mask = null;
            mask_exists = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Mask")) {
            mask = collider.gameObject;
            mask.transform.parent = transform;
            mask.transform.localPosition = new(0, 0.4f);
            scary = true;
        }
        if (collider.gameObject.CompareTag("Item")) {
            item = collider.gameObject;
            item.transform.parent = transform;
            item.transform.localPosition = new(sprite.flipX ? -0.2f: 0.2f, 0);
        }
    }    
}
