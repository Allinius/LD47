using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // movement
    public float movementSpeed = 1.0f;
    public float rotationSpeed = 90.0f;
    
    //tail
    public GameObject tail;
    public float interval = 0.5f;
    public Transform leftPoint;
    public Transform rightPoint;
    
    private GameManager gameManager;
    // tail
    private Rigidbody2D rb2d;
    private TailManager tailManager;
    private float lastUpdate = 0;
    private Vector2 lastLeftPoint;
    private Vector2 lastRightPoint;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        tailManager = tail.GetComponent<TailManager>();
        lastLeftPoint = leftPoint.position;
        lastRightPoint = rightPoint.position;
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
         
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.back * Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
        transform.Translate(Vector2.up * movementSpeed * Time.deltaTime);
        // rb2d.MovePosition((Vector2)transform.position + (Vector2)transform.up * movementSpeed * Time.deltaTime);
        UpdateTail();       
    }

    void FixedUpdate() {
    }

    void OnTriggerEnter2D(Collider2D collider) {
        // Destroy(gameObject);
        Debug.Log("Ending Game");
        gameManager.EndGame();
        // if (collider.gameObject.layer == LayerMask.NameToLayer("Bounds")) {
        //     // player hit a wall
            
        // }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        // Debug.Log(collision);
    }

    private void UpdateTail() {
        if (Time.time - lastUpdate > interval) { 
            lastUpdate = Time.time;
            List<Vector3> pointAnchors = new List<Vector3>();
            pointAnchors.Add(lastRightPoint);
            pointAnchors.Add(lastLeftPoint);
            pointAnchors.Add(leftPoint.position);
            pointAnchors.Add(rightPoint.position);
            tailManager.addSegment(pointAnchors);

            lastRightPoint = rightPoint.position;
            lastLeftPoint = leftPoint.position;
        }
    }    
}
