using UnityEngine;

public class Walls : MonoBehaviour
{
    // Start is called before the first frame update
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

        Vector2[] edgePoints = new[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft};

        // Create EdgeCollider2D on this GameObject
        EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = edgePoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
