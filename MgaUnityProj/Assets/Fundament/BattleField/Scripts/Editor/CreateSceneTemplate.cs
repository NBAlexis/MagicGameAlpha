using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateSceneTemplate : ScriptableWizard
{
    public Texture2D HeightMap;
    public Texture2D GroundMap;
    public string GroundType;
    public int ClifType;
    public string TemplateName;

    [MenuItem("MGA/Create/Scene/Create Template")]
    static void CreateWizard()
    {
        DisplayWizard<CreateSceneTemplate>("Create Scene Template", "Create");
    }

    void OnWizardCreate()
    {
        Texture2D heighttx = (Texture2D)EditorCommonFunctions.GetReadWritable(HeightMap);
        if (null == heighttx || 33 != heighttx.width || 33 != heighttx.height)
        {
            CRuntimeLogger.LogError("必须是33x33的高度贴图");
            return;
        }
        Texture2D groundtx = (Texture2D)EditorCommonFunctions.GetReadWritable(GroundMap);
        if (null == groundtx || 33 != groundtx.width || 33 != groundtx.height)
        {
            CRuntimeLogger.LogError("必须是33x33的地皮贴图");
            return;
        }
        CSceneTexture sceneTx = new CSceneTexture {m_sSceneType = GroundType};
        if (!sceneTx.Load() || 81 != sceneTx.m_pElement.Count)
        {
            CRuntimeLogger.LogError(GroundType + "贴图配置文件有问题");
            return;
        }

        CSceneGroudTemplate groundTemplate = new CSceneGroudTemplate {m_sSceneType = GroundType};
        if (!groundTemplate.Load())
        {
            CRuntimeLogger.LogError(GroundType + "贴图配置文件有问题");
            return;
        }

        Texture2D bigtexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Fundament/BattleField/Artwork/" + GroundType + "/SceneTextures.png");
        if (null == bigtexture)
        {
            CRuntimeLogger.LogError(GroundType + "贴图的Atlas大图不存在？");
            return;
        }

        Vector3[,] verts = new Vector3[33, 33];
        int[,] gtypes = new int[33, 33];
        for (int i = 0; i < 33; ++i)
        {
            for (int j = 0; j < 33; ++j)
            {
                Color h = heighttx.GetPixel(i, j);
                Color g = groundtx.GetPixel(i, j);
                verts[i, j] = new Vector3(i - 16, (h.r - 0.5f) * 6.0f, j - 16);
                gtypes[i, j] = CompareRGB(g.r, g.g, g.b);
            }
        }

        //Make Clifs
        for (int i = 0; i < 32; ++i)
        {
            for (int j = 0; j < 32; ++j)
            {
                if (Mathf.Max(verts[i, j].y, verts[i + 1, j].y, verts[i + 1, j + 1].y, verts[i, j + 1].y)
                  - Mathf.Min(verts[i, j].y, verts[i + 1, j].y, verts[i + 1, j + 1].y, verts[i, j + 1].y) > 2.0f)
                {
                    gtypes[i, j] = ClifType;
                    gtypes[i + 1, j] = ClifType;
                    gtypes[i + 1, j + 1] = ClifType;
                    gtypes[i, j + 1] = ClifType;
                }
            }
        }

        Vector3[] poses = new Vector3[4 * 32 * 32];
        Vector2[] theuvs = new Vector2[4 * 32 * 32];
        int[] trangles = new int[6 * 32 * 32];

        //Create Mesh
        for (int i = 0; i < 32; ++i)
        {
            for (int j = 0; j < 32; ++j)
            {
                Vector2[] uvs = GetUVs(gtypes[i, j], gtypes[i + 1, j], gtypes[i + 1, j + 1], gtypes[i, j + 1], sceneTx, groundTemplate);

                poses[(i * 32 + j) * 4 + 0] = verts[i, j];
                poses[(i * 32 + j) * 4 + 1] = verts[i + 1, j];
                poses[(i * 32 + j) * 4 + 2] = verts[i + 1, j + 1];
                poses[(i * 32 + j) * 4 + 3] = verts[i, j + 1];

                theuvs[(i * 32 + j) * 4 + 0] = uvs[0];
                theuvs[(i * 32 + j) * 4 + 1] = uvs[1];
                theuvs[(i * 32 + j) * 4 + 2] = uvs[2];
                theuvs[(i * 32 + j) * 4 + 3] = uvs[3];

                trangles[(i * 32 + j) * 6 + 0] = (i * 32 + j) * 4 + 0;
                trangles[(i * 32 + j) * 6 + 1] = (i * 32 + j) * 4 + 3;
                trangles[(i * 32 + j) * 6 + 2] = (i * 32 + j) * 4 + 1;

                trangles[(i * 32 + j) * 6 + 3] = (i * 32 + j) * 4 + 1;
                trangles[(i * 32 + j) * 6 + 4] = (i * 32 + j) * 4 + 3;
                trangles[(i * 32 + j) * 6 + 5] = (i * 32 + j) * 4 + 2;
            }            
        }

        Mesh theMesh = new Mesh {vertices = poses, uv = theuvs, triangles = trangles};
        theMesh.RecalculateNormals();
        Unwrapping.GenerateSecondaryUVSet(theMesh);
        theMesh.Optimize();
        AssetDatabase.CreateAsset(theMesh, "Assets/Fundament/BattleField/Artwork/Templates/" + GroundType + "/GenerateMesh/" + TemplateName + ".asset");
    }

    private static int CompareRGB(float r, float g, float b)
    {
        return r > g ? (b > r ? 2 : 0) : (b > g ? 2 : 1);
    }

    private static Vector2[] GetUVs(int lt, int rt, int rb, int lb, CSceneTexture tx, CSceneGroudTemplate tem)
    {
        int iCode2 = lt * CommonFunctions.IntPow(3, 0)
                   + rt * CommonFunctions.IntPow(3, 1)
                   + rb * CommonFunctions.IntPow(3, 2)
                   + lb * CommonFunctions.IntPow(3, 3);

        CSceneTextureElement txelement = tx[iCode2];
        if (null == txelement)
        {
            CRuntimeLogger.LogError("贴图配置文件有问题");
            return null;
        }

        string sTxName = string.Format("T{0}_{1}", txelement.m_iTemplate, Random.Range(0, txelement.m_iTextureCount) + 1);
        Rect uv = CommonFunctions.V42Rect(tem[sTxName].m_vUV);

        int iRot = txelement.m_iRotNumber;
        bool bRot = lt == rt && lt == rb && lt == lb;
        if (bRot)
        {
            iRot += Random.Range(0, 4);
        }

        Vector2[] ret =
        {
            new Vector2(uv.xMin, uv.yMin + uv.height), 
            new Vector2(uv.xMin + uv.width, uv.yMin + uv.height), 
            new Vector2(uv.xMin + uv.width, uv.yMin), 
            new Vector2(uv.xMin, uv.yMin)
        };

        for (int i = 0; i < iRot; ++i)
        {
            Vector2 last = ret[0];
            ret[0] = ret[1];
            ret[1] = ret[2];
            ret[2] = ret[3];
            ret[3] = last;

            //Vector2 last = ret[3];
            //ret[3] = ret[2];
            //ret[2] = ret[1];
            //ret[1] = ret[0];
            //ret[0] = last;
        }

        return ret;
    }
}
