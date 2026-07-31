namespace GraphToolkit.Inspector
{
	public class NoLabelAttribute : PropertyTraitAttribute
	{
		public NoLabelAttribute() : base(PerContainerPhase, 0)
		{
		}
	}
}