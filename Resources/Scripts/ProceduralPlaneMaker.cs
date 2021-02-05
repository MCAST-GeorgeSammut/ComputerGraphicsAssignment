using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBuilder : ProceduralMeshBuilder
{
    public override void AddMaterials(MeshRenderer renderer)
    {
        List<Material> materialsList = new List<Material>();

        //Material redMat = new Material(Shader.Find("Specular"));
        //redMat.color = Color.red;

        //Material greenMat = new Material(Shader.Find("Specular"));
        //greenMat.color = Color.green;

        //Material blueMat = new Material(Shader.Find("Specular"));
        //blueMat.color = Color.blue;

        //Material yellowMat = new Material(Shader.Find("Specular"));
        //yellowMat.color = Color.yellow;

        //Material magentaMat = new Material(Shader.Find("Specular"));
        //magentaMat.color = Color.magenta;

        Material whiteMat = new Material(Shader.Find("Specular"));
        whiteMat.color = Color.white;

        //materialsList.Add(redMat);
        //materialsList.Add(greenMat);
        //materialsList.Add(blueMat);
        //materialsList.Add(yellowMat);
        //materialsList.Add(magentaMat);
        materialsList.Add(whiteMat);

        renderer.materials = materialsList.ToArray();
    }
}


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ProceduralPlaneMaker : MonoBehaviour
{

    [SerializeField]
    private float cellSize = 2f;

    [SerializeField]
    private int width = 100;

    [SerializeField]
    private int height = 100;

    [SerializeField]
    public int subMeshSize = 6;

    // Update is called once per frame
    void Update()
    {
        PlaneBuilder planeBuilder = new PlaneBuilder();
        planeBuilder.SetUpSubmeshes(subMeshSize);

        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshRenderer meshrenderer = this.GetComponent<MeshRenderer>();
        MeshCollider meshcollider = this.GetComponent<MeshCollider>();

        //create points of plane
        Vector3[,] points = new Vector3[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                points[x, y] = new Vector3(cellSize * x, cellSize * y);
            }
        }


        //create the quads

        int submesh = 0;

        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                Vector3 bRight = points[x, y];
                Vector3 bLeft = points[x + 1, y];
                Vector3 tRight = points[x, y + 1];
                Vector3 tLeft = points[x + 1, y + 1];

                //create 2 triangles that make up a quad
                planeBuilder.BuildMeshTriangle(bLeft, tRight, tLeft, submesh % subMeshSize);
                planeBuilder.BuildMeshTriangle(bLeft, bRight, tRight, submesh % subMeshSize);

            }

            submesh++;
        }

        meshFilter.mesh = planeBuilder.CreateMesh();
        meshcollider.sharedMesh = meshFilter.mesh;
        planeBuilder.AddMaterials(meshrenderer);

    }
}
