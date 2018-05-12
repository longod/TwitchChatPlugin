using System;
using System.Collections.Concurrent;
using System.Threading;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;

namespace TwitchChatPlugin {
    /// <summary>
    /// Client of TwitchChat
    /// </summary>
    internal class ChatClient {
        internal ChatClient( string username, string oauth, double latency, string proxyIP = null, int? proxyPort = null, bool dryRun = false, ILogger logger = null ) {
            crentials = new ConnectionCredentials(
                twitchUsername: username,
                twitchOAuth: oauth,
                proxyIP: proxyIP,
                proxyPort: proxyPort );
            client = new TwitchClient( crentials, username, logging: false );  // channnel is same as username.
            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;
            client.OnLog += Client_OnLog;
            client.OnConnectionError += Client_OnConnectionError;
            this.latency = latency;
            this.dryRun = dryRun;
            this.logger = logger;
        }

        #region client operation
        bool IsConnected {
            get {
                lock ( lockObject ) {
                    if ( client.IsConnected ) {
                        return true;
                    }
                    return false;
                }
            }
        }

        internal void Connect() {
            // threadでやってもいいけれど、例外が発生して処理しなくてはならないならこっちだろう
            lock ( lockObject ) {
                if ( !client.IsConnected ) {
                    logger?.Log( "Connecting..." );
                    client.Connect();
                }
            }

            queue = new ConcurrentQueue<Job>();
            terminateThread = false;
            thread = new Thread( new ThreadStart( Process ) );
            thread.Start();
        }

        internal void Disconnect() {
            terminateThread = true;
            if ( !thread.Join( 5000 ) ) { // 念のため適当にタイムアウト
                // exception or error message
                thread.Abort();
            }
            thread = null;
            queue = null;

            // threadでやってもいいけれど…
            lock ( lockObject ) {
                if ( client.IsConnected ) {
                    logger?.Log( "disconnecting..." );
                    client.Disconnect();
                }
            }
        }

        void SendMessage( string text ) {
            lock ( lockObject ) {
                if ( client.IsConnected ) {
                    client.SendMessage( text, dryRun );
                }
            }
        }
        #endregion

        #region Callbacks
        void Client_OnConnected( object sender, OnConnectedArgs e ) {
            logger?.Log( $"{e.BotUsername} connected in Twitch chat." );
        }

        void Client_OnDisconnected( object sender, OnDisconnectedArgs e ) {
            logger?.Log( $"{e.BotUsername} disconnected in Twitch chat." );
        }

        void Client_OnLog( object sender, OnLogArgs e ) {
            logger?.Log( e.Data );
        }

        void Client_OnConnectionError( object sender, OnConnectionErrorArgs e ) {
            logger?.Error( e.Error.Message );
        }
        #endregion


        internal void EnqueueMessage( string text ) {
            logger?.Debug( text );
            queue?.Enqueue( new Job { date = DateTime.Now, text = text } );
        }

        // why this use thread?
        // 送信ジョブを全て把握する必要がある（これはtaskでも可能）
        // 特に終了時に、溜まった未処理ジョブを止める必要がある (これも把握出来ていればtaskでも可能)
        // 複数のジョブを並列で処理する必要が無い、単一処理で完結する
        // 長時間にわたるジョブを多数投げると、taskで用いているthread poolを多数占有しつづける。最悪全て消費する
        // スパム防止として、ジョブが極めて近い間隔で処理されるタイミングを生じないようにする必要がある
        void Process() {
            const int intervalMilliseconds = (int)( ( 30.0 / 20.0 ) * 1000 ); // 20 times per 30 seconds
            long latencyTicks = (long)( latency * TimeSpan.TicksPerSecond ); // seconds to ticks
            Job job;
            job.date = DateTime.MinValue;
            job.text = null;
            bool has_job = false;
            while ( !terminateThread ) {
                // poll queue
                // EnqueueMessageは常にメインからの呼び出しで、キューには常に時間順に詰められている。
                // よって常に先頭要素に対してのみ処理すればよい
                if ( !has_job && ( queue?.TryDequeue( out job ) ?? false ) ) {
                    has_job = true;
                }
                // wait to do a job
                if ( has_job ) {
                    // 時間待ち まとめて寝ないのは、終了待ちが秒オーダーで取れなくなるためだ
                    long elapsedTicks = DateTime.Now.Ticks - job.date.Ticks;
                    if ( elapsedTicks >= latencyTicks ) {
                        has_job = false;
                        SendMessage( job.text ); // lock
                        if ( terminateThread ) {
                            break;
                        }
                        // この時間寝るとも限らないが、次の処理は一定時間あけるようにする
                        // 実際は自身のチャンネルでadminを持っているはずなので100回まで喋っても問題無いはずだ
                        // botとしてならもっと発言していいが、現実的に避けた方が良いだろう
                        Thread.Sleep( intervalMilliseconds );
                    }

                }
                // signalの方が望ましいが、sleepと併用が必要なので複雑になる
                // settings側の分解能が0.1sec (100ms)なので、ナイキスト定理的にその半分程度待てば余裕があるだろう
                Thread.Sleep( 50 );
            }
        }

        ConnectionCredentials crentials = null;
        TwitchClient client = null;
        readonly double latency = 10.0;
        readonly bool dryRun = false;

        // TwitchClientは、thread-safeな可能性はあるが、ざっと見た限りそうではないし、明記されていないのでこれを用いてlockする
        object lockObject = new object();

        struct Job {
            public DateTime date; // tickかmsで十分
            public string text;
        }
        //https://docs.microsoft.com/ja-jp/dotnet/standard/collections/thread-safe/when-to-use-a-thread-safe-collection
        // だが、ロック頻度は稀なので、どっちでもよい。
        ConcurrentQueue<Job> queue = null;
        Thread thread = null;
        volatile bool terminateThread = false;

        ILogger logger = null;
    }
}
