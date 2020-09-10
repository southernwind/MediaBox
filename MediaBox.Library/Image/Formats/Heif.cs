using System.Collections.Generic;
using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Heif;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Heifメタデータ取得クラス
	/// </summary>
	public class Heif : ImageBase {
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
		public override Rational[]? Latitude {
			get;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public override Rational[]? Longitude {
			get;
		}

		/// <summary>
		/// 高度
		/// </summary>
		public override Rational? Altitude {
			get;
		}

		/// <summary>
		/// 緯度方向(N/S)
		/// </summary>
		public override string? LatitudeRef {
			get;
		}

		/// <summary>
		/// 経度方向(E/W)
		/// </summary>
		public override string? LongitudeRef {
			get;
		}

		/// <summary>
		/// 高度方向(0/1)
		/// </summary>
		public override byte? AltitudeRef {
			get;
		}

		/// <summary>
		/// 画像の方向
		/// </summary>
		public override int? Orientation {
			get;
		}


		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>

		internal Heif(Stream stream) : base(stream) {
			this._reader = HeifMetadataReader.ReadMetadata(stream);
			var d = this._reader.OfType<HeicImagePropertiesDirectory>().First();
			var gps = this._reader.FirstOrDefault(x => x is GpsDirectory);
			var ifd0 = this._reader.FirstOrDefault(x => x is ExifDirectoryBase);
			this.Width = d.GetUInt16(HeicImagePropertiesDirectory.TagImageWidth);
			this.Height = d.GetUInt16(HeicImagePropertiesDirectory.TagImageHeight);

			if (ifd0 != null && ifd0.TryGetUInt16(ExifDirectoryBase.TagOrientation, out var orientation)) {
				this.Orientation = orientation;
			}

			if (gps != null) {
				this.Latitude = gps.GetRationalArray(GpsDirectory.TagLatitude);
				this.Longitude = gps.GetRationalArray(GpsDirectory.TagLongitude);
				this.LatitudeRef = gps.GetString(GpsDirectory.TagLatitudeRef);
				this.LongitudeRef = gps.GetString(GpsDirectory.TagLongitudeRef);
				if (gps.TryGetRational(GpsDirectory.TagAltitude, out var r)) {
					this.Altitude = r;
				}
				if (gps.TryGetByte(GpsDirectory.TagAltitudeRef, out var b)) {
					this.AltitudeRef = b;
				}
			}
		}

		public void UpdateRowData(DataBase.Tables.Metadata.Heif rowData) {
			var gps = this._reader.FirstOrDefault(x => x is GpsDirectory);
			var ifd0 = this._reader.FirstOrDefault(x => x is ExifDirectoryBase);
			var subIfd = this._reader.FirstOrDefault(x => x is ExifSubIfdDirectory);

			rowData.ImageDescription = this.GetString(ifd0, ExifDirectoryBase.TagImageDescription);
			rowData.ImageDescription = this.GetString(ifd0, ExifDirectoryBase.TagImageDescription);
			rowData.Make = this.GetString(ifd0, ExifDirectoryBase.TagMake);
			rowData.Model = this.GetString(ifd0, ExifDirectoryBase.TagModel);
			rowData.Orientation = this.GetShort(ifd0, ExifDirectoryBase.TagOrientation);
			rowData.XResolution = this.GetInt(ifd0, ExifDirectoryBase.TagXResolution);
			rowData.YResolution = this.GetInt(ifd0, ExifDirectoryBase.TagYResolution);
			rowData.ResolutionUnit = this.GetShort(ifd0, ExifDirectoryBase.TagResolutionUnit);
			rowData.TransferFunction = this.GetBinary(ifd0, ExifDirectoryBase.TagTransferFunction);
			rowData.Software = this.GetString(ifd0, ExifDirectoryBase.TagSoftware);
			rowData.DateTime = this.GetString(ifd0, ExifDirectoryBase.TagDateTime);
			rowData.Artist = this.GetString(ifd0, ExifDirectoryBase.TagArtist);
			rowData.WhitePoint = ifd0?.GetRationalArray(ExifDirectoryBase.TagWhitePoint)?.Select(x => x.ToByte()).ToArray();
			rowData.PrimaryChromaticities = this.GetBinary(ifd0, ExifDirectoryBase.TagPrimaryChromaticities);
			rowData.YCbCrCoefficients = this.GetBinary(ifd0, ExifDirectoryBase.TagYCbCrCoefficients);
			rowData.YCbCrPositioning = this.GetShort(ifd0, ExifDirectoryBase.TagYCbCrPositioning);
			rowData.ReferenceBlackWhite = this.GetBinary(ifd0, ExifDirectoryBase.TagReferenceBlackWhite);
			rowData.Copyright = this.GetString(ifd0, ExifDirectoryBase.TagCopyright);


			(rowData.ExposureTimeDenominator, rowData.ExposureTimeNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagExposureTime);
			(rowData.FNumberDenominator, rowData.FNumberNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagFNumber);
			rowData.ExposureProgram = this.GetShort(subIfd, ExifDirectoryBase.TagExposureProgram);
			rowData.SpectralSensitivity = this.GetString(subIfd, ExifDirectoryBase.TagSpectralSensitivity);
			rowData.OECF = this.GetBinary(subIfd, ExifDirectoryBase.TagOptoElectricConversionFunction);
			rowData.SensitivityType = this.GetShort(subIfd, ExifDirectoryBase.TagSensitivityType);
			rowData.StandardOutputSensitivity = this.GetInt(subIfd, ExifDirectoryBase.TagStandardOutputSensitivity);
			rowData.RecommendedExposureIndex = this.GetInt(subIfd, ExifDirectoryBase.TagRecommendedExposureIndex);
			rowData.ExifVersion = this.GetBinary(subIfd, ExifDirectoryBase.TagExifVersion);
			rowData.DateTimeOriginal = this.GetString(subIfd, ExifDirectoryBase.TagDateTimeOriginal);
			rowData.DateTimeDigitized = this.GetString(subIfd, ExifDirectoryBase.TagDateTimeDigitized);
			rowData.ComponentsConfiguration = this.GetBinary(subIfd, ExifDirectoryBase.TagComponentsConfiguration);
			(rowData.CompressedBitsPerPixelDenominator, rowData.CompressedBitsPerPixelNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagCompressedAverageBitsPerPixel);
			(rowData.ShutterSpeedValueDenominator, rowData.ShutterSpeedValueNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagShutterSpeed);
			(rowData.ApertureValueDenominator, rowData.ApertureValueNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagAperture);
			(rowData.BrightnessValueDenominator, rowData.BrightnessValueNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagBrightnessValue);
			(rowData.ExposureBiasValueDenominator, rowData.ExposureBiasValueNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagExposureBias);
			(rowData.MaxApertureValueDenominator, rowData.MaxApertureValueNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagMaxAperture);
			(rowData.SubjectDistanceDenominator, rowData.SubjectDistanceNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagSubjectDistance);
			rowData.MeteringMode = this.GetShort(subIfd, ExifDirectoryBase.TagMeteringMode);
			rowData.Flash = this.GetShort(subIfd, ExifDirectoryBase.TagFlash);
			(rowData.FocalLengthDenominator, rowData.FocalLengthNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagFocalLength);
			rowData.MakerNote = this.GetBinary(subIfd, ExifDirectoryBase.TagMakernote);
			rowData.UserComment = this.GetBinary(subIfd, ExifDirectoryBase.TagUserComment);
			rowData.SubSecTime = this.GetString(subIfd, ExifDirectoryBase.TagSubsecondTime);
			rowData.SubSecTimeOriginal = this.GetString(subIfd, ExifDirectoryBase.TagSubsecondTimeOriginal);
			rowData.SubSecTimeDigitized = this.GetString(subIfd, ExifDirectoryBase.TagSubsecondTimeDigitized);
			rowData.FlashpixVersion = this.GetBinary(subIfd, ExifDirectoryBase.TagFlashpixVersion);
			rowData.ColorSpace = this.GetShort(subIfd, ExifDirectoryBase.TagColorSpace);
			rowData.RelatedSoundFile = this.GetString(subIfd, ExifDirectoryBase.TagRelatedSoundFile);
			(rowData.FlashEnergyDenominator, rowData.FlashEnergyNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagFlashEnergy);
			(rowData.FocalPlaneXResolutionDenominator, rowData.FocalPlaneXResolutionNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagFocalPlaneXResolution);
			(rowData.FocalPlaneYResolutionDenominator, rowData.FocalPlaneYResolutionNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagFocalPlaneYResolution);
			rowData.FocalPlaneResolutionUnit = this.GetShort(subIfd, ExifDirectoryBase.TagFocalPlaneResolutionUnit);
			rowData.SubjectLocation = this.GetBinary(subIfd, ExifDirectoryBase.TagSubjectLocation);
			(rowData.ExposureIndexDenominator, rowData.ExposureIndexNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagExposureIndex);
			rowData.SensingMethod = this.GetShort(subIfd, ExifDirectoryBase.TagSensingMethod);
			rowData.FileSource = this.GetInt(subIfd, ExifDirectoryBase.TagFileSource);
			rowData.SceneType = this.GetInt(subIfd, ExifDirectoryBase.TagSceneType);
			rowData.CFAPattern = this.GetBinary(subIfd, ExifDirectoryBase.TagCfaPattern);
			rowData.CustomRendered = this.GetShort(subIfd, ExifDirectoryBase.TagCustomRendered);
			rowData.ExposureMode = this.GetShort(subIfd, ExifDirectoryBase.TagExposureMode);
			rowData.WhiteBalance = this.GetShort(subIfd, ExifDirectoryBase.TagWhiteBalanceMode);
			(rowData.DigitalZoomRatioDenominator, rowData.DigitalZoomRatioNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagDigitalZoomRatio);
			rowData.FocalLengthIn35mmFilm = this.GetShort(subIfd, ExifDirectoryBase.Tag35MMFilmEquivFocalLength);
			rowData.SceneCaptureType = this.GetShort(subIfd, ExifDirectoryBase.TagSceneCaptureType);
			rowData.GainControl = this.GetShort(subIfd, ExifDirectoryBase.TagGainControl);
			rowData.Contrast = this.GetShort(subIfd, ExifDirectoryBase.TagContrast);
			rowData.Saturation = this.GetShort(subIfd, ExifDirectoryBase.TagSaturation);
			rowData.Sharpness = this.GetShort(subIfd, ExifDirectoryBase.TagSharpness);
			rowData.DeviceSettingDescription = this.GetBinary(subIfd, ExifDirectoryBase.TagDeviceSettingDescription);
			rowData.SubjectDistanceRange = this.GetShort(subIfd, ExifDirectoryBase.TagSubjectDistanceRange);
			rowData.ImageUniqueID = this.GetString(subIfd, ExifDirectoryBase.TagImageUniqueId);
			rowData.CameraOwnerName = this.GetString(subIfd, ExifDirectoryBase.TagCameraOwnerName);
			rowData.BodySerialNumber = this.GetString(subIfd, ExifDirectoryBase.TagBodySerialNumber);
			rowData.LensSpecification = this.GetBinary(subIfd, ExifDirectoryBase.TagLensSpecification);
			rowData.LensMake = this.GetString(subIfd, ExifDirectoryBase.TagLensMake);
			rowData.LensModel = this.GetString(subIfd, ExifDirectoryBase.TagLensModel);
			rowData.LensSerialNumber = this.GetString(subIfd, ExifDirectoryBase.TagLensSerialNumber);
			(rowData.GammaDenominator, rowData.GammaNumerator) = this.GetRational(subIfd, ExifDirectoryBase.TagGamma);

			rowData.GPSVersionID = this.GetBinary(gps, GpsDirectory.TagVersionId);
			rowData.GPSLatitudeRef = this.GetString(gps, GpsDirectory.TagLatitudeRef);
			(rowData.GPSLatitudeDoa, rowData.GPSLatitudeMoa, rowData.GPSLatitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagLatitude);
			rowData.GPSLongitudeRef = this.GetString(gps, GpsDirectory.TagLongitudeRef);
			(rowData.GPSLongitudeDoa, rowData.GPSLongitudeMoa, rowData.GPSLongitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagLongitude);
			rowData.GPSAltitudeRef = this.GetInt(gps, GpsDirectory.TagAltitudeRef);
			(rowData.GPSAltitudeDenominator, rowData.GPSAltitudeNumerator) = this.GetRational(gps, GpsDirectory.TagAltitude);
			(rowData.GPSTimeStampHour, rowData.GPSTimeStampMinutes, rowData.GPSTimeStampSeconds) = this.Get3Rational(gps, GpsDirectory.TagTimeStamp);
			rowData.GPSSatellites = this.GetString(gps, GpsDirectory.TagSatellites);
			rowData.GPSStatus = this.GetString(gps, GpsDirectory.TagStatus);
			rowData.GPSMeasureMode = this.GetString(gps, GpsDirectory.TagMeasureMode);
			(rowData.GPSDOPDenominator, rowData.GPSDOPNumerator) = this.GetRational(gps, GpsDirectory.TagDop);
			rowData.GPSSpeedRef = this.GetString(gps, GpsDirectory.TagSpeedRef);
			(rowData.GPSSpeedDenominator, rowData.GPSSpeedNumerator) = this.GetRational(gps, GpsDirectory.TagSpeed);
			rowData.GPSTrackRef = this.GetString(gps, GpsDirectory.TagTrackRef);
			(rowData.GPSTrackDenominator, rowData.GPSTrackNumerator) = this.GetRational(gps, GpsDirectory.TagTrack);
			rowData.GPSImgDirectionRef = this.GetString(gps, GpsDirectory.TagImgDirectionRef);
			(rowData.GPSImgDirectionDenominator, rowData.GPSImgDirectionNumerator) = this.GetRational(gps, GpsDirectory.TagImgDirection);
			rowData.GPSMapDatum = this.GetString(gps, GpsDirectory.TagMapDatum);
			rowData.GPSDestLatitudeRef = this.GetString(gps, GpsDirectory.TagDestLatitudeRef);
			(rowData.GPSDestLatitudeDoa, rowData.GPSDestLatitudeMoa, rowData.GPSDestLatitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagDestLatitude);
			rowData.GPSDestLongitudeRef = this.GetString(gps, GpsDirectory.TagDestLongitudeRef);
			(rowData.GPSDestLongitudeDoa, rowData.GPSDestLongitudeMoa, rowData.GPSDestLongitudeSoa) = this.Get3Rational(gps, GpsDirectory.TagDestLongitude);
			rowData.GPSDestBearingRef = this.GetString(gps, GpsDirectory.TagDestBearingRef);
			(rowData.GPSDestBearingDenominator, rowData.GPSDestBearingNumerator) = this.GetRational(gps, GpsDirectory.TagDestBearing);
			rowData.GPSDestDistanceRef = this.GetString(gps, GpsDirectory.TagDestDistanceRef);
			(rowData.GPSDestDistanceDenominator, rowData.GPSDestDistanceNumerator) = this.GetRational(gps, GpsDirectory.TagDestDistance);
			rowData.GPSProcessingMethod = this.GetBinary(gps, GpsDirectory.TagProcessingMethod);
			rowData.GPSAreaInformation = this.GetBinary(gps, GpsDirectory.TagAreaInformation);
			rowData.GPSDateStamp = this.GetString(gps, GpsDirectory.TagDateStamp);
			rowData.GPSDifferential = this.GetShort(gps, GpsDirectory.TagDifferential);

		}
	}
}
