using System;
using Godot;

public class Basic_Func
{

    Node addingNode;
    public Global_Data data;


    public Basic_Func(Node node)
    {
        addingNode = node;
        data = addingNode.GetNode<Global_Data>("/root/Global_Data");
    }

    public Timer getTimer(float waitTime,string timerOverMethName,bool isOneShoot = false)
    {
        Timer newTimer = new Timer();
        newTimer.WaitTime = waitTime;
        newTimer.OneShot = isOneShoot;
        newTimer.Connect("timeout",this.addingNode,timerOverMethName);

        return newTimer;
    }

    public bool isMouseInsideRect(Vector2 globalPosition,Vector2 size,Vector2 mousePos)
    {
        float m_x = mousePos.x;
        float m_y = mousePos.y;

        float g_x = globalPosition.x;
        float g_y = globalPosition.y;

        float width = size.x;
        float height = size.y;

        if(m_x>g_x && m_y>g_y && m_x<g_x+width && m_y<g_y+height)
        {
            return true;
        }

        return false;
        
    }

    public float followUpPoint(float cPos,float tarPos,float absSpeed)
    {
        absSpeed = (tarPos>cPos)?absSpeed:-absSpeed;
        cPos+=absSpeed;

        cPos = ((cPos>tarPos && absSpeed>0) || (cPos<tarPos && absSpeed<0))?tarPos:cPos;
        return cPos;
    }

    public int minMaxer(int num,int max,int min)
    {
        return (num>max)?max:(num<min)?min:num;
    }

    


}