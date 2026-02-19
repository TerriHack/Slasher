using UnityEngine;

public class Weapon : MonoBehaviour
{
    private bool isSelected;

    public void SelectThisWeapon()
    {
        isSelected = true;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void OnDieUnselect()
    {
        isSelected = false;
    }

    public void PickWeapon()
    {
        Destroy(gameObject);
    }
}
