using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    public GameObject tailSegmentPrefab;
    public int maxSegments = 20;
    public float tailSpectrumLength = 60f;

    private GameManager gameManager;

    private Queue<GameObject> tailSegments = new Queue<GameObject>();

    private float previousUCoord = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

        for (int i = 0; i < maxSegments; i++) {
            GameObject seg = Instantiate(tailSegmentPrefab, transform);
            seg.SetActive(false);
            tailSegments.Enqueue(seg);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addSegment(List<Vector3> points) {
        GameObject seg = tailSegments.Dequeue();
        if (!seg.activeSelf) {
            seg.SetActive(true);
        }
        // create the mesh
        MeshFilter mf = seg.GetComponent<MeshFilter>();
        Vector3[] vertices = points.ToArray();
        int[] triangles = new int[] {
            0,1,2,
            2,3,0
        };
        float newUCoord = (gameManager.GameTime() % tailSpectrumLength) / tailSpectrumLength;
        if (newUCoord - previousUCoord < 0) {
            previousUCoord = 0f;
        }
        Vector2[] uvs = new Vector2[] {
            new Vector2(previousUCoord, 0f),
            new Vector2(previousUCoord, 0f),
            new Vector2(newUCoord, 0f),
            new Vector2(newUCoord, 0f)
        };
        previousUCoord = newUCoord;
        Mesh mesh = new Mesh(); //mf.mesh?
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mf.mesh = mesh;

        // create the collider
        PolygonCollider2D pc2d = seg.GetComponent<PolygonCollider2D>();
        List<Vector2> colPoints = new List<Vector2>();
        for (int i = 0; i < points.Count; i++) {
            colPoints.Add(points[i]);
        }
        pc2d.points = colPoints.ToArray();

        // update z value of segment
        // Vector3 pos = seg.transform.position;
        // pos.z = -gameManager.GameTime() / 10000f;
        // seg.transform.position = pos;

        tailSegments.Enqueue(seg);
    }
}
