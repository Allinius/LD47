using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemySpawner spawner;
    public Transform target;
    public Color color;
    public float moveSpeed = 2.0f;
    public float lookDistance = 2.0f;
    public SpriteRenderer spriteRenderer;
    public Sprite baseSprite;
    public Sprite worriedSprite;
    public Sprite angrySprite;
    public bool drawGizmos = false;

    private Rigidbody2D rb2d;
    private int tailLayerMask;
    private int boundsLayerMask;
    private GameObject player;
    private GameManager gameManager;

   
    private Vector2 direction;
    private bool spawnInvulnerability = true;
    private bool angry = false;

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

        spriteRenderer.color = color;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate() {
        // change target to player if target doesn't exist;
        if (target == null && player != null) {
            target = player.transform;
            angry = true;
            spriteRenderer.sprite = angrySprite;
            spriteRenderer.color = Color.white;
        }

        if (spawnInvulnerability) {
            direction = (target.position - transform.position).normalized;
        }
        // check if tail blocks the path to target
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, Mathf.Infinity, tailLayerMask);
        if (hit.collider != null) {
            EmergencyMove();
        } else {
            if (!angry) {
                spriteRenderer.sprite = baseSprite;
            }
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
            if (!angry && viableDirections.Count <= 4) {
                spriteRenderer.sprite = worriedSprite;
            } else if (!angry) {
                spriteRenderer.sprite = baseSprite;
            }
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
            iTween.ScaleTo(gameObject, iTween.Hash(
                "scale", new Vector3(2f,2f,1f),
                "time", 0.5f,
                "easetype", iTween.EaseType.easeInExpo,
                "oncomplete", "KillSelf"
            ));
        }
    }

    void KillSelf() {
        gameManager.EnemyKilled(angry);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if ((collider.gameObject.transform.position - target.position).magnitude < 0.01f) {
            // Debug.Log("entered target");
            spawner.StartTargetDeath(1f);
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
