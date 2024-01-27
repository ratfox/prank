using UnityEngine;

public class Walls : MonoBehaviour
{
    // Start is called before the first frame topdate
    void Start()
    {
        CreateScreenEdgeColliders();
    }

    void CreateScreenEdgeColliders() {
        var mainCamera = Camera.main;
        // Screen corners in world coordinates
        Vector2 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 topLeft = mainCamera.ScreenToWorldPoint(new Vector2(0, Screen.height));
        Vector2 topRight = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 bottomRight = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, 0));

        // Create EdgeCollider2D on this GameObject
        EdgeCollider2D edgeColliderBottom = gameObject.AddComponent<EdgeCollider2D>();
        edgeColliderBottom.points = new[] {bottomLeft, bottomRight};

        EdgeCollider2D edgeColliderTop = gameObject.AddComponent<EdgeCollider2D>();
        edgeColliderTop.points = new[] {topLeft, topRight};

        EdgeCollider2D edgeColliderLeft = gameObject.AddComponent<EdgeCollider2D>();
        edgeColliderLeft.points = new[] {topLeft, bottomLeft};

        EdgeCollider2D edgeColliderRight = gameObject.AddComponent<EdgeCollider2D>();
        edgeColliderRight.points = new[] {topRight, bottomRight};
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
