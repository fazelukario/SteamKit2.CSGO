using System;
using System.Timers;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;

namespace SteamKit2.CSGO
{
    /// <summary>
    ///     Client for CSGO, allows basic operations such as requesting ranks
    /// </summary>
    public partial class CSGOClient
    {
        //APP ID for csgo
        private const int CSGOAppId = 730;
        private readonly bool _debug;
        private readonly SteamGameCoordinator _gameCoordinator;

        //Contains the callbacks
        private readonly CallbackStore _gcMap = new CallbackStore();

        private readonly SteamClient _steamClient;
        private readonly SteamUser _steamUser;

        private readonly Timer HelloTimer;

        #region Constructor

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="steamClient">A logged in SteamKit2 SteamClient</param>
        /// <param name="callbackManager">The callback manager you used in your log in code</param>
        /// <param name="debug">Wether or not we want to have debug output</param>
        public CSGOClient(SteamClient steamClient, CallbackManager callbackManager, bool debug = false)
        {
            _steamClient = steamClient;
            _steamUser = steamClient.GetHandler<SteamUser>();
            _gameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            _debug = debug;

            callbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGcMessage);

            HelloTimer = new Timer(1000)
            {
                AutoReset = true
            };
            HelloTimer.Elapsed += Knock;
        }

        private void Knock(object state, ElapsedEventArgs elapsedEventArgs)
        {
            if (_debug)
                Console.WriteLine("Knocking");
            var clientmsg = new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello);
            _gameCoordinator.Send(clientmsg, CSGOAppId);
        }

        #endregion

        private void OnGcMessage(SteamGameCoordinator.MessageCallback obj)
        {
            if (_debug)
                Console.WriteLine($"GC Message: {Enum.GetName(typeof(ECsgoGCMsg), obj.EMsg) ?? Enum.GetName(typeof(EMsg), obj.EMsg)}");

            if (obj.EMsg == (uint)EGCBaseClientMsg.k_EMsgGCClientWelcome)
                HelloTimer.Stop();

            if (!_gcMap.TryGetValue(obj.EMsg, out Action<IPacketGCMsg> func))
                return;

            func(obj.Message);
        }

        /// <summary>
        ///     Launches the game
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes</param>
        public void Launch(Action<CMsgClientWelcome> callback)
        {
            _gcMap.Add((uint)EGCBaseClientMsg.k_EMsgGCClientWelcome,
                msg => callback(new ClientGCMsgProtobuf<CMsgClientWelcome>(msg).Body));

            if (_debug)
                Console.WriteLine("Launching CSGO");

            var playGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

            playGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = CSGOAppId
            });

            _steamClient.Send(playGame);

            HelloTimer.Start();
        }
    }
}