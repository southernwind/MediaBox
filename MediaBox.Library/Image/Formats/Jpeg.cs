using System.Collections.Generic;
using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Jpegメタデータ取得クラス
	/// </summary>
	public class Jpeg : ImageBase {
		private readonly IReadOnlyList<MetadataExtractor.Directory> _reader;

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
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>
		internal Jpeg(Stream stream) : base(stream) {
			this._reader = JpegMetadataReader.ReadMetadata(stream);
			var d = this._reader.First(x => x is JpegDirectory);
			var gps = this._reader.FirstOrDefault(x => x is GpsDirectory);
			var ifd0 = this._reader.FirstOrDefault(x => x is ExifDirectoryBase);
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
		}

		public void UpdateRowdata(DataBase.Tables.Metadata.Jpeg rowdata) {
			var gps = this._reader.FirstOrDefault(x => x is GpsDirectory);
			var ifd0 = this._reader.FirstOrDefault(x => x is ExifDirectoryBase);

			if (ifd0 != null) {
				rowdata.ImageDescription = this.GetString(ifd0, ExifDirectoryBase.TagImageDescription);
				rowdata.ImageDescription = this.GetString(ifd0, ExifDirectoryBase.TagImageDescription);
				rowdata.Make = this.GetString(ifd0, ExifDirectoryBase.TagMake);
				rowdata.Model = this.GetString(ifd0, ExifDirectoryBase.TagModel);
				rowdata.Orientation = this.GetShort(ifd0, ExifDirectoryBase.TagOrientation);
				rowdata.XResolution = this.GetInt(ifd0, ExifDirectoryBase.TagXResolution);
				rowdata.YResolution = this.GetInt(ifd0, ExifDirectoryBase.TagYResolution);
				rowdata.ResolutionUnit = this.GetShort(ifd0, ExifDirectoryBase.TagResolutionUnit);
				rowdata.TransferFunction = this.GetBinary(ifd0, ExifDirectoryBase.TagTransferFunction);
				rowdata.Software = this.GetString(ifd0, ExifDirectoryBase.TagSoftware);
				rowdata.DateTime = this.GetString(ifd0, ExifDirectoryBase.TagDateTime);
				rowdata.Artist = this.GetString(ifd0, ExifDirectoryBase.TagArtist);
				rowdata.WhitePoint = this.GetBinary(ifd0, ExifDirectoryBase.TagWhitePoint);
				rowdata.PrimaryChromaticities = this.GetBinary(ifd0, ExifDirectoryBase.TagPrimaryChromaticities);
				rowdata.YCbCrCoefficients = this.GetBinary(ifd0, ExifDirectoryBase.TagYCbCrCoefficients);
				rowdata.YCbCrPositioning = this.GetShort(ifd0, ExifDirectoryBase.TagYCbCrPositioning);
				rowdata.ReferenceBlackWhite = this.GetBinary(ifd0, ExifDirectoryBase.TagReferenceBlackWhite);
				rowdata.Copyright = this.GetString(ifd0, ExifDirectoryBase.TagCopyright);
				(rowdata.ExposureTimeDenominator, rowdata.ExposureTimeNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagExposureTime);
				(rowdata.FNumberDenominator, rowdata.FNumberNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagFNumber);
				rowdata.ExposureProgram = this.GetShort(ifd0, ExifDirectoryBase.TagExposureProgram);
				rowdata.SpectralSensitivity = this.GetString(ifd0, ExifDirectoryBase.TagSpectralSensitivity);
				rowdata.OECF = this.GetBinary(ifd0, ExifDirectoryBase.TagOptoElectricConversionFunction);
				rowdata.SensitivityType = this.GetShort(ifd0, ExifDirectoryBase.TagSensitivityType);
				rowdata.StandardOutputSensitivity = this.GetInt(ifd0, ExifDirectoryBase.TagStandardOutputSensitivity);
				rowdata.RecommendedExposureIndex = this.GetInt(ifd0, ExifDirectoryBase.TagRecommendedExposureIndex);
				rowdata.ExifVersion = this.GetBinary(ifd0, ExifDirectoryBase.TagExifVersion);
				rowdata.DateTimeOriginal = this.GetString(ifd0, ExifDirectoryBase.TagDateTimeOriginal);
				rowdata.DateTimeDigitized = this.GetString(ifd0, ExifDirectoryBase.TagDateTimeDigitized);
				rowdata.ComponentsConfiguration = this.GetBinary(ifd0, ExifDirectoryBase.TagComponentsConfiguration);
				(rowdata.CompressedBitsPerPixelDenominator, rowdata.CompressedBitsPerPixelNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagCompressedAverageBitsPerPixel);
				(rowdata.ShutterSpeedValueDenominator, rowdata.ShutterSpeedValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagShutterSpeed);
				(rowdata.ApertureValueDenominator, rowdata.ApertureValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagAperture);
				(rowdata.BrightnessValueDenominator, rowdata.BrightnessValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagBrightnessValue);
				(rowdata.ExposureBiasValueDenominator, rowdata.ExposureBiasValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagExposureBias);
				(rowdata.MaxApertureValueDenominator, rowdata.MaxApertureValueNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagMaxAperture);
				(rowdata.SubjectDistanceDenominator, rowdata.SubjectDistanceNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagSubjectDistance);
				rowdata.MeteringMode = this.GetShort(ifd0, ExifDirectoryBase.TagMeteringMode);
				rowdata.Flash = this.GetShort(ifd0, ExifDirectoryBase.TagFlash);
				(rowdata.FocalLengthDenominator, rowdata.FocalLengthNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagFocalLength);
				rowdata.MakerNote = this.GetBinary(ifd0, ExifDirectoryBase.TagMakernote);
				rowdata.UserComment = this.GetBinary(ifd0, ExifDirectoryBase.TagUserComment);
				rowdata.SubSecTime = this.GetString(ifd0, ExifDirectoryBase.TagSubsecondTime);
				rowdata.SubSecTimeOriginal = this.GetString(ifd0, ExifDirectoryBase.TagSubsecondTimeOriginal);
				rowdata.SubSecTimeDigitized = this.GetString(ifd0, ExifDirectoryBase.TagSubsecondTimeDigitized);
				rowdata.FlashpixVersion = this.GetBinary(ifd0, ExifDirectoryBase.TagFlashpixVersion);
				rowdata.ColorSpace = this.GetShort(ifd0, ExifDirectoryBase.TagColorSpace);
				rowdata.RelatedSoundFile = this.GetString(ifd0, ExifDirectoryBase.TagRelatedSoundFile);
				(rowdata.FlashEnergyDenominator, rowdata.FlashEnergyNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagFlashEnergy);
				(rowdata.FocalPlaneYResolutionDenominator, rowdata.FocalPlaneYResolutionNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagFocalPlaneYResolution);
				rowdata.FocalPlaneResolutionUnit = this.GetShort(ifd0, ExifDirectoryBase.TagFocalPlaneResolutionUnit);
				rowdata.FocalPlaneResolutionUnit = this.GetShort(ifd0, ExifDirectoryBase.TagFocalPlaneResolutionUnit);
				rowdata.SubjectLocation = this.GetBinary(ifd0, ExifDirectoryBase.TagSubjectLocation);
				(rowdata.ExposureIndexDenominator, rowdata.ExposureIndexNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagExposureIndex);
				rowdata.SensingMethod = this.GetShort(ifd0, ExifDirectoryBase.TagSensingMethod);
				rowdata.FileSource = this.GetInt(ifd0, ExifDirectoryBase.TagFileSource);
				rowdata.SceneType = this.GetInt(ifd0, ExifDirectoryBase.TagSceneType);
				rowdata.CFAPattern = this.GetBinary(ifd0, ExifDirectoryBase.TagCfaPattern);
				rowdata.CustomRendered = this.GetShort(ifd0, ExifDirectoryBase.TagCustomRendered);
				rowdata.ExposureMode = this.GetShort(ifd0, ExifDirectoryBase.TagExposureMode);
				rowdata.WhiteBalance = this.GetShort(ifd0, ExifDirectoryBase.TagWhiteBalance);
				(rowdata.DigitalZoomRatioDenominator, rowdata.DigitalZoomRatioNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagDigitalZoomRatio);
				rowdata.FocalLengthIn35mmFilm = this.GetShort(ifd0, ExifDirectoryBase.Tag35MMFilmEquivFocalLength);
				rowdata.SceneCaptureType = this.GetShort(ifd0, ExifDirectoryBase.TagSceneCaptureType);
				rowdata.GainControl = this.GetShort(ifd0, ExifDirectoryBase.TagGainControl);
				rowdata.Contrast = this.GetShort(ifd0, ExifDirectoryBase.TagContrast);
				rowdata.Saturation = this.GetShort(ifd0, ExifDirectoryBase.TagSaturation);
				rowdata.Sharpness = this.GetShort(ifd0, ExifDirectoryBase.TagSharpness);
				rowdata.DeviceSettingDescription = this.GetBinary(ifd0, ExifDirectoryBase.TagDeviceSettingDescription);
				rowdata.SubjectDistanceRange = this.GetShort(ifd0, ExifDirectoryBase.TagSubjectDistanceRange);
				rowdata.ImageUniqueID = this.GetString(ifd0, ExifDirectoryBase.TagImageUniqueId);
				rowdata.CameraOwnerName = this.GetString(ifd0, ExifDirectoryBase.TagCameraOwnerName);
				rowdata.BodySerialNumber = this.GetString(ifd0, ExifDirectoryBase.TagBodySerialNumber);
				rowdata.LensSpecification = this.GetBinary(ifd0, ExifDirectoryBase.TagLensSpecification);
				rowdata.LensMake = this.GetString(ifd0, ExifDirectoryBase.TagLensMake);
				rowdata.LensModel = this.GetString(ifd0, ExifDirectoryBase.TagLensModel);
				rowdata.LensSerialNumber = this.GetString(ifd0, ExifDirectoryBase.TagLensSerialNumber);
				(rowdata.GammaDenominator, rowdata.GammaNumerator) = this.GetRational(ifd0, ExifDirectoryBase.TagGamma);
			}
			if (gps != null) {
				rowdata.GPSVersionID = this.GetBinary(gps, GpsDirectory.TagVersionId);
				rowdata.GPSLatitudeRef = this.GetString(gps, GpsDirectory.TagLatitudeRef);
				(rowdata.GPSLatitudeDoa, rowdata.GPSLatitudeMoa, rowdata.GPSLatitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagLatitude);
				rowdata.GPSLongitudeRef = this.GetString(gps, GpsDirectory.TagLongitudeRef);
				(rowdata.GPSLongitudeDoa, rowdata.GPSLongitudeMoa, rowdata.GPSLongitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagLongitude);
				rowdata.GPSAltitudeRef = this.GetInt(gps, GpsDirectory.TagAltitudeRef);
				(rowdata.GPSAltitudeDenominator, rowdata.GPSAltitudeNumerator) = this.GetRational(gps, GpsDirectory.TagAltitude);
				(rowdata.GPSTimeStampHour, rowdata.GPSTimeStampMinutes, rowdata.GPSTimeStampSeconds) = this.Get3Rational(gps, GpsDirectory.TagTimeStamp);
				rowdata.GPSSatellites = this.GetString(gps, GpsDirectory.TagSatellites);
				rowdata.GPSStatus = this.GetString(gps, GpsDirectory.TagStatus);
				rowdata.GPSMeasureMode = this.GetString(gps, GpsDirectory.TagMeasureMode);
				(rowdata.GPSDOPDenominator, rowdata.GPSDOPNumerator) = this.GetRational(gps, GpsDirectory.TagDop);
				rowdata.GPSSpeedRef = this.GetString(gps, GpsDirectory.TagSpeedRef);
				(rowdata.GPSSpeedDenominator, rowdata.GPSSpeedNumerator) = this.GetRational(gps, GpsDirectory.TagSpeed);
				rowdata.GPSTrackRef = this.GetString(gps, GpsDirectory.TagTrackRef);
				(rowdata.GPSTrackDenominator, rowdata.GPSTrackNumerator) = this.GetRational(gps, GpsDirectory.TagTrack);
				rowdata.GPSImgDirectionRef = this.GetString(gps, GpsDirectory.TagImgDirectionRef);
				(rowdata.GPSImgDirectionDenominator, rowdata.GPSImgDirectionNumerator) = this.GetRational(gps, GpsDirectory.TagImgDirection);
				rowdata.GPSMapDatum = this.GetString(gps, GpsDirectory.TagMapDatum);
				rowdata.GPSDestLatitudeRef = this.GetString(gps, GpsDirectory.TagDestLatitudeRef);
				(rowdata.GPSDestLatitudeDoa, rowdata.GPSDestLatitudeMoa, rowdata.GPSDestLatitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagDestLatitude);
				rowdata.GPSDestLongitudeRef = this.GetString(gps, GpsDirectory.TagDestLongitudeRef);
				(rowdata.GPSDestLongitudeSoa, rowdata.GPSDestLongitudeMoa, rowdata.GPSDestLongitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagDestLongitude);
				rowdata.GPSDestBearingRef = this.GetString(gps, GpsDirectory.TagDestBearingRef);
				(rowdata.GPSDestBearingDenominator, rowdata.GPSDestBearingNumerator) = this.GetRational(gps, GpsDirectory.TagDestBearing);
				rowdata.GPSDestDistanceRef = this.GetString(gps, GpsDirectory.TagDestDistanceRef);
				(rowdata.GPSDestDistanceDenominator, rowdata.GPSDestDistanceNumerator) = this.GetRational(gps, GpsDirectory.TagDestDistance);
				rowdata.GPSProcessingMethod = this.GetBinary(gps, GpsDirectory.TagProcessingMethod);
				rowdata.GPSAreaInformation = this.GetBinary(gps, GpsDirectory.TagAreaInformation);
				rowdata.GPSDateStamp = this.GetString(gps, GpsDirectory.TagDateStamp);
				rowdata.GPSDifferential = this.GetShort(gps, GpsDirectory.TagDifferential);
			}
		}
	}
}
