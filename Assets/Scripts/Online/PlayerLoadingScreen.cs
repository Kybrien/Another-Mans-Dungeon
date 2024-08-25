using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using TMPro;

[Serializable]
public struct MapImage
{
    public string Name;
    public Texture Texture;
}

public class PlayerLoadingScreen : NetworkBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private RawImage loadingImage;
    [SerializeField] private List<MapImage> mapImages = new List<MapImage>();
    [SerializeField] private TextMeshProUGUI mapText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Texture FindMapImage(string mapName)
    {
        foreach (MapImage mapImage in mapImages)
        {
            if (mapImage.Name == mapName)
            {
                return mapImage.Texture;
            }
        }

        mapText.text = mapName;

        return loadingImage.texture;
    }

    public IEnumerator Load(string mapName)
    {
        loadingImage.texture = FindMapImage(mapName);
        loadingScreen.SetActive(true);
        yield return null;
    }

    public void Complete()
    {
        loadingScreen.SetActive(false);
    }
}
