namespace SandBeige.MediaBox.DataBase.Tables.Metadata {
	/// <summary>
	/// Jpegメタデータテーブル
	/// </summary>
	public class Jpeg {
		/// <summary>
		/// メディアファイルID
		/// </summary>
		public long MediaFileId {
			get;
			set;
		}

		/// <summary>
		/// メディアファイル
		/// </summary>
		public MediaFile MediaFile {
			get;
			set;
		}

		/// <summary>
		/// 画像タイトル
		/// </summary>
		public string ImageDescription {
			get;
			set;
		}

		/// <summary>
		/// 画像入力機器のメーカー名
		/// </summary>
		public string Make {
			get;
			set;
		}

		/// <summary>
		/// 画像入力機器のモデル名
		/// </summary>
		public string Model {
			get;
			set;
		}

		/// <summary>
		/// 画像方向
		/// </summary>
		public short? Orientation {
			get;
			set;
		}

		/// <summary>
		/// 画像の幅の解像度
		/// </summary>
		public double? XResolution {
			get;
			set;
		}

		/// <summary>
		/// 画像の高さの解像度
		/// </summary>
		public double? YResolution {
			get;
			set;
		}

		/// <summary>
		/// 画像の幅と高さの解像度の単位
		/// </summary>
		public short? ResolutionUnit {
			get;
			set;
		}

		/// <summary>
		/// 再生階調カーブ特性(Short[3][25])
		/// </summary>
		public byte[] TransferFunction {
			get;
			set;
		}

		/// <summary>
		/// ソフトウェア
		/// </summary>
		public string Software {
			get;
			set;
		}

		/// <summary>
		/// ファイル変更日時
		/// </summary>
		public string DateTime {
			get;
			set;
		}

		/// <summary>
		/// アーティスト
		/// </summary>
		public string Artist {
			get;
			set;
		}

		/// <summary>
		/// 参照白色点の色度座標値(Rational[2])
		/// </summary>
		public byte[] WhitePoint {
			get;
			set;
		}

		/// <summary>
		/// 原色の色度座標値(Rational[6])
		/// </summary>
		public byte[] PrimaryChromaticities {
			get;
			set;
		}

		/// <summary>
		/// 色変換マトリクス係数(Rational[3])
		/// </summary>
		public byte[] YCbCrCoefficients {
			get;
			set;
		}

		/// <summary>
		/// YCCの画素構成(YとCの位置)
		/// </summary>
		public short? YCbCrPositioning {
			get;
			set;
		}

		/// <summary>
		/// 参照黒色点値と参照白色点値(Rational[6])
		/// </summary>
		public byte[] ReferenceBlackWhite {
			get;
			set;
		}

		/// <summary>
		/// 撮影著作権者/編集著作権者
		/// </summary>
		public string Copyright {
			get;
			set;
		}

		/// <summary>
		/// 露出時間 分母
		/// </summary>
		public long? ExposureTimeDenominator {
			get;
			set;
		}

		/// <summary>
		/// 露出時間 分子
		/// </summary>
		public long? ExposureTimeNumerator {
			get;
			set;
		}

		/// <summary>
		/// Fナンバー 分母
		/// </summary>
		public long? FNumberDenominator {
			get;
			set;
		}

		/// <summary>
		/// Fナンバー 分子
		/// </summary>
		public long? FNumberNumerator {
			get;
			set;
		}

		/// <summary>
		/// 露出プログラム
		/// </summary>
		public short? ExposureProgram {
			get;
			set;
		}

		/// <summary>
		/// スペクトル感度
		/// </summary>
		public string SpectralSensitivity {
			get;
			set;
		}

		/// <summary>
		/// 撮影感度(Short[])
		/// </summary>
		public byte[] PhotographicSensitivity {
			get;
			set;
		}

		/// <summary>
		/// 光電変換関数
		/// </summary>
		public byte[] OECF {
			get;
			set;
		}

		/// <summary>
		/// 感度種別
		/// </summary>
		public short? SensitivityType {
			get;
			set;
		}

		/// <summary>
		/// 標準出力感度
		/// </summary>
		public int? StandardOutputSensitivity {
			get;
			set;
		}

		/// <summary>
		/// 推奨露光指数
		/// </summary>
		public int? RecommendedExposureIndex {
			get;
			set;
		}

		/// <summary>
		/// ISOスピード
		/// </summary>
		public int? ISOSpeed {
			get;
			set;
		}

		/// <summary>
		/// ISOスピードラチチュードyyy
		/// </summary>
		public int? ISOSpeedLatitudeyyy {
			get;
			set;
		}

		/// <summary>
		/// ISOスピードラチチュードzzz
		/// </summary>
		public int? ISOSpeedLatitudezzz {
			get;
			set;
		}

		/// <summary>
		/// Exifバージョン
		/// </summary>
		public byte[] ExifVersion {
			get;
			set;
		}

		/// <summary>
		/// 原画像データの生成日時
		/// </summary>
		public string DateTimeOriginal {
			get;
			set;
		}

		/// <summary>
		/// デジタルデータの作成日時
		/// </summary>
		public string DateTimeDigitized {
			get;
			set;
		}

		/// <summary>
		/// 各コンポーネントの意味
		/// </summary>
		public byte[] ComponentsConfiguration {
			get;
			set;
		}

		/// <summary>
		/// 画像圧縮モード 分母
		/// </summary>
		public long? CompressedBitsPerPixelDenominator {
			get;
			set;
		}

		/// <summary>
		/// 画像圧縮モード 分子
		/// </summary>
		public long? CompressedBitsPerPixelNumerator {
			get;
			set;
		}

		/// <summary>
		/// シャッタースピード 分母
		/// </summary>
		public long? ShutterSpeedValueDenominator {
			get;
			set;
		}

		/// <summary>
		/// シャッタースピード 分子
		/// </summary>
		public long? ShutterSpeedValueNumerator {
			get;
			set;
		}

		/// <summary>
		/// 絞り値 分母
		/// </summary>
		public long? ApertureValueDenominator {
			get;
			set;
		}

		/// <summary>
		/// 絞り値 分子
		/// </summary>
		public long? ApertureValueNumerator {
			get;
			set;
		}

		/// <summary>
		/// 輝度値 分母
		/// </summary>
		public long? BrightnessValueDenominator {
			get;
			set;
		}

		/// <summary>
		/// 輝度値 分子
		/// </summary>
		public long? BrightnessValueNumerator {
			get;
			set;
		}

		/// <summary>
		/// 露光補正値 分母
		/// </summary>
		public long? ExposureBiasValueDenominator {
			get;
			set;
		}

		/// <summary>
		/// 露光補正値 分子
		/// </summary>
		public long? ExposureBiasValueNumerator {
			get;
			set;
		}

		/// <summary>
		/// レンズ最小Ｆ値 分母
		/// </summary>
		public long? MaxApertureValueDenominator {
			get;
			set;
		}

		/// <summary>
		/// レンズ最小Ｆ値 分子
		/// </summary>
		public long? MaxApertureValueNumerator {
			get;
			set;
		}

		/// <summary>
		/// 被写体距離 分母
		/// </summary>
		public long? SubjectDistanceDenominator {
			get;
			set;
		}

		/// <summary>
		/// 被写体距離 分子
		/// </summary>
		public long? SubjectDistanceNumerator {
			get;
			set;
		}

		/// <summary>
		/// 測光方式
		/// </summary>
		public short? MeteringMode {
			get;
			set;
		}

		/// <summary>
		/// 光源
		/// </summary>
		public short? LightSource {
			get;
			set;
		}

		/// <summary>
		/// フラッシュ
		/// </summary>
		public short? Flash {
			get;
			set;
		}

		/// <summary>
		/// レンズ焦点距離 分母
		/// </summary>
		public long? FocalLengthDenominator {
			get;
			set;
		}

		/// <summary>
		/// レンズ焦点距離 分子
		/// </summary>
		public long? FocalLengthNumerator {
			get;
			set;
		}

		/// <summary>
		/// 被写体領域 (Short[2|3|4])
		/// </summary>
		public byte[] SubjectArea {
			get;
			set;
		}

		/// <summary>
		/// メーカノート
		/// </summary>
		public byte[] MakerNote {
			get;
			set;
		}

		/// <summary>
		/// ユーザコメント
		/// </summary>
		public byte[] UserComment {
			get;
			set;
		}

		/// <summary>
		/// DateTimeのサブセック
		/// </summary>
		public string SubSecTime {
			get;
			set;
		}

		/// <summary>
		/// DateTimeOriginalのサブセック
		/// </summary>
		public string SubSecTimeOriginal {
			get;
			set;
		}

		/// <summary>
		/// DateTimeDigitizedのサブセック
		/// </summary>
		public string SubSecTimeDigitized {
			get;
			set;
		}

		/// <summary>
		/// 対応フラッシュピックスバージョン
		/// </summary>
		public byte[] FlashpixVersion {
			get;
			set;
		}

		/// <summary>
		/// 色空間情報
		/// </summary>
		public short? ColorSpace {
			get;
			set;
		}

		/// <summary>
		/// 実効画像幅
		/// </summary>
		public int? PixelXDimension {
			get;
			set;
		}

		/// <summary>
		/// 実効画像高さ
		/// </summary>
		public int? PixelYDimension {
			get;
			set;
		}

		/// <summary>
		/// 関連音声ファイル
		/// </summary>
		public string RelatedSoundFile {
			get;
			set;
		}

		/// <summary>
		/// フラッシュ強度 分母
		/// </summary>
		public long? FlashEnergyDenominator {
			get;
			set;
		}

		/// <summary>
		/// フラッシュ強度 分子
		/// </summary>
		public long? FlashEnergyNumerator {
			get;
			set;
		}

		/// <summary>
		/// 空間周波数応答
		/// </summary>
		public byte[] SpatialFrequencyResponse {
			get;
			set;
		}

		/// <summary>
		/// 焦点面の幅の解像度 分母
		/// </summary>
		public long? FocalPlaneXResolutionDenominator {
			get;
			set;
		}

		/// <summary>
		/// 焦点面の幅の解像度 分子
		/// </summary>
		public long? FocalPlaneXResolutionNumerator {
			get;
			set;
		}

		/// <summary>
		/// 焦点面の高さの解像度 分母
		/// </summary>
		public long? FocalPlaneYResolutionDenominator {
			get;
			set;
		}

		/// <summary>
		/// 焦点面の高さの解像度 分子
		/// </summary>
		public long? FocalPlaneYResolutionNumerator {
			get;
			set;
		}

		/// <summary>
		/// 焦点面解像度単位
		/// </summary>
		public short? FocalPlaneResolutionUnit {
			get;
			set;
		}

		/// <summary>
		/// 被写体位置 (Short[2])
		/// </summary>
		public byte[] SubjectLocation {
			get;
			set;
		}

		/// <summary>
		/// 露出インデックス 分母
		/// </summary>
		public long? ExposureIndexDenominator {
			get;
			set;
		}

		/// <summary>
		/// 露出インデックス 分子
		/// </summary>
		public long? ExposureIndexNumerator {
			get;
			set;
		}

		/// <summary>
		/// センサ方式
		/// </summary>
		public short? SensingMethod {
			get;
			set;
		}

		/// <summary>
		/// ファイルソース
		/// </summary>
		public int? FileSource {
			get;
			set;
		}

		/// <summary>
		/// シーンタイプ
		/// </summary>
		public int? SceneType {
			get;
			set;
		}

		/// <summary>
		/// CFAパターン
		/// </summary>
		public byte[] CFAPattern {
			get;
			set;
		}

		/// <summary>
		/// 個別画像処理
		/// </summary>
		public short? CustomRendered {
			get;
			set;
		}

		/// <summary>
		/// 露出モード
		/// </summary>
		public short? ExposureMode {
			get;
			set;
		}

		/// <summary>
		/// ホワイトバランス
		/// </summary>
		public short? WhiteBalance {
			get;
			set;
		}

		/// <summary>
		/// デジタルズーム倍率 分母
		/// </summary>
		public long? DigitalZoomRatioDenominator {
			get;
			set;
		}

		/// <summary>
		/// デジタルズーム倍率 分子
		/// </summary>
		public long? DigitalZoomRatioNumerator {
			get;
			set;
		}

		/// <summary>
		/// 35mm換算レンズ焦点距離
		/// </summary>
		public short? FocalLengthIn35mmFilm {
			get;
			set;
		}

		/// <summary>
		/// 撮影シーンタイプ
		/// </summary>
		public short? SceneCaptureType {
			get;
			set;
		}

		/// <summary>
		/// ゲイン制御
		/// </summary>
		public short? GainControl {
			get;
			set;
		}

		/// <summary>
		/// 撮影コントラスト
		/// </summary>
		public short? Contrast {
			get;
			set;
		}

		/// <summary>
		/// 撮影彩度
		/// </summary>
		public short? Saturation {
			get;
			set;
		}

		/// <summary>
		/// 撮影シャープネス
		/// </summary>
		public short? Sharpness {
			get;
			set;
		}

		/// <summary>
		/// 撮影条件記述情報
		/// </summary>
		public byte[] DeviceSettingDescription {
			get;
			set;
		}

		/// <summary>
		/// 被写体距離レンジ
		/// </summary>
		public short? SubjectDistanceRange {
			get;
			set;
		}

		/// <summary>
		/// 画像ユニークID
		/// </summary>
		public string ImageUniqueID {
			get;
			set;
		}

		/// <summary>
		/// カメラ所有者名
		/// </summary>
		public string CameraOwnerName {
			get;
			set;
		}

		/// <summary>
		/// カメラシリアル番号
		/// </summary>
		public string BodySerialNumber {
			get;
			set;
		}

		/// <summary>
		/// レンズの仕様情報(Rational[4])
		/// </summary>
		public byte[] LensSpecification {
			get;
			set;
		}

		/// <summary>
		/// レンズのメーカ名
		/// </summary>
		public string LensMake {
			get;
			set;
		}

		/// <summary>
		/// レンズのモデル名
		/// </summary>
		public string LensModel {
			get;
			set;
		}

		/// <summary>
		/// レンズシリアル番号
		/// </summary>
		public string LensSerialNumber {
			get;
			set;
		}

		/// <summary>
		/// 再生ガンマ 分母
		/// </summary>
		public long? GammaDenominator {
			get;
			set;
		}

		/// <summary>
		/// 再生ガンマ 分子
		/// </summary>
		public long? GammaNumerator {
			get;
			set;
		}

		/// <summary>
		/// GPSタグのバージョン
		/// </summary>
		public byte[] GPSVersionID {
			get;
			set;
		}

		/// <summary>
		/// 北緯(N) or 南緯(S)
		/// </summary>
		public string GPSLatitudeRef {
			get;
			set;
		}

		/// <summary>
		/// 緯度 度
		/// </summary>
		public double? GPSLatitudeDoa {
			get;
			set;
		}

		/// <summary>
		/// 緯度 分
		/// </summary>
		public double? GPSLatitudeMoa {
			get;
			set;
		}


		/// <summary>
		/// 緯度 秒
		/// </summary>
		public double? GPSLatitudeSoa {
			get;
			set;
		}


		/// <summary>
		/// 東経(E) or 西経(W)
		/// </summary>
		public string GPSLongitudeRef {
			get;
			set;
		}

		/// <summary>
		/// 経度 度
		/// </summary>
		public double? GPSLongitudeDoa {
			get;
			set;
		}

		/// <summary>
		/// 経度 分
		/// </summary>
		public double? GPSLongitudeMoa {
			get;
			set;
		}

		/// <summary>
		/// 経度 秒
		/// </summary>
		public double? GPSLongitudeSoa {
			get;
			set;
		}

		/// <summary>
		/// 高度の基準
		/// </summary>
		public int? GPSAltitudeRef {
			get;
			set;
		}

		/// <summary>
		/// 高度 分母
		/// </summary>
		public long? GPSAltitudeDenominator {
			get;
			set;
		}

		/// <summary>
		/// 高度 分子
		/// </summary>
		public long? GPSAltitudeNumerator {
			get;
			set;
		}

		/// <summary>
		/// GPS時間 時
		/// </summary>
		public double? GPSTimeStampHour {
			get;
			set;
		}

		/// <summary>
		/// GPS時間 分
		/// </summary>
		public double? GPSTimeStampMinutes {
			get;
			set;
		}

		/// <summary>
		/// GPS時間 秒
		/// </summary>
		public double? GPSTimeStampSeconds {
			get;
			set;
		}

		/// <summary>
		/// 測位に使った衛星信号
		/// </summary>
		public string GPSSatellites {
			get;
			set;
		}

		/// <summary>
		/// GPS受信機の状態
		/// </summary>
		public string GPSStatus {
			get;
			set;
		}

		/// <summary>
		/// GPSの測位方法
		/// </summary>
		public string GPSMeasureMode {
			get;
			set;
		}

		/// <summary>
		/// 測位の信頼性 分母
		/// </summary>
		public long? GPSDOPDenominator {
			get;
			set;
		}

		/// <summary>
		/// 測位の信頼性 分子
		/// </summary>
		public long? GPSDOPNumerator {
			get;
			set;
		}

		/// <summary>
		/// 速度の単位
		/// </summary>
		public string GPSSpeedRef {
			get;
			set;
		}

		/// <summary>
		/// 速度 分母
		/// </summary>
		public long? GPSSpeedDenominator {
			get;
			set;
		}

		/// <summary>
		/// 速度 分子
		/// </summary>
		public double? GPSSpeedNumerator {
			get;
			set;
		}

		/// <summary>
		/// 進行方向の単位
		/// </summary>
		public string GPSTrackRef {
			get;
			set;
		}

		/// <summary>
		/// 進行方向 分母
		/// </summary>
		public long? GPSTrackDenominator {
			get;
			set;
		}

		/// <summary>
		/// 進行方向 分子
		/// </summary>
		public long? GPSTrackNumerator {
			get;
			set;
		}

		/// <summary>
		/// 撮影した画像の方向の単位
		/// </summary>
		public string GPSImgDirectionRef {
			get;
			set;
		}

		/// <summary>
		/// 撮影した画像の方向 分母
		/// </summary>
		public long? GPSImgDirectionDenominator {
			get;
			set;
		}

		/// <summary>
		/// 撮影した画像の方向 分子
		/// </summary>
		public long? GPSImgDirectionNumerator {
			get;
			set;
		}

		/// <summary>
		/// 測位に用いた地図データ
		/// </summary>
		public string GPSMapDatum {
			get;
			set;
		}

		/// <summary>
		/// 目的地の北緯(N) or 南緯(S)
		/// </summary>
		public string GPSDestLatitudeRef {
			get;
			set;
		}

		/// <summary>
		/// 目的地の緯度 度
		/// </summary>
		public double? GPSDestLatitudeDoa {
			get;
			set;
		}

		/// <summary>
		/// 目的地の緯度 分
		/// </summary>
		public double? GPSDestLatitudeMoa {
			get;
			set;
		}

		/// <summary>
		/// 目的地の緯度 秒
		/// </summary>
		public double? GPSDestLatitudeSoa {
			get;
			set;
		}

		/// <summary>
		/// 目的地の東経(E) or 西経(W)
		/// </summary>
		public string GPSDestLongitudeRef {
			get;
			set;
		}

		/// <summary>
		/// 目的地の経度 度
		/// </summary>
		public double? GPSDestLongitudeDoa {
			get;
			set;
		}

		/// <summary>
		/// 目的地の経度 分
		/// </summary>
		public double? GPSDestLongitudeMoa {
			get;
			set;
		}

		/// <summary>
		/// 目的地の経度 秒
		/// </summary>
		public double? GPSDestLongitudeSoa {
			get;
			set;
		}

		/// <summary>
		/// 目的地の方角の単位
		/// </summary>
		public string GPSDestBearingRef {
			get;
			set;
		}

		/// <summary>
		/// 目的の方角 分母
		/// </summary>
		public long? GPSDestBearingDenominator {
			get;
			set;
		}

		/// <summary>
		/// 目的の方角 分子
		/// </summary>
		public long? GPSDestBearingNumerator {
			get;
			set;
		}

		/// <summary>
		/// 目的地までの距離の単位
		/// </summary>
		public string GPSDestDistanceRef {
			get;
			set;
		}

		/// <summary>
		/// 目的地までの距離 分母
		/// </summary>
		public long? GPSDestDistanceDenominator {
			get;
			set;
		}

		/// <summary>
		/// 目的地までの距離 分子
		/// </summary>
		public long? GPSDestDistanceNumerator {
			get;
			set;
		}

		/// <summary>
		/// 測位方式の名称
		/// </summary>
		public byte[] GPSProcessingMethod {
			get;
			set;
		}

		/// <summary>
		/// 測位地点の名称
		/// </summary>
		public byte[] GPSAreaInformation {
			get;
			set;
		}

		/// <summary>
		/// GPS日付
		/// </summary>
		public string GPSDateStamp {
			get;
			set;
		}

		/// <summary>
		/// GPS補正測位
		/// </summary>
		public short? GPSDifferential {
			get;
			set;
		}

		/// <summary>
		/// 水平方向測位誤差 分母
		/// </summary>
		public long? GPSHPositioningErrorDenominator {
			get;
			set;
		}

		/// <summary>
		/// 水平方向測位誤差 分子
		/// </summary>
		public long? GPSHPositioningErrorNumerator {
			get;
			set;
		}


	}
}
