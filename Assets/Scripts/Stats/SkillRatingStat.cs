
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SkillRatingStat : UdonSharpBehaviour
{
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private WinStat winStat;
    [SerializeField] private DeathStat deathStat;
    [SerializeField] private KillStat killStat;
    [SerializeField] private GamesPlayedStat gamesPlayedStat;
    [SerializeField] private StagesCompletedStat stageCompletedStat;
    
    [UdonSynced] private float _skillRating = 1.0F;
    
    public float SkillRating => _skillRating;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        display.text = _skillRating.ToString();
        RequestSerialization();
    }

    public void UpdateSkillRating()
    {
        // TODO finish this
        display.text = _skillRating.ToString();
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        display.text = _skillRating.ToString();
    }
}
