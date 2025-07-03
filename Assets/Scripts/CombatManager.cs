
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum playZoneLocation
{
    OpponentBackline,
    OpponentFrontline,
    PlayerFrontline,
    PlayerBackline
}

public enum attackTarget
{
    forward,
    forwardTwo,
    bisplit,
    trisplit
}

public struct PlayZoneData
{
    public playZoneLocation playZoneLoc;
    public GameObject playZoneObj;
    public int colummIndex;

    public bool isFrontline
    { 
        get { return ((playZoneLoc == playZoneLocation.PlayerFrontline) || (playZoneLoc == playZoneLocation.OpponentFrontline)); } 
    }
    public bool isBackline
    {
        get { return ((playZoneLoc == playZoneLocation.PlayerBackline) || (playZoneLoc == playZoneLocation.OpponentBackline)); }
    }
}


public class CombatManager : MonoBehaviour
{
    //[SerializedDictionary("Play Zone Ref Name", "Play Zone Data")]
    public Dictionary<PlayZone,PlayZoneData> playZones = new Dictionary<PlayZone, PlayZoneData>();

    //PlayZoneData[] playZones;

    [SerializeField]
    private GameObject playZonePrefab;

    [SerializeField]
    private DropZone discardZone;

    public int columns = 4;
    public float width = 10;

    [SerializeField]
    private Material red;

    private void Start()
    {
        CreatePlayZoneRow(playZoneLocation.PlayerBackline,-6.5f);
        CreatePlayZoneRow(playZoneLocation.PlayerFrontline, -2f);
        CreatePlayZoneRow(playZoneLocation.OpponentFrontline, 3f);
        CreatePlayZoneRow(playZoneLocation.OpponentBackline, 7.5f);

    }

    public void EndTurn()
    {
        StartCoroutine(EndTurnSequenceCoroutine(false));
    }
    public void EndOpponentTurn()
    {
        StartCoroutine(EndTurnSequenceCoroutine(true));
    }

    public IEnumerator EndTurnSequenceCoroutine( bool isOpponent)
    {
        if (!isOpponent)
        {
            Debug.Log("End Turn");
            for (int i = 0; i < columns; i++)
            {
                var currentPlayZone = playZones.Values.First(x => x.playZoneLoc == playZoneLocation.PlayerFrontline && x.colummIndex == i);

                if (currentPlayZone.playZoneObj.GetComponentInChildren<Card>() != null)
                {
                    Debug.Log("Card Found");
                    Card currentCard = currentPlayZone.playZoneObj.GetComponentInChildren<Card>();
                    yield return StartCoroutine(currentCard.EndTurnActions());
                }

                currentPlayZone = playZones.Values.First(x => x.playZoneLoc == playZoneLocation.PlayerBackline && x.colummIndex == i);

                if (currentPlayZone.playZoneObj.GetComponentInChildren<Card>() != null)
                {
                    Debug.Log("Card Found");
                    Card currentCard = currentPlayZone.playZoneObj.GetComponentInChildren<Card>();
                    yield return StartCoroutine(currentCard.EndTurnActions());
                }
            }
        }
        else
        {
            Debug.Log("End Turn");
            for (int i = 0; i < columns; i++)
            {
                var currentPlayZone = playZones.Values.First(x => x.playZoneLoc == playZoneLocation.OpponentFrontline && x.colummIndex == i);

                if (currentPlayZone.playZoneObj.GetComponentInChildren<Card>() != null)
                {
                    Debug.Log("Card Found");
                    Card currentCard = currentPlayZone.playZoneObj.GetComponentInChildren<Card>();
                    yield return StartCoroutine(currentCard.EndTurnActions());
                }

                currentPlayZone = playZones.Values.First(x => x.playZoneLoc == playZoneLocation.OpponentBackline && x.colummIndex == i);

                if (currentPlayZone.playZoneObj.GetComponentInChildren<Card>() != null)
                {
                    Debug.Log("Card Found");
                    Card currentCard = currentPlayZone.playZoneObj.GetComponentInChildren<Card>();
                    yield return StartCoroutine(currentCard.EndTurnActions());
                }
            }
        }
        //StopAllCoroutines();
    }
    public void CardDied(Card card)
    {
        card.GetComponent<DragAndDrop>().SetDropZone(discardZone, true);
    }
    private void CreatePlayZoneRow(playZoneLocation location, float rowHieghtOffset)
    {
        //Distance between each instance
        float offsetMult = width / columns;

        for (int i = 0; i < columns; i++)
        {
            //Offset tracks position based on i
            float offset = ((float)i + 0.5f) - (columns / 2.0f);

            //Create Playzones
            var inst = Instantiate(playZonePrefab);
            PlayZoneData data = new PlayZoneData();
            data.playZoneLoc = location;

            if (location == playZoneLocation.OpponentBackline || location == playZoneLocation.OpponentFrontline)
            {
                inst.GetComponent<MeshRenderer>().material = red;
            }

            data.playZoneObj = inst;
            data.colummIndex = i;
            inst.transform.parent = this.transform;
            inst.transform.localPosition = new Vector3(offset * offsetMult, rowHieghtOffset, 0);
            playZones.Add(inst.GetComponent<PlayZone>(), data);
        }
    }

    private playZoneLocation GetOpposingFrontLine( playZoneLocation attacker)
    {
        if (attacker == playZoneLocation.PlayerFrontline || attacker == playZoneLocation.PlayerBackline)
        {
            return playZoneLocation.OpponentFrontline;
        }
        else
        {
            return playZoneLocation.PlayerFrontline;
        }
    }
    private playZoneLocation GetOpposingBackLine(playZoneLocation attacker)
    {
        if (attacker == playZoneLocation.PlayerFrontline || attacker == playZoneLocation.PlayerBackline)
        {
            return playZoneLocation.OpponentBackline;
        }
        else
        {
            return playZoneLocation.PlayerBackline;
        }
    }
    private playZoneLocation GetAlliedFrontLine(playZoneLocation attacker)
    {
        if (attacker == playZoneLocation.PlayerFrontline || attacker == playZoneLocation.PlayerBackline)
        {
            return playZoneLocation.PlayerFrontline;
        }
        else
        {
            return playZoneLocation.OpponentFrontline;
        }
    }
    private playZoneLocation GetAlliedBackLine(playZoneLocation attacker)
    {
        if (attacker == playZoneLocation.PlayerFrontline || attacker == playZoneLocation.PlayerBackline)
        {
            return playZoneLocation.PlayerBackline;
        }
        else
        {
            return playZoneLocation.OpponentBackline;
        }
    }


    public List<PlayZone> GetTarget(Card attacker, targetType targetType)
    {
        PlayZone attackerPlayZone;
        PlayZoneData attackerPZData;
        List<PlayZone> targets = new List<PlayZone>();

        //Get Attacker PlayZone
        if (attacker.transform.parent != null && attacker.transform.parent.GetComponent<PlayZone>() != null)
        {
            attackerPlayZone = attacker.transform.parent.GetComponent<PlayZone>();
            attackerPZData = playZones[attackerPlayZone];
        }
        else
        {
            return null;
        }

        if (targetType == targetType.Standard)
        {
            //Standard Targeting fails from backline
            if (attackerPZData.isBackline)
            {
                return null;
            }

            PlayZoneData target = new();
            target.colummIndex = attackerPZData.colummIndex;
            target.playZoneLoc = GetOpposingFrontLine(attackerPZData.playZoneLoc);

            //Finds first match
            List<PlayZoneData> matches = playZones.Values.Where(x => x.playZoneLoc == target.playZoneLoc && x.colummIndex == target.colummIndex).ToList();
            if (matches.Count != 0)
            {
                //Gets card child obj
                targets.Add(matches[0].playZoneObj.GetComponent<PlayZone>());
            }

        }
        else if (targetType == targetType.Bi_Split)
        {
            //Standard Targeting fails from backline
            if (attackerPZData.isBackline)
            {
                return null;
            }

            PlayZoneData target = new();
            target.colummIndex = attackerPZData.colummIndex - 1;
            target.playZoneLoc = GetOpposingFrontLine(attackerPZData.playZoneLoc);

            //Finds first match
            List<PlayZoneData> matches = playZones.Values.Where(x => x.playZoneLoc == target.playZoneLoc && x.colummIndex == target.colummIndex).ToList();
            if (matches.Count != 0)
            {
                //Gets card child obj
                targets.Add(matches[0].playZoneObj.GetComponent<PlayZone>());
            }

            

            target = new();
            target.colummIndex = attackerPZData.colummIndex + 1;
            target.playZoneLoc = GetOpposingFrontLine(attackerPZData.playZoneLoc);

            //Finds first match
            matches = playZones.Values.Where(x => x.playZoneLoc == target.playZoneLoc && x.colummIndex == target.colummIndex).ToList();
            if (matches.Count != 0)
            {
                //Gets card child obj
                targets.Add(matches[0].playZoneObj.GetComponent<PlayZone>());
            }


        }
        else if (targetType == targetType.Ranged)
        {
            PlayZoneData target = new();
            target.colummIndex = attackerPZData.colummIndex;

            if (attackerPZData.isFrontline)
            {
                target.playZoneLoc = GetOpposingBackLine(attackerPZData.playZoneLoc);
            }
            else if (attackerPZData.isBackline)
            {
                target.playZoneLoc = GetOpposingFrontLine(attackerPZData.playZoneLoc);
            }

            //Finds first match
            List<PlayZoneData> matches = playZones.Values.Where(x => x.playZoneLoc == target.playZoneLoc && x.colummIndex == target.colummIndex).ToList();
            if (matches.Count != 0)
            {
                //Gets card child obj
                targets.Add(matches[0].playZoneObj.GetComponent<PlayZone>());
            }

        }
        else if (targetType == targetType.Partner)
        {
            PlayZoneData target = new();
            target.colummIndex = attackerPZData.colummIndex;

            if (attackerPZData.isFrontline)
            {
                target.playZoneLoc = GetAlliedBackLine(attackerPZData.playZoneLoc);
            }
            else if (attackerPZData.isBackline)
            {
                target.playZoneLoc = GetAlliedFrontLine(attackerPZData.playZoneLoc);
            }

            //Finds first match
            List<PlayZoneData> matches = playZones.Values.Where(x => x.playZoneLoc == target.playZoneLoc && x.colummIndex == target.colummIndex).ToList();
            if (matches.Count != 0)
            {
                //Gets card child obj
                targets.Add(matches[0].playZoneObj.GetComponent<PlayZone>());
            }
        }
        return targets;
    }
}
