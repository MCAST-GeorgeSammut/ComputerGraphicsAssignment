using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class PyramidMaker : ProceduralMeshBuilder
{


    public override void AddMaterials(MeshRenderer renderer)
    {
        List<Material> materialList = new List<Material>();

        Material greenMaterial = new Material(Shader.Find("Specular"));
        greenMaterial.color = Color.green;

        Material blueMaterial = new Material(Shader.Find("Specular"));
        blueMaterial.color = Color.blue;

        Material redMaterial = new Material(Shader.Find("Specular"));
        redMaterial.color = Color.red;

        Material yellowMaterial = new Material(Shader.Find("Specular"));
        yellowMaterial.color = Color.yellow;

        Material magentaMaterial = new Material(Shader.Find("Specular"));
        magentaMaterial.color = Color.magenta;

        Material whiteMaterial = new Material(Shader.Find("Specular"));
        whiteMaterial.color = Color.white;

        materialList.Add(greenMaterial);
        materialList.Add(redMaterial);
        materialList.Add(blueMaterial);
        materialList.Add(magentaMaterial);
        materialList.Add(yellowMaterial);
        materialList.Add(whiteMaterial);

        renderer.materials = materialList.ToArray();
    }
}


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProceduralCubeGenerator : MonoBehaviour
{
    [SerializeField] private Vector3 cubeSize = Vector3.one;

    // Update is called once per frame
    void Update()
    {
        BuildCube();
    }

    public void BuildCube()
    {
        PyramidMaker cubeBuilder = new PyramidMaker();
        cubeBuilder.SetUpSubmeshes(6);

        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshRenderer meshrenderer = this.GetComponent<MeshRenderer>();

        //top vertices
        Vector3 t0 = new Vector3(cubeSize.x, cubeSize.y, -cubeSize.z); //top left
        Vector3 t1 = new Vector3(-cubeSize.x, cubeSize.y, -cubeSize.z);//top right
        Vector3 t2 = new Vector3(-cubeSize.x, cubeSize.y, cubeSize.z);//bottom right of top square
        Vector3 t3 = new Vector3(cubeSize.x, cubeSize.y, cubeSize.z);//bottom left of top square

        //bottom vertices
        Vector3 b0 = new Vector3(cubeSize.x, -cubeSize.y, -cubeSize.z); //bottom left
        Vector3 b1 = new Vector3(-cubeSize.x, -cubeSize.y, -cubeSize.z);//bottom right
        Vector3 b2 = new Vector3(-cubeSize.x, -cubeSize.y, cubeSize.z);//bottom right of bottom square
        Vector3 b3 = new Vector3(cubeSize.x, -cubeSize.y, cubeSize.z);//bottom left of bottom square

        //top square
        cubeBuilder.BuildMeshTriangle(t0, t1, t2, 0);
        cubeBuilder.BuildMeshTriangle(t0, t2, t3, 0);

        //bottom square
        cubeBuilder.BuildMeshTriangle(b2, b1, b0, 1);
        cubeBuilder.BuildMeshTriangle(b3, b2, b0, 1);

        //back square
        cubeBuilder.BuildMeshTriangle(b0, t1, t0, 2);
        cubeBuilder.BuildMeshTriangle(b0, b1, t1, 2);

        //left square
        cubeBuilder.BuildMeshTriangle(b1, t2, t1, 3);
        cubeBuilder.BuildMeshTriangle(b1, b2, t2, 3);

        //left square
        cubeBuilder.BuildMeshTriangle(b2, t3, t2, 4);
        cubeBuilder.BuildMeshTriangle(b2, b3, t3, 4);

        //front square
        cubeBuilder.BuildMeshTriangle(b3, t0, t3, 5);
        cubeBuilder.BuildMeshTriangle(b3, b0, t0, 5);

        meshFilter.mesh = cubeBuilder.CreateMesh();

        cubeBuilder.AddMaterials(meshrenderer);
    }
}
