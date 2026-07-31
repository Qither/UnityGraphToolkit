using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPBehaveEditor
{
    [NodeCustomEditor(typeof(ANPNodeBase))]
    public class NPNodeView : BaseNodeView
    {
        public override void Enable()
        {
            var node = nodeTarget as ANPNodeBase;

            if(node == null)
                return;
            Texture2D iconTexture = Resources.Load<Texture2D>(node.Icon);
            Box box = new Box()
            {
                style =
                {
                    backgroundColor = Color.clear,
                    alignItems      = Align.Center,
                    justifyContent  = Justify.FlexStart,
                    minHeight       = 64,
                    minWidth        = 64,
                    maxHeight       = 256,
                    maxWidth        = 256,
                    paddingTop      = 5,
                    paddingBottom   = 5,
                    paddingLeft     = 5,
                    paddingRight    = 5,
                    borderTopWidth = 2,
                    borderBottomWidth = 2,
                    borderLeftWidth = 2,
                    borderRightWidth = 2,
                    borderTopColor = Color.gray,
                    borderBottomColor = Color.gray,
                    borderLeftColor = Color.gray,
                    borderRightColor = Color.gray,
                    borderTopLeftRadius = 5,
                    borderTopRightRadius = 5,
                    borderBottomLeftRadius = 5,
                    borderBottomRightRadius = 5,
                }
            };
            Image icon = new Image()
            {
                image = iconTexture
            };
            Label label = new Label(node.name)
            {
                style =
                {
                    alignItems      = Align.Center,
                    justifyContent  = Justify.Center,
                    minHeight       = 64,
                    minWidth        = 64,
                    maxHeight       = 256,
                    maxWidth        = 256,
                }
            };
            box.Add(icon);
            box.Add(new Label(node.name));
            controlsContainer.Add(box);
            mainContainer.Remove(mainContainer.ElementAt(0));
        }

        public override void Disable()
        {
        }
    }
}
