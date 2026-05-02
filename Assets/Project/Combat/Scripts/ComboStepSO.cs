using UnityEngine;

[CreateAssetMenu(fileName = "ComboData", menuName = "Data/Combo")]
public class ComboStepSO : ScriptableObject
{
    [SerializeField] private string[] _triggersName;

    public string[] TriggerName { get => _triggersName; }
}
