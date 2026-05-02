using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Basic NPC interaction that can be extended with diffrerent systems if needed.
/// This basic interaction was made in less than 5 minutes using the IInteractable interface
/// </summary>
public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private TMP_Text _txtDialog;

    public void Interact(Interactor interactor)
    {
        _txtDialog.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(IHideDialog());
    }

    private IEnumerator IHideDialog()
    {
        yield return new WaitForSeconds(2f);
        _txtDialog.gameObject.SetActive(false);
    }
}
