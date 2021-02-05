using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : ProceduralMeshBuilder
{
    public override void AddMaterials(MeshRenderer renderer)
    {
        List<Material> materialsList = new List<Material>();

        Material whiteMat = new Material(Shader.Find("Specular"));
        whiteMat.color = Color.white;

        Material blackMat = new Material(Shader.Find("Specular"));
        blackMat.color = Color.black;

        Material redMat = new Material(Shader.Find("Specular"));
        redMat.color = Color.red;

        materialsList.Add(whiteMat);
        materialsList.Add(blackMat);
        materialsList.Add(redMat);
        materialsList.Add(whiteMat);

        renderer.materials = materialsList.ToArray();
    }
}



[System.Serializable]
public class PathShape
{
    public Vector3[] shape = new Vector3[] { -Vector3.up, Vector3.up, -Vector3.up };
}


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ProceduralPathMaker : MonoBehaviour
{

    [SerializeField] private int subMeshSize = 4;

    [SerializeField] private Transform[] Path;

    [SerializeField] PathShape pathShape;

    // Update is called once per frame
    void Update()
    {

        PathBuilder pathBuilder = new PathBuilder();
        pathBuilder.SetUpSubmeshes(subMeshSize);

        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshRenderer meshrenderer = this.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = this.GetComponent<MeshCollider>();

        Vector3[] prevShape = TranslateShape(
            Path[Path.Length - 1].transform.position,
            (Path[0].transform.position - Path[Path.Length - 1].transform.position).normalized, pathShape
            );

        int subMeshIndex = 0;

        for (int i = 0; i < Path.Length; i++)
        {
            Vector3[] currentShape = TranslateShape(
           Path[i].transform.position,
           (Path[(i + 1) % Path.Length].transform.position - Path[i].transform.position).normalized, pathShape
           );

            for (int j = 0; j < currentShape.Length - 1; j++)
            {
                pathBuilder.BuildMeshTriangle(prevShape[j], currentShape[j], currentShape[j + 1], subMeshIndex % subMeshSize);
                pathBuilder.BuildMeshTriangle(prevShape[j + 1], prevShape[j], currentShape[j + 1], subMeshIndex % subMeshSize);
            }

            subMeshIndex++;

            prevShape = currentShape;
        }


        meshFilter.mesh = pathBuilder.CreateMesh();
        meshCollider.sharedMesh = meshFilter.mesh;
        pathBuilder.AddMaterials(meshrenderer);
    }

    private Vector3[] TranslateShape(Vector3 point, Vector3 forward, PathShape shape)
    {
        Vector3[] translatedShape = new Vector3[shape.shape.Length];

        Quaternion forwardRotation = Quaternion.LookRotation(forward);


        for (int i = 0; i < shape.shape.Length; i++)
        {
            translatedShape[i] = (forwardRotation * shape.shape[i]) + point;

        }

        return translatedShape;
    }
}
