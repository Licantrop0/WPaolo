using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;

namespace Kiwi
{
    public class AnimationConfig
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public int FramesNum { get; set; }
        public Texture2D Texture { get; set; }
    }

    public class SpriteCollectionConfig
    {
        public Dictionary<string, Dictionary<string, AnimationConfig>> Sprites { get; set; }

        public SpriteCollectionConfig(string xmlFilePath, ContentManager contentManager)
        {
            var xmlDB = XDocument.Load(xmlFilePath);

            Sprites = (from spriteConfig in xmlDB.Descendants("Sprite")
                       let spriteName = spriteConfig.Attribute("Name").Value
                       let anim = (from animationConfig in spriteConfig.Descendants("Animation")
                                   select new AnimationConfig()
                                   {
                                       Name = animationConfig.Attribute("Name").Value,
                                       FramesNum = int.Parse(animationConfig.Attribute("FramesNum").Value),
                                       Texture = contentManager.Load<Texture2D>(animationConfig.Attribute("Path").Value)
                                   }).ToDictionary(key => key.Name)
                       select new
                       {
                           key = spriteName,
                           value = anim
                       }).ToDictionary(item => item.key, item => item.value);
        }
    }
}
