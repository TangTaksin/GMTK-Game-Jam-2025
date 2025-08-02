using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnswerBox : MonoBehaviour
{
    ItemDisplay answeredObjects;

    public delegate void BoxEvent(AnswerBox box);
    public static BoxEvent AddEvent;

    public void Add(ItemDisplay obj)
    {
        answeredObjects = obj;
        AddEvent?.Invoke(this);
    }

    public ItemDisplay ReadObjects()
    {
        return answeredObjects;
    }
}
