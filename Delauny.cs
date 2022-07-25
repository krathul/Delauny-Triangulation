using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delauny
{
    public class Edge
    {
        public Vector2 a;
        public Vector2 b;

        public Edge(Vector2 A, Vector2 B)
        {
            a = A;
            b = B;
        }

        public bool Equals(Edge e)
        {
            if (a == e.a && b == e.b) return true;
            return false;
        }
    }

    public class Triangle
    {
        public Vector2[] verts;

        public Edge[] edges;

        public Triangle(Vector2 A, Vector2 B, Vector2 C)
        {
            verts = new Vector2[] { A, B, C };
            edges = new Edge[] { new Edge(A,B), new Edge(B,C), new Edge(A,C) };
        }

        public bool Circum(Vector2 x)
        {
            float ad = verts[0].x * verts[0].x + verts[0].y * verts[0].y;
            float bd = verts[1].x * verts[1].x + verts[1].y * verts[1].y;
            float cd = verts[2].x * verts[2].x + verts[2].y * verts[2].y;
            float D = 2 * (verts[0].x * (verts[1].y - verts[2].y) +
                           verts[1].x * (verts[2].y - verts[0].y) +
                           verts[2].x * (verts[0].y - verts[1].y));

            Vector2 Cc = new Vector2(
                                     (ad * (verts[1].y - verts[2].y) + bd * (verts[2].y - verts[0].y) + cd * (verts[0].y - verts[1].y))/D,
                                     (ad * (verts[2].x - verts[1].x) + bd * (verts[0].x - verts[2].x) + cd * (verts[1].x - verts[0].x))/D
                                    );

            float Cr = Vector2.Distance(Cc, verts[0]);
            float Cx = Vector2.Distance(Cc, x);
            //Debug.Log("Circum centre: " + Cc + " radius: " + Cr + " dist: " + Cx);
            if (Cx < Cr) return true;
            return false;
        }
    }

    public class BowyerWatson
    {
        public List<Vector2> points;
        private Triangle SuperTri;

        public BowyerWatson(List<Vector2> P)
        {
            points = P;
            SuperTri = new Triangle(new Vector2(-100, -100), new Vector2(0, 100), new Vector2(100, 0));
        }

        public List<Triangle> Triangulate()
        {
            List<Triangle> TriMesh = new List<Triangle>();
            TriMesh.Add(SuperTri);

            for (int i = 0; i < points.Count; i++)
            {
                //Debug.Log("Point : " + points[i]);
                List<Triangle> badTri = new List<Triangle>();

                foreach (Triangle tri in TriMesh)
                {
                    if (tri.Circum(points[i]))
                    {
                        badTri.Add(tri);
                    }
                }
                //Debug.Log("n badtri: " + badTri.Count);
                List<Edge> Polygon = new List<Edge>();

                foreach (Triangle tri in badTri)
                {
                    foreach (Edge edge in tri.edges)
                    {
                        bool shared = false;
                        foreach (Triangle otherTri in badTri)
                        {
                            if (tri.Equals(otherTri)) continue;
                            foreach (Edge edge1 in otherTri.edges)
                            {
                                if (edge.Equals(edge1))
                                {
                                    shared = true;
                                }
                                //Debug.Log("Edge comparison: " + edge.a + edge.b +  "  " + edge1.a + edge1.b + "  " + shared);
                            }
                        }
       
                        if (!shared) Polygon.Add(edge);
                    }
                }

                //Debug.Log("Side count: " + Polygon.Count);

                foreach (Triangle tri in badTri)
                {
                    TriMesh.Remove(tri);
                }

                //Debug.Log("Creating New triangles");
                foreach (Edge edge in Polygon)
                {
                    //Debug.Log(edge.a + " " + edge.b);
                    TriMesh.Add(new Triangle(points[i], edge.a, edge.b));
                }
                //Debug.Log(TriMesh.Count);
            }


            List<Triangle> bTri = new List<Triangle>();
            foreach (Triangle tri in TriMesh)
            {
                foreach (Vector2 vert in tri.verts)
                {
                    if (vert == SuperTri.verts[0] || vert == SuperTri.verts[1] || vert == SuperTri.verts[2])
                    {
                        bTri.Add(tri);
                        break;
                    }
                }
            }

            foreach (Triangle tri in bTri)
            {
                TriMesh.Remove(tri);
            }

            return TriMesh;

        }

        public Vector3[,] GenMesh()
        {
            List<Triangle> TriMesh = Triangulate();
            //Debug.Log(TriMesh.Count);
            Vector3[,] Tripoints = new Vector3[TriMesh.Count,3];
            for (int i = 0; i < TriMesh.Count; i++)
            {
                Tripoints[i, 0] = new Vector3(TriMesh[i].verts[0].x, TriMesh[i].verts[0].y, 0);
                Tripoints[i, 1] = new Vector3(TriMesh[i].verts[1].x, TriMesh[i].verts[1].y, 0);
                Tripoints[i, 2] = new Vector3(TriMesh[i].verts[2].x, TriMesh[i].verts[2].y, 0);
            }

            return Tripoints;
        }
    }
}

