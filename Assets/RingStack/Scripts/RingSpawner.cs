using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> rings = new List<GameObject>();

    private BodyList bodyList;

    private GameObject ringOne;
    private GameObject ringTwo;

    private int ringOneCount;
    private int ringTwoCount;

    [SerializeField] private float yValue = 0.8f;
    [SerializeField] private float yPlusValue = 1.8f;

    private void Awake()
    {
        bodyList = GetComponent<BodyList>();
    }
    private void Start()
    {
        SelectTwoRings();
        SpawnRings();
    }

    private void SelectTwoRings()
    {
        ringOne = rings[Random.Range(0, rings.Count)];
        ringTwo = rings[Random.Range(0, rings.Count)];

        while (ringOne == ringTwo)
        {
            ringTwo = rings[Random.Range(0, rings.Count)];
        }
    }

    private void SpawnRings()
    {
        //Creates a list that holds 2 types of rings that can be spawned.

        List<GameObject> twoRings = new List<GameObject>();

        twoRings.Add(ringOne);
        twoRings.Add(ringTwo);
        
        //

        for (int i = 0; i < 6; i++)
        {
            GameObject spawnRing = twoRings[Random.Range(0, twoRings.Count)];

            BodyBehaviour body = RandomBody(spawnRing);

            GameObject ringClone = Instantiate(spawnRing, SpawnPos(body), ringOne.transform.rotation, transform);

            ringClone.GetComponent<Ring_ObjectBehaviour>().myBody = body;

            AddToList(body, ringClone);

            CheckRingCount(spawnRing, twoRings);
        }
    }

    private void AddToList(BodyBehaviour _randomBody, GameObject _ringClone)
    {
        _randomBody.containingRings.Add(_ringClone);
        _randomBody.CheckList();

        //There can't be 4 rings in a body. Code beneath removes the body with 3 rings from the spawn list.

        if (_randomBody.containingRings.Count > 2)
        {
            bodyList.bodies.Remove(_randomBody);
        }

        //
    }

    private void CheckRingCount(GameObject ring, List<GameObject> _twoRings)
    {
        //If 3 of same colored ring is created, it is removed from the list. In this way, there are no 4 or more rings of the same color.

        if (ring == ringOne)
        {
            ringOneCount++;
            if (ringOneCount > 2)
            {
                _twoRings.Remove(ringOne);
            }
        }
        else
        {
            ringTwoCount++;
            if (ringTwoCount > 2)
            {
                _twoRings.Remove(ringTwo);
            }
        }

        //
    }

    private Vector3 SpawnPos(BodyBehaviour _randomBody)
    {
        Vector3 bodyPos = _randomBody.transform.position;
        bodyPos.y = yValue + Mathf.Abs(((_randomBody.containingRings.Count) * yPlusValue));
        return bodyPos;
    }

    private BodyBehaviour RandomBody(GameObject _spawnRing)
    {
        BodyBehaviour randomBody = bodyList.bodies[Random.Range(0, bodyList.bodies.Count)];

        int countInt = 0;

        //If the rings of the same color are at the bottom of the 3 bodies, the game cannot be solved.
        //The foreach loop down below prevents this.

        foreach (var body in bodyList.bodies)
        {
            if (body != randomBody)
            {
                if (body.containingRings.Count > 0)
                {
                    if (_spawnRing.GetComponent<Ring_ObjectBehaviour>().color == body.containingRings[0].GetComponent<Ring_ObjectBehaviour>().color)
                    {
                        countInt++;
                    }
                    if (countInt == 2)
                    {
                        randomBody = body;
                    }
                }
            }
        }

        //

        return randomBody;
    }
}
