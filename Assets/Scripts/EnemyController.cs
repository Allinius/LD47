using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2.0f;
    public float lookDistance = 2.0f;
    public bool drawGizmos = false;

    private Rigidbody2D rb2d;
    private int tailLayerMask;
    private int boundsLayerMask;
    private GameObject player;
    private GameManager gameManager;

   
    private Vector2 direction;
    public bool spawnInvulnerability = true;

    private Vector2 gizmoDirection;
    // private List<Vector2> failedLookGizmos = new List<Vector2>();


    private Vector2[] lookDirections = new Vector2[] {
        new Vector2(1.0f, 0.0f), //right
        new Vector2(2.0f, 1.0f).normalized,
        new Vector2(1.0f, 1.0f).normalized,
        new Vector2(1.0f, 2.0f).normalized,
        new Vector2(0.0f, 1.0f), // up
        new Vector2(-1.0f, 2.0f).normalized,
        new Vector2(-1.0f, 1.0f).normalized,
        new Vector2(-2.0f, 1.0f).normalized,
        new Vector2(-1.0f, 0.0f), // left
        new Vector2(-2.0f, -1.0f).normalized,
        new Vector2(-1.0f, -1.0f).normalized,
        new Vector2(-1.0f, -2.0f).normalized,
        new Vector2(0.0f, -1.0f), // down
        new Vector2(1.0f, -2.0f).normalized,
        new Vector2(1.0f, -1.0f).normalized,
        new Vector2(2.0f, -1.0f).normalized
    };
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        tailLayerMask = (1 << LayerMask.NameToLayer("Tail"));
        boundsLayerMask =  (1 << LayerMask.NameToLayer("Bounds"));

        player = GameObject.FindWithTag("Player");
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate() {
        // change target to player if target doesn't exist;
        if (target == null && player != null) {
            target = player.transform;
        }

        if (spawnInvulnerability) {
            direction = (target.position - transform.position).normalized;
        }
        // check if tail blocks the path to target
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, Mathf.Infinity, tailLayerMask);
        if (hit.collider != null) {
            EmergencyMove();
        } else {
            spawnInvulnerability = false;
            direction = (target.position - transform.position).normalized;
        }

        // move the rigidbody
        rb2d.velocity = direction * moveSpeed;

        gizmoDirection = direction;
    }

    void EmergencyMove() {
        List<Vector2> viableDirections = new List<Vector2>();
        List<float> hitDistances = new List<float>();
        for (int i = 0; i < lookDirections.Length; i++) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirections[i], lookDistance, tailLayerMask | boundsLayerMask);
            if (hit.collider == null) {
                viableDirections.Add(lookDirections[i]);
                hitDistances.Add((hit.point - (Vector2)transform.position).magnitude);
            }
        }
        if (viableDirections.Count > 0) {
            spawnInvulnerability = false;
            float minAngle = Mathf.Infinity;
            int minIndex = -1;
            Vector3 toTargetDirection = target.position - transform.position;
            for (int i = 0; i < viableDirections.Count; i++) {
                float angle = Mathf.Abs(Vector3.Angle(toTargetDirection, viableDirections[i]));
                float angleRelative = angle / 180.0f;
                float distanceRelative = hitDistances[i] / (3.0f * lookDistance);
                if (angle < minAngle) {
                    minAngle = angle;
                    minIndex = i;
                }
            }
            direction = viableDirections[minIndex];
        } else if (!spawnInvulnerability) {
            KillSelf();
        }
    }

    void KillSelf() {
        gameManager.EnemyKilled();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if ((collider.gameObject.transform.position - target.position).magnitude < 0.01f) {
            Debug.Log("entered target");
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos() {
        // if(drawGizmos) {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawRay(transform.position, gizmoDirection);
        //     Gizmos.color = Color.red;
        //     Debug.Log(failedLookGizmos.Count);
        //     for (int i = 0; i < failedLookGizmos.Count; i++) {
        //         Gizmos.DrawRay(transform.position, failedLookGizmos[i]);
        //     }
        // }
    }
}
