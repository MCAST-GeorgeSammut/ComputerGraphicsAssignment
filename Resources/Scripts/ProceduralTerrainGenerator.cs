using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class TerrainTextureData
{
    public Texture2D terrainTexture;
    public float minHeight;
    public float maxHeight;
    public Vector2 tileSize;
}

[System.Serializable]
class WeatherData
{
    public GameObject WeatherObject;
    public float minHeight;
    public float maxHeight;
    public Vector3 WeatherScale;
}

[System.Serializable]
class TreeData
{
    public GameObject treeMesh;
    public float minHeight;
    public float maxHeight;
}

[ExecuteInEditMode]
public class ProceduralTerrainGenerator : MonoBehaviour
{
    private Terrain terrain;
    private TerrainData terrainData;

    [SerializeField]
    private Texture2D heightMapImage;

    [SerializeField]
    private Vector3 heightMapScale = new Vector3(1, 1, 1);

    [SerializeField]
    private bool LoadHeightMap = true;

    [SerializeField]
    private bool GeneratePerlinNoiseTerrain = false;

    [SerializeField]
    private bool FlattenTerrain = false;

    [SerializeField]
    private bool AddTexture = false;

    [SerializeField]
    private bool RemoveTexture = false;

    [SerializeField]
    private bool AddTrees = false;

    [SerializeField]
    private bool AddRain= false;

    [SerializeField]
    private bool AddFog = false;

    [SerializeField]
    private bool Add_Water = false;

    [SerializeField]
    private float minRandomHeightRanger = 0f;

    [SerializeField]
    private float maxRandomHeightRanger = 0.01f;

    [SerializeField]
    private float perlinNoiseWidthScale = 0.01f;

    [SerializeField]
    private float perlinNoiseHeightScale = 0.01f;

    //variables for adding textures to our terrain
    [SerializeField]
    private List<TerrainTextureData> terrainTextureDataList;

    [SerializeField]
    private float terrainTextureBlendOffset = 0.01f;

    //variables for adding trees
    [SerializeField]
    private List<TreeData> treeDataList;

    [SerializeField]
    private int maxTrees = 1000;

    [SerializeField]
    private int treeSpacing = 10;

    [SerializeField]
    private float randomXRange = 5.0f;

    [SerializeField]
    private float randomZRange = 5.0f;

    [SerializeField]
    private int terrainLayerIndex = 8;

    [SerializeField]
    private WeatherData weatherObject;

    [SerializeField]
    private GameObject water;

    [SerializeField]
    private GameObject dust;

    [SerializeField]
    private GameObject rain;

    private float waterHeight;

    [SerializeField]
    private float minWaterHeight;

    [SerializeField]
    private float maxWaterHeight;

    private List<float> minHeight;

    private List<float> maxHeight;

    [SerializeField]
    List<GameObject> points = new List<GameObject>();

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = Terrain.activeTerrain.terrainData;

        //randomize path points
        foreach (GameObject point in points)
        {
            point.transform.position = new Vector3(Random.Range(0, terrainData.size.x), Random.Range(0, terrainData.size.z), Random.Range(0, terrainData.size.z));
        }

        AddWater();
    }



    private void OnValidate()
    {
        if (FlattenTerrain)
        {
            LoadHeightMap = false;
            GeneratePerlinNoiseTerrain = false;
        }

        if (RemoveTexture)
        {
            AddTexture = false;
        }

        if (LoadHeightMap)
        {
            UpdateHeightmap();
        }

        if (FlattenTerrain || GeneratePerlinNoiseTerrain)
        {
            SmoothenTerrain();
        }

        if (AddTexture || RemoveTexture)
        {
            TerrainTexture();
        }

        if (AddTrees)
        {
            AddTree();
        }

        if (AddRain)
        {
            AddWeatherObject();
            AddFog = false;

        }

        if (AddFog)
        {
            AddWeatherObject();
            AddRain = false;
        }

        if (Add_Water)
        {
            AddWater();
        }
    }

    

    void UpdateHeightmap()
    {
        //Creating a new empty 2D array of float based on the dimensions of heightmap resolution set in settings
        //float[,] heightMap = new float[terrainData.heightmapResolution,terrainData.heightmapResolution];

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        for (int width = 0; width < terrainData.heightmapResolution; width++)
        {
            for (int height = 0; height < terrainData.heightmapResolution; height++)
            {
                if (LoadHeightMap)
                {
                    heightMap[width, height] = heightMapImage.GetPixel((int)(width * heightMapScale.x), (int)(height * heightMapScale.z)).grayscale * heightMapScale.y;
                }

            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    void SmoothenTerrain()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        for (int width = 0; width < terrainData.heightmapResolution; width++)
        {
            for (int height = 0; height < terrainData.heightmapResolution; height++)
            {
                if (GeneratePerlinNoiseTerrain)
                {
                    heightMap[width, height] += Mathf.PerlinNoise(width * perlinNoiseWidthScale, height * perlinNoiseHeightScale);
                }

                if (FlattenTerrain)
                {
                    heightMap[width, height] = 0;
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    void AddTree()
    {
        TreePrototype[] trees = new TreePrototype[treeDataList.Count];

        for (int i = 0; i < treeDataList.Count; i++)
        {
            trees[i] = new TreePrototype();
            trees[i].prefab = treeDataList[i].treeMesh;
        }
        terrainData.treePrototypes = trees;

        List<TreeInstance> treeInstancesList = new List<TreeInstance>();

        if (AddTrees)
        {
            for (int z = 0; z < terrainData.size.z; z += treeSpacing)
            {
                for (int x = 0; x < terrainData.size.x; x += treeSpacing)
                {
                    for (int treePrototypeIndex = 0; treePrototypeIndex < trees.Length; treePrototypeIndex++)
                    {
                        if (treeInstancesList.Count < maxTrees)
                        {
                            float currentHeight = terrainData.GetHeight(x, z) / terrainData.size.y;
                            if (currentHeight >= treeDataList[treePrototypeIndex].minHeight &&
                                currentHeight <= treeDataList[treePrototypeIndex].maxHeight)
                            {
                                float randomX = (x + Random.Range(-randomXRange, randomXRange)) / terrainData.size.x;
                                float randomZ = (z + Random.Range(-randomZRange, randomZRange)) / terrainData.size.z;

                                TreeInstance treeInstance = new TreeInstance();

                                treeInstance.position = new Vector3(randomX, currentHeight, randomZ);

                                Vector3 treePosition = new Vector3(treeInstance.position.x * terrainData.size.x,
                                    treeInstance.position.y * terrainData.size.y,
                                    treeInstance.position.z * terrainData.size.z) + this.transform.position;

                                RaycastHit raycastHit;
                                int layerMask = 1 << terrainLayerIndex;

                                if (Physics.Raycast(treePosition, Vector3.down, out raycastHit, 100, layerMask)
                                    || Physics.Raycast(treePosition, Vector3.up, out raycastHit, 100, layerMask))
                                {
                                    float treeHeight = (raycastHit.point.y - this.transform.position.y) / terrainData.size.y;

                                    treeInstance.position = new Vector3(treeInstance.position.x, treeHeight, treeInstance.position.z);

                                    treeInstance.rotation = Random.Range(0, 360);
                                    treeInstance.prototypeIndex = treePrototypeIndex;
                                    treeInstance.color = Color.white;
                                    treeInstance.lightmapColor = Color.white;
                                    treeInstance.heightScale = 0.95f;
                                    treeInstance.widthScale = 0.95f;

                                    treeInstancesList.Add(treeInstance);
                                }
                            }
                        }
                    }
                }
            }
        }
        terrainData.treeInstances = treeInstancesList.ToArray();
    }

    void AddWeatherObject()
    {
        float currentHeight;
        do
        {
            currentHeight = terrainData.GetHeight((int)Random.Range(0, terrainData.size.x + 1), (int)Random.Range(0, terrainData.size.z + 1)) / terrainData.size.y;
        }

        while (currentHeight < weatherObject.minHeight && currentHeight > weatherObject.maxHeight);

        if (AddRain)
        {
            weatherObject.WeatherObject = rain;
        }

        else if (AddFog)
        {
            weatherObject.WeatherObject = dust;
        }

        GameObject WeatherInstance = Instantiate(weatherObject.WeatherObject, new Vector3(Random.Range(0, terrainData.size.x), currentHeight * terrainData.size.y, Random.Range(0, terrainData.size.z)), Quaternion.identity);
        WeatherInstance.transform.localScale = weatherObject.WeatherScale;

        if (AddFog)
        {
            RaycastHit raycastHit;
            int layerMask = 1 << terrainLayerIndex;

            if (Physics.Raycast(WeatherInstance.transform.position, Vector3.down, out raycastHit, 1000, layerMask)
                                  || Physics.Raycast(WeatherInstance.transform.position, Vector3.up, out raycastHit, 100, layerMask))
            {
                float fogHeight = (raycastHit.point.y - this.transform.position.y) / terrainData.size.y;

                WeatherInstance.transform.position = new Vector3(WeatherInstance.transform.position.x, fogHeight, WeatherInstance.transform.position.z);

            }
        }
        

    }

    void AddWater()
    {
        GameObject waterGameObject = GameObject.Find("Water");
        waterHeight = Random.Range(minWaterHeight, maxWaterHeight+.01f);

        if (!waterGameObject)
        {
            waterGameObject = Instantiate(water, this.transform.position, this.transform.rotation);
            waterGameObject.name = "Water";

            waterGameObject.transform.position = this.transform.position + new Vector3(
                terrainData.size.x / 2,
                waterHeight * terrainData.size.y,
                terrainData.size.z / 2);

            waterGameObject.transform.localScale = new Vector3(terrainData.size.x, 1, terrainData.size.z);
        }

    }


    void TerrainTexture()
    {
        // TerrainLayer[] terrainLayers = new TerrainLayer[1];

        //terrainLayers[0] = new TerrainLayer();
        //terrainLayers[0].diffuseTexture = terrainTexture;

        TerrainLayer[] terrainLayers = new TerrainLayer[terrainTextureDataList.Count];

        for (int i = 0; i < terrainTextureDataList.Count; i++)
        {
            if (AddTexture)
            {
                terrainLayers[i] = new TerrainLayer();
                terrainLayers[i].diffuseTexture = terrainTextureDataList[i].terrainTexture;
                terrainLayers[i].tileSize = terrainTextureDataList[i].tileSize;
            }

            else if (RemoveTexture)
            {
                terrainLayers[i] = new TerrainLayer();
                terrainLayers[i].diffuseTexture = null;
            }
        }

        terrainData.terrainLayers = terrainLayers;


        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        float[,,] alphaMapList = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int height = 0; height < terrainData.alphamapHeight; height++)
        {
            for (int width = 0; width < terrainData.alphamapWidth; width++)
            {
                float[] splatmap = new float[terrainData.alphamapLayers];

                for (int i = 0; i < terrainTextureDataList.Count; i++)
                {
                    float minHeight = terrainTextureDataList[i].minHeight - terrainTextureBlendOffset;
                    float maxHeight = terrainTextureDataList[i].maxHeight + terrainTextureBlendOffset;

                    if (heightMap[width, height] >= minHeight && heightMap[width, height] <= maxHeight)
                    {
                        splatmap[i] = 1;
                    }
                }
                //normalise splatmap
                NormaliseSplatmap(splatmap);

                for (int j = 0; j < terrainTextureDataList.Count; j++)
                {
                    alphaMapList[width, height, j] = splatmap[j];
                }
            }
        }
        //assign the terrain data's alpha map list to the one we created
        terrainData.SetAlphamaps(0, 0, alphaMapList);


        void NormaliseSplatmap(float[] splatmap)
        {
            float total = 0;

            for (int i = 0; i < splatmap.Length; i++)
            {
                total += splatmap[i];
            }

            for (int i = 0; i < splatmap.Length; i++)
            {
                splatmap[i] = splatmap[i] / total;
            }
        }
    }
}
