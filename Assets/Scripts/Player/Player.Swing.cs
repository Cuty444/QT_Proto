using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {
        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }
        
        private Mesh SwingAreaCreateMesh(float radius, float angle, int segments)
        {
            Mesh mesh = new Mesh();
            int vertexCount = segments + 2;
            int indexCount = segments * 3;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] indices = new int[indexCount];
            float angleRad = angle * Mathf.Deg2Rad;
            float angleStep = angleRad / segments;
            float currentAngle = -angleRad / 2f;
            vertices[0] = Vector3.zero;
            for (int i = 0; i <= segments; i++)
            {
                vertices[i + 1] = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f) * radius;
                currentAngle += angleStep;
            }
            for (int i = 0; i < segments; i++)
            {
                indices[i * 3] = 0;
                indices[i * 3 + 1] = i + 1;
                indices[i * 3 + 2] = i + 2;
            }
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}