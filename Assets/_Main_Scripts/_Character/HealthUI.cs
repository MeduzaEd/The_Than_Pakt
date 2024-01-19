using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private Humanoid _Humanoid;
    public Image _HealthUI;
    private void LateUpdate()
    {
        if (_HealthUI == null) { return; }
        if (_Humanoid == null) { try { _Humanoid = this.transform.parent.GetComponent<Humanoid>(); } catch { return; } }
        //Debug.Log(_HealthUI.rectTransform.right);
        float _HealthValue = 30f * Mathf.Clamp(((float)_Humanoid.Health.Value / (float)_Humanoid.MaxHealth.Value), 0, 1f);
        _HealthUI.rectTransform.sizeDelta =new Vector2(Mathf.Lerp(_HealthUI.rectTransform.rect.width,_HealthValue,0.1f), 2.75f);
    }
}
