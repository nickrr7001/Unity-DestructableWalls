using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DestructableWall : MonoBehaviour
{
    private List<Vector3> verts;
    HashSet<int> removedVerts = new HashSet<int>();
    private void Start()
    {
        MeshFilter mr = GetComponent<MeshFilter>();
        Mesh m = mr.mesh;
        verts = m.vertices.ToList();
        var tris = m.triangles.ToList();
        Mesh newMesh = new Mesh();
        newMesh.Clear();
        newMesh.vertices = verts.ToArray();
        newMesh.triangles = tris.ToArray();
        newMesh.uv = m.uv;
        newMesh.normals = m.normals;
        mr.mesh = newMesh;
       mr.mesh.SetIndices(mr.mesh.GetIndices(0).Concat(mr.mesh.GetIndices(0).Reverse()).ToArray(), MeshTopology.Triangles, 0);
        GetComponent<MeshCollider>().sharedMesh = mr.mesh;
    }
    public void breakWall(Vector3 hitPoint)
    {
        if (GetComponent<Lock>() != null)
        {
            Destroy(GetComponent<Lock>());
        }
        MeshFilter mr = GetComponent<MeshFilter>();
        Mesh m = mr.mesh;
        verts = m.vertices.ToList();
        var tris = m.triangles.ToList();
        
        for (int i = 0; i < verts.Count; i++)
        {
            if ((hitPoint - transform.TransformPoint(verts[i])).magnitude <= 0.075f)
            {
                removedVerts.Add(i);
            }
        }
        int[] vertUsage = new int[verts.Count];

        for (int j = 0; j < tris.Count; j += 3)
        {
            if (removedVerts.Contains(tris[j])|| removedVerts.Contains(tris[j+1])|| removedVerts.Contains(tris[j+2]))
            {
                tris.RemoveAt(j);
                tris.RemoveAt(j);
                tris.RemoveAt(j);
                j -= 3;
            }
            else
            {
                vertUsage[tris[j]]++;
                vertUsage[tris[j+ 1] ]++;
                vertUsage[tris[j+ 2] ]++;
            }
        }
        #region removeDisconnectedParts
        for (int j = 0; j < tris.Count; j += 3)
        {
            if (vertUsage[tris[j]] <= 2 || vertUsage[tris[j+ 1] ] <= 2 || vertUsage[tris[j+ 2] ] <= 2)
            {
                tris.RemoveAt(j);
                tris.RemoveAt(j);
                tris.RemoveAt(j);
                j -= 3;
            }
        }
        #endregion
        Mesh newMesh = new Mesh();
        newMesh.Clear();
        newMesh.vertices = verts.ToArray();
        newMesh.triangles = tris.ToArray();
        newMesh.uv = m.uv;
        newMesh.normals = m.normals;
        mr.mesh = newMesh;
        GetComponent<MeshCollider>().sharedMesh = newMesh;        
    }
}
