namespace GraphToolkit.Inspector
{
	public class FrameAttribute : PropertyTraitAttribute
	{
		public bool IsCollapsable = true;

		public FrameAttribute() : base(ControlPhase, 0)
		{
		}
	}
}
