using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    public GameObject tailSegmentPrefab;
    public int maxSegments = 20;

    private Queue<GameObject> tailSegments = new Queue<GameObject>();
    

    // Start is called before the first frame update
    void Start()
    {
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
        Mesh mesh = new Mesh(); //mf.mesh?
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mf.mesh = mesh;

        // create the collider
        PolygonCollider2D pc2d = seg.GetComponent<PolygonCollider2D>();
        List<Vector2> colPoints = new List<Vector2>();
        for (int i = 0; i < points.Count; i++) {
            colPoints.Add(points[i]);
        }
        pc2d.points = colPoints.ToArray();

        tailSegments.Enqueue(seg);
    }
}
