using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAdjudicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
#if ENABLE_INPUT_SYSTEM
        GetComponentInParent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>().enabled = true;
        GetComponentInParent<UnityEngine.EventSystems.StandaloneInputModule>().enabled = false;
#else
        // GetComponentInParent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>().enabled = false;
        GetComponentInParent<UnityEngine.EventSystems.StandaloneInputModule>().enabled = true;
#endif
    }

}
