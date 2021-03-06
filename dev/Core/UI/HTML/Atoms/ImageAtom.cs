﻿using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Core.UI.HTML.Atoms
{
    public class ImageAtom : AAtom
    {
        public int Value = -1, ValueDown = -1, ValueOver = -1;

        public Image AssociatedImage
        {
            get;
            set;
        }

        private int m_overrideWidth = -1;
        public override int Width
        {
            set
            {
                m_overrideWidth = value;
            }
            get
            {
                if (m_overrideWidth != -1)
                    return m_overrideWidth + 1;
                return AssociatedImage.Texture.Width + 1;
            }
        }

        private int m_overrideHeight = -1;
        public override int Height
        {
            set
            {
                m_overrideHeight = value;
            }
            get
            {
                if (m_overrideHeight != -1)
                    return m_overrideHeight;
                return AssociatedImage.Texture.Height + 1;
            }
        }

        public ImageAtom(IUIResourceProvider provider, int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("<gImg {0}>", Value);
        }
    }
}
