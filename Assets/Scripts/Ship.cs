using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship 
{
    private int size;
    private int value;
    private int remainingParts;

    public Ship(int size,int value)
    {
        this.size = size;
        this.remainingParts = size;
        this.value = value;
    }

    public int getSize()
    {
        return size;
    }

    public int getValue()
    {
        return value;
    }

    public void SinkPart()
    {
        remainingParts--;
    }

    public bool HasBeenSunk()
    {
        return remainingParts < 1;
    }

    

}
