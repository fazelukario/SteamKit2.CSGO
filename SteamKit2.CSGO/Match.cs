using System;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;

namespace SteamKit2.CSGO
{
    public partial class CSGOClient
    {
        /// <summary>
        ///     Request MatchmakingStats from the game coordinator.
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void MatchmakingStatsRequest(Action<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingGC2ClientHello, msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>(msg).Body));

            if (_debug)
                Console.WriteLine("Requesting Matchmaking stats");

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingClient2GCHello>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello);

            _gameCoordinator.Send(clientGcMsgProtobuf, CSGOAppId);
        }

        /// <summary>
        ///     Request the list of currently live games
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestCurrentLiveGames(Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList, msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames);

            _gameCoordinator.Send(clientGcMsgProtobuf, CSGOAppId);
        }

        /// <summary>
        ///     Requests current live game info for given user.
        /// </summary>
        /// <param name="accountId">Account to request</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestLiveGameForUser(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList, msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestLiveGameForUser>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestLiveGameForUser)
            {
                Body =
                {
                    accountid = accountId
                }
            };

            _gameCoordinator.Send(clientGcMsgProtobuf, CSGOAppId);
        }

        //TODO: Find out what this does and what is required
        //TODO: Add correct packettype to Action
        //TODO: Resolve parameter types
        /*public void RequestWatchInfoFriends(dynamic request, Action callback)
        {
            new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientRequestWatchInfoFriends>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientRequestWatchInfoFriends2)
            {
                
            }
        }*/

        /// <summary>
        ///     Requests info about game given a matchId, outcomeId, and token for a game.
        /// </summary>
        /// <param name="request">Request parameters</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        [Obsolete("This hasn't been tested yet")]
        public void RequestGame(GameRequest request, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList, msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestFullGameInfo>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestFullGameInfo);

            if (request.Token.HasValue)
                clientGcMsgProtobuf.Body.token = request.Token.Value;
            if (request.MatchId.HasValue)
                clientGcMsgProtobuf.Body.matchid = request.MatchId.Value;
            if (request.OutcomeId.HasValue)
                clientGcMsgProtobuf.Body.outcomeid = request.OutcomeId.Value;

            _gameCoordinator.Send(clientGcMsgProtobuf, CSGOAppId);
        }

        /// <summary>
        ///     Requests a list of recent games for the given account id
        /// </summary>
        /// <param name="accountId">Account IDd for the request</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        [Obsolete("The accountId parameter for requestRecentGames has been deprecated.")]
        public void RequestRecentGames(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList, msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestRecentUserGames>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestRecentUserGames)
            {
                Body =
                {
                    accountid = accountId
                }
            };

            _gameCoordinator.Send(clientGcMsgProtobuf, CSGOAppId);
        }

        /// <summary>
        ///     Requests a list of recent games for the current account id
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        [Obsolete("The accountId parameter for requestRecentGames has been deprecated.")]
        public void RequestRecentGames(Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            RequestRecentGames(_steamUser.SteamID.AccountID, callback);
        }
    }

    /// <summary>
    ///     Request object for RequestGame
    /// </summary>
    public class GameRequest
    {
        /// <summary>
        ///     UNKNOWN
        /// </summary>
        public uint? Token;
        /// <summary>
        ///     ID of match
        /// </summary>
        public ulong? MatchId;
        /// <summary>
        ///     ID of outcome of match
        /// </summary>
        public uint? OutcomeId;
    }
}