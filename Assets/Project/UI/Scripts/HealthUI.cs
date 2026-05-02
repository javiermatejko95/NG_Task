using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image _healthFillImage;

    private int _currentHealth;
    private int _maxHealth;

    public void Setup(int maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
    }

    public void UpdateHealth(int currentHealth)
    {
        _currentHealth = currentHealth;

        float newValue = (float)currentHealth / (float)_maxHealth;

        _healthFillImage.fillAmount = newValue;
    }
}
