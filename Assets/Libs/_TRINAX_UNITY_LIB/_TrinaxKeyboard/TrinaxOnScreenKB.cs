using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using System.Linq;

namespace TRINAX {
    public class TrinaxOnScreenKB : MonoBehaviour {
        const int MAX_MOBILE_NO = 8;
        const int MAX_NAME_CHAR = 14;

        public TrinaxAudioManager.AUDIOS[] typingSounds;
        public List<TextMeshProUGUI> keyText;

        public TMP_InputField[] inputFields;
        public Color activeColor;
        public Color inactiveColor;

        TextMeshProUGUI numberSymbolText = null;
        TMP_InputField activeField;
        HashSet<int> targetedInputFields = new HashSet<int>();

        int caretPosition;
        int anchorPosition;

        bool isShiftOn = false;
        bool isNumberSymbolOn = false;
        bool isSymbolsOn = false;

        public GameObject[] boards;
        public GameObject[] symbolBoards;

        void Start() { }

        public void Init() {
            for (int i = 0; i < inputFields.Length; ++i) {
                targetedInputFields.Add(inputFields[i].GetHashCode());
            }

            //ActivateFirstInputField();

            for (int i = 0; i < Trinax.Instance.keyboardManager.KeyboardContainer[0].keys.Count; i++) {
                keyText.Add(Trinax.Instance.keyboardManager.KeyboardContainer[0].keys[i].text);
            }

            //isShiftOn = true;
            //ChangeAlphabetCaseDisplay(isShiftOn);
            numberSymbolToggle();
            SymbolToggle();
        }

        void OnEnable() {
            ActivateFirstInputField();
        }

        void Update() {
            if (EventSystem.current.currentSelectedGameObject) {
                TMP_InputField input = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
                if (input != null) {
                    if (targetedInputFields.Contains(input.GetHashCode())) {
                        activeField = input;

                        //Debug.Log(activeField.isFocused + " " + activeField.caretPosition);
                        if (activeField.isFocused) {
                            //Debug.Log(activeField.selectionFocusPosition);
                            //Debug.Log(activeField.caretWidth);
                            //Debug.Log(activeField.selectionAnchorPosition);
                            caretPosition = activeField.caretPosition;
                            anchorPosition = activeField.selectionAnchorPosition;
                        }
                        //Debug.Log(activeField.caretPosition);
                        ColorActiveInputField();
                    }
                    else {
                        // clicked on not a targeted input field.
                    }
                }
                else {
                    for (int i = 0; i < inputFields.Length; ++i) {
                        //inputFields[i].interactable = true;
                    }
                }
            }
        }

        void ShiftToggle() {
            isShiftOn = !isShiftOn;
        }

        void ChangeAlphabetCaseDisplay(bool isUppercase) {
            if (isUppercase) {
                for (int i = 0; i < keyText.Count; i++) {
                    //if (keyText[i].tag == "alphabet_key") 
                    keyText[i].fontStyle = FontStyles.UpperCase;
                }
            }
            else {
                for (int i = 0; i < keyText.Count; i++) {
                    //if (keyText[i].tag == "alphabet_key")
                    keyText[i].fontStyle = FontStyles.LowerCase;
                }
            }
        }

        void numberSymbolToggle() {
            if (boards.Length == 0 || boards == null) return;
            for (int i = 0; i < keyText.Count; i++) {
                if (keyText[i].tag == "alphabet_number_toggle") {
                    numberSymbolText = keyText[i];
                }
            }

            isNumberSymbolOn = !isNumberSymbolOn;
            if (numberSymbolText != null) {
                if (isNumberSymbolOn) {
                    numberSymbolText.text = "#+=";

                    boards[0].SetActive(true);
                    boards[1].SetActive(false);
                }
                else {
                    numberSymbolText.text = "ABC";

                    boards[0].SetActive(false);
                    boards[1].SetActive(true);
                    if (!isSymbolsOn)
                        SymbolToggle();
                }
            }
        }

        void SymbolToggle() {
            if (symbolBoards.Length == 0 || symbolBoards == null) return;
            isSymbolsOn = !isSymbolsOn;
            if (isSymbolsOn) {
                symbolBoards[0].SetActive(true);
                symbolBoards[1].SetActive(false);
            }
            else {
                symbolBoards[0].SetActive(false);
                symbolBoards[1].SetActive(true);
            }
        }

        public void OnKeyDown(KeyCode code, string custom = "") {
            if (activeField != null) {
                string newChar = "";
                string current = activeField.text;

                if (custom.ToUpper() == "CLEAR") {
                    activeField.text = "";
                    //isShiftOn = true;
                    //ChangeAlphabetCaseDisplay(isShiftOn);
                    return;
                }
                if (custom == "alphabet_numbers") {
                    numberSymbolToggle();
                    return;
                }
                if (custom == "symbols") {
                    SymbolToggle();
                    return;
                }
                if (custom == "caps") {
                    isShiftOn = !isShiftOn;
                    ChangeAlphabetCaseDisplay(isShiftOn);
                    return;
                }

                if (code == KeyCode.Backspace) {
                    if (current.Length == 0)
                        return;
                    //activeField.caretPosition;
                    //Debug.Log(caretPosition);
                    //Debug.Log(current);
                    /*
                    if(activeField.onFocusSelectAll)
                    {
                        Debug.Log("onFocusSelectAll");
                        activeField.text = "";
                        caretPosition = 0;
                        return;
                    }
                    *
                    /*if (activeField.selectionStringFocusPosition)
                    {
                        //Debug.Log("selectionStringFocusPosition");
                        activeField.text = "";
                        caretPosition = 0;
                        return;
                    }*/
                    int startPos = 0;
                    int numOfChar = 0;
                    if (caretPosition > anchorPosition) {
                        startPos = anchorPosition;
                        numOfChar = caretPosition - startPos;
                        activeField.text = current.Remove(startPos, numOfChar);
                        anchorPosition = caretPosition = startPos;
                    }
                    else if (anchorPosition > caretPosition) {
                        startPos = caretPosition;
                        numOfChar = anchorPosition - startPos;
                        activeField.text = current.Remove(startPos, numOfChar);
                        anchorPosition = caretPosition = startPos;
                    }
                    else if (caretPosition == anchorPosition && caretPosition - 1 >= 0) {
                        //Debug.Log(caretPosition);
                        startPos = caretPosition;
                        numOfChar = 1;
                        activeField.text = current.Remove(startPos - 1, numOfChar);
                        --caretPosition;
                        --anchorPosition;
                    }

                    //Debug.Log(current);
                    //current = current.Substring(0, current.Length - 1);
                    int length = current.Length - 1;
                    if (length == 0) {
                        //isShiftOn = true;
                        //ChangeAlphabetCaseDisplay(isShiftOn);
                    }
                    return;
                }

                if (!string.IsNullOrEmpty(custom)) {
                    newChar = custom;
                    if (custom == "Yen") newChar = "¥";
                    if (custom == "Pound") newChar = "£";
                    if (custom == "Euro") newChar = "€";
                    if (custom == "BulletPoint") newChar = "•";
                }
                else {
                    switch (code) {
                        case KeyCode.Alpha0:
                            newChar = "0";
                            break;
                        case KeyCode.Alpha1:
                            newChar = "1";
                            break;
                        case KeyCode.Alpha2:
                            newChar = "2";
                            break;
                        case KeyCode.Alpha3:
                            newChar = "3";
                            break;
                        case KeyCode.Alpha4:
                            newChar = "4";
                            break;
                        case KeyCode.Alpha5:
                            newChar = "5";
                            break;
                        case KeyCode.Alpha6:
                            newChar = "6";
                            break;
                        case KeyCode.Alpha7:
                            newChar = "7";
                            break;
                        case KeyCode.Alpha8:
                            newChar = "8";
                            break;
                        case KeyCode.Alpha9:
                            newChar = "9";
                            break;
                        case KeyCode.Space:
                            newChar = " ";
                            break;
                        case KeyCode.At:
                            newChar = "@";
                            break;
                        case KeyCode.Period:
                            newChar = ".";
                            break;
                        case KeyCode.Minus:
                            newChar = "-";
                            break;
                        case KeyCode.Underscore:
                            newChar = "_";
                            break;
                        case KeyCode.Slash:
                            newChar = "/";
                            break;
                        case KeyCode.Colon:
                            newChar = ":";
                            break;
                        case KeyCode.Semicolon:
                            newChar = ";";
                            break;
                        case KeyCode.LeftParen:
                            newChar = "{";
                            break;
                        case KeyCode.RightParen:
                            newChar = "}";
                            break;
                        case KeyCode.Dollar:
                            newChar = "$";
                            break;
                        case KeyCode.Ampersand:
                            newChar = "&";
                            break;
                        case KeyCode.DoubleQuote:
                            newChar = "\"";
                            break;
                        case KeyCode.Comma:
                            newChar = ",";
                            break;
                        case KeyCode.Question:
                            newChar = "?";
                            break;
                        case KeyCode.Exclaim:
                            newChar = "!";
                            break;
                        case KeyCode.Quote:
                            newChar = "'";
                            break;
                        case KeyCode.LeftBracket:
                            newChar = "[";
                            break;
                        case KeyCode.RightBracket:
                            newChar = "]";
                            break;
                        case KeyCode.LeftCurlyBracket:
                            newChar = "{";
                            break;
                        case KeyCode.RightCurlyBracket:
                            newChar = "}";
                            break;
                        case KeyCode.Hash:
                            newChar = "#";
                            break;
                        case KeyCode.Percent:
                            newChar = "%";
                            break;
                        case KeyCode.Caret:
                            newChar = "^";
                            break;
                        case KeyCode.Asterisk:
                            newChar = "*";
                            break;
                        case KeyCode.Plus:
                            newChar = "+";
                            break;
                        case KeyCode.Equals:
                            newChar = "=";
                            break;
                        case KeyCode.Backslash:
                            newChar = "\\";
                            break;
                        case KeyCode.Pipe:
                            newChar = "|";
                            break;
                        case KeyCode.Tilde:
                            newChar = "~";
                            break;
                        case KeyCode.Less:
                            newChar = "<";
                            break;
                        case KeyCode.Greater:
                            newChar = ">";
                            break;
                        case KeyCode.LeftShift:
                            ShiftToggle();
                            ChangeAlphabetCaseDisplay(isShiftOn);
                            break;
                        default:
                            newChar = code.ToString();
                            if (string.IsNullOrEmpty(current) /*&& !activeField.CompareTag("ContactNumber")*/) {
                                if (!isShiftOn)
                                    newChar = newChar.ToLower();
                                else
                                    newChar = newChar.ToUpper();
                                //newChar = newChar.ToUpper();
                                //ShiftToggle();
                                //ChangeAlphabetCaseDisplay(isShiftOn);
                            }
                            else {
                                if (!isShiftOn)
                                    newChar = newChar.ToLower();
                                else
                                    newChar = newChar.ToUpper();
                            }
                            //else 
                            //{
                            //    if (isShiftOn) break;
                            //    char lastChar = current[current.Length - 1];
                            //    if (char.IsWhiteSpace(lastChar))
                            //    {
                            //        newChar = newChar.ToUpper();
                            //    }
                            //    else
                            //    {
                            //        newChar = newChar.ToLower();
                            //    }
                            //}
                            break;
                    }
                }

                // Validates namefield
                //if (ValidateNameField()) return;
                // Validates contactfield
                //if (ValidateContactField(newChar)) return;

                //Debug.Log(caretPosition);
                caretPosition = Mathf.Clamp(caretPosition, 0, activeField.text.Length);
                //Debug.Log(caretPosition);
                //Debug.Log(anchorPosition);
                //Debug.Log(activeField.text.Length);
                if (caretPosition == anchorPosition) {
                    string substring1 = (caretPosition > 0) ? activeField.text.Substring(0, caretPosition) : "";
                    //Debug.Log(substring1);
                    string substring2 = (caretPosition < activeField.text.Length) ?
                        activeField.text.Substring(caretPosition, activeField.text.Length - caretPosition) : "";
                    //Debug.Log(substring2);
                    activeField.text = substring1 + newChar + substring2;
                    ++caretPosition;
                    ++anchorPosition;
                }
                else {
                    int startPos = 0;
                    int endPos = 0;
                    if (caretPosition > anchorPosition) {
                        startPos = anchorPosition;
                        endPos = caretPosition;
                    }
                    else if (anchorPosition > caretPosition) {
                        startPos = caretPosition;
                        endPos = anchorPosition;
                    }
                    string substring1 = (startPos > 0) ? activeField.text.Substring(0, startPos) : "";
                    string substring2 = (endPos < activeField.text.Length) ?
                        activeField.text.Substring(endPos, activeField.text.Length - endPos) : "";

                    activeField.text = substring1 + newChar + substring2;
                    anchorPosition = caretPosition = ++startPos;
                }
            }
        }

        /// <summary>
        /// For valiating name field
        /// </summary>
        /// <returns></returns>
        bool ValidateNameField() {
            Debug.Log(activeField.tag);
            if (activeField.CompareTag("Name")) {
                if (activeField.text.Length >= MAX_NAME_CHAR) {
                    if (Mathf.Abs(caretPosition - anchorPosition) == 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// For validating contact field
        /// </summary>
        /// <param name="_char"></param>
        /// <returns></returns>
        bool ValidateContactField(string _char = "") {
            if (activeField.CompareTag("ContactNumber")) {
                string newChar = _char;
                if (activeField.text.Length >= MAX_MOBILE_NO) {
                    if (Mathf.Abs(caretPosition - anchorPosition) == 0)
                        return true;
                }
                if (!newChar.All(char.IsDigit)) return true;
            }
            return false;
        }

        void ActivateFirstInputField() {
            if (inputFields != null) {
                //inputFields[0].ActivateInputField();
                activeField = inputFields[0];
            }
            ColorActiveInputField();
        }

        void ColorActiveInputField() {
            for (int i = 0; i < inputFields.Length; ++i) {
                inputFields[i].image.color = inactiveColor;
                //inputFields[i].interactable = true;
            }

            if (activeField != null) {
                activeField.image.color = activeColor;
                //activeField.interactable = false;
            }

        }
    }
}