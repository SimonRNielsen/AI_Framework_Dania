using UnityEngine;
using System.Linq;
using System.Collections.Generic;


public class KnownPowerUp
{

    public Vector3 position;
    public float lastTimeSeen;
    public bool collected = false;

    public KnownPowerUp(Vector3 post, float time)
    {

        position = post;
        lastTimeSeen = time;

    }

    public void IsStillValid()
    {

        lastTimeSeen = Time.time;

    }

}


public class KnownEnemy
{

    public Vector3 lastSeenPosition;
    public float lastTimeSeen;
    public int enemyID;

    public KnownEnemy(int id, Vector3 pos, float time)
    {

        enemyID = id;
        lastSeenPosition = pos;
        lastTimeSeen = time;

    }

    public void IsRecentSighting()
    {

        lastTimeSeen = Time.time;

    }

}


public class WorldModel
{

    private Dictionary<int, KnownPowerUp> knownPowerUps = new Dictionary<int, KnownPowerUp>();
    private Dictionary<int, KnownEnemy> knownEnemies = new Dictionary<int, KnownEnemy>();


    public void UpdatePowerUp(int id, Vector3 position)
    {

        if (knownPowerUps.TryGetValue(id, out KnownPowerUp powerUp))
            powerUp.IsStillValid();
        else
            knownPowerUps.TryAdd(id, new KnownPowerUp(position, Time.time));

    }


    public void MarkPowerUpCollected(int id)
    {

        if (knownPowerUps.TryGetValue(id, out KnownPowerUp powerUp))
            powerUp.collected = true;

    }


    public void UpdateEnemy(int enemyID, Vector3 position)
    {

        if (knownEnemies.TryGetValue(enemyID, out KnownEnemy enemy))
        {

            enemy.lastSeenPosition = position;
            enemy.IsRecentSighting();

        }
        else
            knownEnemies.TryAdd(enemyID, new KnownEnemy(enemyID, position, Time.time));

    }


    public Vector3? GetClosestKnownPowerUp(Vector3 myPosition)
    {

        return knownPowerUps.Values.OrderBy(x => Vector3.Distance(myPosition, x.position)).FirstOrDefault()?.position;

    }


    public Vector3? GetMostRecentEnemyPosition()
    {

        return knownEnemies.Values.Where(x => Time.time - x.lastTimeSeen <= 10f).OrderByDescending(x => x.lastTimeSeen).FirstOrDefault()?.lastSeenPosition;

    }


    public void CleanUpData()
    {

        if (knownPowerUps.Count > 0)
            foreach (int key in knownPowerUps.Where(x => x.Value.collected).Select(x => x.Key).ToList())
                knownPowerUps.Remove(key);

    }


    public int GetKnownPowerUpCount()
    {

        return knownPowerUps.Count;

    }


    public int GetKnownEnemyCount()
    {

        return knownEnemies.Count(x => Time.time - x.Value.lastTimeSeen <= 10f);

    }

}