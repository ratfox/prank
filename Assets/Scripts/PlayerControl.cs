using UnityEngine;

public class PlayerControl : MonoBehaviour {
    public float speed = 1f;
    public bool scary = true;
    public float not_scary_since;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start() {
        sprite = GetComponent<SpriteRenderer>();    
    }

    // Update is called once per frame
    void Update() {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");
        if (x != 0 || y != 0) {
            Move(x, y);
        }
        if (scary) {
            sprite.color = new Color(0, 1, 0);
        } else {
            sprite.color = new Color(1, 1, 1);
            if (Time.time - not_scary_since > 10) {
                scary = true;
            }
        }
    }

    private void Move(float x, float y) {
        sprite.flipX = x < 0;
        var pos = transform.position;
        pos += new Vector3(x, y, 0);
        transform.position = Vector2.Lerp(transform.position, pos, Time.deltaTime * speed);
    }
}