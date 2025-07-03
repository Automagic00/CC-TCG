using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using UnityEngine.Events;
using LitMotion;
using LitMotion.Extensions;
using System.Threading.Tasks;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField]
    private CardScriptableObject cardData;

    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private TMP_Text descText;

    [SerializeField]
    private TMP_Text hpText;

    [SerializeField]
    private TMP_Text dmgText;

    [SerializeField]
    private Image creatureImage;

    private int currentHP;
    private int maxHP;

    private List<IEnumerator> actionMethods = new List<IEnumerator>();
    private List<Action> actions = new List<Action>();
    private targetType targetType;

    private CombatManager combatManager;

    private MotionHandle currentMotion;
    private Bounds boundingBox;

    private void Awake()
    {
        combatManager = GameObject.Find("CombatManager").GetComponent<CombatManager>();
        boundingBox = GetComponent<BoxCollider2D>().bounds;
        nameText.text = cardData.cardName;
        descText.text = cardData.description;
        hpText.text = cardData.health.ToString();
        maxHP = cardData.health;
        currentHP = maxHP;
        dmgText.text = cardData.damage.ToString();
        creatureImage.sprite = cardData.cardImage;
        actions = cardData.actions;
        //targetType = cardData.targetType;

        /*if (attackMethod == null)
        {
            attackMethod;
        }*/

        //SetActionMethods();
    }

    public IEnumerator EndTurnActions()
    {
        SetActionMethods();
        //attackMethod.Invoke();
        Debug.Log("in EndTurn");
        foreach (var actionMetod in actionMethods)
        {
            yield return StartCoroutine(actionMetod);
        }

    }

    public IEnumerator Attacked(int damage)
    {
        currentHP -= damage;
        hpText.text = currentHP.ToString();
        currentMotion = LMotion.Shake.Create(0f, 8f, 0.5f).BindToEulerAnglesZ(transform);
        while (currentMotion.IsPlaying())
        {
            yield return null;
        }
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Die ()
    {
        combatManager.CardDied(this);
    }
    public void Healed(int health)
    {
        currentHP += health;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        hpText.text = currentHP.ToString();
        LMotion.Punch.Create(transform.rotation.eulerAngles.z, 8f, 0.5f).WithFrequency(4).BindToEulerAnglesZ(transform);
           
    }

    private void SetActionMethods()
    {
        actionMethods.Clear();
        foreach (var action in actions)
        {
            List<IEnumerator> methodHolder = new List<IEnumerator>();
            List<PlayZone> targets = combatManager.GetTarget(this, action.target);
            if (targets != null)
            {
                foreach (var target in targets)
                {
                    switch (action.action)
                    {
                        case actionType.Attack: methodHolder.Add(AttackSequence(target)); break;
                        case actionType.Heal: methodHolder.Add(HealSequence(target,action.value)); break;
                        default: methodHolder.Add(AttackSequence(target)); break;
                    }
                }
                actionMethods.AddRange(methodHolder);
            }
        }
            
    }

    /*private IEnumerator AttackStandard()
    {
        //PlayZone target = combatManager.GetTarget(this, attackTarget.forward);
        Debug.Log("in Attack Standard");
        yield return StartCoroutine(AttackSequence(target));

        Debug.Log(target.name);
    }*/

    private IEnumerator AttackSequence(PlayZone target)
    {
        Debug.Log("in Attack Sequence");
        if (currentMotion.IsActive())
        {
            currentMotion.Cancel();
        }

        Vector3 relative = transform.InverseTransformPoint(target.transform.position);
        Vector3 hitLocation = new Vector3(target.transform.position.x - (boundingBox.extents.y * relative.normalized.x), target.transform.position.y - (boundingBox.extents.y * relative.normalized.y), target.transform.position.z);
        float aim = -Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        currentMotion = LSequence.Create()
            .Append(LMotion.Create(0f, aim, 0.1f).BindToEulerAnglesZ(transform))
            .Join(LMotion.Create(0f, -8.0f, 0.1f).BindToEulerAnglesX(transform))
            .Join(LMotion.Create(transform.position.z, transform.position.z - 1, 0.1f).BindToPositionZ(transform))
            .Append(LMotion.Create(transform.position.x, hitLocation.x, 0.15f).WithEase(Ease.InBack).BindToPositionX(transform))
            .Join(LMotion.Create(transform.position.y, hitLocation.y, 0.15f).WithEase(Ease.InBack).BindToPositionY(transform))
            .Join(LMotion.Create(-8f, 8f, 0.15f).WithEase(Ease.InBack).WithOnComplete(() => target.Hit(cardData.damage)).BindToEulerAnglesX(transform))
            .AppendInterval(0.1f)
            .Append(LMotion.Create(
                new Vector3(hitLocation.x, hitLocation.y, hitLocation.z - 1),
                transform.position, 0.4f).WithEase(Ease.OutCubic).BindToPosition(transform))

            .Join(LMotion.Create(8f, 0.0f, 0.2f).BindToEulerAnglesX(transform))
            .Join(LMotion.Create(aim, 0, 0.2f).BindToEulerAnglesZ(transform))
            .Run();
        while (currentMotion.IsPlaying())
        {
            yield return null;
        }
    }

    private IEnumerator HealSequence(PlayZone target, int healValue)
    {
        if (target.GetComponentInChildren<Card>() == null)
        {
            yield break;
        }

        Debug.Log("in Attack Sequence");
        if (currentMotion.IsActive())
        {
            currentMotion.Cancel();
        }

        Vector3 relative = transform.InverseTransformPoint(target.transform.position);
        Vector3 hitLocation = new Vector3(target.transform.position.x - (boundingBox.extents.y * relative.normalized.x), target.transform.position.y - (boundingBox.extents.y * relative.normalized.y), target.transform.position.z);
        float aim = -Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        currentMotion = LSequence.Create()
            .Append(LMotion.Create(0f, aim, 0.1f).BindToEulerAnglesZ(transform))
            .Join(LMotion.Create(0f, -8.0f, 0.1f).BindToEulerAnglesX(transform))
            .Join(LMotion.Create(transform.position.z, transform.position.z - 1, 0.1f).BindToPositionZ(transform))
            .Append(LMotion.Create(transform.position.x, hitLocation.x, 0.15f).WithEase(Ease.InBack).BindToPositionX(transform))
            .Join(LMotion.Create(transform.position.y, hitLocation.y, 0.15f).WithEase(Ease.InBack).BindToPositionY(transform))
            .Join(LMotion.Create(-8f, 8f, 0.15f).WithEase(Ease.InBack).WithOnComplete(() => target.Heal(healValue)).BindToEulerAnglesX(transform))
            .AppendInterval(0.1f)
            .Append(LMotion.Create(
                new Vector3(hitLocation.x, hitLocation.y, hitLocation.z - 1),
                transform.position, 0.4f).WithEase(Ease.OutCubic).BindToPosition(transform))

            .Join(LMotion.Create(8f, 0.0f, 0.2f).BindToEulerAnglesX(transform))
            .Join(LMotion.Create(aim, 0, 0.2f).BindToEulerAnglesZ(transform))
            .Run();
        while (currentMotion.IsPlaying())
        {
            yield return null;
        }
    }
}
