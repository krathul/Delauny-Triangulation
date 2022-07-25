using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delauny;
using GenTriangles;

public class SimGen : MonoBehaviour
{
    public GameObject prefab;
    public int N;
    private BowyerWatson Alg;
    private Meshing make;
    private Mesh mesh;

    private List<GameObject> nodeList;

    private List<Vector2> InitPos;
    private List<Vector2> Pos;
    private List<Vector2> Vel;

    public float fieldR;
    public float speed;

    private void Awake()
    {
        mesh = new Mesh();
        nodeList = new List<GameObject>();
        GetComponent<MeshFilter>().mesh = mesh;

        InitPos = new List<Vector2>();
        Pos = new List<Vector2>();
        Vel = new List<Vector2>();

        for (int i = 0; i < N; i++)
        {
            InitPos.Add(new Vector2(Random.value * 24.5f - 13.4f, Random.value * 15.3f - 7.3f));
            Pos.Add(new Vector2(InitPos[i].x, InitPos[i].y));
            Vel.Add(new Vector2(Random.value, Random.value).normalized);
        }
    }

    public Vector2 HitCheck(Vector2 O, Vector2 p, Vector2 v)
    {
        if (Vector2.Distance(p, O) > fieldR)
        {
            v = (-v + new Vector2(-v.y*Random.value, v.x*Random.value)).normalized;
        }
        //Debug.Log("Distance: " + Vector2.Distance(p,O) + " : " + v);
        return v;
    }

    public Color32[] GenColors(int n_Vertices)
    {
        Color32[] Sample = new Color32[] {
            new Color(220f/255f,20f/255f,60f/255f), //Crimson Red
            new Color(255f/255f,0f/255f,0f/255f), //Red
            new Color(240f/255f,128f/255f,128f/255f),
            new Color(255f/255f,140f/255f,0f/255f),
            new Color(205f/255f,92f/255f,92f/255f),
            new Color(255f/255f,127f/255f,80f/255f),
            new Color(0f,0f,0f)
        };
        Color32[] color = new Color32[n_Vertices];
        for (int i = 0; i < n_Vertices; i++)
        {
            color[i] = Sample[(int)Mathf.Floor(Random.value*(Sample.GetLength(0)-1))];
        }
        return color;
    }

    void FixedUpdate()
    {
        //Random.InitState(242);
        for (int i=0;i<N;i++)
        {
            Vel[i] = HitCheck(InitPos[i], Pos[i], Vel[i]);
            Pos[i] += speed*Vel[i]*Time.deltaTime;
            //Debug.Log(Pos[i] + " " + speed * HitCheck(InitPos[i], Pos[i], Vel[i]) * Time.deltaTime);
        }

        //Create and Display mesh
        Alg = new BowyerWatson(Pos);
        Vector3[,] Gen = Alg.GenMesh();
        make = new Meshing(Gen);

        Vector3[] Vertices = make.GetVertices();
        int[] Triangles = make.GetIndices();

        mesh.Clear();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.colors32 = GenColors(Vertices.GetLength(0));

        //Destroy existing nodes
        foreach (GameObject obj in nodeList)
        {
            Destroy(obj);
        }

        //Create nodes
        nodeList = new List<GameObject>();

        for (var i = 0; i < N; i++)
        {
            nodeList.Add(Instantiate(prefab, new Vector3(Pos[i].x, Pos[i].y, 0), Quaternion.identity));
        }

       

    }
}
