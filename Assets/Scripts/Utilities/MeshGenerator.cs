using UnityEngine;

namespace Utilities
{
    public static class MeshGenerator
    {
        private const int DefaultConeSegments = 20;

        public static Mesh CreateCone(float range, float angleInDegrees, int segments = DefaultConeSegments)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[segments + 2];
            int[] triangles = new int[segments * 6];

            vertices[0] = Vector3.zero;

            float radius = Mathf.Tan(angleInDegrees * Mathf.Deg2Rad) * range;

            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = (i / (float)segments) * Mathf.PI * 2f;
                float x = Mathf.Cos(currentAngle) * radius;
                float y = Mathf.Sin(currentAngle) * radius;
                vertices[i + 1] = new Vector3(x, y, range);
            }

            for (int i = 0; i < segments; i++)
            {
                int triIndex = i * 6;
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = i + 1;
                triangles[triIndex + 2] = i + 2;

                triangles[triIndex + 3] = 0;
                triangles[triIndex + 4] = i + 2;
                triangles[triIndex + 5] = i + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
