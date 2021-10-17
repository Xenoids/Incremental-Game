using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{

    // Instance mirip kek GameManager
    // Fungsi -> buat sistem singleton
    // untuk memudahkan panggil script yang bersifat
    // manager dari script lain

    private static AchievementController _instance = null;
    public static AchievementController Instance
    {
        get{
            if(_instance == null)
            {
                _instance = FindObjectOfType<AchievementController>();
            }
            return _instance;
        }
    }


    [SerializeField] private Transform _popUpTrans;
    [SerializeField] private Text _popUptext;

    [SerializeField] private float _popUpShowDuration = 3f;
    
    [SerializeField] private List<AchievementData> _achievementList;

    private float _popUpShowDurationCount;


    // Update is called once per frame
    private void Update()
    {
        if(_popUpShowDurationCount > 0)
        {
            // Kurangi durasi popup lebih dari 0
            _popUpShowDurationCount -= Time.unscaledDeltaTime;

            // Lerp -> fungsi linear interploation,
            // utk mengubah value perlahan

            _popUpTrans.localScale = Vector3.LerpUnclamped(_popUpTrans.localScale,Vector3.one,0.5f);

        }
        else{
            _popUpTrans.localScale = Vector2.LerpUnclamped(_popUpTrans.localScale,Vector3.right,0.5f);
        }
    }

    public void UnlockAchieve(Achievetype type, string val)
    {
        // cari data achieve
        AchievementData achieve = _achievementList.Find(a=>a.Type == type && a.Val == val);
        
        
        // apabila blm diachieve, unlock dan tampilkan
        if(achieve != null && !achieve.isUnlocked)
        {
            achieve.isUnlocked = true;
            ShowAchievePopUp(achieve);
        }
    }

    public void ShowAchievePopUp(AchievementData achieve)
    {
        _popUptext.text = achieve.title;
        _popUpShowDurationCount = _popUpShowDuration;
        _popUpTrans.localScale = Vector2.right;
    }
}


// System.Serializable digunakan agar obj dri script dpt diserialize
// dan dapat diinput dri inspector, jika tidak ada ini,
// variable ga akan muncul di inspector;

[System.Serializable]
public class AchievementData
{
    public string title;

    public Achievetype Type;

    public string Val;

    public bool isUnlocked;

}

public enum Achievetype
{
    UnlockRes
}

