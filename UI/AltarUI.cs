using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Vitrium.UI
{
	public class AltarUI : UIState
	{
		public override void OnInitialize()
		{
			var panel = new UIPanel();
			panel.Width.Set(300, 0);
			panel.Height.Set(300, 0);
			Append(panel);

			var text = new UIText("TEST TEST")
			{
				VAlign = 0.5f,
				HAlign = 0.1f
			};
			panel.Append(text);
		}
	}
}
