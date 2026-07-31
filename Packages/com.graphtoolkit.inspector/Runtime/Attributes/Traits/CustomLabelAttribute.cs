namespace GraphToolkit.Inspector
{
	public class CustomLabelAttribute : PropertyTraitAttribute
	{
		public string Label { get; private set; }
		public string LabelSource { get; private set; }
		public bool AutoUpdate { get; private set; }

		public CustomLabelAttribute(string label) : base(ControlPhase, 0)
		{
			Label = label;
		}

		public CustomLabelAttribute(string labelSource, bool autoUpdate) : base(ControlPhase, 0)
		{
			LabelSource = labelSource;
			AutoUpdate = autoUpdate;
		}
	}
}