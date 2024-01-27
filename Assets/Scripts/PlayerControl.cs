using UnityEngine;

public class PlayerControl : MonoBehaviour {
    public float speed = 1f;
    public bool scary = true;
    public float not_scary_since;
    const int NOT_SCARY_FOR_SECONDS = 1;
    public AudioClip booClip;
    private AudioSource booSource;
    SpriteRenderer sprite;
    Animator animator;

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
        if (scary) {
            sprite.color = new Color(0, 1, 0);
        } else {
            sprite.color = new Color(1, 1, 1);
            if (Time.time - not_scary_since > NOT_SCARY_FOR_SECONDS) {
                scary = true;
            }
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
}
