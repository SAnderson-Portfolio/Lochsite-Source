using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData{
    public int level;
    public int[] artefactsUnlocked;

    public PlayerData (GameManager gm)
    {
        
    }

    
    static void SaveGame()
    {
        int[] unlocked = GameManager.instance.GetArtefactUnlockList();

        PlayerPrefs.SetInt(GameManager.Artefacts.Butterdish.ToString(), unlocked[(int)GameManager.Artefacts.Butterdish]);
        PlayerPrefs.SetInt(GameManager.Artefacts.Ore.ToString(), unlocked[(int)GameManager.Artefacts.Ore]);
        PlayerPrefs.SetInt(GameManager.Artefacts.Knife.ToString(), unlocked[(int)GameManager.Artefacts.Knife]);
        PlayerPrefs.SetInt(GameManager.Artefacts.Lyre.ToString(), unlocked[(int)GameManager.Artefacts.Lyre]);
        PlayerPrefs.SetInt(GameManager.Artefacts.Textile.ToString(), unlocked[(int)GameManager.Artefacts.Textile]);
    }


}
