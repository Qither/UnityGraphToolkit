namespace PiRhoSoft.Utilities
{
	public class CustomTextAttribute : PropertyTraitAttribute
	{
		public string Label { get; private set; }
		public string LabelSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public CustomTextAttribute(string label) : base(ControlPhase, 0)
		{
			Label = label;
		}

		public CustomTextAttribute(string labelSource, bool autoUpdate) : base(ControlPhase, 0)
		{
			LabelSource = labelSource;
			AutoUpdate = autoUpdate;
		}
	}
}