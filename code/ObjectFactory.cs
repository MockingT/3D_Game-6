

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    private static List<GameObject> used = new List<GameObject>();
    private static List<GameObject> free = new List<GameObject>();
    
    public GameObject setObjectOnPos(Vector3 targetposition, Quaternion faceposition)
    {
        int sizeFree = free.Count;
        if (sizeFree != 0)
        {
            used.Add(free[0]);
            free.RemoveAt(0);
            used[used.Count - 1].SetActive(true);
            used[used.Count - 1].transform.position = targetposition;
            used[used.Count - 1].transform.localRotation = faceposition;
        }
        else
        {   // create a new instance and set the position and direction
            GameObject aGameObject = Instantiate(Resources.Load("prefabs/Patrol")
                , targetposition, faceposition) as GameObject;
            used.Add(aGameObject);
        }
        return used[used.Count - 1];
    }

    public void clear_all()
    {
        /*int size = free.Count;
        while(size > 0)
        {
            Destroy(free[size - 1].gameObject);
            free.RemoveAt(size-1);
            size--;
        }*/
        float[] posx = { -5, 7, -5, 5 };
        float[] posz = { -5, -7, 5, 5 };
        for (int i = 0; i < posx.Length; i++)
        {
            used[i].transform.position = new Vector3(posx[i], 0, posz[i]);
            used[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    public void freeObject(GameObject obj)
    {
        obj.SetActive(false);
        used.Remove(obj);
        free.Add(obj);
    }
}
