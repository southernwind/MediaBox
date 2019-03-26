using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Jpegメタデータ取得クラス
	/// </summary>
	public class Jpeg : ImageBase {
		/// <summary>
		/// 幅
		/// </summary>
		public override int Width {
			get;
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public override int Height {
			get;
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public override Rational[] Latitude {
			get;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public override Rational[] Longitude {
			get;
		}

		/// <summary>
		/// 緯度方向(N/S)
		/// </summary>
		public override string LatitudeRef {
			get;
		}

		/// <summary>
		/// 経度方向(E/W)
		/// </summary>
		public override string LongitudeRef {
			get;
		}

		/// <summary>
		/// 画像の方向
		/// </summary>
		public override int? Orientation {
			get;
		}

		/// <summary>
		/// データベース登録用データ
		/// </summary>
		public DataBase.Tables.Metadata.Jpeg RowData {
			get;
		}

		/// <summary>
		/// メタデータの値と名前のペアのリストをを持つタグディレクトリのリスト
		/// </summary>
		public override Attributes<Attributes<string>> Properties {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>
		internal Jpeg(Stream stream) : base(stream) {
			var reader = JpegMetadataReader.ReadMetadata(stream);
			this.Properties = reader.ToProperties();
			var d = reader.First(x => x is JpegDirectory);
			var gps = reader.FirstOrDefault(x => x is GpsDirectory);
			var ifd0 = reader.FirstOrDefault(x => x is ExifDirectoryBase);
			this.Width = d.GetUInt16(JpegDirectory.TagImageWidth);
			this.Height = d.GetUInt16(JpegDirectory.TagImageHeight);

			if (ifd0 != null && ifd0.TryGetUInt16(ExifDirectoryBase.TagOrientation, out var orientation)) {
				this.Orientation = orientation;
			}

			if (gps != null) {
				this.Latitude = gps.GetRationalArray(GpsDirectory.TagLatitude);
				this.Longitude = gps.GetRationalArray(GpsDirectory.TagLongitude);
				this.LatitudeRef = gps.GetString(GpsDirectory.TagLatitudeRef);
				this.LongitudeRef = gps.GetString(GpsDirectory.TagLatitudeRef);
			}

			this.RowData = new DataBase.Tables.Metadata.Jpeg();
			if (ifd0 != null) {
				this.RowData.ImageDescription = this.GetString(ifd0, ExifDirectoryBase.TagImageDescription);
				this.RowData.ImageDescription = this.GetString(ifd0, ExifDirectoryBase.TagImageDescription);
				this.RowData.Make = this.GetString(ifd0, ExifDirectoryBase.TagMake);
				this.RowData.Model = this.GetString(ifd0, ExifDirectoryBase.TagModel);
				this.RowData.Orientation = this.GetShort(ifd0, ExifDirectoryBase.TagOrientation);
				this.RowData.XResolution = this.GetInt(ifd0, ExifDirectoryBase.TagXResolution);
				this.RowData.YResolution = this.GetInt(ifd0, ExifDirectoryBase.TagYResolution);
				this.RowData.ResolutionUnit = this.GetShort(ifd0, ExifDirectoryBase.TagResolutionUnit);
				this.RowData.TransferFunction = this.GetBinary(ifd0, ExifDirectoryBase.TagTransferFunction);
				this.RowData.Software = this.GetString(ifd0, ExifDirectoryBase.TagSoftware);
				this.RowData.DateTime = this.GetString(ifd0, ExifDirectoryBase.TagDateTime);
				this.RowData.Artist = this.GetString(ifd0, ExifDirectoryBase.TagArtist);
				this.RowData.WhitePoint = this.GetBinary(ifd0, ExifDirectoryBase.TagWhitePoint);
				this.RowData.PrimaryChromaticities = this.GetBinary(ifd0, ExifDirectoryBase.TagPrimaryChromaticities);
				this.RowData.YCbCrCoefficients = this.GetBinary(ifd0, ExifDirectoryBase.TagYCbCrCoefficients);
				this.RowData.YCbCrPositioning = this.GetShort(ifd0, ExifDirectoryBase.TagYCbCrPositioning);
				this.RowData.ReferenceBlackWhite = this.GetBinary(ifd0, ExifDirectoryBase.TagReferenceBlackWhite);
				this.RowData.Copyright = this.GetString(ifd0, ExifDirectoryBase.TagCopyright);
				(this.RowData.ExposureTimeDenominator, this.RowData.ExposureTimeNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagExposureTime);
				(this.RowData.FNumberDenominator, this.RowData.FNumberNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagFNumber);
				this.RowData.ExposureProgram = this.GetShort(ifd0, ExifDirectoryBase.TagExposureProgram);
				this.RowData.SpectralSensitivity = this.GetString(ifd0, ExifDirectoryBase.TagSpectralSensitivity);
				this.RowData.OECF = this.GetBinary(ifd0, ExifDirectoryBase.TagOptoElectricConversionFunction);
				this.RowData.SensitivityType = this.GetShort(ifd0, ExifDirectoryBase.TagSensitivityType);
				this.RowData.StandardOutputSensitivity = this.GetInt(ifd0, ExifDirectoryBase.TagStandardOutputSensitivity);
				this.RowData.RecommendedExposureIndex = this.GetInt(ifd0, ExifDirectoryBase.TagRecommendedExposureIndex);
				this.RowData.ExifVersion = this.GetBinary(ifd0, ExifDirectoryBase.TagExifVersion);
				this.RowData.DateTimeOriginal = this.GetString(ifd0, ExifDirectoryBase.TagDateTimeOriginal);
				this.RowData.DateTimeDigitized = this.GetString(ifd0, ExifDirectoryBase.TagDateTimeDigitized);
				this.RowData.ComponentsConfiguration = this.GetBinary(ifd0, ExifDirectoryBase.TagComponentsConfiguration);
				(this.RowData.CompressedBitsPerPixelDenominator, this.RowData.CompressedBitsPerPixelNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagCompressedAverageBitsPerPixel);
				(this.RowData.ShutterSpeedValueDenominator, this.RowData.ShutterSpeedValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagShutterSpeed);
				(this.RowData.ApertureValueDenominator, this.RowData.ApertureValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagAperture);
				(this.RowData.BrightnessValueDenominator, this.RowData.BrightnessValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagBrightnessValue);
				(this.RowData.ExposureBiasValueDenominator, this.RowData.ExposureBiasValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagExposureBias);
				(this.RowData.MaxApertureValueDenominator, this.RowData.MaxApertureValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagMaxAperture);
				(this.RowData.SubjectDistanceDenominator, this.RowData.SubjectDistanceNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagSubjectDistance);
				this.RowData.MeteringMode = this.GetShort(ifd0, ExifDirectoryBase.TagMeteringMode);
				this.RowData.Flash = this.GetShort(ifd0, ExifDirectoryBase.TagFlash);
				(this.RowData.FocalLengthDenominator, this.RowData.FocalLengthNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagFocalLength);
				this.RowData.MakerNote = this.GetBinary(ifd0, ExifDirectoryBase.TagMakernote);
				this.RowData.UserComment = this.GetBinary(ifd0, ExifDirectoryBase.TagUserComment);
				this.RowData.SubSecTime = this.GetString(ifd0, ExifDirectoryBase.TagSubsecondTime);
				this.RowData.SubSecTimeOriginal = this.GetString(ifd0, ExifDirectoryBase.TagSubsecondTimeOriginal);
				this.RowData.SubSecTimeDigitized = this.GetString(ifd0, ExifDirectoryBase.TagSubsecondTimeDigitized);
				this.RowData.FlashpixVersion = this.GetBinary(ifd0, ExifDirectoryBase.TagFlashpixVersion);
				this.RowData.ColorSpace = this.GetShort(ifd0, ExifDirectoryBase.TagColorSpace);
				this.RowData.RelatedSoundFile = this.GetString(ifd0, ExifDirectoryBase.TagRelatedSoundFile);
				(this.RowData.FlashEnergyDenominator, this.RowData.FlashEnergyNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagFlashEnergy);
				(this.RowData.FocalPlaneYResolutionDenominator, this.RowData.FocalPlaneYResolutionNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagFocalPlaneYResolution);
				this.RowData.FocalPlaneResolutionUnit = this.GetShort(ifd0, ExifDirectoryBase.TagFocalPlaneResolutionUnit);
				this.RowData.FocalPlaneResolutionUnit = this.GetShort(ifd0, ExifDirectoryBase.TagFocalPlaneResolutionUnit);
				this.RowData.SubjectLocation = this.GetBinary(ifd0, ExifDirectoryBase.TagSubjectLocation);
				(this.RowData.ExposureIndexDenominator, this.RowData.ExposureIndexNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagExposureIndex);
				this.RowData.SensingMethod = this.GetShort(ifd0, ExifDirectoryBase.TagSensingMethod);
				this.RowData.FileSource = this.GetInt(ifd0, ExifDirectoryBase.TagFileSource);
				this.RowData.SceneType = this.GetInt(ifd0, ExifDirectoryBase.TagSceneType);
				this.RowData.CFAPattern = this.GetBinary(ifd0, ExifDirectoryBase.TagCfaPattern);
				this.RowData.CustomRendered = this.GetShort(ifd0, ExifDirectoryBase.TagCustomRendered);
				this.RowData.ExposureMode = this.GetShort(ifd0, ExifDirectoryBase.TagExposureMode);
				this.RowData.WhiteBalance = this.GetShort(ifd0, ExifDirectoryBase.TagWhiteBalance);
				(this.RowData.DigitalZoomRatioDenominator, this.RowData.DigitalZoomRatioNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagDigitalZoomRatio);
				this.RowData.FocalLengthIn35mmFilm = this.GetShort(ifd0, ExifDirectoryBase.Tag35MMFilmEquivFocalLength);
				this.RowData.SceneCaptureType = this.GetShort(ifd0, ExifDirectoryBase.TagSceneCaptureType);
				this.RowData.GainControl = this.GetShort(ifd0, ExifDirectoryBase.TagGainControl);
				this.RowData.Contrast = this.GetShort(ifd0, ExifDirectoryBase.TagContrast);
				this.RowData.Saturation = this.GetShort(ifd0, ExifDirectoryBase.TagSaturation);
				this.RowData.Sharpness = this.GetShort(ifd0, ExifDirectoryBase.TagSharpness);
				this.RowData.DeviceSettingDescription = this.GetBinary(ifd0, ExifDirectoryBase.TagDeviceSettingDescription);
				this.RowData.SubjectDistanceRange = this.GetShort(ifd0, ExifDirectoryBase.TagSubjectDistanceRange);
				this.RowData.ImageUniqueID = this.GetString(ifd0, ExifDirectoryBase.TagImageUniqueId);
				this.RowData.CameraOwnerName = this.GetString(ifd0, ExifDirectoryBase.TagCameraOwnerName);
				this.RowData.BodySerialNumber = this.GetString(ifd0, ExifDirectoryBase.TagBodySerialNumber);
				this.RowData.LensSpecification = this.GetBinary(ifd0, ExifDirectoryBase.TagLensSpecification);
				this.RowData.LensMake = this.GetString(ifd0, ExifDirectoryBase.TagLensMake);
				this.RowData.LensModel = this.GetString(ifd0, ExifDirectoryBase.TagLensModel);
				this.RowData.LensSerialNumber = this.GetString(ifd0, ExifDirectoryBase.TagLensSerialNumber);
				(this.RowData.GammaDenominator, this.RowData.GammaNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagGamma);
			}
			if (gps != null) {
				this.RowData.GPSVersionID = this.GetBinary(gps, GpsDirectory.TagVersionId);
				this.RowData.GPSLatitudeRef = this.GetString(gps, GpsDirectory.TagLatitudeRef);
				(this.RowData.GPSLatitudeDoa, this.RowData.GPSLatitudeMoa, this.RowData.GPSLatitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagLatitude);
				this.RowData.GPSLongitudeRef = this.GetString(gps, GpsDirectory.TagLongitudeRef);
				(this.RowData.GPSLongitudeDoa, this.RowData.GPSLongitudeMoa, this.RowData.GPSLongitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagLongitude);
				this.RowData.GPSAltitudeRef = this.GetInt(gps, GpsDirectory.TagAltitudeRef);
				(this.RowData.GPSAltitudeDenominator, this.RowData.GPSAltitudeNumerator) = this.GetRational(gps, GpsDirectory.TagAltitude);
				(this.RowData.GPSTimeStampHour, this.RowData.GPSTimeStampMinutes, this.RowData.GPSTimeStampSeconds) = this.Get3Rational(gps, GpsDirectory.TagTimeStamp);
				this.RowData.GPSSatellites = this.GetString(gps, GpsDirectory.TagSatellites);
				this.RowData.GPSStatus = this.GetString(gps, GpsDirectory.TagStatus);
				this.RowData.GPSMeasureMode = this.GetString(gps, GpsDirectory.TagMeasureMode);
				(this.RowData.GPSDOPDenominator, this.RowData.GPSDOPNumerator) = this.GetRational(gps, GpsDirectory.TagDop);
				this.RowData.GPSSpeedRef = this.GetString(gps, GpsDirectory.TagSpeedRef);
				(this.RowData.GPSSpeedDenominator, this.RowData.GPSSpeedNumerator) = this.GetRational(gps, GpsDirectory.TagSpeed);
				this.RowData.GPSTrackRef = this.GetString(gps, GpsDirectory.TagTrackRef);
				(this.RowData.GPSTrackDenominator, this.RowData.GPSTrackNumerator) = this.GetRational(gps, GpsDirectory.TagTrack);
				this.RowData.GPSImgDirectionRef = this.GetString(gps, GpsDirectory.TagImgDirectionRef);
				(this.RowData.GPSImgDirectionDenominator, this.RowData.GPSImgDirectionNumerator) = this.GetRational(gps, GpsDirectory.TagImgDirection);
				this.RowData.GPSMapDatum = this.GetString(gps, GpsDirectory.TagMapDatum);
				this.RowData.GPSDestLatitudeRef = this.GetString(gps, GpsDirectory.TagDestLatitudeRef);
				(this.RowData.GPSDestLatitudeDoa, this.RowData.GPSDestLatitudeMoa, this.RowData.GPSDestLatitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagDestLatitude);
				this.RowData.GPSDestLongitudeRef = this.GetString(gps, GpsDirectory.TagDestLongitudeRef);
				(this.RowData.GPSDestLongitudeSoa, this.RowData.GPSDestLongitudeMoa, this.RowData.GPSDestLongitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagDestLongitude);
				this.RowData.GPSDestBearingRef = this.GetString(gps, GpsDirectory.TagDestBearingRef);
				(this.RowData.GPSDestBearingDenominator, this.RowData.GPSDestBearingNumerator) = this.GetRational(gps, GpsDirectory.TagDestBearing);
				this.RowData.GPSDestDistanceRef = this.GetString(gps, GpsDirectory.TagDestDistanceRef);
				(this.RowData.GPSDestDistanceDenominator, this.RowData.GPSDestDistanceNumerator) = this.GetRational(gps, GpsDirectory.TagDestDistance);
				this.RowData.GPSProcessingMethod = this.GetBinary(gps, GpsDirectory.TagProcessingMethod);
				this.RowData.GPSAreaInformation = this.GetBinary(gps, GpsDirectory.TagAreaInformation);
				this.RowData.GPSDateStamp = this.GetString(gps, GpsDirectory.TagDateStamp);
				this.RowData.GPSDifferential = this.GetShort(gps, GpsDirectory.TagDifferential);
			}
		}

		/// <summary>
		/// 文字取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private string GetString(MetadataExtractor.Directory directory, int tag) {
			return directory.GetString(tag);
		}

		/// <summary>
		/// 数値取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private short GetShort(MetadataExtractor.Directory directory, int tag) {
			if (directory.TryGetInt16(tag, out var value)) {
				return value;
			}
			return default;
		}

		/// <summary>
		/// 数値取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private int GetInt(MetadataExtractor.Directory directory, int tag) {
			if (directory.TryGetInt32(tag, out var value)) {
				return value;
			}
			return default;
		}

		/// <summary>
		/// 文字型取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private byte[] GetBinary(MetadataExtractor.Directory directory, int tag) {
			return directory.GetByteArray(tag);
		}

		/// <summary>
		/// 文字型取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private (long, long) GetRational(MetadataExtractor.Directory directory, int tag) {
			if (directory.TryGetRational(tag, out var value)) {
				return (value.Denominator, value.Numerator);
			}
			return default;
		}

		/// <summary>
		/// 文字型取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private (double, double, double) Get3Rational(MetadataExtractor.Directory directory, int tag) {
			var value = directory.GetRationalArray(tag);
			if (value?.Length == 3) {
				return (value[0].ToDouble(), value[1].ToDouble(), value[2].ToDouble());
			}

			return default;
		}
	}
}
