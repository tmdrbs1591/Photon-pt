using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
   [System.Serializable]
   public class StartPositionArray
    {
        public List<Transform> StartPosition = new List<Transform>();

    }

    public StartPositionArray[] startPositionArrays;
}
