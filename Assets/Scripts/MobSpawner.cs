using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public List<GameObject> monsterPrefabs; // Liste de prefabs de monstres
    public float spawnDelay = 2f; // D�lai entre chaque spawn
    public float spawnRadius = 5f; // Rayon de la zone de spawn
    public int maxMonsters = 10; // Nombre maximum de monstres

    private int currentMonsterCount = 0; // Nombre actuel de monstres

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnMonsters());
    }

    // Coroutine pour g�rer le spawn des monstres avec un d�lai
    IEnumerator SpawnMonsters()
    {
        while (currentMonsterCount < maxMonsters)
        {
            yield return new WaitForSeconds(spawnDelay); // Attendre le d�lai sp�cifi�

            // S�lectionner un prefab de monstre au hasard
            int monsterIndex = Random.Range(0, monsterPrefabs.Count);
            GameObject monsterPrefab = monsterPrefabs[monsterIndex];

            // G�n�rer une position de spawn al�atoire dans un cercle
            Vector2 spawnPosition = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnLocation = new Vector3(spawnPosition.x, 0, spawnPosition.y) + transform.position;

            // Instancier le monstre � la position g�n�r�e
            GameObject spawnedMonster = Instantiate(monsterPrefab, spawnLocation, Quaternion.identity);

            // D�finir une taille al�atoire entre 0.8 et 1.2
            float randomScale = Random.Range(0.8f, 1.2f);
            spawnedMonster.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // Incr�menter le compteur de monstres actuels
            currentMonsterCount++;
        }
    }
}
