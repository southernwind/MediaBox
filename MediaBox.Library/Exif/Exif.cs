using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ExifLib;

namespace SandBeige.MediaBox.Library.Exif {
	internal static class ExifReaderEx {
		public static T GetTagValue<T>(this ExifReader reader, ExifTags tag) {
			reader.GetTagValue(tag, out object result);
			try {
				return (T)result;
			} catch (InvalidCastException ex) {
				Console.WriteLine($"{tag}[{result}] {ex.Message}");
				return default;
			}
		}
	}

	public class Exif {
		private static readonly string[] _extensions = {
			// jpeg
			".jpeg",
			".jpg",
			".jpe",
			".jfif",
			".jfi",
			".jif",
			// tiff
			".tif",
			".tiff",
			// jpeg xr
			".hdp",
			".wdp",
			".jxr",
			// heif
			".heic",
			".heif",
		};
		public Exif(string filePath) {
			if (!_extensions.Contains(Path.GetExtension(filePath)?.ToLower())) {
				return;
			}
			try {
				using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
					var buff = new byte[12];
					fs.Read(buff, 0, buff.Length);
					var span = buff.AsSpan();
					if (span.Slice(0, 2).SequenceEqual(new byte[] { 0xff, 0xd8 })) {
						// Jpeg
						if (!span.Slice(6, 6).SequenceEqual(new byte[] { 0x45, 0x78, 0x69, 0x66, 0, 0 })) {
							// Exifなし
							return;
						}
					} else {
						// Jpeg以外
						// TODO : heicなど他の拡張子対応対応
						return;
					}
					fs.Seek(0, SeekOrigin.Begin);
					using (var reader = new ExifReader(fs, false)) {
						this.GPSVersionID = reader.GetTagValue<byte[]>(ExifTags.GPSVersionID);
						this.GPSLatitudeRef = reader.GetTagValue<string>(ExifTags.GPSLatitudeRef);
						this.GPSLatitude = reader.GetTagValue<double[]>(ExifTags.GPSLatitude);
						this.GPSLongitudeRef = reader.GetTagValue<string>(ExifTags.GPSLongitudeRef);
						this.GPSLongitude = reader.GetTagValue<double[]>(ExifTags.GPSLongitude);
						this.GPSAltitudeRef = reader.GetTagValue<byte?>(ExifTags.GPSAltitudeRef);
						this.GPSAltitude = reader.GetTagValue<double?>(ExifTags.GPSAltitude);
						this.GPSTimestamp = reader.GetTagValue<double[]>(ExifTags.GPSTimestamp);
						this.GPSSatellites = reader.GetTagValue<string>(ExifTags.GPSSatellites);
						this.GPSStatus = reader.GetTagValue<string>(ExifTags.GPSStatus);
						this.GPSMeasureMode = reader.GetTagValue<string>(ExifTags.GPSMeasureMode);
						this.GPSDOP = reader.GetTagValue<double?>(ExifTags.GPSDOP);
						this.GPSSpeedRef = reader.GetTagValue<string>(ExifTags.GPSSpeedRef);
						this.GPSSpeed = reader.GetTagValue<double?>(ExifTags.GPSSpeed);
						this.GPSTrackRef = reader.GetTagValue<string>(ExifTags.GPSTrackRef);
						this.GPSTrack = reader.GetTagValue<double?>(ExifTags.GPSTrack);
						this.GPSImgDirectionRef = reader.GetTagValue<string>(ExifTags.GPSImgDirectionRef);
						this.GPSImgDirection = reader.GetTagValue<double?>(ExifTags.GPSImgDirection);
						this.GPSMapDatum = reader.GetTagValue<string>(ExifTags.GPSMapDatum);
						this.GPSDestLatitudeRef = reader.GetTagValue<string>(ExifTags.GPSDestLatitudeRef);
						this.GPSDestLatitude = reader.GetTagValue<double[]>(ExifTags.GPSDestLatitude);
						this.GPSDestLongitudeRef = reader.GetTagValue<string>(ExifTags.GPSDestLongitudeRef);
						this.GPSDestLongitude = reader.GetTagValue<double[]>(ExifTags.GPSDestLongitude);
						this.GPSDestBearingRef = reader.GetTagValue<string>(ExifTags.GPSDestBearingRef);
						this.GPSDestBearing = reader.GetTagValue<double?>(ExifTags.GPSDestBearing);
						this.GPSDestDistanceRef = reader.GetTagValue<string>(ExifTags.GPSDestDistanceRef);
						this.GPSDestDistance = reader.GetTagValue<double?>(ExifTags.GPSDestDistance);
						this.GPSProcessingMethod = reader.GetTagValue<byte[]>(ExifTags.GPSProcessingMethod);
						this.GPSAreaInformation = reader.GetTagValue<byte[]>(ExifTags.GPSAreaInformation);
						this.GPSDateStamp = reader.GetTagValue<string>(ExifTags.GPSDateStamp);
						this.GPSDifferential = reader.GetTagValue<ushort?>(ExifTags.GPSDifferential);
						this.GPSHPositioningError = reader.GetTagValue<double?>(ExifTags.GPSHPositioningError);
						this.ImageWidth = reader.GetTagValue<uint?>(ExifTags.ImageWidth);
						this.ImageLength = reader.GetTagValue<uint?>(ExifTags.ImageLength);
						this.BitsPerSample = reader.GetTagValue<ushort[]>(ExifTags.BitsPerSample);
						this.Compression = reader.GetTagValue<ushort?>(ExifTags.Compression);
						this.PhotometricInterpretation = reader.GetTagValue<ushort?>(ExifTags.PhotometricInterpretation);
						this.ImageDescription = reader.GetTagValue<string>(ExifTags.ImageDescription);
						this.Make = reader.GetTagValue<string>(ExifTags.Make);
						this.Model = reader.GetTagValue<string>(ExifTags.Model);
						this.StripOffsets = reader.GetTagValue<uint?>(ExifTags.StripOffsets);
						this.Orientation = reader.GetTagValue<ushort?>(ExifTags.Orientation);
						this.SamplesPerPixel = reader.GetTagValue<ushort?>(ExifTags.SamplesPerPixel);
						this.RowsPerStrip = reader.GetTagValue<uint?>(ExifTags.RowsPerStrip);
						this.StripByteCounts = reader.GetTagValue<uint?>(ExifTags.StripByteCounts);
						this.XResolution = reader.GetTagValue<double?>(ExifTags.XResolution);
						this.YResolution = reader.GetTagValue<double?>(ExifTags.YResolution);
						this.PlanarConfiguration = reader.GetTagValue<ushort?>(ExifTags.PlanarConfiguration);
						this.ResolutionUnit = reader.GetTagValue<ushort?>(ExifTags.ResolutionUnit);
						this.TransferFunction = reader.GetTagValue<ushort[]>(ExifTags.TransferFunction);
						this.Software = reader.GetTagValue<string>(ExifTags.Software);
						this.DateTime = reader.GetTagValue<string>(ExifTags.DateTime);
						this.Artist = reader.GetTagValue<string>(ExifTags.Artist);
						this.WhitePoint = reader.GetTagValue<double?>(ExifTags.WhitePoint);
						this.PrimaryChromaticities = reader.GetTagValue<double?>(ExifTags.PrimaryChromaticities);
						this.JPEGInterchangeFormat = reader.GetTagValue<uint?>(ExifTags.JPEGInterchangeFormat);
						this.JPEGInterchangeFormatLength = reader.GetTagValue<uint?>(ExifTags.JPEGInterchangeFormatLength);
						this.YCbCrCoefficients = reader.GetTagValue<double?>(ExifTags.YCbCrCoefficients);
						this.YCbCrSubSampling = reader.GetTagValue<ushort?>(ExifTags.YCbCrSubSampling);
						this.YCbCrPositioning = reader.GetTagValue<ushort?>(ExifTags.YCbCrPositioning);
						this.ReferenceBlackWhite = reader.GetTagValue<double?>(ExifTags.ReferenceBlackWhite);
						this.Copyright = reader.GetTagValue<string>(ExifTags.Copyright);
						this.ExposureTime = reader.GetTagValue<double?>(ExifTags.ExposureTime);
						this.FNumber = reader.GetTagValue<double?>(ExifTags.FNumber);
						this.ExposureProgram = reader.GetTagValue<ushort?>(ExifTags.ExposureProgram);
						this.SpectralSensitivity = reader.GetTagValue<string>(ExifTags.SpectralSensitivity);
						this.PhotographicSensitivity = reader.GetTagValue<ushort?>(ExifTags.PhotographicSensitivity);
						this.OECF = reader.GetTagValue<byte[]>(ExifTags.OECF);
						this.SensitivityType = reader.GetTagValue<ushort?>(ExifTags.SensitivityType);
						this.StandardOutputSensitivity = reader.GetTagValue<uint?>(ExifTags.StandardOutputSensitivity);
						this.RecommendedExposureIndex = reader.GetTagValue<uint?>(ExifTags.RecommendedExposureIndex);
						this.ISOSpeed = reader.GetTagValue<uint?>(ExifTags.ISOSpeed);
						this.ISOSpeedLatitudeyyy = reader.GetTagValue<uint?>(ExifTags.ISOSpeedLatitudeyyy);
						this.ISOSpeedLatitudezzz = reader.GetTagValue<uint?>(ExifTags.ISOSpeedLatitudezzz);
						this.ExifVersion = reader.GetTagValue<byte[]>(ExifTags.ExifVersion);
						this.DateTimeOriginal = reader.GetTagValue<string>(ExifTags.DateTimeOriginal);
						this.DateTimeDigitized = reader.GetTagValue<string>(ExifTags.DateTimeDigitized);
						this.ComponentsConfiguration = reader.GetTagValue<byte[]>(ExifTags.ComponentsConfiguration);
						this.CompressedBitsPerPixel = reader.GetTagValue<double?>(ExifTags.CompressedBitsPerPixel);
						this.ShutterSpeedValue = reader.GetTagValue<double?>(ExifTags.ShutterSpeedValue);
						this.ApertureValue = reader.GetTagValue<double?>(ExifTags.ApertureValue);
						this.BrightnessValue = reader.GetTagValue<double?>(ExifTags.BrightnessValue);
						this.ExposureBiasValue = reader.GetTagValue<double?>(ExifTags.ExposureBiasValue);
						this.MaxApertureValue = reader.GetTagValue<double?>(ExifTags.MaxApertureValue);
						this.SubjectDistance = reader.GetTagValue<double?>(ExifTags.SubjectDistance);
						this.MeteringMode = reader.GetTagValue<ushort?>(ExifTags.MeteringMode);
						this.LightSource = reader.GetTagValue<ushort?>(ExifTags.LightSource);
						this.Flash = reader.GetTagValue<ushort?>(ExifTags.Flash);
						this.FocalLength = reader.GetTagValue<double?>(ExifTags.FocalLength);
						this.SubjectArea = reader.GetTagValue<ushort[]>(ExifTags.SubjectArea);
						this.MakerNote = reader.GetTagValue<byte[]>(ExifTags.MakerNote);
						this.UserComment = reader.GetTagValue<byte[]>(ExifTags.UserComment);
						this.SubsecTime = reader.GetTagValue<string>(ExifTags.SubsecTime);
						this.SubsecTimeOriginal = reader.GetTagValue<string>(ExifTags.SubsecTimeOriginal);
						this.SubsecTimeDigitized = reader.GetTagValue<string>(ExifTags.SubsecTimeDigitized);
						this.XPTitle = reader.GetTagValue<string>(ExifTags.XPTitle);
						this.XPComment = reader.GetTagValue<string>(ExifTags.XPComment);
						this.XPAuthor = reader.GetTagValue<string>(ExifTags.XPAuthor);
						this.XPKeywords = reader.GetTagValue<string>(ExifTags.XPKeywords);
						this.XPSubject = reader.GetTagValue<string>(ExifTags.XPSubject);
						this.FlashpixVersion = reader.GetTagValue<byte[]>(ExifTags.FlashpixVersion);
						this.ColorSpace = reader.GetTagValue<ushort?>(ExifTags.ColorSpace);
						this.PixelXDimension = reader.GetTagValue<uint?>(ExifTags.PixelXDimension);
						this.PixelYDimension = reader.GetTagValue<uint?>(ExifTags.PixelYDimension);
						this.RelatedSoundFile = reader.GetTagValue<string>(ExifTags.RelatedSoundFile);
						this.FlashEnergy = reader.GetTagValue<double?>(ExifTags.FlashEnergy);
						this.SpatialFrequencyResponse = reader.GetTagValue<byte[]>(ExifTags.SpatialFrequencyResponse);
						this.FocalPlaneXResolution = reader.GetTagValue<double?>(ExifTags.FocalPlaneXResolution);
						this.FocalPlaneYResolution = reader.GetTagValue<double?>(ExifTags.FocalPlaneYResolution);
						this.FocalPlaneResolutionUnit = reader.GetTagValue<ushort?>(ExifTags.FocalPlaneResolutionUnit);
						this.SubjectLocation = reader.GetTagValue<ushort[]>(ExifTags.SubjectLocation);
						this.ExposureIndex = reader.GetTagValue<double?>(ExifTags.ExposureIndex);
						this.SensingMethod = reader.GetTagValue<ushort?>(ExifTags.SensingMethod);
						this.FileSource = reader.GetTagValue<byte?>(ExifTags.FileSource);
						this.SceneType = reader.GetTagValue<byte?>(ExifTags.SceneType);
						this.CFAPattern = reader.GetTagValue<byte[]>(ExifTags.CFAPattern);
						this.CustomRendered = reader.GetTagValue<ushort?>(ExifTags.CustomRendered);
						this.ExposureMode = reader.GetTagValue<ushort?>(ExifTags.ExposureMode);
						this.WhiteBalance = reader.GetTagValue<ushort?>(ExifTags.WhiteBalance);
						this.DigitalZoomRatio = reader.GetTagValue<double?>(ExifTags.DigitalZoomRatio);
						this.FocalLengthIn35mmFilm = reader.GetTagValue<ushort?>(ExifTags.FocalLengthIn35mmFilm);
						this.SceneCaptureType = reader.GetTagValue<ushort?>(ExifTags.SceneCaptureType);
						this.GainControl = reader.GetTagValue<ushort?>(ExifTags.GainControl);
						this.Contrast = reader.GetTagValue<ushort?>(ExifTags.Contrast);
						this.Saturation = reader.GetTagValue<ushort?>(ExifTags.Saturation);
						this.Sharpness = reader.GetTagValue<ushort?>(ExifTags.Sharpness);
						this.DeviceSettingDescription = reader.GetTagValue<byte[]>(ExifTags.DeviceSettingDescription);
						this.SubjectDistanceRange = reader.GetTagValue<ushort?>(ExifTags.SubjectDistanceRange);
						this.ImageUniqueID = reader.GetTagValue<string>(ExifTags.ImageUniqueID);
						this.CameraOwnerName = reader.GetTagValue<string>(ExifTags.CameraOwnerName);
						this.BodySerialNumber = reader.GetTagValue<string>(ExifTags.BodySerialNumber);
						this.LensSpecification = reader.GetTagValue<double[]>(ExifTags.LensSpecification);
						this.LensMake = reader.GetTagValue<string>(ExifTags.LensMake);
						this.LensModel = reader.GetTagValue<string>(ExifTags.LensModel);
						this.LensSerialNumber = reader.GetTagValue<string>(ExifTags.LensSerialNumber);
					}
				}
			} catch (ExifLibException) {
				// TODO : 拡張子判定を行う
			}
		}

		public byte[] GPSVersionID { get; set; }
		public string GPSLatitudeRef { get; set; }
		public double[] GPSLatitude { get; set; }
		public string GPSLongitudeRef { get; set; }
		public double[] GPSLongitude { get; set; }
		public byte? GPSAltitudeRef { get; set; }
		public double? GPSAltitude { get; set; }
		public double[] GPSTimestamp { get; set; }
		public string GPSSatellites { get; set; }
		public string GPSStatus { get; set; }
		public string GPSMeasureMode { get; set; }
		public double? GPSDOP { get; set; }
		public string GPSSpeedRef { get; set; }
		public double? GPSSpeed { get; set; }
		public string GPSTrackRef { get; set; }
		public double? GPSTrack { get; set; }
		public string GPSImgDirectionRef { get; set; }
		public double? GPSImgDirection { get; set; }
		public string GPSMapDatum { get; set; }
		public string GPSDestLatitudeRef { get; set; }
		public double[] GPSDestLatitude { get; set; }
		public string GPSDestLongitudeRef { get; set; }
		public double[] GPSDestLongitude { get; set; }
		public string GPSDestBearingRef { get; set; }
		public double? GPSDestBearing { get; set; }
		public string GPSDestDistanceRef { get; set; }
		public double? GPSDestDistance { get; set; }
		public byte[] GPSProcessingMethod { get; set; }
		public byte[] GPSAreaInformation { get; set; }
		public string GPSDateStamp { get; set; }
		public ushort? GPSDifferential { get; set; }
		public double? GPSHPositioningError { get; set; }
		public uint? ImageWidth { get; set; }
		public uint? ImageLength { get; set; }
		public ushort[] BitsPerSample { get; set; }
		public ushort? Compression { get; set; }
		public ushort? PhotometricInterpretation { get; set; }
		public string ImageDescription { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public uint? StripOffsets { get; set; }
		public ushort? Orientation { get; set; }
		public ushort? SamplesPerPixel { get; set; }
		public uint? RowsPerStrip { get; set; }
		public uint? StripByteCounts { get; set; }
		public double? XResolution { get; set; }
		public double? YResolution { get; set; }
		public ushort? PlanarConfiguration { get; set; }
		public ushort? ResolutionUnit { get; set; }
		public ushort[] TransferFunction { get; set; }
		public string Software { get; set; }
		public string DateTime { get; set; }
		public string Artist { get; set; }
		public double? WhitePoint { get; set; }
		public double? PrimaryChromaticities { get; set; }
		public uint? JPEGInterchangeFormat { get; set; }
		public uint? JPEGInterchangeFormatLength { get; set; }
		public double? YCbCrCoefficients { get; set; }
		public ushort? YCbCrSubSampling { get; set; }
		public ushort? YCbCrPositioning { get; set; }
		public double? ReferenceBlackWhite { get; set; }
		public string Copyright { get; set; }
		public double? ExposureTime { get; set; }
		public double? FNumber { get; set; }
		public ushort? ExposureProgram { get; set; }
		public string SpectralSensitivity { get; set; }
		public ushort? PhotographicSensitivity { get; set; }
		public byte[] OECF { get; set; }
		public ushort? SensitivityType { get; set; }
		public uint? StandardOutputSensitivity { get; set; }
		public uint? RecommendedExposureIndex { get; set; }
		public uint? ISOSpeed { get; set; }
		public uint? ISOSpeedLatitudeyyy { get; set; }
		public uint? ISOSpeedLatitudezzz { get; set; }
		public byte[] ExifVersion { get; set; }
		public string DateTimeOriginal { get; set; }
		public string DateTimeDigitized { get; set; }
		public byte[] ComponentsConfiguration { get; set; }
		public double? CompressedBitsPerPixel { get; set; }
		public double? ShutterSpeedValue { get; set; }
		public double? ApertureValue { get; set; }
		public double? BrightnessValue { get; set; }
		public double? ExposureBiasValue { get; set; }
		public double? MaxApertureValue { get; set; }
		public double? SubjectDistance { get; set; }
		public ushort? MeteringMode { get; set; }
		public ushort? LightSource { get; set; }
		public ushort? Flash { get; set; }
		public double? FocalLength { get; set; }
		public ushort[] SubjectArea { get; set; }
		public byte[] MakerNote { get; set; }
		public byte[] UserComment { get; set; }
		public string SubsecTime { get; set; }
		public string SubsecTimeOriginal { get; set; }
		public string SubsecTimeDigitized { get; set; }
		public string XPTitle { get; set; }
		public string XPComment { get; set; }
		public string XPAuthor { get; set; }
		public string XPKeywords { get; set; }
		public string XPSubject { get; set; }
		public byte[] FlashpixVersion { get; set; }
		public ushort? ColorSpace { get; set; }
		public uint? PixelXDimension { get; set; }
		public uint? PixelYDimension { get; set; }
		public string RelatedSoundFile { get; set; }
		public double? FlashEnergy { get; set; }
		public byte[] SpatialFrequencyResponse { get; set; }
		public double? FocalPlaneXResolution { get; set; }
		public double? FocalPlaneYResolution { get; set; }
		public ushort? FocalPlaneResolutionUnit { get; set; }
		public ushort[] SubjectLocation { get; set; }
		public double? ExposureIndex { get; set; }
		public ushort? SensingMethod { get; set; }
		public byte? FileSource { get; set; }
		public byte? SceneType { get; set; }
		public byte[] CFAPattern { get; set; }
		public ushort? CustomRendered { get; set; }
		public ushort? ExposureMode { get; set; }
		public ushort? WhiteBalance { get; set; }
		public double? DigitalZoomRatio { get; set; }
		public ushort? FocalLengthIn35mmFilm { get; set; }
		public ushort? SceneCaptureType { get; set; }
		public ushort? GainControl { get; set; }
		public ushort? Contrast { get; set; }
		public ushort? Saturation { get; set; }
		public ushort? Sharpness { get; set; }
		public byte[] DeviceSettingDescription { get; set; }
		public ushort? SubjectDistanceRange { get; set; }
		public string ImageUniqueID { get; set; }
		public string CameraOwnerName { get; set; }
		public string BodySerialNumber { get; set; }
		public double[] LensSpecification { get; set; }
		public string LensMake { get; set; }
		public string LensModel { get; set; }
		public string LensSerialNumber { get; set; }

		public IEnumerable<TitleValuePair> ToTitleValuePair() {
			string ConvertGpsFunc(double[] co,string re){
				var redic = new Dictionary<string, string> {
					{ "N","北緯" },
					{ "S","南緯" },
					{ "W","西経" },
					{ "E","東経" }
				};
				if (co == null || re == null || co.Length != 3 || !redic.ContainsKey(re)) {
					return null;
				}
				return $"{redic[re]}{co[0]}度{co[1]}分{co[2]}秒";
			}

			string ConvertSizeFunc(double? x,double? y,ushort? unit) {
				if (x == null || y == null || unit == null) {
					return null;
				}
				var units = new Dictionary<ushort?, string> {
					{ 1,"" },
					{ 2,"inches" },
					{ 3,"cm" }
				};
				return $"{x} × {y} {units[unit]}";
			}

			return new Dictionary<string, string>(){
				{ "画像タイトル", this.ImageDescription },
				{ "メーカー", this.Make },
				{ "モデル", this.Model },
				{ "画像の方向", this.Orientation?.ToString() },
				{ "サイズ", ConvertSizeFunc(this.XResolution,this.YResolution,this.ResolutionUnit) },
				{ "ファイル変更日時", this.DateTime },
				{ "露出時間", this.ExposureTime?.ToString() },
				{ "色空間情報", this.ColorSpace?.ToString() },
				{ "露出モード", this.ExposureMode?.ToString() },
				{ "ホワイトバランス", this.WhiteBalance?.ToString() },
				{ "緯度", ConvertGpsFunc(this.GPSLatitude,this.GPSLatitudeRef) },
				{ "経度", ConvertGpsFunc(this.GPSLongitude,this.GPSLongitudeRef) },
				{ "高度", this.GPSAltitude?.ToString() }

			}.Where(x => x.Value != null)
			.Select(x => new TitleValuePair(x.Key, x.Value));
		}
	}

	public class TitleValuePair {
		public TitleValuePair(string title,string value) {
			this.Title = title;
			this.Value = value;
		}

		public string Title {
			get;
			set;
		}

		public string Value {
			get;
			set;
		}
	}
}
