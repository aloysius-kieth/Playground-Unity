using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

[System.Serializable]
public class _PlayerInfoDisplay
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public Image rankImage;
}
public class PlayerInfoDisplay : MonoBehaviour
{
    public _PlayerInfoDisplay playerInfoDisplay;
    public GameObject highlight;
}
