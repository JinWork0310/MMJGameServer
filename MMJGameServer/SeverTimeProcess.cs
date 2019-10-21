using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MMJGameServer {
	public class Timer {
		Stopwatch stopwatch = new Stopwatch();

		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// 経過時間を取得
		/// </summary>
		public double ElapsedSeconds{ get { return stopwatch.Elapsed.TotalSeconds; } }
		public float FElapsedSeconds { get { return (float)stopwatch.Elapsed.TotalSeconds; } }
		public int IElapsedSeconds { get { return (int)stopwatch.Elapsed.TotalSeconds; } }

		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// 高精度カウンタのサポート状況取得
		/// </summary>
		static public bool IsHighResolution{ get{ return Stopwatch.IsHighResolution; } }


		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// タイマースタート
		/// </summary>
		public void Start(){ stopwatch.Start(); }
		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// タイマーをリセット
		/// </summary>
		public void Reset(){ stopwatch.Reset(); }

		public void Restart(){ stopwatch.Restart(); }
	}


	//-------------------------------------------------------------------------------------------
	/// <summary>
	/// サーバー全体の時間を管理するクラス
	/// </summary>
	class ServerTime {
		Timer timer = new Timer();
		Timer progressTimer = new Timer();

		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// updateからupdateまでにかかった時間
		/// </summary>
		public double ExecutedTime{ get; private set; }

		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// 最後に行ったUpdateからの経過時間;
		/// </summary>
		public double ProgressTime { get{ return progressTimer.ElapsedSeconds; } }
		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ServerTime(){
			timer.Start();
		}


		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// 更新処理
		/// </summary>
		public void Update() {
			ExecutedTime = progressTimer.ElapsedSeconds;
			progressTimer.Restart();			
		}


		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// 現在時刻取得
		/// </summary>
		public DateTime Now { get{ return DateTime.Now; } }
		//-------------------------------------------------------------------------------------------
		/// <summary>
		/// サーバー起動してからの経過時間取得
		/// </summary>
		double OperatingTimeSeconds { get{ return timer.ElapsedSeconds; } }


	}

	//-------------------------------------------------------------------------------------------
	/// <summary>
	/// バトル時に使用するタイマー
	/// </summary>
	public class BattleSceneTime {
		Timer timer = new Timer();


	}


	class SecondsCounter{
		Timer timer = new Timer();
		public bool Counting{ get; private set; }

		private int keepTime = 0;

		public SecondsCounter(){
			timer.Start();
		}

		void Update(){
			if(keepTime < timer.IElapsedSeconds){
				keepTime = timer.IElapsedSeconds;
				Counting = true;
			}
			else{
				Counting = false;
			}
		}
	}
}
