using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    // isi
    public Button ResButton;
    public Image ResImg;

    public Text ResDesc;
    public Text ResUpgrade;
    public Text ResUnlock;

    private ResConfig _config;

    private int _level = 1;

    public bool isUnlocked {get; private set;}


    private void Start()
    {
        //ResButton.onClick.AddListener(UpgradeLevel);
        ResButton.onClick.AddListener(()=> {
            if(isUnlocked) UpgradeLevel();
            else UnlockRes(); 
        });
    }

    public void SetConfig(ResConfig config)
    {
        _config = config;

        // ToString("0") berfungsi utk buang angka belakang koma

        ResDesc.text = $"{ _config.nama } Lv. { _level }\n+{ GetOutput ().ToString ("0") }";

        ResUnlock.text = $"Unlock Cost\n{GetUnlock()}";

        ResUpgrade.text = $"Upgrade Cost\n{GetUpgrade()}";

        SetUnlocked(_config.Unlock == 0);
    }

    // Get Method
    public double GetOutput()
    {
        return _config.Output * _level;
    }
    public double GetUpgrade()
    {
        return _config.Upgrade * _level;
    }
    public double GetUnlock()
    {
        return _config.Unlock;
    }

    public void UpgradeLevel()
    {
        double upCost = GetUpgrade();
        // Apabila totalgold masih lebih rendah dari upCost
        if(GameManager.Instance.TotalGold < upCost)
        {
            return;
        }

        // berhasil Upgrade
        GameManager.Instance.AddGold(-upCost);
        _level++;

        ResUpgrade.text =$"Upgrade Cost\n{GetUpgrade()}";
        ResDesc.text = $"{_config.nama} Lv. {_level}\n+ {GetOutput().ToString("0")}";
    }

    public void UnlockRes()
    {
        double UnlockCost = GetUnlock();
        // jika ngga cukup
        if(GameManager.Instance.TotalGold < UnlockCost)
        {
            return;
        }

        SetUnlocked(true);
        GameManager.Instance.ShowNextRes();

        AchievementController.Instance.UnlockAchieve(Achievetype.UnlockRes,_config.nama);
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        ResImg.color = isUnlocked? Color.white : Color.grey;
        ResUnlock.gameObject.SetActive(!unlocked);
        ResUpgrade.gameObject.SetActive(unlocked);
    }


}
