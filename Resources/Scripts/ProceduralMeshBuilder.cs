using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class ProceduralMeshBuilder
{
    private List<Vector3> vertices = new List<Vector3>(); //list of vertices- stores our points in the mesh

    private List<int> indices = new List<int>(); //list of indices that point to the index location in our vertices list

    private List<Vector3> normals = new List<Vector3>(); //this defines the direction of each vertex

    private List<Vector2> uvs = new List<Vector2>(); //store the co-ordinates of our uvs

    public List<int>[] submeshIndices = new List<int>[] { }; //an array of submesh indices 

    //Abstract method for programmatically adding materials, will be overriden in each shape builder
    public abstract void AddMaterials(MeshRenderer renderer);


    //class to create submesh list
    public void SetUpSubmeshes(int submeshCount)
    {
        submeshIndices = new List<int>[submeshCount];

        for (int i = 0; i < submeshCount; i++)
        {
            submeshIndices[i] = new List<int>();
        }
    }

    public void BuildMeshTriangle(Vector3 p0, Vector3 p1, Vector3 p2, int subMesh)
    {
        // a normal is an object such as a line, ray, or vector that is perpendicular to a given object
        Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0).normalized;

        //4. index of each vertex within the list of indices
        int p0Index = vertices.Count;  //eg:0
        int p1Index = vertices.Count + 1;  //eg:1
        int p2Index = vertices.Count + 2; //eg:2

        //5. add the index of each vertrex to the indices list
        indices.Add(p0Index);
        indices.Add(p1Index);
        indices.Add(p2Index);

        submeshIndices[subMesh].Add(p0Index);
        submeshIndices[subMesh].Add(p1Index);
        submeshIndices[subMesh].Add(p2Index);

        //1.add each point to our vertices list
        vertices.Add(p0);
        vertices.Add(p1);
        vertices.Add(p2);

        //2. add normals to our normals list
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        //3. Add each UV co-ordinate to our UV list
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
    }

    public void VerifySubmeshes(Mesh m)
    {
        for (int i = 0; i < submeshIndices.Length; i++)
        {
            if (submeshIndices[i].Count < 3)
            {
                m.SetTriangles(new int[3] { 0, 0, 0 }, i);
            }

            else
            {
                m.SetTriangles(submeshIndices[i].ToArray(), i);
            }
        }
    }

    public Mesh CreateMesh() //build our mesh
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();

        mesh.triangles = indices.ToArray();

        mesh.normals = normals.ToArray();

        mesh.uv = uvs.ToArray();

        mesh.subMeshCount = submeshIndices.Length;

        VerifySubmeshes(mesh);

        return mesh;
    }
}
   


