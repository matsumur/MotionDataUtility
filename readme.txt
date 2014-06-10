* プロジェクト
** MotionDataHandler
- 全体の機能のうち，メインUIを除いたもの
-- 外部でモーション処理やシーケンス処理のDLLを作成して，組み込めるようにしようと思ったときに外部のプログラムが参照する部分として分離

** MotionDataUtil
- 全体の機能のうち，メインUIとiCorpusStudioプラグイン化部分

** MotionDataUtilExe
- MotionDataUtilを単体で起動できるようにするexeファイルを作成するもの

** EVaRTTrackHandler
- EVaRT(Cortex)から出力される.trcファイルを，フレームのトリムやPhaseSpaceの.csvへの変換などを行う

** LLParserGenerator
- スクリプトの構文解析パーサ作成用プロジェクト(EBNFからC#コードを生成)

* クラスの説明書
** MotionDataHandler

*** DataIO/
- EVaRTTrc.cs
-- TrcHeader: EVaRT(Cortex)の.trcファイルのヘッダ情報保持用構造体
-- TrcMarker: .trcのあるフレームのあるマーカ―の座標データ保持用構造体
-- TrcFrame: .trcのあるフレームのフレーム番号，フレーム時刻，マーカーのリストを保持する構造体
-- TrcReader: .trcファイルを読み込むためのクラス
-- TrcWriter: .trcファイルを書き込むためのクラス

- FrameConverter.cs
-- FrameConverter: EVaRTの.trcファイルとPhaseSpaceの.csvファイルの相互変換用のクラス

- PhaseSpaceCsv.cs
-- PhaseSpaceMarker: PhaseSpaceの.csvファイルのあるフレームのあるマーカーの座標保持用構造体
-- PhaseSpaceFrame: PhaseSpaceの.csvファイルのあるフレームのフレーム時刻，マーカーのリストを保持する構造体
-- PhaseSpaceDataReader: PhaseSpaceのデータを読み込むためのクラス
-- PhaseSpaceDataWriter: PhaseSpaceのデータを書き込むためのクラス

*** Misc/
- Algorithms.cs
-- TravelingSalesmanProblem: 巡回セールスマン問題を力づくで解く(平面オブジェクトの外枠を求めるのに使う)ためのクラス
-- Permutation: 順列を列挙するためのクラス
-- PermutationEnumerator: 順列を列挙するための補助クラス(IEnumeratorの実装)
-- Combination: 組合せを列挙するためのクラス
-- SimultaneousEquation: n元n連立一次方程式を解くためのクラス
-- QuadraticEquation: 二次方程式を解くためのクラス

- BijectiveDictionary.cs
-- BijectiveDictionary: 双方向連想配列クラス．RefScriptVariable用

- ChainedSettings.cs
-- ChainedSettings: 複数プロジェクトにまたがったProperties.Settingsを連携させるためのクラス
-- MotionDataHandlerSettings: MotionDataHandler用のSettingsデータを保持するクラス

- CollectionEx.cs
-- CollectionEx: 配列，リスト，Enumerableなどのメソッド定義クラス

- ColorEx.cs
-- ColorEx: 色変換用のメソッド定義クラス

- CharacterSeparatedValues.cs
-- CharacterSeparatedValues: CSVファイル読み込み用クラス

- DialogOKCancel.cs
-- DialogOKCancel: 右下にOK/Cancelボタンのついたダイアログの基底クラス

- DialogOKDataGrid.cs
-- DialogOKDataGrid: 表示用のデータグリッドが付いたダイアログのクラス

- DialogOKMessage.cs
-- DialogOKMessage: メッセージ表示用のダイアログのクラス

- DialogSetSelectRange.cs
-- DialogSetSelectRange: 時間範囲選択用のダイアログのクラス

- DialogSimpleSelect.cs
-- DialogSimpleSelect: ラジオボタンで項目を選択する用のダイアログのクラス

- ErrorLogger.cs
-- ErrorLogger: デバッグ用エラー記録およびメッセージ表示クラス

- IDataChanged.cs
-- IDataChanged: データが更新された状態であるかを伝播するためのインターフェース

- IPluginHostChangedEventArgs.cs
-- IPluginHostChangedEventArgs: PluginHostが変更されたときのイベントでデータを提供するためのクラス

- ITimeInterval.cs
-- ITimeInterval: 時区間の値取得用インターフェース．TimeControllerがすべてのデータの開始時間と終了時間を知るために用いる

- ListViewItemComparer.cs
-- ListViewItemComparer: リストビューコントロールでアイテムのソートを行うためのクラス

- LockDisposable.cs
-- LockDisposable: using(){}の記法で非同期処理のためのロックと解除を行えるようにするためのクラス．通常の共有・排他ロックでは ReaderWriterLockSlim のインスタンスに対してtry-finally構文でロックを行うが，それをusingで行えるようにする
-- DisposableReadLock: LockDisposable.GetReadLock()で得られる共有ロック用オブジェクト
-- DisposableWriteLock: LockDisposable.GetWriteLock()で得られる排他ロック用オブジェクト
-- DisposableUpgradeableReadLock: LockDisposable.GetUpgradeableReadLock()で得られるロック用オブジェクト
-- DisposableReadLockCleanup: 後処理を受け取るDisposableReadLock
-- DisposableWriteLockCleanup: 後処理を受け取るDisposableWriteLock
-- DisposableUpgradeableReadLockCleanup: 後処理を受け取るDisposableUpgradeableReadLock

- MinMaxTester.cs
-- MinMaxTester: 時系列データから最大値と最小値を求めるための構造体

- MouseState.cs
-- RegulatedMouseButton: マウスの左・右・中とShift, Ctrl, Altの組合せパターンを必要分にまとめた列挙体
-- RegulatedMouseClickState: マウスの押下状況の列挙体．クリックとドラッグ
-- RegulatedMouseInfo: 押下の組合せと押下状況を保持する構造体
-- RegulatedMouseControl: マウスのDown, Up, Moveなどが行われた際の状態遷移を求めるクラス

- PathEx.cs
-- PathEx: モーションデータオブジェクトやシーケンスのパス名を操作するためのクラス

- ProcParam.cs
-- ProcParam: モーションデータオブジェクトやシーケンスに対するオペレーションに渡すための仮引数の基底クラス
-- BooleanProcParam: On/Offを表すチェックボックスを作成するためのProcParam継承クラス
-- NumberProcParam: 数値データ用の入力フォームを作成するためのProcParam継承クラス
-- StringProcParam: 文字列入力フォームを作成するためのProcParam継承クラス
-- SingleSelectProcParam: 項目選択用のリストボックスを作成するためのProcParam継承クラス

- ProgressInformation.cs
-- ProgressInformation: プログレスバー表示用の処理状況の値を保持するクラス

- RangeSet.cs
-- RangeSet: 数直線上の範囲の集合を保持するクラス．ObjectExistsビューでオブジェクトの欠損具合を保持するためのもの
-- RangeSet.Range: 範囲(開始位置と終了位置)を保持するクラス

- StreamEx.cs
-- StreamEx: ファイルの入出力用メソッド定義クラス

- TimeController.cs
-- TimeController: 再生時間の制御や，開始・終了時間，カーソル位置，表示範囲，選択範囲を管理するためのクラス
-- TimeController.InternalTimeInterval: ITimeIntervalを実装しない時間幅データがある場合に時間幅リストに加えるためのクラス

- TimePlayer.cs
-- TimePlayer: 再生や停止，時間の設定など行うUIクラス

- TimeSelectionControl.cs
-- TimeSelectionControl: 時間を示すためのルーラーを表示し，時間範囲を選択するためのUIクラス

- VectorEx.cs
-- VectorEx: 三次元ベクトルVector3用のメソッド定義クラス

- WaitForForm.cs
-- WaitForForm: 時間のかかる処理中に表示する用のプログレスバー付きダイアログのクラス
-- WaitForForm.WorkerController: WaitForFormに渡す引数をまとめたクラス

- WeakCollection.cs
-- WeakCollection: 弱参照オブジェクトのジェネリックコレクション．TimeControllerがITimeIntervalを保持する用のリストに使う．ITimeInterval側が登録解除し忘れてもそのうち勝手に解除させるためのもの

*** Motion/DefaultOperations/

- CreateObjectOperation.cs
-- OperationFixedPoint: 座標が変化しない点オブジェクトを作成するオペレーション用クラス
-- OperationLineCollisionPoint: 線分オブジェクトと他のオブジェクトとが交差する位置に点オブジェクトを作成するオペレーション用クラス
-- OperationSphereAroundPointFromSequence: 点オブジェクトを中心とし，半径をシーケンスの値とする球オブジェクトを作成するオペレーション用クラス
-- OperationSphereAroundPoint: 点オブジェクトを中心とする一定半径の球オブジェクトを作成するオペレーション用クラス
-- OperationPointBetweenPoints: 二つの点オブジェクトを内分/外分する点オブジェクトを作成するオペレーション用クラス
-- OperationPointOnLine: 線分オブジェクトの延長上に点オブジェクトを作成するオペレーション用クラス
-- OperationPointCenterOfSphere: 球オブジェクトの中心位置に点オブジェクトを作成するオペレーション用クラス
-- OperationPointCoordinateByTwoLines: 二線分とその法線から成る直交座標系上に固定された点オブジェクトを作成するオペレーション用クラス
-- OperationPointCoordinateByTwoLinesFromSequence: 二線分とその法線から成る直交座標系上でシーケンスの値を座標の値とする点オブジェクトを作成するオペレーション用クラス
-- OperationPointCoordinateByThreePoints: 三つの点オブジェクトから成る直交座標系上に固定された点オブジェクトを作成するオペレーション用クラス
-- OperationLineBetweenPoints: 二つの点オブジェクトを結ぶ線分オブジェクトを作成するオペレーション用クラス
-- OperationAxisLineOfCylinder: 円筒オブジェクトの中心軸から線分オブジェクトを作成するオペレーション用クラス
-- OperationLineBetweenPointAndNearestObject: 点オブジェクトから他のオブジェクトへの最短線分となる線分オブジェクトを作成するオペレーション用クラス
-- OperationLineBetweenClosestPointsOfTwoLines: 二線分に対し，それぞれの線分上にあり距離が最小になるような二点を結ぶ線分オブジェクトを作成するオペレーション用クラス
-- OperationLineTranslateToPoint: 起点または終点が点オブジェクト上になるように平行移動された線分オブジェクトを作成するオペレーション用クラス
-- OperationAddLines: 複数の線分の向きと長さをベクトルとみなし，ベクトル群の総和をとったベクトルを表す線分オブジェクトを作成するオペレーション用クラス
-- OperationCylinderAroundLineFromSequence: 線分オブジェクトを軸とし，半径をシーケンスの値とする円筒オブジェクトを作成するオペレーション用クラス
-- OperationCylinderAroundLine: 線分オブジェクトを軸とする一定半径の円筒オブジェクトを作成するオペレーション用クラス
-- OperationSphereContainingPoints: 複数の点オブジェクトを内包する最小の球オブジェクトを作成するオペレーション用クラス
-- OperationSphereContacting4Points: 四つの点オブジェクトに接する球オブジェクトを作成するオペレーション用クラス
-- OperationCylinderToFloowContainingPoints: 複数の点オブジェクトを内包し，Y軸と平行で一端がY=0となる円筒オブジェクトを作成するオペレーション用クラス
-- OperationPlaneOverPoints: 複数の点オブジェクトによって張られる平面オブジェクトを作成するオペレーション用クラス」
-- OperationLineFromPoint: 点オブジェクトを始点とし，向きが固定の線分オブジェクトを作成するオペレーション用クラス
-- OperationLineFromPointFromSequence: 点オブジェクトを始点とし，向きをシーケンスの値とする線分オブジェクトを作成するオペレーション用クラス

- EditObjectOperation.cs
-- OperationOffsetObject: オブジェクトを一定座標だけ平行移動させる編集オペレーション用クラス
-- OperationReverseLineDirection: 線分オブジェクトの向きを反転させる編集オペレーション用クラス
-- OperationResizeSphere: 球オブジェクトの半径を変更する編集オペレーション用クラス
-- OperationResizeCylinder: 円筒オブジェクトの半径を変更する編集オペレーション用クラス
-- OperationResizeLine: 線分オブジェクトの長さを変更する編集オペレーション用クラス
-- OperationLineParallelToFloor: 線分オブジェクトを床面と平行にする(向きのY成分を零にする)編集オペレーション用クラス

- GeneralOperations.cs
-- OperationRemoveObjects: モーションデータオブジェクトを削除するオペレーション用クラス
-- OperationRename: モーションデータオブジェクトのパスを変更するオペレーション用クラス
-- OperationInterpolateLinear: 欠落したフレームのオブジェクトを前後のフレームから補完するオペレーション用クラス
-- OperationClipFrame: 指定された範囲以外のフレームを削除するオペレーション用クラス

- OutputSequenceOperation.cs
-- OperationOutputPointPosition: 点オブジェクトの座標をシーケンスとして出力するオペレーション用クラス
-- OperationOutputLineDirection: 線分オブジェクトの始点から終点の相対座標をシーケンスとして出力するオペレーション用クラス
-- OperationOutputCylinderRadius: 円筒オブジェクトの半径をシーケンスとして出力するオペレーション用クラス
-- OperationOutputSphereRadius: 球オブジェクトの半径をシーケンスとして出力するオペレーション用クラス
-- OperationOutputPlaneArea: 平面オブジェクトの面積をシーケンスとして出力するオペレーション用クラス
-- OperationOutputRelativeCoordinate: 二線分とその法線から成る直交座標系上における点オブジェクトの座標をシーケンスとして出力するオペレーション用クラス
-- OperationOutputAngleBetweenLines: 二つの線分オブジェクトの成す角の角度をシーケンスとして出力するオペレーション用クラス
-- OperationOutputTotalLengthOfLines: 複数の線分オブジェクトの長さの総和をシーケンスとして出力するオペレーション用クラス
-- OperationOutputLineCollisionTarget: ある線分オブジェクトと他のオブジェクトとの交差の有無及び交差している他のオブジェクト名をラベル列として出力するオペレーション用クラス

*** Motion/Old/

- MotionDataFrame.cs
-- MotionDataFrame: 旧内部形式のモーションデータの一つのフレーム内のデータを保持するクラス．新形式以前に保存されたデータを読み込むために存在する

- MotionDataHeader.cs
-- MotionDataObjectType: 旧内部形式のモーションデータのオブジェクトの種類を表す列挙体．新形式以前に保存されたデータを読み込むために存在する
-- MotionDataObjectInfo: 旧内部形式のモーションデータのオブジェクトメタデータを保持するクラス．新形式以前に保存されたデータを読み込むために存在する
-- MotionDataHeader: 旧内部形式のモーションデータのメタデータリストを保持するクラス．新形式以前に保存されたデータを読み込むために存在する

- MotionDataObject.cs
-- IMotionDataObject: 旧内部形式のモーションデータの各オブジェクトのデータに共通するインターフェース．新形式以前に保存されたデータを読み込むために存在する
-- MotionDataPoint: 旧内部形式のモーションデータの一つのフレーム内の一つの点オブジェクトのデータを保持する構造体．新形式以前に保存されたデータを読み込むために存在する
-- MotionDataLine: 旧内部形式のモーションデータの一つのフレーム内の一つの線分オブジェクトのデータを保持する構造体．新形式以前に保存されたデータを読み込むために存在する
-- MotionDataCylinder: 旧内部形式のモーションデータの一つのフレーム内の一つの円筒オブジェクトのデータを保持する構造体．新形式以前に保存されたデータを読み込むために存在する
-- MotionDataSphere: 旧内部形式のモーションデータの一つのフレーム内の一つの球オブジェクトのデータを保持する構造体．新形式以前に保存されたデータを読み込むために存在する
-- MotionDataPlane: 旧内部形式のモーションデータの一つのフレーム内の一つの平面オブジェクトのデータを保持する構造体．新形式以前に保存されたデータを読み込むために存在する

- MotionDataSet.cs
-- MotionDataSet: 旧内部形式のモーションデータ全体を保持するクラス．新形式以前に保存されたデータを読み込むために存在する

*** Motion/Operation/

- MotionOperation.cs
-- booleanParameter: BooleanProcParam<MotionProcEnv> と等価．MotionOperation用仮引数クラス
-- NumberParameter: NumberProcParam<MotionProcEnv> と等価．MotionOperation用仮引数クラス
-- StringParameter: StringProcParam<MotionProcEnv> と等価．MotionOperation用仮引数クラス
-- SingleSelectParameterqwf: SingleSelectProcParam<MotionProcEnv> と等価．MotionOperation用仮引数クラス
-- MotionObjectSingleSelectParameter: MotionObjectを一つ選択するためのMotionOperation用仮引数クラス
-- MotionProcEnv: MotionOperationの実行の際に与えるデータをまとめたクラス
-- IMotionOperationBase: MotionOperationとしての機能を提供するためのインターフェース
-- IMotionOperationGeneral: その他のMotionOperationとして，IMotionOperationBaseを継承するインターフェース
-- IMotionOperationEditObject: 編集処理のMotionOperationとして，IMotionOperationBaseを継承するインターフェース
-- IMotionOperationCreateObject: オブジェクト作成処理のMotionOperationとして，IMotionOperationBaseを継承するインターフェース
-- IMotionOperationOutputSequence: シーケンス出力用のMotionOperationとして，IMotionOperationBaseを継承するインターフェース
-- MotionOperationScriptFunction: MotionOperationをスクリプト用関数として変換する用のクラス
-- MotionOperationExecution: MotionOperationを実行する際の実行環境クラス
-- MotionOperationEditToCreateWrapper: IMotionOperationEditObjectを継承するMotionOperationをIMotionOperationCreateObjectとして機能させるためのクラス
-- OperationMenuItem: MotionOperationとToolTipMenuItemの組を保持する構造体

- OperationMenuCreator.cs
-- OperationMenuCreator: IMotionOperationBaseのサブインターフェースを継承するクラスからメニューの作成・管理を行うクラス

- DialogMotionOperation.cs
-- DialogMotionOperation: MotionOperationのメニュー実行時に仮引数に値を設定するためのダイアログクラス

- DxCamera.cs
-- DxCamera: DirectXのビューカメラを操作するクラス

- GeometryCalc.cs
-- GeometryCalc: 座標計算用メソッド定義クラス

- IMotionObjectInfoReadable.cs
-- IMotionObjectInfoReadable: MotionObjectInfoを読み取り専用にする際のインターフェース

- MotionDataObjectListView.cs
-- MotionDataObjectListView: モーションデータオブジェクトの名前などを表示・変更するためのUIクラス

- MotionDataObjectSelectList.cs
-- MotionDataObjectSelectList: MotionOperation用の，モーションオブジェクトを選択する用のUIクラス

- MotionDataSet.cs
-- MotionDataSet: モーションデータオブジェクトのMotionObjectInfoやMotionObjectFrame全体を保持するクラス

- MotionDataViewer.cs
-- MotionDataViewer: モーションデータオブジェクトの3Dモデルを表示・操作するためのUIクラス

- MotionFieldState.cs
-- MotionFieldState: MotionDataViewerの座標系情報を保持する構造体

- MotionFrame.cs
-- MotionFrame: 各時点でのモーションデータオブジェクトのデータを保持するクラス
-- ReadOnlyMotionFrame: MotionFrameを読み取り専用にするラッパークラス

- MotionObject.cs
-- MotionObject: モーションデータ用のオブジェクトの共通メソッドとインターフェースを表す抽象基底クラス
-- PointObject: 点オブジェクトのデータを保持するクラス
-- LineObject: 線分オブジェクトのデータを保持するクラス
-- CylinderObject: 円筒オブジェクトのデータを保持するクラス
-- SphereObject: 球オブジェクトのデータを保持するクラス
-- PlaneObject: 平面オブジェクトのデータを保持するクラス

- MotionObjectInfo.cs
-- MotionObjectInfo: モーションオブジェクトのタイプ，パス名，ID，選択の有無，表示属性を保持するクラス
-- MotionObjectInfoList: IDをキーとしてMotionObjectInfoを取得するリストクラス

- ObjectExistenceView.cs
-- ObjectExistenceView: オブジェクトのデータ欠落を表示するUIクラス

- ReadOnlyMotionObjectInfo.cs
-- ReadOnlyMotionObjectInfo: モーションオペレーションの引数として渡すための，MotionObjectInfoを読み取り専用にするラッパークラス

- RenderPrimitive.cs
-- PolygonType: DirectXのPrimitiveTypeの代わりのポリゴンの種類を表す列挙体
-- PolygonVertex: DirectXのPositionNormalColoredの代わりの頂点リスト構造体
-- PolygonDatum: ポリゴンの頂点リストと種類の組を保持するクラス
-- PolygonRenderHint: ポリゴン描画時の表示オプション

*** Script/DefaultFunctions/

- Functions.cs
-- PrintFunction: 文字列を表示コンソールに出力するスクリプト関数を定義するクラス
-- PrintLnFunction: 文字列と改行を表示コンソールに出力するスクリプト関数を定義するクラス
-- CountLengthFunction: スクリプト用の配列変数の要素数を返すスクリプト関数を定義するクラス
-- UsageFunction: 関数の使用法を表示するスクリプト関数を定義するクラス
-- SleepFunction: 指定された秒数待機するスクリプト関数を定義するクラス
-- ToNumberFunction: スクリプト変数を数値変数に型変換するスクリプト関数を定義するクラス
-- ToStringFunction: スクリプト変数を文字列変数に型変換するスクリプト関数を定義するクラス
-- ToBooleanFunction: スクリプト変数を真偽値変数に型変換するスクリプト関数を定義するクラス
-- ToListFunction: スクリプト変数を配列変数に型変換するスクリプト関数を定義するクラス
-- ReferenceEqualsFunction: スクリプト変数の同一オブジェクト判定を行い真偽地変数を返すスクリプト関数を定義するクラス

*** Script/Parse/

- GenericLexParser.cs
-- GenericLexParser: 字句解析の汎用クラス
- LexicalElement.cs
-- LexType: 字句要素の種類を表す列挙体
-- LexicalElement: 一つの字句要素を表す構造体
- LexParser.cs
-- LexParser: スクリプト用字句解析クラス
- ScriptParser.cs
-- ScriptParser: 構文解析の結果ツリー作成クラス．構文解析処理はScriptParserBaseから継承
-- RunControlType: スクリプトの実行時のループbreakや関数returnなどを表す列挙体
- ScriptParser.txt
-- スクリプトの構文解析定義ファイル
- ScriptParserBase.cs
-- ScriptParserBase: 構文解析処理部分の抽象クラス
- SyntaxElement.cs
-- ProgramSyntaxElement: スクリプト全体を表す構文要素クラス
-- SyntaxElement: スクリプトの一つの文を表す構文要素基底クラス
-- ExpressionSyntaxElement: スクリプトの一つの式を表す構文要素基底クラス
-- NumberSyntaxElement: 数値リテラルを表す構文要素クラス
-- StringSyntaxElement: 文字列リテラルを表す構文要素クラス
-- BooleanSyntaxElement: 真偽値リテラルを表す構文要素クラス
-- NullSyntaxElement: NULL値リテラルを表す構文要素クラス
-- ListSyntaxElement: 配列リテラルを表す構文要素クラス
-- BinarySyntaxElement: 二項演算を表す構文要素クラス
-- DeclareSyntaxElement: 変数定義を表す構文要素クラス
-- SubstituteSyntaxElement: 代入式を表す構文要素クラス
-- MultiDeclareSyntaxElement: 複数の変数定義を表す構文要素クラス
-- AccessorSyntaxElement: 変数アクセス・配列アクセスを表す構文要素基底クラス
-- IdentifierSyntaxElement: 変数アクセスを表す構文要素クラス
-- IndexerSyntaxElement: 配列アクセス[]を表す構文要素クラス
-- FunctionSyntaxElement: 関数定義を表す構文要素クラス
-- InvokeSyntaxElement: 関数呼び出しを表す構文要素クラス
-- ControlSyntaxElement: break, continue, returnを表す構文要素クラス
-- AnarySyntaxElement: 単項演算+, -, !を表す構文要素クラス
-- IncrementSyntaxElement: 変数のインクリメント・デクリメントを表す構文要素クラス
-- TernarySyntaxElement: 三項演算子を表す構文要素クラス
-- IfSyntaxElement: if文を表す構文要素クラス
-- DoWhileSyntaxElement: do文，while文を表す構文要素クラス
-- ForSyntaxElement: for文を表す構文要素クラス
-- ForeachSyntaxElement: foreach文を表す構文要素クラス
-- TerminatedExpressionSyntaxElement: 単一式から成る文を表す構文要素クラス
-- BlockSyntaxElement: ブロック{}文を表す構文要素クラス
-- MultiExpressionSyntaxElement: 複数式から成る構文要素クラス
-- ArgumentsSyntaxElement: 関数呼び出しの実引数を表す構文要素クラス
-- DotInvocationSyntaxElement: ドットによる関数呼び出しを表す構文要素クラス
-- ParenthesisSyntaxElement: 括弧()式を表す構文要素クラス
- Utilities.cs
-- スクリプトの構文解析用の簡易クラス群
- VariableAccessor.cs
-- IVariableAccessor: 変数・配列アクセス用のインターフェース
-- IdentifierAccessor: 識別子による変数アクセス処理クラス
-- IndexedVariableAccessor: インデックス[]による変数アクセス処理クラス
-- NullAccessor: 無効なインデックスアクセス時に後続のインデックスを処理するためのクラス

*** Script/

- RefScriptVariable.cs
-- VariableNamedState: スクリプト内のリテラルを変数で置き換える際の置き換え状態を表す列挙体
-- RefScriptVariable: データ処理の実行履歴に対応するスクリプトを生成する中で，処理の戻り値を以降の処理の引数に利用するための名前付き変数クラス
-- ConcatString: 文字列リテラルを分解時の要素文字列と結合文字列のペアを保持するクラス
-- RefConcatStringVariable: 文字列リテラルを分解して各要素を変数に置き換える処理を行う名前付き変数クラス
-- RefListVariable: 名前付き変数のリストを保持する名前付き変数クラス
-- RefFunctionCallHistory: 名前付き引数を保持する関数呼び出し情報を持つ構造体
-- VariableValueEqualityComparer: スクリプト変数の一致を判定するクラス
-- VariableReplaceMap: 変数の値と変数の名前の相互変換を保持するクラス
-- ParameterizedHistories: 引数を変数で置き換えた関数呼び出し履歴を保持するクラス
- ScriptConsole.cs
-- ScriptMessageEventArgs: スクリプト処理の表示コンソールに渡される文字列を保持するイベント変数クラス
-- FunctionCallHistory: 一つのデータ処理呼び出しを保持する構造体
-- ObjectCorkBoard: スクリプト処理時にアクセスするためにモーションデータ全体や全シーケンスを保持するクラス
-- ScriptConsole: スクリプト実行時の母艦クラス
-- ScriptExecutionEnvironment: ScriptConsoleとVariableStorageを保持するクラス
-- IScriptFunction: スクリプトの関数を定義する際のインターフェース
-- ITimeConsumingScriptFunction: 処理に時間がかかる関数を定義する際のインターフェース
-- VariableStorage: 変数環境を保持するクラス
- ScriptVariable.cs
-- ScriptVariableType: スクリプト変数の型を表す列挙体
-- ScriptVariable: スクリプト変数の基底クラス
-- StringVariable: 文字列を保持するスクリプト変数クラス
-- NumberVariable: 数値を保持するスクリプト変数クラス
-- BooleanVariable: 真偽値を保持するスクリプト変数クラス
-- ListVariable: 配列を保持するスクリプト変数クラス
-- RegisteredFunctionVariable: IScriptFunctionを参照を保持するスクリプト変数クラス
-- FunctionVariable: ユーザ定義の関数の参照を保持するスクリプト変数クラス
-- ScriptVariableExtension: スクリプト変数間の演算等のメソッドを定義するクラス

*** Sequence/DefaultOperations/
- OperationCloneSequence.cs
-- OperationCloneSequence: シーケンスを複製するオペレーション用クラス
- OperationLabelBoundary.cs
-- OperationLabelBoundary: ラベル境界の前後を抽出したラベル列を作成するオペレーション用クラス
- OperationLabelSequence.cs
-- OperationLabelMerge: ラベル列の空白部分を他のラベル列で埋めるオペレーション用クラス
-- OperationLabelIntersect: ラベル列の共通部分を求めるオペレーション用クラス
-- OperationRemoveBorderSameAsPrevious: ラベル名の変化のないラベル境界を取り除くオペレーション用クラス
-- OperationLabelRename: ラベル名を変更するオペレーション用クラス
-- OperationLabelToNDimensionalBinarySequence: ラベル列を0/1の多次元数値シーケンスに変換するオペレーション用クラス
-- OperationLabelToNumberSequence: ラベル列を整数の数値シーケンスに変換するオペレーション用クラス
-- OperationLabelExtract: 指定されたラベル名を持つラベルのみを取り出すオペレーション用クラス
-- OperationLabelExtend: ラベルを伸縮させるオペレーション用クラス
-- OperationExtractEmptyLabel: ラベル列の空白部分を抽出するオペレーション用クラス
-- OperationRemoveLabelByLength: 長さが特定のラベルを除去するオペレーション用クラス
-- OperationExtractLabelContaining: あるラベル列のラベルのうち，他のラベル列で指定されたものと共通部分を持つラベルのみを抽出するオペレーション用クラス
-- OperationLabelDirectProductPerSelected: あるラベル列のラベルごとに，他のラベル列の共通部分のラベル名の直積を求めるオペレーション用クラス
-- OperationAppendAdjacentLabelName: 前後のラベル名をつなげたラベル列を作成するオペレーション用クラス
-- OperationPrecisionRecall: 正解ラベル列と比べての適合率・再現率を求めるオペレーション用クラス
-- OperationLabelStatistics: ラベル列の統計情報を表示するオペレーション用クラス
-- OperationLabelDirectProduct: ラベル列の各時点での直積を求めるオペレーション用クラス
- OperationNumericSequence.cs
-- OperationMovingAverage: 数値シーケンスの移動平均を求めるオペレーション用クラス
-- OperationToLabel: 数値シーケンスの値からラベルに変換するオペレーション用クラス
-- OperationClipByLabel: 数値シーケンスを他のラベル列のラベルでクリッピングしたシーケンスを求めるオペレーション用クラス
-- OperationNormalizeAsVector: 多次元数値シーケンスを二乗ノルムで1になるように正規化するオペレーション用クラス
-- OperationNormalizeBySum: 多次元数値シーケンスを各時点での合計が1になるように正規化するオペレーション用クラス
-- OperationStatistics: 数値シーケンスの統計情報を表示するオペレーション用クラス
-- OperationResampling: 数値シーケンスを再サンプリングするオペレーション用クラス
-- OperationGaussian: 数値シーケンスをガウシアンスムーシングするオペレーション用クラス
-- OperationArithmeticOperation: 数値シーケンス間で二項演算をするオペレーション用クラス
-- OperationInnerProduct: 数値シーケンス間で内積を求めるオペレーション用クラス
-- OperationAbsolute: 数値シーケンスの各次元の絶対値を求めるオペレーション用クラス
-- OperationDifferentiate: 数値シーケンスの微分を求めるオペレーション用クラス
-- OperationAverageFlat: 数値シーケンスの全体の平均を求めるオペレーション用クラス
-- OperationGreatest: 多次元数値シーケンスの各時点の最大・最小を求めるオペレーション用クラス
-- OperationExtractSequence: 多次元数値シーケンスの一部シーケンスを取り出すオペレーション用クラス
-- OperationMergeSequence: 数値シーケンスをまとめて多次元数値シーケンスにするオペレーション用クラス
-- OperationUnaryOperation: 数値シーケンスの単項演算を求めるオペレーション用クラス
-- OperationOffsetScale: 数値シーケンスの各時点の値を一次変換するオペレーション用クラス
-- OperationStandardNormalDistribution: 数値シーケンスを全体で平均が0，分散が1になるように一次変換するオペレーション用クラス

*** Sequence/Operation/

- SequenceOperation.cs
-- ISequenceOperation: シーケンスオペレーションを定義する際のインターフェース
-- SequenceOperationScriptFunction: シーケンスオペレーションをスクリプト関数として扱えるようにするラッパークラス
-- SequenceProcEnv: シーケンスオペレーションの実行の際に与えるデータをまとめたクラス

*** Sequence/ViewerFunction/

- DefaultViewerFunctions.cs
-- FunctionSetLabel: 範囲にラベルを設定するシーケンスビュー関数
-- FunctionSetLabelStart: 指定位置にラベル開始を設定するシーケンスビュー関数
-- FunctionSetLabelingBorder: ラベル設定数値境範囲を設定するシーケンスビュー関数
-- FunctionSetLabelingBorderStart:  ラベル設定数値境界値を設定するシーケンスビュー関数
-- FunctionGetTimeline: 指定された名前の数値シーケンスから数値のある時間の配列を返すシーケンスビュー関数
-- FunctionGetValues: 指定された名前の数値シーケンスから数値の配列または二重配列を返すシーケンスビュー関数
- ViewerFunction.cs
-- IViewerFunction: シーケンスビュー関数を定義する際のインターフェース
-- ViewerFunctionScriptFunction: IViewerFunctionをスクリプト関数として扱えるようにするラッパークラス

*** Sequence/

- BorderSelectControl.cs
-- BorderSelectControl: ラベル選択用のリストボックスクラス
- DialogCloseSequenceViewers.cs
-- DialogCloseSequenceViewers: 複数のシーケンスビューを閉じるダイアログクラス
- DialogLabelColorSet.cs
-- DialogLabelColorSet: ラベル列のラベルを色を変更するダイアログクラス
- DialogSequenceBorder.cs
-- DialogSequenceBorder: シーケンスをラベルにする閾値を設定するダイアログクラス
- DialogSequenceOperation.cs
-- DialogSequenceOperation: シーケンスオペレーションのメニュー実行時に仮引数に値を設定するためのダイアログクラス
- ICSLabelSequence.cs
-- ICSLabel: iCorpusStudio型のラベル列形式の一つのラベルデータを保持するクラス
-- ICSLabelSequence: iCorpusStudio型のラベル列データを保持するクラス
- LabelingBorders.cs
-- LabelingBorders: シーケンスをラベルにする閾値データを保持するクラス
- LabelJumpForm.cs
-- LabelJumpForm: ラベル検索ダイアログクラス
- LabelReplaceControl.cs
-- LabelReplaceControl: オペレーション用のラベル名変更パラメータ設定UIクラス
- LabelSequenceSelectControl.cs
-- LabelSequenceSelectControl: シーケンスオペレーション用シーケンス選択パラメータ設定UIクラス
- SequenceData.cs
-- SequenceType: シーケンスの種類を表す列挙体
-- SequenceData: 一つのシーケンス列(ラベル列または数値シーケンス)のデータを保持するクラス
- SequenceIndexSelectControl.cs
-- SequenceIndexSelectControl: 多次元数値シーケンスのうち一つ以上を選択するリストボックスクラス
- SequenceView.cs
-- SequenceView: 一つのシーケンス列を表示するUIクラス
- SequenceViewerController.cs
-- SequenceViewerController: シーケンスビュー全体を管理するクラス
- SequenceViewerInnerComponents.cs
-- SequenceViewerInnerComponents: シーケンスビュー用のダイアログまとめクラス
- TargetSequenceIndexControl.cs
-- TargetSequenceIndexControl: 多次元数値シーケンスのうち一つを選択するコンボボックスクラス
- TimeSeriesValues.cs
-- TimeSeriesRowValue: TimeSeriesValuesのある時刻のデータを表すクラス
-- TimeSeriesValues: シーケンス列内の数値シーケンスデータを保持するクラス
- TimeSeriesValuesCalculation.cs
-- TimeSeriesValuesCalculation: シーケンスオペレーション用の計算を行うメソッド定義クラス

** MotionDataUtil

*** Misc/

- ScriptFunctions.cs
-- FunctionOpenSequenceViewer: SequenceViewerFormを表示するスクリプト関数クラス

*** /

- ChainedSettings.cs
-- MotionDataUtilSettings: MotionDataUtil用のSettingsデータを保持するクラス
- MotionDataUtilityForm.cs
-- MotionDataUtilityForm: モーションデータ用のメインフォームクラス
- MotionDataUtilPlugin.cs
-- MotionDataUtilPlugin: MotionDataUtilityFormをiCorpusStudioのIPluginとして機能させるクラス
- MotionDataViewerForm.cs
-- MotionDataViewerForm: モーションデータオブジェクトを表示するだけのフォームクラス
- Program.cs
-- Program: Dllでなく実行ファイルとしてコンパイルした際のエントリポイント
- ScriptControlForm.cs
-- ScriptControlForm: スクリプト処理用のメインフォームクラス
- SequenceViewerForm.cs
-- SequenceViewerForm: シーケンス用のメインフォームクラス
- TimeControllerPlayerForm.cs
-- TimeControllerPlayerForm: 再生プレーヤフォームクラス
- TSeqViewerPlugin.cs
-- TSeqViewerPlugin: SequenceViewerFormをiCorpusStudioのIPluginとして機能させるクラス

** MotionDataUtilExe

*** /

- Program.cs
-- Program: MotionDataUtilityFormを実行ファイルとして直接起動する際のエントリポイント
