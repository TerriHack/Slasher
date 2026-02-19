using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    private GameObject _currentWeapon;
    
    
    void Start()
    {
        SelectWeapon();
    }

    private void SelectWeapon()
    {
        int selectedIndex = Random.Range(0, weapons.Length-1);
        
        _currentWeapon = weapons[selectedIndex];
        Instantiate(_currentWeapon, transform);
    }
}
