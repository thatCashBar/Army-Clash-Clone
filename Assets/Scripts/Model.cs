using System;
using System.Collections.Generic;
using UnityEngine;

public enum TEAM
{
    PLAYER,
    ENEMY
};
public enum ATTRIBUTE
{
    HP,
    ATK
};
public enum SHAPE
{
    CUBE,
    SPHERE
};
public enum SIZE
{
    SMALL = 3,
    BIG = 5
};
public enum COLOR
{
    BLUE,
    GREEN,
    RED,
    YELLOW
};

public class Model
{
    private int _currentLevel;
    public int CurrentLevel
    {
        get
        {
            return _currentLevel;
        }
        set
        {
            _currentLevel = value;
        }
    }
    private COLOR _playerColor;
    public COLOR PlayerColor
    {
        get
        {
            return _playerColor;
        }
        set
        {
            _playerColor = value;
        }
    }

    public Model(int level)
    {
        CurrentLevel = level;
    }

    public static int SetUnitHP(Unit unit)
    {
        int hp = 100;
        if (unit.shape == SHAPE.CUBE)
        {
            hp += 100;
        }
        else
        {
            hp += 50;
        }
        if (unit.size == SIZE.BIG)
        {
            hp += 50;
        }
        else
        {
            hp -= 50;
        }
        switch (unit.color)
        {
            case COLOR.BLUE:
                if (unit.shape == SHAPE.SPHERE)
                {
                    hp += 50;
                }
                break;
            case COLOR.YELLOW:
                if (unit.shape == SHAPE.CUBE)
                {
                    hp += 100;
                }
                break;
            case COLOR.GREEN:
                if (unit.shape == SHAPE.SPHERE)
                {
                    hp += 150;
                }
                break;
            case COLOR.RED:
                if (unit.shape == SHAPE.CUBE)
                {
                    hp += 200;
                }
                break;
        }
        hp = Math.Max(hp,150);
        return hp;
    }
    public static int SetUnitATK(Unit unit)
    {
        int atk = 10;
        if (unit.shape == SHAPE.CUBE)
        {
            atk += 10;
        }
        else
        {
            atk += 20;
        }
        switch (unit.color)
        {
            case COLOR.BLUE:
                if (unit.shape == SHAPE.CUBE)
                {
                    atk += 10;
                }
                break;
            case COLOR.YELLOW:
                if (unit.shape == SHAPE.SPHERE)
                {
                    atk += 20;
                }
                break;
            case COLOR.GREEN:
                if (unit.shape == SHAPE.CUBE)
                {
                    atk += 30;
                }
                break;
            case COLOR.RED:
                if (unit.shape == SHAPE.SPHERE)
                {
                    atk += 40;
                }
                break;
        }
        atk = Math.Max(atk, 30);
        return atk;
    }
}

public class Unit
{
    public int StartHP { get; set; }
    public int HP { get; set; }
    public int ATK { get; set; }
    private float _speed;
    public float SPEED
    {
        get
        {
            return _speed;
        }
        set
        {
            int hpMin = 150, hpMax = 450;
            float percentage = (value - hpMin) / (hpMax - hpMin);
            _speed = Mathf.Lerp(10f, 5f, percentage);
        }
    }
    private float _atkSpeed;
    public float ATKSPEED
    {
        get
        {
            return _atkSpeed;
        }
        set
        {
            int atkMin = 30, atkMax = 70;
            float percentage = (value - atkMin) / (atkMax - atkMin);
            _atkSpeed = Mathf.Lerp(1f, 2f, percentage);
        }
    }

    public SHAPE shape;
    public SIZE size;
    public COLOR color;

    public List<Soldier> soldiersInUnit = new List<Soldier>();

    public Unit(SHAPE shape, SIZE size, COLOR color)
    {
        this.color = color;
        this.shape = shape;
        this.size = size;
        HP = Model.SetUnitHP(this);
        StartHP = HP;
        ATK = Model.SetUnitATK(this);
        SPEED = HP;
        ATKSPEED = ATK;
    }
}