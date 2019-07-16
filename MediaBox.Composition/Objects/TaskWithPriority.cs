namespace SandBeige.MediaBox.Composition.Objects {


	public class TaskWithPriority<T> {
		public int Priority {
			get;
			set;
		}

		public bool Completed {
			get;
			set;
		}

		public T Object {
			get;
		}

		public TaskWithPriority(T obj) {
			this.Object = obj;
		}
	}
}