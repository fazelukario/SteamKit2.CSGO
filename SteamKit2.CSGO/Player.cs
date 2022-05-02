using System;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;

namespace SteamKit2.CSGO
{
    public partial class CSGOClient
    {
        /// <summary>
        ///     Request a player profile.
        /// </summary>
        /// <param name="accountId">AccountID (SteamID32) of the player.</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void PlayerProfileRequest(uint accountId, Action<CMsgGCCStrike15_v2_PlayersProfile> callback)
        {
            // For gods sake don't ask what the 32 is, i just copied it
            PlayerProfileRequest(accountId, 32, callback);
        }

        /// <summary>
        ///     Request a player profile.
        /// </summary>
        /// <param name="accountId">AccountID (SteamID32) of the player.</param>
        /// <param name="requestLevel">To be honest i have no idea what this does, default is 32</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void PlayerProfileRequest(uint accountId, uint requestLevel, Action<CMsgGCCStrike15_v2_PlayersProfile> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_PlayersProfile, msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_PlayersProfile>(msg).Body));

            if (_debug)
                Console.WriteLine($"Requesting profile for account: {accountId}");

            var clientMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientRequestPlayersProfile>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientRequestPlayersProfile)
            {
                Body =
                {
                    account_id = accountId,
                    request_level = requestLevel
                }
            };

            _gameCoordinator.Send(clientMsgProtobuf, CSGOAppId);
        }
    }
}