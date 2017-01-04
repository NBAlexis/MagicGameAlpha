using UnityEngine;
using System.Collections;

public class SceneConstant
{
    public const float m_fHighgroundHeight = 2.0f;
    public const float m_fCutOffHeightRate = -0.5f;
    public const float m_fCliffRate = 0.7f;

    public const short m_iHightmapSize = 33;
    public const short m_iDecoratemapSize = m_iHightmapSize - 1;
    public const short m_iSceneHeightMapSize = m_iHightmapSize * 2 - 1;
    public const short m_iSceneSize = m_iSceneHeightMapSize - 1;
    public const short m_iSceneSizeSq = m_iSceneSize * m_iSceneSize;

    public const string m_sArtworkPath = "Fundament/BattleField/Artwork/";
    public const string m_sSceneTexturesPath = "/SceneTextures";
    public const string m_sDecoratesPath = "/Decorates";
}
