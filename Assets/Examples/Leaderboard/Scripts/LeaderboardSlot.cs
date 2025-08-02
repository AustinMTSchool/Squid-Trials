using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class LeaderboardSlot : UdonSharpBehaviour
{
    [SerializeField] private Leaderboard leaderboard;
    
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private float _score = -1;

    private void Start()
    {
        leaderboard.SetupSlot(this);
        nameText.text = GetPlayer().displayName;
    }

    public void SetScore(float newScore, int decimals)
    {
        _score = newScore;
        scoreText.text = newScore.ToString($"F{decimals}");
    }
    
    public void SetPosition(int newPosition)
    {
        positionText.text = newPosition.ToString();
    }
    
    public VRCPlayerApi GetPlayer()
    {
        return Networking.GetOwner(gameObject);
    }
    
    public float GetScore()
    {
        return _score;
    }
    
    public void OnDestroy()
    {
        leaderboard.SendCustomEventDelayedFrames(nameof(leaderboard.RefreshLeaderboard),1);
    }
}