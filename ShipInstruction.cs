using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ShipInstruction : MonoBehaviour
{
    public string Name { get;  private set; }
    public TMPro.TMP_Text NameText;
    public TMPro.TMP_Text InstructionInputfieldText;
    public TMPro.TMP_Text InfoText;
    public TMPro.TMP_InputField InstructionInputfield;
    public TMPro.TMP_InputValidator validator;

    public List<String> round = new List<string>();

    private GamePlay gamePlay;
    private Ship ship;

    public void createShip(Ship ship, GamePlay gamePlay) {
        Name = ship.Name;
        NameText.text = Name + ":";
        this.ship = ship;
        this.gamePlay = gamePlay;
        ship.instructions = this;
        ResetInfo();
    }

    internal bool IsValid() {
        return Regex.IsMatch(InstructionInputfieldText.text, @"[MRLSmrls]+");
    }

    internal bool IsFilled() {
        return InstructionInputfieldText.text != "";
    }

    internal int StepCount() {
        return InstructionInputfieldText.text.Replace(" ", String.Empty).Length - 1;
    }

    public void LockInstructions() {
        InstructionInputfield.readOnly = true;
        InstructionInputfield.inputValidator = null;
    }

    public void UnlockInstructions(Boolean delete) {
        InstructionInputfield.readOnly = false;
        //InstructionInputfieldText.SetText("");
        if(delete) {
            //InstructionInputfield.SetTextWithoutNotify("");
            InstructionInputfield.text = "";
        }
        InstructionInputfield.inputValidator = validator;
    }

    public char Step(int step) {
        string textOrigin = Regex.Replace(InstructionInputfieldText.text, @"[^MRLS]", String.Empty);
        string text = "";
        char move = 'S';
        for (int i = 0; i < textOrigin.Length; i++) {
            if (i == step) {
                move = textOrigin[i];
                text += " >" + textOrigin[i] + "< ";
            } else {
                text += textOrigin[i];
            }
        }
        Debug.LogError(Name + " - origin Lenth: " + textOrigin.Length);
        Debug.LogError(" ---- post Lenth: " + text.Length);
        //InstructionInputfieldText.SetText(text);
        //InstructionInputfield.SetTextWithoutNotify(text);
        InstructionInputfield.text = text;
        return move;
    }

    public void Selected() {
        gamePlay.MoveCamera(ship);
    }

    public void Deselected() {
        gamePlay.MoveCamera(null);
    }

    public void ResetInfo() {
        InfoText.text = ship.tcoord.ToString() + HexMetrics.mod(ship.orientation + 60, 360).ToString();
    }
    public void AddInfo(String text) {
        string oldText = InfoText.text;
        if(oldText == String.Empty) {
            InfoText.text = text;
        } else {
            InfoText.text = oldText + ", " + text;
        }
    }
}
