using System.Collections;
using UnityEngine;

public class Credit : MonoBehaviour
{
    public GameObject panelMain;
    public GameObject panelKredit;
    public GameObject panelBlack;

    private void OnEnable()
    {
        // Pastikan coroutine berjalan setiap kali panel aktif
        StartCoroutine(AnimationBack());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowMainPanel();
        }
    }

    private void ShowMainPanel()
    {
        panelMain.SetActive(true);
        panelKredit.SetActive(false);
        panelBlack.SetActive(false);
    }

    private IEnumerator AnimationBack()
    {
        yield return new WaitForSeconds(16f);
        ShowMainPanel();
    }
}
