using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }


    }

    // Fungsi Range(min,max) -> menjaga value agar ttp berada 
    // dan max-nya

    [Range(0f,1f)]
    public float AutoCollectPersen =0.1f;
    public ResConfig[] ResConfigs;
    public Sprite[] resSprites;

    public Transform ResParent;
    public ResourceController ResPrefab;

    public TapText TapTextPrefab;

    public Transform CoinIcon;
    public Text GoldInfo;
    public Text AutoCollectInfo;

    private List<ResourceController> _activeRes = new List<ResourceController>();

    private List<TapText> _tapTextPool = new List<TapText>();
    private float _collectSec;
    public double TotalGold {get; private set;}

    

    // Start is called before the first frame update
    private void Start()
    {
        AddAllRes();
    }

    // Update is called once per frame
    private void Update()
    {
        // Fungsi untuk selalu eksekusi Collect per sec 
        _collectSec += Time.unscaledDeltaTime;
        if(_collectSec >= 1f)
        {
             CollectPerSecond();
            _collectSec = 0f;
        }

        CheckResCost();

        CoinIcon.transform.localScale = 
        Vector3.LerpUnclamped(CoinIcon.transform.localScale,Vector3.one * 2f, 0.15f);

        CoinIcon.transform.Rotate(0f,0f,Time.deltaTime*-100f);
    }
    

    private void AddAllRes()
    {
        bool showRes = true;
        foreach(ResConfig config in ResConfigs)
        {
            GameObject obj = Instantiate(ResPrefab.gameObject, ResParent, false);
            ResourceController res = obj.GetComponent<ResourceController>();
            res.SetConfig(config);
            obj.gameObject.SetActive(showRes);
            if(showRes && !res.isUnlocked)
            {
                showRes =false;
            }
            _activeRes.Add(res);
        }
    }

    public void ShowNextRes()
    {
        foreach(ResourceController res in _activeRes)
        {
            // blm aktif dgn sendirinya
            if(!res.gameObject.activeSelf)
            {
                res.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void CheckResCost()
    {
        foreach(ResourceController res in _activeRes)
        {

           // bool isBuyable = TotalGold >=res.GetUpgrade();
           bool isBuyable = false;
           if(res.isUnlocked)
           {
               // upgrade kalo dah unlock
               isBuyable = TotalGold >=res.GetUpgrade();
           }
           else
           {
               // udah cukup beli
               isBuyable = TotalGold >=res.GetUnlock();
           }
            res.ResImg.sprite = resSprites[isBuyable ? 1 : 0];
        }
    }

    private void CollectPerSecond()
    {
        double output = 0;
        foreach(ResourceController res in _activeRes)
        {
            if(res.isUnlocked)
            output +=res.GetOutput();
        }

        output*= AutoCollectPersen;
        // Fungsi ToString("F1") adalah
        // membulatkan angka menjadi desimal 
        // yg punya 1 angka diblkg koma

        AutoCollectInfo.text = $"Auto Collect: {output.ToString("F1")} / second";

        AddGold(output);
    }

    public void AddGold(double val)
    {
        TotalGold +=val;
        GoldInfo.text = $"Gold: {TotalGold.ToString("0")}";
    }

    public void CollectTap(Vector3 tapPos, Transform parent)
    {
        double output = 0;
        foreach(ResourceController res in _activeRes)
        {
            if(res.isUnlocked)
            output +=res.GetOutput();
        }

        TapText tapText = GetOrCreateTapText();
        tapText.transform.SetParent(parent,false);
        tapText.transform.position = tapPos;

        tapText.Text.text = $"+{output.ToString("0")}";
        tapText.gameObject.SetActive(true);
        CoinIcon.transform.localScale = Vector3.one * 1.75f;

        AddGold(output);
    }

    private TapText GetOrCreateTapText()
    {
        TapText tapText = _tapTextPool.Find (t=> !t.gameObject.activeSelf);
        if(tapText == null)
        {
            tapText = Instantiate(TapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }
        return tapText;
    }

}

 // Fungsi System.Serializeable adlh agar object dapat
    // di-serialize dan value dapat di-set dari inspector
    [System.Serializable]
    public struct ResConfig
    {
        public string nama;
        public double Unlock;
        public double Upgrade;
        public double Output;
    }
