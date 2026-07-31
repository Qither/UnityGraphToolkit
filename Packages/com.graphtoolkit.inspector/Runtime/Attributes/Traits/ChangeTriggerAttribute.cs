namespace GraphToolkit.Inspector
{
	public class ChangeTriggerAttribute : PropertyTraitAttribute
	{
		public string Method { get; private set; }

		public ChangeTriggerAttribute(string method) : base(FieldPhase, 1)
		{
			Method = method;
		}
	}
}