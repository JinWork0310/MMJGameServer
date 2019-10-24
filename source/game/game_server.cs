using System;
using System.Threading;
using System.Runtime.InteropServices;
using MrsLibs.Server;
using MrsLibs.Parser;
using MrsLibs.Signal;
using Game_Process;
using MMJGameServer;


namespace game_server
{
	using MrsServer = IntPtr;
	using MrsConnection = IntPtr;

	/// <summary>
	/// エコーサーバークラス
	/// </summary>
	public class GameServer : Mrs, IDisposable
	{
		/// <summary>
		/// 廃棄フラグ
		/// </summary>
		private bool m_bDisposed = false;

		/// <summary>
		/// 公開キー
		/// </summary>
		private static readonly string s_SslCertificateData =
			"Certificate:\n" +
			"    Data:\n" +
			"        Version: 3 (0x2)\n" +
			"        Serial Number: 1 (0x1)\n" +
			"    Signature Algorithm: sha256WithRSAEncryption\n" +
			"        Issuer: C=JP, ST=Tokyo, L=Tokyo, O=Monobit Inc, CN=root-ca.monobit.com\n" +
			"        Validity\n" +
			"            Not Before: Feb  5 09:44:33 2018 GMT\n" +
			"            Not After : Feb  3 09:44:33 2028 GMT\n" +
			"        Subject: C=JP, ST=Tokyo, O=Monobit Inc, CN=monobit.com\n" +
			"        Subject Public Key Info:\n" +
			"            Public Key Algorithm: rsaEncryption\n" +
			"                Public-Key: (2048 bit)\n" +
			"                Modulus:\n" +
			"                    00:be:49:4b:94:e2:46:be:85:a9:3c:1b:99:ae:1b:\n" +
			"                    e1:0b:17:b1:9d:b8:d9:5e:15:2d:77:05:6a:b0:a7:\n" +
			"                    c3:85:b6:71:c7:99:37:58:04:0e:67:81:b8:fd:20:\n" +
			"                    67:ca:dc:3a:0e:55:08:d2:d3:80:cd:e5:b4:e8:70:\n" +
			"                    8f:ab:09:ac:26:ac:0b:4e:ed:a9:78:46:5f:ae:54:\n" +
			"                    a8:8a:ea:1c:11:43:e6:a3:61:bf:73:dd:a3:6d:d0:\n" +
			"                    7b:51:09:a8:8f:b2:0b:6e:6b:2a:0e:d6:41:99:61:\n" +
			"                    eb:01:09:cd:f6:36:1e:b1:38:f3:25:ff:a8:6a:aa:\n" +
			"                    45:a8:32:79:28:46:28:4e:62:34:57:6d:99:77:7a:\n" +
			"                    cb:9d:ef:ee:d6:c4:b5:15:a5:3e:cb:24:68:8e:37:\n" +
			"                    31:a6:ea:b9:dd:49:0b:fe:3f:a7:f0:c9:13:bb:bf:\n" +
			"                    26:91:fe:68:dc:79:5b:9c:ff:e0:7d:83:14:83:5e:\n" +
			"                    8d:c9:0a:b9:0d:2d:9a:45:23:a3:0d:76:5e:70:57:\n" +
			"                    b6:9e:82:bb:91:47:74:9a:e5:41:32:8a:e2:c8:04:\n" +
			"                    be:08:4c:90:41:66:4f:8d:a2:c7:ac:7d:87:0d:f6:\n" +
			"                    a7:55:37:06:33:16:89:56:85:c8:76:ba:bd:c7:07:\n" +
			"                    fb:cc:66:e5:3d:21:26:4f:9e:1e:58:39:60:2e:5e:\n" +
			"                    c1:e9\n" +
			"                Exponent: 65537 (0x10001)\n" +
			"        X509v3 extensions:\n" +
			"            X509v3 Subject Alternative Name: \n" +
			"                DNS:monobit.com\n" +
			"    Signature Algorithm: sha256WithRSAEncryption\n" +
			"         3b:7e:e0:02:08:ee:9f:aa:df:ea:de:03:11:08:6f:2e:0f:35:\n" +
			"         28:59:93:ad:14:5e:36:cc:61:ba:f1:37:d9:bf:20:46:a7:c8:\n" +
			"         cc:f1:41:70:23:90:f2:26:2a:2d:a6:5e:2f:d2:3a:a8:5e:bd:\n" +
			"         aa:97:f1:ad:17:41:9a:f1:85:63:6b:52:89:b8:b6:56:c3:96:\n" +
			"         54:ae:26:f9:4c:ff:4c:22:c6:70:7d:40:97:c0:e2:4f:a9:0c:\n" +
			"         13:c7:eb:b9:d5:ea:3d:5e:89:16:eb:91:b6:4f:a1:db:82:69:\n" +
			"         4a:9f:91:08:f6:6c:0d:b4:28:99:8e:38:57:8b:b6:1b:21:b1:\n" +
			"         c1:74:44:68:6d:a8:f5:29:dc:10:dc:2c:08:ec:dc:91:51:02:\n" +
			"         ba:d9:7a:b9:74:c4:59:f5:78:23:2e:60:81:8e:d2:53:07:8e:\n" +
			"         79:16:21:f9:75:5f:70:0f:46:be:83:8c:07:84:7b:87:16:46:\n" +
			"         04:df:31:c7:e9:7b:69:dc:56:fc:bc:a2:d5:6d:c1:94:c3:fa:\n" +
			"         01:1f:71:30:4c:c0:36:47:68:17:3d:6d:fc:81:1f:be:a5:9b:\n" +
			"         b6:5d:d3:10:56:57:1a:09:9f:5b:68:a3:73:17:d6:f6:ba:57:\n" +
			"         31:f3:01:d5:5d:1a:e7:b6:e3:36:68:2e:8c:38:c4:8b:12:07:\n" +
			"         8e:76:5c:cc\n" +
			"-----BEGIN CERTIFICATE-----\n" +
			"MIIDPzCCAiegAwIBAgIBATANBgkqhkiG9w0BAQsFADBhMQswCQYDVQQGEwJKUDEO\n" +
			"MAwGA1UECAwFVG9reW8xDjAMBgNVBAcMBVRva3lvMRQwEgYDVQQKDAtNb25vYml0\n" +
			"IEluYzEcMBoGA1UEAwwTcm9vdC1jYS5tb25vYml0LmNvbTAeFw0xODAyMDUwOTQ0\n" +
			"MzNaFw0yODAyMDMwOTQ0MzNaMEkxCzAJBgNVBAYTAkpQMQ4wDAYDVQQIDAVUb2t5\n" +
			"bzEUMBIGA1UECgwLTW9ub2JpdCBJbmMxFDASBgNVBAMMC21vbm9iaXQuY29tMIIB\n" +
			"IjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvklLlOJGvoWpPBuZrhvhCxex\n" +
			"nbjZXhUtdwVqsKfDhbZxx5k3WAQOZ4G4/SBnytw6DlUI0tOAzeW06HCPqwmsJqwL\n" +
			"Tu2peEZfrlSoiuocEUPmo2G/c92jbdB7UQmoj7ILbmsqDtZBmWHrAQnN9jYesTjz\n" +
			"Jf+oaqpFqDJ5KEYoTmI0V22Zd3rLne/u1sS1FaU+yyRojjcxpuq53UkL/j+n8MkT\n" +
			"u78mkf5o3HlbnP/gfYMUg16NyQq5DS2aRSOjDXZecFe2noK7kUd0muVBMoriyAS+\n" +
			"CEyQQWZPjaLHrH2HDfanVTcGMxaJVoXIdrq9xwf7zGblPSEmT54eWDlgLl7B6QID\n" +
			"AQABoxowGDAWBgNVHREEDzANggttb25vYml0LmNvbTANBgkqhkiG9w0BAQsFAAOC\n" +
			"AQEAO37gAgjun6rf6t4DEQhvLg81KFmTrRReNsxhuvE32b8gRqfIzPFBcCOQ8iYq\n" +
			"LaZeL9I6qF69qpfxrRdBmvGFY2tSibi2VsOWVK4m+Uz/TCLGcH1Al8DiT6kME8fr\n" +
			"udXqPV6JFuuRtk+h24JpSp+RCPZsDbQomY44V4u2GyGxwXREaG2o9SncENwsCOzc\n" +
			"kVECutl6uXTEWfV4Iy5ggY7SUweOeRYh+XVfcA9GvoOMB4R7hxZGBN8xx+l7adxW\n" +
			"/Lyi1W3BlMP6AR9xMEzANkdoFz1t/IEfvqWbtl3TEFZXGgmfW2ijcxfW9rpXMfMB\n" +
			"1V0a57bjNmgujDjEixIHjnZczA==\n" +
			"-----END CERTIFICATE-----\n";

		/// <summary>
		/// プライベートキー
		/// </summary>
		private static readonly string s_SslPrivateKeyData =
			"-----BEGIN PRIVATE KEY-----\n" +
			"MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC+SUuU4ka+hak8\n" +
			"G5muG+ELF7GduNleFS13BWqwp8OFtnHHmTdYBA5ngbj9IGfK3DoOVQjS04DN5bTo\n" +
			"cI+rCawmrAtO7al4Rl+uVKiK6hwRQ+ajYb9z3aNt0HtRCaiPsgtuayoO1kGZYesB\n" +
			"Cc32Nh6xOPMl/6hqqkWoMnkoRihOYjRXbZl3esud7+7WxLUVpT7LJGiONzGm6rnd\n" +
			"SQv+P6fwyRO7vyaR/mjceVuc/+B9gxSDXo3JCrkNLZpFI6MNdl5wV7aegruRR3Sa\n" +
			"5UEyiuLIBL4ITJBBZk+NosesfYcN9qdVNwYzFolWhch2ur3HB/vMZuU9ISZPnh5Y\n" +
			"OWAuXsHpAgMBAAECggEBAKvCVSrqpJjM5VmQQEPcfmIY3QZVGD/INeW9SfRzOhWz\n" +
			"/TgBoOcdojLj8Srq2UVPTEgrkW9b4mP3+DfngocMkAvIN9ziwZoDS9J8MDZT40ni\n" +
			"VIkdbkcTxLUW/njDkxdByve8W5ZQ552fuRSS2QppB4NIuJGQF2FQmNed16b2zOMS\n" +
			"R8UMBwpax2KSM1FEi2/wm7D/dTM9KrKF+mL0m9qLpXJBLkb3Ysz1jYVFLzKb6led\n" +
			"C8rhTGdjGJzBkvUqEpntTaxiPcMPJrwXnFZLCsnRY59aQ0HAeaIHofz1F+PrPnlG\n" +
			"LhldWoBJ9DHSvlxptl1H32fWaOiUOHx/4HTWmYrVg0ECgYEA/H+jO8Os+e9a/586\n" +
			"vvFZohSDd/kX+LgekYnGX+d99615/ef6QcVpATjGy4amAz84U1o9vc108HpkL8l2\n" +
			"n5Sz0o8DWlCzq0umLfDcVolw4CUEHSMIlnDiOwuvNMvVlj9mMPD29u2tMZkYhtl7\n" +
			"hBnqRiGEST14NDzwVaVUO5HfKb0CgYEAwOzOT/cQOwsHS2VXmmnCxuQs5nYyHeDc\n" +
			"IYCA/4/pn4QJngXf9Y3MUbrYyBqYikuJdFsUoRG6Gaei1TSqQyN0tE82/kT+d+TW\n" +
			"tez51bYnFy5BKrOjca0ynk+vzQAue3EWKzubvB+Ry2PeXDBiDwLTiFnL6ZrBDdXZ\n" +
			"b6KKg5YC3Z0CgYBMTNWqzaqLrES3UgqSgKQxUjmYG3Ge+yRhnlyPxohOO+HNVDhP\n" +
			"f7QHZnzMK9gmywfeSDq4DEn2EUYNGrf56Rmd2xGMTS696JJC97HdhJLTaNwhYeDK\n" +
			"dTon1ZQQRDg6utXKnEZEv/XCMx0yQq4McThWEPLEnwqf3npRpzAZAC+LkQKBgENd\n" +
			"bNz3RC+Ztj5ZcLF2ZJDWc+c1NmLAdZ0tJd4W/li43jLTklRH4yRWvgOBZepEXgbH\n" +
			"Fvj3G6iBpJYWAa3X35RmZKl8pe5vdZmo2cQKCrRJbm/esh+rfpVQ9e37Nj/cSQVn\n" +
			"lwWlcF84zBgcvODI99wQnuc/JlISbg5RD1TLOMzxAoGBAKAaxCAQSdArvOSf6Q1P\n" +
			"AlCeMGGxqjL9phyh37VDuvZspC9dFZuhgG//EVcf/1q9dZd4Nev4SRl9TjMu5MPY\n" +
			"thPRFkZrj/0ArbT6PBreFUodUv5kp48Qnitk3KMn1QfcR3pO3A5BPaRjyc/7d81F\n" +
			"Md2qrDIp8KRceWB4YV9pjPOX\n" +
			"-----END PRIVATE KEY-----\n";

        /// <summary>
        /// 最大プレイヤー数
        /// </summary>
        private static UInt32 m_MaxPlayer = 4;

        /// <summary>
        /// スリープ時間
        /// </summary>
        private UInt32 m_SleepMsec = 1;

		/// <summary>
		/// サーバーIPアドレス
		/// </summary>
		private string m_ArgServerAddr = "127.0.0.1";

		/// <summary>
		/// サーバーのポート番号
		/// </summary>
		private ushort m_ServerPort = 22222;

		/// <summary>
		/// バックログ
		/// </summary>
		private Int32 m_BackLog = 10;

		/// <summary>
		/// レコード受信用
		/// </summary>
        private bool m_isValidRecord = true;

		/// <summary>
		/// パーサー
		/// </summary>
		private MrsCmdParser m_Parser = null;

		/// <summary>
		/// ループフラグ
		/// </summary>
		private volatile bool m_bLoop = true;

        /// <summary>
        /// ゲーム処理コンポーネント
        /// </summary>
        private static GameProcess m_gameProc = null;

        /// <summary>
        /// 現在の接続元管理
        /// </summary>
        private static MrsConnection[] m_nowConnect;

        /// <summary>
        /// ゲーム稼働フラグ
        /// </summary>
        private static Boolean g_gameon = false;

        ServerTime serverTime;
        private static uint nowplayers = 0;

        static int readybit;

        private static ushort g_payloadType;

        private static bool some1disconnected = false;

		/// <summary>
		/// コールバックの保存
		/// </summary>
		private static MrsNewConnectionCallback m_OnNewConnection = OnNewConnection;
		private static MrsDisconnectCallback m_OnDisconnect = OnDisconnect;
		private static MrsErrorCallback m_OnError = OnError;
		private static MrsReadRecordCallback m_OnReadRecord = OnReadRecord;
        private static MrsReadCallback m_OnRead = OnRead;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GameServer()
		{
            m_nowConnect = new MrsConnection[m_MaxPlayer];
            for(int i = 0; i < m_MaxPlayer; i++)
            {
                m_nowConnect[i] = new MrsServer();
            }
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="args"></param>
		public void Initialize(string[] args)
		{
			m_Parser = new MrsCmdParser('-');
			m_Parser.Parse(args);
			m_SleepMsec = m_Parser.GetNumeric<UInt32>("sleep_msec", 1);
			m_ArgServerAddr = m_Parser.GetString("server_addr", "0.0.0.0");
			m_ServerPort = m_Parser.GetNumeric<ushort>("server_port", 22222);
			m_BackLog = m_Parser.GetNumeric<Int32>("backlog", 10);
            m_isValidRecord = m_Parser.GetNumeric<int>("is_valid_record", 1) != 0 ? true : false;
		}
		
		/// <summary>
		/// コネクションタイプを表す文字列を返す
		/// </summary>
		private static string ConnectionTypeToString(MrsConnection connection)
        {
			MrsConnectionType type = mrs_connection_get_type( connection );
			switch ( type )
            {
			case MrsConnectionType.NONE:{ return "NONE"; }
			case MrsConnectionType.TCP:{ return "TCP"; }
			case MrsConnectionType.UDP:{ return "UDP"; }
			}
			return "INVALID";
		}
		
		/// <summary>
		/// サーバーの実行
		/// </summary>
		public void Run()
		{
			try
			{
                serverTime = new ServerTime();
				mrs_initialize();
				mrs_set_ssl_certificate_data(s_SslCertificateData);
				mrs_set_ssl_private_key_data(s_SslPrivateKeyData);


				using (var sig = new ExitSignal())
				using (var tcp_server = new MrsServerFoundation(MrsConnectionType.TCP, m_ArgServerAddr, m_ServerPort, m_BackLog)) // TCP
				using (var udp_server = new MrsServerFoundation(MrsConnectionType.UDP, m_ArgServerAddr, m_ServerPort, m_BackLog)) // UDP
				{
					sig.SetSignal((obj, e) => {
						SignalEventArgs event_args = e as SignalEventArgs;
						MRS_LOG_DEBUG("Event: {0}", event_args.SignalEnums);
						m_bLoop = false;
						Thread.Sleep(10);
					});

					mrs_server_set_new_connection_callback(tcp_server.Server, OnNewConnection);
					mrs_server_set_new_connection_callback(udp_server.Server, OnNewConnection);

					mrs_set_error_callback(tcp_server.Server, m_OnError);
					mrs_set_error_callback(udp_server.Server, m_OnError);

                    if (!m_isValidRecord)
                    {
                        mrs_set_read_callback(tcp_server.Server, m_OnRead);
                        mrs_set_read_callback(udp_server.Server, m_OnRead);
                    }

                    m_gameProc = new GameProcess();

					MRS_LOG_DEBUG("{0} listening on {1} {2}", ConnectionTypeToString( tcp_server.Server ), m_ArgServerAddr, m_ServerPort);
					MRS_LOG_DEBUG("{0} waiting on {1} {2}", ConnectionTypeToString( udp_server.Server ), m_ArgServerAddr, m_ServerPort);

					while (m_bLoop)
					{
                        if (g_gameon)
                        {
                            m_gameProc.UpdateGame();
                            //g_payloadType = 0x11;
                            //SendDataEveryone();
                        }
                        else
                        {
                            if (some1disconnected)
                            {
                                SortPlayerList();
                            }
                        }

						mrs_update();
						mrs_sleep(m_SleepMsec);
					}
				}
			}
			catch (Exception e)
			{
				MRS_LOG_ERR("exception: {0}", e.Message);
			}
			finally
			{
				mrs_finalize();
			}
		}

		/// <summary>
		/// 廃棄処理
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 廃棄処理の実装部
		/// </summary>
		/// <param name="disposed"></param>
		public void Dispose(bool disposed)
		{
			if (m_bDisposed == true) return;
			if (m_bDisposed != disposed)
			{
			}
			m_bDisposed = disposed;
		}

		/// <summary>
		/// TCPソケットが新しい接続を生成した時に呼ばれる
		/// </summary>
		/// <param name="server"></param>
		/// <param name="server_data"></param>
		/// <param name="client"></param>
		private static void OnNewConnection(MrsServer server, IntPtr server_data, MrsConnection client)
		{
            MRS_LOG_DEBUG("OnNewConnection {0} : {1}", ConnectionTypeToString(client), client.ToString() );

			mrs_set_disconnect_callback(client, m_OnDisconnect);
			mrs_set_error_callback(client, m_OnError);
			mrs_set_read_record_callback(client, m_OnReadRecord);
			OnConnect(client);
		}

		/// <summary>
		/// TCPソケット接続時に呼ばれる
		/// </summary>
		/// <param name="connection"></param>
		private static void OnConnect(MrsConnection connection)
        {
            if (g_gameon || nowplayers >= m_MaxPlayer) { mrs_close(connection); return; }
            for (int i = 0; i < m_MaxPlayer; i++)
            {
                if (m_nowConnect[i].ToInt32() == 0)
                {
                    m_nowConnect[i] = connection;
                    MRS_LOG_DEBUG("OnConnect {0} : {1} local_mrs_version=0x{2:X} remote_mrs_version=0x{3:X}",
                        ConnectionTypeToString(connection), m_nowConnect[i].ToInt32(), mrs_get_version(MRS_VERSION_KEY), mrs_connection_get_remote_version(connection, MRS_VERSION_KEY));
                    nowplayers = mrs_get_connection_num();
                    MRS_LOG_DEBUG("now Connect No.{0} : {1}", i, m_nowConnect[i].ToInt32());
                    break;
                }
                MRS_LOG_DEBUG("now Connect No.{0} : {1}", i, m_nowConnect[i].ToInt32());
            }
            bool connectAll = true;
            for(int j = 0; j < m_MaxPlayer; j++)
            {
                if(m_nowConnect[j].ToInt32() == 0) { connectAll = false;  break; }
            }
            if (connectAll)
            {
            }
			mrs_set_cipher(connection, mrs_cipher_create(MrsCipherType.ECDH));
		}

        /// <summary>
        /// TCPソケット切断時に呼ばれる
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="connection_data"></param>
        private static void OnDisconnect(MrsConnection connection, IntPtr connection_data)
        {
            some1disconnected = true;

            for (int i = 0; i < m_MaxPlayer; i++)
            {
                if (connection == m_nowConnect[i])
                {
                    MRS_LOG_DEBUG("OnDisconnect {0} : {1} local_mrs_version=0x{2:X} remote_mrs_version=0x{3:X}",
                        ConnectionTypeToString(connection), m_nowConnect[i].ToInt32(), mrs_get_version(MRS_VERSION_KEY), mrs_connection_get_remote_version(connection, MRS_VERSION_KEY));
                    m_gameProc.eraseProfileData(i);
                    m_nowConnect[i] = IntPtr.Zero; 

                }
                if (m_nowConnect[i].ToInt32() == 0) {
                    for (int j = i + 1; j < m_MaxPlayer; j++)
                    {
                        if (m_nowConnect[j].ToInt32() != 0)
                        {
                            m_nowConnect[i] = m_nowConnect[j];
                            m_nowConnect[j] = IntPtr.Zero;
                            break;
                        }
                    } 
                }
                //MRS_LOG_DEBUG("now Connect No.{0} : {1}", i, m_nowConnect[i].ToInt32());
            }
            nowplayers = mrs_get_connection_num();

            if (nowplayers <= 2) { g_gameon = false; m_gameProc.CloseGame(); }
        }

		/// <summary>
		/// エラー時に呼ばれる
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="connection_data"></param>
		/// <param name="status"></param>
		private static void OnError(MrsConnection connection, IntPtr connection_data, MrsConnectionError status)
		{
			MRS_LOG_DEBUG( "OnError {0} local_mrs_version=0x{1:X} remote_mrs_version=0x{2:X} status={3}",
				ConnectionTypeToString(connection), mrs_get_version(MRS_VERSION_KEY), mrs_connection_get_remote_version(connection, MRS_VERSION_KEY), mrs_get_connection_error_string(status));
		}

		/// <summary>
		/// レコード受信時に呼ばれる
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="connection_data"></param>
		/// <param name="seqnum"></param>
		/// <param name="options"></param>
		/// <param name="payload_type"></param>
		/// <param name="_payload"></param>
		/// <param name="payload_len"></param>
		private static void OnReadRecord(MrsConnection connection, IntPtr connection_data, UInt32 seqnum, UInt16 options, UInt16 payload_type, IntPtr _payload, UInt32 payload_len)
		{
			ParseRecord(connection, connection_data, seqnum, options, payload_type, _payload, payload_len);
		}

		/// <summary>
		/// レコード受信時に呼ばれる
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="connection_data"></param>
		/// <param name="_data"></param>
		/// <param name="data_len"></param>
		private static void OnRead(MrsConnection connection, IntPtr connection_data, IntPtr _data, UInt32 data_len)
		{
			mrs_write(connection, _data, data_len);
		}

		/// <summary>
		/// レコードのパース
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="connection_data"></param>
		/// <param name="seqnum"></param>
		/// <param name="options"></param>
		/// <param name="payload_type"></param>
		/// <param name="_payload"></param>
		/// <param name="payload_len"></param>
		private static void ParseRecord(MrsConnection connection, IntPtr connection_data, UInt32 seqnum, UInt16 options, UInt16 payload_type, IntPtr _payload, UInt32 payload_len)
		{
			MRS_LOG_DEBUG("ParseRecord seqnum=0x{0:X} options=0x{1:X} payload type={2:X} payload len={3}", seqnum, options, payload_type, payload_len);
            switch (payload_type)
            {
                // MRS_PAYLOAD_TYPE_BEGIN - MRS_PAYLOAD_TYPE_ENDの範囲内で任意のIDを定義し、対応するアプリケーションコードを記述する
                case 0x01:
                    {
                        g_payloadType = 0x02;
                        int i = 0;

                        for (; i < m_MaxPlayer; i++)
                        {
                            if (connection == m_nowConnect[i])
                            {

                                for (int j = 0; j < nowplayers; j++)
                                {
                                    if (i != j)
                                    {
                                        IntPtr p_Send = m_gameProc.getProfileData(j);
                                        mrs_write_record(m_nowConnect[i], options, g_payloadType, p_Send, (uint)m_gameProc.getProfileSize(j));
                                        MRS_LOG_DEBUG("SENT No.{0} Player to No.{1} Player's Data", i, j);
                                    }
                                }
                                break;
                            }
                        }
                        IntPtr p_data = m_gameProc.setProfile(_payload, i);
                        
                        g_payloadType = 0x01;
                        for (int j = 0; j < nowplayers; j++)
                        {
                            if (j == i)
                            {
                                g_payloadType = 0x01;
                                mrs_write_record(m_nowConnect[j], options, g_payloadType, p_data, (uint)Marshal.SizeOf(m_gameProc.getProfile(i)));
                            }
                            if (j != i && m_nowConnect[j] != (IntPtr)0)
                            {
                                g_payloadType = 0x02;
                                mrs_write_record(m_nowConnect[j], options, g_payloadType, p_data, (uint)m_gameProc.getProfileSize(i));
                            }
                        }
                    }
                    break;

                    // 0x03 ゲームスタート準備
                case 0x03:
                    {
                        nowplayers = mrs_get_connection_num();
                        g_payloadType = 0x03;
                        MRS_LOG_DEBUG("received 0x03 data");
                        // ゲーム処理初期化
                        m_gameProc.Initialize();
                        readybit = 0;

                        // ゲームスタートに必要なデータの作成・送信
                        IntPtr sendptr = m_gameProc.getStartData((int)nowplayers);

                        for(int i = 0; i < nowplayers; i++)
                        {
                            mrs_write_record(m_nowConnect[i], options, g_payloadType, sendptr, (uint)m_gameProc.getStartDataSize());
                        }
                        MRS_LOG_DEBUG("sent start data");
                        Marshal.FreeHGlobal(sendptr);
                    }
                    break;

                    // 0x04 クライアント達の初期位置設定待ち
                case 0x04:
                    {
                        int i = 0;
                        for (; i < m_MaxPlayer; i++)
                        {
                            if (connection == m_nowConnect[i]) break;
                        }
                        MRS_LOG_DEBUG("received 0x03 data from Player no.{0}",i);

                        m_gameProc.setPlayerData(i, _payload);


                        readybit += 1 << i;

                        MRS_LOG_DEBUG("READY BIT : {0}", Convert.ToString(readybit, 2));

                        int correctBit = 0;
                        for (int j = 0; j < mrs_get_connection_num(); j++) { correctBit += 1 << j; }
                        if(readybit == correctBit)
                        {
                            g_payloadType = 0x04;
                            SendDataEveryone();
                            MRS_LOG_DEBUG("EVERYONE READY!");

                            g_gameon = true;
                        }
                    }
                    break;

                // 0x05 カウントダウン開始の告知
                case 0x05:
                    {
                        for(int i = 0; i < nowplayers; i++)
                        {
                            if(m_nowConnect[i].ToInt32() != 0) mrs_write_record(m_nowConnect[i], options, payload_type, _payload, payload_len);
                        }
                    }
                    break;

                // 0x06 カウントダウン時間の送受信
                case 0x06:
                    {
                        for (int i = 0; i < nowplayers; i++)
                        {
                            if (m_nowConnect[i].ToInt32() != 0) mrs_write_record(m_nowConnect[i], options, payload_type, _payload, payload_len);
                        }
                    }
                    break;

                // 0x07 ステージ番号の送受信
                case 0x07:
                    unsafe{
                        int stageid = *(int*)_payload;
                        m_gameProc.setStageId(stageid);

                        for (int i = 0; i < nowplayers; i++)
                        {
                            if (m_nowConnect[i].ToInt32() != 0) mrs_write_record(m_nowConnect[i], options, payload_type, _payload, payload_len);
                        }
                    }
                    break;

                // プレイヤーデータ送受信用PayloadType
                case 0x12:
                    {
                        if (g_gameon)
                        {
                            int i = 0;
                            for (; i < m_MaxPlayer; i++)
                            {
                                if (connection == m_nowConnect[i]) break;
                            }

                            m_gameProc.setPlayerData(i, _payload);

                            for (int j = 0; j < mrs_get_connection_num(); j++)
                            {
                                if (j != i) if (m_nowConnect[j].ToInt32() != 0) mrs_write_record(m_nowConnect[j], options, payload_type, _payload, payload_len);
                            }
                        }
                    }
                    break;

                // ショットデータ送受信用Type
                case 0x13:
                    {
                        if (g_gameon)
                        {
                            IntPtr send = m_gameProc.setShotData(_payload);

                            for (int j = 0; j < nowplayers; j++)
                            {
                                if (m_nowConnect[j].ToInt32() != 0) mrs_write_record(m_nowConnect[j], options, payload_type, send, payload_len);
                            }
                        }
                    }
                    break;

                    // 床の落下の合図、全員に合図を送信
                case 0x15:
                    {
                        MRS_LOG_DEBUG("RECIEVE 0x15  FALL FLOOR !!");
                        for(int i = 0; i < nowplayers; i++)
                        {
                            if (m_nowConnect[i].ToInt32() != 0) mrs_write_record(m_nowConnect[i], options, payload_type, _payload, payload_len);
                        }
                    }break;


                //------------------------------------ 死亡判定系 0x2#

                // 落下死
                case 0x21:
                    {
                        MRS_LOG_DEBUG("RECIEVE SOMEONE FALLING !!");
                        for (int i = 0; i < nowplayers; i++)
                        {
                            if (m_nowConnect[i].ToInt32() != 0) mrs_write_record(m_nowConnect[i], options, payload_type, _payload, payload_len);
                        }
                    }
                    break;

                // 被弾死
                case 0x22:
                    {
                        MRS_LOG_DEBUG("RECIEVE SOMEONE TAKING SHOT !!");

                        IntPtr sendHit = m_gameProc.SomeoneDeadHit(_payload);
                        for (int i = 0; i < nowplayers; i++)
                        {
                            if (m_nowConnect[i].ToInt32() != 0) mrs_write_record(m_nowConnect[i], options, payload_type, sendHit, payload_len);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// ゲームデータの一括送信
        /// </summary>
        /// <param name="_ptr">データ構造体のポインタ</param>
        public static void SendDataEveryone()
        {
            IntPtr _ptr = m_gameProc.getDataForSend();
            int _len = m_gameProc.getSendPackSize();

            for (int i = 0; i < nowplayers; i++)
            {
                if (m_nowConnect[i].ToInt32() != 0)
                {
                    mrs_write_record(m_nowConnect[i], 1, g_payloadType, _ptr, (uint)_len);
                    //MRS_LOG_DEBUG("Send Everyone No.{0}", i);
                }
            }
            Marshal.FreeHGlobal(_ptr);
        }


        private static void SortPlayerList()
        {
            m_gameProc.sortPlayerList();

            for (int i = 0; i < m_MaxPlayer; i++)
            {
                sendCorrectProfile(i, m_gameProc.getProfileData(i));
            }

            some1disconnected = false;
        }


        public static void sendCorrectProfile(int _id, IntPtr _sendptr)
        {
            for (int i = 0; i < nowplayers; i++)
            {
                if(i == _id)
                {
                    if (m_nowConnect[i].ToInt32() != 0) mrs_write_record(m_nowConnect[i], 0x01, 0x01, _sendptr, (uint)m_gameProc.getProfileSize(_id));
                }
                else
                {
                    if (m_nowConnect[i].ToInt32() != 0) mrs_write_record(m_nowConnect[i], 0x01, 0x02, _sendptr, (uint)m_gameProc.getProfileSize(_id));
                }
            }
        }

	}// end GameServer class

	/// <summary>
	/// メインクラス
	/// </summary>
	class MrsMain
	{
		static void Main(string[] args)
		{
			using(GameServer server = new GameServer())
			{
				if (server != null)
				{
					server.Initialize(args);
					server.Run();
				}
			}
		}
	}
}
