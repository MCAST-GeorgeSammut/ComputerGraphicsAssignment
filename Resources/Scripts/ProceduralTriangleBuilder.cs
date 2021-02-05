using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TriangleBuilder : ProceduralMeshBuilder
{
    public override void AddMaterials(MeshRenderer renderer)
    {
        List<Material> materialList = new List<Material>();

        Material greenMaterial = new Material(Shader.Find("Specular"));
        greenMaterial.color = Color.green;

        materialList.Add(greenMaterial);

        renderer.materials = materialList.ToArray();
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProceduralTriangleBuilder: MonoBehaviour
{
    [SerializeField] public Vector3 triangleSize = Vector3.one;


    public void Update()
    {
        BuildTriangle();
    }

    public void BuildTriangle()
    {
        TriangleBuilder TriangleBuilder = new TriangleBuilder();
        TriangleBuilder.SetUpSubmeshes(1);

        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshRenderer meshrenderer = this.GetComponent<MeshRenderer>();


        Vector3 p0 = new Vector3(triangleSize.x, triangleSize.y, -triangleSize.z);
        Vector3 p1 = new Vector3(-triangleSize.x, triangleSize.y, -triangleSize.z);
        Vector3 p2 = new Vector3(-triangleSize.x, triangleSize.y, triangleSize.z);

        TriangleBuilder.BuildMeshTriangle(p0, p1, p2, 0);
        meshFilter.mesh = TriangleBuilder.CreateMesh();
        
        TriangleBuilder.AddMaterials(meshrenderer);
    }

}

