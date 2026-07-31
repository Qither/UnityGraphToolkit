namespace GraphToolkit.Inspector
{
	public class ReadOnlyAttribute : PropertyTraitAttribute
	{
		public ReadOnlyAttribute() : base(PerContainerPhase, 10)
		{
		}
	}
}