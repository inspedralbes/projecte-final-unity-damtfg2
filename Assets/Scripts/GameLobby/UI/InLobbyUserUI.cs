using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LobbyRelaySample.UI
{
    /// <summary>
    /// When inside a lobby, this will show information about a player, whether local or remote.
    /// </summary>
    public class InLobbyUserUI : UIPanelBase
    {
        [SerializeField]
        TMP_Text m_DisplayNameText;

        [SerializeField]
        TMP_Text m_StatusText;

        [SerializeField]
        Image m_HostIcon;


        public bool IsAssigned => UserId != null;
        public string UserId { get; set; }
        LocalPlayer m_LocalPlayer;

        public void SetUser(LocalPlayer localPlayer)
        {
            Show();
            m_LocalPlayer = localPlayer;
            UserId = localPlayer.ID.Value;
            SetIsHost(localPlayer.IsHost.Value);
            SetUserStatus(localPlayer.UserStatus.Value);
            SetDisplayName(m_LocalPlayer.DisplayName.Value);
            SubscribeToPlayerUpdates();
        }

        void SubscribeToPlayerUpdates()
        {
            m_LocalPlayer.DisplayName.onChanged += SetDisplayName;
            m_LocalPlayer.UserStatus.onChanged += SetUserStatus;
            m_LocalPlayer.IsHost.onChanged += SetIsHost;
        }

        void UnsubscribeToPlayerUpdates()
        {
            if (m_LocalPlayer == null)
                return;
            if (m_LocalPlayer.DisplayName?.onChanged != null)
                m_LocalPlayer.DisplayName.onChanged -= SetDisplayName;
            if (m_LocalPlayer.UserStatus?.onChanged != null)
                m_LocalPlayer.UserStatus.onChanged -= SetUserStatus;
            if (m_LocalPlayer.IsHost?.onChanged != null)
                m_LocalPlayer.IsHost.onChanged -= SetIsHost;
        }

        void SetDisplayName(string displayName)
        {
            m_DisplayNameText.SetText(displayName);
        }

        void SetUserStatus(PlayerStatus statusText)
        {
            m_StatusText.SetText(SetStatusFancy(statusText));
        }

        void SetIsHost(bool isHost)
        {
            m_HostIcon.enabled = isHost;
        }


        string SetStatusFancy(PlayerStatus status)
        {
            switch (status)
            {
                case PlayerStatus.Lobby:
                    return "<color=#56B4E9>In Lobby</color>"; // Light Blue
                case PlayerStatus.Ready:
                    return "<color=#009E73>Ready</color>"; // Light Mint
                case PlayerStatus.Connecting:
                    return "<color=#F0E442>Connecting...</color>"; // Bright Yellow
                case PlayerStatus.InGame:
                    return "<color=#005500>In Game</color>"; // Green
                default:
                    return "";
            }
        }
    }
}