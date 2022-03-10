using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    public HexGrid grid;

    public enum GameState { Startup, Game }
    public enum RoundState { Start, Moving, BonusMove, EndOfRound, Replay }

    public TMPro.TMP_Dropdown ShipSelectionDropdown;
    public TMPro.TMP_Text ShipSelectionLabel;
    public TMPro.TMP_Text ShipCreationInput;
    public TMPro.TMP_Text ShipCreationPosInput;
    public Toggle ShipCreationFactionToggle;
    public GameObject StartGameButton;
    public Canvas GameUI;
    public GameObject ShipInstructionPrefab;
    public Button GameBackButton;
    public Button GameNextButton;

    private float GameUIShipInstructionOffset = -90f;
    private float GameUIShipInstructionHeight = 50f;
    private GameState currentState = GameState.Startup;
    private RoundState roundState = RoundState.Start;
    private int instructionStep = 0;

    private int stepsPerRound = 5;
    private int bonusSteps = -1;
    private bool simpleMode = true;
    private Vector3 lastManualPosition;

    Dictionary<string, Ship> ships = new Dictionary<string, Ship>();
    Dictionary<string, ShipInstruction> instructions = new Dictionary<string, ShipInstruction>();

    // Start is called before the first frame update
    void Start()
    {
        grid.SetDirectControls(true);
        lastManualPosition = transform.position;
    }

    public void StartGame() {
        currentState = GameState.Game;
        grid.SetDirectControls(false);
        float positionY = GameUIShipInstructionOffset;
        foreach (Ship ship in ships.Values) {
            GameObject instructionObj = Instantiate<GameObject>(ShipInstructionPrefab, GameUI.transform);
            instructionObj.transform.localPosition = new Vector3(200, positionY, 0);
            ShipInstruction instruction = instructionObj.GetComponent<ShipInstruction>();
            instruction.createShip(ship, this);
            positionY -= GameUIShipInstructionHeight;
            instructions.Add(ship.Name, instruction);
        }
    }

    public void AddShip(Canvas canvas) {
        bool isPlayer = ShipCreationFactionToggle.isOn;
        string name = ShipCreationInput.text;
        string pos = ShipCreationPosInput.text;
        if (ships.ContainsKey(name)) {
            Debug.LogError("Ship with that name already exists");
            return;
        }
        ShipSelectionDropdown.AddOptions(new List<string>() { name });
        Ship ship = grid.AddShip(name, isPlayer, pos);
        ships.Add(name, ship);
        if (!StartGameButton.activeSelf) {
            bool friend = false;
            bool foe = false;
            foreach (Ship s in ships.Values) {
                if (s.IsPlayer) {
                    friend = true;
                } else {
                    foe = true;
                }
            }
            if (friend && foe) {
                StartGameButton.SetActive(true);
            }
        }
    }

    public void GameBack() {
        if(simpleMode) {
            GameBackSimple();
        } else {
            switch(roundState) {
                case RoundState.Start:
                    break;
                case RoundState.Moving:
                    GameBackMove();
                    break;
                case RoundState.BonusMove:
                    GameBackBonusMove();
                    break;
                case RoundState.EndOfRound:
                    break;
                case RoundState.Replay:
                    break;
                default:
                    roundState = RoundState.Start;
                    break;
            }
        }
    }

    public void GameNext() {
        if(simpleMode) {
            GameNextSimple();
        } else {
            switch(roundState) {
                case RoundState.Start:
                    GameNextStart();
                    break;
                case RoundState.Moving:
                    GameNextMove();
                    break;
                case RoundState.BonusMove:
                    GameNextBonusMove();
                    break;
                case RoundState.EndOfRound:
                    break;
                case RoundState.Replay:
                    break;
                default:
                    roundState = RoundState.Start;
                    break;
            }
        }
    }

    private void GameBackSimple() {
        if(instructionStep <= 0) {
            Debug.LogError("First step. Cannot go back");
            return;
        }
        if(instructionStep >= stepsPerRound) {
            // End of Round. Lock Instructions
            GameNextButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Next";
            foreach(KeyValuePair<string, ShipInstruction> pair in instructions) {
                pair.Value.LockInstructions();
            }
        }
        instructionStep -= 1;
        foreach(KeyValuePair<string, ShipInstruction> pair in instructions) {
            string name = pair.Key;
            ShipInstruction ins = pair.Value;
            Ship ship = ships[name];
            ins.ResetInfo();
            char move = ins.Step(instructionStep); // Get current step
            switch(move) {
                case 'M':
                    ship.backward();
                    break;
                case 'L':
                    ship.right();
                    break;
                case 'R':
                    ship.left();
                    break;
                default:
                    break;
            }
            ins.Step(instructionStep - 1); // Highlight back step
        }
        Debug.LogWarning("Step " + instructionStep + " of " + stepsPerRound);
        CheckHits();
        if(instructionStep <= 0) {
            // start of Round. Unlock Instructions
            GameBackButton.interactable = false;
            foreach(KeyValuePair<string, ShipInstruction> pair in instructions) {
                pair.Value.UnlockInstructions(false);
            }
        }
    }

    private void GameNextSimple() {
        if(instructionStep >= stepsPerRound) {
            foreach(KeyValuePair<string, ShipInstruction> pair in instructions) {
                pair.Value.UnlockInstructions(true);
            }
            GameNextButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Next";
            instructionStep = 0;
            GameBackButton.interactable = false;
            return;
        }
        if(instructionStep <= 0) {
            // Start of Round. Lock Instructions
            stepsPerRound = -1;
            foreach(KeyValuePair<string, ShipInstruction> pair in instructions) {
                pair.Value.LockInstructions();
                stepsPerRound = Mathf.Max(pair.Value.StepCount(), stepsPerRound);
            }
            if(stepsPerRound > 0) {
                GameBackButton.interactable = true;
            } else {
                stepsPerRound = 5;
                foreach(KeyValuePair<string, ShipInstruction> pair in instructions) {
                    pair.Value.UnlockInstructions(true);
                    pair.Value.ResetInfo();
                    pair.Value.AddInfo("No instructions found");
                }
                return;
            }
        }
        instructionStep += 1;
        foreach(KeyValuePair<string, ShipInstruction> pair in instructions) {
            string name = pair.Key;
            ShipInstruction ins = pair.Value;
            Ship ship = ships[name];
            ins.ResetInfo();
            char move = ins.Step(instructionStep - 1);
            switch(move) {
                case 'M':
                    ship.forward();
                    break;
                case 'L':
                    ship.left();
                    break;
                case 'R':
                    ship.right();
                    break;
                default:
                    break;
            }
        }
        CheckHits();
        Debug.LogWarning("Step " + instructionStep + " of " + stepsPerRound);
        if(instructionStep >= stepsPerRound) {
            // End of Round. Unlock Instructions
            GameNextButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Next Round";
        }
    }

    private void GameNextStart() {
        int stepCount = -1;
        foreach(ShipInstruction ins in instructions.Values) {
            if(!ins.IsFilled()) {
                Debug.LogError("Empty instructions");
                return;
            }
            if(!ins.IsValid()) {
                Debug.LogError("Invalid instructions");
                return;
            }
            if(stepCount < 0) {
                stepCount = ins.StepCount();
            } else if(stepCount != ins.StepCount()) {
                Debug.LogError("Invalid instructions");
                return;
            }
        }
        Debug.LogWarning("Starting Round");
        foreach(ShipInstruction ins in instructions.Values) {
            ins.LockInstructions();
        }
        roundState = RoundState.Moving;
        instructionStep = 0;
        GameNext();
    }

    private void GameNextMove() {
        GameBackButton.interactable = true;
        if(instructionStep >= stepsPerRound) {
            ToBonusRound();
        } else {
            instructionStep += 1;
            Debug.LogWarning("Round " + instructionStep);
            foreach(KeyValuePair<string, ShipInstruction> pair in instructions) {
                string name = pair.Key;
                ShipInstruction ins = pair.Value;
                Ship ship = ships[name];
                char move = ins.Step(instructionStep - 1);
                switch(move) {
                    case 'M':
                        ship.forward();
                        break;
                    case 'L':
                        ship.left();
                        break;
                    case 'R':
                        ship.right();
                        break;
                    default:
                        break;
                }
            }
            CheckHits();
        }
    }

    private void ToBonusRound() {
        roundState = RoundState.BonusMove;
        instructionStep = 0;
        bonusSteps = -1;
        foreach(ShipInstruction ins in instructions.Values) {
            ins.UnlockInstructions(true);
        }
    }

    private void GameNextBonusMove() {
        GameBackButton.interactable = true;
        if(instructionStep == stepsPerRound) {

        } else {
            instructionStep += 1;
            Debug.LogWarning("Round " + instructionStep);
            if(instructionStep == 5) {
                roundState = RoundState.BonusMove;
                instructionStep = 0;
            } else {
                foreach(ShipInstruction ins in instructions.Values) {
                    ins.Step(instructionStep - 1);
                }
            }
            CheckHits();
        }
    }

    private void GameBackMove() {
        if(instructionStep == 0) {
            roundState = RoundState.Start;
            GameBackButton.interactable = false;
        } else {
            instructionStep -= 1;
            if(instructionStep == 0) {
                GameBackButton.interactable = false;
                roundState = RoundState.Start;
            }
        }
    }
    private void GameBackBonusMove() {

    }

    private void CheckHits() {
        foreach (Ship ship in ships.Values) {
            foreach(Ship other in ships.Values) {
                if (ship != other) {
                    Vector3 forward = Vector3.zero;
                    forward.x += HexMetrics.getFactor();
                    forward.z += 0.5f;
                    forward = Quaternion.AngleAxis(ship.transform.rotation.eulerAngles.y, Vector3.up) * forward;

                    Vector3 directionToTarget = (other.transform.position - ship.transform.position).normalized;

                    float dot = Vector3.Dot(forward, directionToTarget);
                    float dist = Vector3.Distance(ship.transform.position, other.transform.position);
                    int facing = (int)(dot * 100);
                    int distance = (int)(dist * 100);
                    Debug.LogError(ship.Name + " > " + other.Name + " = F:" + facing + " D:" + distance);
                    if(dist < 4f) {
                        Debug.LogError(ship.Name + " is in range of hitting " + other.Name);
                        if(dot > 0.8f) {
                            Debug.LogError(" - and is facing. HIT! ");
                            ship.instructions.AddInfo("Hitting " + other.Name);
                        } else if (dist < 2.1f) {
                            Debug.LogError(" - and cores are touching. HIT! ");
                            ship.instructions.AddInfo("Hitting " + other.Name);
                        } else if (dist < 3f) {
                            if (facing > 0) {
                                Debug.LogError(" - and wing is touching. HIT! ");
                                ship.instructions.AddInfo("Hitting " + other.Name);
                            }
                        }
                    }
                }
            }
        }
    }

    private void StartUpGUI() {
        GUILayout.Label("Startup phase. Move ships onto starting position");
        if(GUILayout.Button("End Startup")) {
            StartGame();
        }
    }

    private Ship getShip() {
        return ships[ShipSelectionLabel.text];
    }

    public void Forward() {
        Ship ship = getShip();
        if(ship != null) {
            ship.forward();
        }
    }

    public void Right() {
        Ship ship = getShip();
        if(ship != null) {
            ship.right();
        }
    }

    public void Left() {
        Ship ship = getShip();
        if(ship != null) {
            ship.left();
        }
    }

    public void Backward() {
        Ship ship = getShip();
        if(ship != null) {
            ship.backward();
        }
    }

    public void MoveCamera(Ship ship) {
        if(ship == null) {
            transform.position = lastManualPosition;
        } else {
            Vector3 position = ship.transform.position;
            position.y = 0;
            position.z -= 12;
            transform.position = position;
        }
    }
}
