using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "InputValidator - Instructions.asset", menuName = "Input Validators/Instructions", order = 100)]
public class InstructionValidator: TMPro.TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch) {
        switch(ch) {
            case 'M':
                text += ch;
                pos += 1;
                return ch;
            case 'm':
                text += 'M';
                pos += 1;
                return ch;
            case 'F':
                text += 'M';
                pos += 1;
                return ch;
            case 'f':
                text += 'M';
                pos += 1;
                return ch;
            case 'R':
                text += ch;
                pos += 1;
                return ch;
            case 'r':
                text += "R";
                pos += 1;
                return ch;
            case 'L':
                text += ch;
                pos += 1;
                return ch;
            case 'l':
                text += "L";
                pos += 1;
                return ch;
            case 'S':
                text += ch;
                pos += 1;
                return ch;
            case 's':
                text += "S";
                pos += 1;
                return ch;
            case ' ':
                pos += 1;
                return ch;
        }

        return (char)0;
    }
}
