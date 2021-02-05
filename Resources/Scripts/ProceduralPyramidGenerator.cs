using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyramidBuilder : ProceduralMeshBuilder
{
    public override void AddMaterials(MeshRenderer renderer)
    {
        List<Material> materialList = new List<Material>();

        Material blueMat = new Material(Shader.Find("Specular"));
        blueMat.color = Color.blue;

        Material redMat = new Material(Shader.Find("Specular"));
        redMat.color = Color.red;

        Material greenMat = new Material(Shader.Find("Specular"));
        greenMat.color = Color.green;

        Material yellowMat = new Material(Shader.Find("Specular"));
        yellowMat.color = Color.yellow;

        materialList.Add(yellowMat);
        materialList.Add(greenMat);
        materialList.Add(blueMat);
        materialList.Add(redMat);

        renderer.materials = materialList.ToArray();
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProceduralPyramidGenerator : MonoBehaviour
{
    [SerializeField] private float pyramidSize = 5f;

    // Update is called once per frame
    void Update()
    {
        PyramidBuilder pyramidBuilder = new PyramidBuilder();
        pyramidBuilder.SetUpSubmeshes(4);

        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshRenderer meshrenderer = this.GetComponent<MeshRenderer>();

        //Add points
        Vector3 topPoint = new Vector3(0, pyramidSize, 0);

        Vector3 base0 = Quaternion.AngleAxis(0f, Vector3.up) * Vector3.forward * pyramidSize;

        Vector3 base1 = Quaternion.AngleAxis(240f, Vector3.up) * Vector3.forward * pyramidSize;

        Vector3 base2 = Quaternion.AngleAxis(120f, Vector3.up) * Vector3.forward * pyramidSize;

        //Build Triangles for our pyramid
        pyramidBuilder.BuildMeshTriangle(base0, base1, base2, 0);
        pyramidBuilder.BuildMeshTriangle(base1, base0, topPoint, 1);
        pyramidBuilder.BuildMeshTriangle(base2, topPoint, base0, 2);
        pyramidBuilder.BuildMeshTriangle(topPoint, base2, base1, 3);


        meshFilter.mesh = pyramidBuilder.CreateMesh();

        pyramidBuilder.AddMaterials(meshrenderer);
    }
}
