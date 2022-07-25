using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenTriangles
{
    public class Meshing
    {
        public Vector3[,] Seed;
        private Dictionary<Vector3, int> dict;
        private List<Vector3> Vertices;
        private List<int> Indices;

        public Meshing(Vector3[,] sd)
        {
            Seed = sd;
            Vertices = new List<Vector3>();
            Indices = new List<int>();
            dict = new Dictionary<Vector3, int>();
            GenMesh();
        }

        public void GenMesh()
        {
            int ind = 0;
            for (int i = 0; i < Seed.GetLength(0); i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!dict.ContainsKey(Seed[i, j]))
                    {
                        dict[Seed[i, j]] = ind;
                        Vertices.Add(Seed[i, j]);
                        ind++;
                    }

                    Indices.Add(dict[Seed[i, j]]);
                }

                if (!CheckFrontFace(Vertices[Indices[3 * i]], Vertices[Indices[3 * i + 1]], Vertices[Indices[3 * i + 2]]))
                {
                    (Indices[3 * i + 2], Indices[3 * i + 1]) = (Indices[3 * i + 1], Indices[3 * i + 2]);
                }
            }
        }

        public bool CheckFrontFace(Vector3 A, Vector3 B, Vector3 C)
        {
            Vector3 ba = B - A;
            Vector3 ca = C - A;
            Vector3 n = Vector3.Cross(ba, ca);

            return Vector3.Dot(n, Vector3.back) > 0;
        }

        public Vector3[] GetVertices()
        {
            return Vertices.ToArray();
        }

        public int[] GetIndices()
        {
            return Indices.ToArray();
        }

    }
}