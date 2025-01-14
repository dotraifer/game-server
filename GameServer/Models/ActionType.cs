using System.Runtime.Serialization;

namespace GameServer.Models;

public enum ActionType
{
    [EnumMember(Value = "Login")]
    Login,
    [EnumMember(Value = "UpdateResources")]
    UpdateResources,
    [EnumMember(Value = "SendGift")]
    SendGift
}