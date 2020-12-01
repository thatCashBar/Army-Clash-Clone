using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Controller : MonoBehaviour
{
    private Model _gameModel;
    [Header("Objects")]
    public GameObject fightButton;
    public Transform enemySpawnPoint;
    public Transform mainCameraTransform;
    public Animator chooseColorPopUpAnimator;
    public Animator gameOverBannerAnimator;
    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI fightButtonTMP;
    public TextMeshProUGUI gameOverTMP;
    [Header("Object Poolers")]
    public ObjectPooler cubeUnitsObjectPooler;
    public ObjectPooler sphereUnitsObjectPooler;
    [Header("Materials")]
    public Material[] armyColorMaterials;
    private List<COLOR> _availableColors = new List<COLOR>();
    [Header("Player Grid")]
    public Transform[] gridPieces;
    private Transform[][] _gridPieces = new Transform[][]
    {
        new Transform[7],
        new Transform[7],
        new Transform[7],
        new Transform[7],
        new Transform[7],
        new Transform[7],
        new Transform[7]
    };
    private List<List<Transform>> _activeGridPieces = new List<List<Transform>>();
    private List<Vector3> _possibleSpawnPositions = new List<Vector3>();
    [Header("Misc")]
    [System.NonSerialized] public List<Unit> playerUnitsList = new List<Unit>();
    [System.NonSerialized] public List<Unit> enemyUnitsList = new List<Unit>();
    [System.NonSerialized] public List<Soldier> playerSoldiersList = new List<Soldier>();
    [System.NonSerialized] public List<Soldier> enemySoldiersList = new List<Soldier>();
    private List<Vector3> _possibleEnemySpawnPositions = new List<Vector3>();
    private Vector3 _cameraStartingPosition;
    private Vector3 _playerArmyPosition;
    private Vector3 _enemyArmyPosition;
    [System.NonSerialized] public bool fightOccuring;
    [System.NonSerialized] public bool gameOver;
    private bool _armyContainsBigUnit;


    private void Awake()
    {
        _cameraStartingPosition = mainCameraTransform.position;
    }
    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (_gameModel == null)
        {
            _gameModel = new Model(1);
        }
        else
        {
            _gameModel = new Model(_gameModel.CurrentLevel);
        }
        LoadGrid();
        LoadLists();
        View.SetLevelText(levelTMP, _gameModel.CurrentLevel);
        StartCoroutine(View.TriggerPopUpAnimWithDelay(chooseColorPopUpAnimator));
    }

    private void LateUpdate()
    {
        if (fightOccuring)
        {
            MoveCameraFollowFight();
        }
    }
    private void MoveCameraFollowFight()
    {
        _playerArmyPosition = GetAveragePositionOfArmy(playerSoldiersList);
        _enemyArmyPosition = GetAveragePositionOfArmy(enemySoldiersList);
        Vector3 destinationPosition = Vector3.Lerp(_playerArmyPosition, _enemyArmyPosition, .5f);
        destinationPosition = new Vector3(destinationPosition.x - 280, mainCameraTransform.position.y, mainCameraTransform.position.z);
        float lerpSpeed = .9f * Time.deltaTime;
        if (mainCameraTransform.position != destinationPosition)
        {
            mainCameraTransform.position = Vector3.Lerp(mainCameraTransform.position, destinationPosition, lerpSpeed);
        }
    }
    private Vector3 GetAveragePositionOfArmy(List<Soldier> listOfSoldiers)
    {
        if (listOfSoldiers.Count == 0)
            return Vector3.zero;
        float x = 0f, y = 0f, z = 0f;
        for (int i = 0; i < listOfSoldiers.Count; i++)
        {
            x += listOfSoldiers[i].currentPosition.x;
            y += listOfSoldiers[i].currentPosition.y;
            z += listOfSoldiers[i].currentPosition.z;
        }
        return new Vector3(x / listOfSoldiers.Count, y / listOfSoldiers.Count, z / listOfSoldiers.Count);
    }

    private void LoadGrid()
    {
        int pieceToAdd = 0;
        for (int i = 0; i < _gridPieces.Length; i++)
        {
            for (int j = 0; j < _gridPieces[i].Length; j++)
            {
                _gridPieces[i][j] = gridPieces[pieceToAdd];
                pieceToAdd++;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            List<Transform> rowOfGridPieces = new List<Transform>();
            for (int j = 2; j < 5; j++)
            {
                _gridPieces[i][j].GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 255);
                rowOfGridPieces.Add(_gridPieces[i][j]);
            }
            _activeGridPieces.Add(rowOfGridPieces);
        }
    }
    private void LoadLists()
    {
        FillListOfPossibleSpawnPositions();
        FillListOfPossibleEnemySpawnPositions();
        FillAvailableMaterialsList();
    }
    private void FillListOfPossibleSpawnPositions()
    {
        for (int i = 0; i < _activeGridPieces.Count; i++)
        {
            for (int j = 0; j < _activeGridPieces[i].Count; j++)
            {
                _possibleSpawnPositions.Add(_activeGridPieces[i][j].position);
            }
        }
    }
    private void FillListOfPossibleEnemySpawnPositions()
    {
        Vector3 spawnPosition = enemySpawnPoint.position;
        _possibleEnemySpawnPositions.Add(spawnPosition);
        spawnPosition = new Vector3(enemySpawnPoint.position.x + 36, enemySpawnPoint.position.y, enemySpawnPoint.position.z + 36);
        _possibleEnemySpawnPositions.Add(spawnPosition);
        spawnPosition = new Vector3(enemySpawnPoint.position.x + 36, enemySpawnPoint.position.y, enemySpawnPoint.position.z);
        _possibleEnemySpawnPositions.Add(spawnPosition);
        spawnPosition = new Vector3(enemySpawnPoint.position.x + 36, enemySpawnPoint.position.y, enemySpawnPoint.position.z - 36);
        _possibleEnemySpawnPositions.Add(spawnPosition);
        spawnPosition = new Vector3(enemySpawnPoint.position.x, enemySpawnPoint.position.y, enemySpawnPoint.position.z + 36);
        _possibleEnemySpawnPositions.Add(spawnPosition);
        spawnPosition = new Vector3(enemySpawnPoint.position.x, enemySpawnPoint.position.y, enemySpawnPoint.position.z - 36);
        _possibleEnemySpawnPositions.Add(spawnPosition);
        spawnPosition = new Vector3(enemySpawnPoint.position.x - 36, enemySpawnPoint.position.y, enemySpawnPoint.position.z + 36);
        _possibleEnemySpawnPositions.Add(spawnPosition);
        spawnPosition = new Vector3(enemySpawnPoint.position.x - 36, enemySpawnPoint.position.y, enemySpawnPoint.position.z);
        _possibleEnemySpawnPositions.Add(spawnPosition);
        spawnPosition = new Vector3(enemySpawnPoint.position.x - 36, enemySpawnPoint.position.y, enemySpawnPoint.position.z - 36);
        _possibleEnemySpawnPositions.Add(spawnPosition);
    }
    private void FillAvailableMaterialsList()
    {
        COLOR[] values = (COLOR[])System.Enum.GetValues(typeof(COLOR));
        foreach (COLOR color in values)
        {
            _availableColors.Add(color);
        }
    }

    public void ButtonSetColorBlue()
    {
        _gameModel.PlayerColor = COLOR.BLUE;
        SetupMatch();
    }
    public void ButtonSetColorYellow()
    {
        _gameModel.PlayerColor = COLOR.YELLOW;
        SetupMatch();
    }
    public void ButtonSetColorGreen()
    {
        _gameModel.PlayerColor = COLOR.GREEN;
        SetupMatch();
    }
    public void ButtonSetColorRed()
    {
        _gameModel.PlayerColor = COLOR.RED;
        SetupMatch();
    }

    private void SetupMatch()
    {
        SpawnArmy(TEAM.PLAYER);
        SpawnArmy(TEAM.ENEMY);
        StartCoroutine(View.TriggerPopUpAnimOutroCo(chooseColorPopUpAnimator));
        StartCoroutine(View.DisplayFightButtonWithDelay(fightButton, fightButtonTMP, "Fight!", chooseColorPopUpAnimator.GetCurrentAnimatorStateInfo(0).length));
    }
    private void SpawnArmy(TEAM team)
    {
        COLOR colorToAssign = GetRandomUnitColor(team);
        for (int i = 0; i < 6; i++)
        {
            SpawnRandomUnit(team, colorToAssign);
        }
        _availableColors.Remove(colorToAssign);
        _armyContainsBigUnit = false;
    }
    private void SpawnRandomUnit(TEAM team, COLOR colorToAssign)
    {
        SHAPE randomShapeSelected = GetRandomUnitShapeType();
        SIZE randomSizeSelected = GetRandomUnitSize();
        Unit newUnit = new Unit(randomShapeSelected, randomSizeSelected, colorToAssign);
        AddUnitToListOfUnits(newUnit, team);
        Vector3 unitSpawnPoint = GetUnitSpawnPoint(team);
        List<Vector3> availableSoldierSpawnPoints = SetFormationSpawnPoints(newUnit, unitSpawnPoint);
        for (int i = 0; i < (int)newUnit.size; i++)
        {
            Transform newSoldierTransform;
            if (newUnit.shape == SHAPE.CUBE)
            {
                newSoldierTransform = cubeUnitsObjectPooler.GetPooledObject().GetComponent<Transform>();
            }
            else
            {
                newSoldierTransform = sphereUnitsObjectPooler.GetPooledObject().GetComponent<Transform>();
            }
            int randomChoice = Random.Range(0, availableSoldierSpawnPoints.Count);
            newSoldierTransform.position = new Vector3(availableSoldierSpawnPoints[randomChoice].x, newSoldierTransform.transform.position.y, availableSoldierSpawnPoints[randomChoice].z);
            availableSoldierSpawnPoints.Remove(availableSoldierSpawnPoints[randomChoice]);
            newSoldierTransform.GetComponent<MeshRenderer>().material = armyColorMaterials[(int)newUnit.color];
            SetDirectionSoldierIsFacing(team, newSoldierTransform);
            Soldier newSoldier = newSoldierTransform.GetComponent<Soldier>();
            AddSoldierToListOfSoldiers(team, newSoldier);
            newSoldier.rigidBody.velocity = new Vector3(0, 0, 0);
            newSoldier.controllerScript = this;
            newSoldier.unit = newUnit;
            newSoldier.team = team;
            newSoldier.gameObject.SetActive(true);
        }
    }
    private void AddUnitToListOfUnits(Unit unit, TEAM team)
    {
        if (team.Equals(TEAM.PLAYER))
        {
            playerUnitsList.Add(unit);
        }
        else
        {
            enemyUnitsList.Add(unit);
        }
    }
    private SIZE GetRandomUnitSize()
    {
        SIZE size;
        if (_armyContainsBigUnit)
        {
            size = SIZE.SMALL;
        }
        else
        {
            int max = System.Enum.GetNames(typeof(SHAPE)).Length;
            int randomSizeSelected = Random.Range(0, max);
            if (randomSizeSelected == 0)
            {
                size = SIZE.SMALL;
            }
            else
            {
                size = SIZE.BIG;
                _armyContainsBigUnit = true;
            }
        }
        return size;
    }
    private SHAPE GetRandomUnitShapeType()
    {
        int max = System.Enum.GetNames(typeof(SHAPE)).Length;
        int selection = Random.Range(0, max);
        return (SHAPE)System.Enum.ToObject(typeof(SHAPE), selection);
    }
    private COLOR GetRandomUnitColor(TEAM team)
    {
        COLOR color;
        if (team.Equals(TEAM.PLAYER))
        {
            color = _gameModel.PlayerColor;
        }
        else
        {
            int randomSelect = Random.Range(0,_availableColors.Count);
            color = _availableColors[randomSelect];
        }
        return color;
    }
    private Vector3 GetUnitSpawnPoint(TEAM team)
    {
        Vector3 spawnPosition = new Vector3();
        if (team.Equals(TEAM.PLAYER))
        {
            int selector = Random.Range(0, _possibleSpawnPositions.Count);
            spawnPosition = _possibleSpawnPositions[selector];
            _possibleSpawnPositions.Remove(_possibleSpawnPositions[selector]);
        }
        else
        {
            int selector = Random.Range(0, _possibleEnemySpawnPositions.Count);
            spawnPosition = _possibleEnemySpawnPositions[selector];
            _possibleEnemySpawnPositions.Remove(_possibleEnemySpawnPositions[selector]);
        }
        return spawnPosition;
    }
    private List<Vector3> SetFormationSpawnPoints(Unit unit, Vector3 unitSpawnPoint)
    {
        List<Vector3> listToReturn = new List<Vector3>();
        Vector3 spawnPoint = unitSpawnPoint;
        int distanceApart = 11;
        if (unit.size == SIZE.BIG)
        {
            if (unit.shape == SHAPE.CUBE)
            {
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x - distanceApart, unitSpawnPoint.y, unitSpawnPoint.z);
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x + distanceApart, unitSpawnPoint.y, unitSpawnPoint.z);
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x, unitSpawnPoint.y, unitSpawnPoint.z + distanceApart);
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x, unitSpawnPoint.y, unitSpawnPoint.z - distanceApart);
                listToReturn.Add(spawnPoint);
            }
            else
            {
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x - distanceApart, unitSpawnPoint.y, unitSpawnPoint.z + distanceApart);
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x + distanceApart, unitSpawnPoint.y, unitSpawnPoint.z + distanceApart);
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x - distanceApart, unitSpawnPoint.y, unitSpawnPoint.z - distanceApart);
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x + distanceApart, unitSpawnPoint.y, unitSpawnPoint.z - distanceApart);
                listToReturn.Add(spawnPoint);
            }
        }
        else
        {
            if (unit.shape == SHAPE.CUBE)
            {
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x, unitSpawnPoint.y, unitSpawnPoint.z - distanceApart);
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x, unitSpawnPoint.y, unitSpawnPoint.z + distanceApart);
                listToReturn.Add(spawnPoint);
            }
            else
            {
                spawnPoint = new Vector3(unitSpawnPoint.x + distanceApart, unitSpawnPoint.y, unitSpawnPoint.z);
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x - distanceApart, unitSpawnPoint.y, unitSpawnPoint.z - distanceApart);
                listToReturn.Add(spawnPoint);
                spawnPoint = new Vector3(unitSpawnPoint.x - distanceApart, unitSpawnPoint.y, unitSpawnPoint.z + distanceApart);
                listToReturn.Add(spawnPoint);
            }
        }
        return listToReturn;
    }
    private void SetDirectionSoldierIsFacing(TEAM team, Transform newSoldierTransform)
    {
        if (team.Equals(TEAM.PLAYER))
        {
            newSoldierTransform.eulerAngles = new Vector3(0, 90, 0);
        }
        else
        {
            newSoldierTransform.eulerAngles = new Vector3(0, -90, 0);
        }
    }
    private void AddSoldierToListOfSoldiers(TEAM team, Soldier soldier)
    {
        if (team.Equals(TEAM.PLAYER))
        {
            playerSoldiersList.Add(soldier);
        }
        else
        {
            enemySoldiersList.Add(soldier);
        }
    }

    public void BeginFight()
    {
        fightButton.SetActive(false);
        fightOccuring = true;
    }
    public Transform GetClosestEnemy(Transform transform, TEAM team)
    {
        List<Soldier> listOfEnemySoldiers = new List<Soldier>();
        if (team.Equals(TEAM.PLAYER))
        {
            listOfEnemySoldiers = enemySoldiersList;
        }
        else
        {
            listOfEnemySoldiers = playerSoldiersList;
        }
        Transform enemyTransform = listOfEnemySoldiers[0].GetComponent<Transform>();
        float distance = Vector3.Distance(listOfEnemySoldiers[0].currentPosition, transform.position);
        float smallestDistance = distance;
        for (int i = 0; i < listOfEnemySoldiers.Count; i++)
        {
            distance = Vector3.Distance(listOfEnemySoldiers[i].currentPosition, transform.position);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                enemyTransform = listOfEnemySoldiers[i].GetComponent<Transform>();
            }
        }
        return enemyTransform;
    }
    public void RemoveDeadSoldierFromSoldierList(Soldier fallenSoldier)
    {
        if (fallenSoldier.team.Equals(TEAM.PLAYER))
        {
            playerSoldiersList.Remove(fallenSoldier);
        }
        else
        {
            enemySoldiersList.Remove(fallenSoldier);
        }
    }
    public void RemoveDeadUnitFromUnitList(Soldier lastFallenSoldier)
    {
        if (lastFallenSoldier.team.Equals(TEAM.PLAYER))
        {
            playerUnitsList.Remove(lastFallenSoldier.unit);
        }
        else
        {
            enemyUnitsList.Remove(lastFallenSoldier.unit);

        }
    }

    public void GameOver()
    {
        gameOver = true;
        fightOccuring = false;
        gameOverBannerAnimator.gameObject.SetActive(true);
        if (enemySoldiersList.Count.Equals(0))
        {
            View.SetTextToString(gameOverTMP, "Victory!");
            _gameModel.CurrentLevel++;
        }
        else
        {
            View.SetTextToString(gameOverTMP, "Defeat!");
        }
        StartCoroutine(View.DisplayFightButtonWithDelay(fightButton, fightButtonTMP, "Rematch?", gameOverBannerAnimator.GetCurrentAnimatorStateInfo(0).length));
    }
    private void ClearRemainingSoldiers(List<Soldier> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].gameObject.SetActive(false);
            list[i].transform.position = new Vector3(0, list[i].transform.localScale.y / 2f, 0);
        }
    }
    private void ClearAllLists()
    {
        _availableColors = new List<COLOR>();
        _activeGridPieces = new List<List<Transform>>();
        _possibleSpawnPositions = new List<Vector3>();
        playerUnitsList = new List<Unit>();
        enemyUnitsList = new List<Unit>();
        playerSoldiersList = new List<Soldier>();
        enemySoldiersList = new List<Soldier>();
        _possibleEnemySpawnPositions = new List<Vector3>();
    }
    public IEnumerator RematchCo()
    {
        fightButton.SetActive(false);
        StartCoroutine(View.TriggerPopUpAnimOutroCo(gameOverBannerAnimator));
        ClearRemainingSoldiers(playerSoldiersList);
        ClearRemainingSoldiers(enemySoldiersList);
        ClearAllLists();
        StartCoroutine(ResetCameraPositionCo());
        while (Vector3.Distance(mainCameraTransform.position, _cameraStartingPosition) > 1)
        {
            yield return null;
        }
        gameOver = false;
        StartGame();
    }
    private IEnumerator ResetCameraPositionCo()
    {
        float lerpSpeed = 3.8f * Time.deltaTime;
        while (Vector3.Distance(mainCameraTransform.position, _cameraStartingPosition) > 1)
        {
            mainCameraTransform.position = Vector3.Lerp(mainCameraTransform.position, _cameraStartingPosition, lerpSpeed);
            yield return null;
        }
    }
}