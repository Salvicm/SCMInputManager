using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SCM_InputManager))]
public class SCM_Input_Tester : MonoBehaviour
{
    [SerializeField] private SCM_InputManager manag;
    string text;
    void Start()
    {
        manag = this.gameObject.GetComponent<SCM_InputManager>();
    }
    private void Update()
    {
        text = manag.controller.ToString() + "\n";


        if (manag.GetPrimaryAction_A()) { text += "A1, ";  }
        if (manag.GetPrimaryAction_B()) { text += "B1, ";  }
        if (manag.GetPrimaryAction_X()) { text += "X1, ";  }
        if (manag.GetPrimaryAction_Y()) { text += "Y1, ";  }
        if (manag.GetSecondaryAction_A()) { text += "A2, "; }
        if (manag.GetSecondaryAction_B()) { text += "B2, "; }
        if (manag.GetSecondaryAction_X()) { text += "X2, "; }
        if (manag.GetSecondaryAction_Y()) { text += "Y2, "; }
        if (manag.GetTertiaryAction_A()) { text += "A3, ";  }
        if (manag.GetTertiaryAction_B()) { text += "B3, ";  }
        if (manag.GetTertiaryAction_X()) { text += "X3, ";  }
        if (manag.GetTertiaryAction_Y()) { text += "Y3, ";  }
        if (manag.GetPauseAction()) { text += "Pause, ";  }
        if (manag.GetSelectAction()) { text += "Select, "; }
        if (manag.GetPrimaryStick()) { text += "Stick1, "; }
        if (manag.GetSecondaryStick()) { text += "Stick2, "; }
        text += "\n";
        if (manag.GetPrimaryAction_A_Continuous()) { text += "A1C, "; }
        if (manag.GetPrimaryAction_B_Continuous()) { text += "B1C, "; }
        if (manag.GetPrimaryAction_X_Continuous()) { text += "X1C, "; }
        if (manag.GetPrimaryAction_Y_Continuous()) { text += "Y1C, "; }
        if (manag.GetSecondaryAction_A_Continuous()) { text += "A2C, "; }
        if (manag.GetSecondaryAction_B_Continuous()) { text += "B2C, "; }
        if (manag.GetSecondaryAction_X_Continuous()) { text += "X2C, "; }
        if (manag.GetSecondaryAction_Y_Continuous()) { text += "Y2C, "; }
        if (manag.GetTertiaryAction_A_Continuous()) { text += "A3C, "; }
        if (manag.GetTertiaryAction_B_Continuous()) { text += "B3C, "; }
        if (manag.GetTertiaryAction_X_Continuous()) { text += "X3C, "; }
        if (manag.GetTertiaryAction_Y_Continuous()) { text += "Y3C, "; }
        if (manag.GetPauseAction_Continuous()) { text += "PauseC, "; }
        if (manag.GetSelectAction_Continuous()) { text += "SelectC, "; }
        if (manag.GetPrimaryStick_Continuous()) { text += "Stick1C, "; }
        if (manag.GetSecondaryStick_Continuous()) { text += "Stick2C, "; }

        text += "\nTriggers: ";
        text += manag.GetLeftTrigger().ToString() + ", ";
        text += manag.GetRightTrigger().ToString() + ", ";
        text += "\nPrimary stick: ";
        text += manag.GetPrimaryMovX().ToString() + ", ";
        text += manag.GetPrimaryMovY().ToString() + ", ";
        text += "\nSecondary stick: ";
        text += manag.GetSecondaryMovX().ToString() + ", ";
        text += manag.GetSecondaryMovY().ToString() + ", ";
        text += "\nStick states: ";
        text += manag.GetPrimaryStickState().ToString() + ", ";
        text += manag.GetSecondaryStickState().ToString() + ", ";

        text += "\nStick Angles: ";
        text += manag.GetPrimaryStickAngle().ToString() + ", ";
        text += manag.GetSecondaryStickAngle().ToString() + ", ";

        text += "\nDpad state: ";
        text += manag.GetDpadState().ToString() + ", ";

        text += "\nMovement X and Y: ";
        text += manag.getMovementX().ToString() + ", ";
        text += manag.getMovementY().ToString() + ", ";

       
    }
    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle(); //create a new style variable
        guiStyle.fontSize = 35;
        guiStyle.normal.textColor = Color.yellow;
        // GUI.contentColor = Color.black;
        
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text, guiStyle);
        
    }


}
