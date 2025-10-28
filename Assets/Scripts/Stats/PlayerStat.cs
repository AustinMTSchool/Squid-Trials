
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerStat : UdonSharpBehaviour
{
    [SerializeField] private PlayerStatsManagers playerStatsManagers;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private SkillRatingStat skillRatingStat;
    [SerializeField] private LevelStat levelStat;
    [SerializeField] private WinStat winStat;
    [SerializeField] private KillStat killStat;
    [SerializeField] private PointStat pointStat;
    [SerializeField] private DeathStat deathStat;
    [SerializeField] private StagesCompletedStat stageCompletedStat;
    [SerializeField] private GamesPlayedStat gamesPlayedStat;
    [SerializeField] private TextMeshProUGUI rankDisplay;
    
    public SkillRatingStat SkillRatingStat => skillRatingStat;
    public LevelStat LevelStat => levelStat;
    public WinStat WinStat => winStat;
    public KillStat KillStat => killStat;
    public PointStat PointStat => pointStat;
    public DeathStat DeathStat => deathStat;
    public StagesCompletedStat StageCompletedStat => stageCompletedStat;
    public GamesPlayedStat GamesPlayedStat => gamesPlayedStat;

    private void Start()
    {
        playerStatsManagers.SetupSlot(this);
        nameText.text = GetPlayer().displayName;
    }

    public void SetPosition(int newPosition)
    {
        rankDisplay.text = newPosition.ToString();
    }
    
    public VRCPlayerApi GetPlayer()
    {
        return Networking.GetOwner(gameObject);
    }
    
    public void OnDestroy()
    {
        playerStatsManagers.SendCustomEventDelayedFrames(nameof(playerStatsManagers.RefreshStats),1);
    }
}
