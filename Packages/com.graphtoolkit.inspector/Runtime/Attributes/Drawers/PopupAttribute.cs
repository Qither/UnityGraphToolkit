using System.Collections.Generic;
using System.Linq;

namespace GraphToolkit.Inspector
{
	public class PopupValues<T>
	{
		public List<T> Values;
		public List<string> Options;
	}

	public class PopupAttribute : PropertyTraitAttribute
	{
		public List<int> IntValues { get; private set; }
		public List<float> FloatValues { get; private set; }
		public List<string> StringValues { get; private set; }
		public List<string> Options { get; private set; }
		public string ValuesSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public PopupAttribute(string[] values, string[] options = null) : base(ControlPhase, 10)
		{
			StringValues = values.ToList();
			Options = options?.ToList();
		}

		public PopupAttribute(int[] values, string[] options = null) : base(ControlPhase, 10)
		{
			IntValues = values.ToList();
			Options = options?.ToList();
		}

		public PopupAttribute(float[] values, string[] options = null) : base(ControlPhase, 10)
		{
			FloatValues = values.ToList();
			Options = options?.ToList();
		}

		public PopupAttribute(string valuesSource, bool autoUpdate = true) : base(ControlPhase, 10)
		{
			ValuesSource = valuesSource;
			AutoUpdate = autoUpdate;
		}
	}
}