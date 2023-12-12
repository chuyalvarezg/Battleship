using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile: MonoBehaviour
{
    private Ship content;
    private bool discovered = false;
    private bool attacked = false;
    public GridSystem grid;
    public int coordX;
    public int coordY;

    public void SetContent(Ship content)
    {
        this.content = content;
    }

    public Ship GetContent()
    {
        return this.content;
    }

    public void clicked()
    {
        grid.click(this);
    }

    public void Discover()
    {
        this.discovered = true;
    }

    public bool IsDiscovered()
    {
        return this.discovered;
    }

    public bool Occupied()
    {
        return content != null;
    }

    public bool Attack()
    {
        if (!attacked)
        {
            if (content != null)
            {
                content.SinkPart();
                this.transform.GetChild(1).GetComponent<Text>().text = content.getValue().ToString();
                this.transform.GetChild(0).GetComponent<Image>().enabled = true;
                this.GetComponent<Button>().interactable = false;
                this.discovered = true;
                attacked = true;
                return true;
            }
            else
            {
                this.transform.GetChild(1).GetComponent<Text>().text = "0";
                
               
            }

            this.discovered = true;
            attacked = true;
            this.GetComponent<Button>().interactable = false;
        }
        return false;
    }




}
