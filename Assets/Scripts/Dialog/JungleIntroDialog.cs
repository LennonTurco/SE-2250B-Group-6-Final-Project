using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JungleIntroDialog : MonoBehaviour
{
    [SerializeField] private float introDelay = 0.4f;
    [SerializeField] private List<string> introLines = new List<string>
    {
        "Hide in the shadows of trees to avoid enemy detection.",
        "Enemies cannot detect you while you are hidden there.",
        "Stay away from bushes. Ninjas may be hiding in them, waiting to ambush you!",
        "Follow your current objective to find the right path forward."
    };

    private void Start()
    {
        StartCoroutine(ShowIntroDialog());
    }

    private IEnumerator ShowIntroDialog()
    {
        if (introDelay > 0f)
        {
            yield return new WaitForSeconds(introDelay);
        }

        if (DialogManager.Instance != null && introLines.Count > 0)
        {
            DialogManager.Instance.ShowDialog(introLines);
        }
    }
}
