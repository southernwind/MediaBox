namespace SandBeige.MediaBox.Composition.Interfaces.Services.Objects {

	public record MediaFileUpdateNotificationArgs<T>(long[] TargetMediaFileId, T Detail);
	public record AddTagNotificationDetail(string TagName);
	public record RemoveTagNotificationDetail(string TagName);
	public record SetRateNotificationDetail(int Rate);
}
