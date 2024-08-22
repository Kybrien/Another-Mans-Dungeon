using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLoadingScreen : NetworkBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Load(string mapName)
    {
        loadingScreen.SetActive(true);
        yield return null;
    }

    public void Complete()
    {
        loadingScreen.SetActive(false);
    }
}
