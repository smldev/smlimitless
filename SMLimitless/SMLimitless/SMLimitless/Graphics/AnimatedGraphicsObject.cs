using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Graphics
{
    public class AnimatedGraphicsObject
    {
        private List<Texture2D> textures;
        private string metadata;

        private int frameTime; // the number of rendered frames to display each texture
        private int currentTextureIndex;
        private int elapsedFrames; // how many rendered frames have been drawn since we've lasted changed the current texture

        private bool isLoaded;
        private bool isContentLoaded;

        public AnimatedGraphicsObject()
        {
        }

        public void LoadFromMetadata(string Metadata)
        {
            if (!isLoaded)
            {
                this.metadata = Metadata;
                var split = metadata.Split('>');
                if (split[0] == "anim-single")
                {
                    // Metadata format: anim-single>“//filepath/image.png”,frameLength,frameTime
                }
                else if (split[0] == "anim-spritesheet")
                {
                    // Metadata format: anim-spritesheet>“//filepath/image.png”,width,height,tileNumber1,tileNumber2,tileNumber3,...
                    throw new NotImplementedException();
                }
                else if (split[0] == "anim-spritesheet_r")
                {
                    // Metadata format: anim-spritesheet_r>”//filepath/image.png”,[x,y,width,height],[x,y,width,height],...,frameTime
                    throw new NotImplementedException();
                }
            }
        }
    }
}
