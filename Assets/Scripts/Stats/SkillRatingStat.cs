using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class SkillRatingStat : UdonSharpBehaviour
{
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private WinStat winStat;
    [SerializeField] private DeathStat deathStat;
    [SerializeField] private KillStat killStat;
    [SerializeField] private GamesPlayedStat gamesPlayedStat;
    [SerializeField] private StagesCompletedStat stageCompletedStat;
    
    [UdonSynced] private float _skillRating = 1.0F;
    
    private int _lastWins = 0;
    private int _lastDeaths = 0;
    private int _lastKills = 0;
    private int _lastGamesPlayed = 0;
    private int _lastStagesCompleted = 0;
    
    public float SkillRating => _skillRating;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        _lastWins = winStat.Wins;
        _lastDeaths = deathStat.Deaths;
        _lastKills = killStat.Kills;
        _lastGamesPlayed = gamesPlayedStat.GamesPlayed;
        _lastStagesCompleted = stageCompletedStat.StagesCompleted;
        
        display.text = _skillRating.ToString("F2");
        RequestSerialization();
    }

    public void UpdateSkillRating()
    {
        int currentWins = winStat.Wins;
        int currentDeaths = deathStat.Deaths;
        int currentKills = killStat.Kills;
        int currentGamesPlayed = gamesPlayedStat.GamesPlayed;
        int currentStagesCompleted = stageCompletedStat.StagesCompleted;
        
        Debug.Log($"{currentWins} , {currentGamesPlayed} Estimating your sr now");
        
        // Calculate deltas (what happened this game)
        int gamesPlayedDelta = currentGamesPlayed - _lastGamesPlayed;
        
        // Only update if a game was actually played
        if (gamesPlayedDelta > 0)
        {
            bool wonThisGame = (currentWins - _lastWins) > 0;
            bool diedThisGame = (currentDeaths - _lastDeaths) > 0;
            int killsThisGame = currentKills - _lastKills;
            int stagesThisGame = currentStagesCompleted - _lastStagesCompleted;
            
            // Calculate performance score for this game
            float performanceScore = CalculatePerformanceScore(wonThisGame, diedThisGame, killsThisGame, stagesThisGame);
            
            // Calculate expected performance based on current SR
            float expectedScore = CalculateExpectedScore(_skillRating);
            
            // Calculate SR change based on performance vs expectation
            float srChange = CalculateSRChange(performanceScore, expectedScore, currentGamesPlayed);
            
            // Update skill rating with bounds
            _skillRating += srChange;
            _skillRating = Mathf.Clamp(_skillRating, 0.5f, 3.5f);
            
            // Update previous stats
            _lastWins = currentWins;
            _lastDeaths = currentDeaths;
            _lastKills = currentKills;
            _lastGamesPlayed = currentGamesPlayed;
            _lastStagesCompleted = currentStagesCompleted;
        }
        
        display.text = _skillRating.ToString("F2");
        Debug.Log($"{_skillRating}");
        
        RequestSerialization();
    }
    
    private float CalculatePerformanceScore(bool won, bool died, int kills, int stagesCompleted)
    {
        float score = 0f;
        
        // Win is most important (big positive)
        if (won)
        {
            score += 1.0f;
        }
        
        // Stages completed (shows survival skill)
        // Normalize: assume ~2 stages is average, more is better
        score += Mathf.Clamp(stagesCompleted / 2f, 0f, 1.2f) * 0.8f;
        
        // Not dying is good (especially if you completed stages)
        if (!died && stagesCompleted > 0)
        {
            score += 0.4f;
        }
        
        // Kills are valuable but rare
        // Each kill is worth a decent amount
        score += kills * 0.3f;
        
        // Death penalty (but less if you made it far)
        if (died)
        {
            float deathPenalty = -0.3f;
            // Reduce penalty if completed many stages before dying
            deathPenalty += Mathf.Min(stagesCompleted * 0.05f, 0.2f);
            score += deathPenalty;
        }
        
        return score;
    }
    
    private float CalculateExpectedScore(float currentSR)
    {
        // Expected score scales with SR
        // At SR 1.0: expect ~0.5 performance
        // At SR 2.0: expect ~0.8 performance  
        // At SR 3.0: expect ~1.1 performance
        return 0.2f + (currentSR * 0.3f);
    }
    
    private float CalculateSRChange(float performance, float expected, int totalGames)
    {
        // Calculate difference between performance and expectation
        float difference = performance - expected;
        
        // K-factor: how much ratings change per game
        // Starts higher for new players, stabilizes for experienced players
        float kFactor = 0.15f;
        if (totalGames < 10)
        {
            kFactor = 0.25f; // More volatile for new players
        }
        else if (totalGames < 50)
        {
            kFactor = 0.20f; // Medium volatility
        }
        
        // SR adjustment is proportional to difference
        float srChange = difference * kFactor;
        
        // Diminishing returns at higher SR (harder to climb at top)
        if (_skillRating > 2.5f && srChange > 0)
        {
            srChange *= 0.7f;
        }
        
        // Slight rubber-banding: easier to climb from bottom
        if (_skillRating < 1.2f && srChange > 0)
        {
            srChange *= 1.2f;
        }
        
        return srChange;
    }

    public override void OnDeserialization()
    {
        display.text = _skillRating.ToString("F2");
    }
}