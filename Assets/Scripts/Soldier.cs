using UnityEngine;

public class Soldier : MonoBehaviour
{
    [System.NonSerialized] public Unit unit;
    [System.NonSerialized] public TEAM team;
    [System.NonSerialized] public Controller controllerScript;
    [System.NonSerialized] public Vector3 currentPosition;
    [System.NonSerialized] public Rigidbody rigidBody;
    private readonly float _timeBetweenJumps = .25f;
    private readonly float _combatJumpHeight = 104f;
    private readonly float _casualJumpHeight = 48f;
    private float _attackSpeedCounter;
    private float _currentJumpHeight;
    private float _walkHopCounter;
    private bool _touchingGround;


    private void Awake()
    {
        _attackSpeedCounter = 0f;
        _walkHopCounter = Random.Range(0, _timeBetweenJumps);
        rigidBody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        currentPosition = transform.position;
        if (controllerScript.fightOccuring)
        {
            MoveTowardClosestEnemy();
            Bounce();
        }
    }

    private void Bounce()
    {
        if (_walkHopCounter >= _timeBetweenJumps && _touchingGround)
        {
            rigidBody.AddForce(transform.up * (Mathf.Abs(Physics.gravity.y) * _currentJumpHeight));
            _walkHopCounter = 0f;
        }
        else
        {
            _walkHopCounter += Time.deltaTime;
        }
    }
    private void MoveTowardClosestEnemy()
    {
        try
        {
            float moveSpeed = 10f * Time.deltaTime;
            Transform closestEnemyTransform = controllerScript.GetClosestEnemy(transform, team);
            Soldier closestEnemy = closestEnemyTransform.GetComponent<Soldier>();
            LookAtEnemy(closestEnemyTransform);
            if (Vector3.Distance(transform.position, closestEnemyTransform.position) > 12f)
            {
                transform.position = Vector3.MoveTowards(transform.position, closestEnemyTransform.position, moveSpeed);
            }
            CheckIfCanFight(closestEnemy, closestEnemyTransform);
        }
        catch
        {
            if (rigidBody.velocity.y > 0)
            {
                rigidBody.velocity = new Vector3(0,0,0);
            }
            if (controllerScript.fightOccuring)
            {
                controllerScript.GameOver();
            }
        }
    }
    private void LookAtEnemy(Transform closestEnemyTransform)
    {
        Vector3 delta = closestEnemyTransform.position - transform.position;
        delta.y = 0;
        Quaternion rotation = Quaternion.LookRotation(delta);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4f);
    }
    private void CheckIfCanFight(Soldier closestEnemy, Transform closestEnemyTransform)
    {
        if (Vector3.Distance(transform.position, closestEnemyTransform.position) < 16f)
        {
            if (!_currentJumpHeight.Equals(_combatJumpHeight))
            {
                _currentJumpHeight = _combatJumpHeight;
            }
            EngageEnemyInCombat(closestEnemy);
        }
        else
        {
            if (!_currentJumpHeight.Equals(_casualJumpHeight))
            {
                _currentJumpHeight = _casualJumpHeight;
            }
        }
    }
    private void EngageEnemyInCombat(Soldier closestEnemy)
    {
        if (_attackSpeedCounter >= unit.ATKSPEED)
        {
            AttackEnemy(closestEnemy);
            _attackSpeedCounter = 0;
        }
        else
        {
            _attackSpeedCounter += Time.deltaTime;
        }
    }
    private void AttackEnemy(Soldier closestEnemy)
    {
        closestEnemy.unit.HP -= unit.ATK;
        float unitHPSoldierDeathIncrement = ((float)closestEnemy.unit.StartHP / (int)closestEnemy.unit.size);
        float hpValueToKillSoldier = unitHPSoldierDeathIncrement / closestEnemy.unit.soldiersInUnit.Count;
        if (closestEnemy.unit.HP > 0)
        {
            if (closestEnemy.unit.HP <= hpValueToKillSoldier)
            {
                KillEnemy(closestEnemy);
            }
        }
    }
    private void KillEnemy(Soldier closestEnemy)
    {
        closestEnemy.unit.soldiersInUnit.Remove(closestEnemy);
        closestEnemy.gameObject.SetActive(false);
        closestEnemy.transform.position = new Vector3(0, closestEnemy.transform.localScale.y / 2f, 0);
        controllerScript.RemoveDeadSoldierFromSoldierList(closestEnemy);
        if (closestEnemy.unit.HP <= 0)
        {
            controllerScript.RemoveDeadUnitFromUnitList(closestEnemy);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Ground"))
        {
            _touchingGround = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name.Equals("Ground"))
        {
            _touchingGround = false;
        }
    }
}
